Public Class frmLZCompression

    Dim _compressedData, _decompressedData As Byte()

    Private Sub btnDecompress_Click(sender As Object, e As EventArgs) Handles btnDecompress.Click
        Dim compressedDataAsString As String _
            = txtCompressed.Text

        'Strip out all spaces and line breaks
        compressedDataAsString = compressedDataAsString.Replace(" ", "")
        compressedDataAsString = compressedDataAsString.Replace(vbCrLf, "")

        'Parse the string into bytes
        Dim dataBytes As New List(Of Byte)(compressedDataAsString.Length \ 2)

        For i As Integer = 0 To compressedDataAsString.Length - 1 Step 2
            Dim dataByteAsString = compressedDataAsString.Substring(i, 2)
            Dim dataByte = Convert.ToByte(dataByteAsString, 16)
            dataBytes.Add(dataByte)
        Next

        _compressedData = dataBytes.ToArray

        Dim lzcData As New LZCompressedData(_compressedData, 3, 1)

        _decompressedData = lzcData.DecompressedData

        With txtDecompressed
            .SuspendLayout()
            .Clear()
            .Text = DataToString(_decompressedData, 0)
            .ResumeLayout()
        End With

        btnGenerateBitmap.Enabled = True

    End Sub

    Private Sub btnGenerateBitmap_Click(sender As Object, e As EventArgs) Handles btnGenerateBitmap.Click
        Dim sizeInTiles As New Size(nudWidth.Value, nudHeight.Value)

        Dim tileMapDataStartOffset As Integer = 0
        Dim tileMapDataLength As Integer _
            = sizeInTiles.Width * sizeInTiles.Height * 2
        Dim tileMapData(tileMapDataLength - 1) As Byte
        Array.Copy(_decompressedData, 0, tileMapData, tileMapDataStartOffset, tileMapDataLength)

        Dim tileMapLength As Integer _
            = sizeInTiles.Width * sizeInTiles.Height
        Dim tileMap As New List(Of UShort)(tileMapLength)

        For i As Integer = 0 To tileMapDataLength - 1 Step 2
            Dim bytes As Byte() _
                = {tileMapData(i), tileMapData(i + 1)}
            Array.Reverse(bytes)
            tileMap.Add(BitConverter.ToUInt16(bytes, 0))
        Next

        Dim uniqueTileCount As Integer = nudUniqueTiles.Value

        Dim tileDataStartOffset As Integer = tileMapDataStartOffset + tileMapDataLength
        Dim tileDataLength As Integer _
            = uniqueTileCount * 128
        Dim tileData(tileDataLength - 1) As Byte
        Array.Copy(_decompressedData, tileDataStartOffset, tileData, 0, tileDataLength)

        Dim tileBitmaps As New List(Of Bitmap)(uniqueTileCount)

        Dim tileBaseOffset As Integer = uniqueTileCount * 2

        For tileDataOffset As Integer = 0 To tileDataLength - 1 Step 128
            Dim pixelBytes(127) As Byte
            Array.Copy(tileData, tileDataOffset, pixelBytes, 0, 128)

            Dim pixelsAsRGB555 As New List(Of Byte)(128)
            Dim pixelsAsRGB888 As New List(Of Byte)(196)

            For pixelOffset As Integer = 0 To pixelBytes.Length - 1 Step 2
                Dim bytes As Byte() _
                    = {pixelBytes(pixelOffset), pixelBytes(pixelOffset + 1)}

                Array.Reverse(bytes)

                pixelsAsRGB555.AddRange(bytes)

                Dim pixelAsRGB555 As UShort _
                    = BitConverter.ToUInt16(bytes, 0)

                Dim RGB888(2) As Byte
                For rgbIndex As Integer = 0 To 2
                    RGB888(rgbIndex) = ((pixelAsRGB555 >> (5 * (2 - rgbIndex))) And &H1F) << 3
                Next
                Dim color As Color = Color.FromArgb(RGB888(0), RGB888(1), RGB888(2))
                pixelsAsRGB888.AddRange({color.R, color.G, color.B})

            Next

            tileBitmaps.Add(CreateBitmapFromPixels(pixelsAsRGB888.ToArray, New Size(8, 8), Imaging.PixelFormat.Format24bppRgb))
            'tileBitmaps.Add(CreateBitmapFromPixels(pixelsAsRGB555.ToArray, New Size(8, 8), Imaging.PixelFormat.Format16bppRgb555))

        Next

        Dim sizeInPixels As New Size(sizeInTiles.Width * 8, sizeInTiles.Height * 8)

        Using b As New Bitmap(sizeInPixels.Width, sizeInPixels.Height, Imaging.PixelFormat.Format24bppRgb)
            Using g As Graphics = Graphics.FromImage(b)

                For row As Integer = 0 To sizeInTiles.Height - 1
                    For col As Integer = 0 To sizeInTiles.Width - 1

                        Dim tileMapIndex As Integer _
                            = (row * sizeInTiles.Width) + col

                        Dim tileIndex As Integer _
                            = tileMap(tileMapIndex) - 1

                        Dim drawPos As New Point(col * 8, row * 8)

                        g.DrawImage(tileBitmaps(tileIndex), drawPos)

                    Next
                Next

            End Using

            PictureBox1.Image = b.Clone

            Clipboard.SetImage(PictureBox1.Image)

        End Using



    End Sub

    Private Function CreateBitmapFromPixels(ByRef pixels As Byte(), ByVal size As Size, ByVal pixelFormat As Imaging.PixelFormat)

        Dim b As New Bitmap(size.Width, size.Height, pixelFormat)

        Dim bd As Imaging.BitmapData =
            b.LockBits(New Rectangle(0, 0, b.Width, b.Height),
            Imaging.ImageLockMode.ReadWrite,
            b.PixelFormat)

        Runtime.InteropServices.Marshal.Copy(pixels, 0, bd.Scan0, pixels.Length)

        b.UnlockBits(bd)

        Return b

    End Function

End Class