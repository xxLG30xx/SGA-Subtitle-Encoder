Imports System.Runtime.CompilerServices
Imports System.Windows.Media.Imaging

Module ArrayExtension
    'Array extension methods
    <Extension()>
    Public Function SwapBytes(ByRef a As Byte()) As Byte()
        Dim b As Byte() = a.Clone
        For i As Integer = 0 To b.Length - 1 Step 2
            Array.Reverse(b, i, 2)
        Next
        Return b
    End Function

    <Extension()>
    Public Function Flatten(ByRef a As Byte(,)) As Byte()
        Dim size As New Size(a.GetUpperBound(0) + 1, a.GetUpperBound(1) + 1)

        Dim b(size.Height * size.Width - 1) As Byte

        For y As Integer = 0 To size.Height - 1
            For x As Integer = 0 To size.Width - 1
                Dim index As Integer _
                    = (y * size.Width) + x
                b(index) = a(x, y)
            Next
        Next

        Return b
    End Function

    <Extension()>
    Public Function Articulate(ByRef a As Byte(), ByVal size As Size) As Byte(,)
        Dim b(size.Width - 1, size.Height - 1) As Byte

        For i As Integer = 0 To a.Length - 1
            Dim index As New Point(i Mod size.Width, i \ size.Width)
            b(index.X, index.Y) = a(i)
        Next

        Return b
    End Function

End Module

Module BinaryReaderWriterExtension
    'BinaryReader extension methods
    <Extension()>
    Public Function ReadBigEndianUInt16(ByRef br As IO.BinaryReader) As UShort
        Dim beBytes As Byte() _
            = br.ReadBytes(2)
        Array.Reverse(beBytes)

        If beBytes.Length = 0 Then Return 0

        Return BitConverter.ToUInt16(beBytes, 0)
    End Function

    <Extension()>
    Public Function ReadBigEndianUInt32(ByRef br As IO.BinaryReader) As UInteger
        Dim beBytes As Byte() _
            = br.ReadBytes(4)
        Array.Reverse(beBytes)

        If beBytes.Length = 0 Then Return 0

        Return BitConverter.ToUInt32(beBytes, 0)
    End Function

    <Extension()>
    Public Function ReadBigEndianUInt64(ByRef br As IO.BinaryReader) As ULong
        Dim beBytes As Byte() _
            = br.ReadBytes(8)
        Array.Reverse(beBytes)

        If beBytes.Length = 0 Then Return 0

        Return BitConverter.ToUInt64(beBytes, 0)
    End Function

    <Extension()>
    Public Sub AlignNextRead(ByRef br As IO.BinaryReader, ByVal sizeInBytes As Integer)
        Dim stream As IO.Stream _
            = br.BaseStream
        If stream.Position Mod sizeInBytes <> 0 Then
            stream.Seek(stream.Position Mod sizeInBytes, IO.SeekOrigin.Current)
        End If
    End Sub

    <Extension()>
    Public Function IsAtStartOfCDROMSector(ByRef br As IO.BinaryReader) As Boolean
        If br.BaseStream.Position Mod CD_ROM_MODE1_BYTES_PER_SECTOR = 0 Then
            Return True
        End If
        Return False
    End Function

    <Extension()>
    Public Sub SkipToNextCDROMSector(ByRef br As IO.BinaryReader)
        With br.BaseStream
            Dim nextSectorOffset As Long
            If (.Position Mod CD_ROM_MODE1_BYTES_PER_SECTOR) = 0 Then
                nextSectorOffset = 0
            Else
                nextSectorOffset = CD_ROM_MODE1_BYTES_PER_SECTOR - (.Position Mod CD_ROM_MODE1_BYTES_PER_SECTOR)
            End If
            .Seek(nextSectorOffset, IO.SeekOrigin.Current)
        End With
    End Sub

    'BinaryWriter extension methods
    <Extension()>
    Public Sub WriteBigEndianUInt16(ByRef bw As IO.BinaryWriter, ByVal value As UShort)
        Dim beValue As Byte() _
            = BitConverter.GetBytes(value)
        Array.Reverse(beValue)
        bw.Write(beValue)
    End Sub

    <Extension()>
    Public Sub WriteBigEndianUInt32(ByRef bw As IO.BinaryWriter, ByVal value As UInteger)
        Dim beValue As Byte() _
            = BitConverter.GetBytes(value)
        Array.Reverse(beValue)
        bw.Write(beValue)
    End Sub

End Module

Module BitConverterExtension
    'BitConverter extension methods
    <Extension()>
    Public Function ReverseEndianness(ByRef bc As BitConverter, ByVal value As UShort) As UShort
        Dim bytes As Byte() _
            = BitConverter.GetBytes(value)
        Array.Reverse(bytes)
        Return BitConverter.ToUInt16(bytes, 0)
    End Function

    <Extension()>
    Public Function ReverseEndianness(ByRef bc As BitConverter, ByVal value As UInteger) As UInteger
        Dim bytes As Byte() _
            = BitConverter.GetBytes(value)
        Array.Reverse(bytes)
        Return BitConverter.ToUInt32(bytes, 0)
    End Function

End Module

Module BitmapSourceExtension

    <Extension()>
    Public Function ToBitmap(ByRef bs As BitmapSource) As Bitmap
        Using ms As New IO.MemoryStream
            Dim enc As New BmpBitmapEncoder
            With enc
                .Frames.Add(BitmapFrame.Create(bs))
                .Save(ms)
            End With
            Using b As New Bitmap(ms)
                Return b.Clone
            End Using
        End Using
    End Function

End Module

