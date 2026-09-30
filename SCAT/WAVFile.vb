Imports System.IO

Public Class WAVFile

    ' VERSION HISTORY
    '
    ' VERSION 0.2 (2013/10/29)
    ' - File writing now uses a BinaryWriter instead of old FilePut, etc.
    '
    ' VERSION 0.1 (????/??/??)
    ' - First version

    Const WAVE_FORMAT_UNKNOWN As Short = &H0S               'Unknown Format
    Const WAVE_FORMAT_PCM As Short = &H1S                   'PCM
    Const WAVE_FORMAT_ADPCM As Short = &H2S                 'Microsoft ADPCM Format
    Const WAVE_FORMAT_IEEE_FLOAT As Short = &H3S            'IEEE Float
    Const WAVE_FORMAT_EXTENSIBLE As Short = &HFFFES

    Const SPEAKER_FRONT_LEFT As Integer = &H1I
    Const SPEAKER_FRONT_RIGHT As Integer = &H2I
    Const SPEAKER_FRONT_CENTER As Integer = &H4I
    Const SPEAKER_LOW_FREQUENCY As Integer = &H8I
    Const SPEAKER_BACK_LEFT As Integer = &H10I
    Const SPEAKER_BACK_RIGHT As Integer = &H20I
    Const SPEAKER_FRONT_LEFT_OF_CENTER As Integer = &H40I
    Const SPEAKER_FRONT_RIGHT_OF_CENTER As Integer = &H80I
    Const SPEAKER_BACK_CENTER As Integer = &H100I
    Const SPEAKER_SIDE_LEFT As Integer = &H200I
    Const SPEAKER_SIDE_RIGHT As Integer = &H400I
    Const SPEAKER_TOP_CENTER As Integer = &H800I
    Const SPEAKER_TOP_FRONT_LEFT As Integer = &H1000I
    Const SPEAKER_TOP_FRONT_CENTER As Integer = &H2000I
    Const SPEAKER_TOP_FRONT_RIGHT As Integer = &H4000I
    Const SPEAKER_TOP_BACK_LEFT As Integer = &H8000I
    Const SPEAKER_TOP_BACK_CENTER As Integer = &H10000I
    Const SPEAKER_TOP_BACK_RIGHT As Integer = &H20000I
    Const SPEAKER_RESERVED As Integer = &H80000000I

    Structure RIFFFILEHEADER 'Len = 8
        Dim RiffID As String '="RIFF"
        Dim FileLength As Integer
    End Structure

    Structure RIFFFILEFORMATHEADER 'Len = 4
        Dim FormatID As String
    End Structure

    Structure RIFFCHUNKHEADER 'Len = 8
        Dim ChunkID As String
        Dim ChunkLength As Integer
    End Structure

    Structure WAVEFORMATEX 'Len = 18
        Dim FormatTag As Short
        Dim Channels As Short
        Dim SamplesPerSec As Integer
        Dim AvgBytesPerSec As Integer
        Dim BlockAlign As Short
        Dim BitsPerSample As Short
        Dim Size As Short
    End Structure

    Structure WAVEFORMATEXTENSIBLE 'Len = 22
        Dim ValidBitsPerSample As Short
        Dim ChannelMask As Integer
        Dim SubFormat As Guid
    End Structure

    Public mBitsPerSample As Short
    Public mValidBitsPerSample As Short
    Public mSamplesPerSec As Integer
    Public mChannels As Short
    Private mSampleData8Bit As Byte()
    Private mSampleData16Bit As Short()

    'Private mChannelData As List(Of Byte())

    Public ReadOnly Property MetadataLength As Long
        Get
            Return 38
        End Get
    End Property

    Public ReadOnly Property SampleDataLength As Long
        Get
            Select Case mBitsPerSample
                Case 16
                    Return mSampleData16Bit.LongLength
                Case Else
                    Return mSampleData8Bit.LongLength
            End Select
        End Get
    End Property

    Sub New()

    End Sub

    Sub New(ByVal bitsPerSample As Short, ByVal validBitsPerSample As Short, ByVal samplesPerSecond As Integer)
        mBitsPerSample = bitsPerSample
        mValidBitsPerSample = validBitsPerSample
        mSamplesPerSec = samplesPerSecond
    End Sub

    Sub New(ByVal bitsPerSample As Short, ByVal validBitsPerSample As Short, ByVal samplesPerSecond As Integer, ByVal numChannels As Short)
        Me.New(bitsPerSample, validBitsPerSample, samplesPerSecond)
        mChannels = numChannels
    End Sub

    Sub New(ByVal samplesPerSecond As Integer, ByRef sampleData As Byte())
        Me.New(8, 8, samplesPerSecond, 1)
        mSampleData8Bit = sampleData.Clone
    End Sub

    Sub New(ByVal samplesPerSecond As Integer, ByRef sampleData As Short())
        Me.New(16, 16, samplesPerSecond, 1)
        mSampleData16Bit = sampleData.Clone
    End Sub

    Public Sub Save(ByRef stream As Stream)
        Dim fileHeader As New RIFFFILEHEADER()
        Dim fileFormatHeader As New RIFFFILEFORMATHEADER()
        Dim formatChunkHeader As New RIFFCHUNKHEADER()
        Dim dataChunkHeader As New RIFFCHUNKHEADER()
        Dim wfEx As New WAVEFORMATEX()
        'Dim wfExt As New WAVEFORMATEXTENSIBLE()

        'With wfExt
        '    .ValidBitsPerSample = mValidBitsPerSample
        '    .ChannelMask = SPEAKER_FRONT_LEFT + SPEAKER_FRONT_RIGHT
        '    .SubFormat = New Guid("00000001-0000-0010-8000-00aa00389b71")
        'End With

        With wfEx
            '.wFormatTag = WAVE_FORMAT_EXTENSIBLE
            .FormatTag = WAVE_FORMAT_PCM
            .BitsPerSample = mBitsPerSample
            .SamplesPerSec = mSamplesPerSec
            .Channels = mChannels
            .BlockAlign = (.BitsPerSample * .Channels) \ 8
            .AvgBytesPerSec = .BlockAlign * .SamplesPerSec
            .Size = 0 'Len(wfExt)
        End With

        Dim sampleDataLen As Integer = (SampleDataLength - 1) * wfEx.BlockAlign

        With dataChunkHeader
            .ChunkID = "data" '&H61746164I
            .ChunkLength = sampleDataLen
        End With

        With fileFormatHeader
            .FormatID = "WAVE" '&H45564157I 
        End With

        With formatChunkHeader
            .ChunkID = "fmt " '&H20746D66I 
            '.ChunkLength = Len(wfEx) + Len(wfExt)
            .ChunkLength = Len(wfEx)
        End With

        With fileHeader
            .RiffID = "RIFF" '&H46464952I
            '.lFileSize = Len(fileHeader) + Len(formatHeader) + Len(wfEx) + Len(wfExt) + Len(chunkHeader) + sampleDataLen - Len(fileHeader)
            .FileLength = Len(fileHeader) + Len(fileFormatHeader) + Len(formatChunkHeader) + Len(wfEx) + Len(dataChunkHeader) + sampleDataLen - Len(fileHeader)
        End With

        'Using bw As New BinaryWriter(sOut)
        Dim bw As New BinaryWriter(stream)

        With fileHeader
            bw.Write(.RiffID.ToArray)
            bw.Write(.FileLength)
        End With
        With fileFormatHeader
            bw.Write(.FormatID.ToArray)
        End With
        With formatChunkHeader
            bw.Write(.ChunkID.ToArray)
            bw.Write(.ChunkLength)
        End With
        With wfEx
            bw.Write(.FormatTag)
            bw.Write(.Channels)
            bw.Write(.SamplesPerSec)
            bw.Write(.AvgBytesPerSec)
            bw.Write(.BlockAlign)
            bw.Write(.BitsPerSample)
            bw.Write(.Size)
        End With
        'With wfxtWaveFormatExtensible
        '    bw.Write(.ValidBitsPerSample)
        '    bw.Write(.ChannelMask)
        '    Dim sf As Byte() = .SubFormat.ToByteArray
        '    bw.Write(sf)
        'End With
        With dataChunkHeader
            bw.Write(.ChunkID.ToArray)
            bw.Write(.ChunkLength)
        End With

        If mSampleData8Bit IsNot Nothing Then
            bw.Write(mSampleData8Bit)
        ElseIf mSampleData16Bit IsNot Nothing Then
            For i As Long = 0 To mSampleData16Bit.LongCount - 1
                bw.Write(BitConverter.GetBytes(mSampleData16Bit(i)))
            Next
        End If

        'End Using

    End Sub

    Public Sub Save(ByVal path As String)
        Using fs As New FileStream(path, FileMode.Create)
            Save(fs)
        End Using
    End Sub

End Class
