'Imports System.Windows.Media.Imaging
'Imports System.Windows.Media

Imports System.Drawing.Imaging

Namespace Sega

    Public MustInherit Class VDP
        Implements IDisposable

        Protected _fullColorPaletteAsColors As Color() 'Color()
        Protected MustOverride ReadOnly Property FullColorPaletteAsColors As Color() 'Color()

        'Protected _fullColorPaletteAsARGB As Integer()
        'Protected ReadOnly Property FullColorPaletteAsARGB As Integer()
        '    Get
        '        If _fullColorPaletteAsARGB Is Nothing Then
        '            ReDim _fullColorPaletteAsARGB(FullColorPaletteAsColors.Length - 1)
        '            For i As Integer = 0 To _fullColorPaletteAsARGB.Length - 1
        '                _fullColorPaletteAsARGB(i) = FullColorPaletteAsColors(i).ToArgb
        '            Next
        '        End If
        '        Return _fullColorPaletteAsARGB
        '    End Get
        'End Property

        Protected _paletteAsColors As Color() 'Color()
        Public ReadOnly Property PaletteAsColors As Color() 'Color()
            Get
                Return _paletteAsColors
            End Get
        End Property

        Public ReadOnly Property PaletteAsWindowsMediaColors As Windows.Media.Color()
            Get
                Dim colors As New List(Of Windows.Media.Color)
                For Each color As Color In _paletteAsColors
                    With color
                        colors.Add(Windows.Media.Color.FromRgb(.R, .G, .B))
                    End With
                Next
                Return colors.ToArray
            End Get
        End Property

        'Protected _paletteAsARGB As Integer()
        'Public ReadOnly Property PaletteAsARGB As Integer()
        '    Get
        '        Return _paletteAsARGB
        '    End Get
        'End Property

        Protected _CRAM As UShort()
        Public Property CRAM As UShort()
            Get
                Return _CRAM
            End Get
            Set(value As UShort())
                '_CRAM = value
                Array.Copy(value, _CRAM, value.Length)
                CRAMChanged()
            End Set
        End Property

        Protected Sub InitializeCRAM(ByVal length As Integer)
            ReDim _CRAM(length - 1)
            CRAMChanged()
        End Sub

        Public Sub WriteToCRAM(ByRef value As UShort(), ByVal offset As Integer)
            Array.Copy(value, 0, _CRAM, offset, value.Length)
            CRAMChanged()
        End Sub

        Private Sub CRAMChanged()
            ReDim _paletteAsColors(_CRAM.Length - 1)
            For CRAMIndex As Integer = 0 To CRAM.Length - 1
                _paletteAsColors(CRAMIndex) = FullColorPaletteAsColors(CRAM(CRAMIndex))
            Next
            '_frameBufferPalette = New BitmapPalette(_paletteAsColors)

            'ReDim _paletteAsARGB(_CRAM.Length - 1)
            'For CRAMIndex As Integer = 0 To CRAM.Length - 1
            '    _paletteAsARGB(CRAMIndex) = FullColorPaletteAsARGB(CRAM(CRAMIndex))
            'Next
        End Sub

        Protected _tileSize As New Size(8, 8)

        Protected _sizeInTiles As Size
        Public Overridable Property SizeInTiles As Size
            Get
                Return _sizeInTiles
            End Get
            Set(value As Size)
                Dim oldSize As Size _
                    = _sizeInTiles
                _sizeInTiles = value
                If _sizeInTiles <> oldSize Then
                    With SizeInTiles
                        _sizeInPixels = New Size(.Width * _tileSize.Width, .Height * _tileSize.Height)
                    End With
                    SizeChanged()
                End If
            End Set
        End Property

        Private _sizeInPixels As Size
        Public Property SizeInPixels As Size
            Get
                Return _sizeInPixels
            End Get
            Set(value As Size)
                With value
                    .Width \= 8
                    .Width *= 8
                    .Height \= 8
                    .Height *= 8
                End With
                Dim oldSize As Size _
                    = _sizeInPixels
                _sizeInPixels = value
                If _sizeInPixels <> oldSize Then
                    With SizeInPixels
                        _sizeInTiles = New Size(.Width \ _tileSize.Width, .Height \ _tileSize.Height)
                    End With
                    SizeChanged()
                End If
            End Set
        End Property

        Protected Overridable Sub SizeChanged()
            InitializeFrameBuffer(SizeInPixels.Width * SizeInPixels.Height)
            '_frameBufferBitmap = New WriteableBitmap(SizeInPixels.Width, SizeInPixels.Height, 96, 96, _frameBufferPixelFormat, Nothing)
            _frameBufferBitmap = New Bitmap(SizeInPixels.Width, SizeInPixels.Height, _frameBufferPixelFormat)
        End Sub

        Protected _frameBuffer As Short() 'Byte()
        Protected _frameBufferNeedsUpdate As Boolean

        Protected Sub InitializeFrameBuffer(ByVal length As Integer)
            ReDim _frameBuffer(length - 1)
            ReDim _pixelData(_frameBuffer.Length * 3 - 1)
            InvalidateFrameBuffer()
        End Sub

        Protected Sub InvalidateFrameBuffer()
            _frameBufferNeedsUpdate = True
        End Sub

        Protected MustOverride Sub UpdateFrameBuffer()

        Private _pixelData As Byte()
        Protected ReadOnly Property PixelData() As Byte()
            Get
                Return _pixelData
            End Get
        End Property

        Protected Sub UpdatePixelData()

            Parallel.For(0, _frameBuffer.Length,
                Sub(i As Integer)
                    If _frameBuffer(i) >= 0 Then
                        With PaletteAsColors(_frameBuffer(i))
                            'Array.Copy({ .B, CByte(255), CByte(255)}, 0, _pixelData, i * 3, 3)
                            'Array.Copy({CByte(255), .G, CByte(255)}, 0, _pixelData, i * 3, 3)
                            'Array.Copy({CByte(255), CByte(255), .R}, 0, _pixelData, i * 3, 3)

                            'Array.Copy({ .B, .B, .B}, 0, _pixelData, i * 3, 3)
                            'Array.Copy({ .G, .G, .G}, 0, _pixelData, i * 3, 3)
                            'Array.Copy({ .R, .R, .R}, 0, _pixelData, i * 3, 3)

                            Array.Copy({ .B, .G, .R}, 0, _pixelData, i * 3, 3)
                        End With
                    End If
                End Sub)

            'Dim pixelDataIndex As Integer = 0

            'For i As Integer = 0 To _frameBuffer.Length - 1
            '    If _frameBuffer(i) >= 0 Then
            '        With PaletteAsColors(_frameBuffer(i))
            '            Array.Copy({ .B, .G, .R}, 0, _pixelData, pixelDataIndex, 3)
            '        End With
            '    End If
            '    pixelDataIndex += 3
            'Next
        End Sub

        'Chenged on 2016/4/17: Reverted to normal bitmap
            Protected _frameBufferBitmap As Bitmap 'WriteableBitmap
        Protected _frameBufferPixelFormat As PixelFormat = PixelFormat.Format24bppRgb 'PixelFormat = PixelFormats.Rgb24
        'Protected _frameBufferPalette As BitmapPalette

        'Public ReadOnly Property FrameBufferAsWriteableBitmap() As WriteableBitmap
        '    Get
        '        If _frameBufferNeedsUpdate Then UpdateFrameBuffer()

        '        With _frameBufferBitmap
        '            .Lock()
        '            Dim sourceRect As New Windows.Int32Rect(0, 0, .PixelWidth, .PixelHeight)

        '            Dim pixels As Byte() _
        '                = FrameBufferAsPixelData()

        '            .WritePixels(sourceRect, pixels, .PixelWidth * Math.Ceiling(_frameBufferPixelFormat.BitsPerPixel / 8), 0)

        '            .Unlock()
        '        End With

        '        Return _frameBufferBitmap
        '    End Get
        'End Property

        Public ReadOnly Property FrameBufferAsBitmap() As Bitmap
            Get
                'Return FrameBufferAsWriteableBitmap.ToBitmap
                If _frameBufferNeedsUpdate Then UpdateFrameBuffer()

                _frameBufferBitmap.SetPixelData(PixelData)

                If ProgramOptions.FilerVideoOutput = True Then
                    For iterations As Integer = 0 To 0
                        '_frameBufferBitmap.Undither2x2
                        '_frameBufferBitmap.Undither2x2v2
                        '_frameBufferBitmap.Undither3x3
                        _frameBufferBitmap.Undither3x3v2
                        '_frameBufferBitmap.Undither3x3HV
                    Next
                End If

                Return _frameBufferBitmap
            End Get
        End Property

        Public ReadOnly Property FrameBufferAsUpscaledBitmap(ByVal scaleFactor As Integer) As Bitmap
            Get
                Using b As New Bitmap(SizeInPixels.Width * scaleFactor, SizeInPixels.Height * scaleFactor, _frameBufferPixelFormat)

                    Using g As Graphics = Graphics.FromImage(b)

                    End Using

                    Return b.Clone

                End Using

            End Get
        End Property

        Sub New(ByVal sizeInTiles As Size)
            Me.SizeInTiles = sizeInTiles
        End Sub

        Sub New()
            Me.New(New Size(40, 28))
        End Sub