Module BitmapExtension

    <Extension()>
    Public Function ToBitmapSource(ByRef b As Bitmap) As BitmapSource
        Using ms As New IO.MemoryStream
            b.Save(ms, Imaging.ImageFormat.Bmp)

            Dim createOptions As BitmapCreateOptions _
                = BitmapCreateOptions.PreservePixelFormat
            Dim cacheOption As BitmapCacheOption _
                = BitmapCacheOption.Default

            Dim dec As New BmpBitmapDecoder(ms, createOptions, cacheOption)
            Dim bf As BitmapFrame _
                = dec.Frames(0)
            Return bf.Clone
        End Using

        'Dim pixels As Byte() _
        '        = FrameBufferAsPixelData()

        'Dim frameBufferBD As Imaging.BitmapData =
        '    _frameBufferBitmap.LockBits(New Rectangle(0, 0, _frameBufferBitmap.Width, _frameBufferBitmap.Height),
        '    Imaging.ImageLockMode.ReadWrite,
        '    _frameBufferPixelFormat)

        'Runtime.InteropServices.Marshal.Copy(pixels, 0, frameBufferBD.Scan0, pixels.Length)

        '_frameBufferBitmap.UnlockBits(frameBufferBD)

        'Return _frameBufferBitmap

    End Function

    <Extension()>
    Public Function GetPixelData(ByRef b As Bitmap) As Byte()
        Dim bd As Imaging.BitmapData =
            b.LockBits(New Rectangle(0, 0, b.Width, b.Height), Imaging.ImageLockMode.ReadOnly, b.PixelFormat)

        'Copy bitmap data into an array
        Dim pixelDataLength As Integer = b.Width * b.Height * 3
        Dim pixelData(pixelDataLength - 1) As Byte
        Runtime.InteropServices.Marshal.Copy(bd.Scan0, pixelData, 0, pixelDataLength)

        b.UnlockBits(bd)

        Return pixelData
    End Function

    <Extension()>
    Public Sub SetPixelData(ByRef b As Bitmap, ByVal pixelData As Byte())
        Dim bd As Imaging.BitmapData =
            b.LockBits(New Rectangle(0, 0, b.Width, b.Height), Imaging.ImageLockMode.WriteOnly, b.PixelFormat)

        Runtime.InteropServices.Marshal.Copy(pixelData, 0, bd.Scan0, pixelData.Length)

        b.UnlockBits(bd)
    End Sub

    <Extension()>
    Public Sub Undither2x2(ByRef b As Bitmap)
        Dim bSize As Size = b.Size
        Dim bStride As Integer = bSize.Width * 3

        Dim pixelData As Byte() = b.GetPixelData

        'Convert pixel data to color matrix
        Dim srcColorMatrix(bSize.Width - 1, bSize.Height - 1) As Color
        Parallel.For(0, bSize.Height,
            Sub(y As Integer)
                For x As Integer = 0 To bSize.Width - 1
                    Dim index As Integer = y * bStride + x * 3
                    srcColorMatrix(x, y) = Color.FromArgb(pixelData(index + 2), pixelData(index + 1), pixelData(index))
                Next
            End Sub)

        Dim dstColorMatrix As Color(,) = srcColorMatrix.Clone

        'Use flag array to determine which pixels should be dithered
        Dim flags(bSize.Width - 1, bSize.Height - 1) As Integer

        Parallel.For(0, bSize.Height - 1,
             Sub(y As Integer)
                 For x As Integer = 0 To bSize.Width - 2

                     'Get 2x2 matrix of colors; reference color is (0,0)
                     Dim colorMatrix2x2 As New List(Of Color)
                     For q As Integer = 0 To 1
                         For p As Integer = 0 To 1
                             colorMatrix2x2.Add(srcColorMatrix(x + p, y + q))
                         Next
                     Next

                     'If colorMatrix2x2.Distinct.Count <= 3 Then

                     Dim red, green, blue As Integer
                     For i As Integer = 0 To 3
                         red += colorMatrix2x2(i).R  '* If(i = 0, 2, 1)
                         green += colorMatrix2x2(i).G '* If(i = 0, 2, 1)
                         blue += colorMatrix2x2(i).B '* If(i = 0, 2, 1)
                     Next

                     red = Math.Min(red \ 4, 255)
                     green = Math.Min(green \ 4, 255)
                     blue = Math.Min(blue \ 4, 255)

                     'red = Math.Min(red \ 5, 255)
                     'green = Math.Min(green \ 5, 255)
                     'blue = Math.Min(blue \ 5, 255)

                     Dim newColor As Color _
                            = Color.FromArgb(red, green, blue)
                     dstColorMatrix(x, y) = newColor

                     'End If

                 Next

             End Sub)

        'Convert destination color matrix to destination pixel data
        Parallel.For(0, bSize.Height,
            Sub(y As Integer)
                For x As Integer = 0 To bSize.Width - 1
                    Dim index As Integer = y * bStride + x * 3
                    Array.Copy({dstColorMatrix(x, y).B, dstColorMatrix(x, y).G, dstColorMatrix(x, y).R}, 0, pixelData, index, 3)
                Next
            End Sub)

        b.SetPixelData(pixelData)

    End Sub

    <Extension()>
    Public Sub Undither2x2v2(ByRef b As Bitmap)
        Dim bSize As Size = b.Size
        Dim bStride As Integer = bSize.Width * 3

        Dim pixelData As Byte() = b.GetPixelData

        'Convert pixel data to color matrix
        Dim srcColorMatrix(bSize.Width - 1, bSize.Height - 1) As Color
        Parallel.For(0, bSize.Height,
            Sub(y As Integer)
                For x As Integer = 0 To bSize.Width - 1
                    Dim index As Integer = y * bStride + x * 3
                    srcColorMatrix(x, y) = Color.FromArgb(pixelData(index + 2), pixelData(index + 1), pixelData(index))
                Next
            End Sub)

        Dim dstColorMatrix As Color(,) = srcColorMatrix.Clone

        'Use flag array to determine which pixels should be dithered
        Dim flags(bSize.Width - 1, bSize.Height - 1) As Integer

        Parallel.For(0, bSize.Height - 1,
             Sub(y As Integer)
                 For x As Integer = 0 To bSize.Width - 2

                     'Get 2x2 matrix of colors; reference color is (0,0)
                     Dim pixelMatrix(1, 1) As Color
                     For q As Integer = 0 To 1
                         For p As Integer = 0 To 1
                             pixelMatrix(p, q) = srcColorMatrix(x + p, y + q)
                         Next
                     Next

                     'Calculate similarities to reference pixel as well as average similarity
                     Dim similarity(1, 1) As Single
                     Dim avgSimilarity As Single = 0
                     For q As Integer = 0 To 1
                         For p As Integer = 0 To 1
                             similarity(p, q) = pixelMatrix(p, q).SimilarityTo(pixelMatrix(0, 0))
                             avgSimilarity += similarity(p, q)
                         Next
                     Next
                     avgSimilarity /= 4

                     Dim score As Integer = 0
                     For q As Integer = 0 To 1
                         For p As Integer = 0 To 1
                             If similarity(p, q) < 1 Then
                                 If similarity(p, q) <= avgSimilarity Then
                                     score += 1
                                 End If
                             End If
                         Next
                     Next

                     If score >= 2 Then

                         'If pixelMatrix(0, 0).IsSimilarTo(pixelMatrix(1, 1)) And pixelMatrix(1, 0).IsSimilarTo(pixelMatrix(0, 1)) Then

                         Dim weights As Single(,) _
                            = {{1, 1},
                               {1, 1}}

                         'Adjust weights
                         Dim scale As Single = 0
                         For q As Integer = 0 To 1
                             For p As Integer = 0 To 1
                                 weights(p, q) += 1 - similarity(p, q)
                                 scale += weights(p, q)
                             Next
                         Next
                         If scale = 0 Then Continue For

                         Dim RGB(2) As Single
                         For q As Integer = 0 To 1
                             For p As Integer = 0 To 1
                                 If weights(p, q) <> 0 Then
                                     Dim color As Single() _
                                        = {pixelMatrix(p, q).R, pixelMatrix(p, q).G, pixelMatrix(p, q).B}
                                     For i As Integer = 0 To 2
                                         RGB(i) += color(i) * weights(p, q)
                                     Next
                                 End If
                             Next
                         Next

                         For i As Integer = 0 To 2
                             RGB(i) = Math.Min(RGB(i) / scale, 255)
                         Next

                         Dim newColor As Color _
                            = Color.FromArgb(RGB(0), RGB(1), RGB(2))
                         dstColorMatrix(x, y) = newColor

                         'End If
                     End If

                 Next

             End Sub)

        'Convert destination color matrix to destination pixel data
        Parallel.For(0, bSize.Height,
            Sub(y As Integer)
                For x As Integer = 0 To bSize.Width - 1
                    Dim index As Integer = y * bStride + x * 3
                    Array.Copy({dstColorMatrix(x, y).B, dstColorMatrix(x, y).G, dstColorMatrix(x, y).R}, 0, pixelData, index, 3)
                Next
            End Sub)

        b.SetPixelData(pixelData)

    End Sub

    <Extension()>
    Public Sub Undither3x3(ByRef b As Bitmap)
        Dim bSize As Size = b.Size
        Dim bStride As Integer = bSize.Width * 3

        Dim pixelData As Byte() = b.GetPixelData

        'Convert pixel data to color matrix
        Dim srcColorMatrix(bSize.Width - 1, bSize.Height - 1) As Color
        Parallel.For(0, bSize.Height,
            Sub(y As Integer)
                For x As Integer = 0 To bSize.Width - 1
                    Dim index As Integer = y * bStride + x * 3
                    srcColorMatrix(x, y) = Color.FromArgb(pixelData(index + 2), pixelData(index + 1), pixelData(index))
                Next
            End Sub)

        Dim dstColorMatrix As Color(,) = srcColorMatrix.Clone

        'Use flag array to determine which pixels should be dithered
        Dim flags(bSize.Width - 1, bSize.Height - 1) As Integer

        '1st pass:
        'Compare each pixel with its cardinal neighbors
        'Flag the pixel for dithering if at least three neighbors differ from the reference pixel and also match each other
        Parallel.For(1, bSize.Height - 1,
             Sub(y As Integer)
                 'For y As Integer = 1 To bSize.Height - 2
                 For x As Integer = 1 To bSize.Width - 2

                     Dim pixel As Color _
                         = srcColorMatrix(x, y)

                     Dim cards As Color() _
                         = {srcColorMatrix(x - 1, y), srcColorMatrix(x + 1, y),
                            srcColorMatrix(x, y - 1), srcColorMatrix(x, y + 1)}

                     'Calculate similarities to reference pixel as well as average similarity
                     Dim similarity(3) As Single
                     Dim avgSimilarity As Single = 0
                     For i As Integer = 0 To 3
                         similarity(i) = cards(i).SimilarityTo(pixel)
                         avgSimilarity += similarity(i)
                     Next
                     avgSimilarity /= 4

                     Dim score As Integer = 0
                     For i As Integer = 0 To 3
                         If similarity(i) < 1 Then
                             If similarity(i) <= avgSimilarity Then
                                 score += 1
                             End If
                         End If
                     Next

                     If score >= 3 Then
                         flags(x, y) = 1
                     End If

                 Next

                 'Next
             End Sub)

        '2nd pass:
        'Compare each unflagged pixel to its eight neighbors
        'Flag the pixel for dithering if at least five of its neighbors are flagged
        Dim newPixelsFlagged As Integer
        Do
            newPixelsFlagged = 0
            For y As Integer = 1 To bSize.Height - 2
                For x As Integer = 1 To bSize.Width - 2

                    If flags(x, y) = 0 Then

                        'Count up flagged neighbors
                        Dim score As Integer = 0
                        For q As Integer = -1 To 1
                            For p As Integer = -1 To 1
                                score += If(flags(x + p, y + q) = 0, 0, 1)
                            Next
                        Next

                        If score >= 5 Then
                            flags(x, y) = 1
                            newPixelsFlagged += 1
                        End If

                    End If
                Next
            Next
        Loop Until newPixelsFlagged = 0

        ''3rd pass:
        ''Handle all stragglers
        'Parallel.For(1, bSize.Height - 2,
        ' Sub(y As Integer)
        '     'For y As Integer = 1 To bSize.Height - 2
        '     For x As Integer = 2 To bSize.Width - 3

        '         If flags(x, y) = 0 Then

        '             Dim pixel As Color _
        '                 = srcColorMatrix(x, y)

        '             Dim cards As Color() _
        '                 = {srcColorMatrix(x - 1, y), srcColorMatrix(x + 1, y),
        '                    srcColorMatrix(x, y - 1), srcColorMatrix(x, y + 1)}

        '             'If (cards(0) <> pixel) And (cards(0) = cards(1)) Then
        '             '    flags(x, y) += 1
        '             'End If
        '             'If (cards(2) <> pixel) And (cards(2) = cards(3)) Then
        '             '    flags(x, y) += 2
        '             'End If

        '             If (cards(0) <> pixel) And (cards(0).IsSimilarTo(cards(1))) Then
        '                 flags(x, y) = 1
        '             End If
        '             If (cards(2) <> pixel) And (cards(2).IsSimilarTo(cards(3))) Then
        '                 flags(x, y) = 1
        '             End If

        '         End If
        '     Next
        '     'Next
        ' End Sub)

        '4th pass:
        'Undithering
        Parallel.For(1, bSize.Height - 1,
            Sub(y As Integer)
                'For y As Integer = 1 To bSize.Height - 2
                For x As Integer = 1 To bSize.Width - 2

                    If flags(x, y) > 0 Then

                        'Get 3x3 matrix of pixels; reference pixel is (1,1)
                        Dim pixelMatrix(2, 2) As Color
                        For q As Integer = -1 To 1
                            For p As Integer = -1 To 1
                                pixelMatrix(p + 1, q + 1) = srcColorMatrix(x + p, y + q)
                            Next
                        Next

                        'Dim weights As Single(,) _
                        '    = {{1, 1, 1},
                        '       {1, 1, 1},
                        '       {1, 1, 1}}

                        Dim weights As Single(,) _
                            = {{0.0625, 0.125, 0.0625},
                               {0.125, 0.75, 0.125},
                               {0.0625, 0.125, 0.0625}}

                        ''Calculate similarities to reference pixel as well as average similarity
                        'Dim similarity(2, 2) As Single
                        'For q As Integer = 0 To 2
                        '    For p As Integer = 0 To 2
                        '        similarity(p, q) = pixelMatrix(p, q).SimilarityTo(pixelMatrix(1, 1))
                        '    Next
                        'Next

                        ''Adjust weights
                        'For q As Integer = -1 To 1
                        '    For p As Integer = -1 To 1
                        '        'weights(p + 1, q + 1) += 1 - similarity(p + 1, q + 1)
                        '        weights(p + 1, q + 1) *= flags(x + p, y + q)
                        '    Next
                        'Next

                        Dim scale As Single = 0
                        For q As Integer = 0 To 2
                            For p As Integer = 0 To 2
                                scale += weights(p, q)
                            Next
                        Next
                        If scale = 0 Then Continue For

                        Dim RGB As Single() = {0, 0, 0}
                        For q As Integer = 0 To 2
                            For p As Integer = 0 To 2
                                If weights(p, q) <> 0 Then
                                    Dim color As Integer() _
                                        = {pixelMatrix(p, q).R, pixelMatrix(p, q).G, pixelMatrix(p, q).B}
                                    For i As Integer = 0 To 2
                                        RGB(i) += color(i) * weights(p, q)
                                    Next
                                End If
                            Next
                        Next

                        For i As Integer = 0 To 2
                            RGB(i) = Math.Min(RGB(i) / scale, 255)
                        Next

                        Dim newColor As Color _
                            = Color.FromArgb(CByte(RGB(0)), CByte(RGB(1)), CByte(RGB(2)))
                        dstColorMatrix(x, y) = newColor

                    End If

                Next
                'Next
            End Sub)

        'Convert destination color matrix to destination pixel data
        Parallel.For(0, bSize.Height,
            Sub(y As Integer)
                For x As Integer = 0 To bSize.Width - 1
                    Dim index As Integer = y * bStride + x * 3
                    Array.Copy({dstColorMatrix(x, y).B, dstColorMatrix(x, y).G, dstColorMatrix(x, y).R}, 0, pixelData, index, 3)
                Next
            End Sub)

        b.SetPixelData(pixelData)

    End Sub

    Dim EdgeMasks3x3 As Integer(,,) _
        = {{{0, 1, 1}, {0, 1, 1}, {0, 1, 1}},
           {{0, 0, 0}, {1, 1, 1}, {1, 1, 1}},
           {{1, 1, 0}, {1, 1, 0}, {1, 1, 0}},
           {{1, 1, 1}, {1, 1, 1}, {0, 0, 0}},
           {{0, 1, 1}, {0, 1, 1}, {0, 0, 1}},
           {{1, 0, 0}, {1, 1, 0}, {1, 1, 0}},
           {{1, 1, 1}, {0, 1, 1}, {0, 0, 0}},
           {{1, 1, 1}, {1, 1, 0}, {0, 0, 0}},
           {{1, 1, 1}, {0, 1, 1}, {0, 0, 1}},
           {{1, 0, 0}, {1, 1, 0}, {1, 1, 1}},
           {{0, 0, 1}, {0, 1, 1}, {1, 1, 1}},
           {{1, 1, 1}, {1, 1, 0}, {1, 0, 0}},
           {{0, 0, 0}, {0, 1, 1}, {1, 1, 1}},
           {{0, 0, 0}, {1, 1, 0}, {1, 1, 1}},
           {{0, 0, 1}, {0, 1, 1}, {0, 1, 1}},
           {{1, 1, 0}, {1, 1, 0}, {1, 0, 0}}}

    Dim _gaussianKernel5x5 As Single(,) _
        = {{2, 4, 5, 4, 2},
            {4, 9, 12, 9, 4},
            {5, 12, 15, 12, 5},
            {4, 9, 12, 9, 4},
            {2, 4, 5, 4, 2}}

    Dim _sobelKernelX As Single(,) _
        = {{-1, 0, 1},
           {-2, 0, 2},
           {-1, 0, 1}}

    Dim _sobelKernelY As Single(,) _
        = {{-1, -2, -1},
           {0, 0, 0},
           {1, 2, 1}}

    <Extension()>
    Public Sub Undither3x3v2(ByRef b As Bitmap)
        Dim bSize As Size = b.Size
        Dim bStride As Integer = bSize.Width * 3

        Dim pixelData As Byte() = b.GetPixelData

        'Convert pixel data to color matrix
        Dim srcColorMatrix(bSize.Width - 1, bSize.Height - 1) As Color
        Parallel.For(0, bSize.Height,
            Sub(y As Integer)
                For x As Integer = 0 To bSize.Width - 1
                    Dim index As Integer = y * bStride + x * 3
                    srcColorMatrix(x, y) = Color.FromArgb(pixelData(index + 2), pixelData(index + 1), pixelData(index))
                Next
            End Sub)

        Dim dstColorMatrix As Color(,) = srcColorMatrix.Clone

        Dim srcGrayMatrix(bSize.Width - 1, bSize.Height - 1) As Single
        Parallel.For(0, bSize.Height,
            Sub(y As Integer)
                For x As Integer = 0 To bSize.Width - 1
                    srcGrayMatrix(x, y) = srcColorMatrix(x, y).GetLuminance
                Next
            End Sub)

        Dim dstGrayMatrix(bSize.Width - 1, bSize.Height - 1) As Single '(,) = srcGrayMatrix.Clone

        ''Apply Gaussian filter
        'Parallel.For(2, bSize.Height - 2,
        '     Sub(y As Integer)
        '         For x As Integer = 2 To bSize.Width - 3

        '             'Get 5x5 matrix of pixels; reference pixel is (2,2)
        '             Dim pixelMatrix(4, 4) As Single
        '             For q As Integer = -2 To 2
        '                 For p As Integer = -2 To 2
        '                     pixelMatrix(p + 2, q + 2) = srcGrayMatrix(x + p, y + q)
        '                 Next
        '             Next

        '             Dim newValue As Single = 0
        '             For q As Integer = 0 To 4
        '                 For p As Integer = 0 To 4
        '                     newValue += pixelMatrix(p, q) * _gaussianKernel5x5(p, q)
        '                 Next
        '             Next
        '             newValue /= 159

        '             dstGrayMatrix(x, y) = newValue

        '         Next
        '     End Sub)

        'srcGrayMatrix = dstGrayMatrix.Clone

        'Use flag array to keep track of edge pixels
        Dim edgeFlags(bSize.Width - 1, bSize.Height - 1) As Single
        Dim edgeStrengths(bSize.Width - 1, bSize.Height - 1) As Single
        Dim edgeAngles(bSize.Width - 1, bSize.Height - 1) As Single
        Const OneEightyOverPI As Single = 180 / Math.PI

        'Detect edges
        Parallel.For(2, bSize.Height - 2,
             Sub(y As Integer)
                 For x As Integer = 2 To bSize.Width - 3

                     'Get 3x3 matrix of pixels; reference pixel is (1,1)
                     Dim pixelMatrix(2, 2) As Single
                     For q As Integer = -1 To 1
                         For p As Integer = -1 To 1
                             pixelMatrix(p + 1, q + 1) = srcGrayMatrix(x + p, y + q)
                         Next
                     Next

                     'NOTE: The range of Gx and Gy is (-4,4)
                     '      Negative values mean edges are lighter on the left and top, respectively for Gx and Gy
                     Dim Gx As Single = 0
                     Dim Gy As Single = 0
                     For q As Integer = 0 To 2
                         For p As Integer = 0 To 2 'Step 2
                             Gx += pixelMatrix(p, q) * _sobelKernelX(p, q)
                             Gy += pixelMatrix(p, q) * _sobelKernelY(p, q)
                         Next
                     Next

                     Dim G As Single = Math.Abs(Gx) + Math.Abs(Gy)
                     edgeStrengths(x, y) = G

                     If G >= 0.6 Then
                         edgeFlags(x, y) = 1

                         Dim theta As Single = Math.Atan2(Gy, Gx) * OneEightyOverPI

                         edgeAngles(x, y) = Math.Abs(theta)

                         Dim edgeAngle As Single _
                            = edgeAngles(x, y)

                         If edgeAngle >= 22.5 And edgeAngle < 67.5 Then
                             edgeAngles(x, y) = 45
                         ElseIf edgeAngle >= 67.5 And edgeAngle < 112.5 Then
                             edgeAngles(x, y) = 90
                         ElseIf edgeAngle >= 112.5 And edgeAngle < 157.5 Then
                             edgeAngles(x, y) = 135
                         ElseIf edgeAngle >= 157.5 And edgeAngle < 202.5 Then
                             edgeAngles(x, y) = 180
                         End If

                     Else
                         edgeFlags(x, y) = 0
                     End If

                 Next
             End Sub)

        ''Non-maximum supression
        'Parallel.For(2, bSize.Height - 2,
        '     Sub(y As Integer)
        '         For x As Integer = 2 To bSize.Width - 3

        '             'Get 3x3 matrix of pixel strengths; reference pixel is (1,1)
        '             Dim strengthMatrix(2, 2) As Single
        '             For q As Integer = -1 To 1
        '                 For p As Integer = -1 To 1
        '                     strengthMatrix(p + 1, q + 1) = edgeStrengths(x + p, y + q)
        '                 Next
        '             Next

        '             Dim edgeAngle As Single _
        '                = edgeAngles(x, y)

        '             If edgeAngle >= 22.5 And edgeAngle < 67.5 Then
        '                 If Not ((strengthMatrix(2, 0) < strengthMatrix(1, 1)) And (strengthMatrix(0, 2) < strengthMatrix(1, 1))) Then
        '                     edgeAngles(x, y) = 45
        '                     edgeFlags(x, y) = 0
        '                 End If
        '             ElseIf edgeAngle >= 67.5 And edgeAngle < 112.5 Then
        '                 If Not ((strengthMatrix(1, 0) < strengthMatrix(1, 1)) And (strengthMatrix(1, 2) < strengthMatrix(1, 1))) Then
        '                     edgeAngles(x, y) = 90
        '                     edgeFlags(x, y) = 0
        '                 End If
        '             ElseIf edgeAngle >= 112.5 And edgeAngle < 157.5 Then
        '                 If Not ((strengthMatrix(0, 2) < strengthMatrix(1, 1)) And (strengthMatrix(2, 0) < strengthMatrix(1, 1))) Then
        '                     edgeAngles(x, y) = 135
        '                     edgeFlags(x, y) = 0
        '                 End If
        '             ElseIf edgeAngle >= 157.5 And edgeAngle < 202.5 Then
        '                 If Not ((strengthMatrix(0, 1) < strengthMatrix(1, 1)) And (strengthMatrix(2, 1) < strengthMatrix(1, 1))) Then
        '                     edgeAngles(x, y) = 180
        '                     edgeFlags(x, y) = 0
        '                 End If
        '             End If

        '         Next
        '     End Sub)

        ''Convert destination color matrix to destination pixel data
        'Parallel.For(0, bSize.Height,
        '    Sub(y As Integer)
        '        For x As Integer = 0 To bSize.Width - 1
        '            Dim index As Integer = y * bStride + x * 3
        '            Array.Copy({CByte(dstGrayMatrix(x, y) * 255), CByte(dstGrayMatrix(x, y) * 255), CByte(dstGrayMatrix(x, y) * 255)}, 0, pixelData, index, 3)
        '        Next
        '    End Sub)

        'b.SetPixelData(pixelData)
        'Exit Sub

        '1st pass:
        'Compare each pixel with its cardinal neighbors
        'Flag the pixel for dithering if at least three neighbors differ from the reference pixel and also match each other

        Parallel.For(1, bSize.Height - 1,
            Sub(y As Integer)
                'For y As Integer = 1 To bSize.Height - 2
                For x As Integer = 1 To bSize.Width - 2

                    If edgeFlags(x, y) <> 0 Then
                        'dstColorMatrix(x, y) = Color.Red
                        Continue For
                    End If

                    'Get 3x3 matrix of pixels; reference pixel is (1,1)
                    Dim pixelMatrix(2, 2) As Color
                    For q As Integer = -1 To 1
                        For p As Integer = -1 To 1
                            pixelMatrix(p + 1, q + 1) = srcColorMatrix(x + p, y + q)
                        Next
                    Next

                    ''Calculate similarities to reference pixel as well as average similarity
                    'Dim similarity(2, 2) As Single
                    'Dim avgSimilarity As Single = 0
                    'For q As Integer = 0 To 2
                    '    For p As Integer = 0 To 2
                    '        similarity(p, q) = pixelMatrix(p, q).SimilarityTo(pixelMatrix(1, 1))
                    '        avgSimilarity += similarity(p, q)
                    '    Next
                    'Next
                    'avgSimilarity /= 9

                    Dim score As Integer = 0
                    'For q As Integer = 0 To 2
                    '    For p As Integer = 0 To 2
                    '        If similarity(p, q) < 1 Then
                    '            If similarity(p, q) <= avgSimilarity Then
                    '                score += 1
                    '            End If
                    '        End If
                    '    Next
                    'Next

                    If score >= 0 Then

                        'Dim edgeMatrix(2, 2) As Integer
                        'For q As Integer = 0 To 2
                        '    For p As Integer = 0 To 2
                        '        If similarity(p, q) >= 0.85 Then
                        '            edgeMatrix(p, q) = 1
                        '        End If
                        '    Next
                        'Next

                        ''Compare edge matrix with edge masks
                        'Dim failed As Boolean = False
                        'For r As Integer = 0 To 15
                        '    failed = False
                        '    For q As Integer = 0 To 2
                        '        For p As Integer = 0 To 2
                        '            If edgeMatrix(p, q) <> EdgeMasks3x3(r, p, q) Then
                        '                failed = True
                        '                Exit For
                        '            End If
                        '        Next
                        '        If failed = True Then Exit For
                        '    Next
                        '    If failed = False Then Exit For
                        'Next
                        ''If edge detected
                        'If failed = False Then
                        '    dstColorMatrix(x, y) = Color.Red
                        '    Continue For
                        'End If

                        'Dim weights As Single(,) _
                        '    = {{1, 1, 1},
                        '       {1, 1, 1},
                        '       {1, 1, 1}}

                        Dim weights As Single(,) _
                            = {{1, 3, 1},
                               {3, 5, 3},
                               {1, 3, 1}}

                        'Select Case edgeAngles(x, y)
                        '    Case 45
                        '        weights = {{0, 0, 3},
                        '                   {0, 5, 0},
                        '                   {3, 0, 0}}
                        '    Case 90
                        '        weights = {{0, 3, 0},
                        '                   {0, 5, 0},
                        '                   {0, 3, 0}}
                        '    Case 135
                        '        weights = {{3, 0, 0},
                        '                   {0, 5, 0},
                        '                   {0, 0, 3}}
                        '    Case 180
                        '        weights = {{0, 0, 0},
                        '                   {3, 5, 3},
                        '                   {0, 0, 0}}
                        'End Select

                        'Adjust weights and calculate scale
                        Dim scale As Single = 0
                        For q As Integer = 0 To 2
                            For p As Integer = 0 To 2
                                'weights(p, q) += 1 - similarity(p, q)
                                scale += weights(p, q)
                            Next
                        Next
                        'If scale = 0 Then Continue For

                        Dim RGB(2) As Single
                        For q As Integer = 0 To 2
                            For p As Integer = 0 To 2
                                If weights(p, q) <> 0 Then
                                    Dim color As Single() _
                                        = {pixelMatrix(p, q).R, pixelMatrix(p, q).G, pixelMatrix(p, q).B}
                                    For i As Integer = 0 To 2
                                        RGB(i) += color(i) * weights(p, q)
                                    Next
                                End If
                            Next
                        Next

                        For i As Integer = 0 To 2
                            RGB(i) = Math.Min(RGB(i) / scale, 255)
                        Next

                        Dim newColor As Color _
                            = Color.FromArgb(RGB(0), RGB(1), RGB(2))
                        dstColorMatrix(x, y) = newColor

                    End If

                Next

                'Next
            End Sub)

        'Parallel.For(0, bSize.Height,
        '    Sub(y As Integer)
        '        For x As Integer = 0 To bSize.Width - 6

        '            'Get 5x1 vector of pixels; reference pixel is (0)
        '            Dim pixelVectorX(4) As Color
        '            For p As Integer = 0 To 4
        '                pixelVectorX(p) = dstColorMatrix(x + p, y)
        '            Next

        '            If (pixelVectorX(2) = pixelVectorX(0)) _
        '            And (pixelVectorX(4) = pixelVectorX(0)) Then
        '                Dim colors As Single(,) _
        '                    = {{pixelVectorX(0).R, pixelVectorX(0).G, pixelVectorX(0).B},
        '                       {pixelVectorX(1).R, pixelVectorX(1).G, pixelVectorX(1).B}}

        '                Dim RGB(2) As Single

        '                For j As Integer = 0 To 2
        '                    For i As Integer = 0 To 1
        '                        RGB(j) += colors(i, j)
        '                    Next
        '                    RGB(j) /= 2
        '                    RGB(j) = Math.Min(RGB(j), 255)
        '                Next

        '                Dim newColor As Color _
        '                    = Color.FromArgb(RGB(0), RGB(1), RGB(2))
        '                For p As Integer = 0 To 4
        '                    dstColorMatrix(x + p, y) = newColor
        '                Next

        '                x += 4

        '            End If

        '        Next

        '        'Next
        '    End Sub)

        'Parallel.For(0, bSize.Width,
        '    Sub(x As Integer)
        '        For y As Integer = 0 To bSize.Height - 6

        '            'Get 5x1 vector of pixels; reference pixel is (0)
        '            Dim pixelVectorY(4) As Color
        '            For p As Integer = 0 To 4
        '                pixelVectorY(p) = dstColorMatrix(x, y + p)
        '            Next

        '            If (pixelVectorY(2) = pixelVectorY(0)) _
        '            And (pixelVectorY(4) = pixelVectorY(0)) Then
        '                Dim colors As Single(,) _
        '                    = {{pixelVectorY(0).R, pixelVectorY(0).G, pixelVectorY(0).B},
        '                       {pixelVectorY(1).R, pixelVectorY(1).G, pixelVectorY(1).B}}

        '                Dim RGB(2) As Single

        '                For j As Integer = 0 To 2
        '                    For i As Integer = 0 To 1
        '                        RGB(j) += colors(i, j)
        '                    Next
        '                    RGB(j) /= 2
        '                    RGB(j) = Math.Min(RGB(j), 255)
        '                Next

        '                Dim newColor As Color _
        '                    = Color.FromArgb(RGB(0), RGB(1), RGB(2))
        '                For p As Integer = 0 To 4
        '                    dstColorMatrix(x, y + p) = newColor
        '                Next

        '                y += 4

        '            End If

        '        Next

        '        'Next
        '    End Sub)

        'Convert destination color matrix to destination pixel data
        Parallel.For(0, bSize.Height,
            Sub(y As Integer)
                For x As Integer = 0 To bSize.Width - 1
                    Dim index As Integer = y * bStride + x * 3
                    Array.Copy({dstColorMatrix(x, y).B, dstColorMatrix(x, y).G, dstColorMatrix(x, y).R}, 0, pixelData, index, 3)
                Next
            End Sub)

        b.SetPixelData(pixelData)

