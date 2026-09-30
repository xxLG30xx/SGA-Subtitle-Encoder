Namespace SGA

    Public Enum ChunkType As Byte
        Video81 = &H81              'Macroblock encoded indexed 8bpp video (All 32-bit Digital Pictures ports) [!]
        Video82 = &H82              'Macroblock encoded indexed 8bpp video (extensionless files in DOS and Mac ports) [?!]
        'NOTE: These chunks actually have no type code -- it was added by me
        FileHeader85 = &H85         'Appears at beginning of extensionless files in DOS and Mac ports [?!]
        Video8A = &H8A              'Macroblock encoded indexed 8bpp video (Night Trap DOS .AVC files) [?!]
        Tileset99 = &H99            'LZ-compressed 15 bpp tile set with 8192-byte window size and base 1 copy count (Saturn ports) [!]
        AudioA1 = &HA1              '8-bit sign/magnitude PCM [!]
        AudioA2 = &HA2              '8-bit sign/magnitude PCM (3DO and Saturn ports) [!]
        AudioA3 = &HA3              '8-bit unsigned PCM (extensionless files in DOS and Mac ports) [!]
        AudioAA = &HAA              '8-bit sign/magnitude PCM (Night Trap DOS .AVC files) [?!]
        VideoC1 = &HC1              'Raw (Night Trap SCD, Sewer Shark, Corpse Killer SCD) [!]
        VideoC2 = &HC2              'Pattern generator encoded (Corpse Killer SCD, Slam City, Supreme Warrior, Kids on Site) [!?]
        VideoC4 = &HC4              'Interframe version of C2 (Slam City) [!?]
        VideoC6 = &HC6              'LZ compressed version of C1 with 8192-byte window size and base 0 copy count (Night Trap SCD, Sewer Shark, Make My Video) [!]
        VideoC7 = &HC7              'Same as C6 but with base 1 copy count (Prize Fighter) [!]
        VideoC8 = &HC8              'Same as C6 but with pixels swapped on even lines for better compression (Sewer Shark, Make My Video) [!]
        VideoC9 = &HC9              'Same as C8 but with base 1 copy count (Double Switch) [!]
        VideoCB = &HCB              'Same as C6 but with 4096-byte window size and base 1 copy count (Prize Fighter, Double Switch "DPLOGO", Corpse Killer 32X) [!]
        VideoCD = &HCD              'Same as C8 but with 4096-byte window size and base 1 copy count (Double Switch) [!]
        VideoD1 = &HD1              'Raw with encoded tile map, and no palette data, used for enemy death animations (Sewer Shark) [!]
        VideoD2 = &HD2              'Same as D1 but with palette data (Slam City, Supreme Warrior) [!]
        VideoD3 = &HD3              'Raw animation frame with palette and pixel offset data (Corpse Killer SCD / 32X, Slam City) [!]
        VideoD4 = &HD4              'LZ compressed version of D3 with 4096-byte window size and base 1 copy count (Corpse Killer SCD / 32X, Slam City) [!]
        VideoD5 = &HD5              'LZ compressed version of D2 with 4096-byte window size and base 1 copy count (Supreme Warrior) [!]
        VideoD7 = &HD7              'Macroblock encoded version of D3 (Corpse Killer Saturn) [!]
        VideoE7 = &HE7              'Raw/LZ Compressed tri-frame with 4096-byte window size and base 1 copy count (Make My Video series "*BIG.SGA" files) [!]
        VideoE8 = &HE8              'Special compressed keyframe (Ground Zero Texas) [!]
        VideoE9 = &HE9              'Special compressed interframe (Ground Zero Texas) [!]
        HeaderF0 = &HF0             'Sequence header (Slam City) [!?]
        ContainerF1 = &HF1          'Container for other chunks (Slam City, Supreme Warrior) [!]
        HeaderF2 = &HF2             'Video chunks (Slam City, and in other games as *.F2 files) [?] 
        'HeaderF2 = &HF2            'Header for multi-part chunks of type $99 (Saturn ports) [!]
        ContainerF3 = &HF3          'Container for multi-part $99 chunks in Saturn ports [!]
        ContainerF9 = &HF9          'Container for $8A and $AA chunks in (Night Trap DOS .AVC files) [?!]
        FooterFF = &HFF             'Appears after last chunk in a clip (Supreme Warrior, others) [!]
    End Enum

    <Flags()> Public Enum ChunkFlags As Byte
        Unknown01h = 1
        Unknown02h = 2
        PaletteDataFollowsTileData = 4
        Unknown08h = 8
        Unknown10h = 16
        ContainsCompressedTileMap = 32
        IsHighPriority = 64                   'Used when overlaying graphics, e.g. when Ratigators etc. are killed in Sewer Shark
        ContainsTileMap = 128
    End Enum

    Public Class RawChunk
        Public Offset As Long
        Public EndOffset As Long
        Public DataLocations As SortedList(Of Long, Integer)
        Public ByteOrder As ByteOrder
        Public Type As ChunkType
        Public TrackID As Integer
        Public ReportedLength As UShort
        Public Length As Integer
        Public Data As List(Of Byte)
        Public BytesLeft As Integer

        Sub New()
            Data = New List(Of Byte)
        End Sub

    End Class

    Public MustInherit Class Chunk
        Protected _offset As Long
        Public Property Offset As Long
            Get
                Return _offset
            End Get
            Private Set(value As Long)
                _offset = value
            End Set
        End Property

        Protected _endOffset As Long
        Public Property EndOffset As Long
            Get
                Return _endOffset
            End Get
            Private Set(value As Long)
                _endOffset = value
            End Set
        End Property

        Protected _dataLocations As SortedList(Of Long, Integer)
        Public Property DataLocations As SortedList(Of Long, Integer)
            Get
                Return _dataLocations
            End Get
            Private Set(value As SortedList(Of Long, Integer))
                _dataLocations = value
            End Set
        End Property

        Public ReadOnly Property Sector As Integer
            Get
                Return Offset \ CD_ROM_MODE1_BYTES_PER_SECTOR
            End Get
        End Property

        Protected _byteOrder As ByteOrder
        Public Property ByteOrder As ByteOrder
            Get
                Return _byteOrder
            End Get
            Private Set(value As ByteOrder)
                _byteOrder = value
            End Set
        End Property

        Protected _type As ChunkType
        Public Property Type As ChunkType
            Get
                Return _type
            End Get
            Private Set(value As ChunkType)
                _type = value
            End Set
        End Property

        Protected _trackID As Integer
        Public Property TrackID As Integer
            Get
                Return _trackID
            End Get
            Protected Set(value As Integer)
                _trackID = value
            End Set
        End Property

        'Protected _metaData As Byte()
        'Public Property Metadata As Byte()
        '    Get
        '        Return _metaData
        '    End Get
        '    Protected Set(value As Byte())
        '        _metaData = value
        '    End Set
        'End Property

        'Protected _frameData As Byte()
        'Public Property FrameData As Byte()
        '    Get
        '        Return _frameData
        '    End Get
        '    Protected Set(value As Byte())
        '        _frameData = value
        '    End Set
        'End Property

        Protected _data As Byte()
        Public Property Data As Byte()
            Get
                Return _data
            End Get
            Protected Set(value As Byte())
                _data = value
            End Set
        End Property

        Private _reportedLength As UShort
        Public Property ReportedLength As UShort
            Get
                Return _reportedLength
            End Get
            Private Set(value As UShort)
                _reportedLength = value
            End Set
        End Property

        Public ReadOnly Property Length As Integer
            Get
                Return If(Data Is Nothing, 0, Data.Length)
            End Get
        End Property

        Public Overridable ReadOnly Property MetadataLength As Integer
            Get
                Return If(Data Is Nothing, 0, 8)
            End Get
        End Property

        Public Overridable ReadOnly Property Metadata As Byte()
            Get
                If MetadataLength = 0 Then
                    Return Nothing
                Else
                    Dim bytes(MetadataLength - 1) As Byte
                    Array.Copy(Data, 0, bytes, 0, MetadataLength)
                    Return bytes
                End If
            End Get
        End Property

        Public Overridable ReadOnly Property FrameDataLength As Integer
            Get
                Return Length - MetadataLength
            End Get
        End Property

        Public Overridable ReadOnly Property FrameData As Byte()
            Get
                If FrameDataLength = 0 Then
                    Return Nothing
                Else
                    Dim bytes(FrameDataLength - 1) As Byte
                    Array.Copy(Data, MetadataLength, bytes, 0, FrameDataLength)
                    Return bytes
                End If
            End Get
        End Property

        Public Overridable ReadOnly Property Timecode As UInteger
            Get
                If Data Is Nothing Then
                    Return 0
                ElseIf Data.Length < 4 Then
                    Return 0
                Else
                    Dim returnValue As UInteger =
                        (CUInt(Data(0)) << 24) + (CUInt(Data(1)) << 16) + (CUInt(Data(2)) << 8) + CUInt(Data(3))
                    Return returnValue
                End If
            End Get
        End Property

        Protected Sub New(ByRef rawChunk As RawChunk)
            With rawChunk
                Offset = .Offset
                EndOffset = .Offset
                DataLocations = .DataLocations
                ByteOrder = .ByteOrder
                Type = .Type
                TrackID = .TrackID
                ReportedLength = .ReportedLength
                Data = .Data.ToArray
            End With
        End Sub

        Public Function ToRawChunk() As RawChunk
            Dim rawChunk As New RawChunk()
            With rawChunk
                .Offset = Offset
                .EndOffset = Offset
                .DataLocations = DataLocations
                .ByteOrder = ByteOrder
                .Type = Type
                .TrackID = TrackID
                .ReportedLength = ReportedLength
                .Data = Data.ToList
            End With
            Return rawChunk
        End Function

        Public Shared Function GetNewChunk(ByRef rawChunk As RawChunk) As Chunk
            Select Case rawChunk.Type
                Case ChunkType.AudioA1
                    Return New ChunkA1(rawChunk)
                Case ChunkType.AudioA2
                    Return New ChunkA2(rawChunk)
                Case ChunkType.AudioA3
                    Return New ChunkA3(rawChunk)
                Case ChunkType.AudioAA
                    Return New ChunkAA(rawChunk)
                Case ChunkType.Video81
                    Return New Chunk81(rawChunk)
                Case ChunkType.Video82
                    Return New Chunk82(rawChunk)
                Case ChunkType.Video8A
                    Return New Chunk81(rawChunk)
                Case ChunkType.Tileset99
                    Return New Chunk99(rawChunk)
                Case ChunkType.VideoC1
                    Return New ChunkC1(rawChunk)
                Case ChunkType.VideoC2
                    Return New ChunkC2(rawChunk)
                    'Return New ChunkC2(rawChunk).ToChunkC1
                Case ChunkType.VideoC4
                    Return New ChunkC4(rawChunk)
                Case ChunkType.VideoC6
                    Return New ChunkC6(rawChunk)
                Case ChunkType.VideoC7
                    Return New ChunkC7(rawChunk)
                Case ChunkType.VideoC8
                    Return New ChunkC8(rawChunk)
                Case ChunkType.VideoC9
                    Return New ChunkC9(rawChunk)
                Case ChunkType.VideoCB
                    Return New ChunkCB(rawChunk)
                Case ChunkType.VideoCD
                    Return New ChunkCD(rawChunk)
                Case ChunkType.VideoD1
                    Return New ChunkD1(rawChunk)
                Case ChunkType.VideoD2
                    Return New ChunkD2(rawChunk)
                Case ChunkType.VideoD3
                    Return New ChunkD3(rawChunk)
                Case ChunkType.VideoD4
                    Return New ChunkD4(rawChunk)
                Case ChunkType.VideoD5
                    Return New ChunkD5(rawChunk)
                Case ChunkType.VideoD7
                    Return New ChunkD7(rawChunk)
                Case ChunkType.VideoE7
                    Return New ChunkE7(rawChunk)
                Case ChunkType.VideoE8
                    Return New ChunkE8(rawChunk)
                Case ChunkType.VideoE9
                    Return New ChunkE9(rawChunk)
                Case ChunkType.HeaderF2
                    'Check the metadata for the string ".clp"
                    Dim bytes(11) As Byte
                    Array.Copy(rawChunk.Data.ToArray, 0, bytes, 0, 12)
                    Dim s As String = ""
                    For i As Integer = 0 To 11
                        s &= Chr(bytes(i))
                    Next
                    If s.Contains(".clp") Then
                        Return New ChunkF2(rawChunk)
                    Else
                        Return New Chunk99Part(rawChunk)
                    End If
                Case ChunkType.ContainerF1
                    Return New ChunkF1(rawChunk)
                Case ChunkType.ContainerF9
                    Return New ChunkF9(rawChunk)
                Case ChunkType.FooterFF
                    Return New ChunkFF(rawChunk)
                Case Else
                    Debug.WriteLine($"Encountered unsupported chunk type ${Convert.ToByte(rawChunk.Type):X2}.")
                    Return New UnsupportedChunk(rawChunk)
            End Select
        End Function

        Public Overridable Function InfoToString(ByVal includeData As Boolean) As String
            With Me
                Dim chunkInfo As String =
                    $"${ .Offset:X8} {CByte(.Type):X2} { .TrackID:X4} Len={ .ReportedLength:X4} T={CUInt(.Timecode):X8}"
                If includeData = True Then
                    chunkInfo &=
                        ControlChars.CrLf & ControlChars.CrLf & "Data:" & ControlChars.CrLf &
                        DataToString(Data, MetadataLength)
                End If
                Return chunkInfo
            End With

        End Function

    End Class

    Public MustInherit Class VideoChunk
        Inherits Chunk

        Public Overridable ReadOnly Property SizeInTiles As Size
            Get
                Return New Size(CInt(Data(6)), CInt(Data(7)))
            End Get
        End Property

        Public Overridable ReadOnly Property UniqueTileCount As Integer
            Get
                Return CInt(SizeInTiles.Width) * CInt(SizeInTiles.Height)
            End Get
        End Property

        Public Overridable ReadOnly Property SizeInPixels As Size
            Get
                Return New Size(CInt(SizeInTiles.Width) * 8, CInt(SizeInTiles.Height) * 8)
            End Get
        End Property

        Protected _paletteAsCRAM As UShort()
        Public MustOverride ReadOnly Property PaletteAsCRAM As UShort()

        '<Flags()> Public Enum PaletteInheritanceType As Integer
        '    None
        '    Can
        '    Required
        'End Enum

        Public Sub InheritPalette(ByVal inheritedPalette As UShort())
            _paletteAsCRAM = inheritedPalette
        End Sub

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)

            'Dim metadataLength As Integer = Me.MetadataLength

            ''Added on 2014/9/9: Support for separating metadata from frame data
            'ReDim .Metadata(Me.MetadataLength - 1)
            'Array.Copy(data, .Metadata, .Metadata.Length)
        End Sub

        Public Overrides Function InfoToString(ByVal includeData As Boolean) As String
            With Me
                Dim chunkInfo As String =
                    MyBase.InfoToString(False) & ControlChars.CrLf &
                    $"{ .SizeInTiles.Width}×{ .SizeInTiles.Height} tiles ({ .SizeInPixels.Width}×{ .SizeInPixels.Height} pixels)"
                Return chunkInfo
            End With
        End Function

    End Class

    Public MustInherit Class AudioChunk
        Inherits Chunk

        Public MustOverride ReadOnly Property SampleRate As Double 'Integer

        Public ReadOnly Property FrameRate As Double
            Get
                Dim sampleCount As Integer _
                    = RawSamples.Length
                If sampleCount = 0 Then
                    Return -1
                Else
                    Return 1 / Duration.TotalSeconds
                End If
                'If AudioData.Length = 0 Then
                '    Return 12
                'End If
                'Return Convert.ToDouble(SamplesPerSecond / AudioData.Length)
            End Get
        End Property

        Public ReadOnly Property Duration As TimeSpan
            Get
                Dim sampleCount As Integer _
                    = RawSamples.Length
                Dim frameDuration As Double _
                    = CDbl(sampleCount) / CDbl(SampleRate)
                Return New TimeSpan(frameDuration * 10000000.0)
            End Get
        End Property

        Public ReadOnly Property RawSamples As Byte()
            Get
                Dim len As Integer =
                    Data.Length - MetadataLength

                'Added on 2015/11/28: Fix for popping audio
                If Data.Last = 0 Then len -= 1

                'Added on 2016/4/1: Fix for zero-length chunks
                If len <= 0 Then
                    Return {0}
                End If

                Dim samples(len - 1) As Byte
                Array.Copy(Data, MetadataLength, samples, 0, len)

                'For Each sample As Byte In samples
                '    If sample = 255 Then ' Or sample = 255 Then
                '        Debug.WriteLine($"sample = {sample}")
                '    End If
                'Next

                Return samples
            End Get
        End Property

        Protected _samplesAs8BitUnsigned As Byte()
        Public MustOverride ReadOnly Property SamplesAs8BitUnsigned As Byte()

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

        Public Overrides Function InfoToString(ByVal includeData As Boolean) As String
            Return MyBase.InfoToString(False)
        End Function

    End Class

    Public MustInherit Class Super32XVideoChunk
        Inherits VideoChunk

        Protected Overridable ReadOnly Property PaletteStartOffset As Byte
            Get
                Return Data(4)
            End Get
        End Property

        Protected Overridable ReadOnly Property PaletteEntryCount As Byte
            Get
                Return Data(5)
            End Get
        End Property

        Public Overrides ReadOnly Property PaletteAsCRAM As UShort()
            Get
                If _paletteAsCRAM Is Nothing Then
                    If Me.PaletteEntryCount = 0 Then
                        Return Nothing
                    End If

                    ReDim _paletteAsCRAM(PaletteEntryCount - 1)

                    Dim offset As Integer

                    For i As Integer = 0 To _paletteAsCRAM.Length - 1
                        offset = MetadataLength + (i * 2)

                        Dim bytes(1) As Byte
                        Array.Copy(Data, offset, bytes, 0, 2)
                        If ByteOrder = ByteOrder.BigEndian Then
                            Array.Reverse(bytes)
                        End If
                        _paletteAsCRAM(i) = BitConverter.ToUInt16(bytes, 0)
                    Next
                End If
                Return _paletteAsCRAM
            End Get
        End Property

        Public MustOverride ReadOnly Property FrameBufferData As Short() 'Byte()

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

    End Class

    Public Class Chunk81
        Inherits Super32XVideoChunk

        Protected Overridable ReadOnly Property MacroblockData As Byte()
            Get
                Dim _macroblockData As Byte()
                Dim offset As Integer =
                    MetadataLength + (PaletteEntryCount * 2)
                Dim len As Integer =
                    Data.Length - offset

                ReDim _macroblockData(len - 1)
                Array.Copy(Data, offset, _macroblockData, 0, len)

                Return _macroblockData
            End Get
        End Property

        Public Overridable ReadOnly Property MacroblockList As List(Of Macroblock)
            Get
                Dim _macroblockList As List(Of Macroblock)
                _macroblockList = New List(Of Macroblock)

                Dim macroblockData As Byte() _
                    = Me.MacroblockData

                Dim offset As Integer = 0

                While offset < macroblockData.Length
                    Dim blockType As Byte =
                        macroblockData(offset)
                    offset += 1

                    Dim block As New Macroblock(blockType, Nothing, Nothing) ', ByteOrder)

                    Select Case blockType
                        Case 0
                            _macroblockList.Add(block)
                            Exit While
                        Case >= &H80
                            _macroblockList.Add(block)
                        Case Else
                            Dim colorData As Byte()

                            If block.ColorDataLength > 0 Then
                                If offset + block.ColorDataLength > macroblockData.Length Then
                                    Debug.WriteLine($"{[GetType]().Name}.{Reflection.MethodBase.GetCurrentMethod().Name}: Color data exceeded command data length in chunk @ ${MyBase.Offset:X7}")
                                    Exit While
                                End If

                                ReDim colorData(block.ColorDataLength - 1)
                                Array.Copy(macroblockData, offset, colorData, 0, block.ColorDataLength)

                                If ByteOrder = ByteOrder.LittleEndian Then
                                    colorData = colorData.SwapBytes
                                End If

                                offset += block.ColorDataLength
                            Else
                                colorData = Nothing
                            End If

                            Dim patternData As Byte()

                            If block.PatternDataLength > 0 Then
                                If offset + block.PatternDataLength > macroblockData.Length Then
                                    Debug.WriteLine($"{[GetType]().Name}.{Reflection.MethodBase.GetCurrentMethod().Name}: Layout data exceeded command data length in chunk @ ${MyBase.Offset:X7}")
                                    Exit While
                                End If

                                ReDim patternData(block.PatternDataLength - 1)
                                Array.Copy(macroblockData, offset, patternData, 0, block.PatternDataLength)
                                offset += block.PatternDataLength
                            Else
                                patternData = Nothing
                            End If

                            block = New Macroblock(blockType, colorData, patternData) ', ByteOrder)

                            _macroblockList.Add(block)
                    End Select

                End While

                Return _macroblockList

            End Get
        End Property

        'Shared _blankTile() As Byte _
        '= {255, 255, 255, 255, 255, 255, 255, 255,
        '   255, 255, 255, 255, 255, 255, 255, 255,
        '   255, 255, 255, 255, 255, 255, 255, 255,
        '   255, 255, 255, 255, 255, 255, 255, 255,
        '   255, 255, 255, 255, 255, 255, 255, 255,
        '   255, 255, 255, 255, 255, 255, 255, 255,
        '   255, 255, 255, 255, 255, 255, 255, 255,
        '   255, 255, 255, 255, 255, 255, 255, 255}

        Shared _blankTile As Short() _
        = {-1, -1, -1, -1, -1, -1, -1, -1,
           -1, -1, -1, -1, -1, -1, -1, -1,
           -1, -1, -1, -1, -1, -1, -1, -1,
           -1, -1, -1, -1, -1, -1, -1, -1,
           -1, -1, -1, -1, -1, -1, -1, -1,
           -1, -1, -1, -1, -1, -1, -1, -1,
           -1, -1, -1, -1, -1, -1, -1, -1,
           -1, -1, -1, -1, -1, -1, -1, -1}

        Public Overrides ReadOnly Property FrameBufferData As Short() 'Byte()
            Get
                Dim _frameBufferData(SizeInPixels.Width * SizeInPixels.Height - 1) As Short 'Byte
                Dim frameBufferDrawPos As New Point

                Dim tile(63) As Short 'Byte
                Dim tileDrawPos As New Point(0, 0)

                Dim repeatCount As Integer

                Dim macroblockList As List(Of Macroblock) _
                    = Me.MacroblockList

                Dim block As Macroblock

                For index As Integer = 0 To macroblockList.Count - 1
                    block = macroblockList(index)

                    If block.Type = &H0 Then                        'End of data
                        Exit For
                    ElseIf block.Type >= &H80 Then                  'Repeat code
                        repeatCount = (block.Type And &H7F) + 1
                    Else                                            'Macroblock
                        Select Case block.Type
                            Case &H5B To &H5D
                                Array.Copy(_blankTile, tile, 64)
                            Case Else
                                AddBlockToTile(block, tile, tileDrawPos)
                        End Select

                        UpdateTileDrawPos(tileDrawPos, block.SizeInPixels)

                        If tileDrawPos = Point.Empty Then
                            AddTileToFrameBufferData(tile, _frameBufferData, frameBufferDrawPos, repeatCount)
                        End If

                    End If

                Next

                Return _frameBufferData.Clone
            End Get
        End Property

        Private Sub AddBlockToTile(block As Macroblock, tile As Short(), tileDrawPos As Point)
            Dim frameBufferData As Short() _
                = block.FrameBufferData

            For y = 0 To block.SizeInPixels.Height - 1
                Dim srcIndex As Integer _
                    = y * block.SizeInPixels.Width
                Dim dstIndex As Integer _
                    = (tileDrawPos.Y + y) * 8 + tileDrawPos.X

                Array.Copy(frameBufferData, srcIndex, tile, dstIndex, block.SizeInPixels.Width)
            Next
        End Sub

        Shared Sub UpdateTileDrawPos(ByRef tileDrawPos As Point, sizeInPixels As Size)
            tileDrawPos += sizeInPixels

            With tileDrawPos
                If .X = 8 Then 'If right edge of tile reached
                    If .Y = 8 Then 'If bottom edge of tile reached
                        'TILE FINISHED!!!
                        tileDrawPos = Point.Empty
                    ElseIf .Y = 4 Then 'If bottom edge of tile not reached
                        'Move cursor to left edge of current tile
                        .X -= 8
                    End If
                ElseIf .X = 4 Then 'If right edge of tile not reached
                    'Move cursor to top of tile or tile midpoint
                    .Y -= 4
                End If
            End With

        End Sub

        Private Sub AddTileToFrameBufferData(tile As Short(), frameBufferData As Short(), ByRef frameBufferDrawPos As Point, ByRef repeatCount As Integer)
            Do
                With frameBufferDrawPos
                    Dim destinationIndex As Integer _
                        = .Y + .X

                    For sourceIndex As Integer = 0 To 56 Step 8
                        If destinationIndex >= frameBufferData.Length Then
                            Exit Do
                        End If

                        Array.Copy(tile, sourceIndex, frameBufferData, destinationIndex, 8)
                        destinationIndex += SizeInPixels.Width
                    Next

                    UpdateFrameBufferDrawPos(frameBufferDrawPos)
                End With

                repeatCount -= 1

            Loop Until repeatCount < 0

            repeatCount = 0

        End Sub

        Private Sub UpdateFrameBufferDrawPos(ByRef frameBufferDrawPos As Point)
            With frameBufferDrawPos
                .X += 8
                If .X >= SizeInPixels.Width Then
                    .X = 0
                    .Y += 8 * SizeInPixels.Width
                End If
            End With
        End Sub

        Public Function GetBlockShapeBitmap(Optional ByVal original As Bitmap = Nothing) As Bitmap

            Dim repeatCount As Integer
            Dim tileDrawPos As New Point(0, 0)

            Dim scaleFactor As Integer _
                = 8

            Dim macroblockList As List(Of Macroblock) _
                = Me.MacroblockList

            Dim tileIndex As Integer = 0

            Using b1 As New Bitmap(SizeInPixels.Width * scaleFactor, SizeInPixels.Height * scaleFactor, Imaging.PixelFormat.Format32bppArgb)
                Using g1 As Graphics = Graphics.FromImage(b1)
                    With g1
                        .PixelOffsetMode = Drawing2D.PixelOffsetMode.None
                        .SmoothingMode = Drawing2D.SmoothingMode.None
                    End With

                    If original IsNot Nothing Then
                        g1.DrawImage(original, 0, 0, b1.Width, b1.Height)
                    End If

                    Using b2 As New Bitmap(8 * scaleFactor, 8 * scaleFactor, Imaging.PixelFormat.Format32bppArgb)
                        Using g2 As Graphics = Graphics.FromImage(b2)

                            For Each block As Macroblock In macroblockList
                                If block.Type = &H0 Then
                                    Exit For
                                ElseIf block.Type >= &H80 Then
                                    'repeatCount = cmd.RepeatCount
                                    repeatCount = (block.Type And &H7F) + 1
                                Else
                                    Dim rect As New Rectangle(
                                        tileDrawPos.X * scaleFactor,
                                        tileDrawPos.Y * scaleFactor,
                                        block.SizeInPixels.Width * scaleFactor,
                                        block.SizeInPixels.Height * scaleFactor
                                        )
                                    Using brush As New SolidBrush(Color.FromArgb(63, Color.Blue))
                                        g2.FillRectangle(brush, rect)
                                    End Using
                                    Using pen As New Pen(Brushes.White, 1)
                                        g2.DrawRectangle(pen, rect)
                                    End Using
                                    Using font As New Font("Consolas", 10)
                                        g2.DrawString($"{block.Type:X2}", font, Brushes.White, rect.Location)
                                    End Using

                                    UpdateTileDrawPos(tileDrawPos, block.SizeInPixels)

                                    If tileDrawPos = Point.Empty Then
                                        Do
                                            Dim point As New Point(
                                                (tileIndex Mod SizeInTiles.Width) * 8 * scaleFactor,
                                                (tileIndex \ SizeInTiles.Width) * 8 * scaleFactor
                                                )

                                            g1.DrawImage(b2, point)
                                            g2.Clear(Color.Transparent)

                                            tileIndex += 1

                                            repeatCount -= 1
                                        Loop Until repeatCount < 0
                                    End If

                                End If
                            Next

                        End Using

                    End Using

                End Using

                Return b1.Clone

            End Using

        End Function

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

        Public Overrides Function InfoToString(ByVal includeData As Boolean) As String
            With Me
                Dim chunkInfo As String =
                    MyBase.InfoToString(False) & vbNewLine &
                    $"{ .PaletteEntryCount} palette entries @ ${ .PaletteStartOffset:X2}"
                If includeData = True Then
                    chunkInfo &= $"{vbNewLine}{vbNewLine}Data:{vbNewLine}{DataToString(Data, MetadataLength)}"
                End If
                Return chunkInfo
            End With
        End Function

        Public Class Macroblock
            Private _type As Byte
            Public ReadOnly Property Type As Byte
                Get
                    Return _type
                End Get
            End Property

            Private _colorData As Byte()
            Public ReadOnly Property ColorData As Byte()
                Get
                    'If ByteOrder = ByteOrder.LittleEndian Then
                    '    Return _colorData.SwapBytes
                    'Else
                    Return _colorData
                    'End If
                End Get
            End Property

            Private _patternData As Byte()
            Public ReadOnly Property PatternData As Byte()
                Get
                    If _patternData Is Nothing Then
                        Return GetFixedPatternData(Type)
                    Else
                        Return _patternData
                    End If

                    'If _patternData Is Nothing Then
                    '    _patternData = FixedPatternData()
                    'End If
                    'Return _patternData
                End Get
            End Property

            'Private _byteOrder As ByteOrder
            'Public ReadOnly Property ByteOrder As ByteOrder
            '    Get
            '        Return _byteOrder
            '    End Get
            'End Property

            Sub New(ByVal type As Byte, ByVal colorData As Byte(), ByVal patternData As Byte()) ', ByVal byteOrder As ByteOrder)
                _type = type
                _colorData = colorData
                _patternData = patternData
                '_byteOrder = ByteOrder
            End Sub

            Shared Function IsValidType(ByVal type As Byte) As Boolean
                Select Case type
                    Case &H0 To &H5D
                        Return True
                    Case Else
                        Return False
                End Select
            End Function

            Public ReadOnly Property ColorDataLength As Integer
                Get
                    Select Case Type
                        Case &H1 To &H8, &HC To &HF, &H1C, &H40 To &H5A
                            Return 2
                        Case &H10, &H19
                            Return 4
                        Case &H11, &H12, &H18, &H1F
                            Return 6
                        Case &H9, &HA, &H13, &H14, &H17, &H1B, &H1D, &H1E, &H2F, &H3F
                            Return 8
                        Case &HB, &H15, &H16, &H1A, &H27, &H2B, &H2D, &H2E, &H37, &H3B, &H3D, &H3E
                            Return 10
                        Case &H23, &H25, &H26, &H29, &H2A, &H2C, &H33, &H35, &H36, &H39, &H3A, &H3C
                            Return 12
                        Case &H21, &H22, &H24, &H28, &H31, &H32, &H34, &H38
                            Return 14
                        Case &H20, &H30
                            Return 16
                        Case Else
                            Return 0
                    End Select
                End Get
            End Property

            Public ReadOnly Property PatternDataLength As Integer
                Get
                    Select Case Type
                        Case &H1, &H2
                            Return 8
                        Case &H8, &H10, &H11, &H15, &H17, &H1A, &H1B, &H1D To &H2F
                            Return 4
                        Case &H9 To &HB, &H12 To &H14, &H16, &H18, &H19, &H1C, &H30 To &H3F
                            Return 2
                        Case Else
                            Return 0
                    End Select
                End Get
            End Property

            Public ReadOnly Property SizeInPixels As Size
                Get
                    Select Case Type
                        Case &H1 To &H7, &HC To &HF, &H40 To &H5B
                            Return New Size(8, 8)
                        Case &H8, &H10, &H11, &H15, &H17, &H1A, &H1B, &H1D To &H2F, &H5C
                            Return New Size(8, 4)
                        Case &H9 To &HB, &H12 To &H14, &H16, &H18, &H19, &H1C, &H30 To &H3F, &H5D
                            Return New Size(4, 4)
                        Case Else
                            Return Nothing
                    End Select
                End Get
            End Property

