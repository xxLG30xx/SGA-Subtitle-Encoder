<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SGASequenceViewerControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If

            'Added on 2016/3/3
            _fillBrush.Dispose()
            _frameLinePen.Dispose()
            _highlightBrush.Dispose()
            _highlightPen.Dispose()
            _secondLinePen.Dispose()
            _shadowPen.Dispose()
            _trackSeparatorLinePen.Dispose()

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SGASequenceViewerControl))
        Me.pbxItemDisplay = New System.Windows.Forms.PictureBox()
        Me.ToolTipMain = New System.Windows.Forms.ToolTip(Me.components)
        Me.RightTimelinePanel = New System.Windows.Forms.Panel()
        Me.MainToolStripContainer = New System.Windows.Forms.ToolStripContainer()
        Me.ToolStripMain = New System.Windows.Forms.ToolStrip()
        Me.tsddbtnExport = New System.Windows.Forms.ToolStripDropDownButton()
        Me.tsmiExportCurrentPreview = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiExportAllClips = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsddbtnSelect = New System.Windows.Forms.ToolStripDropDownButton()
        Me.tsmiSelectAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiSelectNone = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsddbtnView = New System.Windows.Forms.ToolStripDropDownButton()
        Me.tsmiClips = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiChunks = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.tslblZoom = New System.Windows.Forms.ToolStripLabel()
        Me.tsbtnHPlus = New System.Windows.Forms.ToolStripButton()
        Me.tsbtnHMinus = New System.Windows.Forms.ToolStripButton()
        Me.tsbtnVMinus = New System.Windows.Forms.ToolStripButton()
        Me.tsbtnVPlus = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        CType(Me.pbxItemDisplay, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RightTimelinePanel.SuspendLayout()
        Me.MainToolStripContainer.ContentPanel.SuspendLayout()
        Me.MainToolStripContainer.TopToolStripPanel.SuspendLayout()
        Me.MainToolStripContainer.SuspendLayout()
        Me.ToolStripMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'pbxItemDisplay
        '
        Me.pbxItemDisplay.BackColor = System.Drawing.Color.Transparent
        Me.pbxItemDisplay.Location = New System.Drawing.Point(0, 0)
        Me.pbxItemDisplay.Name = "pbxItemDisplay"
        Me.pbxItemDisplay.Size = New System.Drawing.Size(72, 24)
        Me.pbxItemDisplay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.pbxItemDisplay.TabIndex = 29
        Me.pbxItemDisplay.TabStop = False
        '
        'ToolTipMain
        '
        Me.ToolTipMain.AutomaticDelay = 0
        Me.ToolTipMain.AutoPopDelay = 100000
        Me.ToolTipMain.InitialDelay = 0
        Me.ToolTipMain.ReshowDelay = 0
        Me.ToolTipMain.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info
        '
        'RightTimelinePanel
        '
        Me.RightTimelinePanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RightTimelinePanel.AutoScroll = True
        Me.RightTimelinePanel.Controls.Add(Me.pbxItemDisplay)
        Me.RightTimelinePanel.Location = New System.Drawing.Point(0, 3)
        Me.RightTimelinePanel.MinimumSize = New System.Drawing.Size(64, 64)
        Me.RightTimelinePanel.Name = "RightTimelinePanel"
        Me.RightTimelinePanel.Size = New System.Drawing.Size(700, 382)
        Me.RightTimelinePanel.TabIndex = 1
        '
        'MainToolStripContainer
        '
        '
        'MainToolStripContainer.ContentPanel
        '
        Me.MainToolStripContainer.ContentPanel.Controls.Add(Me.RightTimelinePanel)
        Me.MainToolStripContainer.ContentPanel.Size = New System.Drawing.Size(700, 385)
        Me.MainToolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MainToolStripContainer.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MainToolStripContainer.Location = New System.Drawing.Point(0, 0)
        Me.MainToolStripContainer.Name = "MainToolStripContainer"
        Me.MainToolStripContainer.Size = New System.Drawing.Size(700, 410)
        Me.MainToolStripContainer.TabIndex = 30
        Me.MainToolStripContainer.Text = "ToolStripContainer"
        '
        'MainToolStripContainer.TopToolStripPanel
        '
        Me.MainToolStripContainer.TopToolStripPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.MainToolStripContainer.TopToolStripPanel.Controls.Add(Me.ToolStripMain)
        Me.MainToolStripContainer.TopToolStripPanel.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MainToolStripContainer.TopToolStripPanel.ForeColor = System.Drawing.Color.Black
        Me.MainToolStripContainer.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        '
        'ToolStripMain
        '
        Me.ToolStripMain.Dock = System.Windows.Forms.DockStyle.None
        Me.ToolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStripMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsddbtnExport, Me.ToolStripSeparator1, Me.tsddbtnSelect, Me.ToolStripSeparator2, Me.tsddbtnView, Me.ToolStripSeparator3, Me.tslblZoom, Me.tsbtnHPlus, Me.tsbtnHMinus, Me.tsbtnVMinus, Me.tsbtnVPlus, Me.ToolStripSeparator4})
        Me.ToolStripMain.Location = New System.Drawing.Point(3, 0)
        Me.ToolStripMain.Name = "ToolStripMain"
        Me.ToolStripMain.Size = New System.Drawing.Size(314, 25)
        Me.ToolStripMain.TabIndex = 0
        '
        'tsddbtnExport
        '
        Me.tsddbtnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsddbtnExport.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiExportCurrentPreview, Me.tsmiExportAllClips})
        Me.tsddbtnExport.Enabled = False
        Me.tsddbtnExport.Image = CType(resources.GetObject("tsddbtnExport.Image"), System.Drawing.Image)
        Me.tsddbtnExport.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsddbtnExport.Name = "tsddbtnExport"
        Me.tsddbtnExport.Size = New System.Drawing.Size(53, 22)
        Me.tsddbtnExport.Text = "&Export"
        '
        'tsmiExportCurrentPreview
        '
        Me.tsmiExportCurrentPreview.Name = "tsmiExportCurrentPreview"
        Me.tsmiExportCurrentPreview.Size = New System.Drawing.Size(158, 22)
        Me.tsmiExportCurrentPreview.Text = "Current Preview"
        Me.tsmiExportCurrentPreview.ToolTipText = "Export the current preview as a single file"
        '
        'tsmiExportAllClips
        '
        Me.tsmiExportAllClips.Name = "tsmiExportAllClips"
        Me.tsmiExportAllClips.Size = New System.Drawing.Size(158, 22)
        Me.tsmiExportAllClips.Text = "All Clips"
        Me.tsmiExportAllClips.ToolTipText = "Export each clip as an individual file"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.ForeColor = System.Drawing.Color.White
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'tsddbtnSelect
        '
        Me.tsddbtnSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsddbtnSelect.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiSelectAll, Me.tsmiSelectNone})
        Me.tsddbtnSelect.Enabled = False
        Me.tsddbtnSelect.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tsddbtnSelect.ForeColor = System.Drawing.SystemColors.ControlText
        Me.tsddbtnSelect.Image = CType(resources.GetObject("tsddbtnSelect.Image"), System.Drawing.Image)
        Me.tsddbtnSelect.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsddbtnSelect.Name = "tsddbtnSelect"
        Me.tsddbtnSelect.Size = New System.Drawing.Size(55, 22)
        Me.tsddbtnSelect.Text = "&Select"
        Me.tsddbtnSelect.ToolTipText = "Select"
        '
        'tsmiSelectAll
        '
        Me.tsmiSelectAll.Name = "tsmiSelectAll"
        Me.tsmiSelectAll.Size = New System.Drawing.Size(108, 22)
        Me.tsmiSelectAll.Text = "&All"
        Me.tsmiSelectAll.ToolTipText = "Select all clips"
        '
        'tsmiSelectNone
        '
        Me.tsmiSelectNone.Name = "tsmiSelectNone"
        Me.tsmiSelectNone.Size = New System.Drawing.Size(108, 22)
        Me.tsmiSelectNone.Text = "&None"
        Me.tsmiSelectNone.ToolTipText = "Deselect all clips"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.ForeColor = System.Drawing.Color.White
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'tsddbtnView
        '
        Me.tsddbtnView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsddbtnView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiClips, Me.tsmiChunks})
        Me.tsddbtnView.Enabled = False
        Me.tsddbtnView.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tsddbtnView.ForeColor = System.Drawing.SystemColors.ControlText
        Me.tsddbtnView.Image = CType(resources.GetObject("tsddbtnView.Image"), System.Drawing.Image)
        Me.tsddbtnView.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsddbtnView.Name = "tsddbtnView"
        Me.tsddbtnView.Size = New System.Drawing.Size(48, 22)
        Me.tsddbtnView.Text = "&View"
        '
        'tsmiClips
        '
        Me.tsmiClips.Checked = True
        Me.tsmiClips.CheckOnClick = True
        Me.tsmiClips.CheckState = System.Windows.Forms.CheckState.Checked
        Me.tsmiClips.Name = "tsmiClips"
        Me.tsmiClips.Size = New System.Drawing.Size(117, 22)
        Me.tsmiClips.Text = "Clips"
        Me.tsmiClips.ToolTipText = "View the current file as a sequence of video and audio clips"
        '
        'tsmiChunks
        '
        Me.tsmiChunks.CheckOnClick = True
        Me.tsmiChunks.Name = "tsmiChunks"
        Me.tsmiChunks.Size = New System.Drawing.Size(117, 22)
        Me.tsmiChunks.Text = "Chunks"
        Me.tsmiChunks.ToolTipText = "View the current file as a sequence of data chunks in their original order"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.ForeColor = System.Drawing.Color.White
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
        '
        'tslblZoom
        '
        Me.tslblZoom.Enabled = False
        Me.tslblZoom.ForeColor = System.Drawing.SystemColors.ControlText
        Me.tslblZoom.Name = "tslblZoom"
        Me.tslblZoom.Size = New System.Drawing.Size(39, 22)
        Me.tslblZoom.Text = "Zoom"
        '
        'tsbtnHPlus
        '
        Me.tsbtnHPlus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbtnHPlus.Enabled = False
        Me.tsbtnHPlus.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tsbtnHPlus.ForeColor = System.Drawing.SystemColors.ControlText
        Me.tsbtnHPlus.Image = CType(resources.GetObject("tsbtnHPlus.Image"), System.Drawing.Image)
        Me.tsbtnHPlus.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbtnHPlus.Name = "tsbtnHPlus"
        Me.tsbtnHPlus.Size = New System.Drawing.Size(23, 22)
        Me.tsbtnHPlus.Text = "→"
        Me.tsbtnHPlus.ToolTipText = "Increase horizontal zoom"
        '
        'tsbtnHMinus
        '
        Me.tsbtnHMinus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbtnHMinus.Enabled = False
        Me.tsbtnHMinus.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tsbtnHMinus.ForeColor = System.Drawing.SystemColors.ControlText
        Me.tsbtnHMinus.Image = CType(resources.GetObject("tsbtnHMinus.Image"), System.Drawing.Image)
        Me.tsbtnHMinus.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbtnHMinus.Name = "tsbtnHMinus"
        Me.tsbtnHMinus.Size = New System.Drawing.Size(23, 22)
        Me.tsbtnHMinus.Text = "←"
        Me.tsbtnHMinus.ToolTipText = "Decrease horizontal zoom"
        '
        'tsbtnVMinus
        '
        Me.tsbtnVMinus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbtnVMinus.Enabled = False
        Me.tsbtnVMinus.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tsbtnVMinus.ForeColor = System.Drawing.SystemColors.ControlText
        Me.tsbtnVMinus.Image = CType(resources.GetObject("tsbtnVMinus.Image"), System.Drawing.Image)
        Me.tsbtnVMinus.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbtnVMinus.Name = "tsbtnVMinus"
        Me.tsbtnVMinus.Size = New System.Drawing.Size(23, 22)
        Me.tsbtnVMinus.Text = "↓"
        Me.tsbtnVMinus.ToolTipText = "Increase vertical zoom"
        '
        'tsbtnVPlus
        '
        Me.tsbtnVPlus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbtnVPlus.Enabled = False
        Me.tsbtnVPlus.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tsbtnVPlus.ForeColor = System.Drawing.SystemColors.ControlText
        Me.tsbtnVPlus.Image = CType(resources.GetObject("tsbtnVPlus.Image"), System.Drawing.Image)
        Me.tsbtnVPlus.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbtnVPlus.Name = "tsbtnVPlus"
        Me.tsbtnVPlus.Size = New System.Drawing.Size(23, 22)
        Me.tsbtnVPlus.Text = "↑"
        Me.tsbtnVPlus.ToolTipText = "Decrease horizontal zoom"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 25)
        '
        'SGASequenceViewerControl
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.Controls.Add(Me.MainToolStripContainer)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MinimumSize = New System.Drawing.Size(700, 300)
        Me.Name = "SGASequenceViewerControl"
        Me.Size = New System.Drawing.Size(700, 410)
        CType(Me.pbxItemDisplay, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RightTimelinePanel.ResumeLayout(False)
        Me.RightTimelinePanel.PerformLayout()
        Me.MainToolStripContainer.ContentPanel.ResumeLayout(False)
        Me.MainToolStripContainer.TopToolStripPanel.ResumeLayout(False)
        Me.MainToolStripContainer.TopToolStripPanel.PerformLayout()
        Me.MainToolStripContainer.ResumeLayout(False)
        Me.MainToolStripContainer.PerformLayout()
        Me.ToolStripMain.ResumeLayout(False)
        Me.ToolStripMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pbxItemDisplay As PictureBox
    Friend WithEvents ToolTipMain As ToolTip
    Friend WithEvents RightTimelinePanel As Panel
    Friend WithEvents MainToolStripContainer As ToolStripContainer
    Friend WithEvents ToolStripMain As ToolStrip
    Friend WithEvents tsddbtnSelect As ToolStripDropDownButton
    Friend WithEvents tsmiSelectAll As ToolStripMenuItem
    Friend WithEvents tsmiSelectNone As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents tsddbtnView As ToolStripDropDownButton
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents tsmiClips As ToolStripMenuItem
    Friend WithEvents tsmiChunks As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents tsbtnHPlus As ToolStripButton
    Friend WithEvents tsbtnHMinus As ToolStripButton
    Friend WithEvents tsbtnVPlus As ToolStripButton
    Friend WithEvents tsbtnVMinus As ToolStripButton
    Friend WithEvents tslblZoom As ToolStripLabel
    Friend WithEvents tsddbtnExport As ToolStripDropDownButton
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents tsmiExportCurrentPreview As ToolStripMenuItem
    Friend WithEvents tsmiExportAllClips As ToolStripMenuItem
End Class
