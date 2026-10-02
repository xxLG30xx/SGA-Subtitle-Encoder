Imports System.IO
Imports System.Drawing.Imaging
Imports System.Windows.Media.Imaging

Namespace SGA

    Public Class Renderer
        Implements Interfaces.ICancelable, Interfaces.IProgressReporter
        Public Event Canceled(sender As Object, e As EventArgs) Implements Interfaces.ICancelable.Canceled
        Public Event ProgressChanged(sender As Object, e As Interfaces.ProgressReporterEventArgs) Implements Interfaces.IProgressReporter.ProgressChanged

        Private _isCancelled As Boolean = False
        Public Property IsCancelled As Boolean Implements Interfaces.ICancelable.IsCanceled
            Get
                Return _isCancelled
            End Get
            Private Set(value As Boolean)
                _isCancelled = value
                If _isCancelled = True Then
                    RaiseEvent Canceled(Me, New EventArgs)
                End If
            End Set
        End Property

        'Private _SGAFile As SGA.File
        Public ReadOnly Property File As File
            Get
                Return Sequence.File
            End Get
            'Protected Set(value As SGA.File)
            '    _SGAFile = value
            'End Set
        End Property

        Private _sequence As Sequence
        Public Property Sequence As Sequence
            Get
                Return _sequence
            End Get
            Protected Set(value As Sequence)
                _sequence = value
            End Set
        End Property

        Private _previewBitmap As Bitmap
        Public Property PreviewBitmap As Bitmap
            Get
                Return _previewBitmap
            End Get
            Private Set(value As Bitmap)
                _previewBitmap = value
            End Set
        End Property

        'TEST Added on 2014/12/1: "Real" Mega Drive, Mega CD, and 32X video hardware support
        Dim mdVDP As New Sega.MegaDrive.VDP
        Dim s32xVDP As New Sega.Super32X.VDP

        Private _VDP As Sega.VDP ' = MDV
        Public Property VDP As Sega.VDP
            Get
                Return _VDP
            End Get
            Private Set(value As Sega.VDP)
                _VDP = value
            End Set
        End Property

        'Private _aspectCorrectionRatio As Single = ASPECT_CORRECTION_RATIO_NONE

        'Added on 2014/8/4: Support for E8 and E9 frames
        Private _frameData As Byte()

        Enum BlankScreenTypeValue
            Black
            Blue
            Green
            Magenta
            Repeat
            ColorBars
        End Enum

        Private _blankScreenType As BlankScreenTypeValue _
            = BlankScreenTypeValue.Green
        Public Property BlankScreenType As BlankScreenTypeValue
            Get
                Return _blankScreenType
            End Get
            Set(value As BlankScreenTypeValue)
                _blankScreenType = value
            End Set
        End Property

        Private _keyColor As Color = Color.FromArgb(0, 231, 0) 'Light green 'Windows.Media.Color = Windows.Media.Color.FromRgb(0, 231, 0)
        Public Property KeyColor As Color 'Windows.Media.Color
            Get
                Return VDP.PaletteAsColors(0) '_keyColor
            End Get
            Set(value As Color) 'Windows.Media.Color)
                _keyColor = value
            End Set
        End Property

        Sub New(ByRef sequence As Sequence)
            Me.Sequence = sequence
            VDP = mdVDP
        End Sub

        'Public Function RenderVideoFrameAsBitmap(ByVal frameNumber As Integer) As Bitmap
        '    Return RenderVideoFrameAsBitmapSource(frameNumber).ToBitmap
        'End Function

        'Public Function RenderVideoFrameAsBitmapSource(ByVal frameNumber As Integer) As BitmapSource
        Public Function RenderVideoFrameAsBitmap(ByVal frameNumber As Integer) As Bitmap

            Dim frame As Frame =
                Sequence.FrameList(frameNumber)

            'Dim frameBitmap As BitmapSource = Nothing

            'Dim renderFrameAsBlank As Boolean = False

            Dim videoChunks As IEnumerable(Of VideoChunk) _
                = frame.VideoChunks.OfType(Of VideoChunk)

            'If videoChunks.Count = 0 Then
            '    renderFrameAsBlank = True
            'Else

            ''Added on 2016/3/14: Skip chunks that can't be seen
            'Dim trackIDs As New List(Of Integer)
            'For Each videoChunk As VideoChunk In videoChunks
            '    trackIDs.Add(videoChunk.TrackID)
            'Next

            For Each videoChunk As VideoChunk In videoChunks

                'Added on 2016/6/14: Chunk F2 still images from Saturn ports
                If TypeOf (videoChunk) Is Chunk99 Then
                    Dim img As Chunk99 _
                        = videoChunk
                    Return img.ToBitmap
                End If

                ''Added on 2016/3/14: Skip chunks that can't be seen
                'If (videoChunk.TrackID And 15) < (trackIDs.Max And 15) Then Continue For

                If TypeOf (videoChunk) Is UnsupportedVideoChunk Then
                    Dim b As New Bitmap(320, 224, PixelFormat.Format24bppRgb)
                    Using g As Graphics = Graphics.FromImage(b)
                        g.Clear(Color.Lime)
                    End Using
                    Return b.Clone
                    'renderFrameAsBlank = True
                ElseIf TypeOf (videoChunk) Is MegaDriveVideoChunk Then
                    VDP = mdVDP
                ElseIf TypeOf (videoChunk) Is Super32XVideoChunk Then
                    VDP = s32xVDP
                End If

                'Added on 2014/8/4: Support for E8 and E9 frames
                'This is here to prevent attempted data reads before decompression
                If TypeOf (videoChunk) Is ChunkE8 Then
                    Dim ChunkE8 As ChunkE8 = videoChunk
                    _frameData = ChunkE8.DecompressFrameData(_frameData)
                End If

                'Set palette
                With VDP
                    If videoChunk.PaletteAsCRAM IsNot Nothing Then
                        If TypeOf (videoChunk) Is Chunk82 Then
                            Dim chunk82 As Chunk82 = videoChunk
                            Dim offset As Integer = chunk82.PaletteEntryWriteOffset
                            'If offset = &H77 Then offset += 1
                            .WriteToCRAM(chunk82.PaletteAsCRAM, offset)
                        Else
                            '.WriteToCRAM(videoChunk.PaletteAsCRAM, 1)
                            .CRAM = videoChunk.PaletteAsCRAM.Clone
                        End If
                    End If

                    If videoChunk.SizeInTiles = Size.Empty Then
                        .SizeInTiles = New Size(1, 1)
                        '.SizeInPixels = videoChunk.SizeInPixels
                    Else
                        .SizeInTiles = videoChunk.SizeInTiles
                        .SizeInPixels = videoChunk.SizeInPixels
                    End If
                End With

                'Added on 2016/3/14: Special handling for enemy death animations in Sewer Shark
                Dim inheritedVideoChunk As VideoChunk = Nothing

                'Added on 2016/6/8: Support for animation chunks on 32-bit systems
                Dim maxSizeInTiles As New Size(0, 0)
                Dim maxSizeInPixels As New Size(0, 0)

                If TypeOf (videoChunk) Is IAnimationChunk Then
                    Dim animationChunk As IAnimationChunk = videoChunk

                    Dim matchingAnimationChunks As IEnumerable(Of IAnimationChunk) _
                        = File.Chunks.OfType(Of IAnimationChunk).Where(Function(chunk As IAnimationChunk) chunk.AnimationID = animationChunk.AnimationID)

                    For Each matchingVideoChunk As VideoChunk In matchingAnimationChunks
                        maxSizeInTiles.Width = Math.Max(maxSizeInTiles.Width, matchingVideoChunk.SizeInTiles.Width)
                        maxSizeInTiles.Height = Math.Max(maxSizeInTiles.Height, matchingVideoChunk.SizeInTiles.Height)
                        maxSizeInPixels.Width = Math.Max(maxSizeInPixels.Width, matchingVideoChunk.SizeInPixels.Width)
                        maxSizeInPixels.Height = Math.Max(maxSizeInPixels.Height, matchingVideoChunk.SizeInPixels.Height)
                    Next
                End If

                If TypeOf (videoChunk) IsNot IAnimationChunk Then
                    If videoChunk.TrackID > 15 Then
                        Dim trackID As Integer _
                            = videoChunk.TrackID
                        Dim timecode As UInteger _
                            = videoChunk.Timecode
                        inheritedVideoChunk = File.Chunks.OfType(Of VideoChunk).Where(Function(chunk As Chunk) (chunk.TrackID < 15) And ((chunk.TrackID And 15) = (trackID And 15)) And chunk.Timecode = timecode).First
                        With inheritedVideoChunk
                            If .PaletteAsCRAM IsNot Nothing Then
                                VDP.CRAM = .PaletteAsCRAM.Clone
                            End If
                        End With
                    End If
                End If

                If VDP Is mdVDP Then
                    Dim mdVideoChunk As MegaDriveVideoChunk _
                        = videoChunk

                    With mdVDP
                        'TODO: Unify the handling of inter-frames

                        'Added on 2015/2/5: Support for C2 and C4 frames
                        If TypeOf (mdVideoChunk) Is ChunkC4 Then
                            For chunkTileDataIndex As Integer = 0 To mdVideoChunk.TileData.Length - 1

                                For pixelIndex As Integer = 0 To 1
                                    Dim srcPixel As Byte = mdVideoChunk.TileData(chunkTileDataIndex) And (15 << 4 * (1 - pixelIndex))
                                    If srcPixel <> 0 Then
                                        .WriteToTileData({(.TileData(chunkTileDataIndex + 32) And (15 << 4 * pixelIndex)) Or srcPixel}, chunkTileDataIndex)
                                    End If
                                Next

                            Next

                            .WriteToTileMapB(mdVideoChunk.TileMap)

                            'frameBitmap = .FrameBufferAsWriteableBitmap

                        ElseIf TypeOf (mdVideoChunk) Is ChunkD1 Then
                            Dim fgVideoChunk As ChunkD1 _
                                    = videoChunk

                            Dim bgVideoChunk As MegaDriveVideoChunk _
                                    = inheritedVideoChunk

                            .WriteToTileData(bgVideoChunk.TileData)
                            .WriteToTileMapB(bgVideoChunk.TileMap)

                            Dim srcTileMapEntry As Sega.MegaDrive.VDP.TileMapEntry
                            Dim srcTileDataIndex As Integer
                            Dim srcTileData(31) As Byte

                            For row As Integer = 0 To .TileMapB.GetUpperBound(1)
                                For col As Integer = 0 To .TileMapB.GetUpperBound(0)
                                    srcTileMapEntry = fgVideoChunk.TileMap(col, row)

                                    If srcTileMapEntry.PatternIndex <> 0 Then
                                        srcTileDataIndex = ((row * .SizeInTiles.Width) + col) * 32
                                        Array.Copy(mdVideoChunk.TileData, srcTileDataIndex, srcTileData, 0, 32)

                                        .WriteToTileData(srcTileData, col, row)
                                        .WriteToTileMapB(srcTileMapEntry, col, row)
                                    End If

                                Next
                            Next

                            'frameBitmap = .FrameBufferAsWriteableBitmap
                        Else
                            .WriteToTileData(mdVideoChunk.TileData)
                            .WriteToTileMapB(mdVideoChunk.TileMap)
                            'frameBitmap = .FrameBufferAsWriteableBitmap
                        End If

                    End With

                    'frameBitmap = RenderMegaDriveVideoFrame(videoChunk, clearFrameBuffer, useGreenScreenBackground).Clone

                    'End If

                ElseIf VDP Is s32xVDP Then
                    Dim s32xVideoChunk As Super32XVideoChunk _
                        = videoChunk

                    With s32xVDP
                        If inheritedVideoChunk IsNot Nothing Then
                            Dim bgVideoChunk As Super32XVideoChunk _
                                = inheritedVideoChunk
                            .WriteToFrameBuffer(bgVideoChunk.FrameBufferData, 0)
                        End If

                        .WriteToFrameBuffer(s32xVideoChunk.FrameBufferData, 0)
                        'frameBitmap = .FrameBufferAsWriteableBitmap
                    End With

                    'frameBitmap = RenderSuper32XVideoFrame(videoChunk, clearFrameBuffer, useGreenScreenBackground).Clone

                    If ProgramOptions.RenderMacroblockOutlines = True Then
                        If TypeOf (videoChunk) Is Chunk81 Then
                            Dim chunk81 As Chunk81 _
                                = videoChunk
                            Return chunk81.GetBlockShapeBitmap(VDP.FrameBufferAsBitmap)
                        End If
                    End If

                End If

                'End If

                If TypeOf (videoChunk) Is IAnimationChunk Then
                    With VDP
                        Dim animationChunk As IAnimationChunk = videoChunk

                        'Dim tempFrameBitmap As New Bitmap(maxSizeInTiles.Width * 8, maxSizeInTiles.Height * 8, .FrameBufferAsBitmap.PixelFormat)
                        Dim tempFrameBitmap As New Bitmap(maxSizeInPixels.Width, maxSizeInPixels.Height, .FrameBufferAsBitmap.PixelFormat)

                        Using g As Graphics = Graphics.FromImage(tempFrameBitmap)
                            g.Clear(Color.FromArgb(KeyColor.R, KeyColor.G, KeyColor.B))
                            g.DrawImage(.FrameBufferAsBitmap, animationChunk.PixelOffset)
                        End Using

                        'frameBitmap = tempFrameBitmap.ToBitmapSource
                        Return tempFrameBitmap '.ToBitmapSource

                    End With
                End If

            Next
            'End If

            'If renderFrameAsBlank = True Then
            '    frameBitmap = RenderBlankVideoFrame(frameNumber)
            'End If

            'Return frameBitmap.Clone
            Return VDP.FrameBufferAsBitmap 'VDP.FrameBufferAsWriteableBitmap

        End Function

        Public Function RenderVideoStream(ByVal path As String) As Size
            If IsCancelled = True Then
                Exit Function
            End If

            RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(0, "Rendering video stream..."))

            Dim frameSize As Size

            Using ms As New MemoryStream
                Dim frameBitmap As Bitmap = RenderVideoFrameAsBitmap(0)
                frameBitmap.Save(ms, Imaging.ImageFormat.Tiff)

                Dim encoderInfo As ImageCodecInfo = ImageCodecInfo.GetImageEncoders.First(Function(info As ImageCodecInfo) info.MimeType = "image/tiff")
                Dim encoderParams As New EncoderParameters(2)
                encoderParams.Param(0) = New EncoderParameter(Encoder.SaveFlag, Convert.ToInt64(EncoderValue.MultiFrame)) 'param
                encoderParams.Param(1) = New EncoderParameter(Encoder.Compression, Convert.ToInt64(EncoderValue.CompressionLZW)) 'param

                Dim tiff As Image = Image.FromStream(ms)
                tiff.Save(path, encoderInfo, encoderParams)

                For frameIndex As Integer = 1 To Sequence.FrameList.Count - 1
                    If IsCancelled = True Then
                        Exit Function
                    End If

                    frameBitmap = RenderVideoFrameAsBitmap(frameIndex)

                    PreviewBitmap = frameBitmap.Clone

                    With frameSize
                        .Width = Math.Max(.Width, frameBitmap.Width)
                        .Height = Math.Max(.Height, frameBitmap.Height)
                    End With

                    encoderParams.Param(0) = New EncoderParameter(Encoder.SaveFlag, Convert.ToInt64(EncoderValue.FrameDimensionPage)) 'param
                    encoderParams.Param(1) = New EncoderParameter(Encoder.Compression, Convert.ToInt64(EncoderValue.CompressionLZW)) 'param

                    tiff.SaveAdd(frameBitmap, encoderParams)

                    RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(
                        (frameIndex / (Sequence.FrameList.Count - 1)) * 100,
                        $"Frame {frameIndex + 1:0000} / {Sequence.FrameList.Count:0000}"
                        ))
                    Application.DoEvents()
                Next

                encoderParams = New EncoderParameters(1)
                encoderParams.Param(0) = New EncoderParameter(Encoder.SaveFlag, Convert.ToInt64(EncoderValue.Flush)) 'param

                tiff.SaveAdd(encoderParams)
                tiff.Dispose()
            End Using

            Return frameSize
        End Function

        Public Function GetFrameFromRenderedVideoStream(ByVal filename As String, ByVal frameIndex As Integer) As Bitmap
            Dim img As Image = Image.FromFile(filename)
            Dim frameCount As Integer = img.GetFrameCount(FrameDimension.Page)
            img.SelectActiveFrame(FrameDimension.Page, frameIndex)
            Return New Bitmap(img)
        End Function

        Private Function RenderBlankVideoFrame(ByVal frameNumber As Integer) As BitmapSource
            'Dim b As New Bitmap(320, 224, PixelFormat.Format24bppRgb)
            'Using g As Graphics = Graphics.FromImage(b)
            '    g.Clear(Color.Lime)
            'End Using
            'Return b.ToBitmapSource

            Dim blankScreenType As BlankScreenTypeValue _
                = _blankScreenType

            Dim frameBuffer As Bitmap

            If VDP Is Nothing Then
                If blankScreenType = BlankScreenTypeValue.Repeat Then
                    blankScreenType = BlankScreenTypeValue.Green
                End If
                frameBuffer = New Bitmap(320, 224, PixelFormat.Format24bppRgb)
            Else
                frameBuffer = VDP.FrameBufferAsBitmap.Clone
            End If

            Using g As Graphics = Graphics.FromImage(frameBuffer)
                With g

                    Dim rect As New Rectangle(0, 0, frameBuffer.Width, frameBuffer.Height)

                    Select Case blankScreenType
                        Case BlankScreenTypeValue.Black
                            .Clear(Color.Black)

                        Case BlankScreenTypeValue.Blue
                            .Clear(Color.Blue)

                        Case BlankScreenTypeValue.Green
                            .Clear(Color.Lime)

                        Case BlankScreenTypeValue.Magenta
                            .Clear(Color.Magenta)

                        Case BlankScreenTypeValue.Repeat
                            Return frameBuffer.ToBitmapSource

                        Case BlankScreenTypeValue.ColorBars
                            Using colorBarBitmap As Bitmap = My.Resources.ResourceManager.GetObject("SMPTE_Color_Bars")
                                '.ScaleTransform(frameBuffer.Width / colorBarBitmap.Width, frameBuffer.Height / colorBarBitmap.Height)
                                '.DrawImage(colorBarBitmap, 0, 0)
                                .DrawImage(colorBarBitmap, rect)
                            End Using

                    End Select

                End With

            End Using

            Return frameBuffer.ToBitmapSource
        End Function

        Private Function RenderMegaDriveVideoFrame(ByRef videoChunk As MegaDriveVideoChunk, ByVal clearFrameBuffer As Boolean, Optional ByVal useGreenScreenBackground As Boolean = False) As Bitmap
            'ActiveVDP = MDV

            ''Added on 2014/8/4: Support for E8 and E9 frames
            'If TypeOf (videoChunk) Is ChunkE8 Then
            '    Dim ChunkE8 As ChunkE8 = videoChunk
            '    _frameData = ChunkE8.DecompressFrameData(_frameData)
            'End If

            ''TODO: Unify the handling of inter-frames

            'With MDV
            '    If videoChunk.PaletteAsCRAM IsNot Nothing Then
            '        .CRAM = videoChunk.PaletteAsCRAM.Clone
            '    End If
            '    .SizeInTiles = videoChunk.SizeInTiles

            '    '.TileData = videoChunk.TileData
            '    '.TileMapB = videoChunk.TileMap

            '    Return .FrameBufferAsBitmap
            'End With

        End Function

        Private Function RenderSuper32XVideoFrame(ByRef videoChunk As Super32XVideoChunk, ByVal clearFrameBuffer As Boolean, Optional ByVal useGreenScreenBackground As Boolean = False) As Bitmap
            'ActiveVDP = S32XVDP
            'With S32XVDP
            '    .FrameBuffer = videoChunk.FrameBufferData.Clone
            '    If videoChunk.PaletteAsCRAM IsNot Nothing Then
            '        .CRAM = videoChunk.PaletteAsCRAM.Clone
            '    End If
            '    .SizeInTiles = videoChunk.SizeInTiles

            '    '.UpdateFrameBuffer()
            '    Return .FrameBufferAsBitmap
            'End With
        End Function

        Private Function RenderFrameInfo(ByRef frame As Frame, ByRef frameBitmap As Bitmap) As Bitmap
            'Added on 2015/5/9: Offset/Timecode display

            Dim videoChunk As VideoChunk = Nothing
            Dim audioChunk As AudioChunk = Nothing

            Dim frameInfo As String = ""

            For Each chunk As Chunk In frame.Chunks
                With chunk
                    frameInfo &= $"{Convert.ToInt32(.Type):X2} { .TrackID:X4} ${ .Offset:X8} T{ .Timecode:X8}" & vbNewLine
                End With
            Next

            Dim infoStringSize As SizeF
            Dim infoStringFont As New Font(FontFamily.GenericMonospace, 8)
            Using g As Graphics = Graphics.FromImage(frameBitmap)
                infoStringSize = g.MeasureString(frameInfo, infoStringFont)
            End Using

            With infoStringSize
                .Width = Math.Ceiling(.Width / 2) * 2
                .Height = Math.Ceiling(.Height / 2) * 2
            End With

            Dim frameInfoBitmap As New Bitmap(
                Math.Max(frameBitmap.Width, infoStringSize.Width),
                frameBitmap.Height + infoStringSize.Height,
                frameBitmap.PixelFormat
                )

            Using g As Graphics = Graphics.FromImage(frameInfoBitmap)
                With g
                    .CompositingQuality = Drawing2D.CompositingQuality.Default
                    .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                    .PixelOffsetMode = Drawing2D.PixelOffsetMode.Default
                    .SmoothingMode = Drawing2D.SmoothingMode.Default
                    .TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit

                    .FillRectangle(Brushes.Gray, New Rectangle(New Point(0, 0), frameInfoBitmap.Size))
                    .DrawImage(frameBitmap, 0, 0)
                    .DrawString(frameInfo, infoStringFont, Brushes.Black, 1, frameBitmap.Height + 1)
                    .DrawString(frameInfo, infoStringFont, Brushes.White, 0, frameBitmap.Height)
                End With

            End Using

            Return frameInfoBitmap
        End Function

        'Public Function RenderAudioStream(Optional ByVal startingFrameNumber As Integer = 0, Optional ByVal programID As Integer = -1) As Short()
        Public Function RenderAudioStream(ByVal startingFrameNumber As Integer, ByVal endingFrameNumber As Integer, Optional ByVal programID As Integer = -1) As Short()
            RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(0, "Rendering audio stream..."))

            Dim outputSamples As New List(Of Short)

            For frameNumber As Integer = startingFrameNumber To endingFrameNumber 'Sequence.FrameList.Count - 1
                Dim audioFrame As Short() =
                    RenderAudioFrame(frameNumber, programID)

                If audioFrame Is Nothing Then
                    With Sequence
                        ReDim audioFrame((.AudioRate \ .VideoRate))
                    End With
                    For i As Integer = 0 To audioFrame.Length - 1
                        audioFrame(i) = 0
                    Next
                End If

                outputSamples.AddRange(audioFrame)

                RaiseEvent ProgressChanged(Me, New Interfaces.ProgressReporterEventArgs(
                    (frameNumber / (Sequence.FrameList.Count - 1)) * 100,
                    $"Frame {frameNumber + 1:0000} / {Sequence.FrameList.Count:0000}"
                    ))
                Application.DoEvents()
            Next

            Return outputSamples.ToArray
        End Function

        Public Function RenderAudioFrame(ByVal frameNumber As Integer, Optional ByVal programID As Integer = -1) As Short()
            Dim frame As Frame =
                Sequence.FrameList(frameNumber)

            If frame.AudioChunks.Count > 0 Then
                Dim audioChunks As IEnumerable(Of AudioChunk) _
                    = frame.AudioChunks

                Dim outputSamples As New List(Of Short)
                Dim prevTrackIDs As New SortedSet(Of Integer)

                For chunkIndex As Integer = 0 To audioChunks.Count - 1
                    Dim audioChunk As AudioChunk _
                        = audioChunks(chunkIndex)

                    'Added on 2016/3/5: Track filtering to support Cantonese audio in Supreme Warrior
                    If (programID < 0) Or (programID = audioChunk.TrackID) Then

                        Dim startIndex As Integer = 0

                        If prevTrackIDs.Contains(audioChunk.TrackID) = True Then
                            startIndex = outputSamples.Count
                        Else
                            prevTrackIDs.Add(audioChunk.TrackID)
                        End If

                        Dim audioData As Byte() _
                        = audioChunk.SamplesAs8BitUnsigned

                        For i As Integer = startIndex To startIndex + audioData.Length - 1
                            Dim sample As Short _
                            = ConvertUnsigned8BitToSigned16Bit(audioData(i - startIndex))
                            If outputSamples.Count <= i Then
                                outputSamples.Add(sample)
                            Else
                                Dim mixedSamples As Integer _
                                = CInt(outputSamples(i)) + CInt(sample)
                                If mixedSamples < 0 Then
                                    mixedSamples = Math.Max(mixedSamples, CInt(Short.MinValue))
                                Else
                                    mixedSamples = Math.Min(mixedSamples, CInt(Short.MaxValue))
                                End If
                                outputSamples(i) = CShort(mixedSamples)
                            End If
                        Next

                    End If


                Next

                Return outputSamples.ToArray

            End If

            Return Nothing

        End Function

        Private Function ConvertUnsigned8BitToSigned16Bit(ByVal sample As Byte) As Short
            Dim signed16Bit As Short _
                = (CShort(sample) - 128) * 256
            Return signed16Bit
        End Function

        Public Sub Cancel() Implements Interfaces.ICancelable.Cancel
            IsCancelled = True
        End Sub

    End Class

End Namespace