#Region "Fine Color Lookup"

            Shared FineColorLookupIndexes As New Dictionary(Of Byte, Byte) From
                {
                    {&H3, &H3},
                    {&H4, &H4},
                    {&H5, &H5},
                    {&H6, &H6},
                    {&H7, &H7},
                    {&HC, &HC},
                    {&HD, &HD},
                    {&HE, &HE},
                    {&HF, &HF},
                    {&H40, &H4},
                    {&H41, &H5},
                    {&H42, &H6},
                    {&H43, &H7},
                    {&H44, &HC},
                    {&H45, &HD},
                    {&H46, &HE},
                    {&H47, &HF},
                    {&H48, &H48},
                    {&H49, &H49},
                    {&H4A, &H4A},
                    {&H4B, &H4B},
                    {&H4C, &H4C},
                    {&H4D, &H4D},
                    {&H4E, &H4E},
                    {&H4F, &H4F},
                    {&H50, &H50},
                    {&H51, &H51},
                    {&H52, &H52},
                    {&H53, &H53},
                    {&H54, &H54},
                    {&H55, &H55},
                    {&H56, &H56},
                    {&H57, &H57},
                    {&H58, &H3},
                    {&H59, &H3},
                    {&H5A, &H3},
                    {&H5B, &H5B},
                    {&H5C, &H5C},
                    {&H5D, &H5D}
                }

            Shared FineColorLookupTable As New Dictionary(Of Byte, Byte()) From
                {
                    {&H3, New Byte() {170, 85, 170, 85, 170, 85, 170, 85}},
                    {&H4, New Byte() {255, 255, 170, 85, 170, 85, 0, 0}},
                    {&H5, New Byte() {250, 245, 234, 213, 168, 84, 160, 80}},
                    {&H6, New Byte() {232, 212, 232, 212, 232, 212, 232, 212}},
                    {&H7, New Byte() {160, 84, 170, 84, 170, 213, 234, 245}},
                    {&HC, New Byte() {0, 0, 170, 85, 170, 85, 255, 255}},
                    {&HD, New Byte() {10, 5, 42, 21, 171, 87, 175, 95}},
                    {&HE, New Byte() {43, 23, 43, 23, 43, 23, 43, 23}},
                    {&HF, New Byte() {175, 87, 171, 85, 42, 85, 42, 5}},
                    {&H48, New Byte() {170, 85, 170, 85, 0, 0, 0, 0}},
                    {&H49, New Byte() {170, 84, 168, 80, 160, 64, 128, 0}},
                    {&H4A, New Byte() {160, 80, 160, 80, 160, 80, 160, 80}},
                    {&H4B, New Byte() {128, 0, 160, 64, 168, 80, 170, 84}},
                    {&H4C, New Byte() {0, 0, 0, 0, 85, 170, 85, 170}},
                    {&H4D, New Byte() {0, 1, 2, 5, 10, 21, 42, 85}},
                    {&H4E, New Byte() {10, 5, 10, 5, 10, 5, 10, 5}},
                    {&H4F, New Byte() {42, 85, 10, 21, 2, 5, 0, 1}},
                    {&H50, New Byte() {255, 255, 255, 255, 170, 85, 170, 85}},
                    {&H51, New Byte() {254, 255, 250, 253, 234, 245, 170, 213}},
                    {&H52, New Byte() {250, 245, 250, 245, 250, 245, 250, 245}},
                    {&H53, New Byte() {170, 213, 234, 245, 250, 253, 254, 255}},
                    {&H54, New Byte() {170, 85, 170, 85, 255, 255, 255, 255}},
                    {&H55, New Byte() {171, 85, 175, 87, 191, 95, 255, 127}},
                    {&H56, New Byte() {175, 95, 175, 95, 175, 95, 175, 95}},
                    {&H57, New Byte() {255, 127, 191, 95, 175, 87, 171, 85}},
                    {&H58, New Byte() {85, 170, 85, 170, 85, 170, 85, 170}},
                    {&H5B, New Byte() {0, 0, 0, 0, 0, 0, 0, 0}},
                    {&H5C, New Byte() {0, 0, 0, 0}},
                    {&H5D, New Byte() {0, 0}}
            }

#End Region

            Shared Function GetFixedPatternData(ByVal type As Byte) As Byte()
                If FineColorLookupIndexes.Keys.Contains(type) Then
                    Return FineColorLookupTable(FineColorLookupIndexes(type))
                Else
                    Return Nothing
                End If

                'Dim fixedPatternData As Byte()

                'Select Case type
                '    Case &H3, &H58 To &H5A

                '        '01010101
                '        '10101010
                '        '01010101
                '        '10101010
                '        '01010101
                '        '10101010
                '        '01010101
                '        '10101010

                '        fixedPatternData = {170, 85, 170, 85, 170, 85, 170, 85} '!

                '    Case &H4, &H40

                '        '11111111
                '        '11111111
                '        '01010101
                '        '10101010
                '        '01010101
                '        '10101010
                '        '00000000
                '        '00000000

                '        fixedPatternData = {255, 255, 170, 85, 170, 85, 0, 0} '! 

                '    Case &H5, &H41

                '        '01011111
                '        '10101111
                '        '01010111
                '        '10101011
                '        '00010101
                '        '00101010
                '        '00000101
                '        '00001010

                '        fixedPatternData = {250, 245, 234, 213, 168, 84, 160, 80} '! 

                '    Case &H6, &H42

                '        '00010111
                '        '00101011
                '        '00010111
                '        '00101011
                '        '00010111
                '        '00101011
                '        '00010111
                '        '00101011

                '        fixedPatternData = {232, 212, 232, 212, 232, 212, 232, 212} '! 

                '    Case &H7, &H43

                '        '00000101
                '        '00101010
                '        '01010101
                '        '00101010
                '        '01010101
                '        '10101011
                '        '01010111
                '        '10101111

                '        fixedPatternData = {160, 84, 170, 84, 170, 213, 234, 245} '!

                '    Case &HC, &H44

                '        '00000000
                '        '00000000
                '        '01010101
                '        '10101010
                '        '01010101
                '        '10101010
                '        '11111111
                '        '11111111

                '        fixedPatternData = {0, 0, 170, 85, 170, 85, 255, 255} '!

                '    Case &HD, &H45

                '        '01010000
                '        '10100000
                '        '01010100
                '        '10101000
                '        '11010101
                '        '11101010
                '        '11110101
                '        '11111010

                '        fixedPatternData = {10, 5, 42, 21, 171, 87, 175, 95} '!

                '    Case &HE, &H46

                '        '11010100
                '        '11101000
                '        '11010100
                '        '11101000
                '        '11010100
                '        '11101000
                '        '11010100
                '        '11101000

                '        fixedPatternData = {43, 23, 43, 23, 43, 23, 43, 23} '!

                '    Case &HF, &H47

                '        '11110101
                '        '11101010
                '        '11010101
                '        '10101010
                '        '01010100
                '        '10101010
                '        '01010100
                '        '10100000

                '        fixedPatternData = {175, 87, 171, 85, 42, 85, 42, 5} '!

                '    Case &H48

                '        '01010101
                '        '10101010
                '        '01010101
                '        '10101010
                '        '00000000
                '        '00000000
                '        '00000000
                '        '00000000

                '        fixedPatternData = {170, 85, 170, 85, 0, 0, 0, 0} '!

                '    Case &H49

                '        '01010101
                '        '00101010
                '        '00010101
                '        '00001010
                '        '00000101
                '        '00000010
                '        '00000001
                '        '00000000

                '        fixedPatternData = {170, 84, 168, 80, 160, 64, 128, 0} '!

                '    Case &H4A

                '        '00000101
                '        '00001010
                '        '00000101
                '        '00001010
                '        '00000101
                '        '00001010
                '        '00000101
                '        '00001010

                '        fixedPatternData = {160, 80, 160, 80, 160, 80, 160, 80} '!

                '    Case &H4B

                '        '00000001
                '        '00000000
                '        '00000101
                '        '00000010
                '        '00010101
                '        '00001010
                '        '01010101
                '        '00101010

                '        fixedPatternData = {128, 0, 160, 64, 168, 80, 170, 84} '!

                '    Case &H4C

                '        '00000000
                '        '00000000
                '        '00000000
                '        '00000000
                '        '10101010
                '        '01010101
                '        '10101010
                '        '01010101

                '        fixedPatternData = {0, 0, 0, 0, 85, 170, 85, 170} '!

                '    Case &H4D

                '        '00000000
                '        '10000000
                '        '01000000
                '        '10100000
                '        '01010000
                '        '10101000
                '        '01010100
                '        '10101010

                '        fixedPatternData = {0, 1, 2, 5, 10, 21, 42, 85} '!

                '    Case &H4E

                '        '01010000
                '        '10100000
                '        '01010000
                '        '10100000
                '        '01010000
                '        '10100000
                '        '01010000
                '        '10100000

                '        fixedPatternData = {10, 5, 10, 5, 10, 5, 10, 5} '!

                '    Case &H4F

                '        '01010100
                '        '10101010
                '        '01010000
                '        '10101000
                '        '01000000
                '        '10100000
                '        '00000000
                '        '10000000

                '        fixedPatternData = {42, 85, 10, 21, 2, 5, 0, 1} '!

                '    Case &H50

                '        '11111111
                '        '11111111
                '        '11111111
                '        '11111111
                '        '01010101
                '        '10101010
                '        '01010101
                '        '10101010

                '        fixedPatternData = {255, 255, 255, 255, 170, 85, 170, 85} '!

                '    Case &H51

                '        '01111111
                '        '11111111
                '        '01011111
                '        '10111111
                '        '01010111
                '        '10101111
                '        '01010101
                '        '10101011

                '        fixedPatternData = {254, 255, 250, 253, 234, 245, 170, 213} '!

                '    Case &H52

                '        '01011111
                '        '10101111
                '        '01011111
                '        '10101111
                '        '01011111
                '        '10101111
                '        '01011111
                '        '10101111

                '        fixedPatternData = {250, 245, 250, 245, 250, 245, 250, 245} '!

                '    Case &H53

                '        '01010101
                '        '10101011
                '        '01010111
                '        '10101111
                '        '01011111
                '        '10111111
                '        '01111111
                '        '11111111

                '        fixedPatternData = {170, 213, 234, 245, 250, 253, 254, 255} '!

                '    Case &H54

                '        '01010101
                '        '10101010
                '        '01010101
                '        '10101010
                '        '11111111
                '        '11111111
                '        '11111111
                '        '11111111

                '        fixedPatternData = {170, 85, 170, 85, 255, 255, 255, 255} '!

                '    Case &H55

                '        '11010101
                '        '10101010
                '        '11110101
                '        '11101010
                '        '11111101
                '        '11111010
                '        '11111111
                '        '11111110

                '        fixedPatternData = {171, 85, 175, 87, 191, 95, 255, 127} '!

                '    Case &H56

                '        '11110101
                '        '11111010
                '        '11110101
                '        '11111010
                '        '11110101
                '        '11111010
                '        '11110101
                '        '11111010

                '        fixedPatternData = {175, 95, 175, 95, 175, 95, 175, 95} '!

                '    Case &H57

                '        '11111111
                '        '11111110
                '        '11111101
                '        '11111010
                '        '11110101
                '        '11101010
                '        '11010101
                '        '10101010

                '        fixedPatternData = {255, 127, 191, 95, 175, 87, 171, 85} '!

                '    Case &H5B

                '        '00000000
                '        '00000000
                '        '00000000
                '        '00000000
                '        '00000000
                '        '00000000
                '        '00000000
                '        '00000000

                '        fixedPatternData = {0, 0, 0, 0, 0, 0, 0, 0} '!

                '    Case &H5C
                '        '00000000
                '        '00000000
                '        '00000000
                '        '00000000
                '        fixedPatternData = {0, 0, 0, 0}

                '    Case &H5D
                '        '0000
                '        '0000
                '        '0000
                '        '0000
                '        fixedPatternData = {0, 0}

                '    Case Else
                '        fixedPatternData = Nothing
                'End Select

                'Return fixedPatternData

            End Function

            Private Function GetPatternDataFromBitstrings(ByVal bitStrings As String()) As Byte()
                Dim stringCount As Integer =
                    bitStrings.Count

                Dim patternBytes(stringCount - 1) As Byte

                For i As Integer = 0 To stringCount - 1
                    Dim bitString As String _
                        = bitStrings(i)
                    patternBytes(i) = GetPatternDataFromBitstring(bitString)
                Next

                Return patternBytes
            End Function

            Private Function GetPatternDataFromBitstring(ByVal bitString As String) As Byte
                bitString = bitString.Reverse
                Return Convert.ToByte(bitString, 2)
            End Function

            Private Function GetBitstringFromPatternData(ByVal patternByte As Byte) As String
                Dim bitString As String _
                    = Convert.ToString(patternByte, 2)
                Return bitString.Reverse
            End Function

#Region "Coarse Color Lookup"

            Shared CoarseColorLookupIndexes As New Dictionary(Of Byte, Byte) From
                {
                    {&H1, &H1},
                    {&H2, &H1},
                    {&H3, &H1},
                    {&H4, &H1},
                    {&H5, &H1},
                    {&H6, &H1},
                    {&H7, &H1},
                    {&H8, &H8},
                    {&H9, &H9},
                    {&HA, &HA},
                    {&HB, &HB},
                    {&HC, &H1},
                    {&HD, &H1},
                    {&HE, &H1},
                    {&HF, &H1},
                    {&H10, &H10},
                    {&H11, &H11},
                    {&H12, &H12},
                    {&H13, &H13},
                    {&H14, &H14},
                    {&H15, &H15},
                    {&H16, &H16},
                    {&H17, &H17},
                    {&H18, &H18},
                    {&H19, &H19},
                    {&H1A, &H1A},
                    {&H1B, &H1B},
                    {&H1C, &H1C},
                    {&H1D, &H1D},
                    {&H1E, &H1E},
                    {&H1F, &H1F},
                    {&H20, &H20},
                    {&H21, &H21},
                    {&H22, &H22},
                    {&H23, &H23},
                    {&H24, &H24},
                    {&H25, &H25},
                    {&H26, &H26},
                    {&H27, &H27},
                    {&H28, &H28},
                    {&H29, &H29},
                    {&H2A, &H2A},
                    {&H2B, &H2B},
                    {&H2C, &H2C},
                    {&H2D, &H2D},
                    {&H2E, &H2E},
                    {&H2F, &H2F},
                    {&H30, &H30},
                    {&H31, &H31},
                    {&H32, &H32},
                    {&H33, &H33},
                    {&H34, &H34},
                    {&H35, &H35},
                    {&H36, &H36},
                    {&H37, &H37},
                    {&H38, &H38},
                    {&H39, &H39},
                    {&H3A, &H3A},
                    {&H3B, &H3B},
                    {&H3C, &H3C},
                    {&H3D, &H3D},
                    {&H3E, &H3E},
                    {&H3F, &H3F},
                    {&H40, &H1},
                    {&H41, &H1},
                    {&H42, &H1},
                    {&H43, &H1},
                    {&H44, &H1},
                    {&H45, &H1},
                    {&H46, &H1},
                    {&H47, &H1},
                    {&H48, &H1},
                    {&H49, &H1},
                    {&H4A, &H1},
                    {&H4B, &H1},
                    {&H4C, &H1},
                    {&H4D, &H1},
                    {&H4E, &H1},
                    {&H4F, &H1},
                    {&H50, &H1},
                    {&H51, &H1},
                    {&H52, &H1},
                    {&H53, &H1},
                    {&H54, &H1},
                    {&H55, &H1},
                    {&H56, &H1},
                    {&H57, &H1},
                    {&H58, &H1},
                    {&H59, &H1},
                    {&H5A, &H1},
                    {&H5B, &H1},
                    {&H5C, &H8},
                    {&H5D, &H1C}
                }

            Shared CoarseColorLookupTable As New Dictionary(Of Byte, Byte()) From
                {
                    {&H1, New Byte() {0, 0, 0, 0,
                                      0, 0, 0, 0,
                                      0, 0, 0, 0,
                                      0, 0, 0, 0,
                                      0, 0, 0, 0,
                                      0, 0, 0, 0,
                                      0, 0, 0, 0,
                                      0, 0, 0, 0}
                    }
                }

