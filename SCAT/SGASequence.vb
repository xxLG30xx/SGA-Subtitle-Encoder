Namespace SGA

    Public Class Sequence
        Implements Interfaces.IProgressReporter
        Public Event ProgressChanged(sender As Object, e As Interfaces.ProgressReporterEventArgs) Implements Interfaces.IProgressReporter.ProgressChanged

        Private _SGAFile As File
        Public Property File As File
            Get
                Return _SGAFile
            End Get
            Private Set(value As File)
                _SGAFile = value
            End Set
        End Property

        Private _trackIDs As List(Of Integer)
        Public ReadOnly Property TrackIDs As List(Of Integer)
            Get
                If _trackIDs Is Nothing Then
                    _trackIDs = New List(Of Integer)
                    For Each chunk As Chunk In File.Chunks
                        _trackIDs.Add(chunk.TrackID)
                    Next
                    _trackIDs = _trackIDs.Distinct.ToList

                    Dim sortedTrackIDs As New List(Of Integer)
                    With sortedTrackIDs
                        .AddRange(_trackIDs.Where(Function(id As Integer) id < 15))
                        .Sort()
                    End With

                    For Each trackID As Integer In _trackIDs.Where(Function(id As Integer) id > 15)
                        Dim matchingTrackID As Integer _
                            = sortedTrackIDs.FindLast(Function(id As Integer) ((id And 15) = (trackID And 15)) And ((id And 240) < (trackID And 240)))
                        Dim insertIndex As Integer _
                            = sortedTrackIDs.IndexOf(matchingTrackID) + 1
                        sortedTrackIDs.Insert(insertIndex, trackID)
                    Next
                    _trackIDs = sortedTrackIDs.ToList

                End If
                Return _trackIDs
            End Get
        End Property

        Private _timecodes As List(Of UInteger)
        Public ReadOnly Property Timecodes As UInteger()
            Get
                If _timecodes Is Nothing Then
                    _timecodes = New List(Of UInteger)
                    For Each chunk As Chunk In File.Chunks
                        _timecodes.Add(chunk.Timecode)
                    Next
                    '_timecodes = _timecodes.Distinct.ToList
                End If
                Return _timecodes.ToArray
            End Get
        End Property

        Private _clips As List(Of Clip)
        Public Property Clips As List(Of Clip)
            Get
                Return _clips
            End Get
            Protected Set(value As List(Of Clip))
                _clips = value
            End Set
        End Property

        Public ReadOnly Property VideoClips As IEnumerable(Of Clip)
            Get
                Dim _videoClips As IEnumerable(Of Clip) _
                    = Clips.Where(Function(clip As Clip) clip.HasVideo = True)
                Return _videoClips
            End Get
        End Property

        Public ReadOnly Property AudioClips As IEnumerable(Of Clip)
            Get
                Dim _audioClips As IEnumerable(Of Clip) _
                    = Clips.Where(Function(clip As Clip) clip.HasAudio = True)
                Return _audioClips
            End Get
        End Property

        Public ReadOnly Property HasVideo As Boolean
            Get
                If VideoClips IsNot Nothing Then
                    Return True
                End If
                Return False

                'If Clips IsNot Nothing Then
                '    For Each clip As Clip In Clips
                '        If clip.HasVideo Then Return True
                '    Next
                'End If
                'Return False
            End Get
        End Property

        Public ReadOnly Property HasAudio As Boolean
            Get
                If AudioClips IsNot Nothing Then
                    Return True
                End If
                Return False

                'If Clips IsNot Nothing Then
                '    For Each clip As Clip In Clips
                '        If clip.HasAudio Then Return True
                '    Next
                'End If
                'Return False
            End Get
        End Property

        Public ReadOnly Property VideoClipCount As Integer
            Get
                If VideoClips IsNot Nothing Then
                    Return VideoClips.Count
                End If
                Return 0

                'Dim count As Integer
                'For Each clip As Clip In Clips
                '    If clip.HasVideo Then count += 1
                'Next
                'Return count
            End Get
        End Property

        Public ReadOnly Property AudioClipCount As Integer
            Get
                If AudioClips IsNot Nothing Then
                    Return AudioClips.Count
                End If
                Return 0

                'Dim count As Integer
                'For Each clip As Clip In Clips
                '    If clip.HasAudio Then count += 1
                'Next
                'Return count
            End Get
        End Property

        Private _sizeInPixels As Size
        Public ReadOnly Property SizeInPixels As Size
            Get
                If HasVideo = True Then
                    If _sizeInPixels.IsEmpty Then
                        For Each clip As Clip In Clips
                            With clip.SizeInPixels
                                _sizeInPixels.Width = Math.Max(.Width, _sizeInPixels.Width)
                                _sizeInPixels.Height = Math.Max(.Height, _sizeInPixels.Height)
                            End With
                        Next
                    End If
                End If
                Return _sizeInPixels
            End Get
        End Property

        Private _videoRate As Double = -1
        Public ReadOnly Property VideoRate As Double
            Get
                If _videoRate < 0 Then
                    '_videoRate = GetMostCommonVideoRate(SGAFile.Chunks.OfType(Of AudioChunk))
                    _videoRate = GetMostCommonVideoRate(File.Chunks)
                End If
                Return _videoRate
            End Get
        End Property

        Shared ValidVideoRates As Double() = {1, 6, 12, 15}
        Shared Function GetMostCommonVideoRate(ByRef chunks As IEnumerable(Of Chunk)) As Double
            Dim videoRatesFound As New Dictionary(Of Double, Integer)

            For Each chunk As Chunk In chunks
                If TypeOf (chunk) Is ContainerChunk Then
                    Dim containerChunk As ContainerChunk _
                        = chunk
                    For Each audiochunk As AudioChunk In containerChunk.AudioChunks
                        AddToFoundRates(audiochunk, videoRatesFound)
                    Next
                ElseIf TypeOf (chunk) Is AudioChunk Then
                    Dim audioChunk As AudioChunk _
                        = chunk
                    AddToFoundRates(audioChunk, videoRatesFound)
                End If
            Next

            If videoRatesFound.Count = 0 Then
                Return ValidVideoRates.First
            End If

            Dim mostCommonRate As Double = videoRatesFound.First.Key
            For Each entry As KeyValuePair(Of Double, Integer) In videoRatesFound
                If entry.Value > videoRatesFound(mostCommonRate) Then
                    mostCommonRate = entry.Key
                End If
            Next
            Return mostCommonRate
        End Function

        Private Shared Sub AddToFoundRates(ByRef audioChunk As AudioChunk, ByRef videoRatesFound As Dictionary(Of Double, Integer))
            Dim rate As Double _
                    = Math.Round(audioChunk.FrameRate)
            If ValidVideoRates.Contains(rate) Then
                If videoRatesFound.Keys.Contains(rate) = True Then
                    videoRatesFound(rate) += 1
                Else
                    videoRatesFound.Add(rate, 1)
                End If
            End If
        End Sub

        Private _audioProgramCount As Integer = -1
        Public ReadOnly Property AudioProgramCount As Integer
            Get
                If _audioProgramCount < 0 Then
                    If HasAudio = False Then
                        _audioProgramCount = 0
                    Else
                        _audioProgramCount = 1

                        Dim containerChunks As IEnumerable(Of ContainerChunk) _
                            = File.Chunks.OfType(Of ContainerChunk)()

                        If containerChunks.Count > 0 Then
                            For Each containerChunk As ContainerChunk In containerChunks
                                If containerChunk.AudioChunks.Count > 1 Then
                                    For Each audioChunk As AudioChunk In containerChunk.AudioChunks
                                        If audioChunk.TrackID = 255 Then
                                            _audioProgramCount = 2
                                            Exit For
                                        End If
                                    Next
                                    If _audioProgramCount > 1 Then Exit For
                                End If
                            Next
                        End If

                    End If

                End If

                Return _audioProgramCount
            End Get
        End Property

        Private _audioRate As Single = -1
        Public ReadOnly Property AudioRate As Single
            Get
                If _audioRate < 0 Then
                    Dim audioChunks As New List(Of AudioChunk)
                    For Each chunk As Chunk In File.Chunks
                        If TypeOf (chunk) Is AudioChunk Then
                            audioChunks.Add(chunk)
                        ElseIf TypeOf (chunk) Is ContainerChunk Then
                            Dim containerChunk As ContainerChunk _
                                = chunk
                            audioChunks.AddRange(containerChunk.AudioChunks)
                        End If
                        _audioRate = GetMostCommonAudioRate(audioChunks)
                    Next
                End If
                Return _audioRate
            End Get
        End Property

        Shared Function GetMostCommonAudioRate(ByRef audioChunks As IEnumerable(Of AudioChunk)) As Single
            Dim audioRatesFound As New Dictionary(Of Double, Integer)

            For Each audiochunk As AudioChunk In audioChunks
                Dim rate As Double _
                    = Math.Round(audiochunk.SampleRate)
                If audioRatesFound.Keys.Contains(rate) = False Then
                    audioRatesFound.Add(rate, 1)
                Else
                    audioRatesFound(rate) += 1
                End If
            Next

            If audioRatesFound.Count = 0 Then
                Return 0
            End If

            Dim mostCommonRate As Double = audioRatesFound.First.Key
            For Each entry As KeyValuePair(Of Double, Integer) In audioRatesFound
                If entry.Value > audioRatesFound(mostCommonRate) Then
                    mostCommonRate = entry.Key
                End If
            Next

            Return mostCommonRate
        End Function

        Shared Function GetMostCommonFrameDuration(ByRef audioChunks As IEnumerable(Of AudioChunk)) As TimeSpan
            Dim audioDurationsFound As New Dictionary(Of TimeSpan, Integer)

            For Each audiochunk As AudioChunk In audioChunks
                Dim duration As TimeSpan _
                    = audiochunk.Duration
                If audioDurationsFound.Keys.Contains(duration) = True Then
                    audioDurationsFound(duration) += 1
                Else
                    audioDurationsFound.Add(duration, 1)
                End If
            Next

            If audioDurationsFound.Count = 0 Then
                Return New TimeSpan(0)
            End If

            Dim mostCommonRate As TimeSpan = audioDurationsFound.First.Key
            For Each entry As KeyValuePair(Of TimeSpan, Integer) In audioDurationsFound
                If entry.Value > audioDurationsFound(mostCommonRate) Then
                    mostCommonRate = entry.Key
                End If
            Next

            Return mostCommonRate
        End Function

        Public Enum TrackSyncStyle
            None 'Night Trap MCD, Make My Video, Corpse Killer MCD
            LocalTimecode 'Prize Fighter, Supreme Warrior
            GlobalTimecode 'Sewer Shark MCD and 3DO
            FrameNumber 'Ground Zero: Texas, Double Switch
            AnimationID 'Corpse Killer zombie animations
        End Enum

        Private _syncStyle As TrackSyncStyle _
            = TrackSyncStyle.None
        Public Property SyncStyle As TrackSyncStyle
            Get
                If TrackIDs.Count = 1 Then
                    _syncStyle = TrackSyncStyle.None
                Else
                    _syncStyle = TrackSyncStyle.LocalTimecode
                    Dim videoChunks As IEnumerable(Of VideoChunk) _
                        = File.Chunks.OfType(Of VideoChunk)

                    For Each videoChunk As VideoChunk In videoChunks
                        If videoChunk.Timecode > &H1000000 Then
                            If videoChunk.Type <> ChunkType.VideoD5 Then
                                _syncStyle = TrackSyncStyle.GlobalTimecode
                                Exit For
                            End If
                        ElseIf (videoChunk.Timecode And 255) > 29 Then
                            _syncStyle = TrackSyncStyle.FrameNumber
                            Exit For
                        End If
                    Next
                End If

                If File.TypeIDs.Contains(ChunkType.VideoD4) Then
                    _videoRate = 12
                    _syncStyle = TrackSyncStyle.AnimationID
                End If

                If _SGAFile.Format = File.FileFormat.Saturn Then
                    _syncStyle = TrackSyncStyle.None
                End If

                Return _syncStyle
            End Get
            Private Set(value As TrackSyncStyle)
                _syncStyle = value
            End Set
        End Property

        Public Property VideoZOrder As Integer()

        Sub New(ByRef file As File)
            Me.File = file
            BuildClips()
            BuildFrameList(0)
        End Sub

        Private Sub BuildClips()
            Debug.WriteLine($"Start of debug output for {[GetType].Name}.{Reflection.MethodBase.GetCurrentMethod().Name}")
            Debug.Indent()

            Clips = New List(Of Clip)

            Dim trackIDSortedChunks As Dictionary(Of Integer, List(Of Chunk)) _
                = SortChunksByTrackID(File.Chunks, TrackIDs)

            'Dim distinctTimecodes As New List(Of UInteger)
            'distinctTimecodes.AddRange(Timecodes.Distinct)

            Dim audioRate As Single _
                = GetMostCommonAudioRate(File.Chunks.OfType(Of AudioChunk))
            Dim videoRate As Double _
                = GetMostCommonVideoRate(File.Chunks.OfType(Of AudioChunk))

            Debug.Write("Media is from ")

            Select Case SyncStyle

                Case TrackSyncStyle.None 'NIGHT TRAP, CORPSE KILLER, MAKE MY VIDEO, SLAM CITY, KIDS ON SITE
                    Debug.WriteLine("Night Trap MCD, Corpse Killer, Make My Video, Slam City, Kids on Site, or a Saturn port")

                    '1. Create a single clip and add it to the clip list

                    Dim chunkSource As IEnumerable(Of Chunk) _
                        = trackIDSortedChunks(TrackIDs.First)

                    Dim clip As New Clip(audioRate, videoRate)

                    With clip
                        .AddRange(MultiplexChunks(chunkSource))
                        .TrackID = TrackIDs.First
                        .StartingFrameNumber = 0
                    End With

                    Clips.Add(clip)

                Case TrackSyncStyle.LocalTimecode
                    'Else 'Prize Fighter, Supreme Warrior
                    Debug.WriteLine("Prize Fighter or Supreme Warrior MCD")

                    '1. Separate chunks into tracks
                    '2. Create clips per track (first track is main track)
                    '   a. End a clip when a chunk of type FF is encountered
                    '      OR when a timecode has a value less than the previous timecode
                    '   b. Set starting frame number to frame number of chunk in first track whose offset is one index lower
                    '3. Add clips to the clip list

                    Dim clip As New Clip(audioRate, videoRate)

                    For Each entry As KeyValuePair(Of Integer, List(Of Chunk)) In trackIDSortedChunks

                        Dim chunks As List(Of Chunk) _
                            = entry.Value
                        Dim clipChunks As New List(Of Chunk)
                        Dim splitHere As Boolean = False

                        For Each chunk As Chunk In chunks

                            clipChunks.Add(chunk)
                            If chunk Is chunks.Last Then
                                splitHere = True
                            Else
                                Dim nextChunk As Chunk _
                                    = chunks(chunks.IndexOf(chunk) + 1)

                                Dim frameNumber As Integer _
                                    = New SMPTETimecode(chunk.Timecode, 30).FrameNumber
                                Dim nextFrameNumber As Integer _
                                    = New SMPTETimecode(nextChunk.Timecode, 30).FrameNumber

                                If nextFrameNumber < frameNumber Then
                                    splitHere = True
                                ElseIf Math.Abs(nextFrameNumber - frameNumber) > 3 Then
                                    splitHere = True
                                End If
                            End If

                            If splitHere = True Then
                                With clip
                                    .AddRange(MultiplexChunks(clipChunks))
                                    .TrackID = entry.Key

                                    If Clips.Count = 0 Then
                                        .StartingFrameNumber = 0
                                    Else
                                        Dim chunkIndex As Integer _
                                            = File.Offsets.IndexOf(clip.First.Chunks(True).First.Offset)

                                        If chunkIndex = 0 Then
                                            .StartingFrameNumber = 1
                                        Else
                                            Dim prevChunk As Chunk _
                                               = File.Chunks(chunkIndex - 1)

                                            Dim firstClip As Clip = Clips.First
                                            For Each frame As Frame In firstClip
                                                If frame.Chunks(True).Contains(prevChunk) Then
                                                    .StartingFrameNumber = firstClip.StartingFrameNumber + firstClip.IndexOf(frame)
                                                    Exit For
                                                End If
                                            Next
                                        End If

                                    End If

                                End With
                                Clips.Add(clip)
                                clip = New Clip(audioRate, videoRate)
                                clipChunks.Clear()
                                splitHere = False
                            End If

                        Next

                    Next

                Case TrackSyncStyle.GlobalTimecode 'Sewer Shark MCD, Sewer Shark 3DO
                    Debug.WriteLine("Sewer Shark MCD or 3DO")

                    '1. Separate chunks into tracks
                    '2. Create clips per track
                    '   a. Start a new clip when there is a gap between once timecode and the next
                    '   b. Set starting frame number to index of timecode of chunks in first frame
                    '3. Add clips to the clip list

                    For Each trackID As Integer In TrackIDs
                        Dim chunks As IEnumerable(Of Chunk) _
                            = File.Chunks.OfType(Of Chunk).Where(Function(chunk As Chunk) chunk.TrackID = trackID)
                        Clips.AddRange(SplitClipsByFrameDelta(chunks, 3))
                    Next

                Case TrackSyncStyle.FrameNumber
                    'If SGAFile.Version = File.FileVersionValue.EarlyMegaCDor32X Then 'GROUND ZERO: TEXAS, DOUBLE SWITCH (Sega CD)
                    Debug.WriteLine("Ground Zero: Texas or Double Switch MCD")

                    '1. Separate chunks into tracks
                    '2. Create clips per track
                    '   a. Start a new clip when a timecode has a value lower than the timecode of the previous frame
                    '   b. Set starting frame number to frame number of chunk in previous track whose offset is one index lower
                    '3. Add clips to the clip list

                    Dim clip As New Clip(audioRate, videoRate)

                    For Each entry As KeyValuePair(Of Integer, List(Of Chunk)) In trackIDSortedChunks

                        Dim chunks As List(Of Chunk) _
                            = entry.Value

                        With clip
                            .AddRange(MultiplexChunks(chunks))
                            .TrackID = entry.Key
                            If Clips.Count = 0 Then
                                .StartingFrameNumber = 0
                            Else
                                Dim videochunks As IEnumerable(Of VideoChunk) _
                                    = chunks.OfType(Of VideoChunk)
                                Dim firstVideoFrameNumber As Integer _
                                    = videochunks.First.Timecode
                                .StartingFrameNumber = firstVideoFrameNumber
                            End If
                        End With

                        Clips.Add(clip)
                        clip = New Clip(audioRate, videoRate)

                    Next

                Case TrackSyncStyle.AnimationID
                    Debug.WriteLine("an animation")

                    Dim clip As New Clip(audioRate, videoRate)

                    For Each entry As KeyValuePair(Of Integer, List(Of Chunk)) In trackIDSortedChunks

                        Dim chunks As New List(Of Chunk)
                        Dim splitHere As Boolean = False

                        For Each chunk As IAnimationChunk In entry.Value
                            chunks.Add(chunk)
                            If chunk Is entry.Value.Last Then
                                splitHere = True
                            Else
                                Dim nextChunk As IAnimationChunk _
                                    = entry.Value(entry.Value.IndexOf(chunk) + 1)
                                If nextChunk.AnimationID <> chunk.AnimationID Then
                                    splitHere = True
                                End If
                            End If

                            If splitHere = True Then
                                With clip
                                    .AddRange(MultiplexChunks(chunks))
                                    .TrackID = entry.Key
                                    .StartingFrameNumber = 0
                                End With

                                Clips.Add(clip)
                                clip = New Clip(audioRate, videoRate)
                                chunks.Clear()
                                splitHere = False

                            End If

                        Next

                    Next

            End Select

            Debug.Unindent()
            Debug.WriteLine($"End of debug output for {[GetType].Name}.{Reflection.MethodBase.GetCurrentMethod().Name}")

        End Sub

        Private Function SplitClipsByFrameDelta(ByRef chunks As IEnumerable(Of Chunk), ByVal frameDeltaMax As Integer) As Clip()
            Dim clip As New Clip(AudioRate, VideoRate)
            Dim clips As New List(Of Clip)

            Dim clipChunks As New List(Of Chunk)
            Dim splitHere As Boolean = False

            Dim distinctTimecodes As New List(Of UInteger)
            distinctTimecodes.AddRange(Timecodes.Distinct)

            Dim prevTimecode As UInteger
            Dim prevFrameNumber As Integer

            Dim frameDelta As Integer

            For Each chunk As Chunk In chunks
                Dim frameNumber As Integer _
                    = New SMPTETimecode(chunk.Timecode, 30).FrameNumber

                If (chunk Is chunks.First) Then
                    splitHere = False
                ElseIf (chunk Is chunks.Last) Then
                    clipChunks.Add(chunk)
                    splitHere = True
                Else
                    frameDelta = frameNumber - prevFrameNumber
                    If (frameDelta > frameDeltaMax) _
                    Or (frameDelta < 0) Then '(frameDelta <= 0)
                        splitHere = True
                    End If
                End If

                prevTimecode = chunk.Timecode
                prevFrameNumber = frameNumber

                If splitHere = True Then
                    If clipChunks.Count > 0 Then
                        With clip
                            .AddRange(MultiplexChunks(clipChunks))
                            .TrackID = chunk.TrackID
                            .StartingFrameNumber = distinctTimecodes.IndexOf(clipChunks.First.Timecode)
                        End With
                        clips.Add(clip)
                    End If

                    clip = New Clip(AudioRate, VideoRate)
                    clipChunks.Clear()
                    splitHere = False
                Else
                    clipChunks.Add(chunk)
                End If

            Next

            Return clips.ToArray
        End Function

        Private Function MultiplexChunks(ByRef chunks As IEnumerable(Of Chunk), Optional ByVal ticks As Long = 0) As Frame()
            Dim duration As New TimeSpan(ticks)

            Dim frames As New List(Of Frame)
            Dim frame As New Frame(duration)

            Dim lastChunk As Chunk = Nothing

            For Each chunk As Chunk In chunks
                If lastChunk IsNot Nothing Then
                    If (TypeOf (chunk) Is ContainerChunk) _
                    Or (TypeOf (chunk) Is VideoChunk) _
                    Or (TypeOf (chunk) Is AudioChunk And TypeOf (lastChunk) Is AudioChunk) Then
                        frames.Add(frame)
                        frame = New Frame(duration)
                    End If
                End If

                frame.Chunks.Add(chunk)
                lastChunk = chunk
            Next

            If frame.Chunks.Count > 0 Then
                frames.Add(frame)
            End If

            Return frames.ToArray

        End Function

        Private _frameList As List(Of Frame)
        Public Property FrameList As List(Of Frame)
            Get
                Return _frameList
            End Get
            Private Set(value As List(Of Frame))
                _frameList = value
            End Set
        End Property

        Public Sub BuildFrameList(ByVal clipIndex As Integer)
            BuildFrameListFromClips({clipIndex})
        End Sub

        Public Sub BuildFrameListFromClips(ByVal clipIndexes() As Integer)
            Dim clipList As New List(Of Clip)
            For Each clipIndex As Integer In clipIndexes
                clipList.Add(Clips(clipIndex))
            Next

            'Added on 2015/11/4: Support for filtering by clip index
            If clipList IsNot Nothing Then
                If clipList.Count > 0 Then

                    Dim audioChunks As IEnumerable(Of AudioChunk) _
                        = File.Chunks.OfType(Of AudioChunk)

                    Dim duration As TimeSpan _
                        = CalculateFrameListDuration(audioChunks)

                    'Select Case SyncStyle
                    '    Case TrackSyncStyle.None, TrackSyncStyle.LocalTimecode

                    '    Case TrackSyncStyle.GlobalTimecode
                    Dim lengthInFrames As Integer = 0
                    Dim startingFrameNumberMin As Integer = Integer.MaxValue
                    For Each clip In clipList
                        With clip
                            lengthInFrames = Math.Max(lengthInFrames, .StartingFrameNumber + .Count)
                            startingFrameNumberMin = Math.Min(startingFrameNumberMin, .StartingFrameNumber)
                        End With
                    Next

                    Dim frames(lengthInFrames - startingFrameNumberMin - 1) As Frame
                    For i As Integer = 0 To frames.Length - 1
                        frames(i) = New Frame(duration)
                    Next

                    Dim frameIndex As Integer

                    For Each clip In clipList
                        For Each frame As Frame In clip
                            With clip
                                frameIndex = .StartingFrameNumber - startingFrameNumberMin + .IndexOf(frame)
                                frames(frameIndex).Chunks.AddRange(frame.Chunks)
                            End With
                        Next
                    Next

                    FrameList = New List(Of Frame)
                    FrameList.AddRange(frames)

                    '    Case TrackSyncStyle.AnimationID

                    'End Select

                End If
            End If

        End Sub

        Public Sub BuildFrameListFromChunks(ByVal chunkIndexes As Integer())
            Dim chunkList As New List(Of Chunk)
            For Each chunkIndex As Long In chunkIndexes
                chunkList.Add(File.Chunks(chunkIndex))
            Next

            Dim audioChunks As IEnumerable(Of AudioChunk) _
                = File.Chunks.OfType(Of AudioChunk)

            Dim duration As TimeSpan _
                = CalculateFrameListDuration(audioChunks)

            FrameList = New List(Of Frame)
            FrameList.AddRange(MultiplexChunks(chunkList, duration.Ticks))

        End Sub

        Private Function CalculateFrameListDuration(ByRef audioChunks As IEnumerable(Of AudioChunk)) As TimeSpan
            Dim mostCommonDuration As TimeSpan _
                = GetMostCommonFrameDuration(audioChunks)

            Dim frameRate As Double
            If mostCommonDuration.Ticks = 0 Then
                frameRate = 12
            Else
                frameRate = Math.Round(durationToFrameRate(mostCommonDuration))
            End If

            Dim duration As TimeSpan _
                = frameRateToDuration(frameRate)

            Return duration
        End Function

        Shared Function frameRateToDuration(ByVal frameRate As Double) As TimeSpan
            If frameRate <= 0 Then Return New TimeSpan(0)
            Return New TimeSpan(Math.Round(((1 / frameRate) * TimeSpan.TicksPerSecond)))
        End Function

        Shared Function durationToFrameRate(ByVal duration As TimeSpan) As Double
            If duration.TotalSeconds = 0 Then Return 0
            Return 1 / duration.TotalSeconds
        End Function

        Private Function SortChunksByOffset(ByRef chunks As List(Of Chunk)) As SortedDictionary(Of Long, Chunk)
            Dim sortedChunks As New SortedDictionary(Of Long, Chunk)
            For Each chunk As Chunk In chunks
                sortedChunks.Add(chunk.Offset, chunk)
            Next
            Return sortedChunks
        End Function

        Private Function SortChunksByTrackID(ByRef chunks As List(Of Chunk), ByVal trackIDs As IEnumerable(Of Integer)) As Dictionary(Of Integer, List(Of Chunk))
            Dim sortedChunks As New Dictionary(Of Integer, List(Of Chunk))
            Dim filteredChunks As IEnumerable(Of Chunk)
            For Each trackID As Integer In trackIDs
                filteredChunks = chunks.Where(Function(chunk As Chunk) chunk.TrackID = trackID)
                sortedChunks.Add(trackID, filteredChunks.ToList)
            Next
            Return sortedChunks
        End Function

        Private Function SortChunksByTimecode(ByRef chunks As List(Of Chunk), ByVal timecodes As UInteger()) As SortedDictionary(Of UInteger, List(Of Chunk))
            Dim sortedChunks As New SortedDictionary(Of UInteger, List(Of Chunk))
            Dim filteredChunks As IEnumerable(Of Chunk)
            For Each timecode As UInteger In timecodes
                filteredChunks = chunks.Where(Function(chunk As Chunk) chunk.Timecode = timecode)
                sortedChunks.Add(timecode, filteredChunks.ToList)
            Next
            Return sortedChunks
        End Function

        Public Function GetFrameInfoAsString(ByVal frameNumber As Integer, Optional ByVal includeData As Boolean = False) As String
            Dim sbFrameInfo As New Text.StringBuilder()

            Dim frame As Frame _
                = FrameList(frameNumber)

            If frame.VideoChunks.Count > 0 Then
                sbFrameInfo.AppendLine("Video:")
                For Each vChunk As VideoChunk In frame.VideoChunks
                    sbFrameInfo.AppendLine(vChunk.InfoToString(includeData))
                Next
                sbFrameInfo.AppendLine()
            End If

            If frame.AudioChunks.Count > 0 Then
                sbFrameInfo.AppendLine("Audio:")
                For Each aChunk As AudioChunk In frame.AudioChunks
                    sbFrameInfo.AppendLine(aChunk.InfoToString(includeData))
                Next
                sbFrameInfo.AppendLine()
            End If

            Return sbFrameInfo.ToString
        End Function

        'Added on 2016/6/4: hiding and muting of clips
        Public Sub MakeClipVisible(ByVal index As Integer)
            Clips(index).Visible = True
        End Sub

        Public Sub MakeAllClipsVisible()
            For i As Integer = 0 To Clips.Count - 1
                MakeClipVisible(i)
            Next
        End Sub

        Public Sub MakeClipAudible(ByVal index As Integer)
            Clips(index).Audible = True
        End Sub

        Public Sub MakeAllClipsAudible()
            For i As Integer = 0 To Clips.Count - 1
                MakeClipAudible(i)
            Next
        End Sub

        Public Sub MakeClipInvisible(ByVal index As Integer)
            Clips(index).Visible = False
        End Sub

        Public Sub MakeAllClipsInvisible()
            For i As Integer = 0 To Clips.Count - 1
                MakeClipInvisible(i)
            Next
        End Sub

        Public Sub MakeClipInaudible(ByVal index As Integer)
            Clips(index).Audible = False
        End Sub

        Public Sub MakeAllClipsInaudible()
            For i As Integer = 0 To Clips.Count - 1
                MakeClipInaudible(i)
            Next
        End Sub

    End Class

End Namespace