Imports System.IO
Imports SCAT.SGA

Public Class frmDOSExtractor
    Inherits frmExtractor

    Dim _myText As String _
        = $"{My.Application.Info.Title} NTMOVIE Sequence Extractor"

    Private Sub frmDOSExtractor_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Text = $"{_myText}"

        Dim result As DialogResult =
            GetInputFilePath("", "NTMOVIE files|NTMOVIE")

        If result = DialogResult.OK Then
            Using fs As New FileStream(_inputFilePath, FileMode.Open, FileAccess.Read)
                'lblStatus.Text = "Identifying file..."
                'Application.DoEvents()
                '_fileID = GetFileID(fs)
                'lblStatus.Text = $"Data is from {(_hashTable(_fileID))}"
                'Text = $"{_hashTable(_fileID)} ({Path.GetFileName(_inputFilePath)}) - {_myText}"

                With tvwItems
                    .Nodes.Clear()
                    '.Nodes.Add(New TreeNode(_hashTable(_fileID)))
                End With

                lblStatus.Text = "Extracting files..."
                Application.DoEvents()
                ExtractFiles(fs)

                'lblStatus.Text = "Locating media chunks..."
                'Application.DoEvents()
                '_chunkPositions = GetChunkPositions(fs)

                'lblStatus.Text = "Locating sequence boundaries..."
                'Application.DoEvents()
                '_splitPositions = GetSplitPositions(fs)

                lblStatus.Text = "Ready"

            End Using

            btnExtract.Enabled = True
        Else
            DialogResult = DialogResult.Cancel
            Close()
        End If
    End Sub

    Structure FileHeader
        Dim FileType As Byte
        Dim VideoRate As Byte
        Dim SizeInTiles As Size
        Dim Unknown1 As UShort
        Dim Unknown2 As UShort
        Dim LengthInFrames As UInteger
        Dim InitialPalette As UShort()
        Dim AudioDataLength As UShort
    End Structure

    Protected Overrides Function ExtractFiles(ByRef fsIn As FileStream) As DialogResult

        'Algorithm:
        '1) Read in a frame on the main stream
        '2) Do different things depending on the hours value of the frame's timecode
        '   0x00: end of clip; next two frames will be a looping still image
        '   0x01: read in next frame on main stream
        '   0x21: ??? maybe game over?
        '   0x40: run loop of current frame and previous frame; found at the end of clips
        '   0x80: read in next eight frames on alternate stream, then continue reading frames on main stream

        Dim fileHeader As FileHeader

        Dim mainStreamData As New List(Of Byte)
        Dim mainTimecodes As New List(Of UInteger)

        Dim altStreamData As New List(Of Byte)
        Dim altTimecodes As New List(Of UInteger)

        Dim destStreamData As List(Of Byte) = mainStreamData
        Dim destTimecodes As List(Of UInteger) = mainTimecodes

        'Dim isOnMainStream As Boolean = True
        Dim isAltStreamPending As Boolean = False

        Dim sequenceIndex As Integer = 0

        Using br As New BinaryReader(fsIn)

            br.BaseStream.Seek(0, SeekOrigin.Begin)

            'Read file header
            With fileHeader
                .FileType = br.ReadByte
                .VideoRate = br.ReadByte
                .SizeInTiles = New Size(br.ReadByte, br.ReadByte)
                .Unknown1 = br.ReadBigEndianUInt16
                .Unknown2 = br.ReadBigEndianUInt16
                .LengthInFrames = br.ReadBigEndianUInt32

                ReDim .InitialPalette(255)
                For i As Integer = 0 To 255
                    .InitialPalette(i) = br.ReadUInt16
                Next

                .AudioDataLength = br.ReadBigEndianUInt16
            End With

            'Go through all frames in file
            For frameNumber As Integer = 0 To fileHeader.LengthInFrames - 1

                'Get flag
                Dim flag As Byte _
                    = br.ReadByte
                br.BaseStream.Seek(-1, SeekOrigin.Current)

                'Read in frame
                AddFrameToStream(br, destStreamData, destTimecodes)

                ''Branch based on flag
                'Select Case flag

                '    Case 0

                '        If destStreamData Is altStreamData _
                '        And isAltStreamPending = True Then
                '            destStreamData = mainStreamData
                '            destTimecodes = mainTimecodes
                '        Else
                '            'Save file
                '            SaveFile(fileHeader, destStreamData, destTimecodes, sequenceIndex)
                '        End If

                '        'If destStreamData Is altStreamData Then
                '        '    If isAltStreamPending = True Then
                '        '        destStreamData = mainStreamData
                '        '        destTimecodes = mainTimecodes
                '        '    End If
                '        'End If

                '    Case 1
                '        'Do nothing

                '    Case &H40
                '        'Save file
                '        SaveFile(fileHeader, destStreamData, destTimecodes, sequenceIndex)

                '        If destStreamData Is mainStreamData Then
                '            If isAltStreamPending = True Then
                '                destStreamData = altStreamData
                '                destTimecodes = altTimecodes
                '            End If
                '        Else
                '            destStreamData = mainStreamData
                '            destTimecodes = mainTimecodes
                '            isAltStreamPending = False
                '        End If

                '    Case &H80
                '        destStreamData = altStreamData
                '        destTimecodes = altTimecodes

                '        isAltStreamPending = True

                '    Case Else
                '        Dim s As String = $"{flag}"

                'End Select


                ''Branch based on flag
                'Select Case flag

                '    Case 0
                '        'Return to main stream or looping still
                '        If frameNumber < fileHeader.LengthInFrames - 1 Then

                '            'Read two frames of a looping still
                '            For i As Integer = 0 To 1
                '                AddFrameToStream(br, destStreamData, destTimecodes)
                '                frameNumber += 1
                '            Next

                '        End If

                '        'Save file
                '        SaveFile(fileHeader, destStreamData, destTimecodes, sequenceIndex)

                '        If destStreamData Is altStreamData Then

                '            destStreamData = mainStreamData
                '            destTimecodes = mainTimecodes

                '        Else

                '            If isAltStreamPending = True Then

                '                destStreamData = altStreamData
                '                destTimecodes = altTimecodes

                '                isAltStreamPending = False

                '            End If

                '        End If

                '    Case 1
                '        'Do nothing

                '    Case &H20
                '        '???

                '    Case &H40
                '        'End of clip

                '    Case &H80
                '        'Read first eight frames of alternate stream
                '        For i As Integer = 0 To 7
                '            AddFrameToStream(br, altStreamData, altTimecodes)
                '            frameNumber += 1
                '        Next

                '        isAltStreamPending = True

                '    Case Else
                '        Dim s As String = $"{flag}"

                'End Select

            Next

        End Using

        'Save file
        SaveFile(fileHeader, destStreamData, destTimecodes, sequenceIndex)

        Return DialogResult.OK

    End Function

    Private Sub AddFrameToStream(ByRef br As BinaryReader, ByRef destStreamData As List(Of Byte), ByRef destTimecodes As List(Of UInteger))
        'Get timecode
        Dim timecode As UInteger _
            = br.ReadBigEndianUInt32
        destTimecodes.Add(timecode)

        'Get metadata from file
        Dim paletteEntryCount As UShort _
            = br.ReadBigEndianUInt16
        Dim patternDataOffset As UShort _
            = br.ReadBigEndianUInt16
        Dim dataLength As UShort _
            = br.ReadBigEndianUInt16
        Dim paletteEntryWriteOffset As UShort _
            = br.ReadBigEndianUInt16

        'Convert metadata to right endianness
        Dim timecodeBytes As Byte() _
            = BitConverter.GetBytes(timecode)
        Array.Reverse(timecodeBytes)

        Dim paletteEntryCountBytes As Byte() _
            = BitConverter.GetBytes(paletteEntryCount)
        Array.Reverse(paletteEntryCountBytes)

        Dim patternDataOffsetBytes As Byte() _
            = BitConverter.GetBytes(patternDataOffset)
        Array.Reverse(patternDataOffsetBytes)

        Dim dataLengthBytes As Byte() _
            = BitConverter.GetBytes(dataLength)
        Array.Reverse(dataLengthBytes)

        Dim paletteEntryWriteOffsetBytes As Byte() _
            = BitConverter.GetBytes(paletteEntryWriteOffset)
        Array.Reverse(paletteEntryWriteOffsetBytes)

        'Add frame data to stream
        With destStreamData
            .AddRange(timecodeBytes)
            .AddRange(paletteEntryCountBytes)
            .AddRange(patternDataOffsetBytes)
            .AddRange(dataLengthBytes)
            .AddRange(paletteEntryWriteOffsetBytes)
            .AddRange(br.ReadBytes(dataLength))
        End With

    End Sub

    Public Sub SaveFile(ByRef fileHeader As FileHeader, ByRef streamData As List(Of Byte), ByRef timecodes As List(Of UInteger), ByRef index As Integer)
        Dim startTimecode As New SMPTETimecode(timecodes(0), 30)

        ''Save current list of chunks to new file
        'Dim fileName As String _
        '    = $"D:\Temp\{index:000} {startTimecode.Minutes:00}{startTimecode.Seconds:00}{startTimecode.Frames:00} {startTimecode.Hours:X2}.SGA"

        'Using fsOut As New FileStream(fileName, FileMode.Create)
        '    Using bwOut As New BinaryWriter(fsOut)

        '        'Write file header
        '        With fileHeader

        '            bwOut.Write(.FileType)
        '            bwOut.Write(.VideoRate)
        '            bwOut.Write(CByte(.SizeInTiles.Width))
        '            bwOut.Write(CByte(.SizeInTiles.Height))
        '            bwOut.WriteBigEndianUInt16(.Unknown1)
        '            bwOut.WriteBigEndianUInt16(.Unknown2)
        '            bwOut.WriteBigEndianUInt32(timecodes.Count)

        '            For i As Integer = 0 To 255
        '                bwOut.Write(.InitialPalette(i))
        '            Next

        '            bwOut.WriteBigEndianUInt16(.AudioDataLength)

        '            'Write framedata
        '            bwOut.Write(streamData.ToArray)
        '        End With

        '    End Using
        'End Using

        Dim node As New TreeNode($"{index:000} {startTimecode.Minutes:00}:{startTimecode.Seconds:00}:{startTimecode.Frames:00}")

        For Each ui As UInteger In timecodes
            Dim timecode As New SMPTETimecode(ui, 30)
            node.Nodes.Add($"{timecode.Minutes:00}:{timecode.Seconds:00}:{timecode.Frames:00} {If(timecode.Hours = 1, "", $"{timecode.Hours:X2}")}")
        Next
        tvwItems.Nodes.Add(node)

        streamData.Clear()
        timecodes.Clear()

        index += 1
    End Sub

End Class