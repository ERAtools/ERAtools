Imports System.Xml
Imports ESRI.ArcGIS.Geodatabase

Public Class WorkspacesNode
    Inherits ERANode

    Public Overrides ReadOnly Property ERANodeType() As ERANode.ERANodeTypeEnum
        Get
            Return ERANodeTypeEnum.Workspaces
        End Get
    End Property

    Public Overrides Sub ValidateNode()
        'No properties of this node to validate
        'Validate child nodes
        For Each node As WorkspaceNode In Me.Nodes
            node.ValidateNode()
        Next
    End Sub
End Class

Public MustInherit Class WorkspaceNode
    Inherits ERANode

    Public Overrides Sub ValidateNode()
        Try
            Me.ClearErrors()
            'Dim ErrorStatus As String = ""
            'ESRIHelpers.getIWorkspaceFromEraNode(Me, ErrorStatus)
            Try
                Me.GetWorkspace()
            Catch
                'errors are caught and reported in GetWorkspace
            End Try
            Dim dupNode As ERANode = Me.ParentForm.getWorkspaceNode(Me.GetAttributeValue("name"))
            If dupNode Is Nothing Then
                'all is well, item not found in the collection
            ElseIf dupNode Is Me Then
                'all is still well, item found is this workspace
            Else
                Me.AddError("Name must be unique. There is already another workspace with that name.", ValidationErrorMessage.TargetEnum.DisplayName)
            End If
            'no children to validate
        Catch ex As Exception
            MessageBox.Show("Error validating workspace node " & Me.Text & ": " & ex.Message, "Error Validating Workspace Node", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Function GetWorkspace() As IWorkspace
        If My.Application.ESRI_Status <> My.ESRI_Status_Enum.Connected Then
            Return Nothing
        End If

        Try
            Dim pPropertySet As ESRI.ArcGIS.esriSystem.IPropertySet = GetWorkspaceProperties()
            If pPropertySet Is Nothing Then Return Nothing
            Dim pWorkspaceFactory As IWorkspaceFactory = GetWorkspaceFactory()

            Return pWorkspaceFactory.Open(pPropertySet, Nothing)
        Catch ex As Exception
            InterpretConnectionException(ex)
            Return Nothing
        End Try
    End Function

    'these methods vary by the specific type of workspace
    Public MustOverride Function GetWorkspaceProperties() As ESRI.ArcGIS.esriSystem.IPropertySet

    Public MustOverride Function GetWorkspaceFactory() As ESRI.ArcGIS.Geodatabase.IWorkspaceFactory

    Public MustOverride Sub InterpretConnectionException(ByVal ex As Exception)

End Class

Public Class AccessWorkspaceNode
    Inherits WorkspaceNode

    Public Property Database() As String
        Get
            Return GetAttributeValue("database")
        End Get
        Set(ByVal value As String)
            SetAttributeValue("database", value)
        End Set
    End Property

    Public Overrides ReadOnly Property ERANodeType() As ERANode.ERANodeTypeEnum
        Get
            Return ERANodeTypeEnum.AccessWorkspace
        End Get
    End Property

    Public Overrides Function GetWorkspaceProperties() As ESRI.ArcGIS.esriSystem.IPropertySet
        Dim pPropset As ESRI.ArcGIS.esriSystem.IPropertySet = New ESRI.ArcGIS.esriSystem.PropertySet
        If IO.File.Exists(Database) Or IO.Directory.Exists(Database) Then
            pPropset.SetProperty("DATABASE", Database)
        Else
            My.Application.EndWait()
            AddError("File or path not found", ValidationErrorMessage.TargetEnum.Database)
            Return Nothing
        End If
        Return pPropset
    End Function


    Public Overrides Function GetWorkspaceFactory() As ESRI.ArcGIS.Geodatabase.IWorkspaceFactory
        Dim pFact As IWorkspaceFactory
        If IO.Path.GetExtension(GetAttributeValue("database")) = ".gdb" Then
            pFact = New ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactory
        Else
            pFact = New ESRI.ArcGIS.DataSourcesGDB.AccessWorkspaceFactory
        End If
        Return pFact
    End Function

    Public Overrides Sub InterpretConnectionException(ByVal ex As System.Exception)
        Dim strError As String
        If IO.Path.GetExtension(GetAttributeValue("database")) = ".gdb" Then
            'path not found errors are reported in GetWorkspace Properties
            If ex.Message = "Error HRESULT E_FAIL has been returned from a call to a COM component." Then
                strError = "Path does not reference a valid file-based geodatabase."
            Else
                'have no idea what's going on here, so just send the exception text back 
                strError = "Unable to open the workspace """ & Database & """. Error message returned: " & ex.Message
            End If
        Else 'mdb
            If ex.Message = "Exception from HRESULT: 0x80040213" Then
                'thrown when the file is not found, or is not really a personal geodatabase file

                If IO.File.Exists(Database) Then
                    Try
                        Dim fs As IO.FileStream = IO.File.Open(Database, IO.FileMode.Open)
                        If fs.CanRead Then
                            'readable file, but doesn't appear to be a valid access database
                            strError = "Unable to open the file """ & Database & """. File is not a valid Access database file."
                        Else
                            'authorized to read it (the UnauthorizedAccessException wasn't thrown), but cannot for some reason
                            strError = "Unknown problem reading the file """ & Database & """: " & ex.Message
                        End If
                        fs.Close()
                    Catch exIO As UnauthorizedAccessException
                        strError = "You do not have permission to open the file """ & Database & """."
                    End Try
                Else
                    strError = "Unable to open the file """ & Database & """. File not found."
                End If
            ElseIf ex.Message = "Exception from HRESULT: 0x80040216" Then
                'file is opened exclusivly
                strError = "Unable to open the file """ & Database & """. The database file has been opened exclusively by another user."
            Else
                'have no idea what's going on here, so just send the exception text back 
                strError = "Unknown problem reading the file """ & Database & """: " & ex.Message
            End If
        End If
        AddError(strError, ValidationErrorMessage.TargetEnum.Database)
    End Sub
End Class

Public Class SDEWorkspaceNode
    Inherits WorkspaceNode


    Public Overrides ReadOnly Property ERANodeType() As ERANode.ERANodeTypeEnum
        Get
            Return ERANodeTypeEnum.SDEWorkspace
        End Get
    End Property

    Public Overrides Function GetWorkspaceFactory() As ESRI.ArcGIS.Geodatabase.IWorkspaceFactory
        Return New ESRI.ArcGIS.DataSourcesGDB.SdeWorkspaceFactory
    End Function

    Public Overrides Function GetWorkspaceProperties() As ESRI.ArcGIS.esriSystem.IPropertySet
        Dim pPropset As ESRI.ArcGIS.esriSystem.IPropertySet = New ESRI.ArcGIS.esriSystem.PropertySet
        pPropset.SetProperty("Server", GetAttributeValue("server"))
        pPropset.SetProperty("Instance", GetAttributeValue("instance"))
        pPropset.SetProperty("Database", GetAttributeValue("database")) ' Ignored with ArcSDE for Oracle 
        pPropset.SetProperty("user", GetAttributeValue("user"))
        pPropset.SetProperty("password", GetAttributeValue("password"))
        pPropset.SetProperty("version", "sde.DEFAULT")
        Return pPropset
    End Function

    Public Overrides Sub InterpretConnectionException(ByVal ex As System.Exception)
        Select Case ex.Message
            Case "SDE not running on server"
                AddError(ex.Message, ValidationErrorMessage.TargetEnum.SDEServer)
            Case "Entry for SDE instance not found in services file"
                AddError(ex.Message, ValidationErrorMessage.TargetEnum.SDEInstance)
            Case "Server machine not found"
                AddError(ex.Message, ValidationErrorMessage.TargetEnum.SDEServer)
            Case "Bad login user"
                AddError(ex.Message, ValidationErrorMessage.TargetEnum.UserNamePassword)
            Case Else
                AddError(ex.Message, ValidationErrorMessage.TargetEnum.TreeviewOnly)

        End Select
    End Sub
End Class

Public Class CoverageWorkspaceNode
    Inherits WorkspaceNode

    Private m_Directory As String
    Public ReadOnly Property Directory() As String
        Get
            Return m_Directory
        End Get
    End Property

    Public Overrides ReadOnly Property ERANodeType() As ERANode.ERANodeTypeEnum
        Get
            Return ERANodeTypeEnum.CoverageWorkspace
        End Get
    End Property

    Public Overrides Function GetWorkspaceFactory() As ESRI.ArcGIS.Geodatabase.IWorkspaceFactory
        Return New ESRI.ArcGIS.DataSourcesFile.ArcInfoWorkspaceFactory
    End Function

    Public Overrides Function GetWorkspaceProperties() As ESRI.ArcGIS.esriSystem.IPropertySet
        m_Directory = GetAttributeValue("directory")
        Dim di As New IO.DirectoryInfo(m_Directory)

        'Check that the directory exists and is readable. It's better to handle this here than to deal with the HRESULT E_FAIL exception thrown by pWorkspaceFactory.Open in GetWorkspace

        If Not di.Exists Then
            Throw New IO.IOException("The directory """ & m_Directory & """ does not exist.")
        End If

        Dim pPropset As ESRI.ArcGIS.esriSystem.IPropertySet = New ESRI.ArcGIS.esriSystem.PropertySet
        pPropset.SetProperty("DATABASE", Directory)

        Return pPropset
    End Function

    Public Overrides Sub InterpretConnectionException(ByVal ex As System.Exception)
        'The likely errors have already been interpreted by code in GetWorkspaceProperties, which will throw meaningful exceptions
        AddError(ex.Message, ValidationErrorMessage.TargetEnum.Database)
    End Sub
End Class


Public Class ShapeWorkspaceNode
    Inherits WorkspaceNode

    Private m_Directory As String
    Public ReadOnly Property Directory() As String
        Get
            Return m_Directory
        End Get
    End Property

    Public Overrides ReadOnly Property ERANodeType() As ERANode.ERANodeTypeEnum
        Get
            Return ERANodeTypeEnum.ShapeWorkspace
        End Get
    End Property

    Public Overrides Function GetWorkspaceFactory() As ESRI.ArcGIS.Geodatabase.IWorkspaceFactory
        Return New ESRI.ArcGIS.DataSourcesFile.ShapefileWorkspaceFactory
    End Function

    Public Overrides Function GetWorkspaceProperties() As ESRI.ArcGIS.esriSystem.IPropertySet
        m_Directory = GetAttributeValue("directory")
        Dim di As New IO.DirectoryInfo(m_Directory)

        'Check that the directory exists and is readable. It's better to handle this here than to deal with the HRESULT E_FAIL exception thrown by pWorkspaceFactory.Open in GetWorkspace

        If Not di.Exists Then
            Throw New IO.IOException("The directory """ & m_Directory & """ does not exist.")
        ElseIf di.GetFiles("*.shp").GetUpperBound(0) = 0 Then
            'This isn't really what I'm worried about, but if the directory exists but the user cannot read it b/c of permissions issues, an exception will be thrown on GetFiles
            Throw New IO.IOException("The directory """ & m_Directory & """ does not contain any shapefiles.")
        End If

        Dim pPropset As ESRI.ArcGIS.esriSystem.IPropertySet = New ESRI.ArcGIS.esriSystem.PropertySet
        pPropset.SetProperty("DATABASE", Directory)

        Return pPropset
    End Function

    Public Overrides Sub InterpretConnectionException(ByVal ex As System.Exception)
        'The likely errors have already been interpreted by code in GetWorkspaceProperties, which will throw meaningful exceptions
        AddError(ex.Message, ValidationErrorMessage.TargetEnum.Database)
    End Sub
End Class

Public Class EraNodeFactory

    Public Shared Function CreateEraNode(ByVal xmlNode As XmlNode)
        Dim NewNode As ERANode
        Select Case xmlNode.Name
            Case "WORKSPACES"
                NewNode = New WorkspacesNode
            Case "SDEWORKSPACE"
                NewNode = New SDEWorkspaceNode
            Case "ACCESSWORKSPACE"
                NewNode = New AccessWorkspaceNode
            Case "SHAPEWORKSPACE"
                NewNode = New ShapeWorkspaceNode
            Case "ISSUE"
                NewNode = New IssueNode
            Case "LAYER"
                NewNode = New LayerNode
            Case "TABLE"
                NewNode = New TableNode
            Case "FIELD"
                NewNode = New FieldNode
            Case Else
                Throw New EraNodeException("Unrecognized XML element: " & xmlNode.Name)
        End Select
        NewNode.XMLNode = xmlNode
        Return NewNode
    End Function

End Class

Public Class EraNodeException
    Inherits Exception
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
End Class


Public Class IssueNode
    Inherits ERANode

    Public Overrides ReadOnly Property ERANodeType() As ERANode.ERANodeTypeEnum
        Get
            Return ERANodeTypeEnum.Issue
        End Get
    End Property

    Public Overrides Sub ValidateNode()
        Me.ClearErrors()
        If Me.Nodes.Count < 1 Then
            AddError("Empty Issue - Please add analysis layers to this issue", ValidationErrorMessage.TargetEnum.TreeviewOnly)
        Else
            'validate children
            For Each node As LayerNode In Me.Nodes
                node.ValidateNode()
            Next
        End If
    End Sub
End Class

Public MustInherit Class LayerTableNode
    Inherits ERANode

    Public Function GetObjectClass(ByVal LogErrors As Boolean) As IObjectClass
        Try
            Dim pObjectClass As IObjectClass
            Dim wsNode As WorkspaceNode = Me.ParentForm.getWorkspaceNode(Me.GetAttributeValue("workspace"))
            If wsNode Is Nothing Then
                If LogErrors Then AddError("Select a workspace from the list", ValidationErrorMessage.TargetEnum.Workspace)
                Return Nothing
            End If
            wsNode.ValidateNode()
            If wsNode.HasError Then
                If LogErrors Then AddError("Invalid workspace", ValidationErrorMessage.TargetEnum.Workspace)
                Return Nothing
            Else
                Dim wspace As IFeatureWorkspace = wsNode.GetWorkspace 'ESRIHelpers.getIWorkspaceFromEraNode(wsNode, strWspaceError)
                If wspace Is Nothing Then
                    If LogErrors Then AddError(wsNode.ErrorText, ValidationErrorMessage.TargetEnum.Workspace)
                    Return Nothing
                End If
                If GetAttributeValue("layerid") = "" Then
                    If LogErrors Then AddError("You must enter or select the name of a valid feature class", ValidationErrorMessage.TargetEnum.LayerTableFieldID)
                    Return Nothing
                End If
                'This whole section has a fundamental flaw: 
                'the enumeration is too specific, looking for FeatureClasses or Tables, but not both, and you get errors if you try to use a feature class as if it were a table,
                'something which should be permitted
                'Try
                '    Dim pEnumDatasetName As IEnumDatasetName
                '    If XMLNode.Name = "LAYER" Then
                '        pEnumDatasetName = DirectCast(wspace, IWorkspace).DatasetNames(esriDatasetType.esriDTFeatureClass)
                '    Else
                '        pEnumDatasetName = DirectCast(wspace, IWorkspace).DatasetNames(esriDatasetType.esriDTTable)
                '    End If


                '    Dim pDatasetName As IDatasetName = pEnumDatasetName.Next
                '    Do While pDatasetName IsNot Nothing
                '        If GetAttributeValue("layerid") = pDatasetName.Name Then
                '            'we found the layer, should be good to go
                '            Exit Try
                '        End If
                '        pDatasetName = pEnumDatasetName.Next
                '    Loop
                '    Throw New Exception("Invalid Layer ID")

                '    'pTable = wspace.OpenFeatureClass(LayerOrTableEraNode.GetAttributeValue("layerid"))
                '    'If pTable Is Nothing Then
                '    '    LayerOrTableEraNode.ErrorText = "Invalid Layer ID"
                '    '    Exit Sub
                '    'End If
                'Catch ex As Exception
                '    If LogErrors Then AddError("Error opening table or feature class: " & ex.Message, ValidationErrorMessage.TargetEnum.LayerTableFieldID)
                '    Return Nothing
                'End Try
                Try
                    If XMLNode.Name = "LAYER" Then
                        pObjectClass = wspace.OpenFeatureClass(GetAttributeValue("layerid"))
                    Else
                        pObjectClass = wspace.OpenTable(GetAttributeValue("layerid"))
                        If pObjectClass Is Nothing Then
                            If LogErrors Then AddError("Invalid Layer ID", ValidationErrorMessage.TargetEnum.LayerTableFieldID)
                            Return Nothing
                        End If
                    End If

                Catch ex As Exception
                    If LogErrors Then AddError("Error opening table or feature class: " & InterpretExceptionMessage(ex.Message), ValidationErrorMessage.TargetEnum.LayerTableFieldID)
                    Return Nothing
                End Try
                If LogErrors Then ValidateWhereExpression(pObjectClass)
                Return pObjectClass
            End If

        Catch ex As Exception
            MessageBox.Show("Error opening object class: " & ex.Message, "Error Opening Object Class", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End Try

    End Function

    Protected Sub ValidateWhereExpression(ByRef pTable As ITable)
        If pTable Is Nothing Then Exit Sub
        If GetAttributeValue("whereexpression") = "" Then Exit Sub
        Dim pCursor As ICursor
        Try
            Dim pQueryFilter As New QueryFilter
            pQueryFilter.WhereClause = GetAttributeValue("whereexpression")
            pCursor = pTable.Search(pQueryFilter, False)
            Dim pRow As IRow = pCursor.NextRow
            If pRow Is Nothing Then
                AddError("The where expression validated correctly, but returns no records.", ValidationErrorMessage.TargetEnum.WhereExpression)
            End If
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor)

        Catch ex As Exception
            'This block is an attempt to interpret the varying and often misleading, or at least non-informative error messages returned when the definition query is not valid
            Dim strMessage As String = ex.Message
            Dim TestMessages As New DataTable
            TestMessages.Columns.Add("ExceptionMessage")
            TestMessages.Columns.Add("InterpretedMessage")

            TestMessages.Rows.Add(New String() {"The owner SID on a per-user subscription doesn't exist (Exception from HRESULT: 0x80040207)", "Invalid syntax in where clause"}) 'shapefile incorrect syntax
            TestMessages.Rows.Add(New String() {"Exception from HRESULT: 0x80040653", "An expected field was not found or could not be retrieved properly."})
            'TestMessages.Rows.Add(New String() {"Invalid SQL syntax") 'What SDE returns if it doesn't find an operator
            'TestMessages.Rows.Add(New String() {"Attribute column not found") 'What SDE returns if a field is referenced that doesn't exist in the dataset
            'TestMessages.Rows.Add(New String() {"Syntax error in string in query expression") 'Personal geodatabase, basic syntax error
            'TestMessages.Rows.Add(New String() {"Too few parameters. Expected") 'Personal geodatabase, referencing field that doesn't exist.
            'TestMessages.Rows.Add(New String() {"Data type mismatch in criteria expression.") 'Personal geodatabase, error as it says

            For Each TestMessageRow As DataRow In TestMessages.Rows
                If strMessage.StartsWith(TestMessageRow("ExceptionMessage"), StringComparison.CurrentCultureIgnoreCase) Then
                    strMessage = TestMessageRow("InterpretedMessage")
                    Exit For
                End If
            Next

            AddError(strMessage, ValidationErrorMessage.TargetEnum.WhereExpression)
        End Try
    End Sub

    Protected Function InterpretExceptionMessage(ByVal ErrorMessage As String) As String
        If ErrorMessage = "Exception from HRESULT: 0x80040351" Then
            ErrorMessage = Me.XMLNode.Name.ToLower & " not found in workspace."
        ElseIf ErrorMessage = "Error HRESULT E_FAIL has been returned from a call to a COM component." Then
            'file is locked, or not really a shapefile or dbf
            ErrorMessage = Me.XMLNode.Name.ToLower & " could not be opened from workspace. The file may be locked by another user or an invalid data set."
            'Access/SDE messages can be fairly specific, but just for consistency's sake:
        ElseIf ErrorMessage.Length >= 20 AndAlso ErrorMessage.Substring(0, 20) = "DBMS table not found" Then
            ErrorMessage = Me.XMLNode.Name.ToLower & " not found in workspace."
        ElseIf ErrorMessage.Length >= 70 AndAlso ErrorMessage.Substring(0, 70) = "The Microsoft Jet database engine cannot find the input table or query" Then
            ErrorMessage = Me.XMLNode.Name.ToLower & " not found in workspace."
        End If
        Return "Error opening " & Me.XMLNode.Name.ToLower & " '" & GetAttributeValue("layerid") & "' in workspace '" & GetAttributeValue("workspace") & "': " & ErrorMessage
    End Function

End Class

Public Class LayerNode
    Inherits LayerTableNode

    Public Overrides ReadOnly Property ERANodeType() As ERANode.ERANodeTypeEnum
        Get
            Return ERANodeTypeEnum.Layer
        End Get
    End Property

    Public Overrides Sub ValidateNode()
        Try
            ClearErrors()
            Dim pFeatureClass As IFeatureClass = GetObjectClass(True)
           
            If GetAttributeValue("name") = "" Then
                AddError("You must enter a name for this analysis", ValidationErrorMessage.TargetEnum.DisplayName)
            End If

            If GetAttributeValue("analysistype") = "" Then
                AddError("You must select an Analysis Type", ValidationErrorMessage.TargetEnum.AnalysisType)
            Else
                'type-specific tests

                Select Case GetAttributeValue("analysistype")
                    Case "pinpoint"
                        'must have at least one field
                        Dim m_FieldNodes As New Generic.List(Of FieldNode)
                        AppendFieldNodes(m_FieldNodes, Me)
                        If m_FieldNodes.Count = 0 Then
                            AddError("This type of analysis must have at least one field.", ValidationErrorMessage.TargetEnum.TreeviewOnly)
                        End If
                    Case "buffer"
                        'must have at least one field
                        Dim m_FieldNodes As New Generic.List(Of FieldNode)
                        AppendFieldNodes(m_FieldNodes, Me)
                        If m_FieldNodes.Count = 0 Then
                            AddError("This type of analysis must have at least one field.", ValidationErrorMessage.TargetEnum.TreeviewOnly)
                        End If
                    Case "weighted average"
                        'must have one (and only one) field, which must be numeric
                        Dim ChildFieldNodes As Generic.List(Of ERANode) = NodesByType(ERANode.ERANodeTypeEnum.Field)
                        If ChildFieldNodes.Count <> 1 Then
                            AddError("This type of analysis must have one (and only one) numeric field.", ValidationErrorMessage.TargetEnum.TreeviewOnly)
                        Else
                            'test that it is numeric
                            If pFeatureClass IsNot Nothing Then
                                Dim intField As Integer = pFeatureClass.Fields.FindField(ChildFieldNodes(0).GetAttributeValue("name"))
                                If intField = -1 Then
                                    'this will be handled in the validation of the field node itself
                                Else
                                    Dim pField As IField = pFeatureClass.Fields.Field(intField)
                                    If Not (pField.VarType = vbSingle Or pField.VarType = vbDouble Or pField.VarType = vbInteger Or pField.VarType = vbLong Or pField.VarType = vbDecimal) Then
                                        AddError("Field is not numeric. This type of analysis must have one (and only one) numeric field.", ValidationErrorMessage.TargetEnum.TreeviewOnly)
                                    End If
                                End If
                            End If

                        End If
                        'feature type must be polygon
                        If pFeatureClass IsNot Nothing AndAlso pFeatureClass.ShapeType <> ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon Then
                            AddError("This type of analysis requires that the layer contain polygon features to work correctly. The analysis will run for point or line features, but will not perform any data summarization.", ValidationErrorMessage.TargetEnum.AnalysisType)
                        End If

                    Case "comparison"
                        'should have no fields (they're just ignored)
                        If NodesByType(ERANode.ERANodeTypeEnum.Field).Count > 0 Then
                            AddError("Fields are ignored for this type of analysis. For clarity, they should not be included in the report definition.", ValidationErrorMessage.TargetEnum.TreeviewOnly)
                        End If
                        'must have one (and only one) child layer
                        Dim ChildTableNodes As Generic.List(Of ERANode) = NodesByType(ERANode.ERANodeTypeEnum.Table)
                        If ChildTableNodes.Count <> 1 Then
                            AddError("This type of analysis requires one (and only one) child layer with which to be compared.", ValidationErrorMessage.TargetEnum.TreeviewOnly)
                        Else
                            'child layer must be polygon or polyline
                            Dim ChildLayerFeatureClass As IFeatureClass = DirectCast(ChildTableNodes(0), LayerTableNode).GetObjectClass(False) 'childtablenodes.ob GetObjectClassFromEraNode(ChildTableNodes.Item(0), frde_parent)
                            If ChildLayerFeatureClass IsNot Nothing AndAlso (ChildLayerFeatureClass.ShapeType <> ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon And ChildLayerFeatureClass.ShapeType <> ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline) Then
                                AddError("This type of analysis requires that the child layer with which to be compared contain polyline or polygon features.", ValidationErrorMessage.TargetEnum.TreeviewOnly)
                            End If
                        End If
                        'layer must contain polygons
                        If pFeatureClass IsNot Nothing AndAlso pFeatureClass.ShapeType <> ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon Then
                            AddError("This type of analysis requires that the layer contain polygon features.", ValidationErrorMessage.TargetEnum.TreeviewOnly)
                        End If

                End Select
            End If

            'validate child nodes
            For Each node As ERANode In Me.Nodes
                node.ValidateNode()
            Next

        Catch ex As Exception
            MessageBox.Show("Error validating analysis: " & ex.Message, "Error Validating Analysis", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    'Appends all field nodes of layer to referenced generic list of fieldnodes, and recursively searches through child tables and appends their field nodes 
    'Used solely for validation of pinpoint and buffer analyses
    Private Sub AppendFieldNodes(ByRef m_FieldNodes As Generic.List(Of FieldNode), ByVal ParentNode As LayerTableNode)
        Dim m_FieldNode As FieldNode

        For Each m_FieldNode In ParentNode.NodesByType(ERANode.ERANodeTypeEnum.Field)
            m_FieldNodes.Add(m_FieldNode)
        Next
        For Each ChildTableNode As TableNode In ParentNode.NodesByType(ERANodeTypeEnum.Table)
            AppendFieldNodes(m_FieldNodes, ChildTableNode)
        Next

    End Sub


End Class

Public Class TableNode
    Inherits LayerTableNode

    Public Overrides ReadOnly Property ERANodeType() As ERANode.ERANodeTypeEnum
        Get
            Return ERANodeTypeEnum.Table
        End Get
    End Property

    Public Overrides Sub ValidateNode()
        Try
            ClearErrors()
            Dim pTable As ITable = GetObjectClass(True)

            If GetAttributeValue("name") = "" Then
                AddError("You must enter a name for this table", ValidationErrorMessage.TargetEnum.DisplayName)
            End If

            Dim ParentNode As LayerTableNode = Parent
            If ParentNode.XMLNode.Name = "LAYER" AndAlso ParentNode.XMLNode.GetAttribute("analysistype") = "comparison" Then
                'child keys should not exist, table is related spatially
                If XMLNode.SelectNodes("KEYS/KEY").Count > 0 Then
                    AddError("Parent/Child keys are not required for relating layers in a comparison resource analysis.", ValidationErrorMessage.TargetEnum.KeyFields)
                End If
            Else 'parent is either a table or layer related by parent/child keys
                If XMLNode.SelectNodes("KEYS/KEY").Count < 1 Then
                    AddError("Parent/Child keys not provided.", ValidationErrorMessage.TargetEnum.KeyFields)
                Else
                    If ParentNode.ERANodeType = ERANode.ERANodeTypeEnum.Layer AndAlso ParentNode.XMLNode.GetAttribute("analysistype").Equals("acreage", StringComparison.CurrentCultureIgnoreCase) Then
                        'Parent is a Feature Comparison Analysis, so it needs to be restricted to fields that are in the output of the layer
                        Dim blnError As Boolean = False
                        For Each keyNode As Xml.XmlElement In XMLNode.SelectNodes("KEYS/KEY")
                            Dim blnFound As Boolean = False
                            For Each parentField As Xml.XmlElement In ParentNode.XMLNode.SelectNodes("FIELD")
                                If parentField.GetAttribute("name").Equals(keyNode.GetAttribute("parent"), StringComparison.CurrentCultureIgnoreCase) Then
                                    blnFound = True
                                    Exit For
                                End If
                            Next
                            If Not blnFound Then
                                If Not blnError Then
                                    AddError("A Table that is the child of a Feature Comparison Analysis MUST have the parent keys in the output of the parent.", ValidationErrorMessage.TargetEnum.KeyFields)
                                End If
                                AddError("The key field " & keyNode.GetAttribute("parent") & " is missing from the parent.", ValidationErrorMessage.TargetEnum.KeyFields)
                                blnError = True
                            End If
                        Next
                    ElseIf ParentNode.isSummarization AndAlso GetAttributeValue("inline").Equals("false", StringComparison.CurrentCultureIgnoreCase) Then
                        'Parent is a Summarization and this is not inline with it, so it needs to be restricted to fields that are in the output of the parent
                        Dim blnError As Boolean = False
                        For Each keyNode As Xml.XmlElement In XMLNode.SelectNodes("KEYS/KEY")
                            Dim blnFound As Boolean = False
                            For Each parentField As Xml.XmlElement In ParentNode.XMLNode.SelectNodes("FIELD")
                                If parentField.GetAttribute("name").Equals(keyNode.GetAttribute("parent"), StringComparison.CurrentCultureIgnoreCase) Then
                                    blnFound = True
                                    Exit For
                                End If
                            Next
                            If Not blnFound Then
                                If Not blnError Then
                                    AddError("A Table that is nested under (not inline with) a Summarized Layer/Table MUST have the parent keys in the output of its parent.", ValidationErrorMessage.TargetEnum.KeyFields)
                                End If
                                AddError("The key field " & keyNode.GetAttribute("parent") & " is missing from the parent.", ValidationErrorMessage.TargetEnum.KeyFields)
                                blnError = True
                            End If
                        Next
                    End If
                    Dim ParentTable As ITable = ParentNode.GetObjectClass(False) 'GetObjectClassFromEraNode(ParentNode, frde_parent, False)
                    If pTable IsNot Nothing AndAlso ParentTable IsNot Nothing Then
                        For Each keyNode As Xml.XmlElement In XMLNode.SelectNodes("KEYS/KEY")
                            Dim iParent As Integer = ParentTable.FindField(keyNode.GetAttribute("parent"))
                            If iParent < 0 Then
                                AddError("Key field " & keyNode.GetAttribute("parent") & " is missing from the parent data source.", ValidationErrorMessage.TargetEnum.KeyFields)
                            End If
                            Dim iChild As Integer = pTable.FindField(keyNode.GetAttribute("child"))
                            If iChild < 0 Then
                                AddError("Key field " & keyNode.GetAttribute("child") & " is missing from the data source.", ValidationErrorMessage.TargetEnum.KeyFields)
                            End If
                            If iChild >= 0 And iParent >= 0 Then
                                'compare data types
                                Dim pChildField As IField = pTable.Fields.Field(iChild)
                                Dim pParentField As IField = ParentTable.Fields.Field(iParent)
                                If pChildField.VarType <> pParentField.VarType Then
                                    AddError("Data type mismatch in key fields (" & keyNode.GetAttribute("parent") & " <> " & keyNode.GetAttribute("child") & ")", ValidationErrorMessage.TargetEnum.KeyFields)
                                End If
                            End If

                        Next
                    End If
                End If
            End If

            'validate child nodes
            For Each node As ERANode In Me.Nodes
                node.ValidateNode()
            Next
        Catch ex As Exception
            MessageBox.Show("Error validating table: " & ex.Message, "Error Validating Table", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class

Public Class FieldNode
    Inherits ERANode

    Public Overrides ReadOnly Property ERANodeType() As ERANode.ERANodeTypeEnum
        Get
            Return ERANodeTypeEnum.Field
        End Get
    End Property

    Public Overrides Sub ValidateNode()
        Try
            ClearErrors()
            Dim ParentNode As LayerTableNode = Me.Parent
            Dim ParentTable As ITable = ParentNode.GetObjectClass(False) 'GetObjectClassFromEraNode(ParentNode, frde_parent, False)
            If ParentTable Is Nothing Then
                'error will be reported by ParentNode
                Exit Sub
            End If
            If ParentTable.Fields.FindField(GetAttributeValue("name")) = -1 Then
                AddError("Could not find field", ValidationErrorMessage.TargetEnum.LayerTableFieldID)
                Exit Sub
            End If
            'no children to validate
        Catch ex As Exception
            MessageBox.Show("Error validating field: " & ex.Message, "Error Validating Field", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
Public MustInherit Class ERANode
    Inherits TreeNode

    Private m_XmlNode As Xml.XmlElement
    Public CachedFields As ESRI.ArcGIS.Geodatabase.IFields

    Public MustOverride ReadOnly Property ERANodeType() As ERANodeTypeEnum

    Public MustOverride Sub ValidateNode()

    Public ReadOnly Property HasError() As Boolean
        Get
            Return Me.Errors.Count > 0
        End Get
    End Property

    Public ReadOnly Property ChildHasError() As Boolean
        Get
            For Each ChildNode As ERANode In Me.Nodes
                If ChildNode.HasError Then
                    Return True
                End If
                If ChildNode.ChildHasError Then
                    Return True
                End If
            Next
            Return False
        End Get
    End Property

    Protected ReadOnly Property ParentForm() As frmReportDefinitionEditor
        Get
            Return Me.TreeView.FindForm()
        End Get
    End Property

    'Public Sub New()
    '    MyBase.New()
    'End Sub

    'Public Sub New(ByRef XmlNode As Xml.XmlElement)
    '    MyBase.New()
    '    Me.XMLNode = XmlNode
    'End Sub

    Public Overrides Function Clone() As Object
        Dim CloneNode As ERANode = MyBase.Clone
        CloneNode.XMLNode = m_XmlNode.CloneNode(False)
        CloneChildren(CloneNode)
        Return CloneNode
    End Function

    Private Sub CloneChildren(ByRef ParentNode As ERANode)
        If ParentNode.ERANodeType = ERANodeTypeEnum.Table Then
            'Clone the KEYS node
            Dim KeysNode As XmlElement = Me.XMLNode.SelectSingleNode("KEYS")
            If Not KeysNode Is Nothing Then _
                ParentNode.XMLNode.AppendChild(KeysNode)
        End If
        For Each childNode As ERANode In ParentNode.Nodes
            ParentNode.XMLNode.AppendChild(childNode.XMLNode)
            CloneChildren(childNode)
        Next
    End Sub


    Public Enum ERANodeTypeEnum
        Workspaces
        SDEWorkspace
        ShapeWorkspace
        AccessWorkspace
        Issue
        Layer
        Field
        Table
    End Enum

    Public Property XMLNode() As Xml.XmlElement
        Get
            Return m_XmlNode
        End Get
        Set(ByVal value As Xml.XmlElement)
            m_XmlNode = value
            Update()
        End Set
    End Property

    'sets the text and image key properties.
    'for some reason it doesn't work to just shadow those properties, as straightforward
    'as that seems. The items appear with empty text and image key values
    Public Sub Update()
        Select Case Me.ERANodeType
            Case ERANodeTypeEnum.AccessWorkspace
                Me.Text = GetMyName() & " (Access Geodatabase)"
                'Case ERANodeTypeEnum.FileGeodatabaseWorkspace
                '    Me.Text = GetMyName() & " (File Geodatabase)"
            Case ERANodeTypeEnum.Field
                Me.Text = GetMyName() & " (Field)"
            Case ERANodeTypeEnum.Issue
                Me.Text = "Issue: " & GetMyName()
            Case ERANodeTypeEnum.Layer
                'Me.Text = GetMyName() & " (" & Strings.StrConv(GetAttributeValue("analysistype"), VbStrConv.ProperCase) & " Analysis)"
                Me.Text = GetMyName() & " ("
                Select Case GetAttributeValue("analysistype")
                    Case "comparison"
                        Me.Text &= "Comparison Resource"
                    Case "distance"
                        Me.Text &= "Distance"
                    Case "acreage"
                        Me.Text &= "Feature Comparison"
                    Case "buffer"
                        Me.Text &= "Generic Resource"
                    Case "pinpoint"
                        Me.Text &= "Pinpoint"
                    Case "weighted average"
                        Me.Text &= "Weighted Average"
                End Select
                Me.Text &= " Analysis)"
            Case ERANodeTypeEnum.SDEWorkspace
                Me.Text = GetMyName() & " (SDE Connection)"
            Case ERANodeTypeEnum.ShapeWorkspace
                Me.Text = GetMyName() & " (Directory)"
            Case ERANodeTypeEnum.Table
                Me.Text = GetMyName() & " (Table)"
            Case ERANodeTypeEnum.Workspaces
                Me.Text = "Workspaces"
            Case -1
                Me.Text = "Error: Unrecognized node type: " & m_XmlNode.Name
                Me.AddError("Unrecognized node type: " & m_XmlNode.Name, ValidationErrorMessage.TargetEnum.TreeviewOnly)
            Case Else
                Me.Text = MyBase.Text
        End Select

        If Me.ERANodeType = ERANodeTypeEnum.Layer Then
            Me.ImageKey = GetAttributeValue("analysistype")
            Me.SelectedImageKey = GetAttributeValue("analysistype")
        ElseIf Me.ERANodeType = ERANodeTypeEnum.SDEWorkspace Or Me.ERANodeType = ERANodeTypeEnum.ShapeWorkspace _
            Or Me.ERANodeType = ERANodeTypeEnum.AccessWorkspace Or Me.ERANodeType = ERANodeTypeEnum.Workspaces Then
            Me.ImageKey = "workspace"
            Me.SelectedImageKey = "workspace"
        Else
            Me.ImageKey = Me.ERANodeType
            Me.SelectedImageKey = Me.ERANodeType
        End If

    End Sub

    Public Function NodesByType(ByVal NodeType As ERANodeTypeEnum) As Generic.List(Of ERANode)
        Dim r As New Generic.List(Of ERANode)

        For Each ChildNode As ERANode In Me.Nodes
            If ChildNode.ERANodeType = NodeType Then
                r.Add(ChildNode)
            End If
        Next
        Return r
    End Function


    Public Function GetAttributeValue(ByVal AttributeName As String) As String
        Return Me.XMLNode.GetAttribute(AttributeName)
    End Function

    Public Sub SetAttributeValue(ByVal AttributeName As String, ByVal AttributeValue As String)
        Me.XMLNode.SetAttribute(AttributeName, AttributeValue)
        'Dim attrib As XmlAttribute = Me.m_XmlNode.Attributes(AttributeName)
        'If attrib Is Nothing Then
        '    attrib = m_XmlNode.OwnerDocument.CreateAttribute(AttributeName)
        '    attrib.Value = AttributeValue
        '    m_XmlNode.Attributes.Append(attrib)
        'Else
        '    attrib.Value = AttributeValue
        'End If
        Update()
    End Sub

    Public Function GetMyName() As String
        If Me.ERANodeType = ERANodeTypeEnum.Field Then
            Return Me.GetAttributeValue("alias")
        Else
            Return Me.GetAttributeValue("name")
        End If
    End Function

    Public Sub RemoveEraNode()
        If Not m_XmlNode.ParentNode Is Nothing Then
            m_XmlNode.ParentNode.RemoveChild(m_XmlNode)
        End If
        Me.Remove()
    End Sub

    Public ReadOnly Property ErrorText() As String
        Get
            If m_Errors.Count = 0 Then Return ""
            Dim al As New ArrayList
            Dim sb As New System.Text.StringBuilder
            For Each e As ValidationErrorMessage In m_Errors
                If sb.Length > 0 Then sb.AppendLine()
                sb.Append(e.ErrorMessage)
            Next

            Return sb.ToString ' throws an error: Join(m_Errors.ToArray, vbNewLine)

        End Get
    End Property

    Private m_Errors As New Generic.List(Of ValidationErrorMessage)

    Public ReadOnly Property Errors() As Generic.List(Of ValidationErrorMessage)
        Get
            Return m_Errors
        End Get
    End Property


    Public Class ValidationErrorMessage
        Private m_Message As String
        Private m_ValidationErrorTarget As TargetEnum

        Public Sub New(ByVal Message As String, ByVal ErrorTarget As ValidationErrorMessage.TargetEnum)
            m_Message = Message
            m_ValidationErrorTarget = ErrorTarget
        End Sub

        Public ReadOnly Property ErrorMessage() As String
            Get
                Return m_Message
            End Get
        End Property

        Public ReadOnly Property ValidationErrorTarget() As TargetEnum
            Get
                Return m_ValidationErrorTarget
            End Get
        End Property

        Public Enum TargetEnum
            TreeviewOnly 'Error is displayed only on the node in the treeview
            DisplayName 'Applies to alias property of a field, or name property of all other types (shown as the display name for layer, table and fields)
            AnalysisType 'Layer only
            LayerTableFieldID 'The Layer ID property of a layer or table, or the field name property
            Workspace 'Layer or table only
            WhereExpression 'Layer or table only
            SortFields 'Layer or table only
            KeyFields 'Table
            SummarizationFunction 'Field
            UseForURL 'field
            URLPrefix 'field
            URLSuffix 'field
            Database 'AccessWorkspace or ShapefileWorkspace, path to file or directory, SDE Database property
            SDEServer 'SDE Server
            SDEInstance 'SDE Instance
            UserNamePassword 'SDE Username or password
        End Enum

        Public Overrides Function ToString() As String
            Return m_Message
        End Function
    End Class

    Public Sub AddError(ByVal ErrorMessage As String, ByVal ErrorTarget As ValidationErrorMessage.TargetEnum)
        If ErrorMessage <> "" Then
            m_Errors.Add(New ValidationErrorMessage(ErrorMessage, ErrorTarget))
            Me.ForeColor = Color.Red
        End If
    End Sub

    Public Sub ClearErrors()
        m_Errors.Clear()
        Me.ForeColor = Color.Black
    End Sub

    Public Sub MoveUp()
        If Me.Index > 0 Then
            Dim I As Integer = Me.Index - 1
            Dim pNode As TreeNode = Me.Parent
            If pNode Is Nothing Then
                Dim tv As TreeView = Me.TreeView
                Remove()
                tv.Nodes.Insert(I, Me)
            Else
                Remove()
                pNode.Nodes.Insert(I, Me)
            End If
            m_XmlNode.ParentNode.InsertBefore(m_XmlNode, m_XmlNode.PreviousSibling)
        End If
    End Sub

    Public Sub MoveDown()
        Dim pNode As TreeNode = Me.Parent
        If pNode Is Nothing Then
            Dim tv As TreeView = Me.TreeView
            If Me.Index < tv.Nodes.Count - 1 Then
                Dim I As Integer = Me.Index + 1
                Remove()
                tv.Nodes.Insert(I, Me)
            End If
        Else
            If Me.Index < pNode.Nodes.Count - 1 Then
                Dim I As Integer = Me.Index + 1
                Remove()
                pNode.Nodes.Insert(I, Me)
            End If
        End If
        m_XmlNode.ParentNode.InsertAfter(m_XmlNode, m_XmlNode.NextSibling)
    End Sub

    Public Sub MoveToTop()
        Dim pNode As TreeNode = Me.Parent
        If pNode Is Nothing Then
            Dim tv As TreeView = Me.TreeView
            Remove()
            tv.Nodes.Insert(1, Me)
        Else
            Remove()
            pNode.Nodes.Insert(0, Me)
        End If
        m_XmlNode.ParentNode.InsertBefore(m_XmlNode, m_XmlNode.ParentNode.FirstChild)
    End Sub

    Public Sub MoveToBottom()
        Dim pNode As TreeNode = Me.Parent
        If pNode Is Nothing Then
            Dim tv As TreeView = Me.TreeView
            Remove()
            tv.Nodes.Add(Me)
        Else
            Remove()
            pNode.Nodes.Add(Me)
        End If
        m_XmlNode.ParentNode.InsertAfter(m_XmlNode, m_XmlNode.ParentNode.LastChild)
    End Sub

    Public ReadOnly Property isTop() As Boolean
        Get
            If Me.Parent Is Nothing Then
                Return Me.Index = 1
            Else
                Return Me.Index = 0
            End If
        End Get
    End Property

    Public ReadOnly Property isBottom() As Boolean
        Get
            If Me.Parent Is Nothing Then
                Return Me.Index = Me.TreeView.Nodes.Count - 1
            Else
                Return Me.Index = Me.Parent.Nodes.Count - 1
            End If
        End Get
    End Property

    Public ReadOnly Property isSummarization() As Boolean
        Get
            Dim myParent As ERANode = Nothing
            If Me.Parent IsNot Nothing AndAlso TypeOf Me.Parent Is ERANode Then
                myParent = Me.Parent
            End If
            Select Case Me.ERANodeType
                Case ERANodeTypeEnum.Field
                    If myParent IsNot Nothing Then
                        Return myParent.isSummarization
                    Else
                        Return False
                    End If
                Case ERANodeTypeEnum.Table
                    If Not Me.GetAttributeValue("inline").Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
                        Return Me.GetAttributeValue("summarization").Equals("true", StringComparison.CurrentCultureIgnoreCase)
                    ElseIf myParent IsNot Nothing Then
                        Return myParent.isSummarization
                    Else
                        Return False
                    End If
                Case ERANodeTypeEnum.Layer
                    Return Me.GetAttributeValue("summarization").Equals("true", StringComparison.CurrentCultureIgnoreCase)
                Case Else
                    Return False
            End Select
        End Get
    End Property

    Public Sub ChangedTableName(ByVal oldName As String, ByVal newName As String)
        If Me.ERANodeType = ERANodeTypeEnum.Table Then
            For Each SortNode As Xml.XmlElement In XMLNode.SelectNodes("SORT_FIELDS/SORT_FIELD")
                Dim strFieldName As String = SortNode.GetAttribute("field")
                If strFieldName.StartsWith(oldName & "_") Then
                    SortNode.SetAttribute("field", strFieldName.Replace(oldName, newName))
                End If
            Next
            If Me.Parent IsNot Nothing AndAlso TypeOf Me.Parent Is ERANode AndAlso Me.GetAttributeValue("inline").ToLower.Equals("true") Then
                CType(Me.Parent, ERANode).ChangedTableName(oldName, newName)
            End If
        ElseIf Me.ERANodeType = ERANodeTypeEnum.Layer Then
            For Each SortNode As Xml.XmlElement In XMLNode.SelectNodes("SORT_FIELDS/SORT_FIELD")
                Dim strFieldName As String = SortNode.GetAttribute("field")
                If strFieldName.StartsWith(oldName & "_") Then
                    SortNode.SetAttribute("field", strFieldName.Replace(oldName, newName))
                End If
            Next
        End If
    End Sub

    Public Sub ChangedFieldName(ByVal oldName As String, ByVal newName As String)
        If Me.ERANodeType = ERANodeTypeEnum.Field Then
            Me.SetAttributeValue("name", newName)
            If Me.Parent IsNot Nothing AndAlso TypeOf Me.Parent Is ERANode Then
                CType(Me.Parent, ERANode).ChangedFieldName(oldName, newName)
            End If
        ElseIf Me.ERANodeType = ERANodeTypeEnum.Table Then
            For Each SortNode As Xml.XmlElement In XMLNode.SelectNodes("SORT_FIELDS/SORT_FIELD")
                Dim strFieldName As String = SortNode.GetAttribute("field")
                If strFieldName.EndsWith("_" & oldName) Then
                    SortNode.SetAttribute("field", strFieldName.Replace(oldName, newName))
                End If
            Next
            If Me.Parent IsNot Nothing AndAlso TypeOf Me.Parent Is ERANode AndAlso Me.GetAttributeValue("inline").ToLower.Equals("true") Then
                CType(Me.Parent, ERANode).ChangedFieldName(oldName, newName)
            End If
        ElseIf Me.ERANodeType = ERANodeTypeEnum.Layer Then
            For Each SortNode As Xml.XmlElement In XMLNode.SelectNodes("SORT_FIELDS/SORT_FIELD")
                Dim strFieldName As String = SortNode.GetAttribute("field")
                If strFieldName.EndsWith("_" & oldName) Then
                    SortNode.SetAttribute("field", strFieldName.Replace(oldName, newName))
                End If
            Next
        End If
    End Sub
End Class
