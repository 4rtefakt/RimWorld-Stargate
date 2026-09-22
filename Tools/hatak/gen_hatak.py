#!/usr/bin/env python3
"""Génère le plan (PrefabDef) du Ha'tak goa'uld : Defs/PrefabDefs/Prefabs_Hatak.xml.

Le vaisseau est construit par tampons (coque octogonale, anneau de cloisons,
pyramide en or, ponts extérieurs), puis vérifié automatiquement :
  - empreinte de chaque bâtiment = sa taille réelle (rotation comprise) ;
  - coque étanche : aucune case intérieure accessible depuis l'extérieur sans porte ;
  - chaque pièce a une pompe à oxygène, accrochée à un mur ;
  - toutes les pièces sont reliées par des portes ;
  - cases d'interaction (consoles, terminaux) libres.
Usage : python3 Tools/hatak/gen_hatak.py  (affiche le plan ASCII, écrit le XML)
Avant du vaisseau = +X (est) : les propulseurs regardent l'est, comme dans VGE.
"""
import os, sys

W, H = 47, 41
EMPTY, FLOOR, DECK = ' ', '.', ','
HULL, GOLD, DOOR = '#', 'W', 'D'

# Bâtiments : lettre -> (defName, taille (x,z), rotation, franchissable pour les salles)
ROTS = {'N': 0, 'E': 1, 'S': 2, 'W': 3}
REL = {0: None, 1: 'Clockwise', 2: 'Opposite', 3: 'Counterclockwise'}
BUILDINGS = {
    'E': ('VGE_EnemyGravEngine', (3, 3), 'N', True),
    'C': ('SG_EnemyGoauldStaffCannon', (3, 3), 'N', False),
    'P': ('VGE_EnemyPointDefenseTurret', (3, 3), 'N', False),
    'T': ('VGE_EnemyLargeThruster', (2, 2), 'E', False),
    'K': ('VGE_EnemyPilotConsole', (3, 2), 'E', True),
    'x': ('VGE_EnemyTargetingTerminal', (1, 1), 'E', True),
    'f': ('VGE_EnemyGravFieldExtender', (1, 1), 'N', True),
    'S': ('SG_Sarcophagus', (1, 2), 'N', True),
    'B': ('VGE_CrewBunkBed', (1, 2), 'N', True),
    'c': ('SealedCrate', (1, 1), 'N', True),
    'b': ('VGE_SpacerBox_Steel', (1, 1), 'N', True),
    'k': ('VGE_SpacerBox_ComponentIndustrial', (1, 1), 'N', True),
    'n': ('SG_Naquadah', (1, 1), 'N', True),
    'O': ('VGE_EnemyOxygenPump', (1, 1), None, True),  # rotation = vers le mur voisin
}
ITEM_STACKS = {'SG_Naquadah': '20~40'}
INTERACTION = {'K': (0, -1), 'x': (0, -1)}  # interactionCellOffset (avant rotation)

g = [[EMPTY] * W for _ in range(H)]


def put(x, z, ch):
    g[z][x] = ch


def rect(x0, z0, x1, z1, ch, only=None):
    for z in range(z0, z1 + 1):
        for x in range(x0, x1 + 1):
            if only is None or g[z][x] in only:
                g[z][x] = ch


def ring(x0, z0, x1, z1, ch, only=None):
    for x in range(x0, x1 + 1):
        for z in (z0, z1):
            if only is None or g[z][x] in only:
                g[z][x] = ch
    for z in range(z0, z1 + 1):
        for x in (x0, x1):
            if only is None or g[z][x] in only:
                g[z][x] = ch


# ---------------------------------------------------------------- ponts extérieurs
rect(1, 11, 5, 29, DECK)      # poupe (propulseurs)
rect(1, 31, 8, 37, DECK)      # nord-ouest
rect(1, 3, 8, 9, DECK)        # sud-ouest
rect(18, 36, 28, 40, DECK)    # nord
rect(18, 0, 28, 4, DECK)      # sud
rect(36, 30, 46, 36, DECK)    # éperon nord-est
rect(36, 4, 46, 10, DECK)     # éperon sud-est

# ---------------------------------------------------------------- coque octogonale
X0, Z0, X1, Z1, CUT = 6, 4, 40, 36, 4


def in_hull(x, z):
    return (X0 <= x <= X1 and Z0 <= z <= Z1
            and (x - X0) + (z - Z0) >= CUT and (X1 - x) + (z - Z0) >= CUT
            and (x - X0) + (Z1 - z) >= CUT and (X1 - x) + (Z1 - z) >= CUT)