#Region " IDisposable Support "
        ' Flag: Has Dispose already been called?
        Dim disposed As Boolean = False

        ' Public implementation of Dispose pattern callable by consumers.
        Public Sub Dispose() _
            Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        ' Protected implementation of Dispose pattern.
        Protected Overridable Sub Dispose(disposing As Boolean)
            If disposed Then Return

            If disposing Then
                ' Free any other managed objects here.
                '
            End If

            ' Free any unmanaged objects here.
            '

            disposed = True
        End Sub

        Protected Overrides Sub Finalize()
            Dispose(False)
        End Sub
#End Region

    End Class

    Namespace MegaDrive
        'Added on 2014/12/1: "Real" Mega Drive, Mega CD, and 32X video hardware support
        Public Class VDP
            Inherits Sega.VDP

            Private Shared _normalLevels As Byte() _
                = {0, 52, 87, 116, 144, 172, 206, 255}
            Private Shared _shadowLevels As Byte() _
                = {0, 29, 52, 70, 87, 101, 116, 130}
            Private Shared _highlightLevels As Byte() _
                = {130, 144, 158, 172, 187, 206, 228, 255}

            Protected Overrides ReadOnly Property FullColorPaletteAsColors As Color() 'Color()
                Get
                    If _fullColorPaletteAsColors Is Nothing Then
                        ReDim _fullColorPaletteAsColors(4095)
                        For R As Integer = 0 To 7
                            For G As Integer = 0 To 7
                                For B As Integer = 0 To 7
                                    '_fullColorPaletteAsColors((B << 9) + (G << 5) + (R << 1)) = Color.FromArgb(R * 33, G * 33, B * 33)
                                    _fullColorPaletteAsColors((B << 9) + (G << 5) + (R << 1)) = Color.FromArgb(_normalLevels(R), _normalLevels(G), _normalLevels(B)) 'Color.FromRgb(_normalLevels(R), _normalLevels(G), _normalLevels(B))
                                Next
                            Next
                        Next
                    End If
                    Return _fullColorPaletteAsColors
                End Get
            End Property

            Protected Overrides Sub SizeChanged()
                MyBase.SizeChanged()
                InitializeTileData(SizeInTiles.Width * SizeInTiles.Height * 32)
                InitializeTileMaps(SizeInTiles.Width, SizeInTiles.Height)
            End Sub

            Private _tileData As Byte() 'List(Of Byte)
            Public ReadOnly Property TileData As Byte() 'List(Of Byte)
                Get
                    Return _tileData
                End Get
            End Property

            Public Shared ReadOnly Property _blankTile As Byte() = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}

            Private Sub InitializeTileData(ByVal length As Integer)
                ReDim _tileData(length + 31)
                InvalidateTiles()
            End Sub

            Public Sub WriteToTileData(ByVal value As Byte())
                WriteToTileData(value, 0)
            End Sub

            Public Sub WriteToTileData(ByVal value As Byte(), ByVal offset As Integer)
                offset += 32
                Array.Copy(value, 0, _tileData, offset, value.Length)
                InvalidateTiles()
            End Sub

            Public Sub WriteToTileData(ByVal value As Byte(), ByVal col As Integer, row As Integer)
                Dim offset As Integer _
                    = ((row * SizeInTiles.Width) + col) * 32
                WriteToTileData(value, offset)
            End Sub

            Public ReadOnly Property TileCount As Integer
                Get
                    If TileData Is Nothing Then
                        Return 0
                    Else
                        Return TileData.Length \ 32
                    End If
                End Get
            End Property

            Private _rawTile As Byte()
            Public ReadOnly Property RawTiles(ByVal index As Integer) As Byte()
                Get
                    _rawTile = _blankTile.Clone
                    If index < TileCount Then
                        Array.Copy(TileData, index * 32, _rawTile, 0, 32)
                    End If
                    Return _rawTile
                End Get
            End Property

            Public Overrides Property SizeInTiles As Size
                Get
                    Return MyBase.SizeInTiles
                End Get
                Set(value As Size)
                    MyBase.SizeInTiles = value
                End Set
            End Property

            Private _tileMapB As TileMapEntry(,)
            Public ReadOnly Property TileMapB As TileMapEntry(,)
                Get
                    Return _tileMapB
                End Get
            End Property

            Private _tileMapA As TileMapEntry(,)
            Public ReadOnly Property TileMapA As TileMapEntry(,)
                Get
                    Return _tileMapA
                End Get
            End Property

            Public Sub WriteToTileMapB(ByVal value As TileMapEntry(,))
                WriteToTileMap(value, _tileMapB)
            End Sub

            Public Sub WriteToTileMapB(ByVal value As TileMapEntry, ByVal col As Integer, ByVal row As Integer)
                WriteToTileMap(value, _tileMapB, col, row)
            End Sub

            Public Sub WriteToTileMapA(ByVal value As TileMapEntry(,))
                WriteToTileMap(value, _tileMapB)
            End Sub

            Public Sub WriteToTileMapA(ByVal value As TileMapEntry, ByVal col As Integer, ByVal row As Integer)
                WriteToTileMap(value, _tileMapA, col, row)
            End Sub

            Private Sub WriteToTileMap(ByVal value As TileMapEntry(,), ByRef tileMap As TileMapEntry(,))
                If tileMap Is Nothing Then
                    ReDim tileMap(SizeInTiles.Width - 1, SizeInTiles.Height - 1)
                End If
                tileMap = value
                InvalidateTiles()
            End Sub

            Private Sub WriteToTileMap(ByVal value As TileMapEntry, ByRef tileMap As TileMapEntry(,), ByVal col As Integer, ByVal row As Integer)
                If tileMap Is Nothing Then
                    ReDim tileMap(SizeInTiles.Width - 1, SizeInTiles.Height - 1)
                End If
                tileMap(col, row) = value
                InvalidateTiles()
            End Sub

            Private Sub InitializeTileMaps(ByVal width As Integer, ByVal height As Integer)
                ReDim _tileMapB(width - 1, height - 1)
                ReDim _tileMapA(width - 1, height - 1)
                InvalidateTiles()
            End Sub

            Private _tiles As Tile(,)
            Private _tilesNeedsUpdate As Boolean = True
            Public ReadOnly Property Tiles() As Tile(,)
                Get
                    If _tilesNeedsUpdate = True Then
                        ReDim _tiles(SizeInTiles.Width - 1, SizeInTiles.Height - 1)

                        For row As Integer = 0 To SizeInTiles.Height - 1
                            For col As Integer = 0 To SizeInTiles.Width - 1

                                _tiles(col, row) = New Tile(True)

                                Dim topTileMapEntry As TileMapEntry _
                                    = Nothing

                                Dim tileMapEntryB As TileMapEntry _
                                    = If(TileMapB Is Nothing, Nothing, TileMapB(col, row))

                                If tileMapEntryB IsNot Nothing Then
                                    topTileMapEntry = tileMapEntryB
                                End If

                                Dim tileMapEntryA As TileMapEntry _
                                    = If(TileMapA Is Nothing, Nothing, TileMapA(col, row))

                                If tileMapEntryA IsNot Nothing Then
                                    If topTileMapEntry IsNot Nothing Then
                                        If (topTileMapEntry.IsHighPriority = False) _
                                        Or (tileMapEntryA.IsHighPriority = True) Then
                                            topTileMapEntry = tileMapEntryA
                                        End If
                                    Else
                                        topTileMapEntry = tileMapEntryA
                                    End If
                                End If

                                If topTileMapEntry IsNot Nothing Then
                                    If topTileMapEntry.PatternIndex <> 0 Then
                                        _tiles(col, row) = New Tile(RawTiles(topTileMapEntry.PatternIndex), topTileMapEntry)
                                    End If
                                End If

                            Next
                        Next
                        _tilesNeedsUpdate = False
                    End If
                    Return _tiles
                End Get
            End Property

            Private Sub InvalidateTiles()
                _tilesNeedsUpdate = True
                InvalidateFrameBuffer()
            End Sub

            Protected Overrides Sub UpdateFrameBuffer()
                Dim rowOffset, colOffset, yOffset, xOffset As Integer

                For row As Integer = 0 To SizeInTiles.Height - 1
                    rowOffset = row * 8 * SizeInPixels.Width
                    For col As Integer = 0 To SizeInTiles.Width - 1
                        colOffset = col * 8

                        Dim tile As Tile _
                            = Tiles(col, row)

                        If tile.IsBlank Then
                            Continue For
                        Else
                            For y As Integer = 0 To 7
                                yOffset = y * SizeInPixels.Width
                                For x As Integer = 0 To 7
                                    xOffset = rowOffset + colOffset + yOffset + x
                                    _frameBuffer(xOffset) = tile.ColorMap(x, y)
                                Next
                            Next
                        End If

                    Next
                Next
                _frameBufferNeedsUpdate = False
                UpdatePixelData()
            End Sub

            Sub New()
                MyBase.New()
                InitializeCRAM(64)
                InitializeTileData(65536)
            End Sub

            Public Class TileMapEntry
                Private _rawData As UShort
                Public Property RawData As UShort
                    Get
                        Return _rawData
                        'Return _bv.Data
                    End Get
                    Private Set(value As UShort)
                        _rawData = value
                        '_bv = New Specialized.BitVector32(value)
                    End Set
                End Property

                'Private _bv As Specialized.BitVector32
                'Private _sectPatternIndex, _sectHFlip, _sectVFlip, _sectPaletteIndex, _sectIsHighPriority As Specialized.BitVector32.Section

                Public Property PatternIndex As UShort
                    Get
                        Return _rawData And &H3FF
                        'Return _bv(_sectPatternIndex)
                    End Get
                    Set(value As UShort)
                        Dim bits As UShort = value And &H3FF
                        _rawData = _rawData Or bits
                        '_bv(_sectPatternIndex) = value
                    End Set
                End Property

                Public Property HFlip As Boolean
                    Get
                        Return If((_rawData >> 11) And 1, True, False)
                        'Return If(_bv(_sectHFlip) = 1, True, False)
                    End Get
                    Set(value As Boolean)
                        Dim bit As UShort = If(value = True, 1, 0)
                        _rawData = _rawData Or (bit << 11)
                        '_bv(_sectHFlip) = If(value = True, 1, 0)
                    End Set
                End Property

                Public Property VFlip As Boolean
                    Get
                        Return If((_rawData >> 12) And 1, True, False)
                        'Return If(_bv(_sectVFlip) = 1, True, False)
                    End Get
                    Set(value As Boolean)
                        Dim bit As UShort = If(value = True, 1, 0)
                        _rawData = _rawData Or (bit << 12)
                        '_bv(_sectVFlip) = If(value = True, 1, 0)
                    End Set
                End Property

                Public Property PaletteIndex As UShort
                    Get
                        Return (_rawData >> 13) And 3
                        'Return _bv(_sectPaletteIndex)
                    End Get
                    Set(value As UShort)
                        Dim bits As UShort = value And 3
                        _rawData = _rawData Or (bits << 13)
                        '_bv(_sectPaletteIndex) = value
                    End Set
                End Property

                Public Property IsHighPriority As Boolean
                    Get
                        Return If((_rawData >> 15) And 1, True, False)
                        'Return If(_bv(_sectIsHighPriority) = 1, True, False)
                    End Get
                    Set(value As Boolean)
                        Dim bit As UShort = If(value = True, 1, 0)
                        _rawData = _rawData Or (bit << 15)
                        '_bv(_sectIsHighPriority) = If(value = True, 1, 0)
                    End Set
                End Property

                Sub New()
                    Me.New(0)
                End Sub

                Sub New(ByVal rawData As UShort)
                    _rawData = rawData
                    '_bv = New Specialized.BitVector32(rawData)

                    '_sectPatternIndex = Specialized.BitVector32.CreateSection(1023)
                    '_sectHFlip = Specialized.BitVector32.CreateSection(1, _sectPatternIndex)
                    '_sectVFlip = Specialized.BitVector32.CreateSection(1, _sectHFlip)
                    '_sectPaletteIndex = Specialized.BitVector32.CreateSection(3, _sectVFlip)
                    '_sectIsHighPriority = Specialized.BitVector32.CreateSection(1, _sectPaletteIndex)

                    'If HFlip = True Then
                    '    Debug.WriteLine("HFlip")
                    'End If

                    'If VFlip = True Then
                    '    Debug.WriteLine("VFlip")
                    'End If

                End Sub

                Sub New(ByVal tileMapEntry As TileMapEntry)
                    Me.New(tileMapEntry.RawData)
                End Sub

                Sub New(ByVal isHighPriority As Boolean, paletteIndex As UShort, vFlip As Boolean, hFlip As Boolean, patternIndex As UShort)
                    Me.New(0)

                    With Me
                        .PatternIndex = patternIndex
                        .HFlip = hFlip
                        .VFlip = vFlip
                        .PaletteIndex = paletteIndex
                        .IsHighPriority = isHighPriority
                    End With

                End Sub

            End Class

            Public Class Tile
                Private _tileMapEntry As TileMapEntry
                Public Property TileMapEntry As TileMapEntry
                    Get
                        Return _tileMapEntry
                    End Get
                    Protected Set(value As TileMapEntry)
                        _tileMapEntry = value
                    End Set
                End Property

                Public ReadOnly Property PatternIndex As Integer
                    Get
                        Return TileMapEntry.PatternIndex
                    End Get
                End Property

                Public ReadOnly Property HFlip As Boolean
                    Get
                        Return TileMapEntry.HFlip
                    End Get
                End Property

                Public ReadOnly Property VFlip As Boolean
                    Get
                        Return TileMapEntry.VFlip
                    End Get
                End Property

                Public ReadOnly Property PaletteIndex As Integer
                    Get
                        Return TileMapEntry.PaletteIndex
                    End Get
                End Property

                Public ReadOnly Property IsHighPriority As Boolean
                    Get
                        Return TileMapEntry.IsHighPriority
                    End Get
                End Property

                Private _isBlank As Boolean = False
                Public Property IsBlank As Boolean
                    Get
                        Return _isBlank
                    End Get
                    Set(value As Boolean)
                        _isBlank = value
                        If value = True Then
                            ReDim ColorMap(0, 0)
                            ColorMap = Nothing
                        Else
                            ReDim ColorMap(7, 7)
                        End If
                    End Set
                End Property

                Private _colorMap As Byte(,)
                Public Property ColorMap As Byte(,)
                    Get
                        Return _colorMap
                    End Get
                    Private Set(value As Byte(,))
                        _colorMap = value
                    End Set
                End Property

                Sub New(ByVal isBlank As Boolean)
                    With Me
                        .IsBlank = isBlank
                    End With
                End Sub

                Private _size As New Size(8, 8)
                Public ReadOnly Property SizeInPixels As Size
                    Get
                        Return _size
                    End Get
                End Property

                Sub New(ByVal rawTileData As Byte(), ByVal tileMapEntry As TileMapEntry) 'ByVal tileMapData As UShort)
                    Me.New(False)

                    Me.TileMapEntry = tileMapEntry

                    'Me.TileMapData = tileMapData

                    Dim colorMapPos As New Point
                    Dim pixelIndex As Byte
                    Dim offset As Integer

                    For y As Integer = 0 To SizeInPixels.Height - 1
                        'Added on 2014/7/5: Support for flipped tiles
                        colorMapPos.Y = If(VFlip = True, SizeInPixels.Height - 1 - y, y)

                        For x As Integer = 0 To SizeInPixels.Width - 1
                            'Added on 2014/7/5: Support for flipped tiles
                            colorMapPos.X = If(HFlip = True, SizeInPixels.Width - 1 - x, x)

                            pixelIndex = x Mod 2

                            ColorMap(colorMapPos.X, colorMapPos.Y) = ((rawTileData(offset) >> ((1 - pixelIndex) * 4)) And 15) + (16 * PaletteIndex)

                            offset += pixelIndex
                        Next

                    Next

                End Sub

                Public Function ToRawTileData() As Byte()
                    Dim byteArray(31) As Byte
                    For y As Integer = 0 To SizeInPixels.Height - 1
                        For x As Integer = 0 To SizeInPixels.Width - 1 Step 2
                            byteArray((y * 4) + (x \ 2)) = (ColorMap(x, y) << 4) + ColorMap(x + 1, y)
                        Next
                    Next
                    Return byteArray
                End Function

                Public ReadOnly Property ToPixelData(ByVal palette As Color()) As Byte()
                    Get
                        'Dim pixelData As New List(Of Byte)
                        Dim pixelData(63) As Byte, pixelIndex As Integer = 0

                        For row As Integer = 0 To SizeInPixels.Height - 1
                            For col As Integer = 0 To SizeInPixels.Width - 1
                                Dim colorIndex As Integer _
                                    = ColorMap(col, row)
                                With palette(colorIndex)
                                    'pixelData.AddRange({ .R, .G, .B})
                                    pixelData(pixelIndex) = .R
                                    pixelData(pixelIndex + 1) = .G
                                    pixelData(pixelIndex + 2) = .B
                                    pixelIndex += 3
                                End With
                            Next
                        Next

                        Return pixelData '.ToArray
                    End Get
                End Property

                Public ReadOnly Property ToPixels() As Byte()
                    Get
                        'Dim pixels As New List(Of Byte)
                        Dim pixels(63) As Byte, pixelIndex As Integer = 0

                        For row As Integer = 0 To SizeInPixels.Height - 1
                            For col As Integer = 0 To SizeInPixels.Width - 1
                                'pixels.Add(ColorMap(col, row))
                                pixels(pixelIndex) = ColorMap(col, row)
                                pixelIndex += 1
                            Next
                        Next

                        Return pixels '.ToArray
                    End Get
                End Property

                Public Function ToBitmap(ByVal palette As Color()) As Bitmap
                    'If IsBlank = True Then
                    Return Nothing
                    'End If

                    'Dim _bitmapPalette As New BitmapPalette(palette)
                    'Dim _bitmap As New WriteableBitmap(_size.Width, _size.Height, 96, 96, Windows.Media.PixelFormats.Indexed8, _bitmapPalette) 'Windows.Media.PixelFormats.Bgr24, _bitmapPalette)

                    'With _bitmap
                    '    '.Lock()
                    '    Dim sourceRect As New Windows.Int32Rect(0, 0, .PixelWidth, .PixelHeight)

                    '    'Dim pixels As Byte() _
                    '    '    = ToPixelData(palette)

                    '    Dim pixels As Byte() _
                    '        = ToPixels

                    '    .WritePixels(sourceRect, pixels, .PixelWidth, 0) '.PixelWidth * 3, 0)
                    '    '.Unlock()
                    'End With

                    'Return BitmapFromWriteableBitmap(_bitmap)

                    'Dim _bitmap As Bitmap

                    '_bitmap = New Bitmap(_size.Width, _size.Height, Imaging.PixelFormat.Format32bppArgb,)
                    'Dim source(_size.Width * _size.Height - 1) As Integer

                    'For y As Integer = 0 To _size.Width - 1
                    '    For x As Integer = 0 To 7
                    '        source((y * _size.Width) + x) = palette(ColorMap(x, y))
                    '    Next
                    'Next

                    'Dim bd As Imaging.BitmapData =
                    '    _bitmap.LockBits(New Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
                    '    Imaging.ImageLockMode.ReadWrite,
                    '    Imaging.PixelFormat.Format32bppArgb)

                    'Runtime.InteropServices.Marshal.Copy(source, 0, bd.Scan0, source.Length)
                    '_bitmap.UnlockBits(bd)

                    'Return _bitmap.Clone

                End Function

            End Class

        End Class

    End Namespace

    Namespace MegaCD
        Public Class System
            'Constants for Sega CD clock frequencies
            Shared ReadOnly Property CrystalFrequency As Double
                Get
                    Return 50000000
                End Get
            End Property

            Shared ReadOnly Property CPUFrequency As Double
                Get
                    Return CrystalFrequency / 4
                End Get
            End Property

            Shared ReadOnly Property PCMFrequencyMax As Double
                Get
                    Return CPUFrequency / 384
                End Get
            End Property

            Shared ReadOnly Property PCMFrequencyIncrement As Double
                Get
                    Return PCMFrequencyMax / 2048
                End Get
            End Property
        End Class

        'Added on 2014/11/27: "Real" Sega CD Font Data Generator
        Public Class FontDataGenerator
            Private _sourceColors As Byte
            Public Property SourceColors As Byte
                Get
                    Return _sourceColors
                End Get
                Set(value As Byte)
                    _sourceColors = value
                    'Added on 2015/1/15: Set source pattern to all zeroes on source color input
                    For i As Integer = 0 To 1
                        _sourcePatternBytes(i) = 0
                    Next
                    _outputPatternNeedsUpdate = True
                End Set
            End Property
            Public ReadOnly Property SourceColor(ByVal index As Integer) As Byte
                Get
                    index = Math.Max(index, 0)
                    index = Math.Min(index, 1)
                    Return (_sourceColors >> (4 * index)) And 15
                End Get
            End Property

            Private _sourcePatternBytes(1) As Byte
            Public Property SourcePatternMSB() As Byte
                Get
                    Return _sourcePatternBytes(0)
                End Get
                Set(value As Byte)
                    _sourcePatternBytes(0) = value
                    _outputPatternNeedsUpdate = True
                End Set
            End Property
            Public Property SourcePatternLSB() As Byte
                Get
                    Return _sourcePatternBytes(1)
                End Get
                Set(value As Byte)
                    _sourcePatternBytes(1) = value
                    _outputPatternNeedsUpdate = True
                End Set
            End Property

            Private _outputPattern(7) As Byte
            Private _outputPatternNeedsUpdate As Boolean = True
            Public ReadOnly Property OutputPattern As Byte()
                Get
                    If _outputPatternNeedsUpdate = True Then
                        For i As Integer = 0 To 7
                            _outputPattern(i) = 0
                        Next

                        Dim sourcePattern As UShort _
                            = (CUShort(SourcePatternMSB) << 8) + CUShort(SourcePatternLSB)

                        Dim patternBit As UShort

                        For fontBitPos As Integer = 15 To 0 Step -1
                            patternBit = (sourcePattern >> fontBitPos) And 1
                            _outputPattern(7 - (fontBitPos \ 2)) += (SourceColor(patternBit) << (4 * (fontBitPos Mod 2)))
                        Next

                        _outputPatternNeedsUpdate = True
                    End If
                    Return _outputPattern
                End Get
            End Property

        End Class

    End Namespace

    Namespace Super32X
        'Added on 2014/12/2: "Real" 32X video hardware support (8bpp mode only)
        Public Class VDP
            Inherits Sega.VDP

            Protected Overrides ReadOnly Property FullColorPaletteAsColors As Color() 'Color()
                Get
                    If _fullColorPaletteAsColors Is Nothing Then
                        ReDim _fullColorPaletteAsColors(65535)
                        For R As Integer = 0 To 31
                            For G As Integer = 0 To 31
                                For B As Integer = 0 To 31
                                    Dim index As Integer _
                                        = (R << 10) + (G << 5) + B
                                    Dim color As Color _
                                        = Color.FromArgb((R << 3) + ((R >> 2) And 7), (G << 3) + ((G >> 2) And 7), (B << 3) + ((B >> 2) And 7))
                                    '_fullColorPaletteAsColors((R << 10) + (G << 5) + B) = Color.FromArgb((R << 3) + ((R >> 2) And 7), (G << 3) + ((G >> 2) And 7), (B << 3) + ((B >> 2) And 7)) 'Color.FromRgb((R << 3) + ((R >> 2) And 7), (G << 3) + ((G >> 2) And 7), (B << 3) + ((B >> 2) And 7))
                                    _fullColorPaletteAsColors(index) = color
                                    _fullColorPaletteAsColors(32768 + index) = Color.FromArgb(0, color)
                                Next
                            Next
                        Next
                    End If
                    Return _fullColorPaletteAsColors
                End Get
            End Property

            Protected Overrides Sub SizeChanged()
                MyBase.SizeChanged()
            End Sub

            'Public Sub WriteToFrameBuffer(ByVal value As Byte(), ByVal offset As Integer)
            Public Sub WriteToFrameBuffer(ByVal value As Short(), ByVal offset As Integer)
                Array.Copy(value, 0, _frameBuffer, offset, value.Length)
                InvalidateFrameBuffer()
                UpdateFrameBuffer()
            End Sub

            Protected Overrides Sub UpdateFrameBuffer()
                _frameBufferNeedsUpdate = False
                UpdatePixelData()
            End Sub

            Sub New()
                MyBase.New()
                InitializeCRAM(256)
                InitializeFrameBuffer(1) '(131072)
            End Sub

        End Class

    End Namespace

End Namespace
