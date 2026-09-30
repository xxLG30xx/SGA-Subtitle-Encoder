<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.lblInfo = New System.Windows.Forms.Label()
        Me.tbxInfo = New System.Windows.Forms.TextBox()
        Me.lblBackgroundTiles = New System.Windows.Forms.Label()
        Me.lblPalette = New System.Windows.Forms.Label()
        Me.ToolTipMain = New System.Windows.Forms.ToolTip(Me.components)
        Me.OpenButton = New System.Windows.Forms.ToolStripButton()
        Me.ScreenshotButton = New System.Windows.Forms.ToolStripButton()
        Me.AboutButton = New System.Windows.Forms.ToolStripButton()
        Me.MainToolStrip = New System.Windows.Forms.ToolStrip()
        Me.ExtractToolStripDropDownButton = New System.Windows.Forms.ToolStripDropDownButton()
        Me.From3DOToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FromDoubleSwitchForWindows95ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SequencesFromNTMOVIEFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OptionsButton = New System.Windows.Forms.ToolStripDropDownButton()
        Me.ShowMacroblockOutlinesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FilterVideoOutputToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.splMain = New System.Windows.Forms.SplitContainer()
        Me.SgaPaletteViewerControl1 = New SCAT.SGAPaletteViewerControl()
        Me.SgaTileViewerControl1 = New SCAT.SGATileViewerControl()
        Me.SgaPlayerControl1 = New SCAT.SGAPlayerControl()
        Me.SgaSequenceViewerControl1 = New SCAT.SGASequenceViewerControl()
        Me.MainToolStrip.SuspendLayout()
        CType(Me.splMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splMain.Panel1.SuspendLayout()
        Me.splMain.Panel2.SuspendLayout()
        Me.splMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblInfo
        '
        Me.lblInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblInfo.AutoSize = True
        Me.lblInfo.BackColor = System.Drawing.Color.Transparent
        Me.lblInfo.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblInfo.ForeColor = System.Drawing.Color.White
        Me.lblInfo.Location = New System.Drawing.Point(592, 316)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Size = New System.Drawing.Size(37, 21)
        Me.lblInfo.TabIndex = 39
        Me.lblInfo.Text = "Info"
        Me.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tbxInfo
        '
        Me.tbxInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbxInfo.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbxInfo.Location = New System.Drawing.Point(592, 340)
        Me.tbxInfo.MaxLength = 128000
        Me.tbxInfo.Multiline = True
        Me.tbxInfo.Name = "tbxInfo"
        Me.tbxInfo.ReadOnly = True
        Me.tbxInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.tbxInfo.Size = New System.Drawing.Size(280, 151)
        Me.tbxInfo.TabIndex = 40
        Me.tbxInfo.WordWrap = False
        '
        'lblBackgroundTiles
        '
        Me.lblBackgroundTiles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblBackgroundTiles.AutoSize = True
        Me.lblBackgroundTiles.BackColor = System.Drawing.Color.Transparent
        Me.lblBackgroundTiles.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBackgroundTiles.ForeColor = System.Drawing.Color.White
        Me.lblBackgroundTiles.Location = New System.Drawing.Point(592, 95)
        Me.lblBackgroundTiles.Name = "lblBackgroundTiles"
        Me.lblBackgroundTiles.Size = New System.Drawing.Size(41, 21)
        Me.lblBackgroundTiles.TabIndex = 36
        Me.lblBackgroundTiles.Text = "Tiles"
        Me.lblBackgroundTiles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPalette
        '
        Me.lblPalette.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPalette.AutoSize = True
        Me.lblPalette.BackColor = System.Drawing.Color.Transparent
        Me.lblPalette.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPalette.ForeColor = System.Drawing.Color.White
        Me.lblPalette.Location = New System.Drawing.Point(592, 0)
        Me.lblPalette.Name = "lblPalette"
        Me.lblPalette.Size = New System.Drawing.Size(56, 21)
        Me.lblPalette.TabIndex = 34
        Me.lblPalette.Text = "Palette"
        Me.lblPalette.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToolTipMain
        '
        Me.ToolTipMain.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info
        '
        'OpenButton
        '
        Me.OpenButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.OpenButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.OpenButton.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OpenButton.ForeColor = System.Drawing.Color.White
        Me.OpenButton.Image = CType(resources.GetObject("OpenButton.Image"), System.Drawing.Image)
        Me.OpenButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.OpenButton.Name = "OpenButton"
        Me.OpenButton.Size = New System.Drawing.Size(61, 37)
        Me.OpenButton.Text = "&Open..."
        Me.OpenButton.ToolTipText = "Open a media file"
        '
        'ScreenshotButton
        '
        Me.ScreenshotButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ScreenshotButton.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ScreenshotButton.ForeColor = System.Drawing.Color.White
        Me.ScreenshotButton.Image = CType(resources.GetObject("ScreenshotButton.Image"), System.Drawing.Image)
        Me.ScreenshotButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ScreenshotButton.Name = "ScreenshotButton"
        Me.ScreenshotButton.Size = New System.Drawing.Size(91, 37)
        Me.ScreenshotButton.Text = "&Screenshot"
        Me.ScreenshotButton.ToolTipText = "Copy the current video frame to the clipboard"
        '
        'AboutButton
        '
        Me.AboutButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.AboutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.AboutButton.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AboutButton.ForeColor = System.Drawing.Color.White
        Me.AboutButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.AboutButton.Name = "AboutButton"
        Me.AboutButton.Size = New System.Drawing.Size(65, 37)
        Me.AboutButton.Text = "&About..."
        Me.AboutButton.ToolTipText = "Display information about this program"
        '
        'MainToolStrip
        '
        Me.MainToolStrip.AutoSize = False
        Me.MainToolStrip.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.MainToolStrip.BackgroundImage = CType(resources.GetObject("MainToolStrip.BackgroundImage"), System.Drawing.Image)
        Me.MainToolStrip.GripMargin = New System.Windows.Forms.Padding(0)
        Me.MainToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.MainToolStrip.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.MainToolStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenButton, Me.ExtractToolStripDropDownButton, Me.ScreenshotButton, Me.AboutButton, Me.OptionsButton})
        Me.MainToolStrip.Location = New System.Drawing.Point(0, 0)
        Me.MainToolStrip.Name = "MainToolStrip"
        Me.MainToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.MainToolStrip.Size = New System.Drawing.Size(880, 40)
        Me.MainToolStrip.TabIndex = 0
        '
        'ExtractToolStripDropDownButton
        '
        Me.ExtractToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ExtractToolStripDropDownButton.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.From3DOToolStripMenuItem, Me.FromDoubleSwitchForWindows95ToolStripMenuItem, Me.SequencesFromNTMOVIEFileToolStripMenuItem})
        Me.ExtractToolStripDropDownButton.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ExtractToolStripDropDownButton.ForeColor = System.Drawing.Color.White
        Me.ExtractToolStripDropDownButton.Image = CType(resources.GetObject("ExtractToolStripDropDownButton.Image"), System.Drawing.Image)
        Me.ExtractToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ExtractToolStripDropDownButton.Name = "ExtractToolStripDropDownButton"
        Me.ExtractToolStripDropDownButton.Size = New System.Drawing.Size(69, 37)
        Me.ExtractToolStripDropDownButton.Text = "&Extract"
        Me.ExtractToolStripDropDownButton.ToolTipText = "Extract media sequences from monolithic files"
        '
        'From3DOToolStripMenuItem
        '
        Me.From3DOToolStripMenuItem.Name = "From3DOToolStripMenuItem"
        Me.From3DOToolStripMenuItem.Size = New System.Drawing.Size(390, 26)
        Me.From3DOToolStripMenuItem.Text = "Sequences from &3DO discdata file..."
        Me.From3DOToolStripMenuItem.ToolTipText = "Extract sequences from a 3DO discdata file"
        '
        'FromDoubleSwitchForWindows95ToolStripMenuItem
        '
        Me.FromDoubleSwitchForWindows95ToolStripMenuItem.Name = "FromDoubleSwitchForWindows95ToolStripMenuItem"
        Me.FromDoubleSwitchForWindows95ToolStripMenuItem.Size = New System.Drawing.Size(390, 26)
        Me.FromDoubleSwitchForWindows95ToolStripMenuItem.Text = "&Assets from Double Switch for Windows 95..."
        '
        'SequencesFromNTMOVIEFileToolStripMenuItem
        '
        Me.SequencesFromNTMOVIEFileToolStripMenuItem.Name = "SequencesFromNTMOVIEFileToolStripMenuItem"
        Me.SequencesFromNTMOVIEFileToolStripMenuItem.Size = New System.Drawing.Size(390, 26)
        Me.SequencesFromNTMOVIEFileToolStripMenuItem.Text = "Sequences from NTMOVIE file..."
        '
        'OptionsButton
        '
        Me.OptionsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.OptionsButton.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowMacroblockOutlinesToolStripMenuItem, Me.FilterVideoOutputToolStripMenuItem})
        Me.OptionsButton.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OptionsButton.ForeColor = System.Drawing.Color.White
        Me.OptionsButton.Image = CType(resources.GetObject("OptionsButton.Image"), System.Drawing.Image)
        Me.OptionsButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.OptionsButton.Name = "OptionsButton"
        Me.OptionsButton.Size = New System.Drawing.Size(78, 37)
        Me.OptionsButton.Text = "&Options"
        Me.OptionsButton.ToolTipText = "Toggle various program options here"
        '
        'ShowMacroblockOutlinesToolStripMenuItem
        '
        Me.ShowMacroblockOutlinesToolStripMenuItem.CheckOnClick = True
        Me.ShowMacroblockOutlinesToolStripMenuItem.Name = "ShowMacroblockOutlinesToolStripMenuItem"
        Me.ShowMacroblockOutlinesToolStripMenuItem.Size = New System.Drawing.Size(263, 26)
        Me.ShowMacroblockOutlinesToolStripMenuItem.Text = "Show &macroblock outlines"
        Me.ShowMacroblockOutlinesToolStripMenuItem.ToolTipText = "Show macroblock outlines for videos from 32-bit ports"
        '
        'FilterVideoOutputToolStripMenuItem
        '
        Me.FilterVideoOutputToolStripMenuItem.CheckOnClick = True
        Me.FilterVideoOutputToolStripMenuItem.Name = "FilterVideoOutputToolStripMenuItem"
        Me.FilterVideoOutputToolStripMenuItem.Size = New System.Drawing.Size(263, 26)
        Me.FilterVideoOutputToolStripMenuItem.Text = "Filter &video output"
        Me.FilterVideoOutputToolStripMenuItem.ToolTipText = "Apply edge-preserving filter to video output"
        '
        'splMain
        '
        Me.splMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.splMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splMain.Location = New System.Drawing.Point(0, 40)
        Me.splMain.Name = "splMain"
        Me.splMain.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'splMain.Panel1
        '
        Me.splMain.Panel1.Controls.Add(Me.SgaPaletteViewerControl1)
        Me.splMain.Panel1.Controls.Add(Me.lblInfo)
        Me.splMain.Panel1.Controls.Add(Me.tbxInfo)
        Me.splMain.Panel1.Controls.Add(Me.SgaTileViewerControl1)
        Me.splMain.Panel1.Controls.Add(Me.SgaPlayerControl1)
        Me.splMain.Panel1.Controls.Add(Me.lblPalette)
        Me.splMain.Panel1.Controls.Add(Me.lblBackgroundTiles)
        Me.splMain.Panel1MinSize = 490
        '
        'splMain.Panel2
        '
        Me.splMain.Panel2.AutoScroll = True
        Me.splMain.Panel2.Controls.Add(Me.SgaSequenceViewerControl1)
        Me.splMain.Panel2MinSize = 240
        Me.splMain.Size = New System.Drawing.Size(880, 801)
        Me.splMain.SplitterDistance = 496
        Me.splMain.SplitterWidth = 8
        Me.splMain.TabIndex = 42
        '
        'SgaPaletteViewerControl1
        '
        Me.SgaPaletteViewerControl1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SgaPaletteViewerControl1.AutoScroll = True
        Me.SgaPaletteViewerControl1.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.SgaPaletteViewerControl1.EntriesPerRow = 16
        Me.SgaPaletteViewerControl1.EntrySize = New System.Drawing.Size(16, 16)
        Me.SgaPaletteViewerControl1.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SgaPaletteViewerControl1.Location = New System.Drawing.Point(592, 26)
        Me.SgaPaletteViewerControl1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.SgaPaletteViewerControl1.Name = "SgaPaletteViewerControl1"
        Me.SgaPaletteViewerControl1.Size = New System.Drawing.Size(280, 66)
        Me.SgaPaletteViewerControl1.TabIndex = 43
        Me.SgaPaletteViewerControl1.VDP = Nothing
        '
        'SgaTileViewerControl1
        '
        Me.SgaTileViewerControl1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SgaTileViewerControl1.AutoScroll = True
        Me.SgaTileViewerControl1.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.SgaTileViewerControl1.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SgaTileViewerControl1.Location = New System.Drawing.Point(592, 119)
        Me.SgaTileViewerControl1.Name = "SgaTileViewerControl1"
        Me.SgaTileViewerControl1.Size = New System.Drawing.Size(280, 194)
        Me.SgaTileViewerControl1.TabIndex = 42
        Me.SgaTileViewerControl1.TileSize = New System.Drawing.Size(32, 32)
        Me.SgaTileViewerControl1.TilesPerRow = 8
        Me.SgaTileViewerControl1.VDP = Nothing
        '
        'SgaPlayerControl1
        '
        Me.SgaPlayerControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SgaPlayerControl1.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.SgaPlayerControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SgaPlayerControl1.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SgaPlayerControl1.Location = New System.Drawing.Point(0, 0)
        Me.SgaPlayerControl1.MinimumSize = New System.Drawing.Size(576, 490)
        Me.SgaPlayerControl1.Name = "SgaPlayerControl1"
        Me.SgaPlayerControl1.Size = New System.Drawing.Size(586, 496)
        Me.SgaPlayerControl1.TabIndex = 1
        '
        'SgaSequenceViewerControl1
        '
        Me.SgaSequenceViewerControl1.AudioColor = System.Drawing.Color.Green
        Me.SgaSequenceViewerControl1.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.SgaSequenceViewerControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SgaSequenceViewerControl1.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SgaSequenceViewerControl1.Location = New System.Drawing.Point(0, 0)
        Me.SgaSequenceViewerControl1.MinimumSize = New System.Drawing.Size(574, 240)
        Me.SgaSequenceViewerControl1.MiscColor = System.Drawing.Color.Purple
        Me.SgaSequenceViewerControl1.MixedColor = System.Drawing.Color.Teal
        Me.SgaSequenceViewerControl1.Name = "SgaSequenceViewerControl1"
        Me.SgaSequenceViewerControl1.Size = New System.Drawing.Size(878, 295)
        Me.SgaSequenceViewerControl1.TabIndex = 0
        Me.SgaSequenceViewerControl1.VideoColor = System.Drawing.Color.Blue
        '
        'frmMain
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(880, 841)
        Me.Controls.Add(Me.splMain)
        Me.Controls.Add(Me.MainToolStrip)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.MinimumSize = New System.Drawing.Size(896, 880)
        Me.Name = "frmMain"
        Me.Text = "SCAT"
        Me.MainToolStrip.ResumeLayout(False)
        Me.MainToolStrip.PerformLayout()
        Me.splMain.Panel1.ResumeLayout(False)
        Me.splMain.Panel1.PerformLayout()
        Me.splMain.Panel2.ResumeLayout(False)
        CType(Me.splMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splMain.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblPalette As System.Windows.Forms.Label
    Friend WithEvents lblBackgroundTiles As System.Windows.Forms.Label
    Friend WithEvents ToolTipMain As System.Windows.Forms.ToolTip
    Friend WithEvents OpenButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents ScreenshotButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents AboutButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents MainToolStrip As System.Windows.Forms.ToolStrip
    Friend WithEvents ExtractToolStripDropDownButton As ToolStripDropDownButton
    Friend WithEvents From3DOToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents tbxInfo As TextBox
    Friend WithEvents lblInfo As Label
    Friend WithEvents SgaTileViewerControl1 As SGATileViewerControl
    Friend WithEvents splMain As SplitContainer
    Friend WithEvents SgaPlayerControl1 As SGAPlayerControl
    Friend WithEvents SgaSequenceViewerControl1 As SGASequenceViewerControl
    Friend WithEvents SgaPaletteViewerControl1 As SGAPaletteViewerControl
    Friend WithEvents FromDoubleSwitchForWindows95ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OptionsButton As ToolStripDropDownButton
    Friend WithEvents ShowMacroblockOutlinesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SequencesFromNTMOVIEFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FilterVideoOutputToolStripMenuItem As ToolStripMenuItem
End Class
