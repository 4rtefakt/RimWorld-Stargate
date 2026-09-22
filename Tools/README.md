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

Une `ERREUR` fait échouer le script (code 1). Un `AVERT` signale une référence introuvable :
à corriger, ou à confirmer en jeu puis à ajouter à `vanilla-verified.txt`.
Limite : les Defs vanilla (Data du jeu) ne sont pas publiques ; une référence vanilla absente
des `[DefOf]` et des dépendances ne peut être prouvée qu'en jeu.
