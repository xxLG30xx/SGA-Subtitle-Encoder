Imports System.IO

Public Class frmWindowsExtractor
    Dim _myText As String _
        = $"{My.Application.Info.Title} Double Switch for Windows 95 file Extractor"

    Dim _fileList As List(Of fileInfo)

    Protected Overrides Sub InitializeHashTable()
        MyBase.InitializeHashTable()
        With _hashTable
            .Add("C5BD52370A10494A85DAA6B75FCDCC0E620B6B3E413DAAE9B618CDD8FE5D5F9A", "Double Switch for Windows 95 Disc 1")
            .Add("9AE62539B1DC6DC48FFAC6477A5382B879CC10E4EC1717935645A1EF67836C35", "Double Switch for Windows 95 Disc 2")
            .Add("70A0E92FDE549CAAF0B63DF95A50D5287908E121BB101F9AB4631E34942C8E01", "Double Switch for Windows 95 Disc 3")
        End With
    End Sub

    Private Sub frmWindowsExtractor_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Text = $"{_myText}"

        Dim result As DialogResult =
            GetInputFilePath("*.gam", "Double Switch for Windows 95 GAM files|DS95DIS1.GAM;DS95DIS2.GAM;DS95DIS3.GAM")

        If result = DialogResult.OK Then
            Using fs As New FileStream(_inputFilePath, FileMode.Open, FileAccess.Read)
                lblStatus.Text = "Identifying file..."
                Application.DoEvents()

                '_fileID = GetFileID(fs)
                'lblStatus.Text = $"Data is from {(_hashTable(_fileID))}"
                'Text = $"{_hashTable(_fileID)} ({Path.GetFileName(_inputFilePath)}) - {_myText}"

                'With tvwItems
                '    .Nodes.Clear()
                '    .Nodes.Add(New TreeNode(_hashTable(_fileID)))
                'End With

                lblStatus.Text = "Reading index..."
                Application.DoEvents()
                _fileList = GetFileList(fs)

                tvwItems.Nodes.Clear()

                For Each fi As fileInfo In _fileList
                    With fi
                        tvwItems.Nodes.Add($"{ .Filename.PadRight(12)} @ ${ .Offset:X8} ({ .Length} bytes)")
                    End With
                Next

                lblStatus.Text = "Ready"

            End Using

            btnExtract.Enabled = True
        Else
            DialogResult = DialogResult.Cancel
            Close()
        End If

    End Sub

    Private Class fileInfo
        Public Property Filename As String
        Public Property Offset As Integer
        Public Property Length As Integer

        Sub New(ByVal filename As String, ByVal offset As Integer, ByVal length As Integer)
            With Me
                .Filename = filename
                .Offset = offset
                .Length = length
            End With
        End Sub
    End Class

    Private Function GetFileList(ByRef fs As FileStream) As List(Of fileInfo)
        Dim fileList = New List(Of fileInfo)

        Debug.WriteLine($"Start of debug output for {Me.Name}.{Reflection.MethodBase.GetCurrentMethod().Name}")
        Debug.Indent()

        'Debug.WriteLine($"Data is from {_hashTable(_fileID)}")

        Using br As New BinaryReader(fs)
            br.BaseStream.Seek(0, SeekOrigin.Begin)

            'Check beginning of file for "FCAT"
            If New String(br.ReadChars(4)) <> "FCAT" Then
                Return Nothing
            End If

            'Get file count
            Dim fileCount As Integer _
                = br.ReadUInt32

            With pbrStatus
                .Minimum = 0
                .Maximum = fileCount
            End With

            For fileIndex As Integer = 1 To fileCount - 1
                pbrStatus.Value = fileIndex
                Application.DoEvents()

                Dim bytes As Byte() _
                    = br.ReadBytes(22)

                'Get filename
                Dim filename As String = ""

                For i As Integer = 0 To 21
                    If bytes(i) = 0 Then
                        Exit For
                    Else
                        filename &= Convert.ToChar(bytes(i))
                    End If
                Next

                Dim offset As Integer _
                    = BitConverter.ToUInt32(bytes, 14)

                Dim endOffset As Integer _
                    = BitConverter.ToUInt32(bytes, 18)

                Dim length As Integer _
                    = endOffset - offset

                fileList.Add(New fileInfo(filename, offset, length))
            Next

        End Using

        Debug.Unindent()
        Debug.WriteLine($"End of debug output for {Me.Name}.{Reflection.MethodBase.GetCurrentMethod().Name}")

        pbrStatus.Value = 0

        Return fileList
    End Function

    Protected Overrides Function ExtractFiles(ByRef fsIn As FileStream) As DialogResult
        Dim result As DialogResult

        Using sfdSave As New FolderBrowserDialog

            With sfdSave
                If _lastOutputDir <> "" Then
                    .SelectedPath = _lastOutputDir
                End If
                '.Description = $"Choose the folder to which you want to extract the files from {_hashTable(_fileID)}."
                .Description = $"Choose the folder to which you want to extract the files."

                .ShowNewFolderButton = True

                result = .ShowDialog()

                If result = System.Windows.Forms.DialogResult.OK Then
                    _outputFilePath = .SelectedPath
                    _lastOutputDir = Path.GetDirectoryName(_outputFilePath)

                    With pbrStatus
                        .Minimum = 0
                        .Maximum = _fileList.Count
                        .Value = 0
                    End With

                    lblStatus.Text = "Extracting files..."

                    Using brIn As New BinaryReader(fsIn)

                        For Each fi As fileInfo In _fileList
                            pbrStatus.Value += 1
                            Application.DoEvents()

                            brIn.BaseStream.Seek(fi.Offset, SeekOrigin.Begin)
                            Dim data As Byte() _
                                = brIn.ReadBytes(fi.Length)

                            Dim filenameOut As String _
                                = $"{ .SelectedPath}\{fi.Filename}"

                            Using fsOut As New FileStream(filenameOut, FileMode.Create)
                                Using bwOut As New BinaryWriter(fsOut)
                                    bwOut.Write(data)
                                End Using
                            End Using
                        Next

                    End Using

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
