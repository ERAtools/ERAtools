Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry

''' <summary>
''' Abstract class from which EraTable and all analysis classes inherit. Inherits from ERAnode class.
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public MustInherit Class EraLayerTable
    Inherits EraNode

    Friend m_ColFields As New Generic.List(Of EraField)
    Friend m_ColTables As New Generic.List(Of EraTable)
    Friend m_SortFieldsNode As Xml.XmlElement
    Friend m_OutputXmlElement As Xml.XmlElement
    Friend m_OutputXmlDoc As Xml.XmlDocument

    ''' <summary>
    ''' Gets a value indicating whether this instance is to display column headers in output.
    ''' </summary>
    ''' <value>
    ''' <c>true</c> if this instance to display headers; otherwise, <c>false</c>.
    ''' </value>
    Public Overridable ReadOnly Property isShowHeaders() As Boolean
        Get
            Return Not GetAttributeValue("showheaders", False).Equals("False", StringComparison.CurrentCultureIgnoreCase)
        End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether this instance is to include data summarization.
    ''' </summary>
    ''' <value>
    ''' <c>true</c> if this instance is to include data summarization; otherwise, <c>false</c>.
    ''' </value>
    Public ReadOnly Property isSummarization() As Boolean
        Get
            Return GetAttributeValue("summarization", False).Equals("True", StringComparison.CurrentCultureIgnoreCase)
        End Get
    End Property

    ''' <summary>
    ''' Gets the where expression used to filter data.
    ''' </summary>
    ''' <value>The where expression used to filter data.</value>
    Public ReadOnly Property WhereExpression() As String
        Get
            Return GetAttributeValue("whereexpression", False)
        End Get
    End Property

    ''' <summary>
    ''' Gets the layer ID (name of the layer or table as it exists in it's data repository).
    ''' </summary>
    ''' <value>The layer ID.</value>
    Public ReadOnly Property LayerID() As String
        Get
            Return GetAttributeValue("layerid", True)
        End Get
    End Property

    ''' <summary>
    ''' Gets the workspace ID (name of the workspace as referenced within the WORKSPACES tag in an input .era analysis definition file).
    ''' </summary>
    ''' <value>The workspace ID.</value>
    Public ReadOnly Property WorkspaceID() As String
        Get
            Return GetAttributeValue("workspace", True)
        End Get
    End Property

    ''' <summary>
    ''' Gets the type of the workspace (ACCESSWORKSPACE, SDEWORKSPACE, or SHAPEWORKSPACE).
    ''' </summary>
    ''' <value>The type of the workspace.</value>
    Public ReadOnly Property WorkspaceType() As String
        Get
            Return m_colWorkspaces(WorkspaceID).TagName
        End Get
    End Property

    ''' <summary>
    ''' Gets a pointer to a feature workspace created by passing the WorkspaceID value for an instance of this class to the GetWorkspace function in modWorkspaces.
    ''' </summary>
    ''' <value>A pointer to a feature workspace.</value>
    Public ReadOnly Property Workspace() As IFeatureWorkspace
        Get
            Try
                Return GetWorkspace(WorkspaceID)
            Catch ex As Exception
                Logging.Log.Error("Exception getting the workspace with ID " & WorkspaceID & ". XML of node with invalid workspace: " & m_XmlElement.OuterXml, ex)
                Throw
            End Try
        End Get
    End Property

    Public Function GetDataSetType() As esriDatasetType
        If TypeOf Workspace Is IWorkspace2 Then
            Dim ws As IWorkspace2 = Workspace
            If ws.NameExists(esriDatasetType.esriDTFeatureClass, LayerID) Then
                Return esriDatasetType.esriDTFeatureClass
            ElseIf ws.NameExists(esriDatasetType.esriDTTable, LayerID) Then
                Return esriDatasetType.esriDTTable
            Else
                Return esriDatasetType.esriDTAny
            End If
        Else
            'brute force method for workspaces that don't support iworkspace2 interface
            'and, I might add, I really hate ESRI's ArcObjects programmers right now...
            Try
                If Workspace.OpenFeatureClass(LayerID) IsNot Nothing Then
                    Return esriDatasetType.esriDTFeatureClass
                ElseIf Workspace.OpenTable(LayerID) IsNot Nothing Then
                    Return esriDatasetType.esriDTTable
                Else
                    Return esriDatasetType.esriDTAny
                End If
            Catch ex As Exception
                AddError(ex, "ERALayerTable.GetDataSetType")
                Return esriDatasetType.esriDTAny
            End Try
        End If
    End Function

    Private m_Table As ITable
    ''' <summary>
    ''' Gets a pointer to an Itable instance opened from the workspace using the LayerID property.
    ''' </summary>
    ''' <value>A pointer to an Itable instance.</value>
    Public ReadOnly Property Table() As ITable
        Get
            If m_Table Is Nothing Then
                If m_FeatureClass IsNot Nothing Then
                    m_Table = m_FeatureClass
                    Return m_Table
                End If
                Try
                    m_Table = Workspace.OpenTable(LayerID)
                Catch ex As System.Runtime.InteropServices.COMException
                    Throw New EraException(InterpretExceptionMessage(ex.Message))
                End Try
            End If
            Return m_Table
        End Get
    End Property

    Private m_FeatureClass As IFeatureClass
    ''' <summary>
    ''' Opens a pointer to an IFeatureClass instance opened with the LayerID property from the referenced workspace.
    ''' </summary>
    ''' <value>A pointer to the layer's feature class.</value>
    Public ReadOnly Property FeatureClass() As IFeatureClass
        Get
            If m_FeatureClass Is Nothing Then
                Try
                    m_FeatureClass = Workspace.OpenFeatureClass(Me.LayerID)
                Catch ex As System.Runtime.InteropServices.COMException
                    Throw New EraException(InterpretExceptionMessage(ex.Message))
                End Try
            End If
            Return m_FeatureClass
        End Get
    End Property

    ''' <summary>
    ''' Opens an instance of a pointer to an IFeatureCursor. Used by most analyses to open a feature cursor of intersecting features. 
    ''' </summary>
    ''' <param name="SpatialFilterGeometry">The spatial filter geometry.</param>
    ''' <returns>A pointer to an IFeatureCursor instance.</returns>
    Public Function OpenFeatureCursor(ByRef SpatialFilterGeometry As IGeometry) As IFeatureCursor
        Dim strDatasetName As String = "unknown"
        Try
            Dim pSpatialFilter As ISpatialFilter = New ESRI.ArcGIS.Geodatabase.SpatialFilter
            pSpatialFilter.Geometry = SpatialFilterGeometry
            pSpatialFilter.SpatialRel = ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelIntersects
            strDatasetName = DirectCast(FeatureClass, IDataset).Name
            pSpatialFilter.WhereClause = WhereExpression
            Return FeatureClass.Search(pSpatialFilter, True)
        Catch ex As Exception
            'This block is an attempt to interpret the varying and often misleading, or at least non-informative error messages returned when the definition query is not valid
            Dim strMessage As String = ex.Message
            Dim TestMessages(7) As String
            TestMessages(0) = "The owner SID on a per-user subscription doesn't exist (Exception from HRESULT: 0x80040207)" 'this is apparently what results from a malfomed Where Clause hitting a shapefile, one with incorrect syntax
            TestMessages(1) = "Exception from HRESULT: 0x80040653" 'not a malformed definition query (i.e., it has the basic syntax correct), but still isn't quite right
            TestMessages(2) = "Invalid SQL syntax" 'What SDE returns if it doesn't find an operator
            TestMessages(3) = "Attribute column not found" 'What SDE returns if a field is referenced that doesn't exist in the dataset
            TestMessages(4) = "Syntax error in string in query expression" 'Personal geodatabase, basic syntax error
            TestMessages(5) = "Too few parameters. Expected" 'Personal geodatabase, referencing field that doesn't exist.
            TestMessages(6) = "Data type mismatch in criteria expression." 'Personal geodatabase, as it says
            For i As Integer = 0 To 6
                'If strMessage.Length >= TestMessages(i).Length AndAlso strMessage.Substring(0, TestMessages(i).Length) = TestMessages(i) Then
                'Isn't this a better way of testing this?
                If strMessage.StartsWith(TestMessages(i)) Then
                    strMessage = "The definition query (or 'Where expression') """ & WhereExpression & """ set for the feature class """ & strDatasetName & """ is invalid."
                    Exit For
                End If
            Next
            'if a match to one of the "TestMessages" is not found, then the "raw" error message will be logged
            Logging.Log.Error("Error opening feature cursor: " & strMessage & vbNewLine & "WhereClause: " & Me.WhereExpression & vbNewLine & "XML: " & Me.m_XmlElement.OuterXml, ex)
            AddError(ex, "ERAAnalysis.OpenFeatureCursor")
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Writes the FIELDS element to the output, and also returns a reference to the element (for use in certain analysis classes that add autogenerated fields, like Distance), and adds FIELD nodes to the newly created FIELDS nodes. May also be passed an existing FIELDS node by reference, in which case only the FIELD nodes will be added to the existing FIELDS node.
    ''' </summary>
    ''' <param name="ExistingFieldsNode">A reference to the existing FIELDS node, if already created.</param>
    Friend Overridable Sub WriteFieldsNode(Optional ByRef ExistingFieldsNode As Xml.XmlElement = Nothing)
        Try
            Dim FieldsNode As Xml.XmlElement
            If ExistingFieldsNode Is Nothing Then
                FieldsNode = Me.m_OutputXmlDoc.CreateElement("FIELDS")
                Me.m_OutputXmlElement.AppendChild(FieldsNode)
                Dim attrib As Xml.XmlAttribute = Me.m_OutputXmlDoc.CreateAttribute("showheaders")
                attrib.Value = IIf(Me.isShowHeaders, "true", "false")
                FieldsNode.Attributes.SetNamedItem(attrib)
            Else
                FieldsNode = ExistingFieldsNode
            End If
            For Each field As EraField In m_ColFields
                If Not field.Summary.Equals("Link", StringComparison.CurrentCultureIgnoreCase) Then
                    field.XmlFieldResults(FieldsNode)
                End If
            Next field
            For Each ChildTable As EraTable In m_ColTables
                If ChildTable.isInlineTable Then
                    ChildTable.WriteFieldsNode(FieldsNode)
                End If
            Next
        Catch
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Writes the "no records" node.
    ''' </summary>
    ''' <param name="NoRecordMessage">The message to display in the output report to indicate that no records were found. Defaults to "No Records Found".</param>
    Friend Sub WriteNoRecordsNode(Optional ByVal NoRecordMessage As String = "No Records Found")
        Dim FieldsNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("FIELDS")
        m_OutputXmlElement.AppendChild(FieldsNode)
        Dim RecordNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("RECORD")
        Dim ValuesNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("VALUES")
        Dim FieldNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("EMPTY")
        RecordNode.SetAttribute("rec", 1)
        FieldNode.InnerText = NoRecordMessage
        ValuesNode.AppendChild(FieldNode)
        RecordNode.AppendChild(ValuesNode)
        m_OutputXmlElement.AppendChild(RecordNode)
    End Sub

    ''' <summary>
    ''' Reads the TABLE and FIELD nodes and creates new ERATable and ERAField child nodes.
    ''' </summary>
    Protected Sub OpenTablesAndFields()
        For Each TableNode As Xml.XmlElement In m_XmlElement.SelectNodes("TABLE")
            Logging.Log.Debug("Adding New Table from " & TableNode.OuterXml)
            Dim ChildTable As New EraTable(Me)
            ChildTable.SetXmlNode(TableNode)
            'ChildNodes.Add(ChildTable)
            m_ColTables.Add(ChildTable)
            If ChildTable.isInnerJoin Then
                m_HasInnerJoins = True
            End If
        Next

        For Each FieldNode As Xml.XmlElement In m_XmlElement.SelectNodes("FIELD")
            Dim NewField As New EraField(Me)
            NewField.SetXmlNode(FieldNode)
            m_ColFields.Add(NewField)
        Next
    End Sub

    ''' <summary>
    ''' Interprets the exception message. Used by both LAYER and TABLE tags to interpret ESRI's often cryptic error messages due to an error opening a table or layer
    ''' </summary>
    ''' <param name="ErrorMessage">The ESRI-generated exception message.</param>
    ''' <returns>An interpreted error message.</returns>
    Protected Function InterpretExceptionMessage(ByVal ErrorMessage As String) As String
        If ErrorMessage = "Exception from HRESULT: 0x80040351" Then
            ErrorMessage = Me.TagName.ToLower & " not found in workspace."
        ElseIf ErrorMessage = "Error HRESULT E_FAIL has been returned from a call to a COM component." Then
            'file is locked, or not really a shapefile or dbf
            ErrorMessage = Me.TagName.ToLower & " could not be opened from workspace. The file may be locked by another user or an invalid data set."
            'Access/SDE messages can be fairly specific, but just for consistency's sake:
        ElseIf ErrorMessage.Length >= 20 AndAlso ErrorMessage.Substring(0, 20) = "DBMS table not found" Then
            ErrorMessage = Me.TagName.ToLower & " not found in workspace."
        ElseIf ErrorMessage.Length >= 70 AndAlso ErrorMessage.Substring(0, 70) = "The Microsoft Jet database engine cannot find the input table or query" Then
            ErrorMessage = Me.TagName.ToLower & " not found in workspace."
        End If
        Return "Error opening " & Me.TagName.ToLower & " '" & Me.LayerID & "' in workspace '" & WorkspaceID & "': " & ErrorMessage
    End Function

    Private m_HasInnerJoins As Boolean = False
    Public ReadOnly Property HasInnerJoins() As Boolean
        Get
            Return m_HasInnerJoins
        End Get
    End Property
End Class


