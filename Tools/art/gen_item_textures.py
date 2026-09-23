#!/usr/bin/env python3
"""Textures PROVISOIRES d'objets et d'icônes de gènes générées par code.
À remplacer par l'art final (cf. Art/PROMPTS_MISTRAL.md) en gardant les chemins.
Usage : python3 Tools/art/gen_item_textures.py
"""
import os
from PIL import Image, ImageDraw

S, OUT = 512, 64
ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..'))


def save(img, rel, size=OUT):
    path = os.path.join(ROOT, 'Textures', rel + '.png')
    os.makedirs(os.path.dirname(path), exist_ok=True)
    img.resize((size, size), Image.LANCZOS).save(path)
    print('écrit', os.path.relpath(path, ROOT))


# Tretonine : fiole à bouchon doré, liquide ambré.
img = Image.new('RGBA', (S, S), (0, 0, 0, 0))
d = ImageDraw.Draw(img)
d.rounded_rectangle([196, 120, 316, 440], radius=50, fill=(215, 225, 230, 235), outline=(60, 70, 80, 255), width=12)
d.rounded_rectangle([208, 250, 304, 428], radius=40, fill=(225, 150, 40, 255))
d.rectangle([206, 70, 306, 130], fill=(200, 160, 50, 255), outline=(90, 70, 20, 255), width=10)
d.ellipse([230, 280, 260, 320], fill=(255, 220, 150, 200))
save(img, 'Things/Item/SG_Tretonin')

# Icône de gène « poche à symbiote » : abdomen avec la poche en X des Jaffa.
img = Image.new('RGBA', (S, S), (0, 0, 0, 0))
d = ImageDraw.Draw(img)
d.ellipse([96, 96, 416, 416], fill=(170, 130, 95, 255), outline=(70, 50, 35, 255), width=16)
d.line([(186, 186), (326, 326)], fill=(90, 40, 30, 255), width=34)
d.line([(326, 186), (186, 326)], fill=(90, 40, 30, 255), width=34)
d.ellipse([226, 226, 286, 286], fill=(225, 190, 90, 255))
save(img, 'UI/Icons/Genes/SG_JaffaPouch', size=64)
