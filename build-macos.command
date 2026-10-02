#!/bin/bash
set -euo pipefail

cd "$(dirname "$0")"

if [[ "$(uname -s)" != "Darwin" ]]; then
  echo "Erreur: ce script doit être exécuté sous macOS."
  exit 1
fi

if ! command -v /usr/libexec/java_home >/dev/null 2>&1; then
  echo "Erreur: impossible de localiser un JDK macOS. Installez un JDK 21."
  exit 1
fi

JAVA_HOME="$(/usr/libexec/java_home -v 21 2>/dev/null || true)"
if [[ -z "$JAVA_HOME" || ! -x "$JAVA_HOME/bin/java" ]]; then
  echo "Erreur: JDK 21 introuvable. Installez un JDK 21 complet."
  exit 1
fi
export JAVA_HOME
export PATH="$JAVA_HOME/bin:$PATH"

java_major="$(java -version 2>&1 | awk -F '[\".]' '/version/ {print $2; exit}')"
if [[ "$java_major" != "21" ]]; then
  echo "Erreur: Java 21 est requis; version détectée: ${java_major:-inconnue}."
  exit 1
fi

if ! command -v jpackage >/dev/null 2>&1; then
  echo "Erreur: jpackage est absent de ce JDK 21."
  exit 1
fi

chmod +x ./gradlew
echo "Construction de SGA Subtitler V0.6 avec ${JAVA_HOME}..."
./gradlew clean packageMac

app="$PWD/build/macos/SGA Subtitler.app"
dmg="$PWD/build/macos/SGA Subtitler V0.6.dmg"
[[ -d "$app" ]] || { echo "Erreur: application non produite: $app"; exit 1; }
[[ -f "$dmg" ]] || { echo "Erreur: DMG non produit: $dmg"; exit 1; }

echo
echo "BUILD MACOS TERMINÉ"
echo "Application: $app"
echo "DMG:         $dmg"
echo
if [[ -t 0 ]]; then read -r -p "Appuyez sur Entrée pour fermer…" _; fi
