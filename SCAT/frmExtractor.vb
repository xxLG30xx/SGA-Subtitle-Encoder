Imports System.IO

Public MustInherit Class frmExtractor

    Protected _inputFilePath, _outputFilePath As String
    Protected _lastInputDir, _lastOutputDir As String
    Protected _fileID As String

    Private Sub frmExtractor_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitializeHashTable()
    End Sub

    Protected _hashTable As Dictionary(Of String, String)
    Protected Overridable Sub InitializeHashTable()
        _hashTable = New Dictionary(Of String, String)
    End Sub

    Protected Function GetInputFilePath(ByVal defaultExt As String, ByVal filter As String) As DialogResult
        Dim result As DialogResult

        Using ofdOpen As New OpenFileDialog

            With ofdOpen
                .AutoUpgradeEnabled = True
                .CheckFileExists = True
                .CheckPathExists = True
                .DefaultExt = defaultExt
                .Filter = filter
                .FilterIndex = 0
                .Multiselect = False

                If _lastInputDir <> "" Then
                    .InitialDirectory = _lastInputDir
                End If

                result = .ShowDialog()

                If result = System.Windows.Forms.DialogResult.OK Then
                    _lastInputDir = Path.GetDirectoryName(.FileName)
                    _inputFilePath = .FileName
                End If

            End With

        End Using

        Return result

    End Function

    Protected Function GetFileID(ByRef fs As FileStream) As String
        Return GetSHA256Hash(fs)
    End Function

    Protected Function GetSHA256Hash(ByRef fs As FileStream) As String
        Dim buffer(fs.Length - 1) As Byte
        With fs
            .Read(buffer, 0, fs.Length)
            .Seek(0, SeekOrigin.Begin)
        End With

        Dim shaM As New Security.Cryptography.SHA256Managed
        Dim result As Byte() _
            = shaM.ComputeHash(buffer)

        Dim sb As New Text.StringBuilder
        With sb
            For Each b As Byte In result
                .Append($"{b:X2}")
            Next
        End With

        Return sb.ToString
    End Function

    Private Sub btnExtract_Click(sender As Object, e As EventArgs) Handles btnExtract.Click
        btnExtract.Enabled = False

        Using fs As New FileStream(_inputFilePath, FileMode.Open, FileAccess.Read)
            ExtractFiles(fs)
        End Using

        btnExtract.Enabled = True
    End Sub

    Protected MustOverride Function ExtractFiles(ByRef fsIn As FileStream) As DialogResult

End Class