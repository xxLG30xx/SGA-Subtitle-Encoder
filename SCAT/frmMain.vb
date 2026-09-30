Imports System.IO
Imports SCAT
Imports SCAT.SGA

Public Class frmMain
    Dim _myText As String _
        = $"{My.Application.Info.Title}"

    Dim _filePath As String
    Friend WithEvents _file As SGA.File
    Friend WithEvents _sequence As Sequence

    Dim _lastOutputDir As String
    Dim _lastInputDir As String

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        MainToolStrip.Renderer = New CustomToolStripRenderer

        Me.Text = _myText
    End Sub

    Private Sub frmMain_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        If SgaPlayerControl1.Player IsNot Nothing Then
            SgaPlayerControl1.Player.Dispose()
        End If
    End Sub

    Private Sub frmMain_LostFocus(sender As Object, e As EventArgs) Handles Me.LostFocus
        SgaPlayerControl1.Pause()
    End Sub

    Private Sub OpenButton_Click(sender As Object, e As EventArgs) Handles OpenButton.Click
        Using ofdOpen As New OpenFileDialog

            With ofdOpen
                .AutoUpgradeEnabled = True
                .CheckFileExists = True
                .CheckPathExists = True
                .DefaultExt = "*.sga"
                '.Filter = "All supported files|*.sga;*.sg?;*.s??*;*.pt1;*.clp|SMV files (Sherlock Holmes)|*.smv|All files (USE AT OWN RISK!)|*.*"
                .Filter = "All supported files|*.sga;*.sg?;*.s??*;*.pt1;*.clp;*.avc;LOGMOVIE;CRMOVIE|All files (USE AT OWN RISK!)|*.*"
                .FilterIndex = 0
                .Multiselect = False

                If _lastInputDir <> "" Then
                    .InitialDirectory = _lastInputDir
                End If

                Dim result As DialogResult _
                    = .ShowDialog()

                If result = Windows.Forms.DialogResult.OK Then
                    _filePath = .FileName
                    _lastInputDir = Path.GetDirectoryName(_filePath)
                    Text = $"{Path.GetFileName(_filePath)} - {_myText}"

                    DisableAllControls()
                    InitializeVisualizers()

                    Using fs As FileStream = .OpenFile
                        OpenSGAFile(fs)
                    End Using

                    EnableAllControls()

                    'For Each ctl As Control In SidePanel.Controls()
                    '    ctl.Enabled = True
                    'Next

                End If

            End With

        End Using

    End Sub

    Private Sub EnableAllControls()
        'Enable all controls
        For Each tsi As ToolStripItem In MainToolStrip.Items
            tsi.Enabled = True
        Next
        'For Each ctl As Control In VideoDisplayPanel.Controls
        '    ctl.Enabled = True
        'Next
        'For Each ctl As Control In SidePanel.Controls()
        '    ctl.Enabled = True
        'Next
        'tabVisualizations.Enabled = True
        'For Each tp As TabPage In tabVisualizations.TabPages
        '    For Each ctl As Control In tp.Controls
        '        ctl.Enabled = True
        '    Next
        'Next
    End Sub

    Private Sub DisableAllControls()
        'Disable all controls
        For Each tsi As ToolStripItem In MainToolStrip.Items
            tsi.Enabled = False
        Next
        'For Each ctl As Control In VideoDisplayPanel.Controls
        '    ctl.Enabled = False
        'Next
        'For Each ctl As Control In SidePanel.Controls()
        '    ctl.Enabled = False
        'Next
        'tabVisualizations.Enabled = False
        'For Each tp As TabPage In tabVisualizations.TabPages
        '    For Each ctl As Control In tp.Controls
        '        ctl.Enabled = False
        '    Next
        'Next
    End Sub

    Private Sub InitializeVisualizers()
        'Initialize visualizers
        tbxInfo.Text = ""
    End Sub

    Private Sub OpenSGAFile(ByRef fs As FileStream)
        _file = New SGA.File(fs)

        _sequence = New Sequence(_file)

        SgaPlayerControl1.LoadSequence(_sequence)
        SgaSequenceViewerControl1.LoadSequence(_sequence)

        'DisplayClipInfo(_sequence)
    End Sub

    Private Sub DisplayClipInfo(ByRef sequence As Sequence)

        With tbxInfo
            .Clear()
            .SuspendLayout()
        End With

        Dim sb As New Text.StringBuilder()

        Dim clipIndex As Integer

        For Each clip As Clip In sequence.Clips
            sb.AppendLine(
                $"Clip {clipIndex:00}: {If(clip.HasVideo, "Video ", "")}{If(clip.HasAudio, "Audio", "")}; {clip.Count} frames starting @ frame {clip.StartingFrameNumber}"
            )
            clipIndex += 1
        Next

        With tbxInfo
            .Text = sb.ToString
            .ResumeLayout()
        End With

    End Sub

    'Private Sub Export(ByVal target As Exporter.ExportFormatValue)
    '    Using fc As New frmExport(_sequence, )
    '        With fc
    '            .Export(_sequence, _filePath)
    '        End With
    '    End Using
    'End Sub

    Private Sub From3DOToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles From3DOToolStripMenuItem.Click
        Dim frm3DO As New frm3DOExtractor
        frm3DO.Show(Me)
    End Sub

    Private Sub FromDoubleSwitchForWindows95ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromDoubleSwitchForWindows95ToolStripMenuItem.Click
        Dim frmWindows As New frmWindowsExtractor
        frmWindowsExtractor.Show(Me)
    End Sub

    Private Sub FromNTMOVIEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SequencesFromNTMOVIEFileToolStripMenuItem.Click
        Dim frmDOS As New frmDOSExtractor
        frmDOS.Show(Me)
    End Sub

    Private Sub ScreenshotButton_Click(sender As Object, e As EventArgs) Handles ScreenshotButton.Click
        If SgaPlayerControl1.pbxVideoFrame.Image IsNot Nothing Then
            Clipboard.SetImage(SgaPlayerControl1.pbxVideoFrame.Image.Clone)
        End If
    End Sub

    Private Sub AboutButton_Click(sender As Object, e As EventArgs) Handles AboutButton.Click
        frmAbout.ShowDialog(Me)
    End Sub

    Private Sub SGAPlayerControl1_FrameNumberChanged(sender As Object, e As SGAPlayerControl.FrameNumberChangedEventArgs) Handles SgaPlayerControl1.FrameNumberChanged
        With tbxInfo
            .Clear()
            .SuspendLayout()
        End With

        Dim sb As New Text.StringBuilder()

        sb.Append(_sequence.GetFrameInfoAsString(e.FrameNumber))

        With tbxInfo
            .Text = sb.ToString
            .ResumeLayout()
        End With
    End Sub

    Private Sub SgaPlayerControl1_VideoFrameChanged(sender As Object, e As Player.VideoFrameChangedEventArgs) Handles SgaPlayerControl1.VideoFrameChanged
        'Added on 2014/8/27: Palette viewer
        SgaPaletteViewerControl1.VDP = SgaPlayerControl1.Player.Renderer.VDP
        'Added on 2014/9/5: Tile viewer
        SgaTileViewerControl1.VDP = SgaPlayerControl1.Player.Renderer.VDP
    End Sub

    Private Sub SgaPlayerControl1_SequenceLoaded(sender As Object, e As SGAPlayerControl.SequenceLoadedEventArgs) Handles SgaPlayerControl1.SequenceLoaded
        With tbxInfo
            .Clear()
            .SuspendLayout()
        End With

        Dim sb As New Text.StringBuilder()

        sb.Append(_sequence.GetFrameInfoAsString(0))

        With tbxInfo
            .Text = sb.ToString
            .ResumeLayout()
        End With
    End Sub

    Private Sub SgaSequenceViewerControl1_CreatePreviewStarted() Handles SgaSequenceViewerControl1.CreatePreviewStarted
        SgaPlayerControl1.Pause()
    End Sub

    Private Sub SgaSequenceViewerControl1_CreatePreviewFinished() Handles SgaSequenceViewerControl1.CreatePreviewFinished
        SgaPlayerControl1.LoadSequence(_sequence)
    End Sub

    Private Sub SgaSequenceViewerControl1_ChunkDoubleClicked(sender As Object, e As SGASequenceViewerControl.ChunkDoubleClickedEventArgs) Handles SgaSequenceViewerControl1.ChunkDoubleClicked
        Dim rawData As New List(Of Byte)

        Using fs As New FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 2048)
            Using br As New BinaryReader(fs)

                Dim dataLocations As SortedList(Of Long, Integer) _
                    = e.chunk.DataLocations

                For Each offset As Long In dataLocations.Keys
                    br.BaseStream.Seek(offset, SeekOrigin.Begin)
                    rawData.AddRange(br.ReadBytes(dataLocations(offset)))
                Next

            End Using
        End Using

        With tbxInfo
            .Clear()
            .SuspendLayout()
            .Text = DataToString(rawData.ToArray, 0)
            .ResumeLayout()
        End With

        'tbxInfo.Text = e.chunk.InfoToString(True)
    End Sub

    Private Sub tsmiLZCompression_Click(sender As Object, e As EventArgs)
        Using newLZCompressionForm As New frmLZCompression
            With newLZCompressionForm
                .ShowDialog(Me)
            End With
        End Using
    End Sub

    'Private Sub ShowMacroblocksToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowMacroblockOutlinesToolStripMenuItem.Click
    '    Select Case ShowMacroblockOutlinesToolStripMenuItem.Checked
    '        Case True
    '            ShowMacroblockOutlinesToolStripMenuItem.Checked = False
    '        Case False
    '            ShowMacroblockOutlinesToolStripMenuItem.Checked = True
    '    End Select
    'End Sub

    Private Sub ShowMacroblocksToolStripMenuItem_CheckedChanged(sender As Object, e As EventArgs) Handles ShowMacroblockOutlinesToolStripMenuItem.CheckedChanged
        ProgramOptions.RenderMacroblockOutlines = ShowMacroblockOutlinesToolStripMenuItem.Checked
    End Sub

    Private Sub FilterVideoOutputToolStripMenuItem_CheckedChanged(sender As Object, e As EventArgs) Handles FilterVideoOutputToolStripMenuItem.CheckedChanged
        ProgramOptions.FilerVideoOutput = FilterVideoOutputToolStripMenuItem.Checked
    End Sub

End Class
