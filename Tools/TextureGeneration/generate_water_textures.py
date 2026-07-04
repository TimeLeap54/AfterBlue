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


def save_soft_patches():
    random.seed(128)
    alpha_mask = Image.new("L", (SIZE, SIZE), 0)
    draw = ImageDraw.Draw(alpha_mask)

    for _ in range(34):
        x = random.randint(-80, SIZE + 80)
        y = random.randint(-80, SIZE + 80)
        radius_x = random.randint(70, 180)
        radius_y = random.randint(45, 135)
        alpha = random.randint(72, 138)
        draw.ellipse((x - radius_x, y - radius_y, x + radius_x, y + radius_y), fill=alpha)

    alpha_mask = alpha_mask.filter(ImageFilter.GaussianBlur(radius=18.0))
    image = Image.new("RGBA", (SIZE, SIZE), (36, 144, 164, 0))
    image.putalpha(alpha_mask)
    image.save(OUT_DIR / "water_soft_patches_v01.png")


def save_surface_glints():
    random.seed(83)
    alpha_mask = Image.new("L", (SIZE, SIZE), 0)
    draw = ImageDraw.Draw(alpha_mask)

    for _ in range(72):
        x = random.randint(0, SIZE)
        y = random.randint(0, SIZE)
        length = random.randint(14, 56)
        thickness = random.choice([1, 2, 2])
        alpha = random.randint(92, 190)
        angle = random.uniform(-0.42, 0.28)

        for segment in range(random.choice([1, 1, 2])):
            gap = segment * random.randint(12, 24)
            sx = x + math.cos(angle) * gap
            sy = y + math.sin(angle) * gap
            ex = sx + math.cos(angle) * length
            ey = sy + math.sin(angle) * length
            draw.line((sx, sy, ex, ey), fill=alpha, width=thickness)

    for _ in range(24):
        x = random.randint(0, SIZE)
        y = random.randint(0, SIZE)
        width = random.randint(24, 72)
        height = random.randint(4, 10)
        alpha = random.randint(42, 92)
        draw.ellipse((x - width, y - height, x + width, y + height), fill=alpha)

    alpha_mask = alpha_mask.filter(ImageFilter.GaussianBlur(radius=1.1))
    image = Image.new("RGBA", (SIZE, SIZE), (218, 252, 255, 0))
    image.putalpha(alpha_mask)
    image.save(OUT_DIR / "water_surface_glints_v01.png")


def save_wave_bands():
    random.seed(154)
    alpha_mask = Image.new("L", (SIZE, SIZE), 0)
    draw = ImageDraw.Draw(alpha_mask)

    for row in range(-40, SIZE + 80, 52):
        phase = random.uniform(0.0, math.tau)
        amplitude = random.uniform(7.0, 18.0)
        length = random.randint(150, 300)
        start_x = random.randint(-80, 120)
        width = random.choice([3, 4, 5])
        alpha = random.randint(84, 150)
        points = []

        for i in range(0, length, 8):
            x = start_x + i
            y = row + math.sin(i * 0.045 + phase) * amplitude + math.sin(i * 0.12 + phase) * 2.0
            points.append((x, y))

        if len(points) > 1:
            draw.line(points, fill=alpha, width=width)

    for _ in range(22):
        x = random.randint(0, SIZE)
        y = random.randint(0, SIZE)
        width = random.randint(60, 150)
        height = random.randint(4, 10)
        alpha = random.randint(28, 74)
        draw.ellipse((x - width, y - height, x + width, y + height), fill=alpha)

    alpha_mask = alpha_mask.filter(ImageFilter.GaussianBlur(radius=2.2))
    image = Image.new("RGBA", (SIZE, SIZE), (232, 255, 255, 0))
    image.putalpha(alpha_mask)
    image.save(OUT_DIR / "water_wave_bands_v01.png")


def main():
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    save_noise()
    save_soft_patches()
    save_surface_glints()
    save_wave_bands()


if __name__ == "__main__":
    main()
