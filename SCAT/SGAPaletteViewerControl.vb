Public Class SGAPaletteViewerControl

    Public Property EntrySize As Size = New Size(16, 16)
    Public Property EntriesPerRow As Integer = 16

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
        SuspendLayout()
        UpdatePaletteBitmap()
        ResumeLayout()
    End Sub

    Private Sub UpdatePaletteBitmap()
        If VDP Is Nothing Then
            Exit Sub
        End If
        If VDP.CRAM Is Nothing Then
            Exit Sub
        End If

        Dim bitmapWidth As Integer _
            = EntrySize.Width * EntriesPerRow
        Dim rect As New Rectangle(0, 0, EntrySize.Width, EntrySize.Height)
        Dim bSize As New Size(bitmapWidth, Math.Ceiling(VDP.CRAM.Length / EntriesPerRow) * rect.Height)

        Using b As New Bitmap(bSize.Width, bSize.Height, Imaging.PixelFormat.Format32bppArgb)
            Dim brush As New SolidBrush(Color.Black)

            Using g As Graphics = Graphics.FromImage(b)
                With g
                    .CompositingQuality = Drawing2D.CompositingQuality.HighSpeed
                    .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                    .SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
                End With

                For i As Integer = 0 To VDP.CRAM.Length - 1
                    brush.Color = VDP.PaletteAsColors(i)
                    With rect
                        .X = .Width * (i Mod EntriesPerRow)
                        .Y = .Height * (i \ EntriesPerRow)
                    End With
                    g.FillRectangle(brush, rect)
                Next
            End Using

            pbxDisplay.Image = b.Clone

        End Using

    End Sub

    Private _paletteEntryIndex, _oldPaletteEntryIndex As Integer
    Private Sub pbxPalette_MouseMove(sender As Object, e As MouseEventArgs) Handles pbxDisplay.MouseMove
        If VDP Is Nothing Then
            Exit Sub
        End If

        _oldPaletteEntryIndex = _paletteEntryIndex
        _paletteEntryIndex = ((e.Y \ 16) * 16) + (e.X \ 16)

        If _paletteEntryIndex <> _oldPaletteEntryIndex Then
            With ToolTipMain
                If _paletteEntryIndex >= VDP.CRAM.Count Then
                    .RemoveAll()
                    .SetToolTip(pbxDisplay, "")
                Else
                    .ToolTipTitle = $"Palette entry ${_paletteEntryIndex:X2}"
                    Dim caption As String _
                        = $"CRAM [${VDP.CRAM(_paletteEntryIndex):X4}]{vbNewLine}{VDP.PaletteAsColors(_paletteEntryIndex).ToString()}"
                    .SetToolTip(pbxDisplay, caption)
                End If
            End With
        End If
    End Sub

End Class
