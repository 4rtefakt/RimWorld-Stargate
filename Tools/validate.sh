#!/usr/bin/env bash
# Valide les Defs XML du mod sans lancer RimWorld (voir Tools/README.md).
#
# Variables (toutes optionnelles, valeurs par défaut = environnement de dev Linux) :
#   DEPS_DIR : dossier contenant les dépendances clonées
#              (VanillaExpandedFramework, VanillaGravshipExpanded, VanillaGravshipExpanded2)
#   NUGET    : cache NuGet (fournit Krafs.Rimworld.Ref, Lib.Harmony, ref. .NET 4.7.2)
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
DEPS_DIR="${DEPS_DIR:-/tmp/claude-0/ref}"
NUGET="${NUGET:-$HOME/.nuget/packages}"
OUT="${OUT:-/tmp/defvalidator-build}"

NF=$(ls -d "$NUGET"/microsoft.netframework.referenceassemblies.net472/*/build/.NETFramework/v4.7.2 | tail -1)
KR=$(ls -d "$NUGET"/krafs.rimworld.ref/*/ref/net472 | tail -1)
HA=$(ls -d "$NUGET"/lib.harmony/*/lib/net472 | tail -1)

dotnet build "$ROOT/Tools/DefValidator" -c Release -o "$OUT" -v quiet -nologo > /dev/null

exec dotnet "$OUT/DefValidator.dll" \
  --mod "$ROOT" \
  --core "$NF/mscorlib.dll" \
  --asm "$NF" --asm "$KR" --asm "$HA" \
  --asm "$DEPS_DIR/VanillaExpandedFramework/1.6/Assemblies" \
  --asm "$DEPS_DIR/VanillaGravshipExpanded/1.6/Assemblies" \
  --asm "$DEPS_DIR/VanillaGravshipExpanded2/1.6/Assemblies" \
  --dep "$DEPS_DIR/VanillaExpandedFramework" \
  --dep "$DEPS_DIR/VanillaGravshipExpanded" \
  --dep "$DEPS_DIR/VanillaGravshipExpanded2" \
  --baseline "$ROOT/Tools/DefValidator/vanilla-verified.txt"
