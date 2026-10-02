Public Class SGAPlayerControl

    Friend WithEvents Player As SGA.Player

    Public ReadOnly Property IsPlaying As Boolean
        Get
            If Player IsNot Nothing Then
                Return Player.IsPlaying
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property VideoFrame As Bitmap
        Get
            Return pbxVideoFrame.Image.Clone
        End Get
    End Property

    Private _frameNumber As Integer
    Public Property FrameNumber As Integer
        Get
            Return trbarFrameNumber.Value
        End Get
        Private Set(value As Integer)
            _frameNumber = value

            If Player.IsPlaying = False Then
                Seek(_frameNumber)
            End If
            UpdateTimeDisplay(_frameNumber)

            RaiseEvent FrameNumberChanged(Me, New FrameNumberChangedEventArgs(_frameNumber))
        End Set
    End Property
    Public Event FrameNumberChanged(sender As Object, e As FrameNumberChangedEventArgs)

    Public Class FrameNumberChangedEventArgs
        Inherits EventArgs

        Private _frameNumber As Integer
        Public ReadOnly Property FrameNumber As Integer
            Get
                Return _frameNumber
            End Get
        End Property

        Sub New(ByVal frameNumber As Integer)
            MyBase.New
            _frameNumber = frameNumber
        End Sub
    End Class

    Sub LoadSequence(ByRef sequence As SGA.Sequence)
        InitializeDisplay()
        Player = New SGA.Player(sequence)
        InitializePlayControls()
        RaiseEvent SequenceLoaded(Me, New SequenceLoadedEventArgs)
    End Sub

    Public Event SequenceLoaded(sender As Object, e As SequenceLoadedEventArgs)

    Public Class SequenceLoadedEventArgs
        Inherits EventArgs
    End Class

    Public Sub InitializeDisplay()
        pbxVideoFrame.Image = Nothing
        pbxAudioFrame.Image = Nothing
    End Sub

    Public Sub InitializePlayControls()
        SuspendLayout()

        If Player.Sequence.FrameList.Count > 0 Then

            With trbarFrameNumber
                .Enabled = True
                .Maximum = Player.Sequence.FrameList.Count - 1
                .LargeChange = Player.Sequence.VideoRate
            End With

            With btnPlayPause
                .Enabled = True
                .Image = My.Resources.ResourceManager.GetObject("Play32px")
                .Focus()
            End With

        Else

            With trbarFrameNumber
                .Enabled = False
                .Maximum = 1
                .LargeChange = 1
            End With

            With btnPlayPause
                .Enabled = False
            End With
        End If

        Dim timecode As New SMPTETimecode(trbarFrameNumber.Maximum, Math.Round(Player.Sequence.VideoRate))
        lblTotalTime.Text = timecode.ToString 'GetFormattedSMTPETimecode(trbarFrameNumber.Maximum, Math.Round(Player.Sequence.VideoRate))

        With trbarFrameNumber
            .Value = 0
            UpdateTimeDisplay(.Value)
            Seek(.Value)
        End With

        ResumeLayout()
    End Sub

    Private Sub UpdateTimeDisplay(ByVal frameNumber As Integer)
        If Player.Sequence.VideoRate <> 0 Then
            Dim timecode As New SMPTETimecode(frameNumber, Math.Round(Player.Sequence.VideoRate))
            lblCurrentTime.Text = timecode.ToString 'GetFormattedSMTPETimecode(frameNumber, CSng(Player.Sequence.VideoRate))
            Application.DoEvents()
        End If
    End Sub

    Sub Play()
        If Player IsNot Nothing Then
            btnPlayPause.Image = My.Resources.ResourceManager.GetObject("Pause32px")
            Player.Play(trbarFrameNumber.Value)
        End If
    End Sub

    Sub Pause()
        If Player IsNot Nothing Then
            btnPlayPause.Image = My.Resources.ResourceManager.GetObject("Play32px")
            Player.Pause()
        End If
    End Sub

    Sub Seek(ByVal frameNumber As Integer)
        If Player IsNot Nothing Then
            Pause()
            Player.Seek(frameNumber)
        End If
    End Sub

    Private Sub PlayPauseButton_Click(sender As Object, e As EventArgs) Handles btnPlayPause.Click
        If IsPlaying = False Then
            Play()
        Else
            Pause()
        End If
        UpdateTimeDisplay(FrameNumber)
    End Sub

    Private Sub Player_VideoFrameChanged(sender As Object, e As SGA.Player.VideoFrameChangedEventArgs) Handles Player.VideoFrameChanged
        pbxVideoFrame.Image = e.FrameAsBitmap
        RaiseEvent VideoFrameChanged(Me, e)
    End Sub

    Public Event VideoFrameChanged(sender As Object, e As SGA.Player.VideoFrameChangedEventArgs)

    Private Sub Player_AudioFrameChanged(ByRef samples As Short()) Handles Player.AudioFrameChanged
        pbxAudioFrame.Image =
            If(samples Is Nothing,
               Nothing,
               RenderAudioFrameBitmap(samples)
            )
    End Sub

    Private Function RenderAudioFrameBitmap(ByRef samples As Short()) As Bitmap

        Using b As New Bitmap(pbxAudioFrame.Width, pbxAudioFrame.Height, Imaging.PixelFormat.Format24bppRgb)
            Using g As Graphics = Graphics.FromImage(b)
                With g
                    .CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                    .InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                    .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                    .SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                End With

                g.FillRectangle(Brushes.Black, 0, 0, b.Width, b.Height)

                Using linePen As New Pen(Brushes.White, 0.25 * (g.DpiX / g.DpiY))

                    With linePen
                        .Alignment = Drawing2D.PenAlignment.Center
                        .EndCap = Drawing2D.LineCap.Round
                        .LineJoin = Drawing2D.LineJoin.Round
                        .StartCap = Drawing2D.LineCap.Round
                    End With

                    Dim baseY As Integer _
                        = b.Height / 2

                    Dim newDrawPos, oldDrawPos As Point
                    With newDrawPos
                        .X = 0
                        .Y = baseY
                    End With
                    oldDrawPos = newDrawPos

                    Dim inc As New PointF(
                        b.Width / samples.Length,
                        (b.Height / 2) / Short.MaxValue
                        )

                    For i As Integer = 0 To samples.Length - 1
                        newDrawPos.X = i * inc.X
                        newDrawPos.Y = baseY + (samples(i) * inc.Y)

                        If (newDrawPos.X <> oldDrawPos.X) Or (newDrawPos.Y <> oldDrawPos.Y) Then
                            g.DrawLine(linePen, oldDrawPos, newDrawPos)
                        End If

                        oldDrawPos = newDrawPos
                    Next

                End Using

            End Using

            Return b.Clone
        End Using

        Return Nothing

    End Function

    Private Sub Player_FrameNumberChanged(ByVal frameNumber As Integer) Handles Player.FrameNumberChanged
        If frameNumber > trbarFrameNumber.Maximum Then
            Pause()
        Else
            trbarFrameNumber.Value = frameNumber
        End If
    End Sub

    Private Sub trbarFrameNumber_ValueChanged(sender As Object, e As EventArgs) Handles trbarFrameNumber.ValueChanged
        FrameNumber = trbarFrameNumber.Value
    End Sub

    Private Sub trbarFrameNumber_MouseDown(sender As Object, e As MouseEventArgs) Handles trbarFrameNumber.MouseDown
        Pause()
    End Sub

End Class
