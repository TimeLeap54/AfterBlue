from pathlib import Path
from PIL import Image, ImageDraw, ImageFilter
import math
import random


ROOT = Path(__file__).resolve().parents[2]
OUT_DIR = ROOT / "Assets" / "Textures" / "Water"
SIZE = 512


def save_noise():
    random.seed(54)
    image = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    pixels = image.load()

    for y in range(SIZE):
        for x in range(SIZE):
            wave_a = math.sin((x * 0.055) + math.sin(y * 0.021) * 2.0)
            wave_b = math.sin((x * 0.018) + (y * 0.052))
            grain = random.random() * 0.35
            value = max(0.0, (wave_a * 0.35 + wave_b * 0.25 + grain) - 0.28)
            alpha = int(min(112, value * 150))
            pixels[x, y] = (210, 250, 255, alpha)

    image = image.filter(ImageFilter.GaussianBlur(radius=1.2))
    image.save(OUT_DIR / "water_noise_soft_v01.png")


def save_ripple_lines():
    random.seed(83)
    image = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    draw = ImageDraw.Draw(image)

    for _ in range(42):
        start_x = random.randint(-120, SIZE)
        start_y = random.randint(0, SIZE)
        length = random.randint(90, 240)
        amplitude = random.uniform(4.0, 13.0)
        thickness = random.choice([1, 2, 2, 3])
        alpha = random.randint(70, 145)
        points = []

        for i in range(0, length, 7):
            x = start_x + i
            y = start_y + math.sin(i * 0.075 + random.random() * 0.2) * amplitude
            points.append((x % SIZE, y % SIZE))

        if len(points) > 1:
            draw.line(points, fill=(225, 252, 255, alpha), width=thickness)

    for _ in range(22):
        x = random.randint(0, SIZE)
        y = random.randint(0, SIZE)
        radius_x = random.randint(18, 58)
        radius_y = random.randint(3, 9)
        alpha = random.randint(48, 96)
        draw.arc(
            (x - radius_x, y - radius_y, x + radius_x, y + radius_y),
            start=random.randint(0, 80),
            end=random.randint(140, 330),
            fill=(230, 255, 255, alpha),
            width=1,
        )

    image = image.filter(ImageFilter.GaussianBlur(radius=0.55))
    image.save(OUT_DIR / "water_ripple_lines_v01.png")


def main():
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    save_noise()
    save_ripple_lines()


if __name__ == "__main__":
    main()
