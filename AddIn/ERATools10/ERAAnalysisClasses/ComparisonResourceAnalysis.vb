Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports System.Runtime.InteropServices

''' <summary>
''' The Comparison Resource analysis summarizes a pair of analysis layers, a “parent” layer (the layer specified in the analysis setup screen), and a “child” layer (set up as a sub-table, as if it were a one-to-many relationship). The parent layer must contain polygon features. The child layer can contain point, line, or polygon features. 
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public Class ComparisonResourceAnalysis
    Inherits ERAAnalysis

    ''' <summary>
    ''' Represents the intersection of the analysis + buffer and the features of the featureclass referenced in the LAYER tag
    ''' </summary>
    Dim m_FirstShape As IGeometry

    ''' <summary>
    ''' Represents the intersection of the analysis + buffer and the features of the featureclass referenced in the child TABLE tag
    ''' </summary>
    Dim m_SecondShape As IGeometry

    ''' <summary>
    ''' Represents the intersection of the firstShape and secondShape
    ''' </summary>
    Dim m_IntersectionShape As IGeometry

    ''' <summary>
    ''' A reference to an EraTable instance that represents the second geographic layer being analyzed.
    ''' </summary>
    Private ChildLayer As EraTable

    ''' <summary>
    ''' Overrides the SetXmlNode of the EraAnalysis class to ensure that the feature type of the feature class referenced in the LAYER tag is polygon, and that there is a single child TABLE tag representing the second layer being compared.
    ''' </summary>
    ''' <param name="EraXmlElement">The XML element that defines an instance of this class.</param>
    Public Overrides Sub SetXmlNode(ByRef EraXmlElement As System.Xml.XmlElement)
        MyBase.SetXmlNode(EraXmlElement)
        If Me.FeatureClass.ShapeType <> esriGeometryType.esriGeometryPolygon Then
            Throw New EraException("Unable to perform Comparison Resource Analysis on point or line resource feature as primary (first) feature type. Layer must contain polygon features.")
        End If

        If Me.m_ColTables.Count < 1 Then
            Throw New EraException("Unable to run perform Comparison Resource Analysis. Second (child) layer not specified as nested <TABLE> tag")
            Exit Sub
        End If
    End Sub

    ''' <summary>
    ''' Writes the XML results.
    ''' </summary>
    Public Overrides Sub WriteXmlResults()
        Try

            If AnalysisPlusBufferShape Is Nothing OrElse AnalysisPlusBufferShape.IsEmpty OrElse AnalysisPlusBufferShape.Dimension <> esriGeometryDimension.esriGeometry2Dimension Then
                WriteNoRecordsNode("Unable to perform Comparison Resource Analysis: analysis feature is not a polygon, and a buffer size of 0 was specified.")
                Exit Sub
            End If

            ChildLayer = Me.m_ColTables(0)
            If ChildLayer.GetDataSetType <> esriDatasetType.esriDTFeatureClass Then
                Throw New EraException("Error: unable to run comparison resource analysis. Reference to second layer is invalid")
            End If
            Dim ChildFeatureClass As IFeatureClass = ChildLayer.FeatureClass
            If ChildFeatureClass Is Nothing Then
                Throw New EraException("Error: unable to run comparison resource analysis. Reference to second layer is invalid")
            End If

            'If System.Diagnostics.Debugger.IsAttached Then
            '    MapDrawing.DeleteElements()
            '    MapDrawing.DrawShape(AnalysisPlusBufferShape)
            '    Application.DoEvents()
            'End If

            Dim MyFeatureCursor As IFeatureCursor = OpenFeatureCursor(AnalysisPlusBufferShape)
            Dim MyFeatureShape As IGeometry = GetAggregateShapeFromFeatureCursor(MyFeatureCursor)
            Marshal.ReleaseComObject(MyFeatureCursor)

            Logging.Log.Debug("Clipping the shape to the buffer and saving in FirstShape")
            If MyFeatureShape Is Nothing Then
                Logging.Log.Debug("Empty First Shape, no features found")
                WriteNoRecordsNode()
                Exit Sub
            Else
                Logging.Log.Debug("Intersect projected ResourceShape1 and Analysis Area")
                m_FirstShape = IntersectShapes(MyFeatureShape, AnalysisPlusBufferShape)
            End If

            'If System.Diagnostics.Debugger.IsAttached Then
            '    MapDrawing.DrawShape(m_FirstShape)
            '    Application.DoEvents()
            'End If

            Logging.Log.Debug("Getting Cursor of Intersection for " & ChildLayer.Name)

            Select Case ChildFeatureClass.ShapeType
                Case esriGeometryType.esriGeometryPoint
                    Dim ChildSFilter As ISpatialFilter = New SpatialFilter
                    ChildSFilter.Geometry = AnalysisPlusBufferShape
                    ChildSFilter.WhereClause = ChildLayer.WhereExpression
                    ChildSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
                    Dim TotalFeaturesInAnalysisArea As Integer = ChildFeatureClass.FeatureCount(ChildSFilter)
                    ChildSFilter.Geometry = m_FirstShape
                    Dim FeaturesInFirstShape As Integer = ChildFeatureClass.FeatureCount(ChildSFilter)
                    WriteFieldsNode("# of Features")
                    WriteCountRecordNodes(FeaturesInFirstShape, TotalFeaturesInAnalysisArea)
                Case Else
                    Dim ChildFeatureCursor As IFeatureCursor = ChildLayer.OpenFeatureCursor(AnalysisPlusBufferShape)
                    Dim ChildShape As IGeometry = GetAggregateShapeFromFeatureCursor(ChildFeatureCursor)
                    Marshal.ReleaseComObject(ChildFeatureCursor)

                    If ChildShape Is Nothing Then
                        Logging.Log.Debug("ChildShape is Nothing")
                    Else
                        Logging.Log.Debug("Intersect TableShape and Buffer")
                        m_SecondShape = IntersectShapes(ChildShape, AnalysisPlusBufferShape)
                        If m_SecondShape.IsEmpty Then
                            Logging.Log.Debug("SecondShape is empty")
                        End If
                    End If

                    Logging.Log.Debug("Intersect ResourceShape1 and TableShape")
                    m_IntersectionShape = IntersectShapes(m_FirstShape, m_SecondShape)
                    If m_IntersectionShape Is Nothing OrElse m_IntersectionShape.IsEmpty Then
                        Logging.Log.Debug("Intersection Shape is Empty")
                    End If

                    Select Case ChildFeatureClass.ShapeType
                        Case esriGeometryType.esriGeometryPolyline
                            WriteFieldsNode("Length (meters)")
                            WriteLengthRecordNodes()
                        Case esriGeometryType.esriGeometryPolygon
                            WriteFieldsNode("Acreage")
                            WriteAcreageRecordNodes()
                    End Select
            End Select

        Catch ex As Exception
            AddError(ex, "ComparisonResourceAnalysis.WriteXmlResults")
            Throw
        End Try

    End Sub

    ''' <summary>
    ''' Returns the geometric intersection of the two shapes, after first ensuring they're in the same coordinate system
    ''' </summary>
    ''' <param name="pFirstShape">Pointer to the first shape being compared.</param>
    ''' <param name="pSecondShape">Pointer to the second shape being compared.</param>
    ''' <returns>Geometry representing the geometric intersection of the two shapes passed as arguments to the function.</returns>
    Private Function IntersectShapes(ByRef pFirstShape As IGeometry, ByRef pSecondShape As IGeometry) As IGeometry
        Logging.Log.Debug("Call to IntersectShapes in ComparisonResourceAnalysis")

        If pFirstShape Is Nothing OrElse pFirstShape.IsEmpty Then
            Logging.Log.Warn("First shape is nothing")
            Return Nothing
        End If
        If pSecondShape Is Nothing OrElse pSecondShape.IsEmpty Then
            Logging.Log.Warn("Second shape is nothing")
            Return Nothing
        End If


        Logging.Log.Debug("Projecting Shapes")
        Try
            'assume them to have the same spatial reference if one or the other is nothing
            If pFirstShape.SpatialReference Is Nothing Then pFirstShape.SpatialReference = pSecondShape.SpatialReference
            If pSecondShape.SpatialReference Is Nothing Then pSecondShape.SpatialReference = pFirstShape.SpatialReference

            If Not CompareSpatialReferences(pFirstShape.SpatialReference, pSecondShape.SpatialReference) Then
                pFirstShape.Project(pSecondShape.SpatialReference)
            End If

        Catch ex As Exception
            AddError(ex, "ComparisonResourceAnalysis.IntersectShapes")
            'Logging.Log.Error("Exception projecting shapes in ComparisonResourceAnalysis.IntersectShapes", ex)
            Throw
        End Try

        Try

            Dim pTopoOp As ESRI.ArcGIS.Geometry.ITopologicalOperator
            Dim pShape As ESRI.ArcGIS.Geometry.IGeometry

            Logging.Log.Debug("Intersecting Shapes")
            Logging.Log.Debug("FirstShape: " & " shapetype: " & pFirstShape.GeometryType & " dimension: " & pFirstShape.Dimension & " envelope: (" & pFirstShape.Envelope.XMax & ", " & pFirstShape.Envelope.XMin & ", " & pFirstShape.Envelope.YMax & ", " & pFirstShape.Envelope.YMin & ")")
            Logging.Log.Debug("pSecondShape: " & " shapetype: " & pSecondShape.GeometryType & " dimension: " & pSecondShape.Dimension & " envelope: (" & pSecondShape.Envelope.XMax & ", " & pSecondShape.Envelope.XMin & ", " & pSecondShape.Envelope.YMax & ", " & pSecondShape.Envelope.YMin & ")")

            pTopoOp = pFirstShape
            If Not pTopoOp.IsKnownSimple Then
                If Not pTopoOp.IsSimple Then
                    pTopoOp.Simplify()
                End If
            End If

            pTopoOp = pSecondShape
            If Not pTopoOp.IsKnownSimple Then
                If Not pTopoOp.IsSimple Then
                    pTopoOp.Simplify()
                End If
            End If

            Logging.Log.Debug("Calling Intersect method on pTopoOp")

            If pFirstShape.GeometryType = esriGeometryType.esriGeometryPolyline Or pSecondShape.GeometryType = esriGeometryType.esriGeometryPolyline Then
                pShape = pTopoOp.Intersect(pFirstShape, esriGeometryDimension.esriGeometry1Dimension)
            Else
                pShape = pTopoOp.Intersect(pFirstShape, esriGeometryDimension.esriGeometry2Dimension)
            End If

            'If System.Diagnostics.Debugger.IsAttached Then
            '    Try
            '        MapDrawing.DeleteElements()
            '        MapDrawing.DrawShape(pFirstShape, pSecondShape)
            '        Application.DoEvents()
            '        If Not pShape.IsEmpty Then
            '            MapDrawing.DrawShape(pShape)
            '            Application.DoEvents()
            '        End If
            '    Catch ex As Exception
            '    End Try
            'End If


            If pShape.IsEmpty Then
                Logging.Log.Debug("Intersection is empty.")
                Dim pRelOp As IRelationalOperator = pSecondShape
                If pRelOp.Contains(pFirstShape) Then
                    Return pSecondShape
                End If
            End If

            Return pShape
        Catch ex As Exception
            'Logging.Log.Error("Error in ComparisonResourceAnalysis.IntersectShapes", ex)
            AddError(ex, "ComparisonResourceAnalysis.IntersectShapes")
            Throw
        End Try
    End Function


    ''' <summary>
    ''' Shadows the WriteFieldsNode from the EraAnalysis base class, as fields are always the same for this kind of an analysis and are not set from the FIELD tags in the analysis configuration file.
    ''' </summary>
    ''' <param name="SummaryFieldName">Name of the summary field to generate, varies by geometry type.</param>
    Private Shadows Sub WriteFieldsNode(ByVal SummaryFieldName As String)
        Try
            'Generate <FIELDS> and <FIELD> nodes
            'For polys, three Fields: Name, Acreage, and Percent
            'For points, Name and count 
            'For lines, Name and legnth
            Dim FieldsNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("FIELDS")

            Dim FieldNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("FIELD")
            FieldNode.SetAttribute("name", "NAME")
            FieldNode.SetAttribute("alias", "Name")
            FieldNode.SetAttribute("visible", "true")
            FieldsNode.AppendChild(FieldNode)

            FieldNode = m_OutputXmlDoc.CreateElement("FIELD")
            FieldNode.SetAttribute("name", SummaryFieldName.ToUpper)
            FieldNode.SetAttribute("alias", SummaryFieldName)
            FieldNode.SetAttribute("visible", "true")
            FieldsNode.AppendChild(FieldNode)

            If SummaryFieldName = "Acreage" Then
                FieldNode = m_OutputXmlDoc.CreateElement("FIELD")
                FieldNode.SetAttribute("name", "PERCENT")
                FieldNode.SetAttribute("alias", "Percent of Analysis Area")
                FieldNode.SetAttribute("visible", "true")
                FieldsNode.AppendChild(FieldNode)
            End If

            m_OutputXmlElement.AppendChild(FieldsNode)
        Catch ex As Exception
            AddError(ex, "ComparisonResourceAnalysis.WriteFieldsNode")
            Throw
        End Try
    End Sub


    ''' <summary>
    ''' Writes the count record nodes. Used when the geometry type of the layer referenced in the TABLE tag is point.
    ''' </summary>
    ''' <param name="FirstCount">The first count.</param>
    ''' <param name="SecondCount">The second count.</param>
    Private Sub WriteCountRecordNodes(ByVal FirstCount As Integer, ByVal SecondCount As Integer)
        Try
            Dim RecordNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("RECORD")
            RecordNode.SetAttribute("rec", 1)

            Dim ValuesNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("VALUES")

            Dim ValueNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("NAME")
            ValueNode.InnerText = "Analysis Area"
            ValuesNode.AppendChild(ValueNode)

            'First Row is Table Length (SecondShape) within Buffer
            ValuesNode = m_OutputXmlDoc.CreateElement("VALUES")
            RecordNode = m_OutputXmlDoc.CreateElement("RECORD")
            RecordNode.SetAttribute("rec", 1)

            ValueNode = m_OutputXmlDoc.CreateElement("NAME", "")
            ValueNode.InnerText = "Total count of " & ChildLayer.Name & " features within Analysis Area"
            ValuesNode.AppendChild(ValueNode)

            ValueNode = m_OutputXmlDoc.CreateElement("Count", "")
            ValueNode.InnerText = Format(SecondCount, "#,##0")
            ValuesNode.AppendChild(ValueNode)

            RecordNode.AppendChild(ValuesNode)
            m_OutputXmlElement.AppendChild(RecordNode)

            'First Row is Layer Length (FirstShape) within Buffer
            ValuesNode = m_OutputXmlDoc.CreateElement("VALUES")
            RecordNode = m_OutputXmlDoc.CreateElement("RECORD", "")
            RecordNode.SetAttribute("rec", 2)

            ValueNode = m_OutputXmlDoc.CreateElement("NAME")
            ValueNode.InnerText = "Total count of " & ChildLayer.Name & " features intersecting features of " & Me.Name & " within Analysis Area"
            ValuesNode.AppendChild(ValueNode)

            ValueNode = m_OutputXmlDoc.CreateElement("COUNT")
            ValueNode.InnerText = Format(FirstCount, "#,##0")
            ValuesNode.AppendChild(ValueNode)

            RecordNode.AppendChild(ValuesNode)
            m_OutputXmlElement.AppendChild(RecordNode)
        Catch ex As Exception
            AddError(ex, "ComparisonResourceAnalysis.WriteCountRecordNodes")
            Throw
        End Try
    End Sub


    ''' <summary>
    ''' Writes the length record nodes. Used when the geometry type of the layer referenced in the TABLE tag is line.
    ''' </summary>
    Private Sub WriteLengthRecordNodes()
        Try
            Dim TotalLengthInAnalysisArea As Double
            If m_SecondShape Is Nothing Then
                TotalLengthInAnalysisArea = 0
            Else
                TotalLengthInAnalysisArea = DirectCast(m_SecondShape, IPolyline).Length
            End If

            Dim LengthInFirstResource As Double
            If m_IntersectionShape Is Nothing Then
                LengthInFirstResource = 0
            Else
                LengthInFirstResource = DirectCast(Me.m_IntersectionShape, IPolyline).Length
            End If

            Dim RecordNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("RECORD")
            RecordNode.SetAttribute("rec", 1)

            Dim ValuesNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("VALUES")

            Dim ValueNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("NAME")
            ValueNode.InnerText = "Analysis Area"
            ValuesNode.AppendChild(ValueNode)

            'First Row is the Total length of features in the child layer within the analysis area
            ValuesNode = m_OutputXmlDoc.CreateElement("VALUES")
            RecordNode = m_OutputXmlDoc.CreateElement("RECORD", "")
            RecordNode.SetAttribute("rec", 2)

            ValueNode = m_OutputXmlDoc.CreateElement("NAME")
            ValueNode.InnerText = "Total length of " & ChildLayer.Name & " features within analysis area"
            ValuesNode.AppendChild(ValueNode)

            ValueNode = m_OutputXmlDoc.CreateElement("LENGTH")
            ValueNode.InnerText = Format(TotalLengthInAnalysisArea, "#,##0")
            ValuesNode.AppendChild(ValueNode)

            RecordNode.AppendChild(ValuesNode)
            m_OutputXmlElement.AppendChild(RecordNode)

            'Second Row is total length of features in the child layer that intersect features of the parent layer within the analysis area
            ValuesNode = m_OutputXmlDoc.CreateElement("VALUES")
            RecordNode = m_OutputXmlDoc.CreateElement("RECORD")
            RecordNode.SetAttribute("rec", 3)

            ValueNode = m_OutputXmlDoc.CreateElement("NAME", "")
            ValueNode.InnerText = "Total length of " & ChildLayer.Name & " features intersecting features of " & Me.Name & " within analysis area"
            ValuesNode.AppendChild(ValueNode)

            ValueNode = m_OutputXmlDoc.CreateElement("LENGTH", "")
            ValueNode.InnerText = Format(LengthInFirstResource, "#,##0")
            ValuesNode.AppendChild(ValueNode)

            RecordNode.AppendChild(ValuesNode)
            m_OutputXmlElement.AppendChild(RecordNode)

        Catch ex As Exception
            AddError(ex, "ComparisonResourceAnalysis.WriteLengthRecordNodes")
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Writes the acreage record nodes. Used when the geometry type of the layer referenced in the TABLE tag is polygon.
    ''' </summary>
    Private Sub WriteAcreageRecordNodes()
        Try
            'First record is the analysis area acreage
            Dim nBufferAcreage As Double = 0
            Dim strBufferAcreage As String
            Try
                nBufferAcreage = GetAcreage(AnalysisPlusBufferShape)
                strBufferAcreage = Format(nBufferAcreage, "#,##0.00")
            Catch ex As Exception
                AddError(ex, "ComparisonResourceAnalysis.WriteAcreageRecordNodes (buffer acreage)")
                Throw
            End Try

            WriteAcreageRecord(m_OutputXmlElement, 1, "Analysis Area", strBufferAcreage, "")

            'Second record is Layer Area (FirstShape) within analysis area
            Dim nFirstAcreage As Double = 0
            Dim strFirstAcreage As String
            Try
                nFirstAcreage = GetAcreage(m_FirstShape)
                strFirstAcreage = Format(nFirstAcreage, "#,##0.00")
            Catch ex As Exception
                AddError(ex, "ComparisonResourceAnalysis.WriteAcreageRecordNodes (first acreage)")
                Throw
            End Try
            WriteAcreageRecord(m_OutputXmlElement, 2, "Total acreage of " & Me.Name & " features within analysis area", strFirstAcreage, GetPercentOfArea(nFirstAcreage, nBufferAcreage))

            'Third record is Table Area (SecondShape) within analysis area
            Dim nSecondAcreage As Double = 0
            Dim strSecondAcreage As String
            Try
                nSecondAcreage = GetAcreage(m_SecondShape)
                strSecondAcreage = Format(nSecondAcreage, "#,##0.00")
            Catch ex As Exception
                AddError(ex, "ComparisonResourceAnalysis.WriteAcreageRecordNodes (second acreage)")
                Throw
            End Try
            WriteAcreageRecord(m_OutputXmlElement, 3, "Total acreage of " & ChildLayer.Name & " features within analysis area", strSecondAcreage, GetPercentOfArea(nSecondAcreage, nBufferAcreage))

            'Fourth Child is Intersection Area (IntersectionShape) of Layer and Table shapes
            Dim nIntersectionAcreage As Double = 0
            Dim strIntersectionAcreage As String
            Try
                nIntersectionAcreage = GetAcreage(m_IntersectionShape)
                strIntersectionAcreage = Format(nIntersectionAcreage, "#,##0.00")
            Catch ex As Exception
                AddError(ex, "ComparisonResourceAnalysis.WriteAcreageRecordNodes (intersection acreage)")
                Throw
            End Try
            WriteAcreageRecord(m_OutputXmlElement, 4, "Total acreage of overlapping " & Me.Name & " and " & ChildLayer.Name & " features within analysis area", strIntersectionAcreage, GetPercentOfArea(nIntersectionAcreage, nBufferAcreage))

        Catch ex As Exception
            AddError(ex, "ComparisonResourceAnalysis.WriteAcreageRecordNodes")
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Writes individual acreage record.
    ''' </summary>
    ''' <param name="m_OutputXmlElement">The XML element to which the results are to be appended.</param>
    ''' <param name="RecordNumber">The record number.</param>
    ''' <param name="NameText">The text to use for the name to identify the record.</param>
    ''' <param name="AcreageText">The text to use for the acreage value.</param>
    ''' <param name="PercentText">The text to use for the percentage value.</param>
    Private Sub WriteAcreageRecord(ByRef m_OutputXmlElement As Xml.XmlElement, ByVal RecordNumber As Integer, ByVal NameText As String, ByVal AcreageText As String, ByVal PercentText As String)
        Dim RecordNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("RECORD")
        RecordNode.SetAttribute("rec", RecordNumber)

        Dim ValuesNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("VALUES")
        Dim ValueNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("NAME")
        ValueNode.InnerText = NameText
        ValuesNode.AppendChild(ValueNode)

        ValueNode = m_OutputXmlDoc.CreateElement("ACREAGE")
        ValueNode.InnerText = AcreageText
        ValuesNode.AppendChild(ValueNode)

        ValueNode = m_OutputXmlDoc.CreateElement("PERCENT")
        ValueNode.InnerText = PercentText
        ValuesNode.AppendChild(ValueNode)


        RecordNode.AppendChild(ValuesNode)
        m_OutputXmlElement.AppendChild(RecordNode)
    End Sub

    ''' <summary>
    ''' Gets the percent of area.
    ''' </summary>
    ''' <param name="numerator">The numerator.</param>
    ''' <param name="denominator">The denominator.</param>
    ''' <returns>Formatted text representing the percentage of the area.</returns>
    Private Function GetPercentOfArea(ByRef numerator As Double, ByRef denominator As Double) As String
        If denominator <> 0 Then
            Return Format((numerator / denominator), "0 %")
        Else
            Return "Divide by 0 Error"
        End If
    End Function

    ''' <summary>
    ''' Gets the aggregate shape from feature cursor.
    ''' </summary>
    ''' <param name="pFeatureCursor">A pointer to an IFeatureCursor instance for which shapes are to be aggregated.</param>
    ''' <returns>A union of all shapes contained in features of the feature cursor</returns>
    Private Function GetAggregateShapeFromFeatureCursor(ByVal pFeatureCursor As IFeatureCursor) As IGeometry
        Try
            Dim pFeature As IFeature = pFeatureCursor.NextFeature
            Dim pSpatialReference As ISpatialReference
            Dim pPolygon As IGeometryBag = New GeometryBag
            Dim pGeometryCollection As IGeometryCollection = pPolygon
            If pFeature Is Nothing Then
                Return Nothing
            Else
                pSpatialReference = pFeature.Shape.SpatialReference
            End If

            Do While Not pFeature Is Nothing
                pGeometryCollection.AddGeometry(pFeature.ShapeCopy)
                pSpatialReference = pFeature.Shape.SpatialReference
                'If System.Diagnostics.Debugger.IsAttached Then
                '    MapDrawing.DeleteElements()
                '    MapDrawing.DrawShape(pFeature.Shape)
                '    Application.DoEvents()
                '    Debug.Print(pFeature.OID)
                '    Debug.Print(DirectCast(pFeature.Shape, IArea).Area)

                'End If
                pFeature = pFeatureCursor.NextFeature

            Loop

            If pGeometryCollection.GeometryCount = 1 Then Return pGeometryCollection.Geometry(0)

            If pGeometryCollection.GeometryCount = 0 Then
                Logging.Log.Warn("There are no shapes in the shape collection (m_colShapes.geometryCount = 0) in CreateUnionedShape")
                Return Nothing
            End If


            'Initialize the union shape based on the geometry type
            Dim pUnioningShape As ESRI.ArcGIS.Geometry.IGeometry
            Select Case pGeometryCollection.Geometry(0).GeometryType
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint
                    pUnioningShape = New ESRI.ArcGIS.Geometry.Multipoint
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultipoint
                    pUnioningShape = New ESRI.ArcGIS.Geometry.Multipoint
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryLine
                    pUnioningShape = New ESRI.ArcGIS.Geometry.Line
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline
                    pUnioningShape = New ESRI.ArcGIS.Geometry.Polyline
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon
                    pUnioningShape = New ESRI.ArcGIS.Geometry.Polygon
                Case Else
                    MsgBox("Unhandled Shape Type " & pGeometryCollection.Geometry(0).GeometryType)
                    Return Nothing
            End Select

            'Construct the union
            Dim pTopoOperator As ESRI.ArcGIS.Geometry.ITopologicalOperator = pUnioningShape
            pTopoOperator.ConstructUnion(pGeometryCollection)

            'reset the spatial reference?
            pUnioningShape.SpatialReference = pSpatialReference

            'If System.Diagnostics.Debugger.IsAttached Then
            '    MapDrawing.DeleteElements()
            '    MapDrawing.DrawShape(pUnioningShape)
            '    Application.DoEvents()
            'End If


            'Return the unioned shape
            Return pUnioningShape
        Catch ex As Exception
            Logging.Log.Error("Error in CreateUnionedShape", ex)
            Throw
        End Try

    End Function
End Class
