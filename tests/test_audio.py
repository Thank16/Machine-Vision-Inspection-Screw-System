import tempfile
import unittest
from pathlib import Path

from kids_clip_maker.audio import make_silence, wav_duration


class AudioTests(unittest.TestCase):
    def test_silence_has_requested_duration(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "silence.wav"
            make_silence(path, 1.25)
            self.assertAlmostEqual(1.25, wav_duration(path), places=2)


if __name__ == "__main__":
    unittest.main()
