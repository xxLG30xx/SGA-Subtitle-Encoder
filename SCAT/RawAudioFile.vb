Public Class RawAudioFile
    'Public RawAudioData As Byte()

    'Public Shared Function FromCDSFile(ByVal CDSFile As CDSFile) As RawAudioFile
    '    Dim rawAudioData As Byte()
    '    Dim rawAudioDataHasBeenFound As Boolean = False
    '    Dim rawAudioDataStartPos As Long
    '    With CDSFile
    '        For i As Integer = 0 To .RawFileData.GetUpperBound(0)
    '            If rawAudioDataHasBeenFound Then
    '                rawAudioData(i - rawAudioDataStartPos) = .RawFileData(i)
    '            Else
    '                If .RawFileData(i) = &H2 Then
    '                    If .RawFileData(i + 1) = &HD0 Then
    '                        rawAudioDataHasBeenFound = True
    '                        rawAudioDataStartPos = i + 2
    '                        ReDim rawAudioData(.RawFileData.GetUpperBound(0) - rawAudioDataStartPos)
    '                        i += 1
    '                    End If
    '                End If
    '            End If
    '        Next
    '    End With
    '    If rawAudioDataHasBeenFound = False Then
    '        Return Nothing
    '    End If
    '    Dim newRawAudioFile As New RawAudioFile
    '    With newRawAudioFile
    '        .RawAudioData = rawAudioData
    '        For i As Integer = 0 To .RawAudioData.GetUpperBound(0)
    '            If .RawAudioData(i) < 128 Then
    '                .RawAudioData(i) = 128 - .RawAudioData(i)
    '            End If
    '        Next
    '    End With
    '    Return newRawAudioFile
    'End Function

    'Public Sub Save(ByVal filename As String)
    '    Dim fsOut As New System.IO.FileStream(filename, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None)
    '    With fsOut
    '        .Write(RawAudioData, 0, RawAudioData.Length)
    '        .Close()
    '    End With
    '    fsOut = Nothing
    'End Sub

End Class