End Sub

    Dim _gaussianKernel As Single(,) _
        = {{3, 4, 3},
           {4, 5, 4},
           {3, 4, 3}}

    <Extension()>
    Public Sub GaussianFilter(ByRef b As Bitmap)
        Dim bSize As Size = b.Size
        Dim bStride As Integer = bSize.Width * 3

        Dim pixelData As Byte() = b.GetPixelData

        'Convert pixel data to color matrix
        Dim srcColorMatrix(bSize.Width - 1, bSize.Height - 1) As Color
        Parallel.For(0, bSize.Height,
            Sub(y As Integer)
                For x As Integer = 0 To bSize.Width - 1
                    Dim index As Integer = y * bStride + x * 3
                    srcColorMatrix(x, y) = Color.FromArgb(pixelData(index + 2), pixelData(index + 1), pixelData(index))
                Next
            End Sub)

        Dim dstColorMatrix As Color(,) = srcColorMatrix.Clone

        'Use flag array to determine which pixels should be dithered
        Dim flags(1, b.Size.Width - 1, b.Size.Height - 1) As Integer

        '1st pass
        Parallel.For(1, bSize.Height - 1,
             Sub(y As Integer)
                 For x As Integer = 1 To bSize.Width - 2
                     Dim refColor As Color _
                         = srcColorMatrix(x, y)

                     Dim cards As Color(,) _
                        = {{srcColorMatrix(x - 1, y), srcColorMatrix(x + 1, y)},
                           {srcColorMatrix(x, y - 1), srcColorMatrix(x, y + 1)}}

                     For i As Integer = 0 To 1
                         If (cards(i, 0) = cards(i, 0)) And (cards(i, 0) <> refColor) Then
                             flags(i, x, y) = 1
                         End If
                     Next

                 Next

             End Sub)

        '2nd pass
        'Dim weights As Single() _
        '    = {1, 2, 1}
        'Dim scale As Single _
        '    = weights.Sum

        Parallel.For(1, bSize.Height - 1,
             Sub(y As Integer)
                 For x As Integer = 1 To bSize.Width - 2

                     If (flags(0, x, y) = 0) And (flags(1, x, y) = 0) Then
                         Continue For
                     End If

                     Dim refColor As Color _
                         = srcColorMatrix(x, y)

                     Dim cards As List(Of Color)() _
                         = {New List(Of Color), New List(Of Color)}

                     cards(0).Add(srcColorMatrix(x - 1, y))
                     'cards(0).Add(srcColorMatrix(x, y))
                     'cards(0).Add(srcColorMatrix(x + 1, y))
                     cards(1).Add(srcColorMatrix(x, y - 1))
                     'cards(1).Add(srcColorMatrix(x, y))
                     'cards(1).Add(srcColorMatrix(x, y + 1))

                     Dim newColors As New List(Of Color)

                     Dim RGB As Integer() _
                        = {refColor.R, refColor.G, refColor.B}

                     For i As Integer = 0 To 1
                         If flags(i, x, y) = 1 Then
                             Dim cardRGB As Integer() _
                                = {cards(i)(0).R, cards(i)(0).G, cards(i)(0).B}

                             For j As Integer = 0 To 2
                                 RGB(j) += cardRGB(j)
                                 RGB(j) \= 2
                             Next

                             'red = Math.Min((CInt(refColor.R) + CInt(cards(i)(0).R)) / 2, 255)
                             'green = Math.Min((CInt(refColor.G) + CInt(cards(i)(0).G)) / 2, 255)
                             'blue = Math.Min((CInt(refColor.B) + CInt(cards(i)(0).B)) / 2, 255)

                             'newColors.Add(Color.FromArgb(red, green, blue))
                         End If
                     Next

                     dstColorMatrix(x, y) = Color.FromArgb(RGB(0), RGB(1), RGB(2))

                     'If newColors.Count = 1 Then
                     '    dstColorMatrix(x, y) = newColors(0)
                     'Else
                     '    Dim avgR, avgG, avgB As Integer
                     '    avgR = Math.Min((CInt(newColors(0).R) + CInt(newColors(1).R)) / 2, 255)
                     '    avgG = Math.Min((CInt(newColors(0).G) + CInt(newColors(1).G)) / 2, 255)
                     '    avgB = Math.Min((CInt(newColors(0).B) + CInt(newColors(1).B)) / 2, 255)

                     '    Dim newColor As Color _
                     '       = Color.FromArgb(avgR, avgG, avgB)

                     '    dstColorMatrix(x, y) = newColor
                     'End If

                 Next
             End Sub)

        'Convert destination color matrix to destination pixel data
        Parallel.For(0, bSize.Height,
            Sub(y As Integer)
                For x As Integer = 0 To bSize.Width - 1
                    Dim index As Integer = y * bStride + x * 3
                    Array.Copy({dstColorMatrix(x, y).B, dstColorMatrix(x, y).G, dstColorMatrix(x, y).R}, 0, pixelData, index, 3)
                Next
            End Sub)

        b.SetPixelData(pixelData)

    End Sub

