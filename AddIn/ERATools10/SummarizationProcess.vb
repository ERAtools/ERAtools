Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Geodatabase
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

<System.Runtime.InteropServices.ComVisible(False)> _
Public Class SummarizationProcess

    Private m_SummaryFields As New Generic.List(Of EraField)
    Private m_GroupByFields As New Generic.List(Of EraField)
    Private m_AllFields As New Generic.List(Of EraField)
    Private myERALayer As EraLayerTable

    Public Sub New(ByRef eraLayer As EraLayerTable)
        myERALayer = eraLayer
    End Sub

    Public Sub ProcessSummarization(ByRef cursor As ICursor, Optional ByRef ParentRecordNode As Xml.XmlElement = Nothing)
        Try
            If ParentRecordNode IsNot Nothing Then
                myERALayer.m_OutputXmlDoc = ParentRecordNode.OwnerDocument
                myERALayer.m_OutputXmlElement = myERALayer.m_OutputXmlDoc.CreateElement(myERALayer.TagName)
            End If
            Dim dtMerge As New DataTable
            Dim drMerge As DataRow
            FillColumnsFromTables(dtMerge, myERALayer)
            Dim row As IRow = cursor.NextRow
            If row Is Nothing Then
                myERALayer.WriteNoRecordsNode()
                Exit Sub
            End If
            'Get all the data from the selected features and related inline tables
            While row IsNot Nothing
                If myERALayer.HasInnerJoins Then
                    For Each childtable As EraTable In myERALayer.m_ColTables
                        If Not childtable.HasMatchingRecords(row) Then
                            row = cursor.NextRow
                            Continue While
                        End If
                    Next
                End If
                drMerge = dtMerge.NewRow()
                dtMerge.Rows.Add(drMerge)
                For Each field As EraField In myERALayer.m_ColFields
                    If Not field.Summary.Equals("Link", StringComparison.CurrentCultureIgnoreCase) Then
                        drMerge(field.ElementName) = field.GetFieldValue(row) 'row.Value(row.Fields.FindField(field.FieldName))
                    End If
                Next
                For Each ChildTable As EraTable In myERALayer.m_ColTables
                    If ChildTable.isInlineTable Then
                        FillDataFromTables(drMerge, ChildTable, row)
                    End If
                Next
                row = cursor.NextRow
            End While
            'Prepare to run the summarizations
            Dim dtOutput As New DataTable
            FillColumnsFromTables(dtOutput, myERALayer)
            SplitFields(myERALayer)
            Dim dvOutput As New DataView(dtOutput)
            Dim drOutput As DataRow
            'Get all the distinct values
            For Each drMerge In dtMerge.Rows
                dvOutput.RowFilter = GetFilterExpression(drMerge)
                If dvOutput.Count < 1 Then
                    drOutput = dtOutput.NewRow
                    For Each field As EraField In m_GroupByFields
                        drOutput(field.ElementName) = drMerge(field.ElementName)
                    Next
                    dtOutput.Rows.Add(drOutput)
                End If
            Next
            'Run all the summarizations
            For Each drOutput In dtOutput.Rows
                For Each field As EraField In m_SummaryFields
                    drOutput(field.ElementName) = dtMerge.Compute(field.Summary & "(" & field.ElementName & ")", GetFilterExpression(drOutput))
                Next
            Next
            'Finally, Output to XML
            myERALayer.WriteFieldsNode()
            For Each drOutput In dtOutput.Rows
                Dim RecordNode As Xml.XmlElement = myERALayer.m_OutputXmlDoc.CreateElement("RECORD")
                myERALayer.m_OutputXmlElement.AppendChild(RecordNode)
                Dim ValuesNode As Xml.XmlElement = myERALayer.m_OutputXmlDoc.CreateElement("VALUES")
                RecordNode.AppendChild(ValuesNode)
                For Each field As EraField In m_AllFields
                    Dim ValueNode As Xml.XmlElement = myERALayer.m_OutputXmlDoc.CreateElement(field.ElementName)
                    ValuesNode.AppendChild(ValueNode)
                    If field.Link Then
                        ValueNode.SetAttribute("link", "true")
                        Dim url As String
                        If (field.UseForURL) <> "" AndAlso dtOutput.Columns.Contains(field.UseForURL) AndAlso Not drOutput.IsNull(field.UseForURL) Then
                            url = field.Prefix & drOutput(field.UseForURL) & field.Suffix
                        Else
                            url = field.Prefix & drOutput(field.ElementName) & field.Suffix
                        End If
                        ValueNode.SetAttribute("useforurl", url)
                    End If
                    If drOutput.IsNull(field.ElementName) Then
                        ValueNode.InnerText = ""
                    Else
                        ValueNode.InnerText = drOutput(field.ElementName)
                    End If
                Next
                ProcessNestedTables(myERALayer, RecordNode, drOutput)
            Next
            'Sort the Records
            If myERALayer.m_SortFieldsNode IsNot Nothing AndAlso myERALayer.m_SortFieldsNode.ChildNodes.Count > 0 Then
                Dim sortList As New Generic.List(Of Xml.XmlElement)
                For Each recordNode As Xml.XmlElement In myERALayer.m_OutputXmlElement.SelectNodes("RECORD")
                    sortList.Add(recordNode)
                Next
                For Each recordNode As Xml.XmlElement In sortList
                    myERALayer.m_OutputXmlElement.RemoveChild(recordNode)
                Next
                sortList.Sort(New RecordComparer(myERALayer.m_SortFieldsNode))
                For Each recordNode As Xml.XmlElement In sortList
                    myERALayer.m_OutputXmlElement.AppendChild(recordNode)
                Next
            End If
        Catch ex As Exception
            myERALayer.AddError(ex, "SummarizationAProcess.ProcessSummarization")
            Throw
        End Try

        'Added as a fix to Issue #2
        If ParentRecordNode IsNot Nothing Then
            ParentRecordNode.AppendChild(myERALayer.m_OutputXmlElement)
        End If
    End Sub

    Private Sub FillColumnsFromTables(ByRef myDT As DataTable, ByRef node As EraLayerTable)
        Try
            Dim esriTable As ITable = node.Table
            For Each field As EraField In node.m_ColFields
                If field.Summary <> "LINK" Then
                    If field.FieldIndex() >= 0 Then
                        Dim pCVDomain As ICodedValueDomain = field.GetCodedValueDomain
                        If pCVDomain IsNot Nothing Then
                            myDT.Columns.Add(field.ElementName, GetType(String))
                        Else
                            If field.Format = "" Then
                                Select Case esriTable.Fields.Field(esriTable.FindField(field.Name)).Type
                                    Case esriFieldType.esriFieldTypeDate
                                        myDT.Columns.Add(field.ElementName, GetType(DateTime))
                                    Case esriFieldType.esriFieldTypeDouble
                                        myDT.Columns.Add(field.ElementName, GetType(Decimal))
                                    Case esriFieldType.esriFieldTypeSingle
                                        myDT.Columns.Add(field.ElementName, GetType(Decimal))
                                    Case esriFieldType.esriFieldTypeInteger
                                        myDT.Columns.Add(field.ElementName, GetType(Integer))
                                    Case esriFieldType.esriFieldTypeString
                                        myDT.Columns.Add(field.ElementName, GetType(String))
                                    Case Else
                                        'What should happen now?
                                        MessageBox.Show("Couldn't handle field """ & field.Name & """ with type: " & esriTable.Fields.Field(esriTable.FindField(field.Name)).Type.ToString)
                                        Throw New EraException("Couldn't handle field """ & field.Name & """ with type: " & esriTable.Fields.Field(esriTable.FindField(field.Name)).Type.ToString)
                                End Select
                            Else
                                'Formatted to text
                                myDT.Columns.Add(field.ElementName, GetType(String))
                            End If
                        End If
                    Else
                        'let's just assume it's a calculated field
                        'TODO: search for [] to make sure it is a calculated field. Maybe a convenience method?
                        myDT.Columns.Add(field.ElementName, GetType(Decimal))
                    End If
                End If

            Next
            For Each ChildTable As EraTable In node.m_ColTables
                If ChildTable.isInlineTable Then
                    FillColumnsFromTables(myDT, ChildTable)
                End If
            Next
        Catch ex As EraException
            myERALayer.AddError(ex, "SummarizationAnalysis.FillColumnsFromTables")
            Throw
        Catch ex As Exception
            myERALayer.AddError(ex, "SummarizationAnalysis.FillColumnsFromTables")
            Throw
        End Try
    End Sub

    Private Sub FillDataFromTables(ByRef myDR As DataRow, ByRef eraTable As EraTable, Optional ByRef ParentRow As IRow = Nothing)
        Try
            If ParentRow Is Nothing Then
                For Each field As EraField In eraTable.m_ColFields
                    myDR(field.ElementName) = DBNull.Value
                Next
                For Each ChildTable As EraTable In eraTable.m_ColTables
                    If ChildTable.isInlineTable Then
                        FillDataFromTables(myDR, ChildTable)
                    End If
                Next
            End If
            Dim nField As Integer
            Dim JoinStatements As New ArrayList
            Dim pQueryFilt As IQueryFilter = New QueryFilter
            For Each KeyPair As EraTable.KeyPairClass In eraTable.KeysList
                nField = ParentRow.Fields.FindField(KeyPair.ParentKey)
                If ParentRow.Value(nField) Is Nothing OrElse ParentRow.Value(nField) Is DBNull.Value Then
                    JoinStatements.Add(KeyPair.ChildKey & " IS NULL")
                Else
                    Select Case ParentRow.Fields.Field(nField).VarType
                        Case VariantType.String
                            JoinStatements.Add(KeyPair.ChildKey & " = '" & ParentRow.Value(nField).ToString.Replace("'", "''") & "'")
                        Case VariantType.Date
                            Select Case myERALayer.WorkspaceType
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

            If eraTable.WhereExpression <> "" Then
                JoinStatements.Add(eraTable.WhereExpression)
            End If

            Dim strSQL As String = Join(JoinStatements.ToArray, " AND ")
            Logging.Log.Debug("Filtering " & eraTable.Name & " using SQL: " & strSQL)

            pQueryFilt.WhereClause = strSQL
            Dim pCursor As ESRI.ArcGIS.Geodatabase.ICursor = eraTable.Table.Search(pQueryFilt, True)
            Dim ThisRow As IRow = pCursor.NextRow
            While ThisRow IsNot Nothing
                If eraTable.HasInnerJoins Then
                    For Each childtable As EraTable In eraTable.m_ColTables
                        If Not childtable.HasMatchingRecords(ThisRow) Then
                            ThisRow = pCursor.NextRow
                            Continue While
                        End If
                    Next
                End If
                Exit While
            End While
            If ThisRow Is Nothing Then
                For Each field As EraField In eraTable.m_ColFields
                    myDR(field.ElementName) = DBNull.Value
                Next
                For Each ChildTable As EraTable In eraTable.m_ColTables
                    If ChildTable.isInlineTable Then
                        FillDataFromTables(myDR, ChildTable)
                    End If
                Next
            Else
                For Each field As EraField In eraTable.m_ColFields
                    'to prevent argumentexceptions, check value first, as getFieldValue converts everthing to string
                    Dim strTempVal As String = field.GetFieldValue(ThisRow)
                    If strTempVal.Length > 0 Then myDR(field.ElementName) = strTempVal
                Next
                For Each ChildTable As EraTable In eraTable.m_ColTables
                    If ChildTable.isInlineTable Then
                        FillDataFromTables(myDR, ChildTable, ThisRow)
                    End If
                Next
                ThisRow = pCursor.NextRow
                While ThisRow IsNot Nothing
                    If eraTable.HasInnerJoins Then
                        For Each childtable As EraTable In eraTable.m_ColTables
                            If Not childtable.HasMatchingRecords(ThisRow) Then
                                ThisRow = pCursor.NextRow
                                Continue While
                            End If
                        Next
                    End If
                    Dim newDR As DataRow = myDR.Table.NewRow
                    newDR.ItemArray = myDR.ItemArray
                    myDR.Table.Rows.Add(newDR)
                    For Each field As EraField In eraTable.m_ColFields
                        newDR(field.ElementName) = field.GetFieldValue(ThisRow, field.FieldName)
                    Next
                    For Each ChildTable As EraTable In eraTable.m_ColTables
                        If ChildTable.isInlineTable Then
                            FillDataFromTables(newDR, ChildTable, ThisRow)
                        End If
                    Next
                    ThisRow = pCursor.NextRow
                End While
            End If
            Marshal.ReleaseComObject(pCursor)
        Catch ex As Exception
            myERALayer.AddError(ex, "SummarizationAnalysis.FillDataFromTables")
            Throw
        End Try
    End Sub

    Private Sub SplitFields(ByRef node As EraLayerTable)
        For Each field As EraField In node.m_ColFields
            If field.Summary.Equals("GroupBy", StringComparison.CurrentCultureIgnoreCase) Then
                m_GroupByFields.Add(field)
            ElseIf field.Summary.Equals("Link", StringComparison.CurrentCultureIgnoreCase) Then
                'do nothing, and don't add to "all fields"
                Continue For
            Else
                m_SummaryFields.Add(field)
            End If
            m_AllFields.Add(field)
        Next
        For Each ChildTable As EraTable In node.m_ColTables
            If ChildTable.isInlineTable Then
                SplitFields(ChildTable)
            End If
        Next
    End Sub

    Private Function GetFilterExpression(ByRef dRow As DataRow) As String
        Try
            Dim clauses As New ArrayList
            Dim strType As String
            For Each field As EraField In m_GroupByFields
                If dRow.IsNull(field.ElementName) Then
                    clauses.Add("[" & field.ElementName & "] IS NULL")
                Else
                    strType = dRow.Table.Columns(field.ElementName).DataType.ToString
                    If strType.Equals("System.String", StringComparison.CurrentCultureIgnoreCase) Then
                        'I would like to be able to use the following when appropriate...But for now, just go strict
                        'clauses.Add("UPPER(" & field.ElementName & ") = '" & dRow(field.ElementName).ToString.ToUpper & "'")
                        clauses.Add("[" & field.ElementName & "] = '" & dRow(field.ElementName).ToString.Replace("'", "''") & "'")
                    ElseIf strType.Equals("System.DateTime", StringComparison.CurrentCultureIgnoreCase) Then
                        clauses.Add("[" & field.ElementName & "] = #" & dRow(field.ElementName) & "#")
                    Else
                        clauses.Add("[" & field.ElementName & "] = " & dRow(field.ElementName))
                    End If
                End If
            Next
            Return Join(clauses.ToArray, " AND ")
        Catch ex As Exception
            myERALayer.AddError(ex, "SummarizationAnalysis.GetFilterExpression")
            Throw
        End Try
    End Function

    Private Sub ProcessNestedTables(ByRef parentNode As EraLayerTable, ByRef ParentRecordNode As Xml.XmlElement, ByRef ParentRow As DataRow)
        Dim inlineTables As New Generic.List(Of EraTable)
        For Each ChildTable As EraTable In parentNode.m_ColTables
            If ChildTable.isInlineTable Then
                inlineTables.Add(ChildTable)
            Else
                ChildTable.WriteXmlTableResults(ParentRecordNode, ParentRow)
            End If
        Next
        For Each ChildTable As EraTable In inlineTables
            ProcessNestedTables(ChildTable, ParentRecordNode, ParentRow)
        Next
    End Sub
End Class
