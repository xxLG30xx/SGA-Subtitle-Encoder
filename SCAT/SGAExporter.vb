Imports System.IO
Imports System.IO.Path
Imports System.Windows.Media.Imaging

Namespace SGA

    Public Class Exporter
        Implements Interfaces.ICancelable, Interfaces.IProgressReporter, IDisposable
        Public Event Canceled(sender As Object, e As EventArgs) Implements Interfaces.ICancelable.Canceled
        Public Event ProgressChanged(sender As Object, e As Interfaces.ProgressReporterEventArgs) Implements Interfaces.IProgressReporter.ProgressChanged

        Enum ExportTypeValue As Integer
            Sequence = 1
            Clips = 2
        End Enum

        Enum ExportFormatValue As Integer
            AVI = 1
            PNG = 2
            WAV = 4
        End Enum

        'Private WithEvents File As File
        Private WithEvents Sequence As Sequence
        Private WithEvents Renderer As Renderer

        'Added on 2015/11/3: Exports clips as separate files
        Private Property ExportType As ExportTypeValue
        'Private Property ExportFormat As ExportFormatValue

        'Private _isSequenceExport As Boolean = False
        Private _clipIndex As Integer

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

        Private _previewBitmap As Bitmap = New Bitmap(1, 1, Imaging.PixelFormat.Format24bppRgb)
        Public Property PreviewBitmap As Bitmap
            Get
                Return _previewBitmap
            End Get
            Protected Set(value As Bitmap)
                _previewBitmap = value
            End Set
        End Property

        'Private MaxFrameSize As Size

        Private _frameBitmapList As List(Of String)
        Private Property FrameBitmapList As List(Of String)
            Get
                If _frameBitmapList Is Nothing Then
                    If IsCancelled = True Then
                        Return Nothing
                    End If

                    RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(0, "Rendering video frame bitmaps..."))

                    'Generate frame bitmaps
                    _frameBitmapList = New List(Of String)

                    For frameNumber As Integer = 0 To Sequence.FrameList.Count - 1
                        If IsCancelled = True Then
                            Return Nothing
                        End If

                        Dim b As Bitmap = Renderer.RenderVideoFrameAsBitmap(frameNumber).Clone

                        If b.Size <> Sequence.SizeInPixels Then
                            Using bResized As New Bitmap(Sequence.SizeInPixels.Width, Sequence.SizeInPixels.Height, b.PixelFormat)
                                Dim pt As New Point((bResized.Width \ 2) - (b.Width \ 2), (bResized.Height \ 2) - (b.Height \ 2))

                                Using g As Graphics = Graphics.FromImage(bResized)
                                    With g
                                        .DrawImage(b, pt)
                                    End With
                                End Using

                                b = bResized.Clone
                            End Using
                        End If

                        PreviewBitmap = b.Clone

                        Dim tempFilename As String _
                            = GetTempFileName()
                        b.Save(tempFilename)
                        _frameBitmapList.Add(tempFilename)

                        b.Dispose()

                        RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(
                            (frameNumber / (Sequence.FrameList.Count - 1)) * 100,
                            $"Frame {frameNumber + 1:0000} / {Sequence.FrameList.Count:0000}"
                            ))
                        Application.DoEvents()
                    Next

                End If
                Return _frameBitmapList
            End Get
            Set(value As List(Of String))
                _frameBitmapList = value
            End Set
        End Property

        'Private _frameBitmapList As List(Of Bitmap)
        'Private Property FrameBitmapList As List(Of Bitmap)
        '    Get
        '        If _frameBitmapList Is Nothing Then
        '            If IsCancelled = True Then
        '                Return Nothing
        '            End If

        '            RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(0, "Rendering video frame bitmaps..."))

        '            'Generate frame bitmaps
        '            _frameBitmapList = New List(Of Bitmap)

        '            For frameNumber As Integer = 0 To Sequence.FrameList.Count - 1
        '                If IsCancelled = True Then
        '                    Return Nothing
        '                End If

        '                Dim b As Bitmap = Renderer.RenderVideoFrameAsBitmap(frameNumber).Clone

        '                If b.Size <> Sequence.SizeInPixels Then
        '                    Dim bResized As New Bitmap(Sequence.SizeInPixels.Width, Sequence.SizeInPixels.Height, b.PixelFormat)

        '                    Dim pt As New Point((bResized.Width \ 2) - (b.Width \ 2), (bResized.Height \ 2) - (b.Height \ 2))

        '                    Using g As Graphics = Graphics.FromImage(bResized)
        '                        With g
        '                            .DrawImage(b, pt)
        '                        End With
        '                    End Using

        '                    b = bResized.Clone

        '                End If

        '                PreviewBitmap = b.Clone

        '                _frameBitmapList.Add(b.Clone)

        '                b.Dispose()

        '                RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(
        '                    (frameNumber / (Sequence.FrameList.Count - 1)) * 100,
        '                    $"Frame {frameNumber + 1:0000} / {Sequence.FrameList.Count:0000}"
        '                    ))
        '                Application.DoEvents()
        '            Next

        '        End If
        '        Return _frameBitmapList
        '    End Get
        '    Set(value As List(Of Bitmap))
        '        _frameBitmapList = value
        '    End Set
        'End Property

        'Private _bmpEncoder As BmpBitmapEncoder
        ''Private _bmpEncoderStream As MemoryStream
        'Private ReadOnly Property BmpEncoder As BmpBitmapEncoder
        '    Get
        '        If _bmpEncoder Is Nothing Then
        '            If IsCancelled = True Then
        '                Return Nothing
        '            End If

        '            RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(0, "Rendering video frame bitmaps..."))

        '            _bmpEncoder = New BmpBitmapEncoder
        '            '_bmpEncoderStream = New MemoryStream

        '            For frameNumber As Integer = 0 To Sequence.FrameList.Count - 1
        '                If IsCancelled = True Then
        '                    Return Nothing
        '                End If

        '                'Dim bmp As WriteableBitmap _
        '                '    = Renderer.RenderVideoFrameAsBitmapSource(frameNumber)

        '                Dim bmp As Bitmap _
        '                    = Renderer.RenderVideoFrameAsBitmap(frameNumber)

        '                PreviewBitmap = bmp

        '                'Dim rgb24Bmp As New FormatConvertedBitmap(bmp, Windows.Media.PixelFormats.Rgb24, Nothing, 0)

        '                _bmpEncoder.Frames.Add(BitmapFrame.Create(bmp)) 'rgb24Bmp))

        '                With MaxFrameSize
        '                    .Width = Math.Max(.Width, bmp.Width)
        '                    .Height = Math.Max(.Height, bmp.Height)
        '                End With

        '                RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(
        '                    (frameNumber / (Sequence.FrameList.Count - 1)) * 100,
        '                    $"Frame {frameNumber + 1:0000} / {Sequence.FrameList.Count:0000}"
        '                    ))
        '                'Application.DoEvents()
        '            Next

        '        End If

        '        Return _bmpEncoder
        '    End Get
        'End Property

        Private _wavFile As WAVFile
        Private Property WavFile As WAVFile
            Get
                If _wavFile Is Nothing Then
                    If Sequence.HasAudio Then
                        '_wavFile = New WAVFile(CInt(Sequence.AudioRate), Renderer.RenderAudioStream)
                        _wavFile = New WAVFile(CInt(Sequence.AudioRate), Renderer.RenderAudioStream(0, Sequence.FrameList.Count - 1))
                    End If
                End If
                Return _wavFile
            End Get
            Set(value As WAVFile)
                _wavFile = value
            End Set
        End Property

        Public Sub New(ByVal sequence As Sequence, Optional ByVal clipIndex As Integer = -1)
            Me.Sequence = sequence
            Renderer = New Renderer(Me.Sequence)

            _clipIndex = clipIndex

            If clipIndex > -1 Then
                ExportType = ExportTypeValue.Clips
                Me.Sequence.BuildFrameListFromClips({clipIndex})
            Else
                ExportType = ExportTypeValue.Sequence
            End If
        End Sub

        Public Sub Export(ByVal outputPath As String, ByVal exportFormat As ExportFormatValue)
            Select Case exportFormat
                Case ExportFormatValue.AVI
                    ExportToAVI(outputPath)
                Case ExportFormatValue.PNG
                    ExportToPNG(outputPath)
                Case ExportFormatValue.WAV
                    ExportToWAV(outputPath)
            End Select
        End Sub

        Public Sub ExportToAVI(ByVal outputPath As String)
            If outputPath = String.Empty Then
                Cancel()
            End If

            If IsCancelled = True Then
                Exit Sub
            End If

            'Estimate output file size to determine whether file splitting is necessary
            Dim splitPositions As New Dictionary(Of Long, Long)

            Dim maxFileSizeInBytes As Long _
                = 1000000000
            Dim estFileSizeInBytes As Long = 0
            Dim frameListPos, lengthInFrames As Long

            For Each file As String In FrameBitmapList
                Dim fi As New FileInfo(file)
                Dim frameSizeInBytes As Long _
                    = fi.Length 'frameSize.Width * frameSize.Height * 3
                estFileSizeInBytes += frameSizeInBytes
                lengthInFrames += 1

                If (estFileSizeInBytes >= maxFileSizeInBytes) _
                Or (file = FrameBitmapList.Last) Then
                    splitPositions.Add(frameListPos, lengthInFrames)
                    estFileSizeInBytes = 0
                    frameListPos += lengthInFrames
                    lengthInFrames = 0
                End If

            Next

            RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(0, "Writing AVI file..."))

            For Each splitPosition As Long In splitPositions.Keys
                Dim startingFramenumber As Integer _
                    = splitPosition
                Dim endingFrameNumber As Integer _
                    = splitPosition + (splitPositions(splitPosition)) - 1

                Dim newOutputPath As String _
                    = outputPath

                If splitPosition <> splitPositions.Keys.First Then
                    newOutputPath = $"{GetFileNameWithoutExtension(outputPath)} {splitPositions(splitPosition):0000}.avi"
                End If

                'Changed on 2015/3/2: Combine AVI writing into frame list generation loop to cut down on memory usage
                Dim am As New AviFile.AviManager(newOutputPath, False)
                Dim vs As AviFile.VideoStream _
                    = am.AddVideoStream(False, Math.Floor(Sequence.VideoRate), (Sequence.SizeInPixels.Width * Sequence.SizeInPixels.Height) * 3, Sequence.SizeInPixels.Width, Sequence.SizeInPixels.Height, PreviewBitmap.PixelFormat)

                For frameNumber As Integer = startingFramenumber To endingFrameNumber
                    Dim b As New Bitmap(FrameBitmapList(frameNumber))
                    vs.AddFrame(b)
                    RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs((frameNumber / (Sequence.FrameList.Count - 1)) * 100, ""))
                Next

                ' Cleanup
                For Each file As String In FrameBitmapList
                    My.Computer.FileSystem.DeleteFile(file)
                Next
                FrameBitmapList = Nothing
                vs = Nothing

                Dim includeAudio As Boolean = False

                Select Case ExportType
                    Case ExportTypeValue.Sequence
                        If Sequence.HasAudio Then
                            includeAudio = True
                        End If
                    Case ExportTypeValue.Clips
                        If Sequence.Clips(_clipIndex).HasAudio Then
                            includeAudio = True
                        End If
                End Select

                ' Generate audio frames and output to AVI audio stream
                If includeAudio = True Then
                    Dim wavFileName As String = My.Computer.FileSystem.GetTempFileName

                    Dim wavFile As New WAVFile(CInt(Sequence.AudioRate), Renderer.RenderAudioStream(startingFramenumber, endingFrameNumber))

                    wavFile.Save(wavFileName)

                    am.AddAudioStream(wavFileName, 0)

                    My.Computer.FileSystem.DeleteFile(wavFileName)
                End If

                am.Close()

                'Cleanup
                am = Nothing
            Next

            'If outputPath = String.Empty Then
            '    Cancel()
            'End If

            'Dim frameBitmapList As List(Of Bitmap) = Me.FrameBitmapList

            'If IsCancelled = True Then
            '    Exit Sub
            'End If

            ''Changed on 2015/3/2: Combine AVI writing into frame list generation loop to cut down on memory usage
            'Dim am As New AviFile.AviManager(outputPath, False)
            'Dim vs As AviFile.VideoStream _
            '    = am.AddVideoStream(False, Math.Floor(Sequence.VideoRate), (Sequence.SizeInPixels.Width * Sequence.SizeInPixels.Height) * 3, Sequence.SizeInPixels.Width, Sequence.SizeInPixels.Height, PreviewBitmap.PixelFormat)

            'For frameNumber As Integer = 0 To Sequence.FrameList.Count - 1
            '    Dim b As Bitmap = frameBitmapList(frameNumber)
            '    vs.AddFrame(b)
            '    RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs((frameNumber / (Sequence.FrameList.Count - 1)) * 100, ""))
            'Next

            '' Cleanup
            'vs = Nothing

            'Dim includeAudio As Boolean = False

            'Select Case ExportType
            '    Case ExportTypeValue.Sequence
            '        If Sequence.HasAudio Then
            '            includeAudio = True
            '        End If
            '    Case ExportTypeValue.Clips
            '        If Sequence.Clips(_clipIndex).HasAudio Then
            '            includeAudio = True
            '        End If
            'End Select

            '' Generate audio frames and output to AVI audio stream
            'If includeAudio = True Then
            '    Dim wavFileName As String = My.Computer.FileSystem.GetTempFileName
            '    WavFile.Save(wavFileName)

            '    am.AddAudioStream(wavFileName, 0)

            '    My.Computer.FileSystem.DeleteFile(wavFileName)
            'End If

            'am.Close()

            ''Cleanup
            'am = Nothing

        End Sub

        Public Sub ExportToPNG(ByVal outputPath As String)
            If outputPath = String.Empty Then
                Cancel()
            End If

            If My.Computer.FileSystem.DirectoryExists(outputPath) = False Then
                My.Computer.FileSystem.CreateDirectory(outputPath)
            End If

            Dim frameBitmapList As List(Of Bitmap) ' = Me.FrameBitmapList

            If IsCancelled = True Then
                Exit Sub
            End If

            RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(0, "Writing PNG files..."))

            For frameNumber As Integer = 0 To Sequence.FrameList.Count - 1
                If IsCancelled = True Then
                    Exit Sub
                End If

                Dim bmp As Bitmap _
                    = frameBitmapList(frameNumber)

                'PreviewBitmap = frameBitmapList(frameNumber)
                PreviewBitmap = bmp.Clone

                Dim rect As New Rectangle(New Point(0, 0), PreviewBitmap.Size)
                With bmp
                    bmp = .Clone(rect, Imaging.PixelFormat.Format32bppArgb)
                    .MakeTransparent(Color.FromArgb(Renderer.KeyColor.R, Renderer.KeyColor.G, Renderer.KeyColor.B))
                End With

                Dim img As Image = PreviewBitmap
                img.Save($"{outputPath}\Frame {frameNumber:0000}.png", Imaging.ImageFormat.Png)

                RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs((frameNumber / (Sequence.FrameList.Count - 1)) * 100, ""))
            Next

        End Sub

        'Private Function GetMaxFrameSize() As Size
        '    Dim frameSize As New Size(0, 0)

        '    Dim frame As Frame

        '    For frameIndex As Integer = 0 To Sequence.FrameList.Count - 1
        '        frame = Sequence.FrameList(frameIndex)

        '        With frameSize
        '            .Width = Math.Max(.Width, frame.SizeInPixels.Width)
        '            .Height = Math.Max(.Height, frame.SizeInPixels.Height + 40)
        '        End With

        '        'RaiseEvent ProgressChanged((frameIndex / (SGARenderer.Frames.Count - 1)) * 100, "")
        '    Next

        '    Return frameSize
        'End Function

        Public Sub ExportToWAV(ByVal outputPath As String)
            If outputPath = String.Empty Then
                Cancel()
            End If

            If IsCancelled = True Then
                Exit Sub
            End If

            RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(0, "Writing WAV file..."))

            WavFile.Save(outputPath)
        End Sub

        Private Sub SGARenderer_ProgressChanged(sender As Object, e As Interfaces.ProgressReporterEventArgs) Handles Renderer.ProgressChanged
            RaiseEvent ProgressChanged(sender, e)
            PreviewBitmap = Renderer.PreviewBitmap
            Application.DoEvents()
        End Sub

        Public Sub Cancel() Implements Interfaces.ICancelable.Cancel
            'IsRunning = False
            Renderer.Cancel()
            IsCancelled = True
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
            'PreviewBitmap.Dispose()
            disposed = True
        End Sub

        Protected Overrides Sub Finalize()
            Dispose(False)
        End Sub
#End Region

    End Class

End Namespace