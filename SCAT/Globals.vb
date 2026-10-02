Imports System.Text
Imports System.Windows.Media.Imaging

Module Globals

    'Constants for reading CD-ROM based data
    Public Const CD_ROM_MODE1_BYTES_PER_SECTOR As Integer = 2048
    Public Const CD_ROM_BYTES_PER_SECOND As Integer = 153600

    Public Const CD_ROM_SECTORS_PER_SECOND As Integer = 75
    Public Const CD_ROM_MILLISECONDS_PER_SECTOR As Double = 1000 / CD_ROM_SECTORS_PER_SECOND

    Public Const CD_ROM_BYTES_PER_MILLISECOND As Double = CD_ROM_BYTES_PER_SECOND / 1000
    Public Const CD_ROM_MILLISECONDS_PER_BYTE As Double = 1 / CD_ROM_BYTES_PER_MILLISECOND

    Public Const CD_ROM_SECTORS_PER_MILLISECOND As Double = CD_ROM_SECTORS_PER_SECOND / 1000

    'Public Const CD_ROM_SECTORS_PER_MILLISECOND As Single = CD_ROM_BYTES_PER_SECTOR / CD_ROM_BYTES_PER_MILLISECOND
    'Public Const CD_ROM_MILLISECONDS_PER_SECTOR As Single = 1 / CD_ROM_SECTORS_PER_MILLISECOND

    'Constants for files with multiple audio/video streams
    Public Const NUMBER_OF_SIMULTANEOUS_STREAMS_MAX = 16

    'Constants for aspect ratio conversion
    Public Const ASPECT_CORRECTION_RATIO_NONE As Single = 1
    Public Const ASPECT_CORRECTION_RATIO_NTSC_4_3 As Single = 10 / 11
    Public Const ASPECT_CORRECTION_RATIO_PAL_4_3 As Single = 11 / 12

    'Public Function ValueToBinaryString(ByVal value As ULong)
    '    Dim sb As New StringBuilder
    '    For Each b As Byte In BitConverter.GetBytes(value)
    '        sb.Append(Convert.ToString(b, 2).PadLeft(8, "0"))
    '    Next
    '    Return sb.ToString
    'End Function

    'Public Function BinaryStringToValue(ByVal binaryString As String) As ULong
    '    Dim byteList As New List(Of Byte)
    '    For i As Integer = 0 To binaryString.Length Step 8
    '        byteList.Add(Convert.ToByte(binaryString.Substring(i, 8), 2))
    '    Next
    '    Return BitConverter.ToUInt64(byteList.ToArray, 0)
    'End Function

    Public Function DataToString(ByRef data As Byte(), ByVal startOffset As Integer, Optional ByVal entriesPerRow As Integer = 16) As String
        Dim sb As New StringBuilder((data.Length - startOffset) * 2)
        'Dim dataString As String = ""

        With sb
            For i As Integer = startOffset To data.Length - 1
                .Append($"{data(i):X2}")
                'dataString &= $"{data(i):X2}"

                If i - startOffset < data.Length - 1 Then
                    If (i - startOffset) Mod entriesPerRow = entriesPerRow - 1 Then
                        .AppendLine()
                    Else
                        .Append(" ")
                    End If
                End If

            Next

            Return .ToString

        End With

    End Function

    Public Function DataToString(ByRef data As UShort(), ByVal startOffset As Integer, Optional ByVal entriesPerRow As Integer = 16) As String
        Dim dataString As String = ""
        For i As Integer = startOffset To data.Length - 1
            dataString &= $"{data(i):X4}"
            If i - startOffset < data.Length - 1 Then
                If (i - startOffset) Mod entriesPerRow = entriesPerRow - 1 Then
                    dataString &= vbCrLf
                Else
                    dataString &= " "
                End If
            End If
        Next
        Return dataString
    End Function

    Public Class SMPTETimecode
        Private _hours As Byte
        Public ReadOnly Property Hours As Byte
            Get
                Return _hours
            End Get
        End Property

        Private _minutes As Byte
        Public ReadOnly Property Minutes As Byte
            Get
                Return _minutes
            End Get
        End Property

        Private _seconds As Byte
        Public ReadOnly Property Seconds As Byte
            Get
                Return _seconds
            End Get
        End Property

        Private _frames As Byte
        Public ReadOnly Property Frames As Byte
            Get
                Return _frames
            End Get
        End Property

        Private _fps As Double
        Public ReadOnly Property FPS As Double
            Get
                Return _fps
            End Get
        End Property

        Public ReadOnly Property FrameNumber As Integer
            Get
                Return (CInt(Hours) * 360 * FPS) + (CInt(Minutes) * 60 * FPS) + (CInt(Seconds) * FPS) + CInt(Frames)
            End Get
        End Property

        Sub New(ByVal hours As Byte, ByVal minutes As Byte, ByVal seconds As Byte, ByVal frames As Byte, ByVal fps As Double)
            _hours = hours
            _minutes = minutes
            _seconds = seconds
            _frames = frames
            _fps = fps
        End Sub

        Sub New(ByVal timecode As UInteger, ByVal fps As Double)
            '_hours = CByte(timecode >> 24)
            '_minutes = CByte((timecode >> 16) And 255)
            '_seconds = CByte((timecode >> 8) And 255)
            '_frames = CByte(timecode And 255)
            '_fps = fps

            Dim bytes As Byte() _
                = BitConverter.GetBytes(timecode)
            Array.Reverse(bytes)
            _hours = bytes(0)
            _minutes = bytes(1)
            _seconds = bytes(2)
            _frames = bytes(3)
            _fps = fps
        End Sub

        Sub New(ByVal frameNumber As Integer, ByVal fps As Double)
            _frames = frameNumber Mod fps
            _seconds = (frameNumber \ fps) Mod 60
            _minutes = (frameNumber \ (fps * 60)) Mod (60 ^ 2)
            _hours = (frameNumber \ (fps * (60 ^ 2))) Mod (60 ^ 3)
            _fps = fps
        End Sub

        Public Overrides Function ToString() As String
            Return $"{_hours:00}:{_minutes:00}:{_seconds:00}:{_frames:00}"
        End Function

        Public Overloads Function ToString(ByVal showHours As Boolean) As String
            Return $"{If(showHours, $"{_hours:00}:", "")}{_minutes:00}:{_seconds:00}:{_frames:00}"
        End Function

    End Class

    'Public Function GetFormattedSMTPETimecode(ByRef timeCodeSMTPE As UInteger) As String
    '    Dim hh, mm, ss, ff As UInteger

    '    hh = (timeCodeSMTPE And &HFF000000) >> 24
    '    mm = (timeCodeSMTPE And &HFF0000) >> 16
    '    ss = (timeCodeSMTPE And &HFF00) >> 8
    '    ff = timeCodeSMTPE And &HFF

    '    Return $"{hh:00}:{mm:00}:{ss:00}:{ff:00}"
    'End Function

    'Public Function GetFormattedSMTPETimecode(ByVal frameNumber As Integer, ByVal fps As Single) As String
    '    Dim hh, mm, ss, ff As UInteger

    '    ff = frameNumber Mod fps
    '    ss = (frameNumber \ fps) Mod 60
    '    mm = (frameNumber \ (fps * 60)) Mod (60 ^ 2)
    '    hh = (frameNumber \ (fps * (60 ^ 2))) Mod (60 ^ 3)

    '    Return $"{hh:00}:{mm:00}:{ss:00}:{ff:00}"
    'End Function

    ''' <summary>
    ''' Gets the value of a string of bits contained in an unsigned integral value
    ''' </summary>
    ''' <param name="value">The value from which to get the bits</param>
    ''' <param name="index">The zero-based start index of the uppermost bit to get</param>
    ''' <param name="count">The number of bits to get</param>
    ''' <returns>The value of the string of bits</returns>
    ''' <remarks>Return values are shifted all the way to the right.</remarks>
    Public Function GetBits(ByVal count As Integer, ByVal value As ULong, ByVal index As Integer) As ULong
        index = index Mod 64
        count = count Mod 64
        Return CULng(value >> (index - (count - 1)) And ((2 ^ count) - 1))
    End Function

    ''' <summary>
    ''' Gets the value of a string of bits contained in an unsigned integral value
    ''' </summary>
    ''' <param name="value">The value from which to get the bits</param>
    ''' <param name="index">The zero-based start index of the uppermost bit to get</param>
    ''' <param name="count">The number of bits to get</param>
    ''' <returns>The value of the string of bits</returns>
    ''' <remarks>Return values are shifted all the way to the right.</remarks>
    Public Function GetBits(ByVal count As Integer, ByVal value As UInteger, ByVal index As Integer) As UInteger
        index = index Mod 32
        count = count Mod 32
        Return CUInt(GetBits(count, CULng(value), index))
    End Function

    ''' <summary>
    ''' Gets the value of a string of bits contained in an unsigned integral value
    ''' </summary>
    ''' <param name="value">The value from which to get the bits</param>
    ''' <param name="index">The zero-based start index of the uppermost bit to get</param>
    ''' <param name="count">The number of bits to get</param>
    ''' <returns>The value of the string of bits</returns>
    ''' <remarks>Return values are shifted all the way to the right.</remarks>
    Public Function GetBits(ByVal count As Integer, ByVal value As UShort, ByVal index As Integer) As UShort
        index = index Mod 16
        count = count Mod 16
        Return CUShort(GetBits(count, CULng(value), index))
    End Function

    ''' <summary>
    ''' Gets the value of a string of bits contained in an unsigned integral value
    ''' </summary>
    ''' <param name="value">The value from which to get the bits</param>
    ''' <param name="index">The zero-based start index of the uppermost bit to get</param>
    ''' <param name="count">The number of bits to get</param>
    ''' <returns>The value of the string of bits</returns>
    ''' <remarks>Return values are shifted all the way to the right.</remarks>
    Public Function GetBits(ByVal count As Integer, ByVal value As Byte, ByVal index As Integer) As Byte
        index = index Mod 8
        count = count Mod 8
        Return CByte(GetBits(count, CULng(value), index))
    End Function

    Public Function GetLowerBits(ByVal count As Integer, ByVal value As ULong) As ULong
        Return value And ((2 ^ count) - 1)
    End Function

    Public Function GetUpperBits(ByVal count As Integer, ByVal value As ULong) As ULong
        Return value >> (count - 1) And GetLowerBits(count, value)
    End Function

    'Private Function ByteToSByte(ByVal value As Byte) As SByte
    '    Return value - 128
    'End Function

    'Private Function SByteToByte(ByVal value As SByte) As Byte
    '    Return value + 128
    'End Function

    Public Class LZCompressedData
        Private _compressedData As Byte()
        Public ReadOnly Property CompressedData As Byte()
            Get
                Return _compressedData
            End Get
        End Property

        Private _wordSize As Integer = 2

        Private _blockFlagLengthInBits As Integer = 16
        Private _blockFlagLengthInBytes As Integer = Math.Ceiling(_blockFlagLengthInBits / 8)
        Private _blockDataLengthInBytes As Integer = _wordSize * _blockFlagLengthInBits
        Private _blockSize As Integer = _blockFlagLengthInBytes + _blockDataLengthInBytes

        Private _lengthBits As Integer
        Private _lengthMask As UShort
        Private _lengthBase As Integer

        Private _decompressedData As Byte()
        Public ReadOnly Property DecompressedData As Byte()
            Get
                If _decompressedData Is Nothing Then

                    Dim lengthRShift As UShort
                    For i As Integer = 0 To _blockFlagLengthInBits - 1
                        If (_lengthMask >> i) And 1 Then
                            lengthRShift = i
                            Exit For
                        End If
                    Next

                    Dim offsetMask As UShort _
                        = Not _lengthMask

                    Dim decompressedDataList As New List(Of Byte)

                    For i As Integer = 0 To _compressedData.Length - 1 Step _blockSize
                        Dim blockFlags As UShort _
                            = (CUShort(_compressedData(i)) * 256) + CUShort(_compressedData(i + 1))

                        Dim blockDataLength As Integer _
                            = Math.Min(_blockDataLengthInBytes, _compressedData.Count - (i + _blockFlagLengthInBytes))

                        Dim blockData(blockDataLength - 1) As Byte
                        Array.Copy(_compressedData, i + _blockFlagLengthInBytes, blockData, 0, blockDataLength)

                        For flagIndex As Integer = 0 To _blockFlagLengthInBits - 1 '(blockData.Count \ 2) - 1
                            Dim blockDataOffset As Integer _
                                = flagIndex * _wordSize

                            Dim blockFlag As Boolean _
                                = Convert.ToBoolean(blockFlags >> (_blockFlagLengthInBits - 1 - flagIndex) And 1)

                            Select Case blockFlag 'blockFlagsAsUShort >> (15 - flagIndex) And 1
                                Case False '0 'literal
                                    For j As Integer = 0 To 1
                                        decompressedDataList.Add(blockData(blockDataOffset + j))
                                    Next
                                Case True '1 'length-offset pair
                                    Dim lengthOffsetPair As UShort _
                                        = (CUShort(blockData(blockDataOffset)) * 256) + CUShort(blockData(blockDataOffset + 1))

                                    Dim length As Integer _
                                        = ((lengthOffsetPair And _lengthMask) >> lengthRShift)

                                    Dim offset As Integer _
                                        = lengthOffsetPair And offsetMask

                                    If length = 0 And offset = 0 Then
                                        'compression is finished -- read to end of data
                                        For j As Integer = i + _blockFlagLengthInBytes + blockDataOffset + _wordSize To _compressedData.Length - 1
                                            decompressedDataList.Add(_compressedData(j))
                                        Next
                                        i = _compressedData.Length
                                        Exit For
                                    Else
                                        length += _lengthBase

                                        ' HACK: to avoid out-of-bounds errors
                                        If offset <= 0 Then offset = 1

                                        For j As Integer = 0 To (length * _wordSize) - 1
                                            decompressedDataList.Add(decompressedDataList(decompressedDataList.Count - offset))
                                        Next
                                    End If

                            End Select
                        Next

                    Next

                    _decompressedData = decompressedDataList.ToArray

                End If

                Return _decompressedData

            End Get
        End Property

        Sub New(ByRef compressedData As Byte(), ByVal lengthMask As UShort, ByVal lengthBase As Integer)
            'ReDim _compressedData(compressedData.Length - 1)
            'Array.Copy(compressedData, _compressedData, compressedData.Length)
            _compressedData = compressedData.Clone

            _lengthMask = lengthMask
            _lengthBase = lengthBase
        End Sub

        Sub New(ByRef compressedData As Byte(), ByVal lengthBits As Integer, ByVal lengthBase As Integer)
            Me.New(compressedData, CUShort(0), lengthBase)

            lengthBits = Math.Max(lengthBits, 0)
            lengthBits = Math.Min(lengthBits, 16)

            _lengthBits = lengthBits

            For i As Integer = 0 To lengthBits - 1
                _lengthMask <<= 1
                _lengthMask += 1
            Next

            _lengthMask <<= 16 - lengthBits
        End Sub

    End Class

    Public Function CreateBitmapFromPixels(ByRef pixels As Byte(), ByVal size As Size, ByVal pixelFormat As Imaging.PixelFormat) As Bitmap

        Dim b As New Bitmap(size.Width, size.Height, pixelFormat)

        Dim bd As Imaging.BitmapData =
            b.LockBits(New Rectangle(0, 0, b.Width, b.Height),
            Imaging.ImageLockMode.ReadWrite,
            b.PixelFormat)

        Runtime.InteropServices.Marshal.Copy(pixels, 0, bd.Scan0, pixels.Length)

        b.UnlockBits(bd)

        Return b

    End Function

    Public Class BigEndian

        Shared Function Get16BitValueFromByteArray(ByRef source As Byte(), ByVal offset As Integer) As UShort
            Dim bytes As Byte() _
                = GetReversedBytesFromByteArray(source, offset, 2)
            Return BitConverter.ToUInt16(bytes, 0)
        End Function

        Shared Function Get32BitValueFromByteArray(ByRef source As Byte(), ByVal offset As Integer) As UInteger
            Dim bytes As Byte() _
                = GetReversedBytesFromByteArray(source, offset, 4)
            Return BitConverter.ToUInt32(bytes, 0)
        End Function

        Shared Function Get64BitValueFromByteArray(ByRef source As Byte(), ByVal offset As Integer) As ULong
            Dim bytes As Byte() _
                = GetReversedBytesFromByteArray(source, offset, 8)
            Return BitConverter.ToUInt64(bytes, 0)
        End Function

        Private Shared Function GetReversedBytesFromByteArray(ByRef source As Byte(), ByVal offset As Integer, ByVal length As Integer) As Byte()
            Dim bytes(length - 1) As Byte
            Array.Copy(source, offset, bytes, 0, length)
            Array.Reverse(bytes)
            Return bytes
        End Function

    End Class

End Module
