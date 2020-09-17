Imports ESRI.ArcGIS.Geodatabase
Imports System.Runtime.InteropServices

<System.Runtime.InteropServices.ComVisible(False)> _
Public Class EraTable
    Inherits EraLayerTable

    Private m_ParentLayerTable As EraLayerTable

    Friend ReadOnly Property ParentLayerTable() As EraLayerTable
        Get
            Return m_ParentLayerTable
        End Get
    End Property

    Public ReadOnly Property isInlineTable() As Boolean
        Get
            '4/1/11: Changed the following to force any table that has no Fields selected for output to 
            'be inline. This way it doesn't display the table name with nothing underneath it.
            Return GetAttributeValue("inline", False).Equals("true", StringComparison.CurrentCultureIgnoreCase) _
                OrElse m_ColFields.Count < 1
        End Get
    End Property

    Public ReadOnly Property isInnerJoin() As Boolean
        Get
            Return GetAttributeValue("innerjoin", False).Equals("true", StringComparison.CurrentCultureIgnoreCase)
        End Get
    End Property

    Public ReadOnly Property ShouldSpatialJoin() As Boolean
        Get
            Return GetAttributeValue("join", False).Equals("Spatial", StringComparison.CurrentCultureIgnoreCase)
        End Get
    End Property

    Public Sub New(ByRef LayerTable As EraLayerTable)
        m_ParentLayerTable = LayerTable
    End Sub

    Public Overrides Sub SetXmlNode(ByRef EraXmlElement As System.Xml.XmlElement)
        Try
            m_XmlElement = EraXmlElement
            m_Name = GetAttributeValue("name", True)
            OpenTablesAndFields()

            'keys collection used to relate the child to the parent
            Dim blnMustHaveKeys As Boolean = True
            If ShouldSpatialJoin Then
                blnMustHaveKeys = False
            ElseIf TypeOf (ParentLayerTable) Is ERAAnalysis Then
                If DirectCast(ParentLayerTable, ERAAnalysis).AnalysisType = "comparison" Then
                    'Keys are not required
                    blnMustHaveKeys = False
                End If
            End If

            m_SortFieldsNode = EraXmlElement.SelectSingleNode("SORT_FIELDS")
            Dim KeysNode As Xml.XmlNode = EraXmlElement.SelectSingleNode("KEYS")
            If KeysNode Is Nothing Then
                If blnMustHaveKeys Then
                    Throw New EraException("Error creating ERATable--No keys specified to link table to parent.")
                End If
            ElseIf KeysNode.SelectSingleNode("KEY") Is Nothing Then
                If blnMustHaveKeys Then
                    Throw New EraException("Error creating ERATable--No keys specified to link table to parent.")
                End If
            Else
                'OK, all is well
                Dim childTable As ITable = Table
                Dim parentTable As ITable = ParentLayerTable.Table
                For Each KeyNode As Xml.XmlElement In KeysNode.SelectNodes("KEY")
                    Dim KeyPair As New KeyPairClass
                    KeyPair.SetXmlNode(KeyNode)
                    If childTable.FindField(KeyPair.ChildKey) = -1 Then
                        Throw New EraException("The key field " & KeyPair.ChildKey & " was not found in the table " & Me.LayerID)
                    End If

                    If parentTable.FindField(KeyPair.ParentKey) = -1 Then
                        Throw New EraException("The key field " & KeyPair.ParentKey & " was not found in the table " & ParentLayerTable.LayerID)
                    End If

                    KeysList.Add(KeyPair)
                Next
            End If
        Catch ex As EraException
            AddError(ex, "ERATable.SetXmlNode")
            Throw
        End Try
    End Sub

#Region "KeysArrayList used to simplify building join statements"
    Friend KeysList As New Generic.List(Of KeyPairClass)

    Friend Class KeyPairClass
        Inherits EraNode

        Public ParentKey As String
        Public ChildKey As String

        Public Overrides Sub SetXmlNode(ByRef EraXmlElement As System.Xml.XmlElement)
            m_XmlElement = EraXmlElement
            Me.ParentKey = GetAttributeValue("parent", True)
            Me.ChildKey = GetAttributeValue("child", True)

        End Sub

        Public Overrides ReadOnly Property TagName() As String
            Get
                Return "KEY"
            End Get
        End Property
    End Class
