<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLZCompression
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
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnDecompress = New System.Windows.Forms.Button()
        Me.txtCompressed = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.nudUniqueColors = New System.Windows.Forms.NumericUpDown()
        Me.nudUniqueTiles = New System.Windows.Forms.NumericUpDown()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnGenerateBitmap = New System.Windows.Forms.Button()
        Me.txtDecompressed = New System.Windows.Forms.TextBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.nudWidth = New System.Windows.Forms.NumericUpDown()
        Me.nudHeight = New System.Windows.Forms.NumericUpDown()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        CType(Me.nudUniqueColors, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudUniqueTiles, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudWidth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudHeight, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TableLayoutPanel1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.TableLayoutPanel2)
        Me.SplitContainer1.Size = New System.Drawing.Size(784, 561)
        Me.SplitContainer1.SplitterDistance = 388
        Me.SplitContainer1.SplitterWidth = 5
        Me.SplitContainer1.TabIndex = 0
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.btnDecompress, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.txtCompressed, 0, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(388, 561)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'btnDecompress
        '
        Me.btnDecompress.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnDecompress.Location = New System.Drawing.Point(3, 507)
        Me.btnDecompress.Name = "btnDecompress"
        Me.btnDecompress.Size = New System.Drawing.Size(382, 51)
        Me.btnDecompress.TabIndex = 0
        Me.btnDecompress.Text = "&Decompress"
        Me.btnDecompress.UseVisualStyleBackColor = True
        '
        'txtCompressed
        '
        Me.txtCompressed.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtCompressed.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCompressed.Location = New System.Drawing.Point(3, 3)
        Me.txtCompressed.MaxLength = 1000000
        Me.txtCompressed.Multiline = True
        Me.txtCompressed.Name = "txtCompressed"
        Me.txtCompressed.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtCompressed.Size = New System.Drawing.Size(382, 498)
        Me.txtCompressed.TabIndex = 1
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 4
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.nudUniqueColors, 3, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.nudUniqueTiles, 3, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.Label4, 2, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.Label3, 2, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.Label2, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.btnGenerateBitmap, 0, 4)
        Me.TableLayoutPanel2.Controls.Add(Me.txtDecompressed, 0, 2)
        Me.TableLayoutPanel2.Controls.Add(Me.PictureBox1, 0, 3)
        Me.TableLayoutPanel2.Controls.Add(Me.Label1, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.nudWidth, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.nudHeight, 1, 1)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 5
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(391, 561)
        Me.TableLayoutPanel2.TabIndex = 3
        '
        'nudUniqueColors
        '
        Me.nudUniqueColors.Dock = System.Windows.Forms.DockStyle.Fill
        Me.nudUniqueColors.Location = New System.Drawing.Point(315, 31)
        Me.nudUniqueColors.Maximum = New Decimal(New Integer() {32768, 0, 0, 0})
        Me.nudUniqueColors.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudUniqueColors.Name = "nudUniqueColors"
        Me.nudUniqueColors.Size = New System.Drawing.Size(73, 25)
        Me.nudUniqueColors.TabIndex = 13
        Me.nudUniqueColors.Value = New Decimal(New Integer() {255, 0, 0, 0})
        '
        'nudUniqueTiles
        '
        Me.nudUniqueTiles.Dock = System.Windows.Forms.DockStyle.Fill
        Me.nudUniqueTiles.Location = New System.Drawing.Point(315, 3)
        Me.nudUniqueTiles.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.nudUniqueTiles.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudUniqueTiles.Name = "nudUniqueTiles"
        Me.nudUniqueTiles.Size = New System.Drawing.Size(73, 25)
        Me.nudUniqueTiles.TabIndex = 12
        Me.nudUniqueTiles.Value = New Decimal(New Integer() {1161, 0, 0, 0})
        '
        'Label4
        '
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Location = New System.Drawing.Point(198, 28)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(111, 28)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "Unique colors"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(198, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(111, 28)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "Unique tiles"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(3, 28)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(111, 28)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Height in tiles"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnGenerateBitmap
        '
        Me.TableLayoutPanel2.SetColumnSpan(Me.btnGenerateBitmap, 4)
        Me.btnGenerateBitmap.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnGenerateBitmap.Enabled = False
        Me.btnGenerateBitmap.Location = New System.Drawing.Point(3, 507)
        Me.btnGenerateBitmap.Name = "btnGenerateBitmap"
        Me.btnGenerateBitmap.Size = New System.Drawing.Size(385, 51)
        Me.btnGenerateBitmap.TabIndex = 5
        Me.btnGenerateBitmap.Text = "&Generate Bitmap"
        Me.btnGenerateBitmap.UseVisualStyleBackColor = True
        '
        'txtDecompressed
        '
        Me.TableLayoutPanel2.SetColumnSpan(Me.txtDecompressed, 4)
        Me.txtDecompressed.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtDecompressed.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDecompressed.Location = New System.Drawing.Point(3, 59)
        Me.txtDecompressed.MaxLength = 1000000
        Me.txtDecompressed.Multiline = True
        Me.txtDecompressed.Name = "txtDecompressed"
        Me.txtDecompressed.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtDecompressed.Size = New System.Drawing.Size(385, 218)
        Me.txtDecompressed.TabIndex = 2
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.Black
        Me.TableLayoutPanel2.SetColumnSpan(Me.PictureBox1, 4)
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Location = New System.Drawing.Point(3, 283)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(385, 218)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 3
        Me.PictureBox1.TabStop = False
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(111, 28)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Width in tiles"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'nudWidth
        '
        Me.nudWidth.Dock = System.Windows.Forms.DockStyle.Fill
        Me.nudWidth.Location = New System.Drawing.Point(120, 3)
        Me.nudWidth.Maximum = New Decimal(New Integer() {64, 0, 0, 0})
        Me.nudWidth.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudWidth.Name = "nudWidth"
        Me.nudWidth.Size = New System.Drawing.Size(72, 25)
        Me.nudWidth.TabIndex = 8
        Me.nudWidth.Value = New Decimal(New Integer() {40, 0, 0, 0})
        '
        'nudHeight
        '
        Me.nudHeight.Dock = System.Windows.Forms.DockStyle.Fill
        Me.nudHeight.Location = New System.Drawing.Point(120, 31)
        Me.nudHeight.Maximum = New Decimal(New Integer() {64, 0, 0, 0})
        Me.nudHeight.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudHeight.Name = "nudHeight"
        Me.nudHeight.Size = New System.Drawing.Size(72, 25)
        Me.nudHeight.TabIndex = 9
        Me.nudHeight.Value = New Decimal(New Integer() {34, 0, 0, 0})
        '
        'frmLZCompression
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 561)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "frmLZCompression"
        Me.Text = "LZ Compression"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        CType(Me.nudUniqueColors, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudUniqueTiles, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudWidth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudHeight, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents btnDecompress As Button
    Friend WithEvents txtCompressed As TextBox
    Friend WithEvents txtDecompressed As TextBox
    Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
    Friend WithEvents btnGenerateBitmap As Button
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents nudWidth As NumericUpDown
    Friend WithEvents nudHeight As NumericUpDown
    Friend WithEvents nudUniqueColors As NumericUpDown
    Friend WithEvents nudUniqueTiles As NumericUpDown
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
End Class
