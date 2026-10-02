Imports System.Windows.Media.Imaging

Public Class SGATileViewerControl

    Public Property TileSize As Size = New Size(32, 32)
    Public Property TilesPerRow As Integer = 8

    Private _VDP As Sega.VDP
    Public Property VDP As Sega.VDP
        Get
            Return _VDP
        End Get
        Set(value As Sega.VDP)
            _VDP = value
            'InitializeDisplay()
            UpdateDisplay()
        End Set
    End Property

    Public Sub InitializeDisplay()
        SuspendLayout()
        pbxDisplay.Image = Nothing
        ResumeLayout()
    End Sub

    Public Sub UpdateDisplay()
        If VDP Is Nothing Then Exit Sub
        If TypeOf (VDP) Is Sega.MegaDrive.VDP Then
            SuspendLayout()
            pbxDisplay.Image = TilesAsBitmap()
            ResumeLayout()
        End If
    End Sub

    Private _paletteIndex As Integer = 0

    Private Function TilesAsBitmap() As Bitmap
        Dim MDV As Sega.MegaDrive.VDP _
            = VDP
        If MDV.TileData Is Nothing Then Return Nothing

        Dim sourceRect As New Windows.Int32Rect(0, 0, 8, 8) 'TileSize.Width, TileSize.Height)
        Dim bSize As New Size(sourceRect.Width * TilesPerRow, Math.Ceiling(MDV.TileCount / TilesPerRow) * sourceRect.Height)
        'Dim sourceRect As New Rectangle(0, 0, 8, 8)
        'Dim bSize As New Size(sourceRect.Width * TilesPerRow, Math.Ceiling(MDV.TileCount / TilesPerRow) * sourceRect.Height)

        Dim bp As New BitmapPalette(MDV.PaletteAsWindowsMediaColors)
        Dim image As New WriteableBitmap(bSize.Width, bSize.Height, 96, 96, Windows.Media.PixelFormats.Indexed8, bp) 'Windows.Media.PixelFormats.Bgr24, _bitmapPalette)
        'Dim image As New Bitmap(bSize.Width, bSize.Height, PixelFormat.Format24bppRgb)

        image.Lock()

        'Dim bd As BitmapData =
        '    image.LockBits(New Rectangle(0, 0, image.Width, image.Height),
        '    ImageLockMode.ReadWrite,
        '    image.PixelFormat)

        'image.UnlockBits(bd)

        'Dim startIndex As Integer = 0

        For i As Integer = 0 To MDV.TileCount - 1
            If MDV.RawTiles(i) IsNot Nothing Then
                Dim tile As Sega.MegaDrive.VDP.Tile _
                    = New Sega.MegaDrive.VDP.Tile(MDV.RawTiles(i), New Sega.MegaDrive.VDP.TileMapEntry(False, _paletteIndex, False, False, i))

                Dim destination As New Point(
                    sourceRect.Width * (i Mod TilesPerRow),
                    sourceRect.Height * (i \ TilesPerRow)
                    )

                Dim sourceBuffer As Byte() _
                    = tile.ToPixels

                image.WritePixels(sourceRect, sourceBuffer, sourceRect.Width, destination.X, destination.Y)

                'Runtime.InteropServices.Marshal.Copy(source, startIndex, bd.Scan0, source.Length)

                'startIndex += source.Length
            End If
        Next

        image.Unlock()

        Using b As New Bitmap(bSize.Width * 4, bSize.Height * 4, Imaging.PixelFormat.Format24bppRgb)

            Using g As Graphics = Graphics.FromImage(b)
                With g
                    .CompositingQuality = Drawing2D.CompositingQuality.HighSpeed
                    .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                    .SmoothingMode = Drawing2D.SmoothingMode.HighSpeed

                    .ScaleTransform(4, 4)

                    .DrawImage(image.tobitmap, 0, 0)
                End With
            End Using

            Return b.Clone

        End Using

        'Dim bitmapWidth As Integer _
        '    = TileSize.Width * TilesPerRow

        'Dim rect As New Rectangle(0, 0, TileSize.Width, TileSize.Height)
        'Dim bSize As New Size(bitmapWidth, Math.Ceiling(TileCount / TilesPerRow) * rect.Height)

        'Dim tile As Sega.MegaDrive.VDP.Tile

        'Using b As New Bitmap(bSize.Width, bSize.Height, Imaging.PixelFormat.Format32bppArgb)
        '    Using g As Graphics = Graphics.FromImage(b)
        '        With g
        '            .CompositingQuality = Drawing2D.CompositingQuality.HighSpeed
        '            .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
        '            .SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
        '        End With

        '        For i As Integer = 0 To MDV.TileCount - 1
        '            If MDV.RawTiles(i) IsNot Nothing Then
        '                tile = New Sega.MegaDrive.VDP.Tile(MDV.RawTiles(i), New Sega.MegaDrive.VDP.TileMapEntry(False, _paletteIndex, False, False, i))
        '                With rect
        '                    .X = .Width * (i Mod TilesPerRow)
        '                    .Y = .Height * (i \ TilesPerRow)
        '                End With
        '                g.DrawImage(tile.ToBitmap(MDV.PaletteAsColors), rect)
        '            End If
        '        Next
        '    End Using

        '    Return b.Clone

        'End Using

    End Function

    Private _tileIndex, _oldTileIndex As Integer
    Private Sub pbxDisplay_MouseMove(sender As Object, e As MouseEventArgs) Handles pbxDisplay.MouseMove
        If VDP Is Nothing Then Exit Sub

        If TypeOf (VDP) Is Sega.MegaDrive.VDP Then
            Dim MDV As Sega.MegaDrive.VDP _
                = VDP

            _oldTileIndex = _tileIndex
            _tileIndex = ((e.Y \ 32) * 8) + (e.X \ 32)

            If _tileIndex <> _oldTileIndex Then
                With ToolTipMain

                    If _tileIndex >= MDV.TileCount Then
                        .RemoveAll()
                        .SetToolTip(pbxDisplay, "")
                    Else
                        .ToolTipTitle = $"Tile ${_tileIndex:X2}"
                        .SetToolTip(pbxDisplay, "") '_tileIndex)
                    End If

                End With
            End If

        End If
    End Sub

    Private Sub pbxDisplay_Click(sender As Object, e As EventArgs) Handles pbxDisplay.Click
        _paletteIndex += 1
        If _paletteIndex > 3 Then _paletteIndex = 0
        UpdateDisplay()
    End Sub

End Class
