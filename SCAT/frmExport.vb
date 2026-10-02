Imports System.IO
Imports System.IO.Path
Imports System.ComponentModel

Public Class frmExport
    Implements Interfaces.ICancelable

    Friend WithEvents Exporter As SGA.Exporter

    'Added on 2015/11/3: Sequence/clip export
    Private _sequence As SGA.Sequence
    Private _clips As List(Of SGA.Clip)

    Private _inputPath As String

    Private Property ExportType As SGA.Exporter.ExportTypeValue
    Private Property ExportFormat As SGA.Exporter.ExportFormatValue

    Private _isCancelled As Boolean = False
    Public Property IsCancelled As Boolean Implements Interfaces.ICancelable.IsCanceled
        Get
            Return _isCancelled
        End Get
        Protected Set(value As Boolean)
            _isCancelled = value
            If _isCancelled = True Then
                RaiseEvent Canceled(Me, New EventArgs)
            End If
        End Set
    End Property

    Public Event Canceled(sender As Object, e As EventArgs) Implements Interfaces.ICancelable.Canceled

    Sub New(ByRef sequence As SGA.Sequence, exportType As SGA.Exporter.ExportTypeValue)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _sequence = sequence
        _inputPath = sequence.File.FileName 'inputPath
        _clips = sequence.Clips
        Me.ExportType = exportType

    End Sub

    'Public Sub ExportCurrentPreview(ByRef sequence As SGA.Sequence)
    '    _sequence = sequence
    '    _inputPath = sequence.File.FileName 'inputPath
    '    _clips = sequence.Clips
    '    ExportType = SGA.Exporter.ExportTypeValue.Sequence
    '    ShowDialog()
    'End Sub

    'Public Sub ExportAllClips(ByRef sequence As SGA.Sequence)
    '    _sequence = sequence
    '    _inputPath = sequence.File.FileName 'inputPath
    '    _clips = sequence.Clips
    '    ExportType = SGA.Exporter.ExportTypeValue.Clips
    '    ShowDialog()
    'End Sub

    Private Sub frmExport_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Export(_inputPath)
    End Sub

    Public Sub Export(ByVal inputPath As String)

        Select Case ExportType
            Case SGA.Exporter.ExportTypeValue.Sequence

                If IsCancelled = False Then
                    Exporter = New SGA.Exporter(_sequence, -1)

                    Dim outputPath As String
                    '= $"{GetDirectoryName(inputPath)}\{GetFileNameWithoutExtension(inputPath)}"

                    If _sequence.HasVideo Then
                        outputPath = GetAVIOutputPath(inputPath)
                        'outputPath &= ".avi"
                        Exporter.Export(outputPath, SGA.Exporter.ExportFormatValue.AVI)
                    Else
                        outputPath = GetWAVOutputPath(inputPath)
                        'outputPath &= ".wav"
                        Exporter.Export(outputPath, SGA.Exporter.ExportFormatValue.WAV)
                    End If
                End If

            Case SGA.Exporter.ExportTypeValue.Clips

                Dim desc As String _
                    = $"Choose a folder in which to save the clips for {GetFileName(inputPath)}."
                Dim path As String _
                    = GetFolderOutputPath(inputPath, desc)

                If path = String.Empty Then
                    Cancel()
                End If

                For i As Integer = 0 To _clips.Count - 1
                    If IsCancelled = False Then
                        Exporter = New SGA.Exporter(_sequence, i)

                        With _clips(i)
                            Dim timecode As UInteger _
                                = .First.Chunks.First.Timecode
                            Dim timeCodeString As String _
                                = New SMPTETimecode(timecode, _sequence.VideoRate).ToString.Replace(":", "")

                            'Dim outputPath As String _
                            '    = $"{GetDirectoryName(inputPath)}\{GetFileNameWithoutExtension(inputPath)} clip {i:000} {timeCodeString}"

                            Dim outputPath As String _
                                = $"{path}\{GetFileName(inputPath).Replace(".", "_")} {i:000} track { .TrackID:X4} time {timeCodeString}"

                            If .HasVideo Then
                                outputPath &= ".avi"
                                Exporter.Export(outputPath, SGA.Exporter.ExportFormatValue.AVI)
                            Else
                                outputPath &= ".wav"
                                Exporter.Export(outputPath, SGA.Exporter.ExportFormatValue.WAV)
                            End If

                        End With

                    End If
                Next

        End Select

        If IsCancelled = True Then
            DialogResult = DialogResult.Cancel
        Else
            DialogResult = DialogResult.OK
        End If

    End Sub

    Private Function GetAVIOutputPath(ByVal inputPath As String) As String
        Return GetFileOutputPath(inputPath, "avi", "Audio/Video Interleave files")
    End Function

    Private Function GetFileOutputPath(ByVal inputPath As String, ByVal fileExtNoDot As String, fileTypeDesc As String) As String
        Using sfd As New SaveFileDialog

            With sfd
                .AutoUpgradeEnabled = True
                .DefaultExt = $"*.{fileExtNoDot}"
                .Filter = $"{fileTypeDesc} (*.{fileExtNoDot})|*.{fileExtNoDot}"

                Dim name As String =
                    GetFileName(inputPath).Replace(".", "_")

                .FileName = name & $".{fileExtNoDot}"

                .InitialDirectory = GetDirectoryName(inputPath)

                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Return .FileName
                Else
                    Return String.Empty
                End If

            End With

        End Using

    End Function

    Private Function GetPNGOutputPath(ByVal inputPath As String) As String
        Dim desc As String _
            = $"The video frames for {GetFileName(inputPath)} will be saved as PNG files in a folder inside the chosen folder called {GetFileName(inputPath).Replace(".", "_")}."
        Dim path As String _
            = GetFolderOutputPath(inputPath, desc)

        If path = String.Empty Then
            Return String.Empty
        Else
            Return path & "\" & GetFileName(inputPath).Replace(".", "_")
        End If
    End Function

    Private Function GetFolderOutputPath(ByVal inputPath As String, ByVal description As String) As String
        Using fbd As New FolderBrowserDialog

            With fbd
                .RootFolder = Environment.SpecialFolder.MyComputer
                .ShowNewFolderButton = True
                .Description = description
                .SelectedPath = GetDirectoryName(inputPath)

                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Return .SelectedPath
                Else
                    Return String.Empty
                End If

            End With

        End Using

    End Function

    Private Function GetWAVOutputPath(ByVal inputFilePath As String) As String
        Return GetFileOutputPath(inputFilePath, "wav", "Wave audio files")
    End Function

    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles Cancel_Button.Click
        Exporter.Cancel()
    End Sub

    Public Sub Cancel() Implements Interfaces.ICancelable.Cancel
        IsCancelled = True
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub _SGAConverter_Canceled() Handles Exporter.Canceled
        Cancel()
    End Sub

    Private Sub _SGAConverter_ProgressChanged(sender As Object, e As Interfaces.ProgressReporterEventArgs) Handles Exporter.ProgressChanged
        If ConversionProgressBar Is Nothing _
        Or ConversionStatusLabel Is Nothing _
        Or pbxVideoFrame Is Nothing Then
            Exit Sub
        End If

        'FIX: Check for NaNs
        If Single.IsNaN(e.ProgressPercentage) = True Then
            ConversionProgressBar.Value = 1
        Else
            ConversionProgressBar.Value = e.ProgressPercentage
        End If

        If e.StatusText <> "" Then
            ConversionStatusLabel.Text = e.StatusText
        End If

        If Exporter.PreviewBitmap IsNot Nothing Then
            pbxVideoFrame.Image = Exporter.PreviewBitmap.Clone
        End If

        Application.DoEvents()

    End Sub

End Class