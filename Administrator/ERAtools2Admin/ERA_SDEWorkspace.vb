Public Class ERA_SDEWorkspace
    Inherits ERAControl

    'Public Sub New(ByRef EraNode As ERANode, ByRef form As frmReportDefinitionEditor)
    '    ' This call is required by the Windows Form Designer.
    '    InitializeComponent()

    '    ' Add any initialization after the InitializeComponent() call.
    '    Me.frde_Parent = form
    '    Me.ERANode = EraNode
    'End Sub

    Friend Overrides Sub SaveChangesToNode()
        Dim strOldName As String = EraNode.GetAttributeValue("name")
        If strOldName <> txtName.Text Then
            'update all references to this workspace
            Dim nc As Xml.XmlNodeList = EraNode.XMLNode.OwnerDocument.SelectNodes("//*[@workspace='" & strOldName & "']")
            For Each n As Xml.XmlElement In nc
                n.SetAttribute("workspace", txtName.Text)
            Next
        End If
        EraNode.SetAttributeValue("name", txtName.Text)
        EraNode.SetAttributeValue("server", txtServer.Text)
        EraNode.SetAttributeValue("instance", txtInstance.Text)
        EraNode.SetAttributeValue("database", txtDatabase.Text)
        If rbDBMS.Checked Then
            EraNode.SetAttributeValue("user", txtUser.Text)
            EraNode.SetAttributeValue("password", txtPassword.Text)
            EraNode.SetAttributeValue("authentication_mode", "DBMS")
        Else
            EraNode.SetAttributeValue("authentication_mode", "OSA")
        End If
        EraNode.SetAttributeValue("version", txtVersion.Text)
        Me.ParentForm.hasChanged = False
    End Sub

    Friend Overrides Sub LoadData()
        txtName.Text = EraNode.GetAttributeValue("name")
        txtServer.Text = EraNode.GetAttributeValue("server")
        txtInstance.Text = EraNode.GetAttributeValue("instance")
        txtDatabase.Text = EraNode.GetAttributeValue("database")
        txtUser.Text = EraNode.GetAttributeValue("user")
        txtPassword.Text = EraNode.GetAttributeValue("password")
        txtVersion.Text = EraNode.GetAttributeValue("version")
        Dim strAuthMode As String = EraNode.GetAttributeValue("authentication_mode")
        If strAuthMode = "OSA" Then
            Me.rbOSA.Checked = True
        Else
            Me.rbDBMS.Checked = True
        End If
        Me.ParentForm.hasChanged = False
        ShowErrors()
    End Sub

    Private Sub AnyTextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtName.TextChanged, txtDatabase.TextChanged, txtInstance.TextChanged, txtPassword.TextChanged, txtServer.TextChanged, txtUser.TextChanged, txtVersion.TextChanged
        If Not Me.ParentForm Is Nothing Then
            Me.ParentForm.hasChanged = True
        End If
    End Sub

    Private Sub txtName_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtName.Validating
        myErrorProvider.SetError(txtName, "")
        If txtName.Text = "" Then
            myErrorProvider.SetError(txtName, "Name is a required field")
            'e.Cancel = True
        Else
            Dim wsnode As ERANode = Me.ParentForm.getWorkspaceNode(txtName.Text)
            If wsnode Is Nothing Then
                'all is well
            ElseIf wsnode Is Me.EraNode Then
                'all is still well
            Else
                myErrorProvider.SetError(txtName, "Name must be unique. There is already another workspace with that name.")
                'e.Cancel = True
            End If
        End If
    End Sub

    Private Sub txtName_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtName.Validated
        'myErrorProvider.SetError(txtName, "")
        'Note: for some reason we would call savechanges after the name changes
        'I think this was just so that the label on the node in the parent form
        'treeview would be updated, but it has the unintended consequence of making
        'it so that minor changes cannot be reverted. For now, I'm commenting out
        'the SaveChanges call, but we need to be sure that that doesn't have some other 
        'unintended consequence. Perhaps a better fix would be something that updated the node label without calling SaveChanges. 
        'SaveChanges()
    End Sub

    Private Sub ERA_SDEWorkspace_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        txtName.Focus()
    End Sub

    Public Overrides Sub ShowErrors()
        myErrorProvider.Clear()
        For Each ve As ERANode.ValidationErrorMessage In Me.EraNode.Errors
            Select Case ve.ValidationErrorTarget
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.DisplayName
                    myErrorProvider.SetError(txtName, ve.ErrorMessage)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.SDEServer
                    myErrorProvider.SetError(txtServer, ve.ErrorMessage)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.SDEInstance
                    myErrorProvider.SetError(txtInstance, ve.ErrorMessage)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.Database
                    myErrorProvider.SetError(txtDatabase, ve.ErrorMessage)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.UserNamePassword
                    If rbDBMS.Checked Then
                        myErrorProvider.SetError(txtUser, ve.ErrorMessage)
                    Else
                        myErrorProvider.SetError(rbOSA, ve.ErrorMessage)
                    End If
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.SDEVersion
                    myErrorProvider.SetError(txtVersion, ve.ErrorMessage)
            End Select
        Next
    End Sub

    Private Sub rbDBMS_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbDBMS.CheckedChanged
        Me.txtUser.Enabled = rbDBMS.Checked
        Me.txtPassword.Enabled = rbDBMS.Checked
    End Sub
End Class
