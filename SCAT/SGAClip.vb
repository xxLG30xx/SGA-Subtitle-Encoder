Namespace SGA

    Public Class Clip
        Inherits List(Of Frame)

        Public Property TrackID As Integer
        Public Property StartingFrameNumber As Integer

        Sub New()
            MyBase.New()
        End Sub

        Public ReadOnly Property HasVideo As Boolean
            Get
                For Each frame As Frame In Me
                    If frame.HasVideo Then Return True
                Next
                Return False
            End Get
        End Property

        Public ReadOnly Property HasAudio As Boolean
            Get
                For Each frame As Frame In Me
                    If frame.HasAudio Then Return True
                Next
                Return False
            End Get
        End Property

        Private _visible As Boolean
        Public Property Visible As Boolean
            Get
                If HasVideo = False Then
                    Return False
                Else
                    Return _visible
                End If
            End Get
            Set(value As Boolean)
                If HasVideo = False Then
                    _visible = False
                Else
                    _visible = value
                End If
            End Set
        End Property

        Private _audible As Boolean
        Public Property Audible As Boolean
            Get
                If HasAudio = False Then
                    Return False
                Else
                    Return _audible
                End If
            End Get
            Set(value As Boolean)
                If HasAudio = False Then
                    _audible = False
                Else
                    _audible = value
                End If
            End Set
        End Property

        Public ReadOnly Property VideoChunks As IEnumerable(Of VideoChunk)
            Get
                If HasVideo Then
                    Dim chunks As New List(Of VideoChunk)
                    For Each frame As Frame In Me
                        chunks.AddRange(frame.VideoChunks)
                    Next
                    Return chunks.ToArray
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public ReadOnly Property AudioChunks As IEnumerable(Of AudioChunk)
            Get
                If HasAudio Then
                    Dim chunks As New List(Of AudioChunk)
                    For Each frame As Frame In Me
                        chunks.AddRange(frame.AudioChunks)
                    Next
                    Return chunks.ToArray
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Private _sizeInPixels As Size
        Public ReadOnly Property SizeInPixels() As Size
            Get
                If HasVideo = True Then
                    If _sizeInPixels.IsEmpty Then
                        For Each frame As Frame In Me
                            For Each chunk As VideoChunk In frame.VideoChunks
                                With chunk.SizeInPixels
                                    _sizeInPixels.Width = Math.Max(.Width, _sizeInPixels.Width)
                                    _sizeInPixels.Height = Math.Max(.Height, _sizeInPixels.Height)
                                End With
                            Next
                        Next
                    End If
                End If
                Return _sizeInPixels
            End Get
        End Property

        Private _videoRate As Double = -1
        Shared DefaultVideoRate As Double = 12
        Public Property VideoRate As Double
            Get
                If HasAudio Then
                    If _videoRate < 0 Then
                        _videoRate = Sequence.GetMostCommonVideoRate(AudioChunks)
                    End If
                Else
                    _videoRate = DefaultVideoRate
                End If
                Return _videoRate
            End Get
            Protected Set(value As Double)
                _videoRate = value
            End Set
        End Property

        Private _audioProgramCount As Integer = -1
        Public ReadOnly Property AudioProgramCount As Integer
            Get
                If _audioProgramCount < 0 Then
                    If HasAudio = True Then
                        '_audioProgramCount = 0
                        For Each frame As Frame In Me
                            _audioProgramCount = Math.Max(frame.AudioChunks.Count, _audioProgramCount)
                        Next
                    Else
                        _audioProgramCount = 0
                    End If
                End If
                Return _audioProgramCount
            End Get
        End Property

        Private _audioRate As Single = -1
        Shared DefaultAudioRate As Single = 16000
        Public Property AudioRate As Single
            Get
                If HasAudio Then
                    If _audioRate < 0 Then
                        _audioRate = Sequence.GetMostCommonAudioRate(AudioChunks)
                    End If
                Else
                    _audioRate = DefaultAudioRate
                End If
                Return _audioRate
            End Get
            Protected Set(value As Single)
                _audioRate = value
            End Set
        End Property

        Sub New(ByVal audioRate As Single, videoRate As Double)
            MyBase.New
            With Me
                .AudioRate = audioRate
                .VideoRate = videoRate
            End With
        End Sub

    End Class

End Namespace

