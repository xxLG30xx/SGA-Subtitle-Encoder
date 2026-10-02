# Guide utilisateur — SGA Subtitler V0.6

1. Cliquez sur **OUVRIR SGA** et choisissez une copie de votre fichier `.SGA`.
   Le profil, la résolution, l'audio et le niveau de support sont détectés
   automatiquement. Le nom du jeu n'est jamais utilisé comme liste blanche.
2. Utilisez **LECTURE / PAUSE** et la timeline pour parcourir la vidéo.
3. **EXTRAIRE WAV** crée une copie WAV de la piste A1 sans modifier le SGA.
4. Cliquez sur **CHARGER SRT**. Les blocs invalides, intervalles inversés ou
   textes vides sont refusés avec un message explicite.
5. Réglez la police, la taille, le gras, la position et le décalage vertical.
   L'aperçu est actualisé sur la frame courante; les lignes longues sont
   centrées et automatiquement repliées dans l'image.
6. Cliquez sur **CRÉER LE SGA SOUS-TITRÉ**. Le nom proposé est
   `NomOriginal_FR.SGA`; choisissez toujours un nouveau fichier.
7. Le succès indique la taille identique, le nombre de chunks modifiés, les
   octets différents et confirme `Audio modifié: 0`.

## Messages de sécurité

- **Codec/profil non pris en charge** : la lecture ou l'opération inverse de ce
  profil n'est pas assez validée. Aucun fichier n'est créé.
- **Ne tient pas dans l'allocation compressée** : la frame sous-titrée dépasse
  son espace original malgré la recompression. Réduisez la taille/le contour ou
  raccourcissez le texte. L'opération échoue avant toute création partielle.
- **Le fichier existe déjà** ou **source ne peut jamais être écrasée** : choisissez
  un nouveau nom. SGA Subtitler utilise une création exclusive.
- **Aucune frame ne correspond** : les timecodes SRT sont hors de la durée SGA.

C1, C8, C9, CD et E8/E9 sont disponibles de bout en bout dans V0.6. Les autres
profils sont signalés selon la matrice de support intégrée et échouent proprement
lorsqu'une opération non validée est demandée.
