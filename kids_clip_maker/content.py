from __future__ import annotations

import json
from dataclasses import dataclass
from pathlib import Path


@dataclass(frozen=True)
class Animal:
    kind: str
    name: str
    line: str
    color: str


@dataclass(frozen=True)
class Lesson:
    title: str
    question: str
    answer: str
    animals: tuple[Animal, ...]


def load_lesson(path: Path) -> Lesson:
    data = json.loads(path.read_text(encoding="utf-8"))
    required = ("title", "question", "answer", "animals")
    missing = [field for field in required if not data.get(field)]
    if missing:
        raise ValueError(f"Missing content fields: {', '.join(missing)}")
    if not isinstance(data["animals"], list):
        raise ValueError("animals must be a list")

    animals = []
    for index, item in enumerate(data["animals"], start=1):
        try:
            animal = Animal(**{key: item[key] for key in ("kind", "name", "line", "color")})
        except (KeyError, TypeError) as exc:
            raise ValueError(f"Invalid animal at position {index}") from exc
        if animal.kind not in {"cat", "dog", "elephant"}:
            raise ValueError(f"Unsupported animal kind: {animal.kind}")
        animals.append(animal)
    if not animals:
        raise ValueError("At least one animal is required")
    return Lesson(data["title"], data["question"], data["answer"], tuple(animals))
