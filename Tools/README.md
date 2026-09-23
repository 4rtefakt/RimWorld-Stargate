# Outils de développement

Rien ici n'est lu par RimWorld ni livré au joueur.

## `validate.sh` — validation des Defs sans lancer le jeu

Charge en métadonnées les DLL du jeu (paquet NuGet `Krafs.Rimworld.Ref`), de Harmony,
de Vanilla Expanded Framework et de Vanilla Gravship Expanded (Ch.1 et Ch.2), puis vérifie :

- que chaque type de Def et chaque `Class="..."` existe ;
- que chaque balise XML correspond à un vrai champ du type ciblé (récursivement) ;
- les valeurs d'enum et de `System.Type` ;
- que chaque référence de Def pointe vers un defName connu : défini par le mod, par une
  dépendance, par une classe `[DefOf]` du jeu, ou listé dans `DefValidator/vanilla-verified.txt` ;
- que chaque clé `"SG_…".Translate()` du C# existe dans toutes les langues `Keyed`.

```bash
# dépendances (une fois) : sources publiques des mods VE
git clone --depth 1 https://github.com/Vanilla-Expanded/VanillaGravshipExpanded.git  $DEPS_DIR/VanillaGravshipExpanded
git clone --depth 1 https://github.com/Vanilla-Expanded/VanillaGravshipExpanded2.git $DEPS_DIR/VanillaGravshipExpanded2
git clone --depth 1 --filter=blob:none --sparse https://github.com/Vanilla-Expanded/VanillaExpandedFramework.git $DEPS_DIR/VanillaExpandedFramework
git -C $DEPS_DIR/VanillaExpandedFramework sparse-checkout set --no-cone '/1.6/Assemblies/*' '/1.6/Defs/*'

DEPS_DIR=... Tools/validate.sh
```

Optionnel : des mods publics non chargés en jeu peuvent servir d'**indices** (`$DEPS_DIR/hints/<mod>/`,
option `--hint`) : leurs références vers des Defs vanilla comptent comme preuves d'existence
(ex. les dépôts `Vanilla-Expanded/VanillaFactionsExpanded-*`, en sparse-checkout des dossiers `Defs`).

Contrôles supplémentaires : chaque champ des classes `[DefOf]` du mod doit désigner une Def existante.

Une `ERREUR` fait échouer le script (code 1). Un `AVERT` signale une référence introuvable :
à corriger, ou à confirmer en jeu puis à ajouter à `vanilla-verified.txt`.
Limite : les Defs vanilla (Data du jeu) ne sont pas publiques ; une référence vanilla absente
des `[DefOf]` et des dépendances ne peut être prouvée qu'en jeu.

## `build.sh` — compilation hors Windows

Compile `Assemblies/Stargate.dll` en pointant vers des copies locales de VEF / VGE
(sous Windows avec les mods abonnés, `dotnet build Source/Stargate/Stargate.csproj -c Release` suffit).

## `hatak/gen_hatak.py` — plan du Ha'tak

Génère `Defs/PrefabDefs/Prefabs_Hatak.xml` à partir d'une construction par tampons et vérifie
automatiquement : empreinte des bâtiments, étanchéité de la coque, une pompe à oxygène par pièce,
accès par portes, cases d'interaction libres. Affiche le plan en ASCII. Ne pas éditer le XML à la main.

## `art/*.py` — textures provisoires

`gen_faction_icons.py` (icônes de faction) et `gen_item_textures.py` (tretonine, porte, DHD,
anneaux, icônes de commande et de gène). À remplacer par l'art final : voir `Art/PROMPTS_MISTRAL.md`.