End Module

Module ColorExtension

    <Extension()>
    Public Function IsSimilarTo(ByRef color2 As Color, ByVal color As Color) As Boolean
        If color2.SimilarityTo(color) > 0.9 Then
            Return True
        End If
        Return False
    End Function

    Const _colorDiffMax As Single = 540.9366876077089 '441.67295593006372

    <Extension()>
    Public Function SimilarityTo(ByRef color1 As Color, ByVal color2 As Color) As Single
        If color1 = color2 Then Return 1

        Dim L1 As Single = color1.GetLuminance 'GetLValue
        Dim L2 As Single = color2.GetLuminance 'GetLValue

        Dim diff As Single = Math.Abs(L1 - L2)

        Return 1 - diff

        'Dim deltaR As Integer = CInt(color2.R) - CInt(color1.R)
        'Dim deltaG As Integer = CInt(color2.G) - CInt(color1.G)
        'Dim deltaB As Integer = CInt(color2.B) - CInt(color1.B)

        'Dim diff As Single = Math.Sqrt(((0.5 * deltaR) ^ 2) + ((2 * deltaG) ^ 2) + ((0.5 * deltaB) ^ 2))

        ''Normalize difference
        'diff /= _colorDiffMax

        'Return 1 - diff
    End Function

    <Extension()>
    Public Function GetLuminance(ByRef color As Color) As Single
        Dim R As Single = color.R / 255
        Dim G As Single = color.G / 255
        Dim B As Single = color.B / 255

        'Return (R + R + G + G + G + B) / 6
        Return (R + G + B) / 3
    End Function

    <Extension()>
    Public Function GetLValue(ByRef color As Color) As Single
        Dim R As Single = color.R / 255
        Dim G As Single = color.G / 255
        Dim B As Single = color.B / 255

        Dim gamma As Single = 2.2

        Dim Y As Single = (0.2126 * (R ^ gamma)) + (0.7152 * (G ^ gamma)) + (0.0722 * (B ^ gamma))

        Dim L As Single = 116 * (Y ^ (1 / 3)) - 16

        Return (L + 16) / 116
    End Function

End Module

Module ByteExtension

    <Extension()>
    Public Function ToSByte(ByRef value As Byte) As SByte
        Return value - 128
    End Function

End Module

Module SByteExtension

    <Extension()>
    Public Function ToByte(ByRef value As SByte) As Byte
        Return value + 128
    End Function

End Module


