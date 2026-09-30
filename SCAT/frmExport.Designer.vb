<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmExport
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
        Me.ConversionStatusLabel = New System.Windows.Forms.Label()
        Me.ConversionProgressBar = New System.Windows.Forms.ProgressBar()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.pbxVideoFrame = New System.Windows.Forms.PictureBox()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        CType(Me.pbxVideoFrame, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ConversionStatusLabel
        '
        Me.ConversionStatusLabel.BackColor = System.Drawing.Color.Transparent
        Me.ConversionStatusLabel.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ConversionStatusLabel.ForeColor = System.Drawing.Color.White
        Me.ConversionStatusLabel.Location = New System.Drawing.Point(8, 8)
        Me.ConversionStatusLabel.Name = "ConversionStatusLabel"
        Me.ConversionStatusLabel.Size = New System.Drawing.Size(232, 24)
        Me.ConversionStatusLabel.TabIndex = 13
        Me.ConversionStatusLabel.Text = "Ready"
        Me.ConversionStatusLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'ConversionProgressBar
        '
        Me.ConversionProgressBar.ForeColor = System.Drawing.Color.Gray
        Me.ConversionProgressBar.Location = New System.Drawing.Point(8, 40)
        Me.ConversionProgressBar.Name = "ConversionProgressBar"
        Me.ConversionProgressBar.Size = New System.Drawing.Size(232, 24)
        Me.ConversionProgressBar.Step = 1
        Me.ConversionProgressBar.TabIndex = 14
        '
        'Cancel_Button
        '
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Cancel_Button.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Cancel_Button.ForeColor = System.Drawing.Color.White
        Me.Cancel_Button.Location = New System.Drawing.Point(248, 8)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(80, 56)
        Me.Cancel_Button.TabIndex = 15
        Me.Cancel_Button.Text = "&Cancel"
        Me.Cancel_Button.UseVisualStyleBackColor = True
        '
        'pbxVideoFrame
        '
        Me.pbxVideoFrame.BackColor = System.Drawing.Color.Black
        Me.pbxVideoFrame.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbxVideoFrame.Location = New System.Drawing.Point(8, 72)
        Me.pbxVideoFrame.Name = "pbxVideoFrame"
        Me.pbxVideoFrame.Size = New System.Drawing.Size(320, 224)
        Me.pbxVideoFrame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pbxVideoFrame.TabIndex = 16
        Me.pbxVideoFrame.TabStop = False
        '
        'frmConvert
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(337, 305)
        Me.Controls.Add(Me.pbxVideoFrame)
        Me.Controls.Add(Me.Cancel_Button)
        Me.Controls.Add(Me.ConversionStatusLabel)
        Me.Controls.Add(Me.ConversionProgressBar)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.Name = "frmConvert"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Converting..."
        CType(Me.pbxVideoFrame, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ConversionStatusLabel As System.Windows.Forms.Label
    Friend WithEvents ConversionProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents pbxVideoFrame As System.Windows.Forms.PictureBox
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
End Class
