from __future__ import annotations

import shutil
import subprocess
import wave
from pathlib import Path

RATE = 22_050


def make_silence(path: Path, seconds: float) -> None:
    frames = int(RATE * seconds)
    with wave.open(str(path), "wb") as output:
        output.setparams((1, 2, RATE, frames, "NONE", "not compressed"))
        output.writeframes(b"\x00\x00" * frames)


def wav_duration(path: Path) -> float:
    with wave.open(str(path), "rb") as source:
        return source.getnframes() / source.getframerate()


def narration(text: str, path: Path, voice: str) -> float:
    executable = shutil.which("espeak-ng") or shutil.which("espeak")
    if voice == "espeak" and not executable:
        raise RuntimeError("--voice espeak requested, but espeak-ng/espeak was not found")
    if voice != "silent" and executable:
        subprocess.run(
            [executable, "-v", "th", "-s", "135", "-w", str(path), text],
            check=True,
            capture_output=True,
        )
        return max(2.8, wav_duration(path) + 0.7)
    duration = max(2.8, min(5.0, len(text) * 0.12))
    make_silence(path, duration)
    return duration
