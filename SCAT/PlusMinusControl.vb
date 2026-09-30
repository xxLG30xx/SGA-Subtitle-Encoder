Public Class PlusMinusControl

    Private _values As Single() = {0.25, 0.5, 1, 2, 4, 8}
    Public Property Values As Single()
        Get
            Return _values
        End Get
        Set(value As Single())
            _values = value
            FactorIndex = Math.Min(FactorIndex, Values.Count - 1)
        End Set
    End Property

    Private _defaultFactorIndex = 2
    Private _factorIndex As Integer = _defaultFactorIndex
    Private Property FactorIndex As Integer
        Get
            Return _factorIndex
        End Get
        Set(value As Integer)
            _factorIndex = value
            If FactorIndex = Values.Count - 1 Then
                btnPlus.Enabled = False
            ElseIf FactorIndex = 0 Then
                btnMinus.Enabled = False
            End If
            RaiseEvent ValueChanged(Me, New ValueChangedEventArgs(Values(_factorIndex)))
        End Set
    End Property

    Public Class ValueChangedEventArgs
        Inherits EventArgs
        Property Value As Single
        Sub New(ByVal value As Single)
            MyBase.New
            With Me
                .Value = value
            End With
        End Sub
    End Class

    Public Event ValueChanged(sender As Object, e As ValueChangedEventArgs)

    Public Sub Reset()
        FactorIndex = _defaultFactorIndex
    End Sub

    Private Sub btnPlus_Click(sender As Object, e As EventArgs) Handles btnPlus.Click
        If FactorIndex < Values.Count - 1 Then
            FactorIndex += 1
            btnMinus.Enabled = True
        End If
    End Sub

    Private Sub btnMinus_Click(sender As Object, e As EventArgs) Handles btnMinus.Click
        If FactorIndex > 0 Then
            FactorIndex -= 1
            btnPlus.Enabled = True
        End If
    End Sub

    Private Sub PlusMinusControl_EnabledChanged(sender As Object, e As EventArgs) Handles Me.EnabledChanged
        SuspendLayout()
        For Each btn As Button In Controls
            btn.Enabled = True
        Next
        FactorIndex = FactorIndex
        ResumeLayout()
    End Sub

End Class