for z in range(H):
    for x in range(W):
        if in_hull(x, z):
            edge = any(not in_hull(x + dx, z + dz) for dx in (-1, 0, 1) for dz in (-1, 0, 1))
            g[z][x] = HULL if edge else FLOOR

# cloisons : anneau central + séparations des blocs
rect(7, 10, 39, 10, HULL, only=FLOOR)
rect(7, 30, 39, 30, HULL, only=FLOOR)
rect(13, 10, 13, 30, HULL, only=FLOOR)
rect(33, 10, 33, 30, HULL, only=FLOOR)
rect(7, 20, 12, 20, HULL, only=FLOOR)
rect(34, 20, 39, 20, HULL, only=FLOOR)

# ---------------------------------------------------------------- pyramide (murs en or)
ring(15, 12, 31, 28, GOLD)
ring(20, 17, 26, 23, GOLD)            # salle du moteur
rect(26, 13, 26, 16, GOLD)            # passerelle (est) : x 27..30
rect(26, 24, 26, 27, GOLD)
rect(20, 13, 20, 16, GOLD)            # salle du trône (ouest) : x 16..19
rect(20, 24, 20, 27, GOLD)

# ---------------------------------------------------------------- portes
for x, z in [
    (15, 20), (20, 20), (26, 20), (31, 15), (20, 14), (20, 26), (26, 14), (26, 26),   # pyramide
    (13, 15), (13, 25), (33, 15), (33, 25), (23, 30), (23, 10),                      # anneau
    (23, 36), (23, 4), (6, 15), (6, 25),                                             # sas extérieurs
]:
    put(x, z, DOOR)


def stamp(ch, minx, minz):
    name, (sx, sz), rot, _ = BUILDINGS[ch]
    r = ROTS[rot] if rot else 0
    w, h = (sz, sx) if r % 2 else (sx, sz)
    for z in range(minz, minz + h):
        for x in range(minx, minx + w):
            assert g[z][x] in (FLOOR, DECK), f"{ch} sur {g[z][x]!r} en ({x},{z})"
            g[z][x] = ch


# ---------------------------------------------------------------- armement et propulsion
stamp('C', 2, 33); stamp('C', 2, 5); stamp('C', 42, 32); stamp('C', 42, 6)
stamp('P', 22, 38); stamp('P', 22, 0)
for z in (12, 16, 23, 27):
    stamp('T', 2, z)

# ---------------------------------------------------------------- cœur de la pyramide
stamp('E', 22, 19)
put(21, 22, 'f'); put(25, 18, 'f')
stamp('K', 29, 19)                    # console de pilotage, face à l'est
put(30, 16, 'x'); put(30, 24, 'x')    # terminaux de ciblage
stamp('S', 17, 20)                    # sarcophage du seigneur
for x, z in [(22, 26), (24, 26), (22, 14), (24, 14)]:
    put(x, z, 'n')
put(21, 26, 'c'); put(25, 26, 'c')

# ---------------------------------------------------------------- blocs extérieurs
for x in (8, 10):                      # dortoirs jaffa
    stamp('B', x, 22); stamp('B', x, 27 - 1); stamp('B', x, 12); stamp('B', x, 16)
for x, z in [(35, 12), (36, 12), (38, 18), (35, 28), (38, 28)]:
    put(x, z, 'c')
for x, z in [(37, 22), (38, 22), (37, 16)]:
    put(x, z, 'b')
put(35, 18, 'k'); put(35, 22, 'k')
for x, z in [(16, 33), (18, 33), (28, 33), (30, 33)]:
    put(x, z, 'n')
for x, z in [(20, 34), (26, 34), (14, 32), (32, 32)]:
    put(x, z, 'c')
for x, z in [(16, 7), (18, 7), (28, 7), (30, 7)]:
    put(x, z, 'b')
for x, z in [(20, 6), (26, 6)]:
    put(x, z, 'k')

# ---------------------------------------------------------------- pompes à oxygène (1 par pièce)
for x, z in [(22, 35), (22, 5), (7, 29), (7, 11), (39, 29), (39, 11),
             (14, 23), (16, 24), (21, 18), (30, 26), (21, 27), (21, 13)]:
    put(x, z, 'O')

# ================================================================= vérifications
WALLS = {HULL, GOLD, DOOR}


def cells_of(ch):
    return [(x, z) for z in range(H) for x in range(W) if g[z][x] == ch]


