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

import math


def gate_ring(d, cx, cy, r_out, r_in, n_chev=9, chev_col=(210, 120, 40, 255)):
    d.ellipse([cx - r_out, cy - r_out, cx + r_out, cy + r_out], fill=(95, 100, 110, 255), outline=(40, 42, 48, 255), width=10)
    d.ellipse([cx - r_in, cy - r_in, cx + r_in, cy + r_in], fill=(0, 0, 0, 0), outline=(40, 42, 48, 255), width=8)
    d.ellipse([cx - (r_out + r_in) / 2 - 6, cy - (r_out + r_in) / 2 - 6, cx + (r_out + r_in) / 2 + 6, cy + (r_out + r_in) / 2 + 6],
              outline=(70, 74, 82, 255), width=6)
    for i in range(n_chev):
        a = -math.pi / 2 + i * 2 * math.pi / n_chev
        pts = [(-26, -r_out - 6), (26, -r_out - 6), (14, -r_out + 34), (-14, -r_out + 34)]
        ca, sa = math.cos(a + math.pi / 2), math.sin(a + math.pi / 2)
        d.polygon([(cx + x * ca - y * sa, cy + x * sa + y * ca) for x, y in pts], fill=chev_col, outline=(60, 30, 10, 255))


# Porte des étoiles (5 x 5 cases dessinées, emprise 5 x 1) : anneau debout vu de face.
img = Image.new('RGBA', (S, S), (0, 0, 0, 0))
d = ImageDraw.Draw(img)
gate_ring(d, S / 2, S / 2 - 10, 236, 170)
d.rectangle([140, 470, 372, 500], fill=(70, 72, 78, 255))          # socle
save(img, 'Things/Building/SG_Stargate', size=320)

# DHD : piédestal à glyphes vu de dessus, cristal rouge au centre.
img = Image.new('RGBA', (S, S), (0, 0, 0, 0))
d = ImageDraw.Draw(img)
d.ellipse([56, 56, 456, 456], fill=(110, 105, 95, 255), outline=(45, 42, 38, 255), width=14)
for ring_r, n in ((175, 19), (120, 19)):
    for i in range(n):
        a = i * 2 * math.pi / n
        x, y = S / 2 + ring_r * math.cos(a), S / 2 + ring_r * math.sin(a)
        d.ellipse([x - 16, y - 16, x + 16, y + 16], fill=(170, 160, 140, 255), outline=(60, 55, 50, 255), width=4)
d.ellipse([196, 196, 316, 316], fill=(210, 40, 30, 255), outline=(90, 20, 15, 255), width=10)
save(img, 'Things/Building/SG_DHD', size=128)

# Anneaux de transport : plaque au sol, cercles dorés concentriques.
img = Image.new('RGBA', (S, S), (0, 0, 0, 0))
d = ImageDraw.Draw(img)
d.ellipse([12, 12, 500, 500], fill=(80, 75, 65, 255), outline=(40, 36, 30, 255), width=12)
for r in (220, 180, 140, 100):
    d.ellipse([S / 2 - r, S / 2 - r, S / 2 + r, S / 2 + r], outline=(215, 175, 70, 255), width=16)
d.ellipse([S / 2 - 40, S / 2 - 40, S / 2 + 40, S / 2 + 40], fill=(215, 175, 70, 255))
save(img, 'Things/Building/SG_TransportRings', size=192)

# Icônes de commande.
img = Image.new('RGBA', (S, S), (0, 0, 0, 0))
d = ImageDraw.Draw(img)
for i, r in enumerate((200, 150, 100)):
    d.ellipse([S / 2 - r, S / 2 - r * 0.45 - 90 + i * 90, S / 2 + r, S / 2 + r * 0.45 - 90 + i * 90], outline=(230, 190, 80, 255), width=26)
save(img, 'UI/Commands/SG_ActivateRings', size=64)

img = Image.new('RGBA', (S, S), (0, 0, 0, 0))
d = ImageDraw.Draw(img)
gate_ring(d, S / 2, S / 2, 220, 160)
d.ellipse([S / 2 - 150, S / 2 - 150, S / 2 + 150, S / 2 + 150], fill=(90, 160, 255, 220))
save(img, 'UI/Commands/SG_StargateEnter', size=64)

img = Image.new('RGBA', (S, S), (0, 0, 0, 0))
d = ImageDraw.Draw(img)
gate_ring(d, S / 2, S / 2, 220, 160, chev_col=(255, 90, 40, 255))
d.polygon([(S / 2, 150), (S / 2 + 70, 290), (S / 2 - 70, 290)], fill=(230, 230, 230, 255))
save(img, 'UI/Commands/SG_StargateDial', size=64)
