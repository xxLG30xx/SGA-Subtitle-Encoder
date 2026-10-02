Public Class CDSFile
    Public RawFileData As Byte()
    Public Shared Function FromFile(ByVal filename As String) As CDSFile
        Dim fileExtension As String _
            = System.IO.Path.GetExtension(filename).ToLower
        Select Case fileExtension
            Case ".cds"
                Try
                    Dim fsIn As New System.IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
                    Dim fileLength As Long _
                        = fsIn.Length
                    If fileLength < 512 + 16 Then
                        Dim message As String _
                            = "Not a valid SegaCD CD-ROM image file.  File size must be at least 528 bytes."
                        Dim fileEx As Exception _
                            = New Exception(message)
                        Throw fileEx
                    End If
                    Dim fileData As Byte()
                    With fsIn
                        ReDim fileData(.Length - 1)
                        .Seek(0, IO.SeekOrigin.Begin)
                        .Read(fileData, 0, .Length)
                        .Close()
                    End With
                    fsIn = Nothing
                    Dim newCDSFile As New CDSFile
                    newCDSFile.RawFileData = fileData
                    Return newCDSFile
                Catch ex As Exception
                    MsgBox(ex.Message)
                    Return Nothing
                End Try
            Case Else
                Dim prompt As String _
                    = fileExtension & " format files are not supported."
                Dim style As MsgBoxStyle _
                    = MsgBoxStyle.OKOnly + MsgBoxStyle.Critical
                Dim title As String _
                    = "Sorry"
                MsgBox(prompt, style, title)
                Return Nothing
        End Select
    End Function
End Class