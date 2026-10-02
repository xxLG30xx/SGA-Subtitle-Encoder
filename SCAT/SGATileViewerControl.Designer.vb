<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SGATileViewerControl
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
        Me.components = New System.ComponentModel.Container()
        Me.pbxDisplay = New System.Windows.Forms.PictureBox()
        Me.ToolTipMain = New System.Windows.Forms.ToolTip(Me.components)
        CType(Me.pbxDisplay, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pbxDisplay
        '
        Me.pbxDisplay.BackColor = System.Drawing.Color.Black
        Me.pbxDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbxDisplay.Location = New System.Drawing.Point(0, 0)
        Me.pbxDisplay.MinimumSize = New System.Drawing.Size(256, 128)
        Me.pbxDisplay.Name = "pbxDisplay"
        Me.pbxDisplay.Size = New System.Drawing.Size(256, 192)
        Me.pbxDisplay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.pbxDisplay.TabIndex = 35
        Me.pbxDisplay.TabStop = False
        '
        'ToolTipMain
        '
        Me.ToolTipMain.AutomaticDelay = 0
        Me.ToolTipMain.AutoPopDelay = 100000
        Me.ToolTipMain.InitialDelay = 0
        Me.ToolTipMain.ReshowDelay = 0
        Me.ToolTipMain.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info
        '
        'SGATileViewerControl
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.Controls.Add(Me.pbxDisplay)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "SGATileViewerControl"
        Me.Size = New System.Drawing.Size(280, 194)
        CType(Me.pbxDisplay, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents pbxDisplay As PictureBox
    Friend WithEvents ToolTipMain As ToolTip
End Class
