#!/usr/bin/env python3
"""Icônes de faction PROVISOIRES (Textures/UI/Factions/*.png).

Formes blanches sur fond transparent : RimWorld les teinte à la couleur de la
faction. À remplacer par l'art final (cf. Art/PROMPTS_MISTRAL.md) en gardant
les mêmes noms de fichiers. Usage : python3 Tools/art/gen_faction_icons.py
"""
import math, os
from PIL import Image, ImageDraw

S = 512           # dessin sur-échantillonné, réduit à 128 px (anticrénelage)
OUT = 128
W = (255, 255, 255, 255)
ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..'))
DEST = os.path.join(ROOT, 'Textures', 'UI', 'Factions')


def canvas():
    img = Image.new('RGBA', (S, S), (0, 0, 0, 0))
    return img, ImageDraw.Draw(img)


def save(img, name):
    os.makedirs(DEST, exist_ok=True)
    img.resize((OUT, OUT), Image.LANCZOS).save(os.path.join(DEST, name + '.png'))


def ring(d, cx, cy, r_out, r_in):
    d.ellipse([cx - r_out, cy - r_out, cx + r_out, cy + r_out], fill=W)
    d.ellipse([cx - r_in, cy - r_in, cx + r_in, cy + r_in], fill=(0, 0, 0, 0))


def poly_at(cx, cy, pts, angle):
    ca, sa = math.cos(angle), math.sin(angle)
    return [(cx + x * ca - y * sa, cy + x * sa + y * ca) for x, y in pts]


# Tau'ri / SGC : la Porte des étoiles (anneau + 9 chevrons).
img, d = canvas()
c = S / 2
ring(d, c, c, 232, 170)
d.ellipse([c - 150, c - 150, c + 150, c + 150], outline=W, width=10)
for i in range(9):
    a = -math.pi / 2 + i * 2 * math.pi / 9
    chevron = [(-34, -250), (34, -250), (18, -196), (-18, -196)]
    # le chevron est défini "vers le haut" (y négatif) puis tourné autour du centre
    pts = poly_at(c, c, chevron, a + math.pi / 2)
    d.polygon(pts, fill=W)
save(img, 'SG_Tauri')

# Grands Maîtres goa'uld : pyramide et disque solaire.
img, d = canvas()
d.polygon([(c, 150), (c + 210, 440), (c - 210, 440)], fill=W)
d.polygon([(c, 230), (c + 120, 400), (c - 120, 400)], fill=(0, 0, 0, 0))
d.polygon([(c, 290), (c + 60, 380), (c - 60, 380)], fill=W)
d.ellipse([c - 62, 20, c + 62, 144], fill=W)
save(img, 'SG_Goauld')

# Tok'ra : deux anneaux entrelacés (l'hôte et le symbiote, à égalité).
img, d = canvas()
ring(d, c - 70, c, 150, 112)
ring(d, c + 70, c, 150, 112)
d.ellipse([c - 70 - 150, c - 150, c - 70 + 150, c + 150], outline=W, width=38)
save(img, "SG_Tokra")

# Jaffa libres : bâton jaffa dressé devant un anneau brisé.
img, d = canvas()
ring(d, c, c, 200, 158)
d.rectangle([c - 40, c - 220, c + 40, c + 220], fill=(0, 0, 0, 0))   # l'anneau est brisé
d.rectangle([c - 16, 40, c + 16, 480], fill=W)                        # hampe
d.polygon([(c, 10), (c + 44, 90), (c, 130), (c - 44, 90)], fill=W)     # tête du bâton
save(img, 'SG_FreeJaffa')

# Asgard : marteau de Thor stylisé.
img, d = canvas()
d.rounded_rectangle([c - 190, 70, c + 190, 210], radius=30, fill=W)   # tête
d.rectangle([c - 34, 200, c + 34, 420], fill=W)                        # manche
d.ellipse([c - 60, 400, c + 60, 480], fill=W)                          # pommeau
d.polygon([(c, 245), (c + 18, 280), (c, 315), (c - 18, 280)], fill=(0, 0, 0, 0))
save(img, 'SG_Asgard')

print('icônes écrites dans', os.path.relpath(DEST, ROOT))
