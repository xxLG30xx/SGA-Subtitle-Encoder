Imports System.Windows.Media.Imaging
Imports System.IO
Imports System.Timers
Imports System.Windows.Threading

Namespace SGA

    Public Class Player
        Implements IDisposable

        Private _Sequence As Sequence
        Public Property Sequence As Sequence
            Get
                Return _Sequence
            End Get
            Protected Set(value As Sequence)
                _Sequence = value
            End Set
        End Property

        Private _Renderer As Renderer
        Public Property Renderer As Renderer
            Get
                Return _Renderer
            End Get
            Protected Set(value As Renderer)
                _Renderer = value
            End Set
        End Property

        Dim _wavFile As WAVFile
        Dim _wavFileMemoryStream As MemoryStream
        Dim _soundPlayer As Media.SoundPlayer

        Dim dispatcherTimer As DispatcherTimer
        Dim _frameTimer As Stopwatch

        'Dim _msPerFrame As Double
        Dim _ticksPerFrame As Long
        Dim _timerAdjustment As TimeSpan

        Dim _frameStartTimes As Long()
        Dim _frameDurations As Long()

        Public ReadOnly Property IsPlaying As Boolean
            Get
                Return dispatcherTimer.IsEnabled
            End Get
        End Property

        Private _frameNumber As Integer
        Private Property FrameNumber As Integer
            Get
                Return _frameNumber
            End Get
            Set(value As Integer)
                _frameNumber = value
                RaiseEvent FrameNumberChanged(_frameNumber)
            End Set
        End Property
        Public Event FrameNumberChanged(ByVal frameNumber As Integer)

        Private _videoFrameBitmap As Bitmap 'BitmapSource
        Private Property VideoFrameBitmap As Bitmap 'BitmapSource
            Get
                Return _videoFrameBitmap
            End Get
            Set(value As Bitmap) 'BitmapSource)
                _videoFrameBitmap = value
                RaiseEvent VideoFrameChanged(Me, New VideoFrameChangedEventArgs(_videoFrameBitmap))
            End Set
        End Property

        Public Class VideoFrameChangedEventArgs
            Inherits EventArgs
            'Public ReadOnly Property FrameAsBitmapSource As BitmapSource
            Private _frameAsBitmap As Bitmap
            Public Property FrameAsBitmap As Bitmap
                Get
                    Return _frameAsBitmap
                End Get
                Protected Set(value As Bitmap)
                    _frameAsBitmap = value
                End Set
            End Property
            Sub New(ByVal frameBitmap As Bitmap) 'BitmapSource)
                MyBase.New
                FrameAsBitmap = frameBitmap
            End Sub
        End Class

        Public Event VideoFrameChanged(sender As Object, e As VideoFrameChangedEventArgs)

        Private _audioFrameSamples As Short()
        Private Property AudioFrameSamples As Short()
            Get
                Return _audioFrameSamples
            End Get
            Set(value As Short())
                _audioFrameSamples = value
                RaiseEvent AudioFrameChanged(_audioFrameSamples)
            End Set
        End Property
        Public Event AudioFrameChanged(ByRef samples As Short())

        Sub New(ByRef sequence As Sequence)
            Me.Sequence = sequence
            Renderer = New Renderer(sequence)

            If sequence.HasAudio Then
                InitializeWAVFile(0)
                InitializeSoundPlayer()
            End If

            FrameNumber = 0

            '_msPerFrame = If(sequence.VideoRate > 0, 1 / sequence.VideoRate, 1) * 1000
            _ticksPerFrame = If(sequence.VideoRate > 0, CLng(Math.Round(TimeSpan.TicksPerSecond / sequence.VideoRate)), TimeSpan.TicksPerSecond)
            _timerAdjustment = New TimeSpan(0)

            'dispatcherTimer = New DispatcherTimer(New TimeSpan(_ticksPerFrame), DispatcherPriority.Normal, AddressOf dispatchTimer_Tick, Dispatcher.CurrentDispatcher)
            dispatcherTimer = New DispatcherTimer(New TimeSpan(_ticksPerFrame / 4), DispatcherPriority.Normal, AddressOf dispatchTimer_Tick, Dispatcher.CurrentDispatcher)
            dispatcherTimer.Stop()

            _frameTimer = New Stopwatch

            ReDim _frameStartTimes(sequence.FrameList.Count - 1)
            ReDim _frameDurations(sequence.FrameList.Count - 1)
            For i As Integer = 1 To _frameStartTimes.Count - 1
                _frameStartTimes(i) = If(i = 0, 0, _frameStartTimes(i - 1) + _frameDurations(i - 1))
                _frameDurations(i) = sequence.FrameList(i).Duration.Ticks
            Next

            PlayFrame(0)
        End Sub

        Sub InitializeWAVFile(ByVal frameNumber As Integer, Optional ByVal programID As Integer = -1)
            '_wavFile = New WAVFile(CInt(Sequence.AudioRate), Renderer.RenderAudioStream(frameNumber, programID))
            _wavFile = New WAVFile(CInt(Sequence.AudioRate), Renderer.RenderAudioStream(frameNumber, Sequence.FrameList.Count - 1, programID))
            _wavFileMemoryStream = New MemoryStream
            _wavFile.Save(_wavFileMemoryStream)
            _wavFileMemoryStream.Seek(0, SeekOrigin.Begin)
        End Sub

        Sub InitializeSoundPlayer()
            _soundPlayer = New Media.SoundPlayer(_wavFileMemoryStream)
            _soundPlayer.Load()
            Do Until _soundPlayer.IsLoadCompleted = True : Loop
        End Sub

        Sub Play(ByVal startingFrameNumber As Integer, Optional ByVal silent As Boolean = False)
            Seek(startingFrameNumber)

            If Sequence.HasAudio Then
                InitializeWAVFile(startingFrameNumber)
                InitializeSoundPlayer()
                _soundPlayer.Play()
            End If

            _frameTimer.Restart()
            dispatcherTimer.Start()
        End Sub

        Dim Dispatcher As Dispatcher = Dispatcher.CurrentDispatcher

        Private Sub dispatchTimer_Tick()
            If _frameTimer.Elapsed.Ticks + _timerAdjustment.Ticks >= _frameStartTimes(FrameNumber) + _frameDurations(FrameNumber) Then
                UpdateUI()
            End If
        End Sub

        Private Sub UpdateUI()
            PlayFrame(FrameNumber)
            AudioFrameSamples = Renderer.RenderAudioFrame(FrameNumber)
            FrameNumber += 1
            If FrameNumber >= Sequence.FrameList.Count Then
                Seek(0)
            End If
        End Sub

        Sub Pause()
            If Sequence.HasAudio Then
                _soundPlayer.Stop()
            End If
            _frameTimer.Reset()
            dispatcherTimer.Stop()
        End Sub

        Sub Seek(ByVal frameNumber As Integer)
            Pause()
            Me.FrameNumber = frameNumber
            _timerAdjustment = New TimeSpan(_frameStartTimes(frameNumber))
            PlayFrame(frameNumber)
        End Sub

        Private Sub PlayFrame(ByVal frameNumber As Integer)
            VideoFrameBitmap = Renderer.RenderVideoFrameAsBitmap(frameNumber) 'Renderer.RenderVideoFrameAsBitmapSource(frameNumber)
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
            If _soundPlayer IsNot Nothing Then
                _soundPlayer.Stop()
                _soundPlayer.Dispose()
            End If

            If _wavFileMemoryStream IsNot Nothing Then
                _wavFileMemoryStream.Dispose()
            End If

            _wavFile = Nothing

            disposed = True
        End Sub

        Protected Overrides Sub Finalize()
            Dispose(False)
        End Sub

#End Region

    End Class

End Namespace