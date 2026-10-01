from __future__ import annotations

import shutil
import subprocess
import tempfile
import wave
from pathlib import Path

from .art import find_font, render_scene
from .audio import narration
from .content import Lesson


def _join_wavs(inputs: list[Path], output: Path) -> None:
    with wave.open(str(inputs[0]), "rb") as first:
        params = first.getparams()
    with wave.open(str(output), "wb") as target:
        target.setparams(params)
        for path in inputs:
            with wave.open(str(path), "rb") as source:
                if source.getparams()[:4] != params[:4]:
                    raise RuntimeError("Narration WAV formats do not match")
                target.writeframes(source.readframes(source.getnframes()))


def generate(lesson: Lesson, output: Path, voice: str = "auto", font: Path | None = None) -> None:
    ffmpeg = shutil.which("ffmpeg")
    if not ffmpeg:
        raise RuntimeError("ffmpeg was not found on PATH")
    font_path = find_font(font)
    output.parent.mkdir(parents=True, exist_ok=True)

    scenes = [(lesson.title, lesson.title, "#F0B84B", None)]
    scenes.extend((animal.name, animal.line, animal.color, animal.kind) for animal in lesson.animals)
    scenes.extend([
        (lesson.question, lesson.question, "#F0B84B", None),
        (lesson.answer, lesson.answer, "#86A8D9", "elephant"),
    ])

    with tempfile.TemporaryDirectory(prefix="kids-clip-") as temporary:
        work = Path(temporary)
        durations: list[float] = []
        wavs: list[Path] = []
        images: list[Path] = []
        for index, (heading, caption, color, kind) in enumerate(scenes):
            image_path = work / f"scene-{index:02d}.png"
            wav_path = work / f"scene-{index:02d}.wav"
            render_scene(image_path, heading, caption, color, kind, font_path)
            durations.append(narration(caption, wav_path, voice))
            images.append(image_path)
            wavs.append(wav_path)

        audio_path = work / "narration.wav"
        _join_wavs(wavs, audio_path)
        concat_path = work / "scenes.txt"
        lines = []
        for image_path, duration in zip(images, durations):
            lines.extend([f"file '{image_path.as_posix()}'", f"duration {duration:.3f}"])
        lines.append(f"file '{images[-1].as_posix()}'")
        concat_path.write_text("\n".join(lines) + "\n", encoding="utf-8")

        subprocess.run(
            [
                ffmpeg, "-hide_banner", "-loglevel", "error", "-y",
                "-f", "concat", "-safe", "0", "-i", str(concat_path),
                "-i", str(audio_path), "-c:v", "libx264", "-pix_fmt", "yuv420p",
                "-r", "30", "-c:a", "aac", "-b:a", "128k", "-shortest",
                "-movflags", "+faststart", str(output),
            ],
            check=True,
        )
