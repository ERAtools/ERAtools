Imports ESRI.ArcGIS.CatalogUI, ESRI.ArcGIS.Catalog, ESRI.ArcGIS.Carto, ESRI.ArcGIS.Geodatabase, ESRI.ArcGIS.GeodatabaseUI
Imports ESRI.ArcGIS.Framework

Public Class ERALayer
    Inherits ERAControl

    Private qb As QueryBuilder
    Private fieldList As ValueCollection

    'Public Sub New(ByRef EraNode As ERANode, ByRef form As frmReportDefinitionEditor)
    '    ' This call is required by the Windows Form Designer.
    '    InitializeComponent()

    '    ' Add any initialization after the InitializeComponent() call.
    '    Me.frde_Parent = form
    '    Me.EraNode = EraNode
    'End Sub

    Private AnalysisTypeNames As String() = {"comparison", "distance", "distance and direction", "acreage", "buffer", "pinpoint", "weighted average"}
    Private Property SelectedAnalysisValue() As String
        Get
            If ddlAnalysisType.SelectedIndex < 0 Then
                Return ""
            End If
            Return AnalysisTypeNames(ddlAnalysisType.SelectedIndex)
        End Get
        Set(ByVal value As String)
            ddlAnalysisType.SelectedIndex = Array.IndexOf(AnalysisTypeNames, value)
        End Set
    End Property

    Private Property SelectedAnalysisType() As AnalysisTypes
        Get
            Return ddlAnalysisType.SelectedIndex
        End Get
        Set(ByVal value As AnalysisTypes)
            ddlAnalysisType.SelectedIndex = value
        End Set
    End Property

    Private Enum AnalysisTypes As Integer
        None = -1
        ComparisonResourse = 0
        Distance = 1
        DistanceAndDirection = 2
        FeatureComparison = 3
        GenericResource = 4
        Pinpoint = 5
        WeightedAverage = 6
    End Enum

    Public ReadOnly Property SelectedAnalysisTypeDisplay() As String
        Get
            Return ddlAnalysisType.Text
        End Get
    End Property

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
                    Exit Property
                ElseIf item.Equals(value, StringComparison.CurrentCultureIgnoreCase) Then
                    ddlLayer.SelectedItem = item
                    Exit Property
                End If
            Next
            ddlLayer.Text = value
        End Set
    End Property

    Private Sub UpdateWorkspaceDDL()
        ddlWorkspace.Items.Clear()
        For Each wsNode As ERANode In ParentForm.WorkspacesNode.Nodes
            ddlWorkspace.Items.Add(wsNode.GetAttributeValue("name"))
        Next
    End Sub

    Public Sub UpdateLayerDDL(ByRef wsNode As WorkspaceNode)
        ddlLayer.Items.Clear()
        For Each strLayer As String In wsNode.LayerCache
            If Not strLayer.EndsWith("(Table)") Then
                ddlLayer.Items.Add(strLayer)
            End If
        Next
    End Sub

    Private Sub UpdateSortFieldsDDL()
        gvSortFields.Rows.Clear()
        colSortFieldName.DataSource = Nothing
        Dim strCollect As New Specialized.StringCollection
        fieldList = New ValueCollection
        If SelectedAnalysisType = AnalysisTypes.Distance Then
            strCollect.Add("Distance")
            strCollect.Add("Location")
            fieldList.AddPair(Xml.XmlConvert.EncodeLocalName(EraNode.GetMyName & "_Distance"), "Distance")
            fieldList.AddPair(Xml.XmlConvert.EncodeLocalName(EraNode.GetMyName & "_Location"), "Location")
        ElseIf SelectedAnalysisType = AnalysisTypes.DistanceAndDirection Then
            strCollect.Add("Distance")
            strCollect.Add("Location")
            strCollect.Add("Direction")
            fieldList.AddPair(Xml.XmlConvert.EncodeLocalName(EraNode.GetMyName & "_Distance"), "Distance")
            fieldList.AddPair(Xml.XmlConvert.EncodeLocalName(EraNode.GetMyName & "_Location"), "Location")
            fieldList.AddPair(Xml.XmlConvert.EncodeLocalName(EraNode.GetMyName & "_Direction"), "Direction")
        ElseIf SelectedAnalysisType = AnalysisTypes.FeatureComparison Then
            strCollect.Add("Feature Comparison Result")
            fieldList.AddPair(Xml.XmlConvert.EncodeLocalName(EraNode.GetMyName & "_ERAsummarizedvalue"), "Feature Comparison Result")
        End If
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

    'Browse for the data set
    Private Sub BrowseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseButton.Click
        Dim LayerInfo As LayerConnectInfo = ESRIHelpers.BrowseForLayers(Me)
        'Dim el As Xml.XmlElement = ESRIHelpers.BrowseForLayers(Me)
        'If el Is Nothing Then
        '    Exit Sub
        'End If
        If LayerInfo Is Nothing Then Exit Sub
        UpdateWorkspaceDDL()
        ddlWorkspace.SelectedItem = LayerInfo.WorkspaceID
        SelectedLayerID = LayerInfo.LayerID
    End Sub

    Private Sub gv_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles gvSortFields.DataError
        'ignore, it just means the selected item isn't a valid field. this is better caught through our custom validation
        e.Cancel = True
    End Sub


    'Validate the where expression
    'Private Sub ValidateButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Try
    '        If My.Application.ESRI_Status <> My.ESRI_Status_Enum.Connected Then
    '            If Not My.Application.ConnectToESRI Then
    '                MessageBox.Show("Unable to check out an ArcView, ArcEditor or ArcInfo license. Status returned:" & vbNewLine & My.Application.GetESRIStatusMessage)
    '                Exit Sub
    '            End If
    '        End If
    '        Dim wsNode As ERANode = Me.frde_Parent.getWorkspaceNode(ddlWorkspace.SelectedItem)
    '        Dim pFeatureWorkspace As IFeatureWorkspace = ESRIHelpers.getIWorkspaceFromEraNode(wsNode)
    '        Dim pFeatureClass As IFeatureClass = pFeatureWorkspace.OpenFeatureClass(SelectedLayerID)
    '        Dim pQueryFilter As New QueryFilter
    '        pQueryFilter.WhereClause = txtWhereExpression.Text
    '        Dim pFeatureCursor As IFeatureCursor = pFeatureClass.Search(pQueryFilter, False)
    '        If pFeatureCursor Is Nothing Then
    '            'todo: I'm not sure if this is the result of a poorly formed where expression or not
    '            MessageBox.Show("The expression is not valid.", "Validation Failed")
    '        Else
    '            Dim pFeature As IFeature = pFeatureCursor.NextFeature
    '            If pFeature Is Nothing Then
    '                ' warn user that no records are returned
    '                MessageBox.Show("The expression was verified successfully, but no records were returned.", "Validation Complete")
    '            Else
    '                MessageBox.Show("The expression was successfully verified.", "Validation Complete")
    '            End If
    '        End If
    '    Catch ex As Exception
    '        'warn user
    '        MessageBox.Show("Returned the following error:" & vbNewLine & ex.Message, "Validation Failed")
    '    End Try
    'End Sub

    'This is where the changes get saved to the ERANode, not to the XML document
    Friend Overrides Sub SaveChangesToNode()
        'Hide the querybuilder, if shown
        Try
            If Not qb Is Nothing Then
                qb.Trash()
                qb = Nothing
            End If
        Catch ex As Exception
            'nothing to do here
        End Try
        With EraNode
            .SetAttributeValue("analysistype", SelectedAnalysisValue)
            .SetAttributeValue("whereexpression", txtWhereExpression.Text)
            .SetAttributeValue("workspace", ddlWorkspace.SelectedItem)
            .SetAttributeValue("layerid", SelectedLayerID)
            .SetAttributeValue("showheaders", cbShowHeaders.Checked)
            If cbSummarization.Visible Then
                .SetAttributeValue("summarization", cbSummarization.Checked)
            Else
                .SetAttributeValue("summarization", False)
            End If
            .CachedFields = Nothing
        End With
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
                If gvr.Cells(0).Value IsNot Nothing Then
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
            End If
        Next
        With EraNode
            If Not .GetMyName.Equals(txtName.Text.Trim) Then
                DirectCast(EraNode, LayerTableNode).ChangedTableName(Xml.XmlConvert.EncodeLocalName(.GetMyName), Xml.XmlConvert.EncodeLocalName(txtName.Text.Trim))
            End If
            .SetAttributeValue("name", txtName.Text.Trim)
        End With
        Debug.Print(EraNode.XMLNode.OuterXml)
        Me.ParentForm.hasChanged = False
    End Sub

    Friend Overrides Sub LoadData()
        With EraNode
            txtName.Text = .GetAttributeValue("name")
            SelectedAnalysisValue = .GetAttributeValue("analysistype")
            txtWhereExpression.Text = .GetAttributeValue("whereexpression")
            UpdateWorkspaceDDL()
            ddlWorkspace.SelectedItem = .GetAttributeValue("workspace")
            SelectedLayerID = .GetAttributeValue("layerid")
            cbShowHeaders.Checked = Not .GetAttributeValue("showheaders").Equals("false", StringComparison.CurrentCultureIgnoreCase)
            cbSummarization.Checked = .isSummarization
            UpdateSortFieldsDDL()
        End With
        Me.ParentForm.hasChanged = False
        ShowErrors()
    End Sub

    Private Sub ddlWorkspace_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlWorkspace.SelectedIndexChanged
        If ddlWorkspace.SelectedIndex = -1 Then
            Exit Sub
        End If
        MyBase.myErrorProvider.SetError(ddlWorkspace, "")
        If EraNode.ErrorText.ToLower = "Select a workspace from the list" Then EraNode.ClearErrors()

        Dim wsNode As WorkspaceNode = ParentForm.getWorkspaceNode(ddlWorkspace.SelectedItem)
        If Not wsNode Is Nothing Then
            Dim wspace As IWorkspace = wsNode.Workspace 'ESRIHelpers.getIWorkspaceFromEraNode(wsNode)
            If Not wspace Is Nothing Then
                My.Application.StartWait()
                UpdateLayerDDL(wsNode)
                My.Application.EndWait()
            Else
                ddlLayer.Items.Clear()
            End If
        Else
            ddlWorkspace.SelectedIndex = -1
        End If
        Me.ParentForm.hasChanged = True
    End Sub

    Private Sub ddlAnalysisType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAnalysisType.SelectedIndexChanged
        'Why aren't we sorting on Weighted Averages?
        If ddlAnalysisType.SelectedIndex > 0 AndAlso ddlAnalysisType.SelectedIndex < 6 Then
            gvSortFields.Enabled = True
            UpdateSortFieldsDDL()
        Else
            gvSortFields.Enabled = False
        End If
        'Make sure that only Pinpoint and Buffered Analysis are available as summarizations
        If SelectedAnalysisType = AnalysisTypes.Pinpoint OrElse SelectedAnalysisType = AnalysisTypes.GenericResource Then
            cbSummarization.Visible = True
        Else
            cbSummarization.Visible = False
        End If
    End Sub

    Private Sub AnyTextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAnalysisType.TextChanged, ddlLayer.TextChanged, ddlWorkspace.TextChanged, txtName.TextChanged, txtWhereExpression.TextChanged
        If Not ParentForm Is Nothing Then
            Me.ParentForm.hasChanged = True
        End If
    End Sub

    Private Sub txtName_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtName.Validating
        myErrorProvider.SetError(sender, "")
        If txtName.Text = "" Then
            myErrorProvider.SetError(txtName, "Name is a required field")
            'e.Cancel = True
        End If
    End Sub

    Private Sub QueryBuilderButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QueryBuilderButton.Click
        If My.Application.ESRI_Status <> My.ESRI_Status_Enum.Connected Then
            If Not My.Application.ConnectToESRI Then
                MessageBox.Show("Cannot display querybuilder. Unable to check out an ArcView, ArcEditor or ArcInfo license. Status returned:" & vbNewLine & My.Application.GetESRIStatusMessage, "Unable to Run Query Builder", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If
        End If
        Try
            If ddlWorkspace.Text = "" Then
                MessageBox.Show("You do not have a connection to a valid workspace. Cannot display query builder without connection to a valid layer in a valid workspace.", "Cannot Show Query Builder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If
            Dim wsNode As WorkspaceNode = ParentForm.getWorkspaceNode(ddlWorkspace.SelectedItem)
            If wsNode Is Nothing Then
                MessageBox.Show("You do not have a connection to a valid workspace. Cannot display query builder without connection to a valid layer in a valid workspace.", "Cannot Show Query Builder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If
            'Dim ErrorStatus As String = ""
            Dim ws As IFeatureWorkspace = wsNode.Workspace 'ESRIHelpers.getIWorkspaceFromEraNode(wsNode, ErrorStatus)
            If ws Is Nothing Then
                MessageBox.Show("You do not have a connection to a valid workspace. Cannot display query builder without connection to a valid layer in a valid workspace.", "Cannot Show Query Builder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If
            If SelectedLayerID = "" Then
                MessageBox.Show("You have not specified a valid layer. Cannot display query builder without connection to a valid layer in a valid workspace.", "Cannot Show Query Builder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If
            Dim pTable As ITable
            Try
                pTable = ws.OpenFeatureClass(SelectedLayerID)
            Catch ex As Exception
                MessageBox.Show("You have not specified a valid layer. Cannot display query builder without connection to a valid layer in a valid workspace. ESRI error message: " & ex.Message, "Cannot Show Query Builder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End Try
            qb = New QueryBuilder
            qb.ShowQueryBuilder(pTable, Me.txtWhereExpression)
        Catch ex As Exception
            MessageBox.Show("Error displaying query builder: " & ex.Message, "Error Displaying Query Builder", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Overrides Sub ShowErrors()
        myErrorProvider.Clear()
        For Each ve As ERANode.ValidationErrorMessage In Me.EraNode.Errors
            Select Case ve.ValidationErrorTarget
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.DisplayName
                    myErrorProvider.SetError(txtName, ve.ErrorMessage)
                Case ERAtools2Admin.ERANode.ValidationErrorMessage.TargetEnum.AnalysisType
                    myErrorProvider.SetError(ddlAnalysisType, ve.ErrorMessage)
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
            End Select
        Next
    End Sub

    Private Sub ERALayer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.txtName.Focus()
    End Sub

    Private Sub cbShowHeaders_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbShowHeaders.CheckedChanged
        ParentForm.hasChanged = True
    End Sub

    Private Sub ddlLayer_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlLayer.Validated
        If ddlLayer.Text <> "" AndAlso (txtName.Text = "New Analysis" OrElse txtName.Text = "") Then
            txtName.Text = AliasNameFormatter.FormatShortNameAlias(SelectedLayerID)
        End If
    End Sub
End Class