def components(ch):
    seen, comps = set(), []
    for c in cells_of(ch):
        if c in seen:
            continue
        stack, comp = [c], []
        seen.add(c)
        while stack:
            x, z = stack.pop()
            comp.append((x, z))
            for dx, dz in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                n = (x + dx, z + dz)
                if 0 <= n[0] < W and 0 <= n[1] < H and n not in seen and g[n[1]][n[0]] == ch:
                    seen.add(n)
                    stack.append(n)
        comps.append(comp)
    return comps


def anchor(minx, minz, w, h, r):
    """Inverse de GenAdj.OccupiedRect / AdjustForRotation."""
    cx, cz = minx + (w - 1) // 2, minz + (h - 1) // 2
    if r == 1 and h % 2 == 0:
        cz += 1
    if r == 2:
        cx += w % 2 == 0
        cz += h % 2 == 0
    if r == 3 and w % 2 == 0:
        cx += 1
    return cx, cz


def rotate(dx, dz, r):
    for _ in range(r):
        dx, dz = dz, -dx
    return dx, dz


errors = []
placed = []  # (defName, (x,z), rotation)
for ch, (name, (sx, sz), rot, _) in BUILDINGS.items():
    for comp in components(ch):
        xs, zs = [c[0] for c in comp], [c[1] for c in comp]
        minx, minz = min(xs), min(zs)
        if ch == 'O':
            x, z = comp[0]
            r = next((i for i, (dx, dz) in enumerate(((0, 1), (1, 0), (0, -1), (-1, 0)))
                      if g[z + dz][x + dx] in (HULL, GOLD)), None)
            if r is None:
                errors.append(f"pompe ({x},{z}) sans mur voisin")
                r = 0
            for c in comp:
                placed.append((name, c, r))
            continue
        r = ROTS[rot]
        w, h = (sz, sx) if r % 2 else (sx, sz)
        if (sx, sz) == (1, 1):
            for c in comp:
                placed.append((name, c, r))
            continue
        # plusieurs bâtiments identiques peuvent se toucher : on découpe en blocs w x h
        todo = set(comp)
        while todo:
            bx = min(c[0] for c in todo)
            bz = min(c[1] for c in todo if c[0] == bx)
            block = {(bx + i, bz + j) for i in range(w) for j in range(h)}
            if not block <= todo:
                errors.append(f"{name} : empreinte irrégulière près de ({bx},{bz})")
                break
            todo -= block
            ax, az = anchor(bx, bz, w, h, r)
            placed.append((name, (ax, az), r))
            if ch in INTERACTION:
                ix, iz = rotate(*INTERACTION[ch], r)
                cell = g[az + iz][ax + ix]
                if cell not in (FLOOR,):
                    errors.append(f"{name} ({ax},{az}) : case d'interaction ({ax+ix},{az+iz}) occupée par {cell!r}")

interior = {(x, z) for z in range(H) for x in range(W) if in_hull(x, z)}

# étanchéité : depuis l'extérieur, sans traverser murs/portes
outside = {(x, z) for z in range(H) for x in range(W) if (x, z) not in interior}
seen = set(outside)
stack = list(outside)
while stack:
    x, z = stack.pop()
    for dx, dz in ((1, 0), (-1, 0), (0, 1), (0, -1)):
        n = (x + dx, z + dz)
        if n in interior and n not in seen and g[n[1]][n[0]] not in WALLS:
            seen.add(n)
            stack.append(n)
leaks = seen & interior
if leaks:
    errors.append(f"coque non étanche : {sorted(leaks)[:5]}")

# pièces : composantes de cases intérieures non-murs
rooms, assigned = [], {}
for c in sorted(interior):
    if g[c[1]][c[0]] in WALLS or c in assigned:
        continue
    room, stack = [], [c]
    assigned[c] = len(rooms)
    while stack:
        x, z = stack.pop()
        room.append((x, z))
        for dx, dz in ((1, 0), (-1, 0), (0, 1), (0, -1)):
            n = (x + dx, z + dz)
            if n in interior and n not in assigned and g[n[1]][n[0]] not in WALLS:
                assigned[n] = len(rooms)
                stack.append(n)
    rooms.append(room)
for i, room in enumerate(rooms):
    pumps = sum(1 for (x, z) in room if g[z][x] == 'O')
    if pumps < 1:
        errors.append(f"pièce {i} ({len(room)} cases, ex. {room[0]}) sans pompe à oxygène")
# connexité par les portes
adj = {i: set() for i in range(len(rooms))}
for (x, z) in cells_of(DOOR):
    near = {assigned[(x + dx, z + dz)] for dx, dz in ((1, 0), (-1, 0), (0, 1), (0, -1)) if (x + dx, z + dz) in assigned}
    for a in near:
        adj[a] |= near - {a}
