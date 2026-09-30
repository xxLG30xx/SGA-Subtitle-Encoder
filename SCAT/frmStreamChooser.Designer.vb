<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmStreamChooser
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.btnOK = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblVideo = New System.Windows.Forms.Label()
        Me.lblAudio = New System.Windows.Forms.Label()
        Me.cbxVideoSubstream = New System.Windows.Forms.ComboBox()
        Me.cbxAudioSubstream = New System.Windows.Forms.ComboBox()
        Me.cbxVideoOverlaySubstream = New System.Windows.Forms.ComboBox()
        Me.lblSub = New System.Windows.Forms.Label()
        Me.cbxMainStream = New System.Windows.Forms.ComboBox()
        Me.lblMain = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'btnOK
        '
        Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnOK.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnOK.ForeColor = System.Drawing.Color.White
        Me.btnOK.Location = New System.Drawing.Point(433, 9)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(79, 94)
        Me.btnOK.TabIndex = 0
        Me.btnOK.Text = "&OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(412, 37)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Multiple streams detected. Choose the streams to display." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "*IMPORTANT SAFETY TIP:" & _
    " Don't cross the streams -- it would be bad." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'lblVideo
        '
        Me.lblVideo.AutoSize = True
        Me.lblVideo.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblVideo.ForeColor = System.Drawing.Color.White
        Me.lblVideo.Location = New System.Drawing.Point(82, 54)
        Me.lblVideo.Name = "lblVideo"
        Me.lblVideo.Size = New System.Drawing.Size(50, 21)
        Me.lblVideo.TabIndex = 3
        Me.lblVideo.Text = "&Video"
        '
        'lblAudio
        '
        Me.lblAudio.AutoSize = True
        Me.lblAudio.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAudio.ForeColor = System.Drawing.Color.White
        Me.lblAudio.Location = New System.Drawing.Point(210, 54)
        Me.lblAudio.Name = "lblAudio"
        Me.lblAudio.Size = New System.Drawing.Size(51, 21)
        Me.lblAudio.TabIndex = 4
        Me.lblAudio.Text = "&Audio"
        '
        'cbxVideoStream
        '
        Me.cbxVideoSubstream.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxVideoSubstream.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbxVideoSubstream.FormattingEnabled = True
        Me.cbxVideoSubstream.Location = New System.Drawing.Point(82, 78)
        Me.cbxVideoSubstream.Name = "cbxVideoStream"
        Me.cbxVideoSubstream.Size = New System.Drawing.Size(60, 25)
        Me.cbxVideoSubstream.TabIndex = 5
        '
        'cbxAudioStream
        '
        Me.cbxAudioSubstream.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxAudioSubstream.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbxAudioSubstream.FormattingEnabled = True
        Me.cbxAudioSubstream.Location = New System.Drawing.Point(214, 78)
        Me.cbxAudioSubstream.Name = "cbxAudioStream"
        Me.cbxAudioSubstream.Size = New System.Drawing.Size(60, 25)
        Me.cbxAudioSubstream.TabIndex = 6
        '
        'cbxVideoSubStream
        '
        Me.cbxVideoOverlaySubstream.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxVideoOverlaySubstream.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbxVideoOverlaySubstream.FormattingEnabled = True
        Me.cbxVideoOverlaySubstream.Location = New System.Drawing.Point(148, 78)
        Me.cbxVideoOverlaySubstream.Name = "cbxVideoSubStream"
        Me.cbxVideoOverlaySubstream.Size = New System.Drawing.Size(60, 25)
        Me.cbxVideoOverlaySubstream.TabIndex = 8
        '
        'lblSub
        '
        Me.lblSub.AutoSize = True
        Me.lblSub.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSub.ForeColor = System.Drawing.Color.White
        Me.lblSub.Location = New System.Drawing.Point(144, 54)
        Me.lblSub.Name = "lblSub"
        Me.lblSub.Size = New System.Drawing.Size(37, 21)
        Me.lblSub.TabIndex = 7
        Me.lblSub.Text = "&Sub"
        '
        'cbxMainStream
        '
        Me.cbxMainStream.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxMainStream.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbxMainStream.FormattingEnabled = True
        Me.cbxMainStream.Location = New System.Drawing.Point(16, 78)
        Me.cbxMainStream.Name = "cbxMainStream"
        Me.cbxMainStream.Size = New System.Drawing.Size(60, 25)
        Me.cbxMainStream.TabIndex = 10
        '
        'lblMain
        '
        Me.lblMain.AutoSize = True
        Me.lblMain.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMain.ForeColor = System.Drawing.Color.White
        Me.lblMain.Location = New System.Drawing.Point(12, 54)
        Me.lblMain.Name = "lblMain"
        Me.lblMain.Size = New System.Drawing.Size(45, 21)
        Me.lblMain.TabIndex = 9
        Me.lblMain.Text = "&Main"
        '
        'frmStreamChooser
        '
        Me.AcceptButton = Me.btnOK
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(520, 111)
        Me.ControlBox = False
        Me.Controls.Add(Me.cbxMainStream)
        Me.Controls.Add(Me.lblMain)
        Me.Controls.Add(Me.cbxVideoOverlaySubstream)
        Me.Controls.Add(Me.lblSub)
        Me.Controls.Add(Me.cbxAudioSubstream)
        Me.Controls.Add(Me.cbxVideoSubstream)
        Me.Controls.Add(Me.lblAudio)
        Me.Controls.Add(Me.lblVideo)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnOK)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmStreamChooser"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Choose streams"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblVideo As System.Windows.Forms.Label
    Friend WithEvents lblAudio As System.Windows.Forms.Label
    Friend WithEvents cbxVideoSubstream As System.Windows.Forms.ComboBox
    Friend WithEvents cbxAudioSubstream As System.Windows.Forms.ComboBox
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents cbxVideoOverlaySubstream As System.Windows.Forms.ComboBox
    Friend WithEvents lblSub As System.Windows.Forms.Label
    Friend WithEvents cbxMainStream As System.Windows.Forms.ComboBox
    Friend WithEvents lblMain As System.Windows.Forms.Label

End Class
