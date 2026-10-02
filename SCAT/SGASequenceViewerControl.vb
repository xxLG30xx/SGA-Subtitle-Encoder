Imports System.ComponentModel

Public Class SGASequenceViewerControl

    Private Property Sequence As SGA.Sequence

    Private Enum ViewModeValue As Integer
        Clips
        Chunks
    End Enum

    Private _viewMode As ViewModeValue = ViewModeValue.Clips
    Private Property ViewMode As ViewModeValue
        Get
            Return _viewMode
        End Get
        Set(value As ViewModeValue)
            _viewMode = value
            Select Case _viewMode
                Case ViewModeValue.Clips
                    _displayBitmap = _clipBitmap
                Case ViewModeValue.Chunks
                    _displayBitmap = _chunkBitmap
            End Select
            ViewModeChanged()
        End Set
    End Property

    Public Sub ViewModeChanged()
        UpdateDisplay()
    End Sub

    Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ToolStripMain.Renderer = New CustomToolStripRenderer
    End Sub

    Sub LoadSequence(ByRef sequence As SGA.Sequence)
        SuspendLayout()

        Me.Sequence = sequence

        'InitializeDisplay()
        CreateBitmaps()

        If tsmiClips.Checked = True Then
            ViewMode = ViewModeValue.Clips
        ElseIf tsmiChunks.Checked = True Then
            ViewMode = ViewModeValue.Chunks
        End If

        Dim changedRects As New List(Of SelectableRectangleF)

        If sequence.VideoClipCount = 1 And sequence.AudioClipCount = 1 Then
            Dim firstVideoClip As SGA.Clip _
                = sequence.VideoClips.First
            Dim firstAudioClip As SGA.Clip _
                = sequence.AudioClips.First

            Dim indexOfVideoClip As Integer _
                = sequence.Clips.IndexOf(firstVideoClip)
            Dim indexOfAudioClip As Integer _
                = sequence.Clips.IndexOf(firstAudioClip)

            ItemRects(indexOfVideoClip).IsSelected = True
            ItemRects(indexOfAudioClip).IsSelected = True

            changedRects.Add(ItemRects(indexOfVideoClip))
            changedRects.Add(ItemRects(indexOfAudioClip))
        Else
            ItemRects(0).IsSelected = True
            changedRects.Add(ItemRects(0))
        End If

        SelectedChanged(changedRects)

        InitializeControls()
        UpdateControls()

        ResumeLayout()

        RaiseEvent SequenceLoaded(Me, New SequenceLoadedEventArgs)
    End Sub

    Public Event SequenceLoaded(sender As Object, e As SequenceLoadedEventArgs)

    Public Class SequenceLoadedEventArgs
        Inherits EventArgs
    End Class

    'Private Sub InitializeDisplay()
    'End Sub

    Private Sub InitializeControls()
        tsddbtnSelect.Enabled = True
        tsmiSelectAll.Enabled = True
        tsmiSelectNone.Enabled = True

        tsddbtnView.Enabled = True
        tsmiClips.Enabled = True
        tsmiChunks.Enabled = True

        'With PlusMinusControlH
        '    .Reset()
        '    .Enabled = True
        'End With
        'With PlusMinusControlV
        '    .Reset()
        '    .Enabled = True
        'End With
    End Sub

    Private Sub UpdateControls()
        If Sequence Is Nothing Then Exit Sub

        'If SelectedIndexes.Count > 0 Then
        '    tsbtnCreatePreview.Enabled = True
        'Else
        '    tsbtnCreatePreview.Enabled = False
        'End If

        If Sequence.FrameList.Count = 0 Then
            tsddbtnExport.Enabled = False
        Else
            tsddbtnExport.Enabled = True
        End If
    End Sub

    Private Sub UpdateDisplay()
        pbxItemDisplay.Image = _displayBitmap
    End Sub

    Private Sub btnCreatePreview_Click(sender As Object, e As EventArgs)
        CreatePreview()
    End Sub

    Private Sub CreatePreview()
        If SelectedIndexes.Count = 0 Then Exit Sub

        SuspendLayout()

        RaiseEvent CreatePreviewStarted()

        Select Case ViewMode
            Case ViewModeValue.Clips
                Sequence.BuildFrameListFromClips(SelectedIndexes.ToArray)
            Case ViewModeValue.Chunks
                Sequence.BuildFrameListFromChunks(SelectedIndexes.ToArray)
        End Select

        'tsbtnCreatePreview.Enabled = False

        ResumeLayout()

        RaiseEvent CreatePreviewFinished()
    End Sub

    Public Event CreatePreviewStarted()
    Public Event CreatePreviewFinished()

    Private Sub tsmiClipsChunks_Click(sender As Object, e As EventArgs) Handles tsmiClips.Click, tsmiChunks.Click
        If sender Is tsmiClips Then
            tsmiClips.Checked = True
            tsmiChunks.Checked = False
            ViewMode = ViewModeValue.Clips
        ElseIf sender Is tsmiChunks Then
            tsmiClips.Checked = False
            tsmiChunks.Checked = True
            ViewMode = ViewModeValue.Chunks
        End If
    End Sub

    Private ReadOnly Property ItemRects As List(Of SelectableRectangleF)
        Get
            Select Case _viewMode
                Case ViewModeValue.Clips
                    Return _clipRects
                Case ViewModeValue.Chunks
                    Return _chunkRects
            End Select
            Return Nothing
        End Get
    End Property

    Private _selectedIndexes As New List(Of Integer)
    Private ReadOnly Property SelectedIndexes As List(Of Integer)
        Get
            If ItemRects Is Nothing Then
                Return Nothing
            End If

            _selectedIndexes.Clear()

            For Each rect As SelectableRectangleF In ItemRects
                If rect.IsSelected = True Then
                    _selectedIndexes.Add(rect.Index)
                End If
            Next

            Return _selectedIndexes
        End Get
    End Property

    ''Added on 2016/4/19: Selection rectangle
    'Private _selectionHasMouse As Boolean
    'Private _selectionStartPoint, _selectionEndPoint As Point
    'Private _selectionRect As Rectangle

    'Private _selectedIndex, _oldSelectedIndex As Integer
    'Private _isSelectedChanged As Boolean = False
    Private Sub SelectedChanged(ByRef changedRects As List(Of SelectableRectangleF))
        UpdateControls()

        If changedRects IsNot Nothing Then
            Using g As Graphics = Graphics.FromImage(_displayBitmap)
                For Each rect As SelectableRectangleF In changedRects
                    DrawRect(rect, g)
                Next
            End Using
        End If

        UpdateDisplay()

        CreatePreview()
    End Sub

    Private _clickedIndex, _oldClickedIndex As Integer

    Private Sub pbxDisplay_MouseDown(sender As Object, e As MouseEventArgs) Handles pbxItemDisplay.MouseDown
        Dim isControlPressed As Boolean = If((ModifierKeys And Keys.Control) <> 0, True, False)
        Dim isAltPressed As Boolean = If((ModifierKeys And Keys.Alt) <> 0, True, False)
        Dim isShiftPressed As Boolean = If((ModifierKeys And Keys.Shift) <> 0, True, False)

        Dim selectRect As New RectangleF(e.X, e.Y, 1, 1)
        If isControlPressed = True Then
            selectRect.Width = _displayBitmap.Width - e.X
        End If

        Dim changedRects As New List(Of SelectableRectangleF)

        For Each rect As SelectableRectangleF In ItemRects
            Dim wasSelected As Boolean _
                = rect.IsSelected

            If rect.BaseRect.IntersectsWith(selectRect) Then
                If isAltPressed = True Then
                    rect.IsSelected = False
                Else
                    rect.IsSelected = True

                    If rect.BaseRect.Contains(e.X, e.Y) Then
                        _clickedIndex = rect.Index
                    End If

                End If
            Else
                If isAltPressed = False And isShiftPressed = False Then
                    rect.IsSelected = False
                End If
            End If

            If rect.IsSelected <> wasSelected Then
                changedRects.Add(rect)
            End If
        Next

        If changedRects.Count > 0 Then
            SelectedChanged(changedRects)
        End If

    End Sub

    Private _hoveredIndex, _oldHoveredIndex As Integer
    'Private _isHoveredChanged As Boolean = True
    'Public Event HoveredChanged()

    Private Sub pbxDisplay_MouseMove(sender As Object, e As MouseEventArgs) Handles pbxItemDisplay.MouseMove
        If ItemRects Is Nothing Then Exit Sub

        _oldHoveredIndex = _hoveredIndex
        _hoveredIndex = -1

        For Each rect As SelectableRectangleF In ItemRects

            If rect.BaseRect.Contains(e.Location) Then

                _hoveredIndex = rect.Index

                rect.IsHovered = True
            Else
                rect.IsHovered = False
            End If
        Next

        If _hoveredIndex <> _oldHoveredIndex Then

            If _hoveredIndex = -1 Then
                With ToolTipMain
                    .RemoveAll()
                    .SetToolTip(pbxItemDisplay, "")
                End With
            Else
                Dim title As String = ""
                Dim caption As String = ""

                Select Case ViewMode
                    Case ViewModeValue.Clips
                        title = "Clip info:"

                        Dim clip As SGA.Clip _
                            = Sequence.Clips(_hoveredIndex)

                        With clip
                            caption = $"{New SMPTETimecode(.StartingFrameNumber, Sequence.VideoRate)}{ControlChars.CrLf}Track { .TrackID:X4}{ControlChars.CrLf}Length {New SMPTETimecode(.Count, Sequence.VideoRate)}"
                        End With

                    Case ViewModeValue.Chunks
                        title = "Chunk info:"

                        Dim chunk As SGA.Chunk _
                            = Sequence.File.Chunks(_hoveredIndex)

                        With chunk
                            caption = .InfoToString(False) '$"${ .Offset:X7}{ControlChars.CrLf}{ .Type.ToString}{ControlChars.CrLf}Track { .TrackID:X4}{ControlChars.CrLf}Time { .Timecode:X8}"
                        End With

                End Select

                With ToolTipMain
                    .ToolTipTitle = title
                    .SetToolTip(pbxItemDisplay, caption)
                End With
            End If

        End If

    End Sub

    Private Sub pbxDisplay_MouseUp(sender As Object, e As MouseEventArgs) Handles pbxItemDisplay.MouseUp
    End Sub

    Private Sub pbxDisplay_DoubleClick(sender As Object, e As EventArgs) Handles pbxItemDisplay.DoubleClick
        Select Case ViewMode
            Case ViewModeValue.Clips
                'CreatePreview()
            Case ViewModeValue.Chunks
                If _clickedIndex <> -1 Then
                    Dim chunk As SGA.Chunk _
                        = Sequence.File.Chunks(_clickedIndex)
                    RaiseEvent ChunkDoubleClicked(Me, New ChunkDoubleClickedEventArgs(chunk))
                End If
        End Select
    End Sub

    Public Event ChunkDoubleClicked(sender As Object, e As ChunkDoubleClickedEventArgs)

    Public Class ChunkDoubleClickedEventArgs
        Inherits EventArgs
        Private _chunk As SGA.Chunk
        Public Property chunk As SGA.Chunk
            Get
                Return _chunk
            End Get
            Protected Set(value As SGA.Chunk)
                _chunk = value
            End Set
        End Property
        Sub New(ByVal chunk As SGA.Chunk)
            MyBase.New
            Me.chunk = chunk
        End Sub
    End Class

    Private Sub btnSelectAll_Click(sender As Object, e As EventArgs) Handles tsmiSelectAll.Click
        SelectAllOrNone(True)
    End Sub

    Private Sub btnSelectNone_Click(sender As Object, e As EventArgs) Handles tsmiSelectNone.Click
        SelectAllOrNone(False)
    End Sub

    Private Sub SelectAllOrNone(ByVal all As Boolean)
        Dim changedRects As New List(Of SelectableRectangleF)

        For Each rect As SelectableRectangleF In ItemRects
            rect.IsSelected = all
            changedRects.Add(rect)
        Next
        '_isSelectedChanged = True
        SelectedChanged(changedRects)
    End Sub

    'Private Sub tsmiExportCurrentPreview_Click(sender As Object, e As EventArgs) Handles tsmiExportCurrentPreview.Click
    '    Using fc As New frmExport(_Sequence, SGA.Exporter.ExportTypeValue.Sequence)
    '        With fc
    '            .ShowDialog()
    '        End With
    '    End Using
    'End Sub

    'Private Sub tsmiExportAllClips_Click(sender As Object, e As EventArgs) Handles tsmiExportAllClips.Click
    '    Using fc As New frmExport(_Sequence, SGA.Exporter.ExportTypeValue.Clips)
    '        With fc
    '            .ShowDialog()
    '        End With
    '    End Using
    'End Sub

    Private Sub Export(sender As Object, e As EventArgs) Handles tsmiExportCurrentPreview.Click, tsmiExportAllClips.Click
        Dim exportType As SGA.Exporter.ExportTypeValue

        If sender Is tsmiExportCurrentPreview Then
            exportType = SGA.Exporter.ExportTypeValue.Sequence
        ElseIf sender Is tsmiExportAllClips Then
            exportType = SGA.Exporter.ExportTypeValue.Clips
        End If

        Using fc As New frmExport(_Sequence, exportType)
            With fc
                .ShowDialog()
            End With
        End Using
    End Sub

    Private Sub ZoomControl_Click(sender As Object, e As EventArgs) Handles tsbtnHPlus.Click, tsbtnHMinus.Click, tsbtnVPlus.Click, tsbtnVMinus.Click

        If sender Is tsbtnHPlus Then

        ElseIf sender Is tsbtnHMinus Then

        ElseIf sender Is tsbtnVPlus Then

        ElseIf sender Is tsbtnVMinus Then

        End If

    End Sub

    Private Sub ZoomControls_ValueChanged(sender As Object, e As PlusMinusControl.ValueChangedEventArgs)
        Dim pmControl As PlusMinusControl = sender
        'Select Case pmControl.Name
        '    Case PlusMinusControlH.Name
        '        _frameRect.Width = _frameRectBase.Width * e.Value
        '    Case PlusMinusControlV.Name
        '        _frameRect.Height = _frameRectBase.Height * e.Value
        'End Select
        If Sequence IsNot Nothing Then
            CreateBitmaps()
            UpdateDisplay()
        End If
    End Sub

    Private Class SelectableRectangleF

        Private _index As Integer
        Public Property Index As Integer
            Get
                Return _index
            End Get
            Protected Set(value As Integer)
                _index = value
            End Set
        End Property

        Private _rect As RectangleF
        Public Property BaseRect As RectangleF
            Get
                Return _rect
            End Get
            Protected Set(value As RectangleF)
                _rect = value
            End Set
        End Property

        Private _color As Color
        Public Property Color As Color
            Get
                Return _color
            End Get
            Protected Set(value As Color)
                _color = value
            End Set
        End Property

        Public Property IsHovered As Boolean
        Public Property IsSelected As Boolean

        Sub New(ByVal index As Integer, ByVal rect As RectangleF, color As Color)
            With Me
                .Index = index
                .BaseRect = rect
                .Color = color
                .IsHovered = False
                .IsSelected = False
            End With
        End Sub

    End Class

    Private _itemRectBase As New RectangleF(0, 0, 8, 16)
    Private _frameRectBase As New RectangleF(0, 0, _itemRectBase.Width + 2, _itemRectBase.Height + 2)
    Private _timeCodeRowHeight As Single = _frameRectBase.Height

    Private _displayBitmap As Bitmap

    <Browsable(True), Category("Appearance")>
    Public Property VideoColor As Color = Color.Blue
    <Browsable(True), Category("Appearance")>
    Public Property AudioColor As Color = Color.Green
    <Browsable(True), Category("Appearance")>
    Public Property MixedColor As Color = Color.Teal
    <Browsable(True), Category("Appearance")>
    Public Property MiscColor As Color = Color.Purple

    Private Sub CreateBitmaps()
        CreateClipRects()
        CreateClipBitmap()
        DrawDisplayBitmap(_clipBitmap)

        CreateChunkRects()
        CreateChunkBitmap()
        DrawDisplayBitmap(_chunkBitmap)
    End Sub

    'Private Sub CreateRects()
    '    'If Sequence Is Nothing Then Exit Sub
    '    CreateClipRects()
    '    CreateChunkRects()
    'End Sub

    Private _clipBitmap As Bitmap

    Private Sub CreateClipBitmap()
        Dim frameCount As Integer
        For Each clip As SGA.Clip In Sequence.Clips
            frameCount = Math.Max(frameCount, clip.StartingFrameNumber + clip.Count)
        Next
        Dim trackCount As Integer = Sequence.TrackIDs.Count

        Dim bitmapSize As Size
        With bitmapSize
            .Width = frameCount * _frameRectBase.Width + 1
            .Height = trackCount * _frameRectBase.Height
        End With

        'Added on 2016/5/3: Make space for timecodes at top
        bitmapSize.Height += _timeCodeRowHeight

        _clipBitmap = New Bitmap(bitmapSize.Width, bitmapSize.Height, Imaging.PixelFormat.Format32bppArgb)
    End Sub

    Private _clipRects As List(Of SelectableRectangleF)

    Private Sub CreateClipRects()
        _clipRects = New List(Of SelectableRectangleF)

        For i As Integer = 0 To Sequence.Clips.Count - 1
            Dim clip As SGA.Clip _
                = Sequence.Clips(i)

            Dim trackIDIndex As Integer _
                = Sequence.TrackIDs.IndexOf(clip.TrackID)

            Dim rect As New RectangleF
            With rect
                .X = clip.StartingFrameNumber * _frameRectBase.Width + 1
                .Y = trackIDIndex * _frameRectBase.Height + 1
                .Width = clip.Count * _frameRectBase.Width - 1
                .Height = _itemRectBase.Height
            End With

            'Added on 2016/5/3: Make space for timecodes at top
            rect.Y += _timeCodeRowHeight

            Dim color As Color
            If clip.HasVideo And clip.HasAudio Then
                color = MixedColor
            ElseIf clip.HasVideo Then
                color = VideoColor
            Else
                color = AudioColor
            End If

            _clipRects.Add(New SelectableRectangleF(i, rect, color))
        Next

    End Sub

    Private _chunkBitmap As Bitmap

    Private Sub CreateChunkBitmap()
        Dim trackCount As Integer = Sequence.TrackIDs.Count

        Dim bitmapSize As Size
        With bitmapSize
            .Width = Sequence.File.Chunks.Count * _frameRectBase.Width + 1
            .Height = trackCount * _frameRectBase.Height
        End With

        _chunkBitmap = New Bitmap(bitmapSize.Width, bitmapSize.Height, Imaging.PixelFormat.Format32bppArgb)
    End Sub

    Private _chunkRects As List(Of SelectableRectangleF)

    Private Sub CreateChunkRects()
        _chunkRects = New List(Of SelectableRectangleF)

        For i As Integer = 0 To Sequence.File.Chunks.Count - 1
            Dim chunk As SGA.Chunk _
                = Sequence.File.Chunks(i)

            Dim trackIDIndex As Integer _
                = Sequence.TrackIDs.IndexOf(chunk.TrackID)

            Dim rect As New RectangleF
            With rect
                .X = i * _frameRectBase.Width + 1
                .Y = trackIDIndex * _frameRectBase.Height + 1
                .Width = _itemRectBase.Width
                .Height = _itemRectBase.Height
            End With

            Dim color As Color
            If TypeOf (chunk) Is SGA.VideoChunk Then
                color = VideoColor
            ElseIf TypeOf (chunk) Is SGA.AudioChunk Then
                color = AudioColor
            ElseIf TypeOf (chunk) Is SGA.ContainerChunk Then
                color = MixedColor
            Else
                color = MiscColor
            End If

            _chunkRects.Add(New SelectableRectangleF(i, rect, color))
        Next

    End Sub

    Private Sub DrawDisplayBitmap(ByRef b As Bitmap)
        Using g As Graphics = Graphics.FromImage(b)
            SetGraphicsQuality(g)
            g.Clear(_backgroundColor)

            DrawFrameLines(g)
            DrawTrackLines(g)
            If b Is _clipBitmap Then
                DrawSecondsLines(g)
                DrawTimecodes(g)
            End If

            If b Is _clipBitmap Then
                DrawRects(_clipRects, g)
            ElseIf b Is _chunkBitmap Then
                DrawRects(_chunkRects, g)
            End If

        End Using
    End Sub

    Private Sub SetGraphicsQuality(ByRef g As Graphics)
        With g
            .CompositingQuality = Drawing2D.CompositingQuality.HighSpeed 'Drawing2D.CompositingQuality.HighQuality
            .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor 'Drawing2D.InterpolationMode.HighQualityBicubic
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighSpeed 'Drawing2D.PixelOffsetMode.HighQuality
            .SmoothingMode = Drawing2D.SmoothingMode.None
        End With
    End Sub

    Private _backgroundColor As Color = Color.Black
    Private _frameLinePen As Pen = New Pen(Color.FromArgb(23, 23, 23), 1)
    Private _secondLinePen As Pen = New Pen(Color.FromArgb(63, 63, 63), 1)
    Private _trackSeparatorLinePen As Pen = New Pen(Color.FromArgb(103, 103, 103), 1)

    Private Sub DrawFrameLines(ByRef g As Graphics)
        For i As Integer = 0 To g.VisibleClipBounds.Width Step _frameRectBase.Width
            g.DrawLine(_frameLinePen, i, 0, i, g.VisibleClipBounds.Height)
        Next
    End Sub

    Private Sub DrawTrackLines(ByRef g As Graphics)
        'Draw track separator lines
        For i As Integer = 0 To g.VisibleClipBounds.Height Step _frameRectBase.Height
            g.DrawLine(_trackSeparatorLinePen, 0, i, g.VisibleClipBounds.Width, i)
        Next
    End Sub

    Private Sub DrawSecondsLines(ByRef g As Graphics)
        For i As Integer = 0 To g.VisibleClipBounds.Width Step (_frameRectBase.Width * Sequence.VideoRate)
            g.DrawLine(_secondLinePen, i, 0, i, g.VisibleClipBounds.Height)
        Next
    End Sub

    Private Sub DrawTimecodes(ByRef g As Graphics)
        Using font As New Font(FontFamily.GenericSansSerif, 8)
            For i As Integer = 0 To g.VisibleClipBounds.Width Step (_frameRectBase.Width * Sequence.VideoRate)
                Dim timecode As New SMPTETimecode(CInt(i \ _frameRectBase.Width), Sequence.VideoRate)
                g.DrawString(timecode.ToString(False), font, Brushes.Gray, i, 0)
            Next
        End Using
    End Sub

    Private _highlightPen As New Pen(Color.FromArgb(95, Color.White), 1)
    Private _shadowPen As New Pen(Color.FromArgb(95, Color.Black), 1)

    Private _fillBrush As New SolidBrush(Color.White)
    Private _highlightBrush As New SolidBrush(Color.FromArgb(95, Color.White))

    Private _selectionRectBrush As New SolidBrush(Color.FromArgb(95, Color.CornflowerBlue))

    Private Sub DrawRects(ByRef rects As List(Of SelectableRectangleF), ByRef g As Graphics)

        With _highlightPen
            .EndCap = Drawing2D.LineCap.Round
        End With

        With _shadowPen
            .EndCap = Drawing2D.LineCap.Round
        End With

        For i As Integer = 0 To rects.Count - 1
            Dim rect As SelectableRectangleF _
                = rects(i)
            DrawRect(rect, g)
        Next

    End Sub

    Private Sub DrawRect(ByRef rect As SelectableRectangleF, ByRef g As Graphics)
        g.FillRectangle(Brushes.Black, rect.BaseRect)

        _fillBrush.Color = rect.Color
        g.FillRectangle(_fillBrush, rect.BaseRect)

        'Added on 2015/11/3: Highlighting clips
        If rect.IsSelected Then
            g.FillRectangle(_highlightBrush, rect.BaseRect)
        End If

        With rect.BaseRect
            g.DrawLine(_highlightPen, .Left, .Top, .Left, .Bottom)
            g.DrawLine(_highlightPen, .Left, .Top, .Right, .Top)
            g.DrawLine(_shadowPen, .Right, .Top, .Right, .Bottom)
            g.DrawLine(_shadowPen, .Left, .Bottom, .Right, .Bottom)
        End With

    End Sub

    Private Sub DrawReversibleRectangle(ByVal p1 As Point, ByVal p2 As Point)
        Dim rc As New Rectangle

        ' Convert the points to screen coordinates.
        p1 = PointToScreen(p1)
        p2 = PointToScreen(p2)

        ' Normalize the rectangle.
        If (p1.X < p2.X) Then
            rc.X = p1.X
            rc.Width = p2.X - p1.X
        Else
            rc.X = p2.X
            rc.Width = p1.X - p2.X
        End If

        If (p1.Y < p2.Y) Then
            rc.Y = p1.Y
            rc.Height = p2.Y - p1.Y
        Else
            rc.Y = p2.Y
            rc.Height = p1.Y - p2.Y
        End If

        ' Draw the reversible frame.
        ControlPaint.DrawReversibleFrame(rc,
             Color.Red, FrameStyle.Dashed)

    End Sub

End Class
