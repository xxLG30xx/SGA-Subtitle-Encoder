Imports System.IO
Imports System.IO.Path

Namespace SGA

    Public Enum ByteOrder As Integer
        BigEndian = 0
        LittleEndian = 1
        MiddleEndian = 2
    End Enum

    Public Class File
        Implements Interfaces.IProgressReporter
        Public Event ProgressChanged(sender As Object, e As Interfaces.ProgressReporterEventArgs) Implements Interfaces.IProgressReporter.ProgressChanged

        Private _fileName As String
        Public Property FileName As String
            Get
                Return _fileName
            End Get
            Private Set(value As String)
                _fileName = value
            End Set
        End Property

        Public Enum FileFormat As Integer
            EarlyMegaCDor32X = 1
            LaterMegaCD = 2
            Saturn = 3
            _3DO = 4
            AVC = 5
            DOSorMac = 6
        End Enum

        Private _version As FileFormat
        Public Property Format As FileFormat
            Get
                Return _version
            End Get
            Private Set(value As FileFormat)
                _version = value
            End Set
        End Property

        Private _sectorHeaders As UShort()
        Public ReadOnly Property SectorHeaders As UShort()
            Get
                Return _sectorHeaders
            End Get
        End Property

        Private _sectorHeadersChecked As Boolean = False
        Private _hasSectorHeaders As Boolean = True
        Public ReadOnly Property HasSectorHeaders As Boolean
            Get
                If _sectorHeadersChecked = False Then

                    If SectorHeaders.Length > 0 Then
                        If Format = 1 Then
                            For Each header As UShort In SectorHeaders
                                If ((header > &H7FE) And (header < &H8100)) _
                                Or ((header > &H8200) And (header < &HA100)) _
                                Or ((header > &HA200) And (header < &HC100)) Then
                                    _hasSectorHeaders = False
                                    Exit For
                                End If
                            Next
                            _sectorHeadersChecked = True
                        End If

                    End If

                End If

                Return _hasSectorHeaders
            End Get
        End Property

        'Private _chunks As Dictionary(Of Long, Chunk)
        'Public Property ChunkDictionary As Dictionary(Of Long, Chunk)
        '    Get
        '        Return _chunks
        '    End Get
        '    Private Set(value As Dictionary(Of Long, Chunk))
        '        _chunks = value
        '    End Set
        'End Property

        Private _chunks As List(Of Chunk)
        Public Property Chunks As List(Of Chunk)
            Get
                Return _chunks
            End Get
            Set(value As List(Of Chunk))
                _chunks = value
            End Set
        End Property

        Private _offsets As List(Of Long)
        Public ReadOnly Property Offsets As List(Of Long)
            Get
                If _offsets Is Nothing Then
                    _offsets = New List(Of Long)
                    For Each chunk As Chunk In Chunks
                        _offsets.Add(chunk.Offset)
                    Next
                End If
                Return _offsets
            End Get
        End Property

        Public ReadOnly Property ChunkAtOffset(ByVal offset As Long) As Chunk
            Get
                If Offsets Is Nothing Then Return Nothing
                If Offsets.Contains(offset) = False Then
                    Return Nothing
                Else
                    For Each chunk As Chunk In Chunks
                        If chunk.Offset = offset Then
                            Return chunk
                        End If
                    Next
                End If
                Return Nothing
            End Get
        End Property

        'Public ReadOnly Property ChunkOffsets As List(Of Long)
        '    Get
        '        Return _chunks.Keys.ToList
        '    End Get
        'End Property

        'Public ReadOnly Property Chunks As List(Of Chunk)
        '    Get
        '        Return _chunks.Values.ToList
        '    End Get
        'End Property

        Private _typeIDs As List(Of ChunkType)
        Public ReadOnly Property TypeIDs As List(Of ChunkType)
            Get
                If _typeIDs Is Nothing Then
                    _typeIDs = New List(Of ChunkType)
                    For Each chunk As Chunk In Chunks
                        _typeIDs.Add(chunk.Type)
                    Next
                    _typeIDs = _typeIDs.Distinct.ToList
                End If
                Return _typeIDs
            End Get
        End Property

        'Added on 2016/6/16: support for different byte orders
        Private _byteOrder As ByteOrder
        Public Property ByteOrder As ByteOrder
            Get
                Return _byteOrder
            End Get
            Set(value As ByteOrder)
                _byteOrder = value
            End Set
        End Property

        Sub New()
            MyBase.New()
        End Sub

        Sub New(ByVal fileName As String)
            Me.New()
            Using fs As New FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 2048)
                Open(fs)
            End Using
        End Sub

        Sub New(ByRef fs As FileStream)
            Me.New()
            Open(fs)
        End Sub

        Private Sub Open(ByRef fs As FileStream)
            RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(0, "Loading..."))
            My.Application.DoEvents()

            FileName = fs.Name
            Dim fileExt As String _
                = GetExtension(FileName)

            Using br As New BinaryReader(fs)
                With br
                    GetAllSectorHeaders(br)

                    If UCase(fileExt) = ".AVC" Then
                        Format = FileFormat.AVC
                        ByteOrder = ByteOrder.LittleEndian
                    Else
                        Format = GetFileFormat(br)
                    End If

                    'ChunkDictionary = New Dictionary(Of Long, Chunk)
                    Chunks = New List(Of Chunk)

                    'Added on 2015/10/23: 3DO file support
                    If Format = FileFormat._3DO Then
                        _sectorHeadersChecked = True
                        _hasSectorHeaders = False
                        GetAllChunks3DO(br)
                    ElseIf Format = FileFormat.AVC Then
                        _sectorHeadersChecked = True
                        _hasSectorHeaders = False
                        GetAllChunksAVC(br)
                    ElseIf Format = FileFormat.DOSorMac Then
                        _sectorHeadersChecked = True
                        _hasSectorHeaders = False
                        GetAllChunksDOSorMac(br)
                    Else
                        GetAllChunks(br)
                    End If

                End With

                'Added on 2016/6/14: Still images from Saturn ports
                Dim chunk99Parts As IEnumerable(Of Chunk99Part) _
                    = Chunks.OfType(Of Chunk99Part)
                If chunk99Parts.Count > 0 Then
                    Dim partList As New Chunk99PartList
                    partList.AddRange(chunk99Parts)
                    Dim tileset As Chunk99 _
                        = partList.ToChunk99
                    Chunks.Insert(0, tileset)
                End If

            End Using

            RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(0, "Ready"))
            My.Application.DoEvents()
        End Sub

        Public Sub Close()
            'TODO: Add code to close the file
        End Sub

        Private Function GetFileFormat(ByRef br As BinaryReader) As FileFormat
            Dim format As FileFormat
            Dim firstbyte As Byte
            With br
                Dim originalPosition As Long _
                    = .BaseStream.Position
                .BaseStream.Seek(0, SeekOrigin.Begin)
                firstbyte = .ReadByte
                If (firstbyte And 15) = 0 Then
                    .ReadByte()
                    If .ReadUInt16 = 0 Then
                        format = FileFormat.Saturn
                    Else
                        format = FileFormat.LaterMegaCD
                    End If
                ElseIf firstbyte = &H3 Then
                    If .ReadByte() = &HD0 Then
                        format = FileFormat._3DO
                    End If
                ElseIf firstbyte = &H85 Then
                    If .ReadByte() = &HC Then
                        format = FileFormat.DOSorMac
                    End If
                Else
                    format = FileFormat.EarlyMegaCDor32X
                End If
                .BaseStream.Seek(originalPosition, SeekOrigin.Begin)
            End With

            Return format
        End Function

        Private Sub GetAllSectorHeaders(ByRef br As BinaryReader)
            Dim sectorHeaderList = New List(Of UShort)

            Dim header As UShort

            With br.BaseStream

                Dim originalPosition As Long _
                    = .Position

                'For filePos As Long = 0 To .Length - CD_ROM_MODE1_BYTES_PER_SECTOR Step CD_ROM_MODE1_BYTES_PER_SECTOR
                For filePos As Long = 0 To .Length - 1 Step CD_ROM_MODE1_BYTES_PER_SECTOR
                    .Seek(filePos, SeekOrigin.Begin)
                    header = ReadBigEndianUInt16(br)
                    sectorHeaderList.Add(header)
                Next

                .Seek(originalPosition, SeekOrigin.Begin)

            End With

            _sectorHeaders = sectorHeaderList.ToArray
        End Sub

        'NOTES (2014/6/21):
        '
        'As of this writing, there are three known file structures for SGA files:
        '    1) Single/multiple chunk with sector headers 
        '       - Seen in the earlier titles
        '       - Each sector begins with a 16-bit header
        '         - The first 8 bits of the first sector always contain a chunk type tag (e.g. &HC1, &HC6, &H81)
        '         - In subsequent sectors, if the top 4 bits of a sector != zero, that indicates a chunk type tag)
        '         - If the top 4 bits == zero, then the next 12 bits will indicate the number of remaining bytes of the current chunk in the current sector
        '           - A value of zero means that the reader should skip to the next sector
        '
        '    2) Single steam type with no sector headers
        '       - Seen in audio-only files from Night Trap and Night Trap 32X (e.g. "2BLUE.SGA")
        '       - The first 8 bits of the first sector contain a chunk type tag (e.g. &HA1)
        '       - Subsequent sectors are headerless
        '       - PROBLEM: How do we know that we're looking at data at the start of a sector rather than a sector header/bytes remaining value?
        '         - If we keep track of how many bytes are left in the current chunk, we can compare that value with the lower 12 bits of the start of the sector
        '           - If the values match what we would expect, then we know we're looking at a bytes remaining value
        '           - Otherwise, we're looking at data
        '           - 
        '         - If the top four bits of the sector == zero, and bytesLeftInChunk == 64, and the next 12 bits of the sector also == 64, then we're looking at a length marker
        '         - If the top four bits of the sector == zero, and bytesLeftInChunk => 2046 (max number of readable bytes per sector), and the next 12 bits of the sector == 2046, then we're looking at a length marker
        '
        '    3) Single/multiple chunk with sector headers and chunk interleaving
        '       - Seen in Slam City (SCD), Prize Fighter (SCD), and Supreme Warrior (SCD)
        '       - Each sector begins with a 16-bit header
        '         - The top 4 bits (always non-zero) indicate the chunk index
        '         - If the bottom 12 bits != zero, then the value indicates the remaining bytes of that chunk in the sector
        '         - If the bottom 12 bits == zero, then that indicates the start of a new chunk on that particular index
        '           - Chunks can "interrupt" other chunks in this way
        '         - PROBLEM: Some of the possible chunk indexes overlap with certain stream types (e.g. 81 (32X), A1 (audio), and possibly others)
        '           - However, we can tell these two types of files apart based on the first sector header
        '             - The bottom four bits for all known stream types are non-zero
        '             - If the top 4 bits of the first sector are non-zero, and the next 4 bits are also zero, then we know we're looking at an interleaving file and can handle it accordingly
        '

        'Private Sub GetAllChunks(ByRef br As BinaryReader)
        '    Select Case Version
        '        Case 1
        '            GetAllChunksV1(br)
        '        Case Else
        '            GetAllChunksV2(br)
        '    End Select
        'End Sub

        Private Sub GetAllChunks(ByRef br As BinaryReader)
            Dim header As UShort
            Dim trackID As Integer = 0

            Dim rawChunk(NUMBER_OF_SIMULTANEOUS_STREAMS_MAX - 1) As RawChunk
            For i As Integer = 0 To rawChunk.Length - 1
                rawChunk(i) = New RawChunk
            Next
            Dim chunkBytesLeftInSector As Integer

            While br.BaseStream.Position < br.BaseStream.Length - 2
                If HasSectorHeaders = True Then
                    AlignNextRead(br)
                End If

                Do 'Read chunk sector-by-sector

                    Select Case Format

                        Case FileFormat.EarlyMegaCDor32X

                            header = br.ReadBigEndianUInt16

                            If header = 0 Then
                                If (HasSectorHeaders = True) _
                                Or ((HasSectorHeaders = False) And (rawChunk(trackID).BytesLeft = 0)) Then
                                    br.SkipToNextCDROMSector
                                    Continue While
                                End If
                            End If

                            If rawChunk(trackID).BytesLeft = 0 Then
                                If ((header >> 15) And 1) = 1 Then 'assume start of new chunk
                                    StartRawChunk(rawChunk(trackID), br, trackID, header, chunkBytesLeftInSector)
                                End If
                            Else
                                br.BaseStream.Seek(-2, IO.SeekOrigin.Current)
                                If br.IsAtStartOfCDROMSector = True Then 'if at start of sector
                                    If HasSectorHeaders = True Then
                                        chunkBytesLeftInSector = header And 4095
                                        br.BaseStream.Seek(2, IO.SeekOrigin.Current)
                                    Else
                                        chunkBytesLeftInSector = Math.Min(rawChunk(trackID).BytesLeft, CD_ROM_MODE1_BYTES_PER_SECTOR)
                                    End If
                                End If
                            End If

                            UpdateRawChunk(rawChunk(trackID), br, chunkBytesLeftInSector)

                        Case FileFormat.LaterMegaCD, FileFormat.Saturn

                            If br.IsAtStartOfCDROMSector = True Then 'if at start of sector
                                'Added on 2015/10/11: Support for Sega Saturn
                                If Format = FileFormat.Saturn Then
                                    trackID = br.ReadBigEndianUInt16
                                End If

                                header = br.ReadBigEndianUInt16

                                'Added on 2015/10/11: Support for Sega Saturn
                                If Format = FileFormat.LaterMegaCD Then
                                    trackID = (header >> 12) And 15
                                End If

                                chunkBytesLeftInSector = header And 4095
                            End If

                            If chunkBytesLeftInSector = 0 Then ' assume start of new chunk
                                header = br.ReadBigEndianUInt16

                                If header = 0 Then
                                    br.SkipToNextCDROMSector
                                    Continue While
                                End If

                                StartRawChunk(rawChunk(trackID), br, trackID, header, chunkBytesLeftInSector)
                            End If

                            UpdateRawChunk(rawChunk(trackID), br, chunkBytesLeftInSector)

                            'Added on 2015/2/28: Special handling for F0 and F1 chunks
                            chunkBytesLeftInSector = Math.Min(rawChunk(trackID).BytesLeft, CD_ROM_MODE1_BYTES_PER_SECTOR - (br.BaseStream.Position Mod CD_ROM_MODE1_BYTES_PER_SECTOR))

                    End Select

                Loop While (rawChunk(trackID).BytesLeft > 0) And (br.BaseStream.Position < br.BaseStream.Length - 2)

                'Added on 2015/2/28: Throw out chunks of type F0
                'If rawChunk(trackID).Type = ChunkType.HeaderF0 Then
                '    Debug.WriteLine($"{[GetType]().Name}.{Reflection.MethodBase.GetCurrentMethod().Name}: Encountered chunk type F0.")
                'Else
                'Debug.WriteLine($"${CByte(rawChunk(trackID).Type):X2} ends at ${br.BaseStream.Position:X8}")
                FinishRawChunk(rawChunk(trackID), br.BaseStream.Position)
                'End If

                If Format = FileFormat.EarlyMegaCDor32X And HasSectorHeaders = True Then
                    ' Seek to start of next sector if the next sector is fewer than 12 bytes away  (i.e. the minimum length of a chunk header)
                    ' because chunk headers in early versions of the format are not observed to split across sector boundaries
                    Dim nextSectorOffset As Long _
                        = CD_ROM_MODE1_BYTES_PER_SECTOR - (br.BaseStream.Position Mod CD_ROM_MODE1_BYTES_PER_SECTOR)
                    If nextSectorOffset < 12 Then
                        br.BaseStream.Seek(nextSectorOffset, SeekOrigin.Current)
                    End If
                End If

            End While

        End Sub

        Private Sub GetAllChunks3DO(ByRef br As BinaryReader)
            Dim header As UShort
            Dim trackID As Integer = 0

            Dim rawChunk(255) As RawChunk
            For i As Integer = 0 To rawChunk.Length - 1
                rawChunk(i) = New RawChunk
            Next

            br.BaseStream.Seek(2, SeekOrigin.Begin)
            While br.BaseStream.Position < br.BaseStream.Length - 2

                header = br.ReadBigEndianUInt16
                trackID = CInt(header And 255)

                With rawChunk(trackID)
                    .Offset = br.BaseStream.Position - 2
                    .DataLocations = New SortedList(Of Long, Integer)
                    .Type = (header >> 8) And 255

                    '.TrackID = (trackID * 4096) + (header And 255)
                    .TrackID = header And 255

                    .ReportedLength = ReadBigEndianUInt16(br)
                    .Length = .ReportedLength
                    .Data = New List(Of Byte)
                    .Data.AddRange(br.ReadBytes(CInt(.Length)))

                    .BytesLeft = 0 '.Length

                    .EndOffset = br.BaseStream.Position - 1
                End With

                Dim newChunk As Chunk =
                    Chunk.GetNewChunk(rawChunk(trackID))
                Chunks.Add(newChunk)
                'If ChunkDictionary.Keys.Contains(rawChunk(trackID).Offset) = False Then
                '    ChunkDictionary.Add(rawChunk(trackID).Offset, newChunk)
                'End If

            End While

        End Sub

        Private Sub GetAllChunksAVC(ByRef br As BinaryReader)
            Dim header As UShort
            Dim trackID As Integer = 0

            Dim rawChunk(255) As RawChunk
            For i As Integer = 0 To rawChunk.Length - 1
                rawChunk(i) = New RawChunk
            Next

            br.BaseStream.Seek(0, SeekOrigin.Begin)
            While br.BaseStream.Position < br.BaseStream.Length - 2
                AlignNextRead(br)

                header = br.ReadBigEndianUInt16
                trackID = CInt(header And 255)

                With rawChunk(trackID)
                    .Offset = br.BaseStream.Position - 2
                    .DataLocations = New SortedList(Of Long, Integer)

                    .ByteOrder = ByteOrder.LittleEndian

                    .Type = (header >> 8) And 255

                    '.TrackID = (trackID * 4096) + (header And 255)
                    .TrackID = header And 255

                    .ReportedLength = br.ReadInt16
                    .Length = .ReportedLength
                    .Data = New List(Of Byte)
                    .Data.AddRange(br.ReadBytes(CInt(.Length)))

                    .BytesLeft = 0 '.Length

                    .EndOffset = br.BaseStream.Position - 1
                End With

                Dim newChunk As Chunk =
                    Chunk.GetNewChunk(rawChunk(trackID))
                Chunks.Add(newChunk)

            End While

        End Sub

        Private Sub GetAllChunksDOSorMac(ByRef br As BinaryReader)
            br.BaseStream.Seek(0, SeekOrigin.Begin)

            While br.BaseStream.Position < br.BaseStream.Length - 2

                Dim fileType As Byte _
                    = br.ReadByte
                Dim videoRate As Byte _
                    = br.ReadByte
                Dim sizeInTiles As New Size(
                    br.ReadByte,
                    br.ReadByte
                    )
                Dim unknown1 As UShort _
                    = br.ReadBigEndianUInt16
                Dim unknown2 As UShort _
                    = br.ReadBigEndianUInt16
                Dim lengthInFrames As UInteger _
                    = br.ReadBigEndianUInt32

                Dim initialPalette(255) As UShort
                For i As Integer = 0 To 255
                    'initialPalette(i) = br.ReadBigEndianUInt16
                    initialPalette(i) = br.ReadUInt16
                Next

                Dim audioDataLength As UShort _
                = br.ReadBigEndianUInt16

                Dim rawChunk As New RawChunk

                Dim lastTimecode As New SMPTETimecode(0, videoRate)

                For frameNumber As Integer = 0 To lengthInFrames - 1

                    Dim timecode As UInteger _
                        = br.ReadBigEndianUInt32

                    Dim thisTimecode As New SMPTETimecode(timecode, 30)

                    If thisTimecode.FrameNumber < lastTimecode.FrameNumber _
                    Or Math.Abs(lastTimecode.FrameNumber - thisTimecode.FrameNumber) > 3 Then
                        Dim s As String _
                            = $"{lastTimecode.FrameNumber} {thisTimecode.FrameNumber}"
                        If Chunks.Count > 0 Then
                            Exit While
                        End If
                    End If

                    Dim paletteEntryCount As UShort _
                        = br.ReadBigEndianUInt16
                    Dim patternDataOffset As UShort _
                        = br.ReadBigEndianUInt16
                    Dim dataLength As UShort _
                        = br.ReadBigEndianUInt16
                    Dim paletteEntryWriteOffset As UShort _
                        = br.ReadBigEndianUInt16

                    Dim trackID As Integer = thisTimecode.Hours

                    'Get audio data
                    rawChunk = New RawChunk
                    With rawChunk
                        .Offset = br.BaseStream.Position

                        .Type = ChunkType.AudioA3

                        .ByteOrder = ByteOrder.BigEndian
                        .TrackID = trackID

                        .ReportedLength = audioDataLength
                        .Length = .ReportedLength + 8

                        .DataLocations = New SortedList(Of Long, Integer)
                        .DataLocations.Add(br.BaseStream.Position, .Length)

                        .Data = New List(Of Byte)

                        'Add timecode
                        Dim timecodeBytes As Byte() _
                        = BitConverter.GetBytes(timecode)
                        Array.Reverse(timecodeBytes)
                        .Data.AddRange(timecodeBytes)

                        'Add sample rate (18.9KHz)
                        .Data.Add(&H49)
                        .Data.Add(&HD4)

                        'Add number of channels
                        .Data.Add(1)
                        .Data.Add(0)

                        .Data.AddRange(br.ReadBytes(audioDataLength - 1))
                        br.BaseStream.Seek(1, SeekOrigin.Current)

                        .BytesLeft = 0 '.Length
                        .EndOffset = br.BaseStream.Position - 1
                    End With

                    Dim newAudioChunk As Chunk =
                    Chunk.GetNewChunk(rawChunk)

                    'Get video data
                    rawChunk = New RawChunk
                    With rawChunk
                        .Offset = br.BaseStream.Position

                        .Type = ChunkType.Video82

                        .ByteOrder = ByteOrder.BigEndian
                        .TrackID = trackID

                        .ReportedLength = dataLength - audioDataLength
                        .Length = .ReportedLength + 8

                        .DataLocations = New SortedList(Of Long, Integer)
                        .DataLocations.Add(br.BaseStream.Position, .Length)

                        .Data = New List(Of Byte)

                        'Add timecode
                        Dim timecodeBytes As Byte() _
                        = BitConverter.GetBytes(timecode)
                        Array.Reverse(timecodeBytes)
                        .Data.AddRange(timecodeBytes)

                        If Chunks.Count = 0 Then
                            paletteEntryCount = 255
                            patternDataOffset += 510
                        End If

                        'Add palette data offset and length
                        .Data.Add(1)
                        .Data.Add(paletteEntryCount)

                        'Add size in tiles
                        .Data.Add(sizeInTiles.Width)
                        .Data.Add(sizeInTiles.Height)

                        'Add color data offset
                        Dim patternDataOffsetBytes As Byte() _
                            = BitConverter.GetBytes(patternDataOffset - audioDataLength)
                        Array.Reverse(patternDataOffsetBytes)
                        .Data.AddRange(patternDataOffsetBytes)

                        'Add palette data write data offset
                        If Chunks.Count = 0 Then
                            .Data.AddRange({0, 0})
                        Else
                            Dim paletteEntryWriteOffsetBytes As Byte() _
                            = BitConverter.GetBytes(paletteEntryWriteOffset)
                            Array.Reverse(paletteEntryWriteOffsetBytes)
                            .Data.AddRange(paletteEntryWriteOffsetBytes)
                        End If

                        If Chunks.Count = 0 Then
                            For i As Integer = 0 To 254
                                .Data.AddRange(BitConverter.GetBytes(initialPalette(i)))
                            Next
                        End If

                        .Data.AddRange(br.ReadBytes(.ReportedLength))
                        .BytesLeft = 0 '.Length
                        .EndOffset = br.BaseStream.Position - 1
                    End With

                    Dim newVideoChunk As Chunk =
                    Chunk.GetNewChunk(rawChunk)

                    Chunks.Add(newVideoChunk)
                    Chunks.Add(newAudioChunk)


                    lastTimecode = New SMPTETimecode(timecode, 30)

                Next

            End While

        End Sub

        Private Sub AlignNextRead(ByRef br As BinaryReader)
            Select Case Format
                Case FileFormat.EarlyMegaCDor32X, FileFormat.LaterMegaCD, FileFormat.AVC
                    br.AlignNextRead(2)
                Case FileFormat.Saturn
                    br.AlignNextRead(4)
            End Select

            'Select Case Version
            '    Case 0, 1, 2
            '        br.AlignNextRead(2)
            '    Case 3
            '        br.AlignNextRead(4)
            'End Select
        End Sub

        Private Sub StartRawChunk(ByRef rawChunk As RawChunk, ByRef br As BinaryReader, ByVal trackID As Integer, ByVal header As UShort, ByRef chunkBytesLeftInSector As Integer)
            With rawChunk
                .Offset = br.BaseStream.Position - 2

                .ByteOrder = ByteOrder

                .Type = (header >> 8) And 255

                .TrackID = trackID
                '.TrackID = (trackID * 4096)
                'Added on 2015/10/11: Support for Sega Saturn
                If Format <> FileFormat.Saturn Then
                    .TrackID = .TrackID Or (header And 255)
                    '.TrackID += (header And 255)
                End If

                .ReportedLength = ReadBigEndianUInt16(br)
                .Length = .ReportedLength

                .DataLocations = New SortedList(Of Long, Integer)
                .DataLocations.Add(.Offset, Math.Min(.ReportedLength, 4))

                .Data = New List(Of Byte)(.ReportedLength)
                .BytesLeft = .Length
                chunkBytesLeftInSector = Math.Min(.BytesLeft, CD_ROM_MODE1_BYTES_PER_SECTOR - (br.BaseStream.Position Mod CD_ROM_MODE1_BYTES_PER_SECTOR))
            End With
        End Sub

        Private Sub UpdateRawChunk(ByRef rawChunk As RawChunk, ByRef br As BinaryReader, ByVal chunkBytesLeftInSector As Integer)
            With rawChunk
                Dim chunkByteReadCount As Integer _
                    = Math.Min(.BytesLeft, chunkBytesLeftInSector)

                .DataLocations.Add(br.BaseStream.Position, chunkByteReadCount)

                .Data.AddRange(br.ReadBytes(chunkByteReadCount))
                .BytesLeft -= chunkByteReadCount
            End With
        End Sub

        Private Sub FinishRawChunk(ByRef rawChunk As RawChunk, ByVal position As Long)
            rawChunk.EndOffset = position - 1
            Dim newChunk As Chunk =
                Chunk.GetNewChunk(rawChunk)
            Chunks.Add(newChunk)
            'If ChunkDictionary.Keys.Contains(rawChunk.Offset) = False Then
            '    ChunkDictionary.Add(rawChunk.Offset, newChunk)
            'End If
        End Sub

    End Class

End Namespace


