Imports System.Runtime.InteropServices
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry

''' <summary>
''' The Weighted Average analysis summarizes the numeric values found in the specified field in the layer being analyzed based on the proportion of the intersecting area (the overlap of the analysis feature or buffer area and the feature in the target layer) within the entire analysis area. This analysis type is primarily designed for analysis of polygon resource features within a polygon analysis feature, or buffered line or polygon analysis feature.
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public Class WeightedAverageAnalysis
    Inherits ERAAnalysis

    Private ValueAccumulator As Generic.Dictionary(Of String, Double)
    Private AreaAccumulator As Generic.Dictionary(Of String, Double)
    Private blnPolygon As Boolean

    'The Weighted Average Algorithm
    'For each record in the Layer intersected by the Analysis Shape
    '   Clip the record's SHAPE with the Analysis Shape
    '   Store the (Clipped Shape's Area / Analysis Shape Area) as AreaRatio
    '   Select the AnalysisField's value (numeric)
    '   Multiply the AnalysisField's value by the AreaRatio
    '   Add the product to a Value m_Accumulator
    'The entire recordset yields a single, numeric result
    'The return node contains one record
    'Example:
    '    <LAYER name="LDI" type="weighted average">
    '     <FIELDS>
    '      <FIELD name="BASE.STATEWIDE_LANDUSE_1995.LDI" alias="LDI"/>
    '     </FIELDS>
    '     <RECORD rec="1">
    '      <BASE.STATEWIDE_LANDUSE_1995.LDI link="false">3.12500
    '      </BASE.STATEWIDE_LANDUSE_1995.LDI>
    '     </RECORD>
    '    </LAYER>


    ''' <summary>
    ''' Writes the XML results.
    ''' Analysis Area of a weighted average analysis will either be the AnalysisShape plus BufferShape (if AnalysisShape is a polygon)
    ''' or just the BufferShape (if AnalysisShape is a point or line). This is determined in the calling sub in ERAAnalysis.
    ''' If the buffer distance is 0, then just the (polygon) AnalysisShape is used. 
    ''' If the buffer distance is 0 and the feature is not a polygon, then a warning is noted in the output and the analysis is not performed.
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
            m_OutputXmlElement.SetAttribute("name", Me.Name & " (within buffer)")
            PerformAnalysisSub(BufferShape)
        Else
            WriteNoRecordsNode("Unable to perform Weighted Average Analysis: analysis feature is not a polygon, and a buffer size of 0 was specified.")
        End If
    End Sub

    ''' <summary>
    ''' Performs the analysis.
    ''' </summary>
    ''' <param name="pShape">A pointer to the shape used to perform the analysis.</param>
    Private Sub PerformAnalysisSub(ByRef pShape As IGeometry)
        Try
            Dim pRowShape As IGeometry
            ValueAccumulator = New Generic.Dictionary(Of String, Double)
            AreaAccumulator = New Generic.Dictionary(Of String, Double)
            For Each field As EraField In m_ColFields
                ValueAccumulator.Add(field.ElementName, 0)
                AreaAccumulator.Add(field.ElementName, 0)
            Next
            Dim pFeatureCursor As IFeatureCursor = OpenFeatureCursor(pShape)
            Dim pFeat As IFeature = pFeatureCursor.NextFeature
            If pFeat Is Nothing Then
                WriteNoRecordsNode()
                Marshal.ReleaseComObject(pFeatureCursor)
                Exit Sub
            End If
            'pRow is not nothing::pCursor contains at least one record
            WriteFieldsNode()
            If pFeat.Shape.Dimension = esriGeometryDimension.esriGeometry2Dimension Then
                blnPolygon = True
            Else
                blnPolygon = False
            End If
            'Generate <RECORD> nodes and <field name> nodes
            While pFeat IsNot Nothing
                pRowShape = pFeat.Shape
                For Each field As EraField In m_ColFields
                    Dim objVal As Object = pFeat.Value(pFeat.Fields.FindField(field.Name))
                    If Not (objVal Is Nothing OrElse objVal Is DBNull.Value) Then
                        If IsNumeric(objVal) Then
                            If blnPolygon Then
                                AddValue(pShape, pRowShape, Val(objVal), field)
                            Else 'no weighting is done for linear or point features
                                ValueAccumulator(field.ElementName) += Val(objVal)
                            End If
                        End If
                    End If
                Next
                'Increment counter and get next Record from QueryResults
                pFeat = pFeatureCursor.NextFeature
            End While
            WriteRecordNode()
            Marshal.ReleaseComObject(pFeatureCursor)
        Catch ex As Exception
            AddError(ex, "WeightedAverageAnalysis.PerformAnalysisSub")
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Gets the weighted product.
    ''' </summary>
    ''' <param name="pAnalysisShape">A pointer to the shape being analyzed (AnalysisShape or BufferShape).</param>
    ''' <param name="pResourceShape">A pointer to a shape for a feature being analyzed.</param>
    ''' <param name="dValue">A numeric value selected from the feature being analyzed.</param>
    ''' <param name="field">The EraField that is being calculated.</param>
    Private Sub AddValue(ByVal pAnalysisShape As IGeometry, ByVal pResourceShape As IGeometry, ByVal dValue As Double, ByVal field As EraField)
        Try
            Dim pTopoOp As ITopologicalOperator = pAnalysisShape
            Dim pOverlapArea As IArea = pTopoOp.Intersect(pResourceShape, esriGeometryDimension.esriGeometry2Dimension)
            AreaAccumulator(field.ElementName) += pOverlapArea.Area
            Dim pResourceArea As IArea = pResourceShape
            If field.isIndexed Then
                ValueAccumulator(field.ElementName) += pOverlapArea.Area * dValue
            Else
                ValueAccumulator(field.ElementName) += pOverlapArea.Area * dValue / pResourceArea.Area
            End If
        Catch ex As Exception
            AddError(ex, "WeightedAverageAnalysis.AddValue")
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Writes the record node.
    ''' </summary>
    Private Sub WriteRecordNode()
        Dim RecordNode As Xml.XmlElement = Me.m_OutputXmlDoc.CreateElement("RECORD")
        RecordNode.SetAttribute("rec", 1)
        Dim ValuesNode As Xml.XmlElement = Me.m_OutputXmlDoc.CreateElement("VALUES")
        'Now the m_Accumulator array contains values for each ERAField
        'Loop through the m_colFields collection and generate one RECORD node
        'that contains one <field name> nodefor each field
        'Example:
        '            <RECORD rec="1">
        '               <VALUES>
        '                <BASE.STATEWIDE_LANDUSE_1995.LDI link="false">3.125
        '                </BASE.STATEWIDE_LANDUSE_1995.LDI>
        '                <BASE.STATEWIDE_LANDUSE_1995.LSI link="false">6.7834
        '                </BASE.STATEWIDE_LANDUSE_1995.LSI>
        '               </VALUES>
        '            </RECORD>

        For Each field As EraField In m_ColFields
            If ValueAccumulator.ContainsKey(field.ElementName) Then
                Dim ValueNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement(field.ElementName)
                ValueNode.SetAttribute("link", "false")
                Dim d As Double
                If blnPolygon And field.isIndexed Then
                    d = ValueAccumulator(field.ElementName) / AreaAccumulator(field.ElementName)
                Else
                    d = ValueAccumulator(field.ElementName)
                End If
                ValueNode.InnerText = Format(d, "#,##0.####")

                ValuesNode.AppendChild(ValueNode)
            End If
        Next
        RecordNode.AppendChild(ValuesNode)
        m_OutputXmlElement.AppendChild(RecordNode)
    End Sub
End Class