reach, stack = {0}, [0]
while stack:
    for n in adj[stack.pop()]:
        if n not in reach:
            reach.add(n)
            stack.append(n)
if len(reach) != len(rooms):
    errors.append(f"pièces inaccessibles : {sorted(set(range(len(rooms))) - reach)}")

# ================================================================= affichage + XML
for z in reversed(range(H)):
    print(f"{z:2d} " + ''.join(g[z]))
print(f"{len(rooms)} pièces, {len(placed)} objets placés")
if errors:
    print("ERREURS :")
    for e in errors:
        print("  - " + e)
    sys.exit(1)


def runs(cells):
    out = []
    for z in sorted({c[1] for c in cells}):
        xs = sorted(c[0] for c in cells if c[1] == z)
        start = prev = xs[0]
        for x in xs[1:] + [None]:
            if x is not None and x == prev + 1:
                prev = x
                continue
            out.append(f"({start},{z},{prev},{z})")
            if x is not None:
                start = prev = x
    return out


lines = ['<?xml version="1.0" encoding="utf-8" ?>', '<Defs>', '',
         "  <!-- ============================================================",
         "       PLAN DU HA'TAK GOA'ULD — FICHIER GÉNÉRÉ par Tools/hatak/gen_hatak.py",
         "       (ne pas éditer à la main : modifier le script puis le relancer).",
         "       Coque octogonale de gravship, pyramide centrale en or (passerelle,",
         "       salle du trône avec sarcophage, moteur gravitationnel au cœur),",
         "       dortoirs jaffa, soutes (naquadah), 4 canons à plasma et 2 tourelles",
         "       de défense rapprochée sur les ponts, 4 propulseurs à la poupe.",
         "       ============================================================ -->",
         '  <PrefabDef>', '    <defName>SG_Hatak_1</defName>', f'    <size>({W},{H})</size>', '    <things>']


def emit_rects(tag, cells, stuff=None):
    lines.append(f'      <{tag}>')
    lines.append('        <rects>')
    lines.extend(f'          <li>{r}</li>' for r in runs(cells))
    lines.append('        </rects>')
    if stuff:
        lines.append(f'        <stuff>{stuff}</stuff>')
    lines.append(f'      </{tag}>')


emit_rects('GravshipHull', cells_of(HULL))
emit_rects('Wall', cells_of(GOLD), 'Gold')
emit_rects('Door', cells_of(DOOR), 'Gold')
groups = {}
for name, pos, r in placed:
    groups.setdefault((name, r), []).append(pos)
for (name, r), poss in sorted(groups.items()):
    lines.append(f'      <{name}>')
    lines.append('        <positions>')
    lines.extend(f'          <li>({x}, 0, {z})</li>' for x, z in sorted(poss))
    lines.append('        </positions>')
    if REL[r]:
        lines.append(f'        <relativeRotation>{REL[r]}</relativeRotation>')
    if name in ITEM_STACKS:
        lines.append(f'        <stackCountRange>{ITEM_STACKS[name]}</stackCountRange>')
    lines.append(f'      </{name}>')
lines.append('    </things>')
solid = [(x, z) for z in range(H) for x in range(W) if g[z][x] != EMPTY]
lines += ['    <terrain>', '      <VGE_EnemySubstructure>', '        <rects>']
lines += [f'          <li>{r}</li>' for r in runs(solid)]
lines += ['        </rects>', '      </VGE_EnemySubstructure>', '    </terrain>',
          '    <modExtensions>', '      <li Class="VEF.Storyteller.PrefabExtension">', '        <roofs>', '          <li>',
          '            <def>RoofConstructed</def>', '            <rects>']
lines += [f'              <li>{r}</li>' for r in runs(sorted(interior))]
lines += ['            </rects>', '          </li>', '        </roofs>', '      </li>', '    </modExtensions>', '  </PrefabDef>', '',
          '  <VEF.Storyteller.StructureSetDef>', '    <defName>SG_HatakSet</defName>', '    <structureLayouts>', '      <li>',
          '        <pattern>SG_Hatak_[0-9]+</pattern>', '        <offset>(0,0,0)</offset>', '        <randomRotated>false</randomRotated>',
          '      </li>', '    </structureLayouts>', '  </VEF.Storyteller.StructureSetDef>', '', '</Defs>', '']
root = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..'))
out = os.path.join(root, 'Defs', 'PrefabDefs', 'Prefabs_Hatak.xml')
os.makedirs(os.path.dirname(out), exist_ok=True)
with open(out, 'w', encoding='utf-8') as f:
    f.write('\n'.join(lines))
print("écrit :", os.path.relpath(out, root))
