Imports System.ComponentModel

Public Class CustomToolStripRenderer
    Inherits ToolStripSystemRenderer

    <Browsable(True), Category("Appearance")>
    Public Property BackColor As Color = Color.FromArgb(64, 64, 64)

    Private _backBrush As SolidBrush
    Protected ReadOnly Property BackBrush As SolidBrush
        Get
            If _backBrush Is Nothing Then
                _backBrush = New SolidBrush(BackColor)
            End If
            Return _backBrush
        End Get
    End Property

    <Browsable(True), Category("Appearance")>
    Public Property ForeColor As Color = Color.White

    Protected Overrides Sub OnRenderArrow(e As ToolStripArrowRenderEventArgs)
        If e.Item.Enabled = True Then
            e.ArrowColor = ForeColor
        End If
        MyBase.OnRenderArrow(e)
    End Sub

    Protected Overrides Sub OnRenderButtonBackground(e As ToolStripItemRenderEventArgs)
        MyBase.OnRenderButtonBackground(e)
    End Sub

    Protected Overrides Sub OnRenderDropDownButtonBackground(e As ToolStripItemRenderEventArgs)
        MyBase.OnRenderDropDownButtonBackground(e)
    End Sub

    Protected Overrides Sub OnRenderGrip(e As ToolStripGripRenderEventArgs)
        MyBase.OnRenderGrip(e)
    End Sub

    Protected Overrides Sub OnRenderImageMargin(e As ToolStripRenderEventArgs)
        MyBase.OnRenderImageMargin(e)
    End Sub

    Protected Overrides Sub OnRenderItemBackground(e As ToolStripItemRenderEventArgs)
        MyBase.OnRenderItemBackground(e)
        If e.Item.Selected Then
            e.Graphics.FillRectangle(Brushes.Blue, e.Item.ContentRectangle)
        End If
    End Sub

    Protected Overrides Sub OnRenderItemCheck(e As ToolStripItemImageRenderEventArgs)
        Using g As Graphics = Graphics.FromImage(e.Image)
            With g
                .Clear(Color.Transparent)
                Using font As New Font("Wingdings", 10)
                    Using brush As New SolidBrush(ForeColor)
                        .DrawString("l", font, brush, 0, 0)
                    End Using
                End Using
                If e.Item.Enabled = False Then
                    g.DrawImage(CreateDisabledImage(e.Image), 0, 0)
                End If
            End With
        End Using

        MyBase.OnRenderItemCheck(e)
    End Sub

    Protected Overrides Sub OnRenderItemImage(e As ToolStripItemImageRenderEventArgs)
        MyBase.OnRenderItemImage(e)
    End Sub

    Protected Overrides Sub OnRenderItemText(e As ToolStripItemTextRenderEventArgs)
        e.TextColor = ForeColor
        MyBase.OnRenderItemText(e)
    End Sub

    Protected Overrides Sub OnRenderLabelBackground(e As ToolStripItemRenderEventArgs)
        MyBase.OnRenderLabelBackground(e)
    End Sub

    Protected Overrides Sub OnRenderMenuItemBackground(e As ToolStripItemRenderEventArgs)
        MyBase.OnRenderMenuItemBackground(e)
    End Sub

    Protected Overrides Sub OnRenderOverflowButtonBackground(e As ToolStripItemRenderEventArgs)
        MyBase.OnRenderOverflowButtonBackground(e)
    End Sub

    Protected Overrides Sub OnRenderSeparator(e As ToolStripSeparatorRenderEventArgs)
        e.Item.ForeColor = ForeColor
        MyBase.OnRenderSeparator(e)
    End Sub

    Protected Overrides Sub OnRenderSplitButtonBackground(e As ToolStripItemRenderEventArgs)
        MyBase.OnRenderSplitButtonBackground(e)
    End Sub

    Protected Overrides Sub OnRenderStatusStripSizingGrip(e As ToolStripRenderEventArgs)
        MyBase.OnRenderStatusStripSizingGrip(e)
    End Sub

    Protected Overrides Sub OnRenderToolStripBackground(e As ToolStripRenderEventArgs)
        MyBase.OnRenderToolStripBackground(e)
        e.Graphics.FillRectangle(BackBrush, e.AffectedBounds)
    End Sub

    Protected Overrides Sub OnRenderToolStripBorder(e As ToolStripRenderEventArgs)
        'MyBase.OnRenderToolStripBorder(e)
    End Sub

    Protected Overrides Sub OnRenderToolStripContentPanelBackground(e As ToolStripContentPanelRenderEventArgs)
        MyBase.OnRenderToolStripContentPanelBackground(e)
    End Sub

    Protected Overrides Sub OnRenderToolStripStatusLabelBackground(e As ToolStripItemRenderEventArgs)
        MyBase.OnRenderToolStripStatusLabelBackground(e)
    End Sub

    Protected Overrides Sub OnRenderToolStripPanelBackground(e As ToolStripPanelRenderEventArgs)
        MyBase.OnRenderToolStripPanelBackground(e)
        e.Graphics.FillRectangle(BackBrush, e.ToolStripPanel.ClientRectangle)
    End Sub

End Class
