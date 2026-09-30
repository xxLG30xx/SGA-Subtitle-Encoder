Namespace Interfaces

    Public Interface ICancelable
        Event Canceled(sender As Object, e As EventArgs)
        Property IsCanceled As Boolean
        Sub Cancel()
    End Interface

    Public Class ProgressReporterEventArgs
        Inherits EventArgs
        Property ProgressPercentage As Single
        Property StatusText As String
        Sub New(ByVal progressPercentage As Single, ByVal statusText As String)
            MyBase.New
            With Me
                .ProgressPercentage = progressPercentage
                .StatusText = statusText
            End With
        End Sub
    End Class

    Public Interface IProgressReporter
        Event ProgressChanged(sender As Object, e As ProgressReporterEventArgs)
    End Interface

End Namespace