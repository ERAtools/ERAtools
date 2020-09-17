Public Class ERAIssue
    Inherits ERAControl

    'Public Sub New(ByRef EraNode As ERANode, ByRef form As frmReportDefinitionEditor)
    '    ' This call is required by the Windows Form Designer.
    '    InitializeComponent()

    '    ' Add any initialization after the InitializeComponent() call.
    '    Me.frde_Parent = form
    '    Me.ERANode = EraNode
    'End Sub

    'Friend Overrides Function ValidateData() As Boolean
    '    Return Not String.IsNullOrEmpty(txtIssueName.Text)
    'End Function


    Friend Overrides Sub SaveChangesToNode()
        'This is where the changes get saved to the ERANode, not to the XML document
        EraNode.SetAttributeValue("name", txtIssueName.Text)
        EraNode.SetAttributeValue("meta", txtIssueDescription.Text)
        Me.ParentForm.hasChanged = False
    End Sub

    Friend Overrides Sub LoadData()
        txtIssueName.Text = EraNode.GetAttributeValue("name")
        txtIssueDescription.Text = EraNode.GetAttributeValue("meta")
        Me.ParentForm.hasChanged = False
        ShowErrors()
    End Sub

    Private Sub AnyTextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtIssueName.TextChanged, txtIssueDescription.TextChanged
        If Not Me.ParentForm Is Nothing Then
            Me.ParentForm.hasChanged = True
        End If
    End Sub

    Private Sub txtIssueName_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtIssueName.Validating
        myErrorProvider.SetError(sender, "")
        If txtIssueName.Text = "" Then
            myErrorProvider.SetError(txtIssueName, "Name is a required field")
            'e.Cancel = True
        End If
    End Sub

    Private Sub txtIssueName_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtIssueName.Validated
        'myErrorProvider.SetError(txtIssueName, "")
        'SaveChanges()
    End Sub

    Public Overrides Sub ShowErrors()
        myErrorProvider.Clear()
        myErrorProvider.SetError(txtIssueName, EraNode.ErrorText)
    End Sub

    Private Sub ERAIssue_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.txtIssueName.Focus()
    End Sub
End Class
