from __future__ import annotations

import argparse
from pathlib import Path

from .content import load_lesson
from .generator import generate

DEFAULT_CONTENT = Path(__file__).parent / "content" / "animals_th.json"


def parser() -> argparse.ArgumentParser:
    result = argparse.ArgumentParser(description="Create an offline-first educational kids clip")
    result.add_argument("--content", type=Path, default=DEFAULT_CONTENT, help="lesson JSON file")
    result.add_argument("--output", type=Path, default=Path("output/animals-th.mp4"), help="output MP4")
    result.add_argument("--font", type=Path, help="Thai TrueType/OpenType font")
    result.add_argument("--voice", choices=("auto", "espeak", "silent"), default="auto")
    return result


def main(argv: list[str] | None = None) -> int:
    args = parser().parse_args(argv)
    lesson = load_lesson(args.content)
    generate(lesson, args.output, args.voice, args.font)
    print(f"Created {args.output}")
    return 0
