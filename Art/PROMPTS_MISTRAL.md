# Prompts Mistral (Le Chat) pour les assets du mod

Les textures actuelles sont des **placeholders** (assets vanilla/VGE teintés ou formes générées par
`Tools/art/*.py`). Ce document donne, pour chaque asset, **le fichier à produire** et **un prompt**.
Les prompts sont en anglais (les modèles d'image les suivent mieux) ; tout le reste est en français.

## Mode d'emploi

1. Dans Le Chat, coller le **bloc de style commun** puis le **prompt de l'asset**.
2. Le Chat génère en **1024×1024 sans transparence** → le fond est demandé **blanc pur uni**.
3. **Détourer** : Photopea / GIMP (*Couleurs → Couleur vers alpha*, blanc) ou remove.bg.
4. **Recadrer** au plus près puis **redimensionner** à la taille indiquée (PNG 32 bits avec alpha).
5. Déposer le fichier au chemin indiqué (depuis la racine du mod).
   - Chemins marqués **(remplace)** : il suffit d'écraser le placeholder, rien d'autre à faire.
   - Chemins marqués **(nouveau)** : le dire à Claude, qui rebranchera la def (1 ligne de XML).
6. Garder une version 1024 px quelque part (`Art/sources/`, non livré) pour pouvoir retravailler.

### Bloc de style commun (à coller avant chaque prompt)

```
RimWorld game art style: simple hand-painted 2D sprite, top-down view with a slight 3/4 angle,
flat muted colors, soft shading, thick dark outline, readable silhouette at small size,
no text, no letters, no watermark, no shadow on the ground, single object centered,
plain pure white background.
```

### Conventions RimWorld utiles
- **Graphic_Single** : un seul PNG. **Graphic_Multi** : `_north`, `_east`, `_south` (l'ouest = miroir de l'est).
- **Graphic_StackCount** : dossier avec `_a`, `_b`, `_c` (petit tas → gros tas).
- Icônes d'interface (factions, gènes, xénotypes) : **silhouette blanche** sur fond transparent
  (le jeu les teinte) — demander « white flat silhouette, pure black background » puis inverser/détourer.
- Échelle : 1 case = 64 px en jeu ; une texture est dessinée sur `drawSize` cases.

---

## 1. Objets

| Asset | Fichier | Taille |
|---|---|---|
| Naquadah (tas) | `Textures/Things/Item/Resource/SG_Naquadah/SG_Naquadah_a.png`, `_b`, `_c` **(nouveau)** | 64×64 |
| Larve goa'uld | `Textures/Things/Item/SG_GoauldLarva.png` **(nouveau)** | 64×64 |
| Symbiote goa'uld adulte | `Textures/Things/Item/SG_GoauldSymbiote.png` **(nouveau)** | 64×64 |
| Symbiote tok'ra | `Textures/Things/Item/SG_TokraSymbiote.png` **(nouveau)** | 64×64 |
| Tretonine | `Textures/Things/Item/SG_Tretonin.png` **(remplace)** | 64×64 |

- **Naquadah** (faire 3 images : 1 lingot, 3 lingots, pile de 6) :
  `a small pile of dense dark metallic ore ingots with a faint green-blue glow in the cracks, alien mineral naquadah, gunmetal grey with teal highlights`
- **Larve** : `a small pale translucent eel-like alien larva with tiny fins and a glowing yellow sheen, curled, slimy, Stargate Goa'uld larva`
- **Symbiote adulte** : `an adult alien serpent symbiote, grey-silver scaled eel with a flared cobra-like head and small fins, glowing eyes, curled in an S shape`
- **Symbiote tok'ra** : same as above with `pale blue-grey scales and a calm blue glow`
- **Tretonine** : `a small sealed glass vial of amber liquid with a golden cap and faint alien hieroglyph engravings`

## 2. Armes (icônes au sol / en main)

| Asset | Fichier | Taille |
|---|---|---|
| Bâton jaffa | `Textures/Things/Item/Equipment/SG_StaffWeapon.png` **(nouveau)** | 128×128 |
| Zat'nik'tel | `Textures/Things/Item/Equipment/SG_Zat.png` **(nouveau)** | 64×64 |
| Kara kesh | `Textures/Things/Item/Equipment/SG_KaraKesh.png` **(nouveau)** | 64×64 |

- **Bâton jaffa** : `a long ornate bronze and gold staff weapon lying diagonally, bulbous emitter head with opening petals, Egyptian-style engravings, Stargate Jaffa staff weapon`
- **Zat** : `a compact alien sidearm shaped like a coiled cobra, dark bronze and black, snake head folded at the front, blue energy glow, Stargate zat'nik'tel`
- **Kara kesh** : `a golden hand device worn on the palm and fingers, with a red-orange jewel in the center of the palm, Egyptian ornaments, Goa'uld ribbon device`

## 3. Vêtements et armures

⚠️ Les textures **portées** demandent 15 images par vêtement (5 morphologies × 3 directions, gabarit
précis) : difficile à obtenir d'un générateur. On fait d'abord les **icônes** ; les textures portées
restent en placeholder (ou seront retouchées à la main à partir des vanilla).

| Asset | Fichier (icône) | Taille |
|---|---|---|
| Armure de garde jaffa | `Textures/Things/Apparel/SG_JaffaArmor/SG_JaffaArmor.png` **(nouveau)** | 64×64 |
| Casque serpent | `Textures/Things/Apparel/SG_JaffaHelmet/SG_JaffaHelmet.png` **(nouveau)** | 64×64 |
| Parure goa'uld | `Textures/Things/Apparel/SG_GoauldRobe/SG_GoauldRobe.png` **(nouveau)** | 64×64 |
| Uniforme du SGC | `Textures/Things/Apparel/SG_SGCUniform/SG_SGCUniform.png` **(nouveau)** | 64×64 |

- **Armure jaffa** : `heavy ceremonial armor chest piece of chainmail and bronze plates, broad shoulder guards, dark leather, Stargate Jaffa serpent guard armor, laid flat`
- **Casque serpent** : `a bronze helmet shaped like a cobra head with segmented hood and glowing red eyes, Stargate serpent guard helmet`
- **Parure goa'uld** : `an opulent robe of red and gold silk with Egyptian collar and jeweled chest ornaments, folded`
- **Uniforme SGC** : `a folded military olive-green combat jacket with tactical vest and a round patch showing a ring with chevrons`

## 4. Bâtiments

| Asset | Fichier | Taille | Remarque |
|---|---|---|---|
| Porte des étoiles | `Textures/Things/Building/SG_Stargate.png` **(remplace)** | 320×320 | dessinée sur 5×5 cases, le bas de l'anneau touche le sol |
| DHD | `Textures/Things/Building/SG_DHD.png` **(remplace)** | 96×96 | vu de dessus |
| Anneaux de transport | `Textures/Things/Building/SG_TransportRings.png` **(remplace)** | 192×192 | plaque au sol, vue du dessus |
| Sarcophage | `Textures/Things/Building/SG_Sarcophagus/SG_Sarcophagus_north.png`, `_east`, `_south` **(nouveau)** | 128×128 | lit 1×2, dessiné sur 2×2 |
| Générateur au naquadah | `Textures/Things/Building/SG_NaquadahGenerator.png` **(nouveau)** | 128×128 | 2×2 |
| Canon à plasma (base) | `Textures/Things/Building/SG_StaffCannon_Base.png` **(nouveau)** | 192×192 | 3×3 |
| Canon à plasma (tourelle) | `Textures/Things/Building/SG_StaffCannon_Top.png` **(nouveau)** | 256×256 | canon pointé vers le HAUT de l'image |
| Canon à ions (base) | `Textures/Things/Building/SG_IonCannon_Base.png` **(nouveau)** | 320×320 | 5×5 |
| Canon à ions (tourelle) | `Textures/Things/Building/SG_IonCannon_Top.png` **(nouveau)** | 384×384 | pointé vers le haut |
| Bouclier goa'uld | `Textures/Things/Building/SG_GoauldShield.png` **(nouveau)** | 384×384 | 5×5, dessiné sur 6×6 |

- **Porte** : `the Stargate, a huge standing ring of dark grey metal covered with alien glyphs, nine orange-red triangular chevrons evenly around the ring, seen from the front, standing upright on a small stone base, empty inside`
- **DHD** : `top-down view of a round stone pedestal console with two concentric rings of grey glyph buttons around a large red glowing dome in the center, Stargate dial home device`
- **Anneaux** : `top-down view of a circular golden floor platform with concentric engraved rings and Egyptian patterns, Goa'uld transport rings platform, flush with the ground`
- **Sarcophage** (faire 3 vues : dessus-avant, côté, dessus-arrière) : `a golden Egyptian sarcophagus with a hinged lid decorated with hieroglyphs and blue enamel, glowing seams, Goa'uld healing sarcophagus`
- **Générateur** : `a compact alien reactor, a squat metal cylinder with a glowing green-blue naquadah core visible through slots, cables at the base`
- **Canon à plasma (base)** : `top-down view of a heavy square turret mount in gold and sandstone with Egyptian patterns, circular socket in the middle`
- **Canon à plasma (tourelle)** : `top-down view of a large golden staff cannon barrel pointing up, bulbous emitter at the tip, ornate Goa'uld design`
- **Canon à ions (base)** : `top-down view of a large sleek pale grey-blue hexagonal turret mount, clean alien design, soft blue lights`
- **Canon à ions (tourelle)** : `top-down view of a sleek elongated ion cannon with a glowing blue emitter pointing up, smooth white-grey alien hull plates, Asgard technology`
- **Bouclier** : `top-down view of a large golden pyramid-shaped shield generator with a glowing core and four emitter spikes, Goa'uld technology`

## 5. Icônes d'interface (silhouettes blanches)

Prompt commun : `flat white silhouette icon, bold simple shapes, centered, pure black background, no gradient, no text`
(puis inverser / détourer le noir en transparent).

| Icône | Fichier | Taille | Sujet |
|---|---|---|---|
| Faction Tau'ri | `Textures/UI/Factions/SG_Tauri.png` **(remplace)** | 128 | la Porte des étoiles (anneau + 9 chevrons) |
| Faction Goa'uld | `Textures/UI/Factions/SG_Goauld.png` **(remplace)** | 128 | pyramide surmontée d'un disque solaire / tête de cobra |
| Faction Tok'ra | `Textures/UI/Factions/SG_Tokra.png` **(remplace)** | 128 | symbole tok'ra : deux arcs entrelacés |
| Faction Jaffa libres | `Textures/UI/Factions/SG_FreeJaffa.png` **(remplace)** | 128 | bâton jaffa dressé, anneau brisé |
| Faction Asgard | `Textures/UI/Factions/SG_Asgard.png` **(remplace)** | 128 | marteau de Thor stylisé |
| Gène poche à symbiote | `Textures/UI/Icons/Genes/SG_JaffaPouch.png` **(remplace)** | 64 | abdomen avec poche en X |
| Gène régénération (sarcophage) | `Textures/UI/Icons/Genes/SG_SarcophagusRegen.png` **(nouveau)** | 64 | sarcophage |
| Gène matriarche | `Textures/UI/Icons/Genes/SG_QueenBrooder.png` **(nouveau)** | 64 | serpent enroulé autour d'œufs |
| Gène essence vitale (Wraith) | `Textures/UI/Icons/Genes/SG_VitalEssence.png` **(nouveau)** | 64 | goutte lumineuse / main ouverte |
| Gène drain de vie (Wraith) | `Textures/UI/Icons/Genes/SG_LifeDrain.png` **(nouveau)** | 64 | main griffue avec organe dans la paume |
| Xénotypes (×8) | `Textures/UI/Icons/Xenotypes/SG_Tauri.png`, `SG_Jaffa`, `SG_Goauld`, `SG_GoauldQueen`, `SG_Tokra`, `SG_Asgard`, `SG_Unas`, `SG_Wraith` **(nouveau)** | 64 | tête de profil de chaque race (Jaffa : marque dorée au front ; Asgard : grands yeux noirs ; Unas : reptilien ; Wraith : cheveux longs, peau pâle) |
| Commande « activer les anneaux » | `Textures/UI/Commands/SG_ActivateRings.png` **(remplace)** | 64 | trois anneaux superposés |
| Commande « franchir la porte » | `Textures/UI/Commands/SG_StargateEnter.png` **(remplace)** | 64 | anneau avec vortex bleu |
| Commande « nouvelle adresse » | `Textures/UI/Commands/SG_StargateDial.png` **(remplace)** | 64 | anneau avec chevron rouge |

(Les icônes de commande peuvent rester en couleur : fond transparent, couleurs vives.)

## 6. Non concernés (garder les placeholders)
- **Minerai de naquadah** : texture « atlas » reliée (murs de roche), impossible à générer d'un bloc.
- **Projectiles** (plasma, zat, faisceau) : les effets vanilla teintés suffisent.
- **Textures portées** des vêtements : voir §3.
