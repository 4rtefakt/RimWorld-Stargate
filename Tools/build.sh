#!/usr/bin/env bash
# Compile Assemblies/Stargate.dll en pointant vers des copies locales des mods VEF/VGE
# (sous Windows avec les mods abonnés sur le Workshop, un simple `dotnet build` suffit).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
DEPS_DIR="${DEPS_DIR:-/tmp/claude-0/ref}"
dotnet build "$ROOT/Source/Stargate/Stargate.csproj" -c Release -nologo \
  -p:VEFDir="$DEPS_DIR/VanillaExpandedFramework" \
  -p:VGEDir="$DEPS_DIR/VanillaGravshipExpanded" \
  -p:VGE2Dir="$DEPS_DIR/VanillaGravshipExpanded2" "$@"
