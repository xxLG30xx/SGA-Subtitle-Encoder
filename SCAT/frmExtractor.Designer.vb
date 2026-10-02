<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial MustInherit Class frmExtractor
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
        Me.btnExtract = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.pbrStatus = New System.Windows.Forms.ProgressBar()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.tvwItems = New System.Windows.Forms.TreeView()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnExtract
        '
        Me.btnExtract.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnExtract.BackgroundImage = Global.SCAT.My.Resources.Resources._1x1
        Me.TableLayoutPanel1.SetColumnSpan(Me.btnExtract, 2)
        Me.btnExtract.Enabled = False
        Me.btnExtract.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnExtract.Location = New System.Drawing.Point(4, 296)
        Me.btnExtract.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnExtract.Name = "btnExtract"
        Me.btnExtract.Size = New System.Drawing.Size(576, 40)
        Me.btnExtract.TabIndex = 1
        Me.btnExtract.Text = "Extract..."
        Me.btnExtract.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.pbrStatus, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.lblStatus, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.tvwItems, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnExtract, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(584, 361)
        Me.TableLayoutPanel1.TabIndex = 3
        '
        'pbrStatus
        '
        Me.pbrStatus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbrStatus.Location = New System.Drawing.Point(295, 344)
        Me.pbrStatus.Name = "pbrStatus"
        Me.pbrStatus.Size = New System.Drawing.Size(286, 14)
        Me.pbrStatus.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.pbrStatus.TabIndex = 4
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblStatus.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStatus.Location = New System.Drawing.Point(3, 341)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(286, 20)
        Me.lblStatus.TabIndex = 5
        Me.lblStatus.Text = "Ready"
        '
        'tvwSequences
        '
        Me.tvwItems.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.TableLayoutPanel1.SetColumnSpan(Me.tvwItems, 2)
        Me.tvwItems.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvwItems.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tvwItems.ForeColor = System.Drawing.Color.White
        Me.tvwItems.Location = New System.Drawing.Point(3, 3)
        Me.tvwItems.Name = "tvwSequences"
        Me.tvwItems.Size = New System.Drawing.Size(578, 285)
        Me.tvwItems.TabIndex = 6
        '
        'frm3DOSequenceExtractor
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(584, 361)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ForeColor = System.Drawing.Color.White
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MinimumSize = New System.Drawing.Size(600, 400)
        Me.Name = "frm3DOSequenceExtractor"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents btnExtract As Button
    Friend WithEvents pbrStatus As ProgressBar
    Friend WithEvents lblStatus As Label
    Friend WithEvents tvwItems As TreeView
End Class
