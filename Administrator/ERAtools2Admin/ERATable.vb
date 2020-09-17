Imports ESRI.ArcGIS.CatalogUI, ESRI.ArcGIS.Catalog, ESRI.ArcGIS.Carto, ESRI.ArcGIS.Geodatabase, ESRI.ArcGIS.Geometry

Public Class ERATable
    Inherits ERAControl

    Private qb As QueryBuilder
    Private keysNode As Xml.XmlElement
    Private fieldList As ValueCollection

    'Public Sub New(ByRef EraNode As ERANode, ByRef form As frmReportDefinitionEditor)
    '    ' This call is required by the Windows Form Designer.
    '    InitializeComponent()

    '    ' Add any initialization after the InitializeComponent() call.
    '    Me.frde_Parent = form
    '    Me.ERANode = EraNode
    'End Sub

    Private Property SelectedLayerID() As String
        Get
            If ddlLayer.SelectedItem Is Nothing Then
                If ddlLayer.Text.Contains(" (") Then
                    Return ddlLayer.Text.Substring(0, ddlLayer.Text.LastIndexOf("(") - 1)
                Else
                    Return ddlLayer.Text
                End If
            Else
                If ddlLayer.SelectedItem.ToString.Contains(" (") Then
                    Return ddlLayer.SelectedItem.ToString.Substring(0, ddlLayer.SelectedItem.ToString.LastIndexOf("(") - 1)
                Else
                    Return ddlLayer.SelectedItem
                End If
            End If
        End Get
        Set(ByVal value As String)
            For Each item As String In ddlLayer.Items
                If item.StartsWith(value & " (", StringComparison.CurrentCultureIgnoreCase) Then
                    ddlLayer.SelectedItem = item
                    UpdateCanSpatialJoin()
                    Exit Property
                ElseIf item.Equals(value, StringComparison.CurrentCultureIgnoreCase) Then
                    ddlLayer.SelectedItem = item
                    UpdateCanSpatialJoin()
                    Exit Property
                End If
            Next
            ddlLayer.Text = value
            UpdateCanSpatialJoin()
        End Set
    End Property

    Private Sub UpdateCanSpatialJoin()
        If ddlLayer.Text.EndsWith("(Table)") OrElse DirectCast(EraNode.Parent, ERANode).isSummarization _
            OrElse DirectCast(EraNode.Parent, LayerTableNode).GetDataSetType = esriDatasetType.esriDTTable _
            OrElse DirectCast(EraNode, LayerTableNode).GetDataSetType = esriDatasetType.esriDTTable Then
            rbAttributeJoin.Checked = True
            rbSpatialJoin.Enabled = False
            rbAttributeJoin.Enabled = False
        ElseIf DirectCast(EraNode.Parent, ERANode).GetAttributeValue("analysistype").ToLower = "comparison" Then
            rbSpatialJoin.Checked = True
            rbSpatialJoin.Enabled = False
            rbAttributeJoin.Enabled = False
        Else
            rbSpatialJoin.Enabled = True
            rbAttributeJoin.Enabled = True
        End If
    End Sub

    Private Sub UpdateWorkspaceDDL()
        ddlWorkspace.Items.Clear()
        For Each wsNode As ERANode In ParentForm.WorkspacesNode.Nodes
            ddlWorkspace.Items.Add(wsNode.GetAttributeValue("name"))
        Next
    End Sub

    Public Sub UpdateLayerDDL(ByRef wsNode As WorkspaceNode)
        ddlLayer.Items.Clear()
        'Comparison Resourse Analysis layers require the child table to be a FeatureClass, so only add tables if it's not the child of one
        Dim eraParent As ERANode = EraNode.Parent
        If eraParent IsNot Nothing AndAlso eraParent.GetAttributeValue("analysistype").Equals("comparison", StringComparison.CurrentCultureIgnoreCase) Then
            For Each strLayer As String In wsNode.LayerCache
                If Not strLayer.EndsWith("(Table)") Then
                    ddlLayer.Items.Add(strLayer)
                End If
            Next
        Else
            For Each strLayer As String In wsNode.LayerCache
                ddlLayer.Items.Add(strLayer)
            Next
        End If
    End Sub

    Private Sub UpdateChildKeyDDL()
        If EraNode.CachedFields Is Nothing OrElse EraNode.CachedFields.FieldCount < 1 Then
            EraNode.CachedFields = Nothing
            Dim wsNode As WorkspaceNode = ParentForm.getWorkspaceNode(ddlWorkspace.SelectedItem)
            If wsNode IsNot Nothing Then
                Dim wspace As IFeatureWorkspace = wsNode.Workspace 'ESRIHelpers.getIWorkspaceFromEraNode(wsNode)
                If wspace IsNot Nothing AndAlso WorkspaceContains(wspace, SelectedLayerID) Then
                    Try
                        Dim dset As ITable = wspace.OpenTable(SelectedLayerID)
                        My.Application.StartWait()
                        EraNode.CachedFields = dset.Fields
                        My.Application.EndWait()
                    Catch ex As Exception
                        MessageBox.Show("Error updating child key drop-down list: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub
                    End Try
                End If
            End If
        End If
        If EraNode.CachedFields Is Nothing OrElse EraNode.CachedFields.FieldCount < 1 Then
            EraNode.CachedFields = Nothing
            Exit Sub
        End If
        Me.ddlChildKey.Items.Clear()
        For i As Integer = 0 To EraNode.CachedFields.FieldCount - 1
            ddlChildKey.Items.Add(EraNode.CachedFields.Field(i).Name)
        Next
    End Sub

    Private Sub UpdateParentKeyDDL()
        'Populate the drop-down list of fields
        Dim pNode As ERANode = EraNode.Parent
        If pNode Is Nothing Then
            Exit Sub
        End If
        Dim blnStrict As Boolean = False
        If pNode.ERANodeType = EraNode.ERANodeTypeEnum.Layer AndAlso pNode.XMLNode.GetAttribute("analysistype").Equals("acreage", StringComparison.CurrentCultureIgnoreCase) Then
            blnStrict = True
        ElseIf pNode.isSummarization AndAlso Not Me.cbInline.Checked Then
            blnStrict = True
        End If
        If blnStrict Then
            'Only use fields that are listed in the output of the parents
            ddlParentKey.Items.Clear()
            ParentKeyAliases.Clear()
            GetStrictParentKeys(pNode)
        Else
            If pNode.CachedFields IsNot Nothing AndAlso pNode.CachedFields.FieldCount < 1 Then
                pNode.CachedFields = Nothing
            End If
            If pNode.CachedFields Is Nothing Then
                Dim wsNode As WorkspaceNode = ParentForm.getWorkspaceNode(pNode.GetAttributeValue("workspace"))
                If wsNode Is Nothing Then
                    Exit Sub
                End If
                Dim wspace As IFeatureWorkspace = wsNode.Workspace 'ESRIHelpers.getIWorkspaceFromEraNode(wsNode)
                If wspace IsNot Nothing AndAlso WorkspaceContains(wspace, pNode.GetAttributeValue("layerid")) Then
                    Try
                        Dim dset As ITable = wspace.OpenTable(pNode.GetAttributeValue("layerid"))
                        My.Application.StartWait()
                        pNode.CachedFields = dset.Fields
                        My.Application.EndWait()
                    Catch ex As Exception
                        MessageBox.Show("Error updating parent key drop-down list: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub
                    End Try
                End If
            End If
            If pNode.CachedFields Is Nothing OrElse pNode.CachedFields.FieldCount < 1 Then
                pNode.CachedFields = Nothing
                Exit Sub
            End If
            ddlParentKey.Items.Clear()
            For i As Integer = 0 To pNode.CachedFields.FieldCount - 1
                ddlParentKey.Items.Add(pNode.CachedFields.Field(i).Name)
            Next
        End If
    End Sub

    Private ParentKeyAliases As New Dictionary(Of String, String)

    Private Sub GetStrictParentKeys(ByVal node As ERANode)
        If node Is Nothing Then Exit Sub
        If node.GetAttributeValue("inline").Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            GetStrictParentKeys(node.Parent)
        End If
        For Each fieldNode As Xml.XmlElement In node.XMLNode.SelectNodes("FIELD")
            'ddlParentKey.Items.Add(New ValuePair(fieldNode.GetAttribute("name"), fieldNode.GetAttribute("alias")))
            ddlParentKey.Items.Add(fieldNode.GetAttribute("alias"))
            ParentKeyAliases.Add(fieldNode.GetAttribute("alias"), fieldNode.GetAttribute("name"))
        Next
    End Sub

    Private Function GetParentKey(ByVal name As String) As String
        For Each key As String In ParentKeyAliases.Keys
            If ParentKeyAliases(key) = name Then
                Return key
            End If
        Next
        Return name
    End Function

    Private Sub UpdateSortFieldsDDL()
        gvSortFields.Rows.Clear()
        colSortFieldName.DataSource = Nothing
        Dim strCollect As New Specialized.StringCollection
        fieldList = New ValueCollection
        GetSortFields(EraNode, strCollect)
        colSortFieldName.DataSource = strCollect

        Dim SortFieldsNode As Xml.XmlElement = EraNode.XMLNode.SelectSingleNode("SORT_FIELDS")
        If SortFieldsNode IsNot Nothing Then
            Dim gvr As DataGridViewRow
            For Each sortNode As Xml.XmlElement In SortFieldsNode.SelectNodes("SORT_FIELD")
                If strCollect.Contains(fieldList.FindKeyForValue(sortNode.GetAttribute("field"))) Then
                    gvr = gvSortFields.Rows(gvSortFields.Rows.Add())
                    gvr.Cells(0).Value = fieldList.FindKeyForValue(sortNode.GetAttribute("field"))
                    If sortNode.GetAttribute("direction").Equals("descending", StringComparison.CurrentCultureIgnoreCase) Then
                        gvr.Cells(1).Value = "Descending"
                    Else
                        gvr.Cells(1).Value = "Ascending"
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub GetSortFields(ByVal parentNode As ERANode, ByRef strCollect As Specialized.StringCollection)
        Dim dName As String
        For Each fieldNode As Xml.XmlElement In parentNode.XMLNode.SelectNodes("FIELD")
            dName = GetSortFieldDisplayFormat(parentNode, fieldNode)
            strCollect.Add(dName)
            fieldList.AddPair(GetSortFieldNameFormat(parentNode, fieldNode), dName)
        Next
        For Each childNode As ERANode In parentNode.Nodes
            If childNode.ERANodeType = ERAtools2Admin.ERANode.ERANodeTypeEnum.Table AndAlso childNode.XMLNode.GetAttribute("inline").Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
                GetSortFields(childNode, strCollect)
            End If
        Next
    End Sub

    Private Function GetSortFieldNameFormat(ByVal parentNode As ERANode, ByVal fieldNode As Xml.XmlElement) As String
        Dim sb As New System.Text.StringBuilder
        sb.Append(parentNode.GetMyName & "_")
        If Me.cbSummarization.Checked AndAlso fieldNode.GetAttribute("summary").Length > 2 Then
            sb.Append(fieldNode.GetAttribute("summary") & "_")
        End If
        sb.Append(fieldNode.GetAttribute("name"))
        Return Xml.XmlConvert.EncodeLocalName(sb.ToString)
    End Function

    Private Function GetSortFieldDisplayFormat(ByVal parentNode As ERANode, ByVal fieldNode As Xml.XmlElement) As String
        Dim sb As New System.Text.StringBuilder
        If parentNode IsNot EraNode Then
            sb.Append(parentNode.GetMyName & " / ")
        End If
        sb.Append(fieldNode.GetAttribute("alias"))
        If Me.cbSummarization.Checked AndAlso fieldNode.GetAttribute("summary").Length > 2 Then
            sb.Append(" (" & fieldNode.GetAttribute("summary") & ")")
        End If
        Return sb.ToString
    End Function

    Private Sub ddlWorkspace_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlWorkspace.SelectedIndexChanged
        If ddlWorkspace.SelectedIndex = -1 Or My.Application.ESRI_Status <> My.ESRI_Status_Enum.Connected Then
            ddlLayer.Items.Clear()
            Exit Sub
        End If
        Try
            Dim wsNode As WorkspaceNode = ParentForm.getWorkspaceNode(ddlWorkspace.SelectedItem)
            If Not wsNode Is Nothing Then
                UpdateLayerDDL(wsNode)
            Else
                ddlWorkspace.SelectedIndex = -1
            End If
            ParentForm.hasChanged = True
        Catch ex As Exception
            MessageBox.Show("Error filling layer drop-down list: " & ex.Message, "Error Filling Layer Drop-Down List", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ddlLayer_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlLayer.Validated
        If ddlLayer.Text <> "" AndAlso (txtName.Text = "New Table" OrElse txtName.Text = "") Then
            txtName.Text = AliasNameFormatter.FormatShortNameAlias(SelectedLayerID)
        End If
    End Sub

    Private Sub ddlLayer_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ddlLayer.Validating
        myErrorProvider.SetError(sender, "")
        If ddlWorkspace.SelectedIndex = -1 Or My.Application.ESRI_Status <> My.ESRI_Status_Enum.Connected Then
            ddlChildKey.Items.Clear()
            Exit Sub
        End If
        Try
            UpdateCanSpatialJoin()
            EraNode.CachedFields = Nothing
            UpdateChildKeyDDL()
            ParentForm.hasChanged = True
        Catch ex As Exception
            MessageBox.Show("Error filling layer drop-down list: " & ex.Message, "Error Filling Layer Drop-Down List", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Friend Overrides Sub SaveChangesToNode()
        Try
            If Not qb Is Nothing Then
                qb.Trash()
                qb = Nothing
            End If
            With EraNode
                .SetAttributeValue("whereexpression", txtWhereExpression.Text)
                .SetAttributeValue("workspace", ddlWorkspace.SelectedItem)
                .SetAttributeValue("layerid", SelectedLayerID)
                .SetAttributeValue("showheaders", cbShowHeaders.Checked)
                .SetAttributeValue("summarization", cbSummarization.Checked)
                .SetAttributeValue("inline", cbInline.Checked)
                .SetAttributeValue("innerjoin", cbInnerJoin.Checked)
                .CachedFields = Nothing
            End With
            keysNode.RemoveAll()
            If rbSpatialJoin.Checked Then
                EraNode.SetAttributeValue("join", "Spatial")
            Else
                EraNode.SetAttributeValue("join", "Attribute")
                For Each nRow As DataGridViewRow In Me.gvKeyFields.Rows
                    If Not String.IsNullOrEmpty(nRow.Cells(0).Value) AndAlso Not String.IsNullOrEmpty(nRow.Cells(1).Value) Then
                        Dim key As Xml.XmlElement = keysNode.OwnerDocument.CreateElement("KEY")
                        If ParentKeyAliases.ContainsKey(nRow.Cells(0).Value) Then
                            key.SetAttribute("parent", ParentKeyAliases(nRow.Cells(0).Value))
                        Else
                            key.SetAttribute("parent", nRow.Cells(0).Value)
                        End If
                        key.SetAttribute("child", nRow.Cells(1).Value)
                        keysNode.AppendChild(key)
                    End If
                Next
            End If
            Dim SortFieldsNode As Xml.XmlElement = EraNode.XMLNode.SelectSingleNode("SORT_FIELDS")
            If SortFieldsNode Is Nothing Then
                SortFieldsNode = EraNode.XMLNode.OwnerDocument.CreateElement("SORT_FIELDS")
                EraNode.XMLNode.AppendChild(SortFieldsNode)
            Else
                SortFieldsNode.RemoveAll()
            End If
            Dim sortNode As Xml.XmlElement
            For Each gvr As DataGridViewRow In gvSortFields.Rows
                If Not gvr.IsNewRow Then
                    If fieldList.Contains(gvr.Cells(0).Value.ToString) Then
                        sortNode = SortFieldsNode.OwnerDocument.CreateElement("SORT_FIELD")
                        sortNode.SetAttribute("field", fieldList(gvr.Cells(0).Value.ToString).Value)
                        If Not String.IsNullOrEmpty(gvr.Cells(1).Value) AndAlso gvr.Cells(1).Value.ToString.Equals("descending", StringComparison.CurrentCultureIgnoreCase) Then
                            sortNode.SetAttribute("direction", "descending")
                        Else
                            sortNode.SetAttribute("direction", "ascending")
                        End If
                        SortFieldsNode.AppendChild(sortNode)
                    End If
                End If
            Next
            With EraNode
                If Not .GetMyName.Equals(txtName.Text.Trim) Then
                    DirectCast(EraNode, LayerTableNode).ChangedTableName(Xml.XmlConvert.EncodeLocalName(.GetMyName), Xml.XmlConvert.EncodeLocalName(txtName.Text.Trim))
                End If
                .SetAttributeValue("name", txtName.Text.Trim)
            End With
            ParentForm.hasChanged = False
        Catch ex As Exception
            LogException(ex, "Exception in ERATable.SaveChangesToNode: " & ex.Message)
            MessageBox.Show("An error occurred saving changes to the table report element: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Friend Overrides Sub LoadData()
        With EraNode
            Dim strParent As String
            Dim pNode As ERANode = .Parent
            If pNode IsNot Nothing Then
                cbInnerJoin.Text = String.Format(cbInnerJoin.Text, pNode.GetAttributeValue("layerid"))
            End If
            cbInnerJoin.Checked = .GetAttributeValue("innerjoin").Equals("true", StringComparison.CurrentCultureIgnoreCase)
            txtName.Text = .GetAttributeValue("name")
            txtWhereExpression.Text = .GetAttributeValue("whereexpression")
            UpdateWorkspaceDDL()
            ddlWorkspace.SelectedItem = .GetAttributeValue("workspace")
            SelectedLayerID = .GetAttributeValue("layerid")
            cbSummarization.Checked = .isSummarization
            cbShowHeaders.Checked = Not .GetAttributeValue("showheaders").Equals("false", StringComparison.CurrentCultureIgnoreCase)
            cbInline.Checked = .GetAttributeValue("inline").Equals("true", StringComparison.CurrentCultureIgnoreCase)
            Me.UpdateParentKeyDDL()
            Me.UpdateChildKeyDDL()
            keysNode = .XMLNode.SelectSingleNode("KEYS")
            If keysNode Is Nothing Then
                keysNode = .XMLNode.OwnerDocument.CreateElement("KEYS")
                .XMLNode.AppendChild(keysNode)
            End If
            gvKeyFields.Rows.Clear()
            For Each key As Xml.XmlElement In keysNode.ChildNodes
                Dim nRow As DataGridViewRow = gvKeyFields.Rows(gvKeyFields.Rows.Add)
                strParent = GetParentKey(key.GetAttribute("parent"))
                If Not ddlParentKey.Items.Contains(strParent) Then
                    ddlParentKey.Items.Add(strParent)
                End If
                nRow.Cells(0).Value = strParent
                If Not ddlChildKey.Items.Contains(key.GetAttribute("child")) Then
                    ddlChildKey.Items.Add(key.GetAttribute("child"))
                End If
                nRow.Cells(1).Value = key.GetAttribute("child")
            Next
            UpdateSortFieldsDDL()
            rbAttributeJoin.Checked = True
            If rbAttributeJoin.Enabled AndAlso .GetAttributeValue("join").Equals("Spatial", StringComparison.CurrentCultureIgnoreCase) Then
                rbSpatialJoin.Checked = True
            End If
        End With
        ParentForm.hasChanged = False
        ShowErrors()
    End Sub

    Private Sub AnyTextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtName.TextChanged, txtWhereExpression.TextChanged
        If Not ParentForm Is Nothing Then
            ParentForm.hasChanged = True
        End If
    End Sub

    Private Sub txtName_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtName.Validating
        myErrorProvider.SetError(sender, "")
        If txtName.Text = "" Then
            myErrorProvider.SetError(txtName, "Name is a required field")
            'e.cancel = True
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

    'Browse for the data set
    Private Sub BrowseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseButton.Click
        Dim LayerInfo As LayerConnectInfo = ESRIHelpers.BrowseForLayers(Me)
        If LayerInfo Is Nothing Then Exit Sub
        'Dim el As Xml.XmlElement = ESRIHelpers.BrowseForLayers(Me)
        'If el Is Nothing Then
        '    Exit Sub
        'End If
        UpdateWorkspaceDDL()
        ddlWorkspace.SelectedItem = LayerInfo.WorkspaceID
        SelectedLayerID = LayerInfo.LayerID
        UpdateChildKeyDDL()
    End Sub

    Private Sub QueryBuilderButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QueryBuilderButton.Click
        If My.Application.ESRI_Status <> My.ESRI_Status_Enum.Connected Then
            If Not My.Application.ConnectToESRI Then
                MessageBox.Show("Unable to check out an ArcView, ArcEditor or ArcInfo license. Status returned:" & vbNewLine & My.Application.GetESRIStatusMessage)
                Exit Sub
            End If
        End If
        Dim wsNode As WorkspaceNode = ParentForm.getWorkspaceNode(ddlWorkspace.SelectedItem)
        Dim ws As IFeatureWorkspace = wsNode.Workspace 'ESRIHelpers.getIWorkspaceFromEraNode(wsNode)
        Try
            Dim ptable As ITable = ws.OpenTable(SelectedLayerID)
            qb = New QueryBuilder
            qb.ShowQueryBuilder(ptable, Me.txtWhereExpression)
        Catch ex As Exception
            MessageBox.Show("An error occurred opening the query builder for this table: " & ex.Message, "Error Opening Query Builder", MessageBoxButtons.OK)
        End Try
    End Sub

    Public Overrides Sub ShowErrors()
        myErrorProvider.Clear()
        For Each ve As ERANode.ValidationErrorMessage In Me.EraNode.Errors
            Select Case ve.ValidationErrorTarget
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.DisplayName
                    myErrorProvider.SetError(txtName, ve.ErrorMessage)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.Workspace
                    myErrorProvider.SetError(BrowseButton, ve.ErrorMessage)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.LayerTableFieldID
                    myErrorProvider.SetError(ddlLayer, ve.ErrorMessage)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.WhereExpression
                    myErrorProvider.SetError(txtWhereExpression, ve.ErrorMessage)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.SortFields
                    Dim sb As New System.Text.StringBuilder(myErrorProvider.GetError(gvSortFields))
                    If sb.Length > 0 Then
                        sb.AppendLine()
                    End If
                    sb.Append(ve.ErrorMessage)
                    myErrorProvider.SetError(gvSortFields, sb.ToString)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.KeyFields
                    Dim sb As New System.Text.StringBuilder(myErrorProvider.GetError(gvSortFields))
                    If sb.Length > 0 Then
                        sb.AppendLine()
                    End If
                    sb.Append(ve.ErrorMessage)
                    myErrorProvider.SetError(Me.gvKeyFields, sb.ToString)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.JoinOption
                    myErrorProvider.SetError(Me.rbSpatialJoin, ve.ErrorMessage)
            End Select
        Next
    End Sub

    Private Sub ERATable_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.txtName.Focus()
    End Sub

    Private Sub gv_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles gvKeyFields.DataError, gvSortFields.DataError
        'ignore, it just means the selected item isn't a valid field. this is better caught through our custom validation
        e.Cancel = True
    End Sub

    Private Sub gvKeyFields_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles gvKeyFields.EditingControlShowing
        Dim c As ComboBox = e.Control
        If c IsNot Nothing Then
            c.DropDownStyle = ComboBoxStyle.DropDown
        End If
    End Sub

    Private Sub gvKeyFields_CellValidating(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellValidatingEventArgs) Handles gvKeyFields.CellValidating
        Dim eFV As Object = e.FormattedValue
        If Not String.IsNullOrEmpty(eFV) Then
            If e.ColumnIndex = ddlParentKey.Index Then
                If Not ddlParentKey.Items.Contains(eFV) Then
                    ddlParentKey.Items.Add(eFV)
                End If
            ElseIf e.ColumnIndex = ddlChildKey.Index Then
                If Not ddlChildKey.Items.Contains(eFV) Then
                    ddlChildKey.Items.Add(eFV)
                End If
            End If
            gvKeyFields.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = eFV
            gvKeyFields.NotifyCurrentCellDirty(True)
            ParentForm.hasChanged = True
        End If
    End Sub

    Private Sub cbInline_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbInline.CheckedChanged
        Dim strParent As String
        Dim pNode As ERANode = Me.EraNode.Parent
        If pNode Is Nothing Then pNode = Me.EraNode 'just in case
        If cbInline.Checked Then
            cbShowHeaders.Enabled = False
            cbShowHeaders.Checked = Not pNode.GetAttributeValue("showheaders").Equals("false", StringComparison.CurrentCultureIgnoreCase)
            cbSummarization.Enabled = False
            cbSummarization.Checked = pNode.isSummarization
            gvSortFields.Enabled = False
            gvSortFields.Visible = False
            lblSortFields.Visible = False
            gvKeyFields.Height = Me.Height - 234
            If pNode.isSummarization Then
                UpdateParentKeyDDL()
                For Each row As DataGridViewRow In gvKeyFields.Rows
                    If row.Cells(0).Value Is Nothing Then Continue For
                    strParent = row.Cells(0).Value
                    If ParentKeyAliases.ContainsKey(strParent) Then
                        strParent = ParentKeyAliases(strParent)
                    End If
                    If ddlParentKey.Items.Contains(strParent) Then
                        row.Cells(0).Value = strParent
                    Else
                        row.Cells(0).Value = Nothing
                    End If
                Next
            End If
        Else
            cbShowHeaders.Enabled = True
            cbShowHeaders.Checked = Not Me.EraNode.GetAttributeValue("showheaders").Equals("false", StringComparison.CurrentCultureIgnoreCase)
            cbSummarization.Enabled = True
            cbSummarization.Checked = Me.EraNode.isSummarization
            gvSortFields.Enabled = True
            gvSortFields.Visible = True
            lblSortFields.Visible = True
            gvKeyFields.Height = 119
            If pNode.isSummarization Then
                UpdateParentKeyDDL()
                For Each row As DataGridViewRow In gvKeyFields.Rows
                    If row.Cells(0).Value Is Nothing Then Continue For
                    strParent = GetParentKey(row.Cells(0).Value)
                    If ddlParentKey.Items.Contains(strParent) Then
                        row.Cells(0).Value = strParent
                    Else
                        row.Cells(0).Value = Nothing
                    End If
                Next
            End If
        End If
        ParentForm.hasChanged = True
    End Sub

    Private Sub cbShowHeaders_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbShowHeaders.CheckedChanged
        ParentForm.hasChanged = True
    End Sub

    Private Sub cbSummarization_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbSummarization.CheckedChanged
        ParentForm.hasChanged = True
    End Sub

    Private Sub rbAttributeJoin_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbAttributeJoin.CheckedChanged
        If rbAttributeJoin.Checked Then
            gvKeyFields.Enabled = True
            gvKeyFields.Visible = True
            lblKeyFields.Visible = True
            lblSortFields.Top = 365
            gvSortFields.Top = 356
            If Me.Height > 400 Then gvSortFields.Height = Me.Height - 359
        End If
    End Sub

    Private Sub rbSpatialJoin_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbSpatialJoin.CheckedChanged
        If rbSpatialJoin.Checked Then
            gvKeyFields.Enabled = False
            gvKeyFields.Visible = False
            lblKeyFields.Visible = False
            lblSortFields.Top = 240
            gvSortFields.Top = 231
            If Me.Height > 400 Then gvSortFields.Height = Me.Height - 234
        End If
    End Sub
End Class
