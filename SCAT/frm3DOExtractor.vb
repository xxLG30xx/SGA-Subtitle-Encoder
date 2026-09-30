Imports System.IO

Public Class frm3DOExtractor
    Dim _myText As String _
    = $"{My.Application.Info.Title} 3DO Sequence Extractor"

    'Dim _inputFilePath, _outputFilePath As String
    'Dim _lastInputDir, _lastOutputDir As String
    'Dim _fileID As String

    Dim _chunkPositions As Dictionary(Of Long, ChunkMetadata)
    Dim _splitPositions As SortedSet(Of Long)

    Dim _sequenceTreeNode As New TreeNode
    'Dim _sbOutput As System.Text.StringBuilder

    'Private Sub frm3DOExtractor_Load(sender As Object, e As EventArgs) Handles Me.Load
    '    InitializeHashTable()
    'End Sub

    Protected Overrides Sub InitializeHashTable()
        MyBase.InitializeHashTable()
        With _hashTable
            .Add("21CF0F389BD86135FBC6E6424D2CD2574EF2C3DD2475F267FE1A2A0A2818FE2A", "Night Trap Disc 1")
            .Add("57EFD1FE1A3B413F53816BCEEA82096DA76DBA57AC676618AB8B7213CDD01F1D", "Night Trap Disc 2")
            .Add("CC8A781DBC92D9907AFD004727E4ED222C562B4D92EAFE6E0BF0484797BCE490", "Sewer Shark")
            .Add("C31072D6C9CB9767B176DB39F11CFE5A4BD6D8DA173276E1C5910ADE3EB22A9C", "Corpse Killer")
            .Add("8B90EDA8DAD04DE050E02F3A8B684F0DFD050EDB033F3E5E69917BBF03270BE2", "Supreme Warrior Disc 1")
            .Add("FFF42D84FD94713ED8E77DA2D11FD1A3F458CB98B4AD5C19EB428EA9EDFF881C", "Supreme Warrior Disc 2")
        End With
    End Sub

    Private Sub frm3DOExtractor_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Text = $"{_myText}"

        Dim result As DialogResult =
            GetInputFilePath("*.bin", "Disc Data files|discdata.bin;disc1data.bin;disc2data.bin;filelist.bin")

        If result = DialogResult.OK Then
            Using fs As New FileStream(_inputFilePath, FileMode.Open, FileAccess.Read)
                lblStatus.Text = "Identifying file..."
                Application.DoEvents()
                _fileID = GetFileID(fs)
                lblStatus.Text = $"Data is from {(_hashTable(_fileID))}"
                Text = $"{_hashTable(_fileID)} ({Path.GetFileName(_inputFilePath)}) - {_myText}"

                With tvwItems
                    .Nodes.Clear()
                    .Nodes.Add(New TreeNode(_hashTable(_fileID)))
                End With

                lblStatus.Text = "Locating media chunks..."
                Application.DoEvents()
                _chunkPositions = GetChunkPositions(fs)

                lblStatus.Text = "Locating sequence boundaries..."
                Application.DoEvents()
                _splitPositions = GetSplitPositions(fs)

                lblStatus.Text = "Ready"

            End Using

            btnExtract.Enabled = True
        Else
            DialogResult = DialogResult.Cancel
            Close()
        End If
    End Sub

    'Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
    '    Dim result As DialogResult =
    '        GetInputFilePath()
    '    If result = DialogResult.OK Then
    '        Using fs As New FileStream(_inputFilePath, FileMode.Open, FileAccess.Read)
    '            tbxOutput.Clear()
    '            lblStatus.Text = "Identifying file..."
    '            Application.DoEvents()
    '            _fileID = GetFileID(fs)
    '            Text = $"{_hashTable(_fileID)} - {My.Application.Info.ProductName}"

    '            _sbOutput = New Text.StringBuilder
    '            With _sbOutput
    '                .AppendLine(_hashTable(_fileID))
    '            End With

    '            lblStatus.Text = "Locating media chunks..."
    '            Application.DoEvents()
    '            _chunkPositions = GetChunkPositions(fs)

    '            lblStatus.Text = "Locating sequence boundaries..."
    '            Application.DoEvents()
    '            _splitPositions = GetSplitPositions(fs)

    '            lblStatus.Text = "Ready"

    '            tbxOutput.Text = _sbOutput.ToString
    '        End Using

    '        btnExtract.Enabled = True
    '    End If

    'End Sub

    'Private Sub btnExtract_Click(sender As Object, e As EventArgs) Handles btnExtract.Click
    '    btnExtract.Enabled = False

    '    Using fs As New FileStream(_inputFilePath, FileMode.Open, FileAccess.Read)
    '        ExtractFiles(fs)
    '    End Using

    '    btnExtract.Enabled = True
    'End Sub

    'Private Function GetInputFilePath() As DialogResult
    '    Dim result As DialogResult

    '    Using ofdOpen As New OpenFileDialog

    '        With ofdOpen
    '            .AutoUpgradeEnabled = True
    '            .CheckFileExists = True
    '            .CheckPathExists = True
    '            .DefaultExt = "*.bin"
    '            .Filter = "Disc Data files|discdata.bin;disc1data.bin;disc2data.bin;filelist.bin"
    '            .FilterIndex = 0
    '            .Multiselect = False

    '            If _lastInputDir <> "" Then
    '                .InitialDirectory = _lastInputDir
    '            End If

    '            result = .ShowDialog()

    '            If result = System.Windows.Forms.DialogResult.OK Then
    '                _inputFilePath = .FileName
    '                _lastInputDir = Path.GetDirectoryName(_inputFilePath)
    '            End If

    '        End With

    '    End Using

    '    Return result

    'End Function

    Private Class ChunkMetadata

        Protected _type As Byte
        Public ReadOnly Property Type As Byte
            Get
                Return _type
            End Get
        End Property

        Protected _trackID As Byte
        Public ReadOnly Property TrackID As Byte
            Get
                Return _trackID
            End Get
        End Property

        Protected _length As UShort
        Public ReadOnly Property Length As UShort
            Get
                Return _length
            End Get
        End Property

        Protected _flag As Byte
        Public ReadOnly Property Flag As Byte
            Get
                Return _flag
            End Get
        End Property

        Protected _timeCode As Integer
        Public ReadOnly Property Timecode As UInteger
            Get
                Return _timeCode
            End Get
        End Property

        Protected _other As UInteger
        Public ReadOnly Property Other As UInteger
            Get
                Return _other
            End Get
        End Property

        Sub New(ByVal chunkType As Byte, ByVal trackID As Byte, ByVal length As UShort, ByVal timeCode As UInteger, ByVal other As UInteger)
            _type = chunkType
            _trackID = trackID
            _length = length
            _flag = CByte(timeCode >> 24)
            _timeCode = timeCode And &HFFFFFF
            _other = other
        End Sub

        Shared Function GetFrameCount(ByVal timeCode As UInteger)
            Return (((timeCode And &HFF0000) >> 16) * 1800) + (((timeCode And &HFF00) >> 8) * 30) + (timeCode And &HFF)
        End Function

    End Class

    Private Class VideoChunkMetadata
        Inherits ChunkMetadata

        Public ReadOnly Property PaletteOffset As Byte
            Get
                Return GetPaletteOffset(_other)
            End Get
        End Property
        Shared Function GetPaletteOffset(ByVal other As UInteger) As Byte
            Return CByte(other >> 24)
        End Function

        Public ReadOnly Property PaletteEntryCount As Byte
            Get
                Return GetPaletteEntryCount(_other)
            End Get
        End Property
        Shared Function GetPaletteEntryCount(ByVal other As UInteger) As Byte
            Return CByte((other >> 16) And 255)
        End Function

        Public ReadOnly Property FrameSizeInPixels As Size
            Get
                Return GetFrameSizeInPixels(_other)
            End Get
        End Property
        Shared Function GetFrameSizeInPixels(ByVal other As UInteger) As Size
            Return New Size(CInt((other >> 8) And 255) * 8, CInt(other And 255) * 8)
        End Function

        Sub New(ByVal chunkType As Byte, ByVal trackID As Byte, ByVal length As UShort, ByVal timeCode As UInteger, ByVal other As ULong)
            MyBase.New(chunkType, trackID, length, timeCode, other)
        End Sub

    End Class

    Private Class AudioChunkMetadata
        Inherits ChunkMetadata

        Public ReadOnly Property SampleRate As Integer
            Get
                Return GetSampleRate(_other)
            End Get
        End Property
        Shared Function GetSampleRate(ByVal other As UInteger) As Integer
            Dim sampleRate As Integer _
                = other >> 16
            Return sampleRate
        End Function

        Public ReadOnly Property FrameRate As Integer
            Get
                Return GetFrameRate(_other, Length)
            End Get
        End Property
        Shared Function GetFrameRate(ByVal other As UInteger, ByVal length As UShort) As Double
            Dim frameRate As Double _
                = GetSampleRate(other) / (length - 8)
            If Double.IsInfinity(frameRate) Then Return 0
            Return frameRate
        End Function

        Sub New(ByVal chunkType As Byte, ByVal trackID As Byte, ByVal length As UShort, ByVal timeCode As UInteger, ByVal other As ULong)
            MyBase.New(chunkType, trackID, length, timeCode, other)
        End Sub

    End Class

    Private Function GetChunkPositions(ByRef fs As FileStream) As Dictionary(Of Long, ChunkMetadata)
        If _hashTable.Keys.Contains(_fileID) = False Then
            Return Nothing
        End If

        Dim chunkPositions = New Dictionary(Of Long, ChunkMetadata)

        Dim chunkType As Byte
        Dim trackID As Byte
        Dim length As UShort
        Dim flag As Byte
        Dim timecode As UInteger
        Dim other As UInteger
        Dim paletteOffset As Integer
        Dim sampleRate As Integer
        Dim frameRate As Double
        Dim roundedFrameRate As Integer

        Dim isChunkTypeValid As Boolean
        Dim isTrackIDValid As Boolean
        Dim isLengthValid As Boolean
        Dim isFlagValid As Boolean
        Dim isTimecodeValid As Boolean
        Dim isPaletteOffsetValid As Boolean
        Dim isFrameSizeValid As Boolean
        Dim isSampleRateValid As Boolean
        Dim isFrameRateValid As Boolean

        Dim isVideo As Boolean = False
        Dim isAudio As Boolean = False

        Dim videoTrackIDMax As Byte = 255
        Dim audioTrackIDMax As Byte = 255
        Dim lengthMin As UShort = 8
        Dim flagMax As Byte = 255

        Debug.WriteLine($"Start of debug output for {Me.Name}.{Reflection.MethodBase.GetCurrentMethod().Name}")
        Debug.Indent()

        Select Case _hashTable(_fileID)
            Case "Night Trap Disc 1", "Night Trap Disc 2"

            Case "Sewer Shark"
                flagMax = 1

            Case "Corpse Killer"
                videoTrackIDMax = 0
                audioTrackIDMax = 0

            Case "Supreme Warrior Disc 1", "Supreme Warrior Disc 2"

        End Select

        Debug.WriteLine($"Data is from {_hashTable(_fileID)}")

        Dim br As New BinaryReader(fs)
        br.BaseStream.Seek(0, SeekOrigin.Begin)

        With pbrStatus
            .Minimum = 0
            .Maximum = br.BaseStream.Length
            .Value = 0
        End With

        While br.BaseStream.Position < br.BaseStream.Length - 12

            Do While br.BaseStream.Position < br.BaseStream.Length - 12

                pbrStatus.Value = br.BaseStream.Position
                Application.DoEvents()

                ' Check chunk type
                chunkType = br.ReadByte

                If chunkType = &H81 Then
                    isVideo = True
                    isAudio = False
                ElseIf chunkType = &HA2 Then
                    isVideo = False
                    isAudio = True
                Else
                    isVideo = False
                    isAudio = False
                End If

                If (isVideo = True) Or (isAudio = True) Then
                    isChunkTypeValid = True
                Else
                    isChunkTypeValid = False
                    Exit Do
                End If

                ' Check track ID
                trackID = br.ReadByte

                If (isVideo And (trackID <= videoTrackIDMax)) _
                Or (isAudio And (trackID <= audioTrackIDMax)) Then
                    isTrackIDValid = True
                Else
                    isTrackIDValid = False
                    Exit Do
                End If

                ' Check length
                length = br.ReadBigEndianUInt16

                If length >= lengthMin Then
                    isLengthValid = True
                Else
                    isLengthValid = False
                    br.BaseStream.Seek(-2, SeekOrigin.Current)
                    Exit Do
                End If

                ' Check timecode
                timecode = br.ReadBigEndianUInt32

                flag = CByte(timecode >> 24)

                If flag <= flagMax Then
                    isFlagValid = True
                Else
                    isFlagValid = False
                    br.BaseStream.Seek(-4, SeekOrigin.Current)
                    Exit Do
                End If

                If ((timecode And 255) <= 30) _
                And (((timecode >> 8) And 255) <= 60) Then
                    isTimecodeValid = True
                Else
                    isTimecodeValid = False
                    br.BaseStream.Seek(-4, SeekOrigin.Current)
                    Exit Do
                End If

                ' Check other metadata
                other = br.ReadBigEndianUInt32

                If isVideo Then
                    paletteOffset = other >> 24
                    Select Case paletteOffset
                        Case 0, 1 'Valid
                            isPaletteOffsetValid = True
                        Case Else
                            isPaletteOffsetValid = False
                            br.BaseStream.Seek(-4, SeekOrigin.Current)
                            Exit Do
                    End Select

                ElseIf isAudio Then
                    sampleRate = AudioChunkMetadata.GetSampleRate(other)
                    Select Case sampleRate
                        Case 18900, 22050 'Valid
                            isSampleRateValid = True
                        Case Else 'NOT OK
                            isSampleRateValid = False
                            br.BaseStream.Seek(-4, SeekOrigin.Current)
                            Exit Do
                    End Select

                    'frameRate = AudioChunkMetadata.GetVideoFrameRate(other, length)
                    'roundedFrameRate = Math.Round(frameRate)
                    'Select Case roundedFrameRate
                    '    Case 6, 12, 15 'OK
                    '        isFrameRateValid = True
                    '    Case Else 'NOT OK
                    '        isFrameRateValid = False
                    '        br.BaseStream.Seek(-4, SeekOrigin.Current)
                    '        Exit Do
                    'End Select

                End If

                Dim metadata As New ChunkMetadata(chunkType, trackID, length, timecode, other)
                chunkPositions.Add(br.BaseStream.Position - 12, metadata)
                br.BaseStream.Seek(length - 8, SeekOrigin.Current)

            Loop

            If isChunkTypeValid = False Then
                'Debug.WriteLine($"Invalid chunk type of {chunkType:X2} at {br.BaseStream.Position:X8}.")
            End If

            If isTrackIDValid = False Then
                'Debug.WriteLine($"Invalid {If(isVideo, "video", "audio")} track ID value of {If(isVideo, videoTrackIDMax, audioTrackIDMax):X2} at {br.BaseStream.Position:X8}.")
            End If

            If isLengthValid = False Then
                'Debug.WriteLine($"Invalid chunk length of {length:X4} at {br.BaseStream.Position:X8}.")
                br.BaseStream.Seek(1, SeekOrigin.Current)
            End If

            If isFlagValid = False Then
                'Debug.WriteLine($"Invalid flag value of {flag:X2} at {br.BaseStream.Position:X8}.")
                br.BaseStream.Seek(1, SeekOrigin.Current)
            End If

            If isTimecodeValid = False Then
                'Debug.WriteLine($"Invalid time code of {timecode:X6} at {br.BaseStream.Position:X8}.")
                br.BaseStream.Seek(1, SeekOrigin.Current)
            End If

            If isPaletteOffsetValid = False Then
                'Debug.WriteLine($"Invalid palette offset of {paletteOffset} at ${br.BaseStream.Position:X8}.")
                br.BaseStream.Seek(1, SeekOrigin.Current)
            End If

            If isSampleRateValid = False Then
                'Debug.WriteLine($"Invalid sample rate of {sampleRate} Hz at ${br.BaseStream.Position:X8}.")
                br.BaseStream.Seek(1, SeekOrigin.Current)
            End If

            'If isFrameRateValid = False Then
            '    Debug.WriteLine($"Encountered invalid framerate of {roundedFrameRate} fps at ${br.BaseStream.Position:X8}.")
            '    br.BaseStream.Seek(1, SeekOrigin.Current)
            'End If

            If (br.BaseStream.Position Mod 512) <> 0 Then
                br.BaseStream.Seek(512 - (br.BaseStream.Position Mod 512), SeekOrigin.Current)
            End If

        End While

        Debug.Unindent()
        Debug.WriteLine($"End of debug output for {Me.Name}.{Reflection.MethodBase.GetCurrentMethod().Name}")

        pbrStatus.Value = 0

        Return chunkPositions
    End Function

    Private Function GetSplitPositions(ByRef fs As FileStream) As SortedSet(Of Long)
        If _hashTable.Keys.Contains(_fileID) = False Then
            Return Nothing
        End If

        With pbrStatus
            .Minimum = 0
            .Maximum = _chunkPositions.Keys.Last
            .Value = 0
        End With

        Dim splitPositions As New SortedSet(Of Long)

        If _hashTable(_fileID) = "Sewer Shark" Then

            splitPositions = GetSplitPositionsSewerShark()

        Else

            Dim splitHere As Boolean = True

            'Dim prevPosition, positionDelta As Long
            Dim prevVideoTrackID, prevAudioTrackID As Byte
            Dim prevVideoFlag, prevAudioFlag As Byte
            Dim prevVideoTimecode, prevAudioTimecode As UInteger
            Dim prevVideoFrameCount, prevAudioFrameCount As Integer

            Dim videoFrameDelta, audioFrameDelta As Integer

            Dim frameDeltaMax As Integer
            Select Case _hashTable(_fileID)
                Case "Night Trap Disc 1", "Night Trap Disc 2", "Supreme Warrior Disc 1", "Supreme Warrior Disc 2"
                    frameDeltaMax = 3
                Case "Corpse Killer"
                    frameDeltaMax = 10
            End Select

            'Dim prevSampleRate, prevFrameRate As Integer

            For Each position As Long In _chunkPositions.Keys
                pbrStatus.Value = position
                Application.DoEvents()

                Dim metadata As ChunkMetadata _
                = _chunkPositions(position)

                With metadata

                    Dim prevTrackID As Byte
                    Dim prevFlag As Byte
                    Dim prevTimecode As UInteger
                    Dim prevFrameCount As Integer

                    Dim frameCount As Integer _
                    = (((.Timecode And &HFF0000) >> 16) * 1800) + (((.Timecode And &HFF00) >> 8) * 30) + (.Timecode And &HFF)

                    If .Type = &H81 Then
                        prevTrackID = prevVideoTrackID
                        prevFlag = prevVideoFlag
                        prevTimecode = prevVideoTimecode
                        prevFrameCount = prevVideoFrameCount

                        videoFrameDelta = CInt(frameCount) - CInt(prevVideoFrameCount)
                    ElseIf .Type = &HA2 Then
                        prevTrackID = prevAudioTrackID
                        prevFlag = prevAudioFlag
                        prevTimecode = prevAudioTimecode
                        prevFrameCount = prevAudioFrameCount

                        audioFrameDelta = CInt(frameCount) - CInt(prevAudioFrameCount)
                    End If

                    If (position = _chunkPositions.Keys.First) Then
                        splitHere = True
                    End If

                    If .Type = &H81 Then

                        Select Case _hashTable(_fileID)
                            Case "Night Trap Disc 1", "Night Trap Disc 2", "Corpse Killer"
                                If (videoFrameDelta > frameDeltaMax) _
                                Or (videoFrameDelta < 0) Then
                                    splitHere = True
                                End If
                            Case "Supreme Warrior Disc 1", "Supreme Warrior Disc 2"
                                If .TrackID = 0 And .Timecode = 0 Then
                                    splitHere = True
                                End If
                        End Select

                        Select Case _hashTable(_fileID)
                            Case "Night Trap Disc 1", "Night Trap Disc 2"

                                If (.TrackID <> prevTrackID) _
                            Or (.Flag <> prevFlag) _
                            Or ((.Timecode <> 0) And (videoFrameDelta = 0)) Then
                                    splitHere = True
                                End If

                            Case "Corpse Killer"

                            Case "Supreme Warrior Disc 1", "Supreme Warrior Disc 2"

                        End Select

                        prevVideoTrackID = .TrackID
                        prevVideoFlag = .Flag
                        prevVideoTimecode = .Timecode
                        prevVideoFrameCount = frameCount

                    ElseIf .Type = &HA2 Then

                        'Select Case _hashTable(_fileID)
                        '    Case "Night Trap Disc 1", "Night Trap Disc 2"

                        '    'Case "Sewer Shark"

                        '    Case "Corpse Killer"
                        '        'TODO: Add checking for switches in frame rate

                        '    Case "Supreme Warrior Disc 1", "Supreme Warrior Disc 2"

                        'End Select

                        prevAudioTrackID = .TrackID
                        prevAudioFlag = .Flag
                        prevAudioTimecode = .Timecode
                        prevAudioFrameCount = frameCount

                    End If

                End With

                HandleSplit(position, splitPositions, splitHere)

                AddNodeToSequenceNode(metadata, position)

            Next

        End If

        lblStatus.Text = $"{_chunkPositions.Count} chunks / {splitPositions.Count} sequences found"
        pbrStatus.Value = 0

        Return splitPositions
    End Function

    'Private Function GetFrameCount(ByVal timecode As UInteger) As Integer
    '    Dim frameCount As Integer _
    '        = CInt((((timecode And &HFF0000) >> 16) * 1800) + (((timecode And &HFF00) >> 8) * 30) + (timecode And &HFF))
    '    Return frameCount
    'End Function

    Private Function GetSplitPositionsSewerShark() As SortedSet(Of Long)
        Dim splitPositions As New SortedSet(Of Long)
        Dim splitHere As Boolean = True

        'Dim prevPosition, positionDelta As Long
        Dim prevFlag As Byte
        Dim prevTimecode As UInteger
        Dim prevFrameNumber As Integer

        Dim frameDelta As Integer
        Dim frameDeltaMax As Integer = 2

        For Each position As Long In _chunkPositions.Keys
            pbrStatus.Value = position
            Application.DoEvents()

            If (position = _chunkPositions.Keys.First) Then
                splitHere = True
            End If

            Dim metadata As ChunkMetadata _
                = _chunkPositions(position)

            With metadata
                Dim frameNumber As Integer _
                    = New SMPTETimecode(.Timecode, 30).FrameNumber

                frameDelta = frameNumber - prevFrameNumber

                If (frameDelta > frameDeltaMax) _
                Or (frameDelta < 0) Then
                    splitHere = True
                End If

                prevFlag = .Flag
                prevTimecode = .Timecode
                prevFrameNumber = frameNumber
            End With

            HandleSplit(position, splitPositions, splitHere)

            AddNodeToSequenceNode(metadata, position)

        Next

        Return splitPositions
    End Function

    Private Sub HandleSplit(ByVal position As Long, ByRef splitPositions As SortedSet(Of Long), ByRef splitHere As Boolean)
        If splitHere = True Then
            If splitPositions.Contains(position) = False Then
                splitPositions.Add(position)
            End If

            AddNodeToTreeView()

            splitHere = False
        End If
    End Sub

    Private Sub AddNodeToTreeView()
        With _sequenceTreeNode.Nodes
            If .Count > 1 Then
                _sequenceTreeNode.Text = $"Sequence ({ .Count} chunks)"
                tvwItems.Nodes(0).Nodes.Add(_sequenceTreeNode.Clone)
                .Clear()
            End If
        End With
    End Sub

    Private Sub AddNodeToSequenceNode(ByVal metadata As ChunkMetadata, ByVal position As Long)
        With metadata
            _sequenceTreeNode.Nodes.Add(New TreeNode($"${position:X8} {If(.Type = &H81, "Video", "Audio")} ${ .Type:X2} trak={ .TrackID:X2} len={ .Length:X4} time={ .Flag:X2}{ .Timecode:X8} meta={ .Other:X8}"))
        End With
    End Sub

    Protected Overrides Function ExtractFiles(ByRef fsIn As FileStream) As DialogResult
        Dim result As DialogResult

        Using sfdSave As New FolderBrowserDialog

            With sfdSave
                If _lastOutputDir <> "" Then
                    .SelectedPath = _lastOutputDir
                End If
                .Description = $"Choose the folder to which you want to extract the sequences from {_hashTable(_fileID)}."
                .ShowNewFolderButton = True

                result = .ShowDialog()

                If result = System.Windows.Forms.DialogResult.OK Then
                    _outputFilePath = .SelectedPath
                    _lastOutputDir = Path.GetDirectoryName(_outputFilePath)

                    Dim header As UShort
                    Dim length As UShort

                    Dim sequenceIndex As Integer
                    Dim minutes, seconds, frames As Integer
                    Dim data As Byte()

                    Dim brIn As New BinaryReader(fsIn)

                    Dim fileNameOut As String

                    Dim fsOut As FileStream = Nothing
                    Dim bwOut As BinaryWriter = Nothing

                    With pbrStatus
                        .Minimum = 0
                        .Maximum = _chunkPositions.Keys.Last
                        .Value = 0
                    End With

                    lblStatus.Text = "Extracting SGA files..."

                    For Each position As Long In _chunkPositions.Keys
                        pbrStatus.Value = position
                        Application.DoEvents()

                        If _splitPositions.Contains(position) Then
                            Dim info As ChunkMetadata _
                                = _chunkPositions(position)

                            Select Case _hashTable(_fileID)
                                'Case "Night Trap Disc 1", "Night Trap Disc 2", "Corpse Killer"
                                Case "Supreme Warrior Disc 1", "Supreme Warrior Disc 2"
                                    fileNameOut = $"{ .SelectedPath}\{sequenceIndex:0000}.SGA"
                                    sequenceIndex += 1
                                Case Else
                                    minutes = info.Timecode >> 16
                                    seconds = (info.Timecode >> 8) And 255
                                    frames = info.Timecode And 255

                                    fileNameOut = $"{ .SelectedPath}\{minutes:00}{seconds:00}{frames:00}{info.TrackID:0}{info.Flag:0}.SGA"

                                    Dim altFileIndex As Integer = 0
                                    While My.Computer.FileSystem.FileExists(fileNameOut) = True
                                        altFileIndex += 1
                                        fileNameOut = fileNameOut.Remove(fileNameOut.Length - If(altFileIndex > 1, 6, 4)) & $"_{altFileIndex:0}.SGA"
                                    End While
                            End Select

                            If bwOut IsNot Nothing Then
                                bwOut.Close()
                            End If

                            fsOut = New FileStream(fileNameOut, FileMode.Create)
                            bwOut = New BinaryWriter(fsOut)

                            'Added on 2015/10/23: Mark SGA file as 3DO format
                            bwOut.Write(CByte(&H3))
                            bwOut.Write(CByte(&HD0))

                        End If

                        brIn.BaseStream.Seek(position, SeekOrigin.Begin)

                        header = brIn.ReadBigEndianUInt16
                        length = brIn.ReadBigEndianUInt16
                        data = brIn.ReadBytes(CInt(length))

                        With bwOut
                            bwOut.WriteBigEndianUInt16(header)
                            bwOut.WriteBigEndianUInt16(length)
                            .Write(data)
                        End With

                    Next

                    If bwOut IsNot Nothing Then
                        bwOut.Close()
                    End If

                End If

            End With

        End Using

        With pbrStatus
            .Value = 0
        End With

        lblStatus.Text = "Ready"

        Return result

    End Function

End Class
