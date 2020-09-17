Public Class ConfirmDialog
    Public result As Integer

    Private Sub btnNo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNo.Click
        result = 0
        Me.Close()
    End Sub

    Private Sub btnYes_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnYes.Click
        If Me.CheckBox1.Checked Then
            result = 2
        Else
            result = 1
        End If
        Me.Close()
    End Sub

End Class