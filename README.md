# SGA Subtitle Encoder

V0.6 adds a command-line, preserve-first SGA analysis and fixed-size patching
core. The original WinForms SCAT source remains the codec reference; the new
code does not replace or modify source SGA files.

```sh
python -m sga_subtitler.cli audit *.SGA
python -m sga_subtitler.cli frames "Night Trap.SGA" artifacts/frames --count 3
python -m sga_subtitler.cli audio "Night Trap.SGA" artifacts/night-trap.wav
python -m sga_subtitler.cli test-patch "Night Trap.SGA" \
  "artifacts/Night Trap - subtitle test.SGA" --frames 3
```

See [`docs/REAL_SGA_AUDIT.md`](docs/REAL_SGA_AUDIT.md) for measured results,
scope, codec mapping, and the fixed-size validation report.

## Application Java V0.6

Le produit utilisateur est désormais l'application graphique Java située sous
`java/`. Elle offre ouverture/analyse SGA, aperçu et timeline C1, import SRT,
styles portables, extraction WAV et création C1/C8/C9/CD/E8/E9 fixed-size avec propagation minimale des interframes. Consultez
[`docs/JAVA_APPLICATION.md`](docs/JAVA_APPLICATION.md) pour le support exact et
le packaging `.app`/`.dmg` macOS avec runtime embarqué.

Le mode d'emploi final est disponible dans
[`docs/USER_GUIDE.md`](docs/USER_GUIDE.md). Les opérations longues utilisent
une progression non bloquante et toute impossibilité fixed-size échoue avant
l'écriture du fichier de sortie.

## Build macOS

Avec un JDK 21 installé sur macOS, exécutez `./gradlew packageMac` ou
double-cliquez sur `build-macos.command`. Le Gradle Wrapper est inclus et le
runtime Java est embarqué dans `build/macos/SGA Subtitler.app`. Le DMG final est
créé sous `build/macos/SGA Subtitler V0.6.dmg`.
