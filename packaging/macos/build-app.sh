#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/../.."
if [[ "$(uname -s)" != "Darwin" ]]; then
  echo "Le bundle .app/.dmg doit être produit sur macOS avec un JDK 21 contenant jpackage." >&2
  exit 2
fi
rm -rf build/macos
mkdir -p build/macos
jpackage \
  --type app-image \
  --name "SGA Subtitler" \
  --app-version "6.0.0" \
  --vendor "LC30" \
  --input build/libs \
  --main-jar SGA-Subtitler-V0.6.jar \
  --main-class com.sgasubtitler.app.SgaSubtitlerApp \
  --dest build/macos \
  --java-options "-Dapple.laf.useScreenMenuBar=true"
jpackage \
  --type dmg \
  --name "SGA Subtitler" \
  --app-version "6.0.0" \
  --vendor "LC30" \
  --app-image "build/macos/SGA Subtitler.app" \
  --dest build/macos

generated_dmg="$(find build/macos -maxdepth 1 -type f -name 'SGA Subtitler*.dmg' -print -quit)"
if [[ -z "$generated_dmg" ]]; then
  echo "Erreur: jpackage n'a produit aucun DMG." >&2
  exit 1
fi
final_dmg="build/macos/SGA Subtitler V0.6.dmg"
if [[ "$generated_dmg" != "$final_dmg" ]]; then
  mv -f "$generated_dmg" "$final_dmg"
fi

echo "Application créée: $PWD/build/macos/SGA Subtitler.app"
echo "DMG créé:         $PWD/$final_dmg"
