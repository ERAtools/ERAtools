Imports ESRI.ArcGIS.CatalogUI, ESRI.ArcGIS.Catalog, ESRI.ArcGIS.Carto, ESRI.ArcGIS.Geodatabase

Public Class ERAField
    Inherits ERAControl

    Private Property SelectedFieldName() As String
        Get
            If ddlName.SelectedItem Is Nothing Then
                Return ddlName.Text
            Else
                Return ddlName.SelectedItem
            End If
        End Get
        Set(ByVal value As String)
            For Each item As String In ddlName.Items
                If item.Equals(value, StringComparison.InvariantCultureIgnoreCase) Then
                    ddlName.SelectedItem = item
                    Exit Property
                End If
            Next
            ddlName.Text = value
        End Set
    End Property

    Private Property selectedUseForURL() As String
        Get
            If ddlUseForURL.SelectedItem Is Nothing Then
                Return ddlUseForURL.Text
            Else
                Return ddlUseForURL.SelectedItem
            End If
        End Get
        Set(ByVal value As String)
            For Each item As String In ddlUseForURL.Items
                If item.Equals(value, StringComparison.InvariantCultureIgnoreCase) Then
                    ddlUseForURL.SelectedItem = item
                    Exit Property
                End If
            Next
            ddlUseForURL.Text = value
        End Set
    End Property

    Private Sub FillFieldList()
        Me.ddlName.Items.Clear()
        If My.Application.ESRI_Status <> My.ESRI_Status_Enum.Connected Then
            Exit Sub
        End If
        'Populate the drop-down list of fields
        Try
            Dim pNode As ERANode = EraNode.Parent
            If pNode.CachedFields Is Nothing Then
                Dim wsNode As WorkspaceNode = Me.ParentForm.getWorkspaceNode(pNode.GetAttributeValue("workspace"))
                If wsNode Is Nothing Then Exit Sub
                Dim wspace As IFeatureWorkspace = wsNode.Workspace ' ESRIHelpers.getIWorkspaceFromEraNode(wsNode)
                If wspace Is Nothing Then Exit Sub
                If Not WorkspaceContains(wspace, pNode.GetAttributeValue("layerid")) Then Exit Sub
                Dim table As ITable = wspace.OpenTable(pNode.GetAttributeValue("layerid"))
                If table Is Nothing Then Exit Sub
                pNode.CachedFields = table.Fields
            End If
            Dim str As String
            For i As Integer = 0 To pNode.CachedFields.FieldCount - 1
                str = pNode.CachedFields.Field(i).Name
                Me.ddlName.Items.Add(str)
                Me.ddlUseForURL.Items.Add(str)
            Next
        Catch ex As Exception
            MessageBox.Show("An error occurred reading the list of fields from the data source: " & ex.Message, "Error Reading Fields", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cbLink_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbLink.CheckedChanged
        gbHyperlink.Visible = cbLink.Checked
        Me.ParentForm.hasChanged = True
    End Sub

    Friend Overrides Sub SaveChangesToNode()
        If Not EraNode.GetAttributeValue("name").Equals(SelectedFieldName) Then
            EraNode.ChangedFieldName(EraNode.GetAttributeValue("name"), SelectedFieldName)
        End If
        'EraNode.SetAttributeValue("name", SelectedFieldName)
        EraNode.SetAttributeValue("alias", txtAlias.Text)
        EraNode.SetAttributeValue("link", cbLink.Checked)
        EraNode.SetAttributeValue("useforurl", selectedUseForURL)
        EraNode.SetAttributeValue("prefix", txtPrefix.Text)
        EraNode.SetAttributeValue("suffix", txtSuffix.Text)
        EraNode.SetAttributeValue("format", txtFormat.Text)


        If Not Me.ddlFunction.Visible OrElse Me.ddlFunction.SelectedIndex < 1 Then
            'fix to bug found in training 5/13/08, if you change the summary, then try to change
            'it back in Group by, it won't "take", and will reset back to the attribute
            'selected previously
            EraNode.XMLNode.RemoveAttribute("summary")
        ElseIf Me.ddlFunction.SelectedIndex = 1 Then
            EraNode.SetAttributeValue("summary", "SUM")
        ElseIf Me.ddlFunction.SelectedIndex = 2 Then
            EraNode.SetAttributeValue("summary", "AVG")
        ElseIf Me.ddlFunction.SelectedIndex = 3 Then
            EraNode.SetAttributeValue("summary", "MIN")
        ElseIf Me.ddlFunction.SelectedIndex = 4 Then
            EraNode.SetAttributeValue("summary", "MAX")
        ElseIf Me.ddlFunction.SelectedIndex = 5 Then
            EraNode.SetAttributeValue("summary", "COUNT")
        ElseIf Me.ddlFunction.SelectedIndex = 6 Then
            EraNode.SetAttributeValue("summary", "LINK")
        End If
        If rbIndexData.Checked Then
            EraNode.SetAttributeValue("wavg_type", "indexed")
        ElseIf rbRawData.Checked Then
            EraNode.SetAttributeValue("wavg_type", "rawdata")
        End If
        Me.ParentForm.hasChanged = False
    End Sub

    Friend Overrides Sub LoadData()
        txtAlias.Text = EraNode.GetAttributeValue("alias")
        cbLink.Checked = EraNode.GetAttributeValue("link").Equals("true", StringComparison.OrdinalIgnoreCase)
        txtPrefix.Text = EraNode.GetAttributeValue("prefix")
        txtSuffix.Text = EraNode.GetAttributeValue("suffix")
        txtFormat.Text = EraNode.GetAttributeValue("format")
        FillFieldList()
        SelectedFieldName = EraNode.GetAttributeValue("name")
        selectedUseForURL = EraNode.GetAttributeValue("useforurl")
        If EraNode.isSummarization Then
            Me.lblFunction.Visible = True
            Me.ddlFunction.Visible = True
            Me.ddlFunction.SelectedIndex = 0    'Defaults to GroupBy
            If EraNode.GetAttributeValue("summary").Equals("SUM", StringComparison.CurrentCultureIgnoreCase) Then
                Me.ddlFunction.SelectedIndex = 1
            ElseIf EraNode.GetAttributeValue("summary").Equals("AVG", StringComparison.CurrentCultureIgnoreCase) Then
                Me.ddlFunction.SelectedIndex = 2
            ElseIf EraNode.GetAttributeValue("summary").Equals("MIN", StringComparison.CurrentCultureIgnoreCase) Then
                Me.ddlFunction.SelectedIndex = 3
            ElseIf EraNode.GetAttributeValue("summary").Equals("MAX", StringComparison.CurrentCultureIgnoreCase) Then
                Me.ddlFunction.SelectedIndex = 4
            ElseIf EraNode.GetAttributeValue("summary").Equals("COUNT", StringComparison.CurrentCultureIgnoreCase) Then
                Me.ddlFunction.SelectedIndex = 5
            ElseIf EraNode.GetAttributeValue("summary").Equals("LINK", StringComparison.CurrentCultureIgnoreCase) Then
                Me.ddlFunction.SelectedIndex = 6
            End If
        End If
        If DirectCast(EraNode.Parent, ERANode).GetAttributeValue("analysistype").Equals("weighted average", StringComparison.CurrentCultureIgnoreCase) Then
            lblFunction.Visible = True
            lblFunction.Text = "Weighted Average Type"
            rbIndexData.Visible = True
            rbIndexData.Top = 56
            rbIndexData.Left = 128
            rbRawData.Visible = True
            rbRawData.Top = 83
            rbRawData.Left = 128
            cbLink.Visible = False
            If EraNode.GetAttributeValue("wavg_type").Equals("rawdata", StringComparison.CurrentCultureIgnoreCase) Then
                rbRawData.Checked = True
            Else
                rbIndexData.Checked = True
            End If
        End If
        Me.ParentForm.hasChanged = False
        ShowErrors()
    End Sub

    Private Sub txtURL_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPrefix.TextChanged, txtSuffix.TextChanged
        lblLinkExample.Text = txtPrefix.Text & "x72x" & txtSuffix.Text
        Me.ParentForm.hasChanged = True
    End Sub

    Private Sub AnyTextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtAlias.TextChanged, ddlName.TextChanged, txtFormat.TextChanged
        If Not Me.ParentForm Is Nothing Then
            Me.ParentForm.hasChanged = True
        End If
    End Sub

    Private Sub ddlName_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ddlName.Validating
        myErrorProvider.SetError(sender, "")
        If ddlName.Text = "" Then
            myErrorProvider.SetError(ddlName, "Field Name is required")
            'e.Cancel = True
        Else
            ShowErrors()
            If EraNode.ErrorText = "Could not find field" Then
                If ddlName.SelectedIndex >= 0 Then
                    myErrorProvider.SetError(ddlName, "")
                End If
            End If
        End If
    End Sub

    Private Sub ddlName_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlName.Validated
        'myErrorProvider.SetError(ddlName, "")
        If (txtAlias.Text = "" OrElse txtAlias.Text = "New Field") And ddlName.Text <> "" Then
            txtAlias.Text = AliasNameFormatter.FormatShortNameAlias(ddlName.Text)
        End If
        'Note: for some reason we would call savechanges after the name changes
        'I think this was just so that the label on the node in the parent form
        'treeview would be updated, but it has the unintended consequence of making
        'it so that minor changes cannot be reverted. For now, I'm commenting out
        'the SaveChanges call, but we need to be sure that that doesn't have some other 
        'unintended consequence. Perhaps a better fix would be something that updated the node label without calling SaveChanges.
        'SaveChanges()
    End Sub

    Private Sub txtAlias_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtAlias.Validating, txtFormat.Validating
        myErrorProvider.SetError(sender, "")
        If txtAlias.Text = "" Then
            myErrorProvider.SetError(txtAlias, "Display Name is a required field")
            'e.Cancel = True
        End If
    End Sub

    Public Overrides Sub ShowErrors()
        myErrorProvider.Clear()

        For Each ve As ERANode.ValidationErrorMessage In EraNode.Errors
            Select Case ve.ValidationErrorTarget
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.DisplayName
                    myErrorProvider.SetError(Me.txtAlias, EraNode.ErrorText)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.LayerTableFieldID
                    myErrorProvider.SetError(Me.ddlName, EraNode.ErrorText)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.UseForURL
                    myErrorProvider.SetError(Me.ddlUseForURL, EraNode.ErrorText)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.URLPrefix
                    myErrorProvider.SetError(Me.txtPrefix, EraNode.ErrorText)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.URLSuffix
                    myErrorProvider.SetError(Me.txtSuffix, EraNode.ErrorText)
            End Select
        Next
    End Sub

    Private Sub ERAField_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ddlName.Focus()
    End Sub

    Private Sub ddlName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlName.SelectedIndexChanged
        'Todo: detect type of field, enable/disable ddlDateFormat accordingly
    End Sub
End Class