#End Region

            Public ReadOnly Property ColorIndexes() As Byte()
                Get
                    Dim _colorIndexes As Byte()

                    Select Case Type
                        Case &H1 To &H7, &HC To &HF, &H40 To &H5B
                            _colorIndexes =
                                    {0, 0, 0, 0,
                                     0, 0, 0, 0,
                                     0, 0, 0, 0,
                                     0, 0, 0, 0,
                                     0, 0, 0, 0,
                                     0, 0, 0, 0,
                                     0, 0, 0, 0,
                                     0, 0, 0, 0}
                            'Case &H2, &H8, &H5C
                        Case &H8, &H5C
                            _colorIndexes =
                                    {0, 0, 0, 0,
                                     0, 0, 0, 0,
                                     0, 0, 0, 0,
                                     0, 0, 0, 0}
                        Case &H9
                            _colorIndexes =
                                    {0, 1,
                                     2, 1,
                                     3, 3,
                                     3, 3}
                        Case &HA
                            _colorIndexes =
                                    {0, 1,
                                     0, 2,
                                     3, 3,
                                     3, 3}
                        Case &HB
                            _colorIndexes =
                                    {0, 1,
                                     2, 3,
                                     4, 4,
                                     4, 4}
                        Case &H10
                            _colorIndexes =
                                    {0, 0, 1, 1,
                                     0, 0, 1, 1,
                                     0, 0, 1, 1,
                                     0, 0, 1, 1}
                        Case &H11
                            _colorIndexes =
                                    {0, 0, 1, 1,
                                     0, 0, 1, 1,
                                     0, 0, 2, 2,
                                     0, 0, 2, 2}
                        Case &H12
                            _colorIndexes =
                                    {0, 1,
                                     0, 1,
                                     2, 2,
                                     2, 2}
                        Case &H13
                            _colorIndexes =
                                    {0, 0,
                                     0, 0,
                                     1, 2,
                                     3, 2}
                        Case &H14
                            _colorIndexes =
                                    {0, 0,
                                     0, 0,
                                     1, 2,
                                     1, 3}
                        Case &H15
                            _colorIndexes =
                                    {0, 1, 2, 2,
                                     0, 1, 2, 2,
                                     3, 4, 2, 2,
                                     3, 4, 2, 2}
                        Case &H16
                            _colorIndexes =
                                    {0, 0,
                                     0, 0,
                                     1, 2,
                                     3, 4}
                        Case &H17
                            _colorIndexes =
                                    {0, 1, 2, 2,
                                     0, 1, 2, 2,
                                     3, 3, 2, 2,
                                     3, 3, 2, 2}
                        Case &H18
                            _colorIndexes =
                                    {0, 0,
                                     0, 0,
                                     1, 2,
                                     1, 2}
                        Case &H19
                            _colorIndexes =
                                    {0, 0,
                                     0, 0,
                                     1, 1,
                                     1, 1}
                        Case &H1A
                            _colorIndexes =
                                    {0, 0, 1, 2,
                                     0, 0, 1, 2,
                                     0, 0, 3, 4,
                                     0, 0, 3, 4}
                        Case &H1B
                            _colorIndexes =
                                    {0, 0, 1, 2,
                                     0, 0, 1, 2,
                                     0, 0, 3, 3,
                                     0, 0, 3, 3}
                        Case &H1C, &H5D
                            _colorIndexes =
                                    {0, 0,
                                     0, 0,
                                     0, 0,
                                     0, 0}
                        Case &H1D
                            _colorIndexes =
                                    {0, 0, 1, 1,
                                     0, 0, 1, 1,
                                     2, 3, 1, 1,
                                     2, 3, 1, 1}
                        Case &H1E
                            _colorIndexes =
                                    {0, 0, 1, 1,
                                     0, 0, 1, 1,
                                     0, 0, 2, 3,
                                     0, 0, 2, 3}
                        Case &H1F
                            _colorIndexes =
                                    {0, 0, 1, 1,
                                     0, 0, 1, 1,
                                     2, 2, 1, 1,
                                     2, 2, 1, 1}
                        Case &H20
                            _colorIndexes =
                                    {0, 1, 2, 3,
                                     0, 1, 2, 3,
                                     4, 5, 6, 7,
                                     4, 5, 6, 7}
                        Case &H21
                            _colorIndexes =
                                    {0, 1, 2, 3,
                                     0, 1, 2, 3,
                                     4, 5, 6, 6,
                                     4, 5, 6, 6}
                        Case &H22
                            _colorIndexes =
                                    {0, 1, 2, 3,
                                     0, 1, 2, 3,
                                     4, 4, 5, 6,
                                     4, 4, 5, 6}
                        Case &H23
                            _colorIndexes =
                                    {0, 1, 2, 3,
                                     0, 1, 2, 3,
                                     4, 4, 5, 5,
                                     4, 4, 5, 5}
                        Case &H24
                            _colorIndexes =
                                    {0, 1, 2, 2,
                                     0, 1, 2, 2,
                                     3, 4, 5, 6,
                                     3, 4, 5, 6}
                        Case &H25
                            _colorIndexes =
                                    {0, 1, 2, 2,
                                     0, 1, 2, 2,
                                     3, 4, 5, 5,
                                     3, 4, 5, 5}
                        Case &H26
                            _colorIndexes =
                                    {0, 1, 2, 2,
                                     0, 1, 2, 2,
                                     3, 3, 4, 5,
                                     3, 3, 4, 5}
                        Case &H27
                            _colorIndexes =
                                    {0, 1, 2, 2,
                                     0, 1, 2, 2,
                                     3, 3, 4, 4,
                                     3, 3, 4, 4}
                        Case &H28
                            _colorIndexes =
                                    {0, 0, 1, 2,
                                     0, 0, 1, 2,
                                     3, 4, 5, 6,
                                     3, 4, 5, 6}
                        Case &H29
                            _colorIndexes =
                                    {0, 0, 1, 2,
                                     0, 0, 1, 2,
                                     3, 4, 5, 5,
                                     3, 4, 5, 5}
                        Case &H2A
                            _colorIndexes =
                                    {0, 0, 1, 2,
                                     0, 0, 1, 2,
                                     3, 3, 4, 5,
                                     3, 3, 4, 5}
                        Case &H2B
                            _colorIndexes =
                                    {0, 0, 1, 2,
                                     0, 0, 1, 2,
                                     3, 3, 4, 4,
                                     3, 3, 4, 4}
                        Case &H2C
                            _colorIndexes =
                                    {0, 0, 1, 1,
                                     0, 0, 1, 1,
                                     2, 3, 4, 5,
                                     2, 3, 4, 5}
                        Case &H2D
                            _colorIndexes =
                                    {0, 0, 1, 1,
                                     0, 0, 1, 1,
                                     2, 3, 4, 4,
                                     2, 3, 4, 4}
                        Case &H2E
                            _colorIndexes =
                                    {0, 0, 1, 1,
                                     0, 0, 1, 1,
                                     2, 2, 3, 4,
                                     2, 2, 3, 4}
                        Case &H2F
                            _colorIndexes =
                                    {0, 0, 1, 1,
                                     0, 0, 1, 1,
                                     2, 2, 3, 3,
                                     2, 2, 3, 3}
                        Case &H30
                            _colorIndexes =
                                    {0, 1,
                                     2, 3,
                                     4, 5,
                                     6, 7}
                        Case &H31
                            _colorIndexes =
                                    {0, 1,
                                     2, 3,
                                     4, 5,
                                     6, 5}
                        Case &H32
                            _colorIndexes =
                                    {0, 1,
                                     2, 3,
                                     4, 5,
                                     4, 6}
                        Case &H33
                            _colorIndexes =
                                    {0, 1,
                                     2, 3,
                                     4, 5,
                                     4, 5}
                        Case &H34
                            _colorIndexes =
                                    {0, 1,
                                     2, 1,
                                     3, 4,
                                     5, 6}
                        Case &H35
                            _colorIndexes =
                                    {0, 1,
                                     2, 1,
                                     3, 4,
                                     5, 4}
                        Case &H36
                            _colorIndexes =
                                    {0, 1,
                                     2, 1,
                                     3, 4,
                                     3, 5}
                        Case &H37
                            _colorIndexes =
                                    {0, 1,
                                     2, 1,
                                     3, 4,
                                     3, 4}
                        Case &H38
                            _colorIndexes =
                                    {0, 1,
                                     0, 2,
                                     3, 4,
                                     5, 6}
                        Case &H39
                            _colorIndexes =
                                    {0, 1,
                                     0, 2,
                                     3, 4,
                                     5, 4}
                        Case &H3A
                            _colorIndexes =
                                    {0, 1,
                                     0, 2,
                                     3, 4,
                                     3, 5}
                        Case &H3B
                            _colorIndexes =
                                    {0, 1,
                                     0, 2,
                                     3, 4,
                                     3, 4}
                        Case &H3C
                            _colorIndexes =
                                    {0, 1,
                                     0, 1,
                                     2, 3,
                                     4, 5}
                        Case &H3D
                            _colorIndexes =
                                    {0, 1,
                                     0, 1,
                                     2, 3,
                                     4, 3}
                        Case &H3E
                            _colorIndexes =
                                    {0, 1,
                                     0, 1,
                                     2, 3,
                                     2, 4}
                        Case &H3F
                            _colorIndexes =
                                    {0, 1,
                                     0, 1,
                                     2, 3,
                                     2, 3}
                        Case Else
                            _colorIndexes = Nothing
                    End Select

                    'If ByteOrder = ByteOrder.LittleEndian Then
                    '    Return _colorDataIndexes.SwapBytes
                    'Else
                    Return _colorIndexes
                    'End If

                End Get
            End Property

            Private ReadOnly Property PatternBits() As Byte()
                Get
                    Dim _patternBits(SizeInPixels.Width * SizeInPixels.Height - 1) As Byte

                    Dim patternData As Byte() _
                        = Me.PatternData

                    'Parallel.For(0, _patternBits.Length \ 8,
                    '    Sub(byteIndex As Integer)
                    '        Dim patternBitIndex As Integer = byteIndex * 8
                    '        For bitIndex As Integer = 0 To 7
                    '            _patternBits(patternBitIndex) = (patternData(byteIndex) >> bitIndex) And 1
                    '            patternBitIndex += 1
                    '        Next
                    '    End Sub)

                    Dim patternBitIndex As Integer = 0

                    For byteIndex As Integer = 0 To (_patternBits.Length \ 8) - 1
                        For bitIndex As Integer = 0 To 7
                            _patternBits(patternBitIndex) = (patternData(byteIndex) >> bitIndex) And 1
                            patternBitIndex += 1
                        Next
                    Next

                    Return _patternBits

                End Get
            End Property

            Public ReadOnly Property FrameBufferData() As Short() 'Byte()
                Get
                    Dim _frameBufferData(SizeInPixels.Width * SizeInPixels.Height - 1) As Short 'Byte

                    Dim colorIndexes As Byte() _
                        = Me.ColorIndexes

                    If colorIndexes IsNot Nothing Then
                        Dim colorData As Byte() _
                            = Me.ColorData
                        Dim patternBits As Byte() _
                             = Me.PatternBits

                        'Parallel.For(0, _frameBufferData.Length,
                        '    Sub(i As Integer)
                        '        _frameBufferData(i) = colorData(colorIndexes(i \ 2) * 2 + If(patternBits(i) = 0, 1, 0))
                        '    End Sub)

                        For i As Integer = 0 To _frameBufferData.Length - 1
                            _frameBufferData(i) = colorData(colorIndexes(i \ 2) * 2 + If(patternBits(i) = 0, 1, 0))
                        Next

                    End If

                    Return _frameBufferData

                End Get
            End Property

        End Class

    End Class

    Public Class Chunk82
        Inherits Chunk81

        Public Overrides ReadOnly Property MetadataLength As Integer
            Get
                Return 12
            End Get
        End Property

        Protected ReadOnly Property ColorDataOffset As UShort
            Get
                Return PaletteEntryCount * 2
            End Get
        End Property

        Protected ReadOnly Property PatternDataOffset As UShort
            Get
                'Bytes 8 and 9
                Dim _patternDataOffset As Byte() _
                    = {Metadata(8), Metadata(9)}
                Array.Reverse(_patternDataOffset)
                Return BitConverter.ToInt16(_patternDataOffset, 0)
            End Get
        End Property

        Public ReadOnly Property PaletteEntryWriteOffset As UShort
            Get
                'Bytes 10 and 11
                Dim _paletteEntryWriteOffset As Byte() _
                    = {Metadata(10), Metadata(11)}
                Array.Reverse(_paletteEntryWriteOffset)
                Return BitConverter.ToInt16(_paletteEntryWriteOffset, 0)
            End Get
        End Property

        Protected Overrides ReadOnly Property MacroblockData As Byte()
            Get
                Dim _macroblockData As New List(Of Byte)

                Dim colorDatalength As Integer = PatternDataOffset - ColorDataOffset 'PatternDataOffset
                Dim colorData(colorDatalength - 1) As Byte
                Array.Copy(FrameData, ColorDataOffset, colorData, 0, colorDatalength)

                Dim patternDatalength As Integer _
                    = FrameData.Length - PatternDataOffset
                Dim patternData(patternDatalength - 1) As Byte
                Array.Copy(FrameData, PatternDataOffset, patternData, 0, patternDatalength)

                Dim colorBytes, patternBytes As Byte()

                Dim prevBlockType As Byte

                Dim tileData As New List(Of Byte)
                Dim tileDrawPos As New Point

                Dim colorDataPos As Integer = 0

                For patternDataPos As Integer = 0 To patternDatalength - 1
                    Dim blockType As Byte _
                        = patternData(patternDataPos)

                    Select Case blockType
                        Case 0
                            _macroblockData.Add(0)
                            Exit For

                        Case Else
                            Dim blockData As New List(Of Byte)

                            blockData.Add(blockType)

                            Dim block As New Macroblock(blockType, Nothing, Nothing) ', ByteOrder)

                            Select Case block.ColorDataLength
                                Case 0
                                    colorBytes = Nothing
                                Case Else
                                    ReDim colorBytes(block.ColorDataLength - 1)
                                    Try
                                        Array.Copy(colorData, colorDataPos, colorBytes, 0, block.ColorDataLength)

                                        'For i As Integer = 0 To colorBytes.Length - 1
                                        '    If colorBytes(i) = 255 Then
                                        '        colorBytes(i) = 0
                                        '    End If
                                        'Next

                                        blockData.AddRange(colorBytes)

                                    Catch ex As Exception
                                        Debug.WriteLine(ex.Message)
                                        Exit For
                                    Finally
                                        colorDataPos += block.ColorDataLength
                                    End Try
                            End Select

                            Select Case block.PatternDataLength
                                Case 0
                                    patternBytes = Nothing
                                Case Else
                                    ReDim patternBytes(block.PatternDataLength - 1)
                                    Try
                                        If block.PatternData Is Nothing Then
                                            'Block has not fixed pattern bits, so read from data
                                            Array.Copy(patternData, patternDataPos + 1, patternBytes, 0, block.PatternDataLength)
                                        Else
                                            'Reverse fixed pattern data bits
                                            patternBytes = block.PatternData

                                            For i As Integer = 0 To patternBytes.Length - 1
                                                Dim bits As String _
                                                = Convert.ToString(patternBytes(i), 2).PadLeft(8, "0")
                                                Dim reversedBits As String _
                                                = StrReverse(bits)
                                                patternBytes(i) = Convert.ToByte(reversedBits, 2)
                                            Next
                                        End If

                                        blockData.AddRange(patternBytes)

                                    Catch ex As Exception
                                        Debug.WriteLine(ex.Message)
                                        Exit For
                                    Finally
                                        patternDataPos += block.PatternDataLength
                                    End Try
                            End Select

                            tileData.AddRange(blockData.ToArray)

                            UpdateTileDrawPos(tileDrawPos, block.SizeInPixels)

                            If tileDrawPos = Point.Empty Then
                                Dim count As Integer = 0

                                ' Check for following repeat code
                                Dim nextByte As Byte _
                                    = patternData(patternDataPos + 1)

                                If nextByte >= &H80 Then
                                    Select Case nextByte
                                        Case &H80 To &H86
                                            patternDataPos += 1
                                            count = patternData(patternDataPos) Xor &H80
                                        Case &H87
                                            patternDataPos += 2
                                            count = patternData(patternDataPos)
                                        Case &HF0 To &HFF
                                            If nextByte = &HFF Then
                                                patternDataPos += 2
                                                count = patternData(patternDataPos)
                                            Else
                                                patternDataPos += 1
                                                count = patternData(patternDataPos) And 7 '15
                                            End If
                                    End Select
                                    count += 1
                                End If

                                For i As Integer = 0 To count
                                    _macroblockData.AddRange(tileData)
                                Next

                                tileData.Clear()
                            End If

                    End Select

                    prevBlockType = blockType

                Next

                Return _macroblockData.ToArray

            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

    End Class

    Public Class Chunk8A
        Inherits Chunk81

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

    End Class

    Public Class ChunkA1
        Inherits AudioChunk

        Public Overrides ReadOnly Property SampleRate As Double 'Integer
            Get
                Dim bytes As Byte() _
                    = {Data(4) And 15, Data(5)}
                If ByteOrder = ByteOrder.BigEndian Then
                    Array.Reverse(bytes)
                End If
                Dim mult As Double _
                    = BitConverter.ToInt16(bytes, 0)
                'Dim mult As Double _
                '    = (CDbl(Data(4) And 15) * 256) + CDbl(Data(5))
                Dim rate As Double _
                    = mult * Sega.MegaCD.System.PCMFrequencyIncrement
                Return rate
            End Get
        End Property

        Public Overrides ReadOnly Property SamplesAs8BitUnsigned As Byte()
            Get
                If _samplesAs8BitUnsigned Is Nothing Then
                    Dim samples As Byte() _
                        = RawSamples
                    ReDim _samplesAs8BitUnsigned(samples.Length - 1)

                    Dim signBit, magnitudeBits As Byte

                    For i As Integer = 0 To samples.Length - 1
                        signBit = (samples(i) >> 7) And 1
                        magnitudeBits = samples(i) And &H7F

                        '_samplesAs8BitUnsigned(i) = 128 + If(signBit = 1, -magnitudeBits, magnitudeBits) '((-1 * signBit) * magnitudeBits)

                        If signBit = 1 Then
                            _samplesAs8BitUnsigned(i) = samples(i) '128 - magnitudeBits
                        Else
                            _samplesAs8BitUnsigned(i) = 127 - magnitudeBits '128 + magnitudeBits
                        End If
                    Next
                End If
                Return _samplesAs8BitUnsigned
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

        Public Overrides Function InfoToString(ByVal includeData As Boolean) As String
            With Me
                Dim chunkInfo As String =
                    MyBase.InfoToString(False) _
                    & ControlChars.CrLf _
                    & $"8-bit sign/magnitude PCM{vbCrLf}{Math.Round(.SampleRate, 6)} samples/sec{vbCrLf}{Math.Round(.Duration.TotalSeconds, 6)} secs ({Math.Round(.FrameRate, 6)} frames/sec)"
                If includeData = True Then
                    chunkInfo &=
                        ControlChars.CrLf & ControlChars.CrLf & "Data:" & ControlChars.CrLf &
                        DataToString(Data, MetadataLength)
                End If

                Return chunkInfo
            End With
        End Function

    End Class

    Public Class ChunkA2
        Inherits ChunkA1

        Public Overrides ReadOnly Property SampleRate As Double 'Integer
            Get
                If ByteOrder = ByteOrder.LittleEndian Then
                    Return 22050
                Else
                    Dim bytes(1) As Byte
                    'Dim offset As Integer _
                    '    = If(ByteOrder = ByteOrder.LittleEndian, 0, 4)
                    Dim offset As Integer _
                        = 4
                    Array.Copy(Data, offset, bytes, 0, 2)
                    Array.Reverse(bytes)
                    Return BitConverter.ToInt16(bytes, 0)
                End If
                'Return CDbl(Data(4) * 256) + CDbl(Data(5))
            End Get
        End Property

        Public Overrides ReadOnly Property SamplesAs8BitUnsigned As Byte()
            Get
                If _samplesAs8BitUnsigned Is Nothing Then

                    If ByteOrder = ByteOrder.LittleEndian Then
                        Dim samplesLength As Integer _
                            = Data.Length - 8
                        ReDim _samplesAs8BitUnsigned(samplesLength - 1)
                        Array.Copy(Data, 8, _samplesAs8BitUnsigned, 0, samplesLength)
                    Else
                        Dim samples As Byte() _
                            = RawSamples
                        ReDim _samplesAs8BitUnsigned(samples.Length - 1)

                        Dim signBit, magnitudeBits As Byte

                        For i As Integer = 0 To samples.Length - 1
                            signBit = (samples(i) >> 7) And 1
                            magnitudeBits = samples(i) And &H7F
                            _samplesAs8BitUnsigned(i) = magnitudeBits + If(signBit = 0, 128, 0)
                        Next
                    End If
                End If
                Return _samplesAs8BitUnsigned
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

    End Class

    Public Class ChunkA3
        Inherits ChunkA2

        Public Overrides ReadOnly Property SamplesAs8BitUnsigned As Byte()
            Get
                If _samplesAs8BitUnsigned Is Nothing Then
                    Dim samplesLength As Integer _
                        = FrameData.Length
                    ReDim _samplesAs8BitUnsigned(samplesLength - 1)
                    Array.Copy(FrameData, 0, _samplesAs8BitUnsigned, 0, samplesLength)
                End If
                Return _samplesAs8BitUnsigned
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

    End Class

    Public Class ChunkAA
        Inherits ChunkA2

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

    End Class

    Public MustInherit Class MegaDriveVideoChunk
        Inherits VideoChunk

        'Changed on 2014/11/25: read/write support for changing flags
        Public Property Flags As Byte
            Get
                Return Data(4)
            End Get
            Set(value As Byte)
                Data(4) = value
            End Set
        End Property

        ''Added on 2014/11/30: Support for chunks that contain no palette
        'Public Overridable ReadOnly Property ContainsPaletteData As Boolean
        '    Get
        '        If (Me.Flags And SGAChunkFlags.ContainsPaletteData) Then
        '            Return True
        '        Else
        '            Return False
        '        End If
        '    End Get
        'End Property

        'Added on 2014/11/25: More generic support for palette preceding tile data
        Public Overridable ReadOnly Property PaletteDataFollowsTileData As Boolean
            Get
                If (Flags And ChunkFlags.PaletteDataFollowsTileData) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        'Added on 2014/6/28: More generic support for tilemaps
        Public Overridable ReadOnly Property ContainsCompressedTileMap As Boolean
            Get
                If (Flags And ChunkFlags.ContainsCompressedTileMap) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        'Added on 2014/6/28: More generic support for tilemaps
        Public Overridable ReadOnly Property ContainsTileMap As Boolean
            Get
                If (Flags And ChunkFlags.ContainsTileMap) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        'Added on 2014/6/28: More generic support for chunks with high priority tiles
        Public Overridable ReadOnly Property IsHighPriority As Boolean
            Get
                If (Flags And ChunkFlags.IsHighPriority) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        'Protected _tileData As Byte()
        Protected Shared _blankTile As Byte() = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        Public MustOverride ReadOnly Property TileData As Byte()

        Public MustOverride ReadOnly Property RawTiles(ByVal index As Integer) As Byte()

        'Public MustOverride ReadOnly Property Tiles As Tile(,)

        Public ReadOnly Property PaletteCount As Byte
            Get
                Return Data(5)
            End Get
        End Property

        Protected _paletteData() As Byte
        Public MustOverride ReadOnly Property PaletteData As Byte()

        'Public MustOverride ReadOnly Property TileMap() As UShort(,)
        Public MustOverride ReadOnly Property TileMap() As Sega.MegaDrive.VDP.TileMapEntry(,)

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

    End Class

    Public Class ChunkC1
        Inherits MegaDriveVideoChunk 'SGAVideoChunk

        'Add support for frames that swap pixels on even lines
        Public Property SwapPixelsOnEvenLines As Boolean

        Public Overrides ReadOnly Property MetadataLength As Integer
            Get
                If ContainsTileMap = True Then
                    Return 10
                Else
                    Return MyBase.MetadataLength
                End If
            End Get
        End Property

        Public Overrides ReadOnly Property UniqueTileCount As Integer
            Get
                Dim count As Integer
                If ContainsTileMap Then
                    Dim countBytes As Byte() _
                        = {Data(8), Data(9)}
                    Array.Reverse(countBytes)
                    count = CInt(BitConverter.ToInt16(countBytes, 0))
                    'count = (CInt(Me.Data(8)) * 256) + CInt(Me.Data(9))
                Else
                    count = MyBase.UniqueTileCount
                End If
                Return count
            End Get
        End Property

        Protected Overridable ReadOnly Property TileDataOffset As Integer
            Get
                If PaletteDataFollowsTileData = True Then
                    Return MetadataLength
                Else
                    Return PaletteDataOffset + PaletteDatalength
                End If
            End Get
        End Property

        Protected Overridable ReadOnly Property TileDataLength As Integer
            Get
                Return UniqueTileCount * 32
            End Get
        End Property

        Protected _tileData As Byte()
        Public Overrides ReadOnly Property TileData As Byte()
            Get
                If _tileData Is Nothing Then
                    ReDim _tileData(TileDataLength - 1)
                    Array.Copy(Data, TileDataOffset, _tileData, 0, _tileData.Length)

                    ''Changed on 2015/2/27: Add blank tile to start of tile data
                    'ReDim _tileData(TileDataLength + 31)
                    'Array.Copy(_blankTile, _tileData, 32)
                    'Array.Copy(Data, TileDataOffset, _tileData, 32, _tileData.Length - 32)

                    If SwapPixelsOnEvenLines = True Then
                        For i As Integer = 4 To _tileData.Length Step 8
                            For j As Integer = 0 To 3
                                _tileData(i + j) = ((_tileData(i + j) And 240) \ 16) + ((_tileData(i + j) And 15) * 16)
                            Next
                        Next
                    End If
                End If
                Return _tileData
            End Get
        End Property

        Protected Overridable ReadOnly Property PaletteDataOffset As Integer
            Get
                If PaletteDataFollowsTileData = True Then
                    Return TileDataOffset + TileDataLength
                Else
                    Return MetadataLength
                End If
            End Get
        End Property

        Protected ReadOnly Property PaletteDatalength As Integer
            Get
                Return PaletteCount * 18 'Each palette is 18 bytes long
            End Get
        End Property

        'Protected _paletteData As Byte()
        Public Overrides ReadOnly Property PaletteData As Byte()
            Get
                If _paletteData Is Nothing Then
                    If PaletteCount > 0 Then
                        ReDim _paletteData(PaletteDatalength - 1)
                        Array.Copy(Data, PaletteDataOffset, _paletteData, 0, _paletteData.Length)
                    Else
                        Return Nothing
                    End If
                End If
                Return _paletteData
            End Get
        End Property

        Protected Overridable ReadOnly Property PaletteMapDataOffset As Integer
            Get
                If ContainsTileMap = True Then
                    Return -1
                Else
                    Return PaletteDataOffset + PaletteDatalength
                End If
            End Get
        End Property

        Protected ReadOnly Property PaletteMapDataLength As Integer
            Get
                If ContainsTileMap = True _
                Or PaletteCount < 2 Then
                    Return 0
                Else
                    Dim bitsPerPaletteMapEntry As Integer _
                        = (PaletteCount + 1) \ 2
                    Return Math.Ceiling((SizeInTiles.Width * SizeInTiles.Height * bitsPerPaletteMapEntry) / 8)
                End If
            End Get
        End Property

        Protected _paletteMapData As Byte()
        Protected Overridable ReadOnly Property PaletteMapData As Byte()
            Get
                If _paletteMapData Is Nothing Then
                    If ContainsTileMap = True _
                    Or PaletteCount < 2 Then
                        Return Nothing
                    Else
                        ReDim _paletteMapData(PaletteMapDataLength - 1)
                        Array.Copy(Data, PaletteMapDataOffset, _paletteMapData, 0, _paletteMapData.Length)
                    End If
                End If
                Return _paletteMapData
            End Get
        End Property

        Protected Overridable ReadOnly Property TileMapDataOffset As Integer
            Get
                If ContainsTileMap = True Then
                    Return PaletteDataOffset + PaletteDatalength
                Else
                    Return 0
                End If
            End Get
        End Property

        Protected Overridable ReadOnly Property TileMapDataLength As Integer
            Get
                If ContainsTileMap = True Then
                    If ContainsCompressedTileMap Then
                        Return Data.Length - TileMapDataOffset
                    Else
                        Return CInt(SizeInTiles.Width) * CInt(SizeInTiles.Height) * 2
                    End If
                Else
                    Return 0
                End If
            End Get
        End Property

        Protected _tileMapData As UShort()
        Public Overridable Property TileMapData As UShort()
            Get
                If _tileMapData Is Nothing Then
                    If ContainsTileMap Then

                        Dim startOffset As Integer =
                            TileMapDataOffset
                        Dim len As Integer =
                            TileMapDataLength

                        Dim index As UShort
                        Dim indexList As New List(Of UShort)

                        'Added on 2014/9/5: Support for compressed tile maps
                        If ContainsCompressedTileMap Then
                            Dim runningIndex As UShort

                            For offset As Integer = startOffset To (startOffset + len) - 1
                                index = CUShort(Data(offset)) << 8

                                If (index And &H8000) = 0 Then
                                    If offset < Data.Length - 1 Then
                                        index += CUShort(Data(offset + 1))
                                        offset += 1
                                    End If
                                Else
                                    runningIndex += 1
                                    index += runningIndex
                                End If

                                indexList.Add(index)
                            Next
                        Else
                            'HACK: Fix for incorrect tilemap lengths (seen in Supreme Warrior "DEL.CLP")
                            If (startOffset + len) > Data.Length Then Return Nothing

                            For offset As Integer = startOffset To (startOffset + len) - 1 Step 2
                                index = (CUShort(Data(offset)) << 8) + CUShort(Data(offset + 1))
                                indexList.Add(index)
                            Next
                        End If

                        _tileMapData = indexList.ToArray
                    End If
                End If
                Return _tileMapData
            End Get
            Protected Set(value As UShort())
                _tileMapData = value
            End Set
        End Property

        Protected _paletteMap As Byte(,)
        Protected Overridable ReadOnly Property PaletteMap As Byte(,)
            Get
                If _paletteMap Is Nothing Then
                    If PaletteMapData Is Nothing Then
                        Return Nothing
                    End If

                    ReDim _paletteMap(SizeInTiles.Width - 1, SizeInTiles.Height - 1)

                    Dim paletteMapDataOffset As Integer
                    Dim paletteMapDatum As Byte
                    Dim bitMask As Byte

                    'Added on 2014/9/16: Unified decoding of 1-bit and 2-bit palette entries
                    Dim bitsPerEntry As Integer =
                        If(Me.PaletteCount = 2, 1, 2)
                    Dim bitMaskBits As Byte =
                        If(bitsPerEntry = 1, 1, 3)

                    Dim bitIndex As Integer
                    Dim bitPos As Byte
                    Dim paletteMapValue As Byte

                    For row As Integer = 0 To SizeInTiles.Height - 1
                        For col As Integer = 0 To SizeInTiles.Width - 1
                            paletteMapDataOffset = ((row * SizeInTiles.Width) + col) \ (8 \ bitsPerEntry)
                            paletteMapDatum = PaletteMapData(paletteMapDataOffset)
                            bitIndex = ((row * SizeInTiles.Width) + col) Mod (8 \ bitsPerEntry)
                            bitPos = (8 - bitsPerEntry) - (bitsPerEntry * bitIndex)
                            bitMask = bitMaskBits << bitPos
                            paletteMapValue = (paletteMapDatum And bitMask) >> bitPos
                            _paletteMap(col, row) = paletteMapValue
                        Next
                    Next

                End If
                Return _paletteMap
            End Get
        End Property

        Protected _tileMap As Sega.MegaDrive.VDP.TileMapEntry(,) 'UShort(,)
        Public Overrides ReadOnly Property TileMap As Sega.MegaDrive.VDP.TileMapEntry(,) 'UShort(,)
            Get
                If _tileMap Is Nothing Then
                    ReDim _tileMap(SizeInTiles.Width - 1, SizeInTiles.Height - 1)

                    If TileMapData Is Nothing Then
                        Dim patternIndex As Integer
                        'Dim rawTileMapEntry As UShort

                        For row As Integer = 0 To SizeInTiles.Height - 1
                            For col As Integer = 0 To SizeInTiles.Width - 1
                                patternIndex += 1

                                Dim newTileMapEntry As New Sega.MegaDrive.VDP.TileMapEntry
                                With newTileMapEntry
                                    .PatternIndex = patternIndex
                                    If PaletteMap IsNot Nothing Then
                                        .PaletteIndex = PaletteMap(col, row)
                                    End If
                                End With
                                _tileMap(col, row) = newTileMapEntry

                                'tileIndex += 1
                                'rawTileMapEntry = tileIndex
                                'If PaletteMap IsNot Nothing Then
                                '    rawTileMapEntry += CUShort(PaletteMap(col, row)) << 13
                                'End If
                                '_tileMap(col, row) = New Sega.MegaDrive.VDP.TileMapEntry(rawTileMapEntry)
                            Next
                        Next

                    Else
                        Dim index As Integer
                        For row As Integer = 0 To SizeInTiles.Height - 1
                            For col As Integer = 0 To SizeInTiles.Width - 1
                                _tileMap(col, row) = New Sega.MegaDrive.VDP.TileMapEntry(TileMapData(index))
                                index += 1
                            Next
                        Next
                    End If

                    'ReDim _tileMap(SizeInTiles.Width - 1, SizeInTiles.Height - 1)

                    'If TileMapData Is Nothing Then
                    '    Dim tileIndex As Integer
                    '    Dim tileMapEntry As UShort
                    '    For row As Integer = 0 To SizeInTiles.Height - 1
                    '        For col As Integer = 0 To SizeInTiles.Width - 1
                    '            tileIndex += 1
                    '            tileMapEntry = tileIndex
                    '            If PaletteMap IsNot Nothing Then
                    '                tileMapEntry += CUShort(PaletteMap(col, row)) << 13
                    '            End If
                    '            _tileMap(col, row) = tileMapEntry
                    '        Next
                    '    Next
                    'Else
                    '    Dim index As Integer
                    '    For row As Integer = 0 To SizeInTiles.Height - 1
                    '        For col As Integer = 0 To SizeInTiles.Width - 1
                    '            _tileMap(col, row) = TileMapData(index)
                    '            index += 1
                    '        Next
                    '    Next
                    'End If

                End If
                Return _tileMap
            End Get
        End Property

        Public Overrides ReadOnly Property RawTiles(ByVal index As Integer) As Byte()
            Get
                Dim rawTile(31) As Byte
                Array.Copy(TileData, index * 32, rawTile, 0, 32)
                Return rawTile
            End Get
        End Property

        Protected Shared Function GetPaletteRGBValues(ByVal paletteData As Byte()) As Byte(,,)
            Dim paletteCount As Integer = paletteData.Length \ 18
            Dim paletteRGBValues(paletteData.Length \ 18, 15, 2) As Byte

            Dim offset As Integer = 0
            Dim colorIndex As Integer

            For paletteIndex As Integer = 0 To paletteCount - 1
                For RGBIndex As Integer = 0 To 2
                    paletteRGBValues(paletteIndex, colorIndex, RGBIndex) = 0
                    For power As Integer = 0 To 2
                        For byteIndex As Integer = 0 To 1
                            offset = (paletteIndex * 18) + (RGBIndex * 6) + (power * 2) + byteIndex
                            For bitPos As Integer = 7 To 0 Step -1
                                colorIndex = 8 - (8 * byteIndex) + bitPos
                                paletteRGBValues(paletteIndex, colorIndex, RGBIndex) += ((paletteData(offset) >> bitPos) And 1) * (2 ^ power)
                            Next
                        Next
                    Next
                Next
            Next
            Return paletteRGBValues
        End Function

        Public Overrides ReadOnly Property PaletteAsCRAM As UShort()
            Get
                If _paletteAsCRAM Is Nothing Then
                    If Me.PaletteCount = 0 Then
                        Return Nothing
                    End If

                    ReDim _paletteAsCRAM(63)

                    Dim paletteRGBValues As Byte(,,) _
                        = GetPaletteRGBValues(PaletteData)

                    For paletteIndex As Integer = 0 To PaletteCount - 1
                        For colorIndex As Integer = 0 To 15
                            For RGBIndex As Integer = 0 To 2
                                _paletteAsCRAM((16 * paletteIndex) + colorIndex) +=
                                    CUShort(paletteRGBValues(paletteIndex, colorIndex, RGBIndex)) << ((RGBIndex * 4) + 1)
                            Next
                        Next
                    Next

                End If
                Return _paletteAsCRAM
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            SwapPixelsOnEvenLines = False
        End Sub

        Public Overrides Function InfoToString(ByVal includeData As Boolean) As String
            With Me
                Dim chunkInfo As String =
                    MyBase.InfoToString(False) & vbNewLine &
                    $"Flags: {Convert.ToString(.Flags, 2).PadLeft(8, "0")}{vbNewLine}{ .PaletteCount} palette{If(.PaletteCount = 0 Or .PaletteCount > 1, "s", "")}{vbNewLine}{ .UniqueTileCount} unique tile{If(.UniqueTileCount = 0 Or .UniqueTileCount > 1, "s", "")}"
                If includeData = True Then
                    chunkInfo &= $"{vbNewLine}{vbNewLine}Data:{vbNewLine}" & DataToString(Data, MetadataLength)
                End If
                Return chunkInfo
            End With
        End Function

    End Class

    Public Class ChunkC2
        Inherits ChunkC1

        Public Overrides ReadOnly Property MetadataLength As Integer
            Get
                Return 12
            End Get
        End Property

        'Protected Overrides ReadOnly Property PaletteFollowsTileData As Boolean
        '    Get
        '        Return False
        '    End Get
        'End Property

        Public Overrides ReadOnly Property ContainsTileMap As Boolean
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property CodeDataLength As UShort
            Get
                Dim len As UShort _
                    = (CUShort(Data(8)) << 8) + CUShort(Data(9))
                Return len
            End Get
        End Property

        Public ReadOnly Property ColorDataLength As UShort
            Get
                Dim len As UShort _
                    = (CUShort(Data(10)) << 8) + CUShort(Data(11))
                Return len
            End Get
        End Property

        Public ReadOnly Property PatternDataLength As UShort
            Get
                Return Data.Length - PatternDataOffset
            End Get
        End Property

        Protected Overrides ReadOnly Property PaletteDataOffset As Integer
            Get
                Return MetadataLength
            End Get
        End Property

        'HACK added on 2015/1/1: Decode frame on display instead of on load
        Private _isDataDecoded As Boolean = False

        'Added on 2014/11/25: Support for building tile data from commands
        Private Shadows _tileData() As Byte
        Public Overrides ReadOnly Property TileData As Byte()
            Get
                'HACK added on 2015/1/1: Decode frame on display instead of on load
                If _isDataDecoded = False Then
                    DecodeData()
                    _isDataDecoded = True
                End If

                Return _tileData
            End Get
        End Property

        Public Overrides ReadOnly Property RawTiles(index As Integer) As Byte()
            Get
                Return MyBase.RawTiles(index)
            End Get
        End Property

        Private ReadOnly Property CodeDataOffset As Integer
            Get
                Dim offset As Integer =
                    PaletteDataOffset + PaletteData.Length
                Return offset
            End Get
        End Property

        Private _codeData As Byte()
        Public Property CodeData As Byte()
            Get
                If _codeData Is Nothing Then
                    With Me
                        ReDim _codeData(.CodeDataLength - 1)
                        Array.Copy(.Data, .CodeDataOffset, _codeData, 0, .CodeDataLength)
                    End With
                End If
                Return _codeData
            End Get
            Private Set(value As Byte())
                _codeData = value
            End Set
        End Property

        Private ReadOnly Property ColorDataOffset As Integer
            Get
                Dim offset As Integer =
                    CodeDataOffset + CodeDataLength
                Return offset
            End Get
        End Property

        Private _colorData As Byte()
        Public ReadOnly Property ColorData As Byte()
            Get
                If _colorData Is Nothing Then
                    With Me
                        ReDim _colorData(.ColorDataLength - 1)
                        Array.Copy(.Data, .ColorDataOffset, _colorData, 0, .ColorDataLength)
                    End With
                End If
                Return _colorData
            End Get
        End Property

        Private ReadOnly Property PatternDataOffset As Integer
            Get
                Dim offset As Integer =
                    ColorDataOffset + ColorDataLength
                Return offset
            End Get
        End Property

        Private _patternData As Byte()
        Public ReadOnly Property PatternData As Byte()
            Get
                If _patternData Is Nothing Then
                    With Me
                        ReDim _patternData(.PatternDataLength - 1)
                        Array.Copy(.Data, .PatternDataOffset, _patternData, 0, .PatternDataLength)
                    End With
                End If
                Return _patternData
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            'DecodeData()
        End Sub

        '** Memory pointers used by the routine
        'Dim EXT_008B As UInteger = &H16866
        'Dim EXT_0094 As UInteger = &H16878
        'Dim EXT_016B As UInteger = &H17026  'tile count
        'Dim EXT_016C As UInteger = &H17028  'end of tile data + 256
        'Dim EXT_016D As UInteger = &H1702C  'length of palette data
        'Dim EXT_016E As UInteger = &H1702E  'start address of palette data
        'Dim EXT_016F As UInteger = &H17032  'address of data flags
        'Dim EXT_0170 As UInteger = &H17036  'number of tiles that are repeats of other tiles
        'Dim EXT_0171 As UInteger = &H1703C  'start address of frame metadata
        'Dim EXT_0172 As UInteger = &H1704C
        'Dim EXT_0173 As UInteger = &H17050
        'Dim EXT_01B5 As UInteger = &HC3D08  'current frame type
        'Dim EXT_025B As UInteger = &HFF804D 'Location of Sega CD font generator

        'Pointer to pattern data
        Dim _activePatternData As Byte()
        Dim _codeDataPos, _colorDataPos, _patternDataPos, _tileDataPos As UInteger, _stackPointer As New Stack(Of UInteger)

        Shared FG As Sega.MegaCD.FontDataGenerator

        'Added on 2015/1/25: Code/subcode log for debugging purposes
        Dim _codeLogString As String, _codeLog As List(Of String)
        Public ReadOnly Property CodeLog As List(Of String)
            Get
                If _codeLog Is Nothing Then
                    _codeLog = New List(Of String)
                    'Added on 2015/1/25: Code/subcode log for debugging purposes
                    _codeLog.Add("Tile,Index,Code,Subcode(s)")
                End If
                Return _codeLog
            End Get
        End Property

        Protected Overridable Sub DecodeData()
            '** Decode routine for C2, C3, and C4
            '   (The values below are used in the original code)
            '
            'Address registers:
            'a0 Scratch RAM
            'a1 INPUT position in command data
            'a2 INPUT postion in source color data
            'a3 INPUT position in source pattern data
            'a4 OUTPUT position in decoded tile data
            'a5 OUTPUT position in decoded tile map
            'a6 INPUT/OUTPUT points to the Sega CD's built-in font data generator @ $FF804D
            'a7 Stack pointer
            '
            'Data registers:
            'd0 ~ d5 Used for various purposes
            'd6 Number of actual tile data decoded (i.e. not references to other tiles)
            'd7 Counter for number of tiles left to decode

            _codeDataPos = 0 : _colorDataPos = 0 : _patternDataPos = 0 : _tileDataPos = 0

            _activePatternData = PatternData

            ReDim Preserve _tileData((UniqueTileCount * 32) - 1)
            'ReDim Preserve _tileData(TileCount * 32 + 31)
            '_tileDataPos += 32
            ReDim _tileMapData(UniqueTileCount - 1)

            Dim tileMapIndex, tilesRemaining As UInteger
            tileMapIndex = 0
            tilesRemaining = UniqueTileCount ' - 1

            Dim code As UInteger, subCode As UInteger()

            Dim tileMapEntry As UShort
            Dim tileMapList As New List(Of UShort)

            'Added on 2014/11/27: "Real" Sega CD Font Data Generator
            FG = New Sega.MegaCD.FontDataGenerator

            While tilesRemaining > 0
                ' HACK added on 2014/11/26: fix for wrong code lengths?
                If _codeDataPos >= CodeData.Length Then
                    Exit While
                End If

                'Increment number of actual tiles decoded
                tileMapIndex += 1

                'Create tile map entry and add it to the tile map
                code = ReadCodeByte(True)

                tileMapEntry = (CUShort(code And &HC0) << 7) + tileMapIndex
                tileMapList.Add(tileMapEntry)

                code = code And &H3F

                'Added on 2015/1/25: Code/subcode log for debugging purposes
                _codeLogString = $"{UniqueTileCount - tilesRemaining + 1:0000},{tileMapIndex:0000},${code:X2},"

                Select Case code
                    Case 0 To &HF
                        'Added on 2015/1/16: Break apart codes before calling subroutines to make for easier debugging
                        subCode = {code And 15}

                        'Draw 8x8 section at (0,0)
                        Decode8x8Tile(subCode(0))

                    Case &H10 To &H13
                        code <<= 8
                        code += ReadCodeByte(True)

                        'Added on 2015/1/16: Break apart codes before calling subroutines to make for easier debugging
                        subCode = {(code >> 5) And 31, code And 31}

                        'Added on 2015/1/25: Code/subcode log for debugging purposes
                        For i As Integer = 0 To 1
                            _codeLogString += $"{If(i > 0, " ", "")}${subCode(i):X2}"
                        Next

                        'Draw 8x4 section at (0,0)
                        Decode8x4Section(subCode(0))
                        'Draw 8x4 section at (0,4)
                        Decode8x4Section(subCode(1))

                    Case &H14 To &H17
                        code <<= 8
                        code += ReadCodeByte(True)
                        code <<= 8
                        code += ReadCodeByte(True)

                        'Added on 2015/1/16: Break apart codes before calling subroutines to make for easier debugging
                        subCode = {(code >> 13) And 31, (code >> 8) And 31, code And 31}

                        'Added on 2015/1/25: Code/subcode log for debugging purposes
                        For i As Integer = 0 To 2
                            _codeLogString += $"{If(i > 0, " ", "")}${subCode(i):X2}"
                        Next

                        'Draw 8x4 section at (0,0)
                        Decode8x4Section(subCode(0))
                        'Draw 4x4 section at (0,4)
                        Decode4x4Section(subCode(1))
                        _tileDataPos += 2
                        'Draw 4x4 section at (4,4)
                        Decode4x4Section(subCode(2))
                        _tileDataPos += 14

                    Case &H18 To &H1B
                        code <<= 8
                        code += ReadCodeByte(True)
                        code <<= 8
                        code += ReadCodeByte(True)

                        'Added on 2015/1/16: Break apart codes before calling subroutines to make for easier debugging
                        subCode = {(code >> 13) And 31, (code >> 8) And 31, code And 31}

                        'Added on 2015/1/25: Code/subcode log for debugging purposes
                        For i As Integer = 0 To 2
                            _codeLogString += $"{If(i > 0, " ", "")}${subCode(i):X2}"
                        Next

                        'Draw 4x4 section at (0,0)
                        Decode4x4Section(subCode(0))
                        _tileDataPos += 2
                        'Draw 4x4 section at (4,0)
                        Decode4x4Section(subCode(1))
                        _tileDataPos += 14
                        'Draw 8x4 section at (0,4)
                        Decode8x4Section(subCode(2))

                    Case &H1C To &H1F
                        code <<= 8
                        code += ReadCodeByte(True)

                        'Added on 2015/1/16: Break apart codes before calling subroutines to make for easier debugging
                        subCode = {(code >> 5) And 31, code And 31}

                        'Added on 2015/1/25: Code/subcode log for debugging purposes
                        For i As Integer = 0 To 1
                            _codeLogString += $"{If(i > 0, " ", "")}${subCode(i):X2}"
                        Next

                        'Draw 4x8 section at (0,0)
                        Decode4x8Section(subCode(0))
                        _tileDataPos += 2
                        'Draw 4x8 section at (4,0)
                        Decode4x8Section(subCode(1))
                        _tileDataPos += 30

                    Case &H20 To &H2F
                        code <<= 8
                        code += ReadCodeByte(True)
                        code <<= 8
                        code += ReadCodeByte(True)

                        'Added on 2015/1/16: Break apart codes before calling subroutines to make for easier debugging
                        subCode = {(code >> 15) And 31, (code >> 10) And 31, (code >> 5) And 31, code And 31}

                        'Added on 2015/1/25: Code/subcode log for debugging purposes
                        For i As Integer = 0 To 3
                            _codeLogString += $"{If(i > 0, " ", "")}${subCode(i):X2}"
                        Next

                        'Draw 4x4 section at (0,0)
                        Decode4x4Section(subCode(0))
                        _tileDataPos += 2
                        'Draw 4x4 section at (4,0)
                        Decode4x4Section(subCode(1))
                        _tileDataPos += 14
                        'Draw 4x4 section at (0,4)
                        Decode4x4Section(subCode(2))
                        _tileDataPos += 2
                        'Draw 4x4 section at (4,4)
                        Decode4x4Section(subCode(3))
                        _tileDataPos += 14

                    Case &H30 To &H3F
                        code <<= 8
                        code += ReadCodeByte(True)
                        code += 1

                        tileMapEntry = ((code And &HC00) << 1) + (code And &H3FF)
                        tileMapList(tileMapList.Count - 1) = (tileMapList(tileMapList.Count - 1) And &HE000) + tileMapEntry

                        'Added on 2015/1/25: Code/subcode log for debugging purposes
                        _codeLogString = $"{UniqueTileCount - tilesRemaining + 1:0000},{tileMapEntry And &H3FF:0000},${(code >> 8) And &H3F:X2}"

                        tileMapIndex -= 1

                        'Case Else
                        '    Debug.WriteLine($"Encountered unrecognized C2 code: ${code:X2}")

                End Select

                tilesRemaining -= 1

                'Added on 2015/1/25: Code/subcode log for debugging purposes
                CodeLog.Add(_codeLogString)

            End While

            'HACK added on 2015/1/1: Make sure tile map is big enough
            Do Until tileMapList.Count = UniqueTileCount
                tileMapList.Add(CUShort(1))
            Loop

            TileMapData = tileMapList.ToArray

            ''Added on 2015/1/25: Code/subcode log for debugging purposes
            'Using fs As New IO.FileStream("D:\Temp\CodeLog.csv", IO.FileMode.Create, IO.FileAccess.Write)
            '    Using sw As New IO.StreamWriter(fs, System.Text.UTF8Encoding.UTF8)
            '        For Each s As String In _codeLog
            '            sw.Write(s & vbNewLine)
            '        Next
            '    End Using
            'End Using

        End Sub

        ''' <summary>
        ''' Specifies the style of source color input
        ''' </summary>
        ''' <remarks>For use with Mega CD 2-color pattern generator</remarks>
        Enum SourceColorInputStyle As Integer
            ''' <summary>
            ''' Do not input source color data
            ''' </summary>
            ''' <remarks></remarks>
            None
            ''' <summary>
            ''' Input source color data
            ''' </summary>
            ''' <remarks></remarks>
            Normal
            ''' <summary>
            ''' Input source color data only if top bit of associated pattern data is set
            ''' </summary>
            ''' <remarks></remarks>
            Conditional
        End Enum

        ''' <summary>
        ''' Specifies the style of source pattern input
        ''' </summary>
        ''' <remarks>For use with Mega CD 2-color pattern generator</remarks>
        Enum SourcePatternInputStyle As Integer
            ''' <summary>
            ''' Do not input source pattern data
            ''' </summary>
            ''' <remarks></remarks>
            None
            ''' <summary>
            ''' Input source pattern data
            ''' </summary>
            ''' <remarks></remarks>
            Normal
            ''' <summary>
            ''' Input source pattern data and preserve the bottom 4 bits of pattern data for use in generating the next pattern
            ''' </summary>
            ''' <remarks></remarks>
            Preserve
        End Enum

        Shared _blank8x8Tile As Byte() _
            = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}

        Shared _presetPatternData As Byte() _
            = {&H55, &HAA, &H55, &HAA, &H55, &HAA, &H55, &HAA,
               &H55, 0, &H55, 0, &H55, 0, &H55, 0,
               0, &H55, 0, &H55, 0, &H55, 0, &H55,
               &H55, &H80, &H55, &H80, &H55, &H80, &H55, &H80,
               &H55, 2, &H55, 2, &H55, 2, &H55, 2,
               &H50, &H50, &H50, &H50, &H50, &H50, &H50, &H50,
               &H7F, &HFF, &H7F, &HFF, &H7F, &HFF, &H7F, &HFF,
               &H7F, &HFF, &H7F, &HFF, &HFF, &HFF, &HFF, &HFF}

        Sub Decode8x8Tile(ByVal code As UInteger)
            Select Case code And 15
                Case 0
                    If Type = ChunkType.VideoC4 Then
                        'Skip to next tile
                        _tileDataPos += 32
                    Else
                        'Blank out an 8x8 tile
                        Array.Copy(_blank8x8Tile, 0, _tileData, _tileDataPos, 32)
                        _tileDataPos += 32
                    End If

                Case 1
                    'Build a solid color tile
                    Decode8x8(SourcePatternInputStyle.None)
                    _tileDataPos += 32

                Case 2
                    'Build a 2-color patterned tile
                    Decode8x8(SourcePatternInputStyle.Normal)
                    _tileDataPos += 32

                Case 3
                    'Build a four-color tile by copying from specified offsets of a preset pattern
                    With FG
                        Dim sourceColors As Byte() _
                            = {ReadColorByte(True), ReadColorByte(True)}
                        Dim colorCombos As Byte() _
                            = Get4ColorCombos(sourceColors(0), sourceColors(1))

                        Dim offsets As Byte

                        For i As Integer = 0 To 15
                            offsets = ReadPatternByte(True)
                            For j As Integer = 1 To 0 Step -1
                                WriteByteToOutput(colorCombos((offsets >> (4 * j)) And 15), True)
                            Next
                        Next

                        .SourceColors = sourceColors(1)
                    End With

                Case 4
                    'Build a 2-color tile using new source colors and preset pattern data
                    _activePatternData = _presetPatternData
                    _stackPointer.Push(_patternDataPos)
                    _patternDataPos = CInt(ReadCodeByte(True)) * 8

                    Decode8x8(SourcePatternInputStyle.Normal)
                    _tileDataPos += 32

                    _activePatternData = PatternData
                    _patternDataPos = _stackPointer.Pop

                Case 5 '!
                    'Align color data offset to an even value
                    _colorDataPos += Math.Abs((ColorDataOffset Mod 2) - (_colorDataPos Mod 2))

                    'Copy an 8x8 tile directly from color data
                    Array.Copy(ColorData, _colorDataPos, _tileData, _tileDataPos, 32)
                    _colorDataPos += 32
                    _tileDataPos += 32

                Case 6
                    With FG
                        'Build 8x4 section using new source colors at (0,0)
                        Decode8x4(0, SourceColorInputStyle.Normal, SourcePatternInputStyle.Normal)
                        _tileDataPos += 16
                        'Build 4x4 section using current source colors at (0,4)
                        Decode4x4(0, SourceColorInputStyle.None, SourcePatternInputStyle.Normal)
                        _tileDataPos += 2
                    End With

                    'Draw 4x4 pattern at (4,4)
                    code = ReadCodeByte(True) And 31

                    'Added on 2015/1/25: Code/subcode log for debugging purposes
                    _codeLogString += $"${code:X2}"

                    Decode4x4Section(code)
                    _tileDataPos += 14

                Case 7
                    With FG
                        'Build 8x4 section using new source colors at (0,0)
                        Decode8x4(0, SourceColorInputStyle.Normal, SourcePatternInputStyle.Normal)
                        _tileDataPos += 16
                        'Build 4x4 section using current source colors at (4,4)
                        Decode4x4(2, SourceColorInputStyle.None, SourcePatternInputStyle.Normal)
                    End With

                    'Draw 4x4 pattern at (4,0)
                    code = ReadCodeByte(True) And 31

                    'Added on 2015/1/25: Code/subcode log for debugging purposes
                    _codeLogString += $"${code:X2}"

                    Decode4x4Section(code)
                    _tileDataPos += 16

                Case 8
                    With FG
                        'Build 4x4 section using new source colors at (0,0)
                        Decode4x4(0, SourceColorInputStyle.Normal, SourcePatternInputStyle.Normal)
                        'Build 8x4 section using current source colors at (0,4)
                        Decode8x4(16, SourceColorInputStyle.None, SourcePatternInputStyle.Normal)
                        _tileDataPos += 2
                    End With

                    'Draw 4x4 pattern at (4,0)
                    code = ReadCodeByte(True) And 31

                    'Added on 2015/1/25: Code/subcode log for debugging purposes
                    _codeLogString += $"${code:X2}"

                    Decode4x4Section(code)
                    _tileDataPos += 30

                Case 9
                    'Draw 4x4 pattern at (0,0)
                    code = ReadCodeByte(True) And 31

                    'Added on 2015/1/25: Code/subcode log for debugging purposes
                    _codeLogString += $"${code:X2}"

                    Decode4x4Section(code)

                    With FG
                        'Draw 4x4 pattern at (4,0)
                        Decode4x4(2, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                        _tileDataPos += 16
                        'Draw 8x4 pattern at (0,4) using same source colors
                        Decode8x4(0, SourceColorInputStyle.None, SourcePatternInputStyle.Normal)
                        _tileDataPos += 16
                    End With

                Case &HA
                    'Draw 8x8 pattern using current source pattern
                    With FG
                        .SourceColors = ReadColorByte(True)
                        For i = 0 To 7
                            Array.Copy(.OutputPattern, 0, _tileData, _tileDataPos, 4)
                            _tileDataPos += 4
                        Next
                    End With

                    'Draw a 2x1 pattern directly from color data at specified coordinates in tile
                    code = ReadCodeByte(True) And 31
                    _tileData(_tileDataPos - 32 + code) = ReadColorByte(True)

                Case &HB, &HC 'TODO: FIX THIS PROCEDURE
                    'Draw an 8x8 using preset pattern data and two bytes of new pattern data
                    Dim presetPatternDataCopy(7) As Byte
                    Array.Copy(_presetPatternData, CInt(ReadCodeByte(True)) * 8, presetPatternDataCopy, 0, 8)

                    Dim newPatternData As Byte() _
                        = If(code = &HB, {CByte(ReadPatternByte(True))}, {CByte(ReadPatternByte(True)), CByte(ReadPatternByte(True))})
                    Array.Reverse(newPatternData)

                    Dim newPatternDataInsertPos As Integer _
                        = ReadCodeByte(True)
                    For i As Integer = 0 To newPatternData.Count - 1
                        presetPatternDataCopy((newPatternDataInsertPos >> (4 * i)) And 7) = newPatternData(i)
                    Next

                    _activePatternData = presetPatternDataCopy
                    _stackPointer.Push(_patternDataPos)
                    _patternDataPos = 0

                    Decode8x8(SourcePatternInputStyle.Normal)
                    _tileDataPos += 32

                    _activePatternData = PatternData
                    _patternDataPos = _stackPointer.Pop

                Case &HD
                    'Draw an 8x8 pattern at (0,0) using new source color and pattern data
                    Decode8x8(SourcePatternInputStyle.Normal)
                    _tileDataPos += 32

                    'Draw a 2x1 pattern directly from color data at specified coordinates in tile
                    Dim offset As Integer _
                         = CInt(ReadCodeByte(True)) And &H1F
                    _tileData(_tileDataPos - 32 + offset) = ReadColorByte(True)

                Case &HE
                    'Draw an 8x8 pattern at (0,0) using new source color and pattern data
                    Decode8x8(SourcePatternInputStyle.Normal)
                    _tileDataPos += 32

                    'Draw two 2x1 patterns directly from color data at specified coordinates in upper half of tile
                    Dim offsetData As Integer _
                         = CInt(ReadCodeByte(True))
                    Dim offsets As Integer() _
                        = {(offsetData >> 4) And 15, offsetData And 15}
                    offsets(1) += offsets(0)

                    For i As Integer = 0 To 1
                        _tileData(_tileDataPos - 32 + offsets(i)) = ReadColorByte(True)
                    Next

                Case &HF
                    'Draw an 8x8 pattern at (0,0) using new source color and pattern data
                    Decode8x8(SourcePatternInputStyle.Normal)
                    _tileDataPos += 32

                    'Draw two 2x1 patterns directly from color data at specified coordinates in lower half of tile
                    Dim offsetData As Integer _
                         = CInt(ReadCodeByte(True))
                    Dim offsets As Integer() _
                        = {(offsetData >> 4) And 15, offsetData And 15}
                    offsets(0) += 16
                    offsets(1) += offsets(0)

                    For i As Integer = 0 To 1
                        _tileData(_tileDataPos - 32 + offsets(i)) = ReadColorByte(True)
                    Next

            End Select

        End Sub

        Sub Decode8x4Section(ByVal code As UInteger)
            Select Case code And 31
                Case 0
                    If Type = ChunkType.VideoC4 Then
                        'Skip to next tile
                        _tileDataPos += 16
                    Else
                        'Blank out an 8x8 tile
                        Array.Copy(_blank8x8Tile, 0, _tileData, _tileDataPos, 16)
                        _tileDataPos += 16
                    End If

                Case 1
                    'Decode an 8x4 section using new colors and existing pattern data
                    Decode8x4(0, SourceColorInputStyle.Normal, SourcePatternInputStyle.None)
                    _tileDataPos += 16
                Case 2
                    'Decode an 8x4 section conditionally using new colors and new pattern data
                    Decode8x4(0, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16
                Case 3
                    Decode4x4(0, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    Decode4x4(2, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16
                Case 4
                    Decode4x4(0, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    Decode4x2(2)
                    Decode4x2(10)
                    _tileDataPos += 16
                Case 5
                    Decode4x4(0, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    Decode4x2(2)
                    Decode2x2(10, SourcePatternInputStyle.Preserve)
                    Decode2x2(11, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16
                Case 6
                    Decode4x4(0, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    Decode2x2(2, SourcePatternInputStyle.Preserve)
                    Decode2x2(3, SourcePatternInputStyle.Normal)
                    Decode4x2(10)
                    _tileDataPos += 16
                Case 7
                    Decode4x4(0, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    Decode2x2(2, SourcePatternInputStyle.Preserve)
                    Decode2x2(3, SourcePatternInputStyle.Normal)
                    Decode2x2(10, SourcePatternInputStyle.Preserve)
                    Decode2x2(11, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16
                Case 8
                    Decode4x2(0)
                    Decode4x4(2, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    Decode4x2(8)
                    _tileDataPos += 16
                Case 9 '!
                    Decode4x2(0)
                    Decode4x4(2, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16
                Case &HA
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode4x4(2, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    Decode4x2(8)
                    _tileDataPos += 16
                Case &HB '!
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode4x4(2, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16
                Case &HC
                    Decode4x2(0)
                    Decode4x2(2)
                    Decode4x2(8)
                    Decode4x2(10)
                    _tileDataPos += 16
                Case &HD '!
                    Decode4x2(0)
                    Decode4x2(2)
                    Decode4x2(8)
                    Decode2x2(10, SourcePatternInputStyle.Preserve)
                    Decode2x2(11, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16
                Case &HE
                    Decode4x2(0)
                    Decode4x2(2)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode4x2(10)
                    _tileDataPos += 16
                Case &HF
                    Decode4x2(0)
                    Decode4x2(2)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode2x2(10, SourcePatternInputStyle.Preserve)
                    Decode2x2(11, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16
                Case &H10
                    Decode4x2(0)
                    Decode2x2(2, SourcePatternInputStyle.Preserve)
                    Decode2x2(3, SourcePatternInputStyle.Normal)
                    Decode4x2(8)
                    Decode4x2(10)
                    _tileDataPos += 16
                Case &H11
                    Decode4x2(0)
                    Decode2x2(2, SourcePatternInputStyle.Preserve)
                    Decode2x2(3, SourcePatternInputStyle.Normal)
                    Decode4x2(8)
                    Decode2x2(10, SourcePatternInputStyle.Preserve)
                    Decode2x2(11, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16
                Case &H12
                    Decode4x2(0)
                    Decode2x2(2, SourcePatternInputStyle.Preserve)
                    Decode2x2(3, SourcePatternInputStyle.Normal)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode4x2(10)
                    _tileDataPos += 16
                Case &H13
                    Decode4x2(0)
                    Decode2x2(2, SourcePatternInputStyle.Preserve)
                    Decode2x2(3, SourcePatternInputStyle.Normal)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode2x2(10, SourcePatternInputStyle.Preserve)
                    Decode2x2(11, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16
                Case &H14
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode4x2(2)
                    Decode4x2(8)
                    Decode4x2(10)
                    _tileDataPos += 16
                Case &H15
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode4x2(2)
                    Decode4x2(8)
                    Decode2x2(10, SourcePatternInputStyle.Preserve)
                    Decode2x2(11, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16
                Case &H16
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode4x2(2)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode4x2(10)
                    _tileDataPos += 16
                Case &H17
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode4x2(2)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode2x2(10, SourcePatternInputStyle.Preserve)
                    Decode2x2(11, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16
                Case &H18 '!
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode2x2(2, SourcePatternInputStyle.Preserve)
                    Decode2x2(3, SourcePatternInputStyle.Normal)
                    Decode4x2(8)
                    Decode4x2(10)
                    _tileDataPos += 16
                Case &H19
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode2x2(2, SourcePatternInputStyle.Preserve)
                    Decode2x2(3, SourcePatternInputStyle.Normal)
                    Decode4x2(8)
                    Decode2x2(10, SourcePatternInputStyle.Preserve)
                    Decode2x2(11, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16
                Case &H1A
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode2x2(2, SourcePatternInputStyle.Preserve)
                    Decode2x2(3, SourcePatternInputStyle.Normal)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode4x2(10)
                    _tileDataPos += 16
                Case &H1B
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode2x2(2, SourcePatternInputStyle.Preserve)
                    Decode2x2(3, SourcePatternInputStyle.Normal)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode2x2(10, SourcePatternInputStyle.Preserve)
                    Decode2x2(11, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16
                Case &H1C
                    'Draw an 8x4 section using four colors by copying from specified offsets of a pre-drawn pattern
                    With FG
                        Dim sourceColors As Byte() _
                            = {ReadColorByte(True), ReadColorByte(True)}
                        Dim colorCombos As Byte() _
                            = Get4ColorCombos(sourceColors(0), sourceColors(1))

                        Dim offsets As Byte

                        For i As Integer = 0 To 7
                            offsets = ReadPatternByte(True)
                            For j As Integer = 1 To 0 Step -1
                                WriteByteToOutput(colorCombos((offsets >> (4 * j)) And 15), True)
                            Next
                        Next

                        .SourceColors = sourceColors(1)
                    End With

                Case &H1D
                    'Draw an 8x4 section directly from color data
                    Array.Copy(ColorData, _colorDataPos, _tileData, _tileDataPos, 16)
                    _colorDataPos += 16
                    _tileDataPos += 16

                Case &H1E 'TODO: FIX THIS PROCEDURE
                    'Draw an 8x4 using preset pattern data
                    _activePatternData = _presetPatternData
                    _stackPointer.Push(_patternDataPos)
                    _patternDataPos = CInt(ReadCodeByte(True)) * 8

                    Decode8x4(0, SourceColorInputStyle.Normal, SourcePatternInputStyle.Normal)
                    _tileDataPos += 16

                    _activePatternData = PatternData
                    _patternDataPos = _stackPointer.Pop

                Case &H1F
                    'Do NOTHING

            End Select

        End Sub

        Sub Decode4x8Section(ByVal code As UInteger)
            Select Case code And 31
                Case 0
                    If Type = ChunkType.VideoC4 Then
                        'Do nothing
                        Exit Sub
                    Else
                        'Clear 4x8 section of tile
                        For i = 0 To 28 Step 4
                            For j = 0 To 1
                                _tileData(_tileDataPos + i + j) = 0
                            Next
                        Next
                    End If
                Case 1
                    'Decode a 4x8 using new colors and existing pattern data
                    Decode4x8(0, SourceColorInputStyle.Normal, SourcePatternInputStyle.None)
                Case 2
                    'Decode a 4x8 using new colors and new pattern data
                    Decode4x8(0, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                Case 3
                    Decode4x4(0, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    Decode4x4(16, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                Case 4
                    Decode4x4(0, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    Decode4x2(16)
                    Decode4x2(24)
                Case 5
                    Decode4x4(0, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    Decode4x2(16)
                    Decode2x2(24, SourcePatternInputStyle.Preserve)
                    Decode2x2(25, SourcePatternInputStyle.Normal)
                Case 6
                    Decode4x4(0, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    Decode2x2(16, SourcePatternInputStyle.Preserve)
                    Decode2x2(17, SourcePatternInputStyle.Normal)
                    Decode4x2(24)
                Case 7
                    Decode4x4(0, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                    Decode2x2(16, SourcePatternInputStyle.Preserve)
                    Decode2x2(17, SourcePatternInputStyle.Normal)
                    Decode2x2(24, SourcePatternInputStyle.Preserve)
                    Decode2x2(25, SourcePatternInputStyle.Normal)
                Case 8
                    Decode4x2(0)
                    Decode4x2(8)
                    Decode4x4(16, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                Case 9
                    Decode4x2(0)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode4x4(16, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                Case &HA
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode4x2(8)
                    Decode4x4(16, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                Case &HB
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode4x4(16, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                Case &HC
                    Decode4x2(0)
                    Decode4x2(8)
                    Decode4x2(16)
                    Decode4x2(24)
                Case &HD
                    Decode4x2(0)
                    Decode4x2(8)
                    Decode4x2(16)
                    Decode2x2(24, SourcePatternInputStyle.Preserve)
                    Decode2x2(25, SourcePatternInputStyle.Normal)
                Case &HE
                    Decode4x2(0)
                    Decode4x2(8)
                    Decode2x2(16, SourcePatternInputStyle.Preserve)
                    Decode2x2(17, SourcePatternInputStyle.Normal)
                    Decode4x2(24)
                Case &HF
                    Decode4x2(0)
                    Decode4x2(8)
                    Decode2x2(16, SourcePatternInputStyle.Preserve)
                    Decode2x2(17, SourcePatternInputStyle.Normal)
                    Decode2x2(24, SourcePatternInputStyle.Preserve)
                    Decode2x2(25, SourcePatternInputStyle.Normal)
                Case &H10
                    Decode4x2(0)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode4x2(16)
                    Decode4x2(24)
                Case &H11
                    Decode4x2(0)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode4x2(16)
                    Decode2x2(24, SourcePatternInputStyle.Preserve)
                    Decode2x2(25, SourcePatternInputStyle.Normal)
                Case &H12
                    Decode4x2(0)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode2x2(16, SourcePatternInputStyle.Preserve)
                    Decode2x2(17, SourcePatternInputStyle.Normal)
                    Decode4x2(24)
                Case &H13
                    Decode4x2(0)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode2x2(16, SourcePatternInputStyle.Preserve)
                    Decode2x2(17, SourcePatternInputStyle.Normal)
                    Decode2x2(24, SourcePatternInputStyle.Preserve)
                    Decode2x2(25, SourcePatternInputStyle.Normal)
                Case &H14
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode4x2(8)
                    Decode4x2(16)
                    Decode4x2(24)
                Case &H15
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode4x2(8)
                    Decode4x2(16)
                    Decode2x2(24, SourcePatternInputStyle.Preserve)
                    Decode2x2(25, SourcePatternInputStyle.Normal)
                Case &H16
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode4x2(8)
                    Decode2x2(16, SourcePatternInputStyle.Preserve)
                    Decode2x2(17, SourcePatternInputStyle.Normal)
                    Decode4x2(24)
                Case &H17
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode4x2(8)
                    Decode2x2(16, SourcePatternInputStyle.Preserve)
                    Decode2x2(17, SourcePatternInputStyle.Normal)
                    Decode2x2(24, SourcePatternInputStyle.Preserve)
                    Decode2x2(25, SourcePatternInputStyle.Normal)
                Case &H18 '!
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode4x2(16)
                    Decode4x2(24)
                Case &H19
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode4x2(16)
                    Decode2x2(24, SourcePatternInputStyle.Preserve)
                    Decode2x2(25, SourcePatternInputStyle.Normal)
                Case &H1A
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode2x2(16, SourcePatternInputStyle.Preserve)
                    Decode2x2(17, SourcePatternInputStyle.Normal)
                    Decode4x2(24)
                Case &H1B
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Decode2x2(16, SourcePatternInputStyle.Preserve)
                    Decode2x2(17, SourcePatternInputStyle.Normal)
                    Decode2x2(24, SourcePatternInputStyle.Preserve)
                    Decode2x2(25, SourcePatternInputStyle.Normal)
                Case &H1C
                    'Draw a 4x8 section using four colors by copying from specified offsets of a pre-drawn pattern
                    With FG
                        Dim sourceColors As Byte() _
                            = {ReadColorByte(True), ReadColorByte(True)}
                        Dim colorCombos As Byte() _
                            = Get4ColorCombos(sourceColors(0), sourceColors(1))

                        Dim offsets As Byte

                        For i As Integer = 0 To 7
                            offsets = ReadPatternByte(True)
                            For j As Integer = 1 To 0 Step -1
                                _tileData(_tileDataPos + (4 * i) + (1 - j)) = colorCombos((offsets >> (4 * j)) And 15)
                            Next
                        Next

                        .SourceColors = sourceColors(1)
                    End With

                Case &H1D
                    For i = 0 To 7
                        For j = 0 To 1
                            _tileData(_tileDataPos + (4 * i) + j) = ReadColorByte(True)
                        Next
                    Next

                Case &H1E
                    'Draw a 4x8 using previous pattern data
                    _activePatternData = _presetPatternData
                    _stackPointer.Push(_patternDataPos)
                    _patternDataPos = CInt(ReadCodeByte(True)) * 8

                    Decode4x8(0, SourceColorInputStyle.Normal, SourcePatternInputStyle.Normal)

                    _activePatternData = PatternData
                    _patternDataPos = _stackPointer.Pop

                Case &H1F
                    'Do NOTHING

            End Select

        End Sub

        Sub Decode4x4Section(ByVal code As UInteger)
            Select Case code And 31
                Case 0
                    If Type = ChunkType.VideoC4 Then
                        'Do nothing
                        Exit Sub
                    Else
                        'Blank 4x4 section
                        For j = 0 To 12 Step 4
                            _tileData(_tileDataPos + j) = 0
                        Next
                    End If
                Case 1 '!
                    'Decode 4x4 using new source colors and existing pattern data
                    Decode4x4(0, SourceColorInputStyle.Normal, SourcePatternInputStyle.None)
                Case 2 '!
                    'Decode 4x4 using new source colors and new pattern data
                    Decode4x4(0, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
                Case 3 '!
                    Decode4x2(0)
                    Decode4x2(8)
                Case 4
                    Decode4x2(0)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                Case 5 '!
                    Decode4x2(0)
                    Decode2x2(8, SourcePatternInputStyle.Normal)
                    Copy2x2(9)
                Case 6 '!
                    Decode4x2(0)
                    Copy2x1(8)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Copy2x1(12)
                Case 7 '!
                    Decode4x2(0)
                    Copy4x2(8)
                Case 8
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode4x2(8)
                Case 9 '!
                    Decode2x2(0, SourcePatternInputStyle.Normal)
                    Copy2x2(1)
                    Decode4x2(8)
                Case &HA '!
                    Copy2x1(0)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Copy2x1(4)
                    Decode4x2(8)
                Case &HB '!
                    Copy4x2(0)
                    Decode4x2(8)
                Case &HC
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                Case &HD
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Decode2x2(8, SourcePatternInputStyle.Normal)
                    Copy2x2(9)
                Case &HE '!
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Copy2x1(8)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Copy2x1(12)
                Case &HF
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Copy4x2(8)
                Case &H10 '!
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Copy2x2(1)
                    Decode2x2(8, SourcePatternInputStyle.Normal)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                Case &H11 '!
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Copy2x2(1)
                    Decode2x2(8, SourcePatternInputStyle.Normal)
                    Copy2x2(9)
                Case &H12 '!
                    Decode2x2(0, SourcePatternInputStyle.Preserve)
                    Copy2x2(1)
                    Copy2x1(8)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Copy2x1(12)
                Case &H13 '!
                    Decode2x2(0, SourcePatternInputStyle.Normal)
                    Copy2x2(1)
                    Copy4x2(8)
                Case &H14 '!
                    Copy2x1(0)
                    Decode2x2(1, SourcePatternInputStyle.Preserve)
                    Copy2x1(4)
                    Decode2x2(8, SourcePatternInputStyle.Normal)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                Case &H15 '!
                    Copy2x1(0)
                    Decode2x2(1, SourcePatternInputStyle.Preserve)
                    Copy2x1(4)
                    Decode2x2(8, SourcePatternInputStyle.Normal)
                    Copy2x2(9)
                Case &H16 '!
                    Copy2x1(0)
                    Decode2x2(1, SourcePatternInputStyle.Preserve)
                    Copy2x2(4)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Copy2x1(12)
                Case &H17 '!
                    Copy2x1(0)
                    Decode2x2(1, SourcePatternInputStyle.Normal)
                    Copy2x2(4)
                    Copy2x1(9)
                    Copy2x1(12)
                    Copy2x1(13)
                Case &H18 '!
                    Copy4x2(0)
                    Decode2x2(8, SourcePatternInputStyle.Preserve)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                Case &H19
                    Copy4x2(0)
                    Decode2x2(8, SourcePatternInputStyle.Normal)
                    Copy2x2(9)
                Case &H1A '!
                    Copy4x2(0)
                    Copy2x1(8)
                    Decode2x2(9, SourcePatternInputStyle.Normal)
                    Copy2x1(12)
                Case &H1B, &H1D '!
                    Copy4x4(0)
                Case &H1C
                    'Draw a 4x4 section using four colors by copying from specified offsets of a pre-drawn pattern
                    With FG
                        Dim sourceColors As Byte() _
                            = {ReadColorByte(True), ReadColorByte(True)}
                        Dim colorCombos As Byte() _
                            = Get4ColorCombos(sourceColors(0), sourceColors(1))

                        Dim offsets As Byte

                        For i As Integer = 0 To 3
                            offsets = ReadPatternByte(True)
                            For j As Integer = 1 To 0 Step -1
                                _tileData(_tileDataPos + (4 * i) + (1 - j)) = colorCombos((offsets >> (4 * j)) And 15)
                            Next
                        Next

                        .SourceColors = sourceColors(1)
                    End With

                Case &H1E
                    'Draw a 4x4 using previous pattern data
                    _activePatternData = _presetPatternData
                    _stackPointer.Push(_patternDataPos)
                    _patternDataPos = CInt(ReadCodeByte(True)) * 8

                    Decode4x4(0, SourceColorInputStyle.Normal, SourcePatternInputStyle.Normal)

                    _activePatternData = PatternData
                    _patternDataPos = _stackPointer.Pop

                Case &H1F
                    'Do NOTHING

            End Select

        End Sub

        Private Function ReadDataByte(ByRef source As Byte(), ByRef offset As UInteger, ByVal postIncrement As Boolean) As UInteger
            ''HACK added on 2015/1/22: Keep offsets within bounds
            'If offset >= source.Length Then
            '    Debug.WriteLine("Out of bounds offset in prodecure ""ReadDataByte""")
            '    offset = source.Length - 1
            'End If

            Dim returnValue As Byte _
                = source(offset)
            offset += If(postIncrement = True, 1, 0)
            Return returnValue
        End Function

        Private _lastReadCodeByte As Byte
        Private Function ReadCodeByte(ByVal postIncrement As Boolean) As UInteger
            _lastReadCodeByte = ReadDataByte(CodeData, _codeDataPos, postIncrement)
            Return _lastReadCodeByte
        End Function

        Private _lastReadColorByte As Byte
        Private Function ReadColorByte(ByVal postIncrement As Boolean) As UInteger
            _lastReadColorByte = ReadDataByte(ColorData, _colorDataPos, postIncrement)
            Return _lastReadColorByte
        End Function

        Private _lastReadPatternByte As Byte
        Private Function ReadPatternByte(ByVal postIncrement As Boolean) As UInteger
            _lastReadPatternByte = ReadDataByte(_activePatternData, _patternDataPos, postIncrement)
            Return _lastReadPatternByte
        End Function

        Private Sub WriteByteToOutput(ByVal data As Byte, ByVal postIncrement As Integer)
            _tileData(_tileDataPos) = data
            _tileDataPos += If(postIncrement = True, 1, 0)
        End Sub

        Private Sub WriteWordToOutput(ByVal data As UShort, ByVal postIncrement As Integer)
            For i As Integer = 0 To 1
                _tileData(_tileDataPos + i) = CByte(data >> (8 * i)) And &HFF
            Next
            _tileDataPos += If(postIncrement = True, 2, 0)
        End Sub

        Private Sub WriteLongToOutput(ByVal data As UInteger, ByVal postIncrement As Integer)
            For i As Integer = 0 To 3
                _tileData(_tileDataPos + i) = CByte(data >> (8 * i)) And &HFF
            Next
            _tileDataPos += If(postIncrement = True, 4, 0)
        End Sub

        Shared _4ColorCombos As Byte() _
            = {&H0, &H1, &H2, &H3, &H10, &H11, &H12, &H13, &H20, &H21, &H22, &H23, &H30, &H31, &H32, &H33}
        Shared Function Get4ColorCombos(sourceColorPair1 As Byte, sourceColorPair2 As Byte) As Byte()
            Dim sourceColorPairs As Byte() _
                = {sourceColorPair1, sourceColorPair2}
            Dim colors(3) As Byte

            For i As Integer = 0 To 1
                For j As Integer = 0 To 1
                    colors(2 * i + j) = (sourceColorPairs(i) >> (4 * j)) And 15
                Next
            Next

            Dim colorCombo, colorCombos(15) As Byte

            For i As Integer = 0 To 15
                colorCombo = 0
                For j As Integer = 0 To 1
                    colorCombo += colors((_4ColorCombos(i) >> (4 * j)) And 15) << (4 * j)
                Next
                colorCombos(i) = colorCombo
            Next

            Return colorCombos
        End Function

        Sub Decode2Color(
            ByVal width As Integer, height As Integer, offset As Integer,
            ByVal scInputStyle As SourceColorInputStyle, ByVal spInputStyle As SourcePatternInputStyle
            )

            Dim fontDataOffset As Integer = 0
            Dim fontDataLength As Integer = If(width * height < 16, 3, 7)
            Dim patternBytesPerRow As Integer = width \ 2

            With FG
                If scInputStyle = SourceColorInputStyle.Normal Then
                    .SourceColors = ReadColorByte(True)
                ElseIf scInputStyle = SourceColorInputStyle.Conditional Then
                    If (ReadPatternByte(False) And &H80) = 0 Then
                        .SourceColors = ReadColorByte(True)
                    End If
                    _activePatternData(_patternDataPos) = _activePatternData(_patternDataPos) And &H7F
                End If

                For row As Integer = 0 To height - 1
                    If fontDataOffset = 0 Then
                        If spInputStyle <> SourcePatternInputStyle.None Then
                            .SourcePatternMSB = ReadPatternByte(True)
                            If fontDataLength = 7 Then
                                .SourcePatternLSB = ReadPatternByte(True)
                            End If
                        End If
                    End If

                    Array.Copy(.OutputPattern, fontDataOffset, _tileData, _tileDataPos + offset + (4 * row), patternBytesPerRow)
                    fontDataOffset += patternBytesPerRow

                    If fontDataOffset >= fontDataLength Then
                        fontDataOffset = 0
                    End If
                Next

            End With

            If spInputStyle = SourcePatternInputStyle.Preserve Then
                _patternDataPos -= 1
                _activePatternData(_patternDataPos) <<= 4
            End If

        End Sub

        Sub Decode8x8(ByVal spInputStyle As SourcePatternInputStyle)
            Decode2Color(8, 8, 0, SourceColorInputStyle.Normal, spInputStyle)
        End Sub

        Sub Decode8x4(ByVal offset As Integer, ByVal scInputStyle As SourceColorInputStyle, spInputStyle As SourcePatternInputStyle)
            Decode2Color(8, 4, offset, scInputStyle, spInputStyle)
        End Sub

        Sub Decode4x8(ByVal offset As Integer, ByVal scInputStyle As SourceColorInputStyle, spInputStyle As SourcePatternInputStyle)
            Decode2Color(4, 8, offset, scInputStyle, spInputStyle)
        End Sub

        Sub Decode4x4(ByVal offset As Integer, ByVal scInputStyle As SourceColorInputStyle, spInputStyle As SourcePatternInputStyle)
            Decode2Color(4, 4, offset, scInputStyle, spInputStyle)
        End Sub

        Sub Decode4x2(ByVal offset As Integer)
            Decode2Color(4, 2, offset, SourceColorInputStyle.Conditional, SourcePatternInputStyle.Normal)
        End Sub

        Sub Decode2x2(ByVal offset As Integer, spInputStyle As SourcePatternInputStyle)
            Decode2Color(2, 2, offset, SourceColorInputStyle.Conditional, spInputStyle)
        End Sub

        Sub CopyFromColorData(ByVal width As Integer, height As Integer, ByVal offset As Integer, Optional ByVal skipSourceColorUpdate As Boolean = False)
            ''HACK added on 2015/1/22: Keep offsets within bounds
            'If _tileDataPos + offset >= Me._tileData.Length Then
            '    Debug.WriteLine("Out of bounds offset in prodecure ""Copy2x1""")
            '    _tileDataPos = Me._tileData.Length - 1 - offset
            'End If

            For i As Integer = 0 To height - 1
                For j = 0 To (width \ 2) - 1
                    _tileData(_tileDataPos + offset + (i * 4) + j) = ReadColorByte(True)
                Next
            Next

            If skipSourceColorUpdate = False Then
                FG.SourceColors = _lastReadColorByte
            End If

        End Sub

        Sub Copy8x8()
            CopyFromColorData(8, 8, 0, True)
        End Sub

        Sub Copy4x4(ByVal offset As Integer)
            CopyFromColorData(4, 4, offset)
        End Sub

        Sub Copy4x2(ByVal offset As Integer)
            CopyFromColorData(4, 2, offset)
        End Sub

        Sub Copy4x1(ByVal offset As Integer)
            CopyFromColorData(4, 1, offset)
        End Sub

        Sub Copy2x2(ByVal offset As Integer)
            CopyFromColorData(2, 2, offset)
        End Sub

        Sub Copy2x1(ByVal offset As Integer)
            CopyFromColorData(2, 1, offset)
        End Sub

        Public Function ToChunkC1() As ChunkC1
            Dim rawChunkC1 As New RawChunk
            With rawChunkC1
                .Offset = Offset
                .DataLocations = DataLocations
                .Type = ChunkType.VideoC1
                .TrackID = TrackID
                '.SubstreamID = Me.SubstreamID
                .Length = Length

                .Data = New List(Of Byte)

                For i As Integer = 0 To 3
                    .Data.Add(0)
                Next
                .Data.Add(Data(4) Or 128)
                For i As Integer = 5 To 7
                    .Data.Add(Data(i))
                Next

                Dim tileCountBytes As Byte() _
                    = BitConverter.GetBytes(CUShort(UniqueTileCount))
                Array.Reverse(tileCountBytes)
                .Data.AddRange(tileCountBytes)

                Dim tileDataMinusBlankTile(TileData.Length - 33) As Byte
                Array.Copy(TileData, 32, tileDataMinusBlankTile, 0, TileData.Length - 32)
                .Data.AddRange(tileDataMinusBlankTile)
                .Data.AddRange(PaletteData)

                Dim tileMapEntryBytes As Byte()
                For Each tileMapEntry As UShort In TileMapData
                    tileMapEntryBytes = BitConverter.GetBytes(tileMapEntry)
                    Array.Reverse(tileMapEntryBytes)
                    .Data.AddRange(tileMapEntryBytes)
                Next

                .BytesLeft = 0
            End With

            Return New ChunkC1(rawChunkC1)
        End Function

        Public Overrides Function InfoToString(ByVal includeData As Boolean) As String
            Dim chunkInfo As String = MyBase.InfoToString(False) & vbNewLine _
                & $"Palette data @{PaletteDataOffset + Offset + 4:X8} len={PaletteDatalength:X4}{vbNewLine}Code data @{CodeDataOffset + Offset + 4:X8} len={CodeDataLength:X4}{vbNewLine}Color data @{ColorDataOffset + Offset + 4:X8} len={ColorDataLength:X4}"
            Return chunkInfo
        End Function

    End Class

    Public Class ChunkC4
        Inherits ChunkC2

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

        Public Sub InheritTileData(ByRef tileData As Byte())
            _tileData = tileData.Clone
        End Sub

    End Class

    Interface ILZCompressedData
        ReadOnly Property CountMask As UShort
        ReadOnly Property CountBase As Integer
        ReadOnly Property IsDataDecompressed As Boolean
        Function GetDecompressedData(ByRef compressedData As Byte()) As Byte()
    End Interface

    Public MustInherit Class CompressedChunkC1
        Inherits ChunkC1
        Implements ILZCompressedData

        Protected Overridable ReadOnly Property CountMask As UShort Implements ILZCompressedData.CountMask
            Get
                Return &HE000
            End Get
        End Property

        Protected Overridable ReadOnly Property CountBase As Integer Implements ILZCompressedData.CountBase
            Get
                Return 0
            End Get
        End Property

        Protected _isDataDecompressed As Boolean = False
        Protected ReadOnly Property IsDataDecompressed As Boolean Implements ILZCompressedData.IsDataDecompressed
            Get
                Return _isDataDecompressed
            End Get
        End Property

        'Protected Property SwapPixelsOnEvenLines As Boolean

        'Added on 2014/11/25: Support for building tile data from commands
        Public Overrides ReadOnly Property TileData As Byte()
            Get
                'HACK added on 2015/1/1: Decode frame on display instead of on load
                If IsDataDecompressed = False Then
                    DecompressData()
                End If
                Return MyBase.TileData
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            '_isDataDecompressed = False
            DecompressData()
        End Sub

        Protected Overridable Sub DecompressData()
            Dim metadata(MetadataLength - 1) As Byte
            Array.Copy(Data, metadata, metadata.Length)

            Dim compressedData(Data.Length - MetadataLength - 1) As Byte
            Array.Copy(Data, MetadataLength, compressedData, 0, compressedData.Length)

            Dim decompressedData As New List(Of Byte)
            With decompressedData
                .AddRange(metadata)
                .AddRange(GetDecompressedData(compressedData))
                Data = .ToArray
            End With

            _isDataDecompressed = True
        End Sub

        Function GetDecompressedData(ByRef compressedData As Byte()) As Byte() Implements ILZCompressedData.GetDecompressedData
            Return New LZCompressedData(compressedData, CountMask, CountBase).DecompressedData

            Dim blockHeader As UShort
            Dim blockDataLen As Integer
            Dim blockData() As Byte
            Dim blockDataOffset As Integer
            Dim blockDataBytes As UShort

            Dim countRShift As UShort
            For i As Integer = 0 To 15
                If (CountMask >> i) And 1 Then
                    countRShift = i
                    Exit For
                End If
            Next

            Dim offsetMask As UShort _
                = Not CountMask

            Dim count As Integer
            Dim offset As Integer
            Dim decompressedData As New List(Of Byte)

            For i As Integer = 0 To compressedData.Length - 1 Step 34
                blockHeader = (CUShort(compressedData(i)) * 256) + CUShort(compressedData(i + 1))

                blockDataLen = Math.Min(32, compressedData.Count - (i + 2))
                ReDim blockData(blockDataLen - 1)
                Array.Copy(compressedData.ToArray, i + 2, blockData, 0, blockDataLen)

                For blockPos As Integer = 0 To (blockData.Count \ 2) - 1
                    blockDataOffset = blockPos * 2

                    Select Case blockHeader >> (15 - blockPos) And 1
                        Case 0
                            For j As Integer = 0 To 1
                                decompressedData.Add(blockData(blockDataOffset + j))
                            Next
                        Case 1
                            blockDataBytes = (CUShort(blockData(blockDataOffset)) << 8) + CUShort(blockData(blockDataOffset + 1))
                            count = ((blockDataBytes And CountMask) >> countRShift)
                            offset = blockDataBytes And offsetMask

                            If count = 0 And offset = 0 Then
                                'compression is finished -- read to end of data
                                For j As Integer = i + (blockDataOffset + 4) To compressedData.Count - 1
                                    decompressedData.Add(compressedData(j))
                                Next
                                Return decompressedData.ToArray
                            End If

                            count += CountBase

                            ' HACK: to avoid out-of-bounds errors
                            If offset <= 0 Then
                                offset = 1
                            End If
                            For j As Integer = 0 To (count * 2) - 1
                                decompressedData.Add(decompressedData(decompressedData.Count - offset))
                            Next

                    End Select
                Next

            Next

            Return decompressedData.ToArray
            'Return Nothing

        End Function

    End Class

    Public Class ChunkC6
        Inherits CompressedChunkC1 'ChunkC1

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            If ContainsTileMap = False Then
                SwapPixelsOnEvenLines = True
            End If
            'HACK: Fix for "LOGO.SGA" from Sewer Shark
            If (Timecode >= &H1320415) And (Timecode <= &H1321319) And (Offset <= &H185AFA) Then
                SwapPixelsOnEvenLines = False
            End If
        End Sub

    End Class

    Public Class ChunkC7
        Inherits CompressedChunkC1

        Protected Overrides ReadOnly Property CountBase As Integer
            Get
                Return 1
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            SwapPixelsOnEvenLines = False
        End Sub

    End Class

    Public Class ChunkC8
        Inherits CompressedChunkC1

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            SwapPixelsOnEvenLines = True
        End Sub

    End Class

    Public Class ChunkC9
        Inherits CompressedChunkC1

        Protected Overrides ReadOnly Property CountBase As Integer
            Get
                Return 1
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            SwapPixelsOnEvenLines = True
        End Sub

    End Class

    Public Class ChunkCB
        Inherits CompressedChunkC1

        Protected Overrides ReadOnly Property CountMask As UShort
            Get
                Return &HF000
            End Get
        End Property

        Protected Overrides ReadOnly Property CountBase As Integer
            Get
                Return 1
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            SwapPixelsOnEvenLines = False
        End Sub

    End Class

    Public Class ChunkCD
        Inherits ChunkCB

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            SwapPixelsOnEvenLines = True
        End Sub

    End Class

    'Public MustInherit Class VideoOverlayChunk
    '    Inherits ChunkC1

    '    Public Overrides ReadOnly Property SizeInTiles As Size
    '        Get
    '            Return New Size(40, 28)
    '        End Get
    '    End Property

    '    Shadows _tileMapData As UShort()
    '    Public Overridable Shadows Property TileMapData As UShort()
    '        Get
    '            Return _tileMapData
    '        End Get
    '        Protected Set(value As UShort())
    '            _tileMapData = value
    '        End Set
    '    End Property

    '    Sub New(ByRef rawChunk As RawChunk)
    '        MyBase.New(rawChunk)
    '    End Sub

    'End Class

    Interface IVideoOverlayChunk

    End Interface

    Public Class ChunkD1
        'Inherits VideoOverlayChunk
        Inherits ChunkC1

        'Added on 2014/6/28: More generic support for tilemaps
        Public Overrides ReadOnly Property ContainsTileMap As Boolean
            Get
                Return True
            End Get
        End Property

        'Added on 2014/6/28: More generic support for tilemaps
        Public Overrides ReadOnly Property ContainsCompressedTileMap As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides ReadOnly Property MetadataLength As Integer
            Get
                Return 8
            End Get
        End Property

        Public Overrides ReadOnly Property SizeInTiles As Size
            Get
                Return New Size(18, 13)
            End Get
        End Property

        Public Overrides ReadOnly Property UniqueTileCount As Integer
            Get
                Return CInt(Data(6))
            End Get
        End Property

        Public Overrides ReadOnly Property PaletteData As Byte()
            Get
                Return Nothing
            End Get
        End Property

        Public Overrides ReadOnly Property PaletteAsCRAM As UShort()
            Get
                Return _paletteAsCRAM
            End Get
        End Property

        Public Overridable ReadOnly Property LayoutDataLength As Integer
            Get
                Return CInt(Data(7))
            End Get
        End Property

        Private _layoutData As Byte()
        Public ReadOnly Property LayoutData As Byte()
            Get
                If _layoutData Is Nothing Then
                    'Changed on 2015/11/19: Add blank tile to start of tile data
                    Dim offset As Integer =
                        MetadataLength + (UniqueTileCount * 32) 'TileData.Length - 32
                    Dim len As Integer =
                         LayoutDataLength

                    ReDim _layoutData(len - 1)
                    Array.Copy(Data, offset, _layoutData, 0, len)
                End If
                Return _layoutData
            End Get
        End Property

        Protected Overrides ReadOnly Property PaletteMapData As Byte()
            Get
                If _paletteMapData Is Nothing Then
                    Dim offset As Integer =
                        MetadataLength + (UniqueTileCount * 32) + LayoutDataLength 'TileData.Length + LayoutData.Length
                    Dim len As Integer _
                        = Math.Floor((CInt(UniqueTileCount) / 8) + 0.875)

                    ReDim _paletteMapData(len - 1)

                    If offset >= Data.Length Then
                        For i As Integer = 0 To _paletteMapData.Length - 1
                            _paletteMapData(i) = 0
                        Next
                    Else
                        Array.Copy(Data, offset, _paletteMapData, 0, len)
                    End If
                End If
                Return _paletteMapData
            End Get
        End Property

        Protected Shadows _paletteMap As Byte()
        Public Shadows ReadOnly Property PaletteMap As Byte()
            Get
                If _paletteMap Is Nothing Then
                    ReDim _paletteMap(UniqueTileCount - 1)

                    If PaletteMapData Is Nothing Then
                        For tileIndex As Integer = 0 To UniqueTileCount - 1
                            _paletteMap(tileIndex) = 0
                        Next
                        Return _paletteMap
                    End If

                    Dim paletteMapDataOffset As Integer
                    Dim paletteMapEntry As Byte

                    Dim bitIndex As Integer
                    Dim bitPos As Byte

                    For tileIndex As Integer = 0 To UniqueTileCount - 1
                        paletteMapDataOffset = tileIndex \ 8
                        paletteMapEntry = PaletteMapData(paletteMapDataOffset)
                        bitIndex = tileIndex Mod 8
                        bitPos = 7 - bitIndex
                        _paletteMap(tileIndex) = (paletteMapEntry >> bitPos) And 1
                    Next
                End If
                Return _paletteMap
            End Get
        End Property

        Public Overrides ReadOnly Property TileMap As Sega.MegaDrive.VDP.TileMapEntry(,) 'UShort(,)
            Get
                If _tileMap Is Nothing Then
                    ReDim _tileMap(SizeInTiles.Width - 1, SizeInTiles.Height - 1)

                    For row As Integer = 0 To _tileMap.GetUpperBound(1)
                        For col As Integer = 0 To _tileMap.GetUpperBound(0)
                            _tileMap(col, row) = New Sega.MegaDrive.VDP.TileMapEntry
                        Next
                    Next

                    Dim tilePositions As Point() _
                        = GetTilePositions(LayoutData)

                    For i As Integer = 0 To tilePositions.Count - 1
                        tilePositions(i) = New Point(Math.Min(tilePositions(i).X, SizeInTiles.Width - 1), Math.Min(tilePositions(i).Y, SizeInTiles.Height - 1))
                        Dim patternIndex As Integer = ((tilePositions(i).Y * SizeInTiles.Width) + tilePositions(i).X) + 1
                        _tileMap(tilePositions(i).X, tilePositions(i).Y) = New Sega.MegaDrive.VDP.TileMapEntry(False, PaletteMap(i), False, False, patternIndex)
                    Next

                End If
                Return _tileMap
            End Get
        End Property

        Public Sub InheritTileMap(ByVal inheritedTileMap As Sega.MegaDrive.VDP.TileMapEntry(,))
            Dim newTileMap(TileMap.GetUpperBound(0), TileMap.GetUpperBound(1)) As Sega.MegaDrive.VDP.TileMapEntry

            For row As Integer = 0 To TileMap.GetUpperBound(1)
                For col As Integer = 0 To TileMap.GetUpperBound(0)
                    If TileMap(col, row).PatternIndex = 0 Then
                        TileMap(col, row) = New Sega.MegaDrive.VDP.TileMapEntry(inheritedTileMap(col, row))
                    End If
                Next
            Next
        End Sub

        Shared Function GetTilePositions(ByVal encodedTileMapData As Byte()) As Point()
            Dim tilePositions As New List(Of Point)

            Dim layoutCode As Byte
            Dim layoutCount As SByte
            Dim layoutPos As Point = New Point(0, -1)

            For i As Integer = 0 To encodedTileMapData.Length - 1
                layoutCode = (encodedTileMapData(i) >> 6) And 3
                layoutCount = encodedTileMapData(i) And 63

                If (layoutCode And 2) <> 0 Then
                    layoutPos.Y += 1
                    layoutPos.X = 0
                End If

                If (layoutCode And 1) <> 0 Then
                    layoutPos.X += layoutCount + 1
                Else
                    For j As Integer = 0 To layoutCount
                        tilePositions.Add(New Point(layoutPos.X, layoutPos.Y))
                        layoutPos.X += 1
                    Next
                End If
            Next
            Return tilePositions.ToArray
        End Function

        Protected Overrides ReadOnly Property TileDataOffset As Integer
            Get
                Return MetadataLength
            End Get
        End Property

        Public Overrides ReadOnly Property TileData As Byte()
            Get
                If _tileData Is Nothing Then
                    Dim myBaseTileData As Byte() _
                        = MyBase.TileData
                    ReDim _tileData(SizeInTiles.Width * SizeInTiles.Height * 32 - 1)

                    Dim sourceIndex As Integer = 0
                    Dim destinationIndex As Integer = 0

                    Dim tilePositions As Point() _
                        = GetTilePositions(LayoutData)

                    For i As Integer = 0 To tilePositions.Count - 1
                        sourceIndex = i * 32
                        destinationIndex = ((tilePositions(i).Y * SizeInTiles.Width) + tilePositions(i).X) * 32
                        Array.Copy(myBaseTileData, sourceIndex, _tileData, destinationIndex, 32)
                    Next

                End If

                Return _tileData
            End Get
        End Property

        Public Sub InheritTileData(ByVal inheritedTileData As Byte())
            Dim sourceIndex As Integer = 0
            Dim destinationIndex As Integer = 0

            For row As Integer = 0 To TileMap.GetUpperBound(1)
                For col As Integer = 0 To TileMap.GetUpperBound(0)
                    If TileMap(col, row).PatternIndex = 0 Then
                        sourceIndex = ((row * SizeInTiles.Width) + col) * 32
                        destinationIndex = sourceIndex
                        Array.Copy(inheritedTileData, sourceIndex, TileData, destinationIndex, 32)
                    End If
                Next
            Next

        End Sub

        Public Overrides ReadOnly Property RawTiles(ByVal index As Integer) As Byte()
            Get
                If index > 0 Then
                    Dim rawTile(31) As Byte
                    'Array.Copy(Me.TileData, index * 32, rawTile, 0, 32)
                    'Changed on 2015/11/19: Add blank tile to start of tile data
                    Array.Copy(TileData, (index - 1) * 32, rawTile, 0, 32)
                    Return rawTile
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

        Public Overrides Function InfoToString(ByVal includeData As Boolean) As String
            With Me
                Dim chunkInfo As String =
                    MyBase.InfoToString(False)
                If includeData = True Then
                    chunkInfo &= $"{vbNewLine}{vbNewLine}Layout data (length = { .LayoutDataLength:X2}):{vbNewLine}{DataToString(LayoutData, 0)}"
                End If
                Return chunkInfo
            End With
        End Function

    End Class

    Public Class ChunkD2
        Inherits ChunkD5

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

        Protected Overrides Sub DecompressData()

        End Sub

        Shadows ReadOnly Property _countMask As UShort
            Get
                Return Nothing
            End Get
        End Property

        Shadows ReadOnly Property _countBase As Integer
            Get
                Return Nothing
            End Get
        End Property

    End Class

    Public Interface IAnimationChunk
        ReadOnly Property AnimationID As UShort
        ReadOnly Property PixelOffset As Point
    End Interface

    Public Class ChunkD3
        Inherits ChunkC1
        Implements IAnimationChunk

        'NOTES:
        'This type of chunk contains four extra bytes of metadata: two 8-bit values and one 16-bit value
        'Palette data (when present) comes before tile data, which is LZSS compressed like C6 chunks

        Public Overrides ReadOnly Property MetadataLength As Integer
            Get
                Return 12
            End Get
        End Property

        Protected Overrides ReadOnly Property PaletteDataOffset As Integer
            Get
                If Me.PaletteCount = 0 Then
                    Return -1
                Else
                    Return MetadataLength
                End If
            End Get
        End Property

        Public Overrides ReadOnly Property PaletteAsCRAM As UShort()
            Get
                _paletteAsCRAM = MyBase.PaletteAsCRAM
                If _paletteAsCRAM IsNot Nothing Then
                    _paletteAsCRAM(0) = &HE0
                End If
                Return _paletteAsCRAM
            End Get
        End Property

        Public Overrides ReadOnly Property TileMap As Sega.MegaDrive.VDP.TileMapEntry(,) 'UShort(,)
            Get
                _tileMap = MyBase.TileMap

                Dim newTileMap(SizeInTiles.Width - 1, SizeInTiles.Height - 1) As Sega.MegaDrive.VDP.TileMapEntry

                Dim pos As New Point

                For row As Integer = 0 To SizeInTiles.Height - 1
                    For col As Integer = 0 To SizeInTiles.Width - 1
                        With pos
                            .X = (row Mod 2) + ((col * 2) Mod SizeInTiles.Width)
                            .Y = ((row \ 2) * 2) + (col \ (SizeInTiles.Width \ 2))
                        End With
                        newTileMap(col, row) = _tileMap(pos.X, pos.Y)
                    Next
                Next

                'Dim newTileMap(SizeInTiles.Width - 1, SizeInTiles.Height - 1) As UShort

                'Dim pos As New Point

                'For row As Integer = 0 To SizeInTiles.Height - 1
                '    For col As Integer = 0 To SizeInTiles.Width - 1
                '        With pos
                '            .X = (row Mod 2) + ((col * 2) Mod SizeInTiles.Width)
                '            .Y = ((row \ 2) * 2) + (col \ (SizeInTiles.Width \ 2))
                '        End With
                '        newTileMap(col, row) = _tileMap(pos.X, pos.Y)
                '    Next
                'Next

                Return newTileMap
            End Get
        End Property

        Protected Overrides ReadOnly Property TileDataOffset As Integer
            Get
                If Me.PaletteCount = 0 Then
                    Return MetadataLength
                Else
                    Return PaletteDataOffset + PaletteDatalength
                End If
            End Get
        End Property

        Protected ReadOnly Property CompressedTileDataOffset As Integer
            Get
                Return TileDataOffset
            End Get
        End Property

        Protected ReadOnly Property CompressedTileDataLength As Integer
            Get
                Return Data.Length - MetadataLength - PaletteDatalength
            End Get
        End Property

        Public ReadOnly Property PixelOffset As Point Implements IAnimationChunk.PixelOffset
            Get
                Return New Point(Data(8), Data(9))
            End Get
        End Property

        Public Overrides ReadOnly Property SizeInPixels As Size
            Get
                Dim newSize As Size _
                    = MyBase.SizeInPixels
                With newSize
                    .Width += If(PixelOffset.X < 8, 0, Math.Ceiling(PixelOffset.X / 8) * 8)
                    .Height += If(PixelOffset.Y < 8, 0, Math.Ceiling(PixelOffset.Y / 8) * 8)
                End With
                Return newSize
            End Get
        End Property

        Public ReadOnly Property AnimationID As UShort Implements IAnimationChunk.AnimationID
            Get
                Return (CUShort(Data(10)) << 8) + CUShort(Data(11))
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            SwapPixelsOnEvenLines = False
        End Sub

        Public Overrides Function InfoToString(ByVal includeData As Boolean) As String
            With Me
                Dim chunkInfo As String =
                MyBase.InfoToString(False) & vbNewLine &
                $"Pixel offset: { .PixelOffset.X},{ .PixelOffset.Y}" & vbNewLine &
                $"Animation ID: { .AnimationID:X4}"

                If includeData = True Then
                    chunkInfo &= $"{vbNewLine}{vbNewLine}Data (length = { .Data.Length:X4}h):{vbNewLine}{DataToString(Data, MetadataLength)}"
                End If

                Return chunkInfo
            End With
        End Function

    End Class

    Public Class ChunkD4
        Inherits CompressedChunkC1
        Implements IAnimationChunk

        'NOTES:
        'This type of chunk contains four extra bytes of metadata: two 8-bit values and one 16-bit value
        'Palette data (when present) comes before tile data, which is LZSS compressed like C6 chunks

        Public Overrides ReadOnly Property MetadataLength As Integer
            Get
                Return 12
            End Get
        End Property

        Protected Overrides ReadOnly Property PaletteDataOffset As Integer
            Get
                If Me.PaletteCount = 0 Then
                    Return -1
                Else
                    Return MetadataLength
                End If
            End Get
        End Property

        Public Overrides ReadOnly Property PaletteAsCRAM As UShort()
            Get
                _paletteAsCRAM = MyBase.PaletteAsCRAM
                If _paletteAsCRAM IsNot Nothing Then
                    _paletteAsCRAM(0) = &HE0
                End If
                Return _paletteAsCRAM
            End Get
        End Property

        Public Overrides ReadOnly Property TileMap As Sega.MegaDrive.VDP.TileMapEntry(,) 'UShort(,)
            Get
                _tileMap = MyBase.TileMap

                Dim newTileMap(SizeInTiles.Width - 1, SizeInTiles.Height - 1) As Sega.MegaDrive.VDP.TileMapEntry 'UShort

                Dim pos As New Point

                For row As Integer = 0 To SizeInTiles.Height - 1
                    For col As Integer = 0 To SizeInTiles.Width - 1
                        With pos
                            .X = (row Mod 2) + ((col * 2) Mod SizeInTiles.Width)
                            .Y = ((row \ 2) * 2) + (col \ (SizeInTiles.Width \ 2))
                        End With
                        newTileMap(col, row) = _tileMap(pos.X, pos.Y)
                    Next
                Next

                'Dim newTileMap(SizeInTiles.Width - 1, SizeInTiles.Height - 1) As UShort

                'Dim pos As New Point

                'For row As Integer = 0 To SizeInTiles.Height - 1
                '    For col As Integer = 0 To SizeInTiles.Width - 1
                '        With pos
                '            .X = (row Mod 2) + ((col * 2) Mod SizeInTiles.Width)
                '            .Y = ((row \ 2) * 2) + (col \ (SizeInTiles.Width \ 2))
                '        End With
                '        newTileMap(col, row) = _tileMap(pos.X, pos.Y)
                '    Next
                'Next

                Return newTileMap
            End Get
        End Property

        Protected Overrides ReadOnly Property TileDataOffset As Integer
            Get
                If Me.PaletteCount = 0 Then
                    Return MetadataLength
                Else
                    Return PaletteDataOffset + PaletteDatalength
                End If
            End Get
        End Property

        Protected ReadOnly Property CompressedTileDataOffset As Integer
            Get
                Return TileDataOffset
            End Get
        End Property

        Protected ReadOnly Property CompressedTileDataLength As Integer
            Get
                Return Data.Length - MetadataLength - PaletteDatalength
            End Get
        End Property

        Public ReadOnly Property PixelOffset As Point Implements IAnimationChunk.PixelOffset
            Get
                Return New Point(Data(8), Data(9))
            End Get
        End Property

        Public Overrides ReadOnly Property SizeInPixels As Size
            Get
                Dim newSize As Size _
                    = MyBase.SizeInPixels
                With newSize
                    .Width += If(PixelOffset.X < 8, 0, Math.Ceiling(PixelOffset.X / 8) * 8)
                    .Height += If(PixelOffset.Y < 8, 0, Math.Ceiling(PixelOffset.Y / 8) * 8)
                End With
                Return newSize
            End Get
        End Property

        Public ReadOnly Property AnimationID As UShort Implements IAnimationChunk.AnimationID
            Get
                Return (CUShort(Data(10)) << 8) + CUShort(Data(11))
            End Get
        End Property

        Protected Overrides ReadOnly Property CountMask As UShort
            Get
                Return &HF000
            End Get
        End Property

        Protected Overrides ReadOnly Property CountBase As Integer
            Get
                Return 1
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            SwapPixelsOnEvenLines = False
            'Added on 2015/7/3: Set TrackID to AnimationID for better filtering support
            TrackID = AnimationID
        End Sub

        Protected Overrides Sub DecompressData()
            Dim decompressedData As New List(Of Byte)
            Dim metadata(MetadataLength - 1) As Byte
            Dim paletteData As Byte() _
                = Nothing
            Dim compressedTileData(CompressedTileDataLength - 1) As Byte

            Array.Copy(Data, metadata, metadata.Length)
            If PaletteCount > 0 Then
                ReDim paletteData(PaletteDatalength - 1)
                Array.Copy(Data, PaletteDataOffset, paletteData, 0, paletteData.Length)
            End If
            Array.Copy(Data, CompressedTileDataOffset, compressedTileData, 0, compressedTileData.Length)

            decompressedData.AddRange(metadata)
            If paletteData IsNot Nothing Then
                decompressedData.AddRange(paletteData)
            End If
            decompressedData.AddRange(GetDecompressedData(compressedTileData))

            Data = decompressedData.ToArray

            _isDataDecompressed = True
        End Sub

        Public Overrides Function InfoToString(ByVal includeData As Boolean) As String
            With Me
                Dim chunkInfo As String =
                MyBase.InfoToString(False) & vbNewLine &
                $"Pixel offset: { .PixelOffset.X},{ .PixelOffset.Y}" & vbNewLine &
                $"Animation ID: { .AnimationID:X4}"

                If includeData = True Then
                    chunkInfo &= $"{vbNewLine}{vbNewLine}Data (length = { .Data.Length:X4}h):{vbNewLine}{DataToString(Data, MetadataLength)}"
                End If

                Return chunkInfo
            End With
        End Function

    End Class

    Public Class ChunkD5
        Inherits CompressedChunkC1 'UnsupportedVideoChunk 

        Public Overrides ReadOnly Property MetadataLength As Integer
            Get
                Return 10
            End Get
        End Property

        'Added on 2014/6/28: More generic support for tilemaps
        Public Overrides ReadOnly Property ContainsTileMap As Boolean
            Get
                Return True
            End Get
        End Property

        'Added on 2014/6/28: More generic support for tilemaps
        Public Overrides ReadOnly Property ContainsCompressedTileMap As Boolean
            Get
                Return True
            End Get
        End Property

        Private _sizeInTiles As Size = New Size(24, 17)
        Public Overrides ReadOnly Property SizeInTiles As Size
            Get
                Return _sizeInTiles
            End Get
        End Property

        Public Overrides ReadOnly Property UniqueTileCount As Integer
            Get
                Dim count As Integer
                Dim bytes As Byte() _
                    = {Data(6), Data(7)}
                Array.Reverse(bytes)
                count = CInt(BitConverter.ToInt16(bytes, 0))
                Return count
            End Get
        End Property

        Public Overrides ReadOnly Property PaletteAsCRAM As UShort()
            Get
                'Added on 2015/6/2: Change first palette color to green
                _paletteAsCRAM = MyBase.PaletteAsCRAM
                _paletteAsCRAM(0) = &HE0
                Return _paletteAsCRAM
            End Get
        End Property

        Public Overridable ReadOnly Property LayoutDataLength As Integer
            Get
                Dim len As Integer
                Dim bytes As Byte() _
                    = {Data(8), Data(9)}
                Array.Reverse(bytes)
                len = CInt(BitConverter.ToInt16(bytes, 0))
                Return len
            End Get
        End Property

        Private ReadOnly Property LayoutDataOffset As Integer
            Get
                Return MetadataLength + TileDataLength + PaletteDatalength
            End Get
        End Property

        Private _layoutData As Byte()
        Public ReadOnly Property LayoutData As Byte()
            Get
                If _layoutData Is Nothing Then
                    ReDim _layoutData(LayoutDataLength - 1)
                    Array.Copy(Data, LayoutDataOffset, _layoutData, 0, LayoutDataLength)
                End If
                Return _layoutData
            End Get
        End Property

        Public Overrides ReadOnly Property TileMap As Sega.MegaDrive.VDP.TileMapEntry(,) 'UShort(,)
            Get
                If _tileMap Is Nothing Then
                    If SizeInTiles = Size.Empty Then Return Nothing

                    ReDim _tileMap(SizeInTiles.Width - 1, SizeInTiles.Height - 1)

                    For i As Integer = 0 To _tileMap.GetUpperBound(0)
                        For j As Integer = 0 To _tileMap.GetUpperBound(1)
                            _tileMap(i, j) = New Sega.MegaDrive.VDP.TileMapEntry(0)
                        Next
                    Next

                    Dim tilePositions As Point() _
                        = ChunkD1.GetTilePositions(LayoutData)

                    Dim paletteIndex As UShort

                    For i As Integer = 0 To tilePositions.Count - 1
                        paletteIndex = 0 '1 << 13
                        'HACK: Keep tilePosition values in bounds
                        If (tilePositions(i).X > SizeInTiles.Width - 1) Or (tilePositions(i).Y > SizeInTiles.Height - 1) Then
                            Debug.WriteLine("{Me.GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod().Name}: Tile layout position in tilemap exceeded expected boundaries.")
                        End If

                        tilePositions(i) = New Point(Math.Min(tilePositions(i).X, SizeInTiles.Width - 1), Math.Min(tilePositions(i).Y, SizeInTiles.Height - 1))

                        _tileMap(tilePositions(i).X, tilePositions(i).Y) = New Sega.MegaDrive.VDP.TileMapEntry(paletteIndex + i + 1)
                    Next

                End If
                Return _tileMap
            End Get
        End Property

        Protected Overrides ReadOnly Property CountMask As UShort
            Get
                Return &HF000
            End Get
        End Property

        Protected Overrides ReadOnly Property CountBase As Integer
            Get
                Return 1
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            SwapPixelsOnEvenLines = False
        End Sub

    End Class

    Public Class ChunkD7
        Inherits Chunk81
        Implements IAnimationChunk

        Public Overrides ReadOnly Property MetadataLength As Integer
            Get
                Return 12
            End Get
        End Property

        Protected Overrides ReadOnly Property PaletteStartOffset As Byte
            Get
                Return 0 'Data(4)
            End Get
        End Property

        Public Overrides ReadOnly Property PaletteAsCRAM As UShort()
            Get
                _paletteAsCRAM = MyBase.PaletteAsCRAM

                _paletteAsCRAM(0) = &H3E0

                Return _paletteAsCRAM
            End Get
        End Property

        Public Overrides ReadOnly Property SizeInPixels As Size
            Get
                Dim newSize As Size _
                    = MyBase.SizeInPixels
                With newSize
                    .Width += If(PixelOffset.X < 8, 0, Math.Ceiling(PixelOffset.X / 8) * 8)
                    .Height += If(PixelOffset.Y < 8, 0, Math.Ceiling(PixelOffset.Y / 8) * 8)
                End With
                Return newSize
            End Get
        End Property

        Public ReadOnly Property PixelOffset As Point Implements IAnimationChunk.PixelOffset
            Get
                Return New Point(Data(8), Data(9))
            End Get
        End Property

        Public ReadOnly Property AnimationID As UShort Implements IAnimationChunk.AnimationID
            Get
                Return (CUShort(Data(10)) << 8) + CUShort(Data(11))
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

        Public Overrides Function InfoToString(ByVal includeData As Boolean) As String
            With Me
                Dim chunkInfo As String =
                MyBase.InfoToString(False) & vbNewLine &
                $"Pixel offset: { .PixelOffset.X},{ .PixelOffset.Y}" & vbNewLine &
                $"Animation ID: { .AnimationID:X4}"

                If includeData = True Then
                    chunkInfo &= $"{vbNewLine}{vbNewLine}Data (length = { .Data.Length:X4}h):{vbNewLine}{DataToString(Data, MetadataLength)}"
                End If

                Return chunkInfo
            End With
        End Function

    End Class

    Public Class ChunkE7
        Inherits CompressedChunkC1

        Public Overrides ReadOnly Property MetadataLength As Integer
            Get
                Return 14 '_dataOffset
            End Get
        End Property

        Protected Overrides ReadOnly Property CountMask As UShort
            Get
                Return &HF000
            End Get
        End Property

        Protected Overrides ReadOnly Property CountBase As Integer
            Get
                Return 1
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            SwapPixelsOnEvenLines = True
        End Sub

        Protected Overrides Sub DecompressData()
            Dim sfOffset As Integer = MetadataLength
            Dim sfLength As Integer = 0
            Dim sfTopBit As Integer
            Dim sfData As Byte()

            Dim decompressedData As New List(Of Byte)

            Dim metadata(MetadataLength - 1) As Byte
            Array.Copy(Data, metadata, metadata.Length)
            decompressedData.AddRange(metadata)

            For i = 0 To 2
                sfOffset += sfLength
                sfLength = (CInt((Data(8 + (i * 2))) And 255) << 8) + CInt(Data(9 + (i * 2)))
                sfTopBit = (sfLength And 32768) >> 15
                sfLength = sfLength And 32767

                ReDim sfData(sfLength - 1)
                Array.Copy(Data, sfOffset, sfData, 0, sfData.Length)

                If sfTopBit = 1 Then
                    decompressedData.AddRange(sfData)
                Else
                    decompressedData.AddRange(GetDecompressedData(sfData))
                End If
            Next

            ' Add palette data, etc to the output stream
            sfOffset += sfLength
            ReDim sfData(Data.Length - sfOffset - 1)
            Array.Copy(Data, sfOffset, sfData, 0, sfData.Length)
            decompressedData.AddRange(sfData)

            Data = decompressedData.ToArray

            _isDataDecompressed = True
        End Sub

    End Class

    Public Class ChunkE8
        Inherits ChunkC1

        Private _compressedFrameData As Byte()
        Public ReadOnly Property CompressedFrameData As Byte()
            Get
                Return _compressedFrameData
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)

            ReDim _compressedFrameData(Data.Length - MetadataLength - 1)
            Array.Copy(Data, MetadataLength, _compressedFrameData, 0, _compressedFrameData.Length)

            'DecompressFrameData()
        End Sub

        Public Function DecompressFrameData(Optional ByVal inheritedFrameData As Byte() = Nothing) As Byte()
            Dim metadata(MetadataLength - 1) As Byte
            Array.Copy(Data, metadata, metadata.Length)

            Dim decompressedFrameData As Byte() _
                = GetDecompressedFrameData(inheritedFrameData)

            Dim newData As New List(Of Byte)
            With newData
                .AddRange(metadata)
                .AddRange(decompressedFrameData)
            End With

            _data = newData.ToArray

            'Force recalculation of all currently calculated data
            _paletteAsCRAM = Nothing
            _paletteMap = Nothing
            _tileData = Nothing
            _tileMap = Nothing

            Return decompressedFrameData.Clone
        End Function

        Private Function GetDecompressedFrameData(Optional ByVal inheritedFrameData As Byte() = Nothing) As Byte()
            Dim codeWord As Byte
            Dim flagBits As UInteger
            Dim flagBit As Byte

            Dim offset As Integer

            ''TEST Added on 2014/8/3: keep a list of code words and flag bits to figure out how they're used
            'Dim codesAndFlags As New List(Of KeyValuePair(Of String, String))

            Dim decompressedFrameData As New List(Of Byte)

            Dim dataSource As IEnumerable(Of Byte)

            For i As Integer = 0 To CompressedFrameData.Length - 1
                codeWord = CompressedFrameData(i)

                Select Case codeWord
                    Case 0 '32 bytes of uncompressed data
                        For j As Integer = i + 1 To i + 32
                            decompressedFrameData.Add(CompressedFrameData(j))
                        Next
                        i += 32

                    Case 1 To 15
                        flagBits = 0
                        For j As Integer = 3 To 0 Step -1
                            i += 1
                            flagBits += CUInt(CompressedFrameData(i)) << (8 * j)
                        Next

                        For j As Integer = 31 To 0 Step -1
                            flagBit = CByte((flagBits And (1 << j)) >> j)
                            Select Case flagBit
                                Case 0 'is a literal byte
                                    i += 1
                                    decompressedFrameData.Add(CompressedFrameData(i))

                                Case 1 'is a byte to be copied from previously decompressed data
                                    Select Case codeWord
                                        Case 1 '! 0001
                                            offset = 1 '!
                                        Case 2 '! 0010
                                            offset = 4 '!
                                        Case 3 '! 0011
                                            offset = 8 '!
                                        Case 4 'X 0100
                                            offset = 32
                                        Case 5 '! 0101
                                            offset = 0 '!
                                        Case 6 '! 0110
                                            offset = 1 '!
                                        Case 7 '! 0111
                                            offset = -1 '!
                                        Case 8 '! 1000
                                            offset = 4 '!
                                        Case 9 '! 1001
                                            offset = -4 '!
                                        Case 10 '! 1010
                                            offset = 8 '!
                                        Case 11 '! 1011
                                            offset = -8 '!
                                        Case 12 '! 1100
                                            offset = 32 '!
                                        Case 13 '! 1101
                                            offset = -32 '!
                                        Case 14 'X 1110
                                            offset = 64
                                        Case 15 'X 1111
                                            offset = -64
                                    End Select

                                    If codeWord >= 5 Then
                                        dataSource = inheritedFrameData
                                    Else
                                        dataSource = decompressedFrameData '.ToArray
                                    End If

                                    If dataSource Is Nothing Then
                                        decompressedFrameData.Add(0)
                                    Else
                                        decompressedFrameData.Add(dataSource(Math.Min(dataSource.Count - 1, decompressedFrameData.Count - offset)))
                                    End If

                            End Select
                        Next

                    ''TEST Added on 2014/8/3: keep a list of code words and flag bits to figure out how they're used
                    'codesAndFlags.Add(New KeyValuePair(Of String, String)("0x" & Hex(compCodeWord).PadLeft(2, "0"), "0x" & Hex(compFlagBits).PadLeft(8, "0")))

                    Case &HFF 'halfway / finished marker?
                        If i Mod 2 = 0 Then
                            i += 1
                        End If

                        If CompressedFrameData.Length - (i + 1) = 32 Then
                            For j As Integer = i + 1 To CompressedFrameData.Length - 1
                                decompressedFrameData.Add(CompressedFrameData(j))
                            Next
                            Exit For
                        Else
                            Continue For
                        End If

                    Case Else
                        decompressedFrameData.Add(CompressedFrameData(i))

                End Select

            Next

            Return decompressedFrameData.ToArray

        End Function

    End Class

    Public Class ChunkE9
        Inherits ChunkE8

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

    End Class

    Public Class UnsupportedVideoChunk
        Inherits VideoChunk

        Public Overrides ReadOnly Property PaletteAsCRAM As UShort()
            Get
                Return Nothing
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

        Public Overrides Function InfoToString(ByVal includeData As Boolean) As String
            With Me
                Dim chunkInfo As String _
                    = MyBase.InfoToString(True)
                Return chunkInfo
            End With
        End Function

    End Class

    Public Class ChunkF0
        Inherits Chunk

        Public Overrides ReadOnly Property Timecode As UInteger
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property FrameCount As Integer
            Get
                Return CInt(Data(0) * 256) + CInt(Data(1))
            End Get
        End Property

        Public ReadOnly Property FramesPerSecond As Integer
            Get
                Return CInt(Data(3))
            End Get
        End Property

        Public ReadOnly Property SizeInTiles As Size
            Get
                Return New Size(CInt(Data(4)), CInt(Data(5)))
            End Get
        End Property

        Public ReadOnly Property SizeInPixels As Size
            Get
                Return New Size(SizeInTiles.Width * 8, SizeInTiles.Height * 8)
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

    End Class

    Public MustInherit Class ContainerChunk
        Inherits Chunk

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

        Protected _chunks As List(Of Chunk)
        Public Property Chunks As List(Of Chunk)
            Get
                If _chunks Is Nothing Then
                    Return Nothing
                Else
                    Return _chunks
                End If
            End Get
            Protected Set(value As List(Of Chunk))
                _chunks = value
            End Set
        End Property

        Public ReadOnly Property VideoChunks As VideoChunk()
            Get
                Return Chunks.OfType(Of VideoChunk).ToArray
            End Get
        End Property

        Public ReadOnly Property AudioChunks As AudioChunk()
            Get
                Return Chunks.OfType(Of AudioChunk).ToArray
            End Get
        End Property

        Public ReadOnly Property ContainsVideo As Boolean
            Get
                If VideoChunks.Count > 0 Then
                    Return True
                End If
                Return False

                'For Each chunk As Chunk In Me.Chunks
                '    If TypeOf chunk Is VideoChunk Then
                '        Return True
                '    End If
                'Next
                'Return False
            End Get
        End Property

        Public ReadOnly Property ContainsAudio As Boolean
            Get
                If AudioChunks.Count > 0 Then
                    Return True
                End If
                Return False
            End Get
        End Property

        Public Overrides ReadOnly Property Timecode As UInteger
            Get
                If ContainsVideo = True Then
                    Return VideoChunks.First.Timecode
                Else
                    Return AudioChunks.First.Timecode
                End If
            End Get
        End Property

    End Class

    Public Class ChunkF1
        Inherits ContainerChunk

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)

            Chunks = New List(Of Chunk)

            'Dim rawChunkOffset As Integer _
            '    = 4
            Dim rawChunkOffset As Integer _
                = If(ByteOrder = ByteOrder.LittleEndian, 0, 4)

            While rawChunkOffset <= Data.Length - 2
                Dim releasedRawChunk As RawChunk _
                    = GetContainedChunk(rawChunkOffset)
                releasedRawChunk.Offset += Offset
                Chunks.Add(Chunk.GetNewChunk(releasedRawChunk))
                rawChunkOffset += releasedRawChunk.Length + 4
            End While

            'If Chunks.Count > 2 Then
            '    Debug.WriteLine("foo queue")
            'End If

            ReDim Preserve Data(3)

        End Sub

        Protected Function GetContainedChunk(ByVal offset As Integer) As RawChunk
            Dim rawChunk As New RawChunk
            With rawChunk
                .Offset = offset

                .ByteOrder = ByteOrder

                .Type = Data(offset)
                .TrackID = Data(offset + 1)

                Dim bytes(1) As Byte
                Array.Copy(Data, offset + 2, bytes, 0, 2)
                If ByteOrder = ByteOrder.BigEndian Then
                    Array.Reverse(bytes)
                End If
                .Length = BitConverter.ToInt16(bytes, 0)
                '.Length = (CUShort(Data(offset + 2)) << 8) + CUShort(Data(offset + 3))

                Dim chunkData(.Length - 1) As Byte
                Array.Copy(Data, offset + 4, chunkData, 0, chunkData.Length)

                .Data.AddRange(chunkData)
            End With
            Return rawChunk
        End Function

    End Class

    Public Class ChunkF2
        Inherits Chunk

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

    End Class

    Public Class Chunk99Part
        Inherits Chunk

        'NOTES (2016/5/13):
        '- There appear to be two variants of this chunk type
        '  (1) found in Megadrive ports
        '  (2) found in Saturn ports
        '- Chunks of type 2 appear to have a length limit of $7FFA, but they can be chained together.
        '  Chained chunks contain only 8 bytes of metadata (chunk type, track ID, and time code)
        '
        'NOTES on Saturn version
        '  METADATA:
        '  - Values are stored as unsigned, big-endian 16-bit words
        '  - The first two bytes contain the chunk type and track ID
        '  - The next word contains the length
        '  - The next dblword contains the number of chained chunks that follow
        '  - The next dblword contains the total length of the current chunk and all chained chunks
        '    - The top byte is always set to $99, and can be ignored
        '  - The next two words are unknown
        '  - The next two words contain the screen width and height, respectively
        '  - The next word contains the length of palette data in words
        '  - The next two words contain the width and height in 8x8 tiles, respectively
        '  - The next word contains the number of unique tiles
        '  - The next word is unknown
        '  - The next word appears to contain an offset to something
        '
        '  DATA:
        '  - The data are LZ compressed:
        '    - The block size is 34 bytes
        '    - The first word contains compression flags (0 = raw, 1 = compressed)
        '    - Length / offset pair is as follows:
        '      - length = top 3 bits + 1
        '      - offset = bottom 13 bits
        '  - Data begins with tile map
        '    - Each time map entry is 2 bytes in length
        '  - Next comes tile data
        '    - Tiles 128 bytes in length
        '    - They consist of raw RGB555 pixels, 2 bytes each

        Public Overrides ReadOnly Property MetadataLength As Integer
            Get
                Return 4
            End Get
        End Property

        Protected ReadOnly Property RemainingPartCount As UInteger
            Get
                Return BigEndian.Get32BitValueFromByteArray(Data, 4)
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

    End Class

    Public Class Chunk99PartList
        Inherits List(Of Chunk99Part)

        Public Function ToChunk99() As Chunk99
            'Concatenate the frame data from all parts
            Dim chunk99Bytes As New List(Of Byte)

            For Each part As Chunk99Part In Me
                chunk99Bytes.AddRange(part.FrameData)
            Next

            Dim rawChunk As New RawChunk

            With rawChunk
                .Offset = First.Offset
                .EndOffset = Last.EndOffset
                .DataLocations = First.DataLocations
                .Type = ChunkType.Tileset99
                .TrackID = First.TrackID

                Dim realLengthBytes As Byte() _
                    = {0, chunk99Bytes(1), chunk99Bytes(2), chunk99Bytes(3)}
                Array.Reverse(realLengthBytes)
                Dim realLength As Integer _
                    = BitConverter.ToInt32(realLengthBytes, 0)
                .Length = realLength
                '.ReportedLength = realLength

                chunk99Bytes.RemoveRange(0, 4)

                .Data.AddRange(chunk99Bytes)
            End With

            Return New Chunk99(rawChunk)

        End Function

    End Class

    Public Class Chunk99
        Inherits Super32XVideoChunk

        Public Overrides ReadOnly Property MetadataLength As Integer
            Get
                Return 20
            End Get
        End Property

        Protected Property _totalLengthOffset As Integer = 6
        Public Overridable ReadOnly Property TotalLength As UShort
            Get
                Return BigEndian.Get16BitValueFromByteArray(Data, _totalLengthOffset)
            End Get
        End Property

        Protected Property _timecodeOffset As Integer = 0
        Public Overrides ReadOnly Property Timecode As UInteger
            Get
                Return BigEndian.Get32BitValueFromByteArray(Data, _timecodeOffset)
            End Get
        End Property

        Protected Property _sizeInPixelsOffset As Integer = 4
        Public Overrides ReadOnly Property SizeInPixels As Size
            Get
                Return New Size(
                    BigEndian.Get16BitValueFromByteArray(Data, _sizeInPixelsOffset),
                    BigEndian.Get16BitValueFromByteArray(Data, _sizeInPixelsOffset + 2)
                    )
            End Get
        End Property

        Protected Overrides ReadOnly Property PaletteStartOffset As Byte
            Get
                Return 0 'Data(4)
            End Get
        End Property

        Protected Property _paletteEntryCountOffset As Integer = 8
        Protected Overrides ReadOnly Property PaletteEntryCount As Byte
            Get
                Return BigEndian.Get16BitValueFromByteArray(Data, _paletteEntryCountOffset)
            End Get
        End Property

        Protected Property _sizeInTilesOffset As Integer = 10
        Public Overrides ReadOnly Property SizeInTiles As Size
            Get
                Return New Size(
                    BigEndian.Get16BitValueFromByteArray(Data, _sizeInTilesOffset),
                    BigEndian.Get16BitValueFromByteArray(Data, _sizeInTilesOffset + 2)
                    )
            End Get
        End Property

        Protected Property _uniqueTileCountOffset As Integer = 14
        Public Overrides ReadOnly Property UniqueTileCount As Integer
            Get
                Return BigEndian.Get16BitValueFromByteArray(Data, _uniqueTileCountOffset)
            End Get
        End Property

        Private ReadOnly Property TileMapDataStartOffset As Integer
            Get
                Return 0
            End Get
        End Property

        Private ReadOnly Property TileMapDataLength As Integer
            Get
                With SizeInTiles
                    Return .Width * .Height * 2
                End With
            End Get
        End Property

        Private ReadOnly Property TileMapData As Byte()
            Get
                Dim _tileMapData(TileMapDataLength - 1) As Byte
                Array.Copy(FrameData, 0, _tileMapData, TileMapDataStartOffset, TileMapDataLength)
                Return _tileMapData
            End Get
        End Property

        Private ReadOnly Property TileMapLength As Integer
            Get
                With SizeInTiles
                    Return .Width * .Height
                End With
            End Get
        End Property

        Private ReadOnly Property TileMap As UShort()
            Get
                Dim _tileMap As New List(Of UShort)(TileMapLength)

                Dim _tileMapData As Byte() _
                    = TileMapData

                For i As Integer = 0 To TileMapDataLength - 1 Step 2
                    Dim bytes(1) As Byte
                    Array.Copy(_tileMapData, i, bytes, 0, 2)
                    Array.Reverse(bytes)
                    _tileMap.Add(BitConverter.ToUInt16(bytes, 0))
                Next

                Return _tileMap.ToArray
            End Get
        End Property

        Private ReadOnly Property TileDataStartOffset As Integer
            Get
                Return TileMapDataStartOffset + TileMapDataLength
            End Get
        End Property

        Private ReadOnly Property TileDataLength As Integer
            Get
                Return UniqueTileCount * 128
            End Get
        End Property

        Private ReadOnly Property TileData As Byte()
            Get
                Dim _tileData(TileDataLength - 1) As Byte
                Array.Copy(FrameData, TileDataStartOffset, _tileData, 0, TileDataLength)
                Return _tileData
            End Get
        End Property

        Public Overrides ReadOnly Property PaletteAsCRAM As UShort()
            Get
                '1. Create a list of unique 15-bit pixel color values stored in the tile data
                '2. Return it as an array

                Return {0}

                'If _paletteAsCRAM Is Nothing Then
                '    If Me.PaletteEntryCount = 0 Then
                '        Return Nothing
                '    End If

                '    ReDim _paletteAsCRAM(PaletteEntryCount - 1)

                '    Dim offset As Integer

                '    For i As Integer = 0 To _paletteAsCRAM.Length - 1
                '        offset = MetadataLength + (i * 2)
                '        _paletteAsCRAM(i) = CUShort(Data(offset)) * 256 + CUShort(Data(offset + 1))
                '    Next
                'End If
                'Return _paletteAsCRAM
            End Get
        End Property

        Public Overrides ReadOnly Property FrameBufferData As Short() 'Byte()
            Get
                '1. Read the tile map
                '2. Read the tile pixel data
                '3. Convert the tile pixel data from 15-bit direct to 8-bit indexed
                '   (This should be possible!)
                '4. Build the frame buffer data using the new 8-bit pixel data and the tile map

                Return {0}
            End Get
        End Property

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            DecompressFrameData()
        End Sub

        Private Sub DecompressFrameData()
            'Decompress data
            Dim lzcData As New LZCompressedData(FrameData.Clone, CInt(3), CInt(1))

            Dim decompressedFrameData As Byte() _
                = lzcData.DecompressedData

            'Preserve metadata information
            ReDim Preserve Data(MetadataLength + decompressedFrameData.Length - 1)
            Array.Copy(decompressedFrameData, 0, Data, MetadataLength, decompressedFrameData.Length)
        End Sub

        Public Function ToBitmap() As Bitmap
            Dim tileBitmaps As New List(Of Bitmap)(UniqueTileCount)

            Dim tileData As Byte() _
                = Me.TileData

            For tileDataOffset As Integer = 0 To TileDataLength - 1 Step 128
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

            Dim sizeInPixels As New Size(SizeInTiles.Width * 8, SizeInTiles.Height * 8)

            Dim tileMap As UShort() _
                = Me.TileMap

            Using b As New Bitmap(sizeInPixels.Width, sizeInPixels.Height, Imaging.PixelFormat.Format24bppRgb)
                Using g As Graphics = Graphics.FromImage(b)

                    For row As Integer = 0 To SizeInTiles.Height - 1
                        For col As Integer = 0 To SizeInTiles.Width - 1

                            Dim tileMapIndex As Integer _
                                = (row * SizeInTiles.Width) + col

                            Dim tileIndex As Integer _
                                = (tileMap(tileMapIndex) And &H7FF) - 1

                            Using tb As Bitmap = tileBitmaps(tileIndex).Clone
                                Dim hflip As Byte _
                                 = (tileMap(tileMapIndex) >> 13) And 1
                                If hflip = 1 Then
                                    tb.RotateFlip(RotateFlipType.RotateNoneFlipX)
                                End If

                                Dim vflip As Byte _
                                 = (tileMap(tileMapIndex) >> 14) And 1
                                If vflip = 1 Then
                                    tb.RotateFlip(RotateFlipType.RotateNoneFlipY)
                                End If

                                Dim drawRect As New Rectangle(col * 8, row * 8, 8, 8)
                                g.DrawImage(tb, drawRect)
                            End Using

                        Next
                    Next

                End Using

                Return b.Clone

            End Using

        End Function

    End Class

    Public Class ChunkF3
        Inherits Chunk

        Public Overrides ReadOnly Property MetadataLength As Integer
            Get
                Return 14
            End Get
        End Property

        'NOTES (2016/6/12):
        '- This is a metadata-only container chunk for chunk type $99, used in Saturn ports
        '
        '  METADATA:
        '  - Values are stored as unsigned, big-endian 16-bit words
        '  - The first two bytes contain the chunk type and track ID, respectively
        '  - The next dblword contains the SMPTE time code
        '  - The next word contains the number of F2 chunks contained in the file
        '  - The next dblword contains the length of the $99 chunk

        Public Property PartList As Chunk99PartList

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
            PartList = New Chunk99PartList
        End Sub

    End Class

    Public Class ChunkF9
        Inherits ChunkF1

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

    End Class

    Public Class ChunkFF
        Inherits Chunk

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

    End Class

    Public Class UnsupportedChunk
        Inherits Chunk

        Sub New(ByRef rawChunk As RawChunk)
            MyBase.New(rawChunk)
        End Sub

    End Class

End Namespace

