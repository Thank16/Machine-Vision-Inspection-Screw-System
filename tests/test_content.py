import json
import tempfile
import unittest
from pathlib import Path

from kids_clip_maker.content import load_lesson


class LessonTests(unittest.TestCase):
    def write_content(self, data: dict) -> Path:
        temporary = tempfile.NamedTemporaryFile(mode="w", suffix=".json", delete=False, encoding="utf-8")
        json.dump(data, temporary, ensure_ascii=False)
        temporary.close()
        self.addCleanup(Path(temporary.name).unlink, missing_ok=True)
        return Path(temporary.name)

    def test_loads_valid_lesson(self):
        path = self.write_content({
            "title": "สัตว์", "question": "อะไร", "answer": "ช้าง",
            "animals": [{"kind": "cat", "name": "แมว", "line": "เหมียว", "color": "#fff"}],
        })
        lesson = load_lesson(path)
        self.assertEqual("แมว", lesson.animals[0].name)

    def test_rejects_unsupported_animal(self):
        path = self.write_content({
            "title": "สัตว์", "question": "อะไร", "answer": "ช้าง",
            "animals": [{"kind": "snake", "name": "งู", "line": "ฟ่อ", "color": "#fff"}],
        })
        with self.assertRaisesRegex(ValueError, "Unsupported animal kind"):
            load_lesson(path)


if __name__ == "__main__":
    unittest.main()
