<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SGAPlayerControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SGAPlayerControl))
        Me.trbarFrameNumber = New System.Windows.Forms.TrackBar()
        Me.pbxVideoFrame = New System.Windows.Forms.PictureBox()
        Me.btnPlayPause = New System.Windows.Forms.Button()
        Me.lblCurrentTime = New System.Windows.Forms.Label()
        Me.lblTotalTime = New System.Windows.Forms.Label()
        Me.pbxAudioFrame = New System.Windows.Forms.PictureBox()
        CType(Me.trbarFrameNumber, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbxVideoFrame, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbxAudioFrame, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'trbarFrameNumber
        '
        Me.trbarFrameNumber.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trbarFrameNumber.AutoSize = False
        Me.trbarFrameNumber.Enabled = False
        Me.trbarFrameNumber.LargeChange = 15
        Me.trbarFrameNumber.Location = New System.Drawing.Point(0, 398)
        Me.trbarFrameNumber.Maximum = 0
        Me.trbarFrameNumber.Name = "trbarFrameNumber"
        Me.trbarFrameNumber.Size = New System.Drawing.Size(576, 37)
        Me.trbarFrameNumber.TabIndex = 7
        Me.trbarFrameNumber.TickStyle = System.Windows.Forms.TickStyle.TopLeft
        '
        'pbxVideoFrame
        '
        Me.pbxVideoFrame.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbxVideoFrame.BackColor = System.Drawing.Color.Black
        Me.pbxVideoFrame.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.pbxVideoFrame.Location = New System.Drawing.Point(0, 0)
        Me.pbxVideoFrame.MinimumSize = New System.Drawing.Size(576, 320)
        Me.pbxVideoFrame.Name = "pbxVideoFrame"
        Me.pbxVideoFrame.Size = New System.Drawing.Size(576, 320)
        Me.pbxVideoFrame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pbxVideoFrame.TabIndex = 0
        Me.pbxVideoFrame.TabStop = False
        '
        'btnPlayPause
        '
        Me.btnPlayPause.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPlayPause.Enabled = False
        Me.btnPlayPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPlayPause.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPlayPause.ForeColor = System.Drawing.Color.White
        Me.btnPlayPause.Image = CType(resources.GetObject("btnPlayPause.Image"), System.Drawing.Image)
        Me.btnPlayPause.Location = New System.Drawing.Point(244, 438)
        Me.btnPlayPause.Margin = New System.Windows.Forms.Padding(0)
        Me.btnPlayPause.MinimumSize = New System.Drawing.Size(88, 44)
        Me.btnPlayPause.Name = "btnPlayPause"
        Me.btnPlayPause.Size = New System.Drawing.Size(88, 44)
        Me.btnPlayPause.TabIndex = 8
        Me.btnPlayPause.UseVisualStyleBackColor = True
        '
        'lblCurrentTime
        '
        Me.lblCurrentTime.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblCurrentTime.AutoSize = True
        Me.lblCurrentTime.BackColor = System.Drawing.Color.Transparent
        Me.lblCurrentTime.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCurrentTime.ForeColor = System.Drawing.Color.White
        Me.lblCurrentTime.Location = New System.Drawing.Point(3, 444)
        Me.lblCurrentTime.Name = "lblCurrentTime"
        Me.lblCurrentTime.Size = New System.Drawing.Size(134, 32)
        Me.lblCurrentTime.TabIndex = 9
        Me.lblCurrentTime.Text = "00:00:00:00"
        Me.lblCurrentTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTotalTime
        '
        Me.lblTotalTime.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblTotalTime.AutoSize = True
        Me.lblTotalTime.BackColor = System.Drawing.Color.Transparent
        Me.lblTotalTime.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotalTime.ForeColor = System.Drawing.Color.White
        Me.lblTotalTime.Location = New System.Drawing.Point(439, 444)
        Me.lblTotalTime.Name = "lblTotalTime"
        Me.lblTotalTime.Size = New System.Drawing.Size(134, 32)
        Me.lblTotalTime.TabIndex = 10
        Me.lblTotalTime.Text = "00:00:00:00"
        Me.lblTotalTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'pbxAudioFrame
        '
        Me.pbxAudioFrame.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbxAudioFrame.BackColor = System.Drawing.Color.Black
        Me.pbxAudioFrame.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.pbxAudioFrame.Location = New System.Drawing.Point(0, 326)
        Me.pbxAudioFrame.MinimumSize = New System.Drawing.Size(576, 64)
        Me.pbxAudioFrame.Name = "pbxAudioFrame"
        Me.pbxAudioFrame.Size = New System.Drawing.Size(576, 64)
        Me.pbxAudioFrame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pbxAudioFrame.TabIndex = 25
        Me.pbxAudioFrame.TabStop = False
        '
        'SGAPlayerControl
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.Controls.Add(Me.pbxAudioFrame)
        Me.Controls.Add(Me.lblTotalTime)
        Me.Controls.Add(Me.lblCurrentTime)
        Me.Controls.Add(Me.btnPlayPause)
        Me.Controls.Add(Me.trbarFrameNumber)
        Me.Controls.Add(Me.pbxVideoFrame)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MinimumSize = New System.Drawing.Size(576, 490)
        Me.Name = "SGAPlayerControl"
        Me.Size = New System.Drawing.Size(576, 490)
        CType(Me.trbarFrameNumber, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbxVideoFrame, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbxAudioFrame, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents pbxVideoFrame As PictureBox
    Friend WithEvents trbarFrameNumber As TrackBar
    Friend WithEvents btnPlayPause As Button
    Friend WithEvents lblCurrentTime As Label
    Friend WithEvents lblTotalTime As Label
    Friend WithEvents pbxAudioFrame As PictureBox
End Class
