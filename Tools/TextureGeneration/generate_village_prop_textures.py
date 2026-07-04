from pathlib import Path
from PIL import Image, ImageDraw, ImageFilter
import random


ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / "Assets" / "Textures" / "Props"
SIZE = 1024


def jitter(color, amount=20):
    return tuple(max(0, min(255, c + random.randint(-amount, amount))) for c in color)


def roof_texture():
    random.seed(521)
    image = Image.new("RGB", (SIZE, SIZE), (42, 48, 49))
    draw = ImageDraw.Draw(image)
    tile_w = 82
    tile_h = 56

    for y in range(-tile_h, SIZE + tile_h, tile_h):
        offset = 0 if (y // tile_h) % 2 == 0 else tile_w // 2
        for x in range(-tile_w, SIZE + tile_w, tile_w):
            rect = (x + offset, y, x + offset + tile_w - 3, y + tile_h - 4)
            draw.rectangle(rect, fill=jitter((47, 53, 53), 9), outline=jitter((24, 30, 31), 5))

    for _ in range(130):
        x = random.randint(0, SIZE)
        y = random.randint(0, SIZE)
        length = random.randint(18, 90)
        draw.line((x, y, x + random.randint(-35, 35), y + length), fill=jitter((20, 25, 26), 6), width=1)

    for _ in range(90):
        x = random.randint(0, SIZE)
        y = random.randint(0, SIZE)
        w = random.randint(24, 160)
        h = random.randint(4, 18)
        color = random.choice([(65, 88, 42), (86, 102, 68), (130, 134, 110), (38, 64, 34)])
        draw.ellipse((x - w, y - h, x + w, y + h), fill=jitter(color, 10))

    for _ in range(2600):
        x = random.randrange(SIZE)
        y = random.randrange(SIZE)
        r, g, b = image.getpixel((x, y))
        delta = random.randint(-8, 8)
        image.putpixel((x, y), (max(0, min(255, r + delta)), max(0, min(255, g + delta)), max(0, min(255, b + delta))))

    image = image.filter(ImageFilter.GaussianBlur(radius=0.25))
    image.save(OUT / "roof_slate_grime_v01.png")


def concrete_texture():
    random.seed(522)
    image = Image.new("RGB", (SIZE, SIZE), (105, 112, 108))
    draw = ImageDraw.Draw(image)

    for _ in range(240):
        x = random.randint(0, SIZE)
        y = random.randint(0, SIZE)
        w = random.randint(10, 180)
        h = random.randint(4, 60)
        draw.ellipse((x - w, y - h, x + w, y + h), fill=jitter(random.choice([(74, 82, 80), (128, 132, 120), (43, 55, 52), (154, 154, 136)]), 20))

    for _ in range(95):
        x = random.randint(0, SIZE)
        y = random.randint(0, SIZE)
        draw.line((x, y, x + random.randint(-160, 160), y + random.randint(-90, 150)), fill=jitter((42, 48, 47), 12), width=random.choice([1, 1, 2]))

    image = image.filter(ImageFilter.GaussianBlur(radius=0.45))
    image.save(OUT / "wet_concrete_grime_v01.png")


def pole_texture():
    random.seed(523)
    image = Image.new("RGB", (SIZE, SIZE), (105, 96, 82))
    draw = ImageDraw.Draw(image)

    for x in range(SIZE):
        stripe = random.randint(-8, 8)
        for y in range(SIZE):
            if random.random() < 0.018:
                stripe += random.randint(-5, 5)
            base = (112 + stripe, 104 + stripe // 2, 88 + stripe // 3)
            image.putpixel((x, y), tuple(max(0, min(255, c + random.randint(-4, 4))) for c in base))

    for _ in range(120):
        x = random.randint(0, SIZE)
        y = random.randint(0, SIZE)
        w = random.randint(6, 36)
        h = random.randint(24, 180)
        color = random.choice([(62, 49, 39), (104, 66, 42), (45, 70, 40), (142, 137, 112)])
        draw.ellipse((x - w, y - h, x + w, y + h), fill=jitter(color, 10))

    for y in range(170, SIZE, 210):
        draw.rectangle((0, y, SIZE, y + random.randint(8, 18)), fill=jitter((70, 55, 42), 12))

    image = image.filter(ImageFilter.GaussianBlur(radius=0.35))
    image.save(OUT / "rusted_pole_grime_v01.png")


def main():
    OUT.mkdir(parents=True, exist_ok=True)
    roof_texture()
    concrete_texture()
    pole_texture()


if __name__ == "__main__":
    main()
