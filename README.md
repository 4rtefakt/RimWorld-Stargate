# Stargate — Mod RimWorld 1.6

Mod sur l'univers Stargate. Voir [PLAN.md](PLAN.md) pour le plan complet (races, équipement, factions, vaisseaux, quêtes).

**Le dossier du dépôt EST le mod** (il contient `About/`, `Defs/`, `Assemblies/`…). Le sous-dossier `Source/` (code C#) n'est pas lu par RimWorld.

## Prérequis
- RimWorld **1.6** + DLC **Biotech**, **Ideology**, **Odyssey** (installés ✓).
- **.NET SDK** (8 ou +) pour compiler le C# — `dotnet build`.
- **git** ✓.

## Structure
```
About/          métadonnées du mod (About.xml)
Defs/           contenu XML (items, gènes, xénotypes, factions…)
Assemblies/     DLL compilée (générée depuis Source/, ignorée par git)
Source/         code C# + .csproj (non livré au joueur)
Textures/       PNG (placeholders pour l'instant)
```

## Compiler
```powershell
dotnet build Source/Stargate/Stargate.csproj -c Release
```
Le DLL `Stargate.dll` est écrit directement dans `Assemblies/`.

## Tester en jeu
Lier le dépôt dans le dossier `Mods` de RimWorld (jonction, ne demande pas les droits admin) :
```powershell
New-Item -ItemType Junction `
  -Path "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\Stargate" `
  -Target "C:\Users\kores\repos\rimworld-sg1"
```
Puis : lancer RimWorld → activer **Stargate** dans la liste des mods (après les DLC) → vérifier dans le log (`Ctrl+F12` ou le dossier Player.log) le message `[Stargate] Assembly chargée` et la présence de l'objet **naquadah** (dev mode → spawn).
