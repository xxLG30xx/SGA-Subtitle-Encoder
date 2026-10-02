Imports System.Windows.Forms

Public Class frmStreamChooser

    Dim _SGAFile As SGA.File
    Dim _SGARenderer As SGA.Renderer

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        With Me
            '_SGARenderer.MainStreamID = Convert.ToInt32(.cbxMainStream.SelectedItem, 16)
            '_SGARenderer.VideoStreamID = Convert.ToInt32(.cbxVideoStream.SelectedItem, 16)
            '_SGARenderer.VideoSubStreamID = Convert.ToInt32(.cbxVideoSubStream.SelectedItem, 16)
            '_SGARenderer.AudioStreamID = Convert.ToInt32(.cbxAudioStream.SelectedItem, 16)
            'SetStreamID(_SGARenderer.StreamID, .cbxMainStream)
            'SetStreamID(_SGARenderer.VideoTrackID, .cbxVideoSubstream)
            'SetStreamID(_SGARenderer.VideoOverlayTrackID, .cbxVideoOverlaySubstream)
            'SetStreamID(_SGARenderer.AudioTrackID, .cbxAudioSubstream)
        End With

        DialogResult = System.Windows.Forms.DialogResult.OK
        Close()
    End Sub

    Private Sub SetStreamID(ByRef streamID As Integer, ByRef cbx As ComboBox)
        streamID = Convert.ToInt32(cbx.SelectedItem, 16)
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        DialogResult = System.Windows.Forms.DialogResult.Cancel
        Close()
    End Sub

    Public Sub New(ByRef SgaRenderer As SGA.Renderer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _SGARenderer = SgaRenderer
        _SGAFile = _SGARenderer.File

        'PopulateComboBox(cbxMainStream, _SGAFile.MainStreamIDs.ToArray)
        'PopulateComboBox(cbxVideoSubstream, _SGAFile.VideoTrackIDs.ToArray)
        'PopulateComboBox(cbxVideoOverlaySubstream, _SGAFile.VideoOverlayTrackIDs.ToArray)
        'PopulateComboBox(cbxAudioSubstream, _SGAFile.AudioTrackIDs.ToArray)

    End Sub

    Private Sub PopulateComboBox(ByRef cbx As ComboBox, ByRef IDs As UShort())
        With cbx
            For Each ID As UShort In IDs
                .Items.Add($"{ID:X4}")
            Next
            If .Items.Count = 0 Then
                .Enabled = False
            Else
                .SelectedIndex = 0
            End If
        End With
    End Sub

End Class
