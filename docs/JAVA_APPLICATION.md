# SGA Subtitler V0.6 — application Java

## Fonctions livrées

L'application Swing autonome ouvre et analyse les quatre profils réels, affiche
les métadonnées, lit les frames C1/C8/C9/CD/E8/E9 dans une timeline, superpose un SRT réel,
permet de sélectionner une famille Java portable, la taille, le gras et la
position, extrait A1 en WAV et crée un nouveau SGA C1/C8/C9/CD/E8/E9 fixed-size. Le choix de
sortie propose `<Nom>_FR.SGA` et refuse le chemin source ou un fichier existant.

Les contrôles avant écriture refusent un profil hors de la famille de tuiles validée, un SRT vide, une
absence de frames concernées, un payload de taille différente, un changement
d'audio ou un écrasement. Les familles C8/C9/CD sont décompactées, rendues,
modifiées, recompressées avec leurs paramètres SCAT puis réinjectées uniquement
si le flux tient dans l'allocation d'origine. E8/E9 est décodé avec sa chaîne interframe puis réencodé par groupes de commandes SCAT. Seule la chaîne minimale allant jusqu'à la prochaine keyframe E8 est recalculée; chaque payload doit tenir dans son allocation ou l'opération échoue.

## Matrice de support

| Profils | Connu SCAT | Moteur Java | Pixels | Fixture réelle | Sous-titrage fixed-size |
|---|---|---|---|---|---|
| C1 | oui | oui | validés | Night Trap | validé |
| C8 | oui | oui, LZ base 0 + permutation | validés | Sewer Shark | validé |
| C9 | oui | oui, LZ base 1 + permutation | validés | Double Switch | validé |
| CD | oui | oui, fenêtre 4K + permutation | validés | Double Switch | validé |
| E8/E9 | oui | décodeur et encodeur interframe SCAT | validés | Ground Zero Texas | validé |
| C6/C7/CB | oui | codec tuiles/LZ implémenté | tests algorithmiques | aucune | non validé sur vrai SGA |
| C2/C4, D1–D5/D7, E7, 81/82/8A | oui | registre et paramètres SCAT | non validés | aucune | non supporté |

Le registre est fondé sur les chunks et leurs paramètres, non sur des classes
portant le nom d'un jeu. Une base séparée associe les familles connues aux jeux
documentés par SCAT uniquement pour l'identification et l'affichage.

## Lancer et construire

- `gradle run` lance l'application depuis les sources.
- `gradle fatJar` produit `build/libs/SGA-Subtitler-V0.6.jar`.
- Sur macOS, `gradle packageMac` appelle `jpackage` et produit une application
  `.app` puis un `.dmg` avec runtime Java embarqué dans `build/macos/`.

Le bundle macOS doit être construit sur macOS, car `jpackage` ne réalise pas de
cross-packaging. L'utilisateur du `.app`/`.dmg` n'a besoin ni de Python, ni du
Terminal, ni d'une installation Java séparée.

## BUILD MACOS

Prérequis développeur :

- macOS ;
- un JDK 21 complet, incluant `jpackage`.

Depuis la racine du dépôt :

```sh
./gradlew packageMac
```

Résultat :

```text
build/macos/SGA Subtitler.app
build/macos/SGA Subtitler V0.6.dmg
```

Le Gradle Wrapper est inclus : aucune installation système de Gradle n'est
nécessaire. Le runtime Java est intégré dans l'application par `jpackage`, de
sorte que l'utilisateur final n'a pas besoin d'installer Java.

Il est également possible de double-cliquer sur `build-macos.command`. Ce
script vérifie macOS, sélectionne le JDK 21, contrôle `jpackage`, lance le
Wrapper, puis affiche les chemins exacts de l'application et du DMG produits.
