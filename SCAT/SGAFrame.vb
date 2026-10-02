Namespace SGA

    Public Class Frame
        Private _chunks As List(Of Chunk)
        Public Property Chunks(Optional ByVal getContainerChunk As Boolean = False) As List(Of Chunk)
            Get
                If _chunks IsNot Nothing Then
                    If _chunks.Count = 1 Then
                        If TypeOf (_chunks(0)) Is ContainerChunk _
                        And getContainerChunk = False Then
                            Dim containerChunk As ContainerChunk _
                                = _chunks(0)
                            Return containerChunk.Chunks
                        End If
                    End If
                End If
                Return _chunks
            End Get
            Set(value As List(Of Chunk))
                _chunks = value
            End Set
        End Property

        Public ReadOnly Property VideoChunks As VideoChunk()
            Get
                Return Chunks.OfType(Of VideoChunk).ToArray
            End Get
        End Property

        Public ReadOnly Property AudioChunks As AudioChunk()
            Get
                Return Chunks.OfType(Of AudioChunk).ToArray
            End Get
        End Property

        Public ReadOnly Property HasVideo As Boolean
            Get
                Dim videoChunks As VideoChunk() _
                    = Me.VideoChunks
                If videoChunks.Count > 0 Then
                    Return True
                End If
                Return False
            End Get
        End Property

        Public ReadOnly Property HasAudio As Boolean
            Get
                Dim audioChunks As AudioChunk() _
                    = Me.AudioChunks
                If audioChunks.Count > 0 Then
                    Return True
                End If
                Return False
            End Get
        End Property

        Private _sizeInPixels As Size
        Public Property SizeInPixels As Size
            Get
                If _sizeInPixels.IsEmpty Then
                    If HasVideo Then
                        _sizeInPixels = VideoChunks.First.SizeInPixels
                    Else
                        _sizeInPixels = Size.Empty
                    End If
                End If
                Return _sizeInPixels
            End Get
            Protected Set(value As Size)
                _sizeInPixels = value
            End Set
        End Property

        Private _duration As TimeSpan
        Public Property Duration As TimeSpan
            Get
                If _duration.Ticks = 0 Then
                    If HasAudio Then
                        Dim ticks As Long
                        For Each audioChunk As AudioChunk In AudioChunks
                            ticks += audioChunk.Duration.Ticks
                        Next
                        _duration = New TimeSpan(ticks)
                    Else
                        _duration = New TimeSpan(0, 0, 1)
                    End If
                End If
                Return _duration
            End Get
            Protected Set(value As TimeSpan)
                _duration = value
            End Set
        End Property

        Sub New()
            Chunks = New List(Of Chunk)
        End Sub

        Sub New(ByVal sizeInPixels As Size)
            Me.New()
            Me.SizeInPixels = sizeInPixels
        End Sub

        Sub New(ByVal duration As TimeSpan)
            Me.New()
            Me.Duration = duration
        End Sub

        Sub New(ByVal sizeInPixels As Size, ByVal duration As TimeSpan)
            Me.New(sizeInPixels)
            Me.Duration = duration
        End Sub

    End Class

End Namespace