Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports System.Runtime.InteropServices

''' <summary>
''' The Feature Comparison analysis summarizes the unique values found in features in the resource layer that intersect the analysis area, and reports the amount of those features found. Actual output varies by the geometry types of the analysis feature and layer being analyzed.
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public Class FeatureComparisonAnalysis
    Inherits ERAAnalysis
    'The Feature Comparison Algorithm
    'Acreage will be applied to a single Field in a layer
    'It is assumed to be the first field
    'Additional fields should be ignored
    'For each record in the Layer intersected by the Analysis Shape
    '   Clip the record's SHAPE with the Analysis Shape
    '   Multiply the Clipped Shape's Area (square meters) by the constant to get acreas
    'Multiple records with the same value must be consolidated
    'In addition, since the acreage field does not exist, a FIELD node must be create
    'and a childnode must be added to each RECORD node to represent acreage
    'Example:
    '    <LAYER name="LDI" type="acreage">
    '     <FIELDS>
    '      <FIELD name="BASE.STATEWIDE_LANDUSE_1995.Attribute" alias="Land Use"/>
    '     </FIELDS>
    '     <RECORD rec="1">
    '      <BASE.STATEWIDE_LANDUSE_1995.Attribute link="false">Coniferous Plantations
    '      </BASE.STATEWIDE_LANDUSE_1995.Attribute>
    '      <ACREAGE>5.012
    '      </ACREAGE>
    '     </RECORD>
    '     <RECORD rec="2">
    '      <BASE.STATEWIDE_LANDUSE_1995.Attribute link="false">Wetland Hardwood Forests
    '      </BASE.STATEWIDE_LANDUSE_1995.Attribute>
    '      <ACREAGE>10.35
    '      </ACREAGE>
    '     </RECORD>
    '     <RECORD rec="3">
    '      <BASE.STATEWIDE_LANDUSE_1995.Attribute link="false">Shrub and Brushland
    '      </BASE.STATEWIDE_LANDUSE_1995.Attribute>
    '      <ACREAGE>22.0
    '      </ACREAGE>
    '     </RECORD>
    '    </LAYER>





    ''' <summary>
    ''' Writes the XML results.
    ''' </summary>
    Public Overrides Sub WriteXmlResults()
        'Need to make sure that analysis feature is a polygon, or if point or line, that a buffer is supplied
        If AnalysisShape.Dimension = ESRI.ArcGIS.Geometry.esriGeometryDimension.esriGeometry2Dimension Then
            PerformAnalysisSub(AnalysisShape)

            If BufferShape IsNot Nothing AndAlso Not BufferShape.IsEmpty Then
                WriteXmlHeader(m_OutputXmlElement.ParentNode)
                m_OutputXmlElement.SetAttribute("name", Me.Name & " (within buffer)")
                PerformAnalysisSub(BufferShape)
            End If
        ElseIf BufferShape IsNot Nothing AndAlso Not BufferShape.IsEmpty Then
            PerformAnalysisSub(BufferShape)
        Else
            MyBase.WriteNoRecordsNode("Unable to perform Feature Comparison Analysis: analysis feature is not a polygon, and a buffer size of 0 was specified.")
        End If
    End Sub

    ''' <summary>
    ''' Performs the bulk of the analysis, called by WriteXmlResults
    ''' If AnalysisShape is a polygon, and a buffer is supplied, this sub is called twice, once to write the results of the AnalysisShape, and again for the BufferShape
    ''' If AnalysisShape is a point or line, this sub is called just once with the BufferShape
    ''' If AnalysisShape is a point or line, and no buffer is supplied, the "No Records" node is written with a warning message.
    ''' </summary>
    ''' <param name="AnalysisOrBufferArea">The analysis or buffer area to analyze.</param>
    Private Sub PerformAnalysisSub(ByRef AnalysisOrBufferArea As IGeometry)
        Try
            Dim SummarizedValueFormatString As String = "#,##0" 'default format string used to present the length/count. When area is reported, this value is changed to show two digits after the decimal.

            Dim pRowShape As IGeometry
            Dim pRow As IFeature
            Dim strSelectedValue As String

            Dim dtResults As New DataTable

            Dim myFeatureClass As IFeatureClass = FeatureClass
            Dim pFeatureCursor As IFeatureCursor = OpenFeatureCursor(AnalysisOrBufferArea)

            pRow = pFeatureCursor.NextFeature
            If pRow Is Nothing Then
                WriteNoRecordsNode()
            Else
                'Add a summary field
                Dim SummaryFieldName As String
                Select Case myFeatureClass.ShapeType
                    Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon
                        'sum acreage based on unique combination of input field values
                        SummaryFieldName = "Total Area (acres)"
                        SummarizedValueFormatString = "#,##0.00"
                    Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline
                        'sum length of features based on unique combo of input field values
                        SummaryFieldName = "Total Length (meters)"
                    Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint
                        'sum # of features based on unique combo of input field values
                        SummaryFieldName = "# Features"
                    Case Else
                        'do we support anything else? treat as count?
                        'for now, treating as count.
                        SummaryFieldName = "# Features (unrecognized shape type)"
                End Select
                Dim ef As New EraField(Me)
                ef.Name = "ERAsummarizedvalue"
                ef.FieldAlias = SummaryFieldName
                Me.m_ColFields.Add(ef)
                Dim percentage As EraField = Nothing
                If myFeatureClass.ShapeType = esriGeometryType.esriGeometryPolygon Then
                    percentage = New EraField(Me)
                    percentage.Name = "PercentArea"
                    percentage.FieldAlias = "Percent of Area"
                    Me.m_ColFields.Add(percentage)
                End If
                Me.WriteFieldsNode()
                Me.m_ColFields.Remove(ef)
                If percentage IsNot Nothing Then
                    Me.m_ColFields.Remove(percentage)
                End If
                For Each field As EraField In m_ColFields
                    dtResults.Columns.Add(field.ElementName)
                Next
                dtResults.Columns.Add("ERAsummarizedvalue", GetType(System.Double))

                Dim drResult As DataRow
                Dim iCurrentFeature As Integer = 1

                'Generate <RECORD> nodes and <field name> nodes
                While pRow IsNot Nothing
                    If HasInnerJoins Then
                        For Each childtable As EraTable In m_ColTables
                            If Not childtable.HasMatchingRecords(pRow) Then
                                pRow = pFeatureCursor.NextFeature
                                Continue While
                            End If
                        Next
                    End If
                    'Get the shape
                    pRowShape = pRow.Shape

                    'get list of values for all selected fields
                    'filter dtResults to see if a row with this combo of values exists
                    Dim strFilter As String = ""
                    'Dim oValue As Object
                    For Each field As EraField In m_ColFields
                        Try
                            strSelectedValue = field.GetFieldValue(pRow) '.Value(nFieldNumber)
                            'strSelectedValue = Nz(oValue)
                            strFilter &= "[" & field.ElementName & "] = '" & strSelectedValue.Replace("'", "''") & "' AND "
                        Catch ex As Exception
                            Logging.Log.Error("Error obtaining value for " & field.FieldName, ex)
                            strSelectedValue = "Error obtaining value for " & field.FieldAlias & ": " & ex.Message
                        End Try
                        'strSelectedValue = Trim(Nz(pRow.Value(pRow.Fields.FindField(field.FieldName))))
                        'Changed Replace("',", "''") to Replace("'", "''")
                    Next


                    If strFilter <> "" Then
                        Logging.Log.Debug("Using classification filter: " & strFilter)

                        'at least one classification field is present, so trim off the last "AND"
                        Dim dv As New DataView(dtResults)
                        dv.RowFilter = strFilter.Substring(0, strFilter.Length - 5)
                        If dv.Count = 0 Then
                            'row does not already exist with these values, so create a new one
                            drResult = dtResults.NewRow
                            For Each field As EraField In m_ColFields
                                'Dim objVal As Object = pRow.Value(pRow.Fields.FindField(field.FieldName))

                                'strSelectedValue = Nz(objVal)
                                drResult(field.ElementName) = field.GetFieldValue(pRow)
                            Next
                            drResult("ERAsummarizedvalue") = 0
                            dtResults.Rows.Add(drResult)
                        Else
                            'row already exists, get a reference to the existing row
                            drResult = dv(0).Row
                        End If
                    Else
                        'no classification field?
                        'just report the total #/length/area of all
                        'features in the analysis area.
                        If dtResults.Rows.Count = 0 Then
                            drResult = dtResults.NewRow
                            drResult("ERAsummarizedvalue") = 0
                            dtResults.Rows.Add(drResult)
                        Else
                            drResult = dtResults.Rows(0)
                        End If
                    End If

                    'add 1/length/area to ERAsummarizedvalue field
                    Select Case myFeatureClass.ShapeType
                        Case esriGeometryType.esriGeometryPolygon
                            'sum acreage based on unique combination of input field values
                            drResult("ERAsummarizedvalue") += CalculateIntersectionAcreage(AnalysisOrBufferArea, pRowShape)
                        Case esriGeometryType.esriGeometryPolyline
                            'sum length of features based on unique combo of input field values
                            drResult("ERAsummarizedvalue") += CalculateLength(AnalysisOrBufferArea, pRowShape)
                        Case esriGeometryType.esriGeometryPoint
                            'sum # of features based on unique combo of input field values
                            drResult("ERAsummarizedvalue") += 1
                        Case Else
                            'do we support anything else? treat as count?
                            'for now, treating as count.
                            drResult("ERAsummarizedvalue") += 1
                    End Select
                    Marshal.ReleaseComObject(pRow)

                    'Increment counter and get next Record from QueryResults
                    Try
                        pRow = pFeatureCursor.NextFeature
                    Catch ex As Exception
                        With Logging.Log
                            .Error("Error moving to next feature in feature cursor. Extended debug info will follow", ex)
                            Try
                                .Debug("Current input feature: " & iCurrentFeature)
                                .Debug(dtResults.Rows.Count & " rows in output")
                                .Debug("Current output row state: " & drResult.RowState)
                                .Debug("Output row values: " & Join(drResult.ItemArray, " | "))
                                Dim pSpatialFilter As ISpatialFilter = New ESRI.ArcGIS.Geodatabase.SpatialFilter
                                pSpatialFilter.Geometry = AnalysisOrBufferArea
                                pSpatialFilter.SpatialRel = ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelIntersects
                                pSpatialFilter.WhereClause = WhereExpression

                                .Debug("# features in feature cursor: " & FeatureClass.FeatureCount(pSpatialFilter))

                            Catch ex2 As Exception
                                .Error("Error processing extended debug info", ex2)
                            End Try
                            Throw
                        End With
                    End Try
                    iCurrentFeature += 1
                End While

                'WriteRecordNodes(m_outputxmlelement)
                Dim iRow As Integer = 1

                Dim totalAcreage As Double = GetAcreage(AnalysisOrBufferArea)
                For Each drResult In dtResults.Rows
                    Dim RecordNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("RECORD")
                    RecordNode.SetAttribute("rec", iRow)
                    m_OutputXmlElement.AppendChild(RecordNode)
                    Dim ValuesNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("VALUES")
                    RecordNode.AppendChild(ValuesNode)
                    For Each field As EraField In m_ColFields
                        Dim valueNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement(field.ElementName)
                        'valueNode.SetAttribute("link", "false")
                        valueNode.InnerText = drResult(field.ElementName)
                        ValuesNode.AppendChild(valueNode)
                    Next

                    Dim AcreageNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement(Xml.XmlConvert.EncodeLocalName(Name & "_ERAsummarizedvalue"))
                    'AcreageNode.SetAttribute("link", "false")
                    AcreageNode.InnerText = Format(drResult("ERAsummarizedvalue"), SummarizedValueFormatString)
                    ValuesNode.AppendChild(AcreageNode)

                    If myFeatureClass.ShapeType = esriGeometryType.esriGeometryPolygon Then
                        Dim percentageNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("PercentArea")
                        Dim acreage As Double = drResult("ERAsummarizedvalue")
                        percentageNode.InnerText = Format(acreage / totalAcreage, "##0.## %")
                        ValuesNode.AppendChild(percentageNode)
                    End If
                    iRow += 1

                    '2/29/2008: Adding support for Child Tables
                    For Each ChildTable As EraTable In m_ColTables
                        ChildTable.WriteXmlTableResults(RecordNode, drResult)
                    Next
                Next

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

                'Putting the Total Row in after the sorting takes place so that it stays at the bottom
                Try
                    Dim TotalNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("RECORD")
                    Dim ValuesNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("VALUES")
                    TotalNode.AppendChild(ValuesNode)
                    Dim ValueNode As Xml.XmlElement = Nothing
                    For Each field As EraField In m_ColFields
                        ValueNode = m_OutputXmlDoc.CreateElement(field.ElementName)
                        ValuesNode.AppendChild(ValueNode)
                    Next
                    If ValueNode Is Nothing Then
                        'This means that there were no fields, which means that there was only one row in the output
                        'and it was the totals, so just skip out of this part...
                        Exit Try
                    End If
                    ValueNode.InnerText = "TOTAL:"
                    ValueNode = m_OutputXmlDoc.CreateElement(Xml.XmlConvert.EncodeLocalName(Name & "_ERAsummarizedvalue"))
                    Dim acreage As Double = dtResults.Compute("SUM(ERAsummarizedvalue)", "")
                    ValueNode.InnerText = Format(acreage, SummarizedValueFormatString)
                    ValuesNode.AppendChild(ValueNode)
                    If myFeatureClass.ShapeType = esriGeometryType.esriGeometryPolygon Then
                        ValueNode = m_OutputXmlDoc.CreateElement("PercentArea")
                        ValueNode.InnerText = Format(acreage / totalAcreage, "##0.## %")
                        ValuesNode.AppendChild(ValueNode)
                    End If
                    m_OutputXmlElement.AppendChild(TotalNode)
                Catch
                    Throw
                End Try
            End If

            '2-12-09: moved here from just after the End While loop through rows in the feature cursor.
            'i suspect that the feature cursors were being opened, but not closed when no features were found.

            Logging.Log.Debug("Releasing cursor for " & Me.Name)
            Marshal.ReleaseComObject(pFeatureCursor)
            pFeatureCursor = Nothing


        Catch ex As Exception
            AddError(ex, "FeatureComparisonAnalysis.PerformAnalysisSub")
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Calculates the acreage of an intersection between the two IGeometry pointers passed to this function.
    ''' </summary>
    ''' <param name="pShape">The shape being used for the analysis area (AnalysisShape or BufferShape).</param>
    ''' <param name="pRowShape">A shape selected from the layer.</param>
    ''' <returns>The area in acres of the overlapping area between the two shapes.</returns>
    Private Function CalculateIntersectionAcreage(ByRef pShape As ESRI.ArcGIS.Geometry.IGeometry, ByRef pRowShape As ESRI.ArcGIS.Geometry.IGeometry) As Double
        Try
            If pShape Is Nothing Then
                Throw New EraException("AnalysisOrBufferArea shape is nothing in FeatureComparisonAnalysis.CalculateIntersectionAcreage")
            End If
            If pRowShape Is Nothing OrElse pRowShape.IsEmpty Then
                Throw New EraException("Resource shape is nothing in FeatureComparisonAnalysis.CalculateIntersectionAcreage")
            End If

            Dim pTopoOp As ESRI.ArcGIS.Geometry.ITopologicalOperator = pShape
            Dim pClipShape As ESRI.ArcGIS.Geometry.IGeometry = pTopoOp.Intersect(pRowShape, ESRI.ArcGIS.Geometry.esriGeometryDimension.esriGeometry2Dimension)

            Return GetAcreage(pClipShape)

        Catch ex As EraException
            AddError(ex, "FeatureComparisonAnalysis.CalculateIntersectionAcreage")
            Throw
        Catch ex As System.Runtime.InteropServices.COMException
            AddError(ex, "FeatureComparisonAnalysis.CalculateIntersectionAcreage")
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Calculates the length of an intersection between the two IGeometry pointers passed to this function.
    ''' </summary>
    ''' <param name="pShape">The shape being used for the analysis area (AnalysisShape or BufferShape).</param>
    ''' <param name="pRowShape">A shape selected from the layer.</param>
    ''' <returns>The length in meters of the portion of the pRowShape geometry that intersects pShape.</returns>
    Private Function CalculateLength(ByRef pShape As ESRI.ArcGIS.Geometry.IGeometry, ByRef pRowShape As ESRI.ArcGIS.Geometry.IGeometry) As Double
        If pShape Is Nothing Then
            Throw New EraException("AnalysisOrBufferArea shape is nothing in FeatureComparisonAnalysis.CalculateIntersectionAcreage")
        End If
        If pRowShape Is Nothing Then
            Throw New EraException("Resource shape is nothing in FeatureComparisonAnalysis.CalculateIntersectionAcreage")
        End If
        Try

            Dim pTopoOp As ESRI.ArcGIS.Geometry.ITopologicalOperator = pShape
            Dim pClipShape As ESRI.ArcGIS.Geometry.IPolyline = pTopoOp.Intersect(pRowShape, ESRI.ArcGIS.Geometry.esriGeometryDimension.esriGeometry1Dimension)

            Return pClipShape.Length
        Catch ex As System.Runtime.InteropServices.COMException
            AddError(ex, "FeatureComparisonAnalysis.CalculateLength")
            Throw
        End Try

    End Function
End Class