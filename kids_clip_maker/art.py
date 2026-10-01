from __future__ import annotations

from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

SIZE = (1080, 1920)


def find_font(preferred: Path | None = None) -> Path:
    candidates = [
        preferred,
        Path("/usr/share/fonts/truetype/noto/NotoSansThai-Bold.ttf"),
        Path("/usr/share/fonts/opentype/noto/NotoSansThai-Bold.ttf"),
        Path("/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf"),
        Path("C:/Windows/Fonts/tahoma.ttf"),
    ]
    for candidate in candidates:
        if candidate and candidate.is_file():
            return candidate
    raise RuntimeError("No usable font found; pass a Thai font with --font")


def _centered(draw: ImageDraw.ImageDraw, xy: tuple[int, int], text: str, font, fill: str) -> None:
    draw.text(xy, text, font=font, fill=fill, anchor="mm", align="center", stroke_width=2, stroke_fill="#FFFFFF")


def _face(draw: ImageDraw.ImageDraw, color: str, kind: str) -> None:
    cx, cy = 540, 900
    if kind in {"cat", "dog"}:
        if kind == "cat":
            draw.polygon([(270, 730), (330, 480), (470, 690)], fill=color)
            draw.polygon([(810, 730), (750, 480), (610, 690)], fill=color)
        else:
            draw.ellipse((225, 655, 375, 1050), fill="#A66B3F")
            draw.ellipse((705, 655, 855, 1050), fill="#A66B3F")
        draw.ellipse((280, 610, 800, 1160), fill=color, outline="#513B35", width=14)
    else:
        draw.ellipse((245, 590, 835, 1160), fill=color, outline="#43546F", width=14)
        draw.ellipse((145, 690, 355, 1040), fill=color, outline="#43546F", width=12)
        draw.ellipse((725, 690, 935, 1040), fill=color, outline="#43546F", width=12)
        draw.rounded_rectangle((490, 890, 625, 1320), radius=65, fill=color, outline="#43546F", width=12)
    draw.ellipse((410, cy - 70, 455, cy - 15), fill="#222222")
    draw.ellipse((625, cy - 70, 670, cy - 15), fill="#222222")
    draw.arc((455, cy + 5, 625, cy + 145), start=10, end=170, fill="#513B35", width=12)
    if kind in {"cat", "dog"}:
        draw.ellipse((515, cy - 5, 565, cy + 40), fill="#6D3A3A")


def render_scene(path: Path, heading: str, caption: str, color: str, kind: str | None, font_path: Path) -> None:
    image = Image.new("RGB", SIZE, "#FFF5D6")
    draw = ImageDraw.Draw(image)
    draw.ellipse((-180, -130, 430, 410), fill="#BCE8DE")
    draw.ellipse((760, 1510, 1260, 2040), fill="#FFD2DA")
    title_font = ImageFont.truetype(str(font_path), 76)
    caption_font = ImageFont.truetype(str(font_path), 66)
    if kind:
        _face(draw, color, kind)
    else:
        draw.rounded_rectangle((155, 590, 925, 1170), radius=90, fill="#FFFFFF", outline="#F0B84B", width=18)
        _centered(draw, (540, 880), "?", ImageFont.truetype(str(font_path), 300), "#F0B84B")
    _centered(draw, (540, 250), heading, title_font, "#394867")
    draw.rounded_rectangle((95, 1420, 985, 1665), radius=55, fill="#FFFFFF", outline=color, width=12)
    _centered(draw, (540, 1540), caption, caption_font, "#394867")
    image.save(path, optimize=True)
