# Machine Vision Inspection Screw System

This repository also contains a small, zero-subscription **kids clip maker**. It
creates an original, vertical animal-learning video for children aged 3–5. All
artwork is drawn locally; no stock media or paid API is required.

## Create a kids clip

Prerequisites:

- Python 3.10+
- `ffmpeg` on `PATH`
- an optional local `espeak-ng`/`espeak` installation for narration

```bash
python -m pip install -r requirements.txt
python -m kids_clip_maker --output output/animals-th.mp4
```

The default output is a 1080×1920 H.264 MP4. The generator uses a local speech
engine when one is available and otherwise creates a silent video, so video
generation never depends on a network service. Use `--voice silent` to request
that behaviour explicitly, or `--voice espeak` to fail clearly when the local
speech engine is unavailable.

Content is data-driven. Edit `kids_clip_maker/content/animals_th.json`, or pass
another JSON file with `--content`, to change the title, question, colours, and
animal facts. Run `python -m kids_clip_maker --help` for all options.

```bash
python -m unittest discover -s tests -v
```