#End Region

    Public Overloads Sub WriteXmlTableResults(ByRef ParentRecordNode As Xml.XmlElement, ByRef ParentRow As IRow)
        'Dim TableNode As Xml.XmlElement = ParentRecordNode.OwnerDocument.CreateElement("TABLE")
        Dim pCursor As ICursor
        Try
            If ShouldSpatialJoin Then
                If Not TypeOf ParentRow Is IFeature Then
                    Throw New EraException("Table Name: " & Me.Name & " - The Parent Row could not be converted to an IFeature!")
                End If
                Dim pGeometry As ESRI.ArcGIS.Geometry.IGeometry = DirectCast(ParentRow, IFeature).ShapeCopy
                pCursor = OpenFeatureCursor(pGeometry)
            Else
                Dim nField As Integer
                Dim JoinStatements As New ArrayList
                Dim pQueryFilt As IQueryFilter = New QueryFilter
                For Each KeyPair As EraTable.KeyPairClass In KeysList
                    nField = ParentRow.Fields.FindField(KeyPair.ParentKey)
                    If ParentRow.Value(nField) Is Nothing OrElse ParentRow.Value(nField) Is DBNull.Value Then
                        JoinStatements.Add(KeyPair.ChildKey & " IS NULL")
                    Else
                        Select Case ParentRow.Fields.Field(nField).VarType
                            Case VariantType.String
                                JoinStatements.Add(KeyPair.ChildKey & " = '" & ParentRow.Value(nField).ToString.Replace("'", "''") & "'")
                            Case VariantType.Date
                                Select Case Me.WorkspaceType
                                    Case "ACCESSWORKSPACE"
                                        JoinStatements.Add(KeyPair.ChildKey & " = #" & ParentRow.Value(nField) & "#")
                                    Case "SDEWORKSPACE"
                                        Dim ParentRowDate As Date = ParentRow.Value(nField)
                                        Dim strParentRowDate As String = Format(ParentRowDate, "MM/dd/yyyy HH:mm:ss")
                                        JoinStatements.Add(KeyPair.ChildKey & " = to_date('" & strParentRowDate & "','MM/DD/YYYY HH24:MI:SS')")
                                    Case "SHAPEWORKSPACE"
                                        Dim ParentRowDate As Date = ParentRow.Value(nField)
                                        Dim strParentRowDate As String = Format(ParentRowDate, "yyyy-MM-dd")
                                        JoinStatements.Add(KeyPair.ChildKey & " = date '" & strParentRowDate & "'")
                                End Select
                            Case Else
                                JoinStatements.Add(KeyPair.ChildKey & " = " & ParentRow.Value(nField))
                        End Select
                    End If
                Next

                If Me.WhereExpression <> "" Then
                    JoinStatements.Add(Me.WhereExpression)
                End If
                Dim strSQL As String = "(" & Join(JoinStatements.ToArray, " AND ") & ")"
                pQueryFilt.WhereClause = strSQL

                Logging.Log.Debug("Filtering " & Me.Name & " using SQL: " & strSQL)
                Try
                    Dim iRows As Long
                    If GetDataSetType() = esriDatasetType.esriDTFeatureClass Then
                        iRows = FeatureClass.FeatureCount(pQueryFilt)
                    Else
                        iRows = Table.RowCount(pQueryFilt)
                    End If

                    Logging.Log.Debug("   Filter should return " & iRows & " rows.")
                Catch ex As Exception
                    Debug.Print(ex.Message)
                End Try


                If GetDataSetType() = esriDatasetType.esriDTFeatureClass Then
                    pCursor = FeatureClass.Search(pQueryFilt, True)
                Else
                    pCursor = Table.Search(pQueryFilt, True)
                End If
            End If
            If Me.isSummarization Then
                Dim SumProcess As New SummarizationProcess(Me)
                SumProcess.ProcessSummarization(pCursor, ParentRecordNode)
            Else
                WriteXmlTableResults(ParentRecordNode, pCursor)
            End If
            Logging.Log.Debug("Releasing cursor for " & Me.Name)
            Marshal.ReleaseComObject(pCursor)
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Overloads Sub WriteXmlTableResults(ByRef ParentRecordNode As Xml.XmlElement, ByRef ParentRow As DataRow)
        Try
            Logging.Log.Debug("Writing results for subquery table " & Me.LayerID)
            Dim esriTable As ITable = Me.Table
            Dim strFieldName As String
            Dim nField As Integer
            Dim JoinStatements As New ArrayList
            Dim pQueryFilt As IQueryFilter = New QueryFilter

            For Each KeyPair As EraTable.KeyPairClass In Me.KeysList
                nField = KeyPair.ParentKey.LastIndexOf(".")
                If nField < 0 Then
                    strFieldName = Me.ParentLayerTable.Name & "_" & KeyPair.ParentKey
                Else
                    strFieldName = Me.ParentLayerTable.Name & "_" & KeyPair.ParentKey.Substring(nField)
                End If
                If Not ParentRow.Table.Columns.Contains(strFieldName) Then
                    'Name might be encoded (TODO: is it always?)
                    strFieldName = Xml.XmlConvert.EncodeLocalName(strFieldName)
                    If Not ParentRow.Table.Columns.Contains(strFieldName) Then
                        Throw New EraException("The field '" & strFieldName & "' was not found in the parent table.")
                    End If
                End If
                If ParentRow.IsNull(strFieldName) Then
                    JoinStatements.Add(KeyPair.ChildKey & " IS NULL")
                Else
                    nField = esriTable.FindField(KeyPair.ChildKey)
                    Select Case esriTable.Fields.Field(nField).VarType
                        Case VariantType.String
                            JoinStatements.Add(KeyPair.ChildKey & " = '" & ParentRow(strFieldName).ToString.Replace("'", "''") & "'")
                        Case VariantType.Date
                            Select Case Me.WorkspaceType
                                Case "ACCESSWORKSPACE"
                                    JoinStatements.Add(KeyPair.ChildKey & " = #" & ParentRow(strFieldName) & "#")
                                Case "SDEWORKSPACE"
                                    Dim ParentRowDate As Date = ParentRow(strFieldName)
                                    Dim strParentRowDate As String = Format(ParentRowDate, "MM/dd/yyyy HH:mm:ss")
                                    JoinStatements.Add(KeyPair.ChildKey & " = to_date('" & strParentRowDate & "','MM/DD/YYYY HH24:MI:SS')")
                                Case "SHAPEWORKSPACE"
                                    Dim ParentRowDate As Date = ParentRow(strFieldName)
                                    Dim strParentRowDate As String = Format(ParentRowDate, "yyyy-MM-dd")
                                    JoinStatements.Add(KeyPair.ChildKey & " = date '" & strParentRowDate & "'")
                            End Select
                        Case Else
                            JoinStatements.Add(KeyPair.ChildKey & " = " & ParentRow(strFieldName))
                    End Select
                End If
            Next

            If Me.WhereExpression <> "" Then
                JoinStatements.Add(Me.WhereExpression)
            End If

            Dim strSQL As String = Join(JoinStatements.ToArray, " AND ")
            pQueryFilt.WhereClause = strSQL

            Logging.Log.Debug("Filtering " & Me.Name & " using SQL: " & strSQL)
            Try
                Dim iRows As Long
                If GetDataSetType() = esriDatasetType.esriDTFeatureClass Then
                    iRows = FeatureClass.FeatureCount(pQueryFilt)
                Else
                    iRows = Table.RowCount(pQueryFilt)
                End If

                Logging.Log.Debug("   Filter should return " & iRows & " rows.")
            Catch ex As Exception
                Debug.Print(ex.Message)
            End Try

            Dim pCursor As ICursor
            If GetDataSetType() = esriDatasetType.esriDTFeatureClass Then
                pCursor = FeatureClass.Search(pQueryFilt, True)
            Else
                pCursor = Table.Search(pQueryFilt, True)
            End If
            'Check to see if this table is a Summarization Table
            If Me.isSummarization Then
                Dim SumProcess As New SummarizationProcess(Me)
                SumProcess.ProcessSummarization(pCursor, ParentRecordNode)
            Else
                WriteXmlTableResults(ParentRecordNode, pCursor)
            End If
            Logging.Log.Debug("Releasing cursor for " & Me.Name)
            Marshal.ReleaseComObject(pCursor)
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Overloads Sub WriteXmlTableResults(ByRef ParentRecordNode As Xml.XmlElement, ByRef pCursor As ICursor)
        Try
            Me.m_OutputXmlDoc = ParentRecordNode.OwnerDocument
            Dim ThisRow As IRow
            Dim ValuesNode As Xml.XmlElement
            If isInlineTable Then
                ValuesNode = ParentRecordNode.SelectSingleNode("VALUES")
                ThisRow = pCursor.NextRow
                If ThisRow Is Nothing Then
                    Dim valueNode As Xml.XmlElement
                    For Each FieldNode As EraField In m_ColFields
                        valueNode = ValuesNode.SelectSingleNode(FieldNode.ElementName)
                        If valueNode Is Nothing Then
                            valueNode = Me.m_OutputXmlDoc.CreateElement(FieldNode.ElementName)
                            ValuesNode.AppendChild(valueNode)
                        Else
                            valueNode.InnerText = ""
                        End If
                    Next
                    For Each ChildTable As EraTable In m_ColTables
                        If ChildTable.isInlineTable Then
                            ChildTable.WriteBlankValueNodes(ValuesNode)
                        End If
                    Next
                Else
                    Dim vNode As Xml.XmlElement
                    For Each fieldNode As EraField In Me.m_ColFields
                        vNode = ValuesNode.SelectSingleNode(fieldNode.ElementName)
                        If vNode IsNot Nothing Then
                            fieldNode.ReplaceXmlRecordResults(vNode, ThisRow)
                        Else
                            fieldNode.XmlRecordResults(ValuesNode, ThisRow)
                        End If
                    Next

                    For Each ChildTable As EraTable In m_ColTables
                        ChildTable.WriteXmlTableResults(ParentRecordNode, ThisRow)
                    Next

                    ThisRow = pCursor.NextRow
                    While ThisRow IsNot Nothing
                        If HasInnerJoins Then
                            For Each childtable As EraTable In m_ColTables
                                If Not childtable.HasMatchingRecords(ThisRow) Then
                                    ThisRow = pCursor.NextRow
                                    Continue While
                                End If
                            Next
                        End If
                        Dim NextRecordNode As Xml.XmlElement = ParentRecordNode.CloneNode(True)

                        For Each tableNode As Xml.XmlElement In NextRecordNode.SelectNodes("TABLE")
                            NextRecordNode.RemoveChild(tableNode)
                        Next
                        ParentRecordNode.ParentNode.AppendChild(NextRecordNode)
                        ValuesNode = NextRecordNode.SelectSingleNode("VALUES")
                        For Each fieldNode As EraField In Me.m_ColFields
                            vNode = ValuesNode.SelectSingleNode(fieldNode.ElementName)
                            If vNode IsNot Nothing Then
                                fieldNode.ReplaceXmlRecordResults(vNode, ThisRow)
                            Else
                                fieldNode.XmlRecordResults(ValuesNode, ThisRow)
                            End If
                        Next

                        For Each ChildTable As EraTable In m_ColTables
                            ChildTable.WriteXmlTableResults(NextRecordNode, ThisRow)
                        Next
                        'Marshal.ReleaseComObject(ThisRow)

                        ThisRow = pCursor.NextRow
                    End While
                End If
            Else 'Is not Inline Table
                ThisRow = pCursor.NextRow
                If ThisRow Is Nothing Then
                    'Note: if we decide to make this return an empty table, move the block that adds the table header data
                    'and remove the Exit Sub that follows in this block, nest the fields/records section in
                    'an else block under this If block
                    Logging.Log.Info("No matching records found in " & Me.Name)
                Else
                    Me.m_OutputXmlElement = m_OutputXmlDoc.CreateElement(TagName)
                    ParentRecordNode.AppendChild(m_OutputXmlElement)
                    'Table Name
                    m_OutputXmlElement.SetAttribute("name", Me.Name)

                    'FIELDS node and individual FIELD nodes
                    WriteFieldsNode()

                    Dim intRow As Integer = 1
                    While ThisRow IsNot Nothing
                        If HasInnerJoins Then
                            For Each childtable As EraTable In m_ColTables
                                If Not childtable.HasMatchingRecords(ThisRow) Then
                                    ThisRow = pCursor.NextRow
                                    Continue While
                                End If
                            Next
                        End If
                        Dim RecordNode As Xml.XmlElement = Me.m_OutputXmlDoc.CreateElement("RECORD")
                        RecordNode.SetAttribute("rec", intRow)
                        ValuesNode = Me.m_OutputXmlDoc.CreateElement("VALUES")
                        For Each Field As EraField In m_ColFields
                            Field.XmlRecordResults(ValuesNode, ThisRow)
                        Next Field
                        RecordNode.AppendChild(ValuesNode)
                        m_OutputXmlElement.AppendChild(RecordNode)

                        'recursive call on this function to get next nested table, if any
                        'this is where we would recursively call  to get child table results
                        For Each ChildTable As EraTable In m_ColTables
                            ChildTable.WriteXmlTableResults(RecordNode, ThisRow)
                        Next

                        intRow += 1
                        'Marshal.ReleaseComObject(ThisRow)

                        ThisRow = pCursor.NextRow
                    End While

                    'Sort the Records
                    If m_SortFieldsNode IsNot Nothing AndAlso m_SortFieldsNode.ChildNodes.Count > 0 Then
                        Dim sortList As New Generic.List(Of Xml.XmlElement)
                        For Each recordNode As Xml.XmlElement In m_OutputXmlElement.SelectNodes("RECORD")
                            sortList.Add(recordNode)
                        Next
                        For Each recordNode As Xml.XmlElement In sortList
                            m_OutputXmlElement.RemoveChild(recordNode)
                        Next
                        sortList.Sort(New RecordComparer(m_SortFieldsNode))
                        For Each recordNode As Xml.XmlElement In sortList
                            m_OutputXmlElement.AppendChild(recordNode)
                        Next
                    End If
                End If
            End If
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Overrides ReadOnly Property TagName() As String
        Get
            Return "TABLE"
        End Get
    End Property

    Public Overrides ReadOnly Property isShowHeaders() As Boolean
        Get
            If isInlineTable Then
                Return Me.ParentLayerTable.isShowHeaders
            Else
                Return Not GetAttributeValue("showheaders", False).Equals("False", StringComparison.CurrentCultureIgnoreCase)
            End If
        End Get
    End Property

    Friend Sub WriteBlankValueNodes(ByRef ValuesNode As Xml.XmlElement)
        Try
            Dim valueNode As Xml.XmlElement
            If Me.m_OutputXmlDoc Is Nothing Then
                'I have no idea how this happens, but it does
                m_OutputXmlDoc = ValuesNode.OwnerDocument
            End If
            For Each FieldNode As EraField In m_ColFields
                valueNode = ValuesNode.SelectSingleNode(FieldNode.ElementName)
                If valueNode Is Nothing Then
                    valueNode = Me.m_OutputXmlDoc.CreateElement(FieldNode.ElementName)
                    ValuesNode.AppendChild(valueNode)
                Else
                    valueNode.InnerText = ""
                End If
            Next
            For Each ChildTable As EraTable In m_ColTables
                If ChildTable.isInlineTable Then
                    ChildTable.WriteBlankValueNodes(ValuesNode)
                End If
            Next
        Catch ex As Exception
            Throw
        End Try

    End Sub

    Public Function HasMatchingRecords(ByVal ParentRow As IRow) As Boolean
        'If this isn't an inner join, then we don't care if there aren't any child records.
        If Not isInnerJoin Then Return True
        Dim pCursor As ICursor = Nothing
        Dim blnResult As Boolean = False
        Try
            If ShouldSpatialJoin Then
                If Not TypeOf ParentRow Is IFeature Then
                    Throw New EraException("Table Name: " & Me.Name & " - The Parent Row could not be converted to an IFeature!")
                End If
                Dim pGeometry As ESRI.ArcGIS.Geometry.IGeometry = DirectCast(ParentRow, IFeature).ShapeCopy
                pCursor = OpenFeatureCursor(pGeometry)
            Else
                Dim nField As Integer
                Dim JoinStatements As New ArrayList
                Dim pQueryFilt As IQueryFilter = New QueryFilter
                For Each KeyPair As EraTable.KeyPairClass In KeysList
                    nField = ParentRow.Fields.FindField(KeyPair.ParentKey)
                    If ParentRow.Value(nField) Is Nothing OrElse ParentRow.Value(nField) Is DBNull.Value Then
                        JoinStatements.Add(KeyPair.ChildKey & " IS NULL")
                    Else
                        Select Case ParentRow.Fields.Field(nField).VarType
                            Case VariantType.String
                                JoinStatements.Add(KeyPair.ChildKey & " = '" & ParentRow.Value(nField).ToString.Replace("'", "''") & "'")
                            Case VariantType.Date
                                Select Case Me.WorkspaceType
                                    Case "ACCESSWORKSPACE"
                                        JoinStatements.Add(KeyPair.ChildKey & " = #" & ParentRow.Value(nField) & "#")
                                    Case "SDEWORKSPACE"
                                        Dim ParentRowDate As Date = ParentRow.Value(nField)
                                        Dim strParentRowDate As String = Format(ParentRowDate, "MM/dd/yyyy HH:mm:ss")
                                        JoinStatements.Add(KeyPair.ChildKey & " = to_date('" & strParentRowDate & "','MM/DD/YYYY HH24:MI:SS')")
                                    Case "SHAPEWORKSPACE"
                                        Dim ParentRowDate As Date = ParentRow.Value(nField)
                                        Dim strParentRowDate As String = Format(ParentRowDate, "yyyy-MM-dd")
                                        JoinStatements.Add(KeyPair.ChildKey & " = date '" & strParentRowDate & "'")
                                End Select
                            Case Else
                                JoinStatements.Add(KeyPair.ChildKey & " = " & ParentRow.Value(nField))
                        End Select
                    End If
                Next
                If Not String.IsNullOrEmpty(Me.WhereExpression) Then
                    JoinStatements.Add(Me.WhereExpression)
                End If
                pQueryFilt.WhereClause = "(" & Join(JoinStatements.ToArray, " AND ") & ")"
                If GetDataSetType() = esriDatasetType.esriDTFeatureClass Then
                    pCursor = FeatureClass.Search(pQueryFilt, True)
                Else
                    pCursor = Table.Search(pQueryFilt, True)
                End If
            End If
            If pCursor Is Nothing Then
                Return False
            ElseIf Not HasInnerJoins Then
                blnResult = pCursor.NextRow IsNot Nothing
            Else
                Dim pRow As IRow = pCursor.NextRow
                While pRow IsNot Nothing
                    For Each childTable As EraTable In m_ColTables
                        If Not childTable.HasMatchingRecords(pRow) Then
                            pRow = pCursor.NextRow
                            Continue While
                        End If
                    Next
                    'If it makes it to this point, then pRow must have passed through all of the child tables.
                    blnResult = True
                    Exit While
                End While
            End If
        Catch ex As Exception
            If pCursor IsNot Nothing Then Marshal.ReleaseComObject(pCursor)
            AddError(ex, "ERATable.HasMatchingRecords")
            Throw
        End Try
        If pCursor IsNot Nothing Then Marshal.ReleaseComObject(pCursor)
        Return blnResult
    End Function
End Class