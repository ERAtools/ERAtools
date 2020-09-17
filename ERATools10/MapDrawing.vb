Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Display
Imports System.Windows.Forms

''' <summary>
''' Handles the drawing of shapes on the map
''' Originally by Chris Sands/FDEP
''' Modified to speed map drawing by Bill Beers, URS Corp.
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public Class MapDrawing
    Private Sub New()
        'just to shut up FxCop
    End Sub

    ''' <summary>
    ''' Helper function to return an IActiveView interface of the current focus map.
    ''' </summary>
    ''' <value>The map which currently has focus.</value>
    Private Shared ReadOnly Property FocusMap() As IActiveView
        Get
            Return My.ArcMap.Document.FocusMap
        End Get
    End Property

    ''' <summary>
    ''' Gets a simple fill symbol to use for the buffer shape.
    ''' </summary>
    ''' <returns>An ISymbol interface of a SimpleFillSymbol object.</returns>
    Private Shared Function GetBufferSymbol() As ISymbol
        Dim pSymbol As New SimpleFillSymbol
        Dim pColor As New RgbColor
        pColor.Red = 255
        pSymbol.Style = esriSimpleFillStyle.esriSFSForwardDiagonal
        pSymbol.Color = pColor
        Return pSymbol
    End Function


    ''' <summary>
    ''' Gets a symbol for the analysis shape.
    ''' </summary>
    ''' <param name="pShape">A pointer to the analysis shape.</param>
    ''' <returns>An ISymbol interface to a symbol appropriate to the geometry type of the analysis shape.</returns>
    Private Shared Function GetSymbolForShape(ByRef pShape As IGeometry) As ISymbol
        Try
            Dim pColor As New RgbColor
            pColor.Blue = 150
            pColor.Green = 88
            pColor.Red = 30
            Dim pSymbol As ISymbol
            Select Case pShape.GeometryType
                Case esriGeometryType.esriGeometryPoint
                    Dim tempSymbol As New SimpleMarkerSymbol
                    tempSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle
                    tempSymbol.Color = pColor
                    pSymbol = tempSymbol
                Case esriGeometryType.esriGeometryMultipoint
                    Dim tempSymbol As New SimpleMarkerSymbol
                    tempSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle
                    tempSymbol.Color = pColor
                    pSymbol = tempSymbol
                Case esriGeometryType.esriGeometryPolyline
                    Dim tempSymbol As New SimpleLineSymbol
                    tempSymbol.Style = esriSimpleLineStyle.esriSLSSolid
                    tempSymbol.Color = pColor
                    pSymbol = tempSymbol
                Case esriGeometryType.esriGeometryLine
                    Dim tempSymbol As New SimpleLineSymbol
                    tempSymbol.Style = esriSimpleLineStyle.esriSLSSolid
                    tempSymbol.Color = pColor
                    pSymbol = tempSymbol
                Case esriGeometryType.esriGeometryPolygon
                    Dim tempSymbol As New SimpleFillSymbol
                    tempSymbol.Style = esriSimpleFillStyle.esriSFSSolid
                    tempSymbol.Color = pColor
                    pSymbol = tempSymbol
                Case Else
                    Throw New EraException("Invalid Geometry Type")
            End Select
            Return pSymbol
        Catch ex As Exception
            Logging.Log.Error("Error in GetSymbolForShape", ex)
            MessageBox.Show("An error occurred setting the symbol to use for drawing the analysis feature on the map: " & ex.Message, "Error Drawing Features", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Draws the analysis shape, and optionally a buffer around that shape, on the current focus map.
    ''' </summary>
    ''' <param name="pShape">A pointer to the analysis shape.</param>
    ''' <param name="pBufferShape">A pointer to the buffer shape.</param>
    Public Shared Sub DrawShape(ByRef pShape As IGeometry, Optional ByRef pBufferShape As IGeometry = Nothing)
        Dim pDisplay As IDisplay
        Dim pGraphicsContainer As IGraphicsContainer
        Dim pElement As IElement = Nothing

        Dim pMarkerElement As IMarkerElement
        Dim pLineElement As ILineElement
        Dim pPolygonElement As IFillShapeElement
        Dim pBufferElement As IFillShapeElement

        Dim DrawPhase As esriViewDrawPhase = esriViewDrawPhase.esriViewGraphics

        'DeleteElements()

        Try
            pDisplay = My.ArcMap.ThisApplication.Display
            pGraphicsContainer = FocusMap()

            'Draw the analysis shape
            If pShape Is Nothing Then
                Logging.Log.Warn("Analysis Shape is Nothing.")
                Exit Sub
            End If

            Dim pSymbol As ISymbol = GetSymbolForShape(pShape)

            Logging.Log.Debug("Drawing Analysis Shape.")
            pDisplay.SetSymbol(pSymbol)

            pShape.Project(DirectCast(pGraphicsContainer, IMap).SpatialReference)

            Dim pShapeOrBuffer As IGeometry = pShape

            Dim pGeoCol As IGeometryCollection
            Dim iRow As Integer
            Select Case pShape.GeometryType
                Case esriGeometryType.esriGeometryPoint
                    pElement = New MarkerElement
                    pMarkerElement = pElement
                    pMarkerElement.Symbol = pSymbol
                    pElement.Geometry = pShape
                    pGraphicsContainer.AddElement(pElement, 0)
                    pElement.Activate(pDisplay)
                    pElement.Draw(pDisplay, Nothing)
                Case esriGeometryType.esriGeometryMultipoint
                    pGeoCol = pShape
                    For iRow = 0 To pGeoCol.GeometryCount - 1
                        pElement = New MarkerElement
                        pMarkerElement = pElement
                        pMarkerElement.Symbol = pSymbol
                        pElement.Geometry = pGeoCol.Geometry(iRow)
                        pGraphicsContainer.AddElement(pElement, 0)
                        pElement.Activate(pDisplay)
                        DirectCast(pElement, IElementProperties).Name = "ERAtoolsGraphicElement"
                        pElement.Draw(pDisplay, Nothing)
                    Next iRow
                Case esriGeometryType.esriGeometryPolyline
                    pElement = New LineElement
                    pLineElement = pElement
                    pLineElement.Symbol = pSymbol
                    pElement.Geometry = pShape
                    pGraphicsContainer.AddElement(pElement, 0)
                    pDisplay.DrawPolyline(pShape)
                Case esriGeometryType.esriGeometryLine
                    pElement = New LineElement
                    pLineElement = pElement
                    pLineElement.Symbol = pSymbol
                    pElement.Geometry = pShape
                    pGraphicsContainer.AddElement(pElement, 0)
                    pDisplay.DrawPolyline(pShape)
                Case esriGeometryType.esriGeometryPolygon
                    pElement = New PolygonElement
                    pPolygonElement = pElement
                    pPolygonElement.Symbol = pSymbol
                    pElement.Geometry = pShape
                    pGraphicsContainer.AddElement(pElement, 0)
                    pDisplay.DrawPolygon(pShape)
                Case Else
                    'Do nothing
            End Select

            If pElement IsNot Nothing Then DirectCast(pElement, IElementProperties).Name = "ERAtoolsGraphicElement"

            'if pDisplay buffer flag is false, do not Display
            If pBufferShape IsNot Nothing AndAlso Not pBufferShape.IsEmpty Then
                Logging.Log.Debug("Drawing Buffer Shape.")
                pBufferShape.Project(DirectCast(pGraphicsContainer, IMap).SpatialReference)

                pElement = New PolygonElement
                DirectCast(pElement, IElementProperties).Name = "ERAtoolsGraphicElement"

                Dim pBufferSymbol As ISymbol = GetBufferSymbol()
                pBufferElement = pElement
                pBufferElement.Symbol = pBufferSymbol

                pElement.Geometry = pBufferShape
                pGraphicsContainer.AddElement(pElement, 0)

                pDisplay.SetSymbol(pBufferSymbol)
                pDisplay.DrawPolygon(pBufferShape)
                pShapeOrBuffer = pBufferShape
            End If

            'makes sure the map will contain the shape to be drawn
            'especially helpful when displaying the buffer
            Dim mapExtent As IGeometry
            Dim shapeCompare As IRelationalOperator
            mapExtent = FocusMap.Extent
            shapeCompare = mapExtent
            'If mapExtent.SpatialReference IsNot pShape.SpatialReference Then
            '    pShape.Project(mapExtent.SpatialReference)
            'End If

            'If the current map does not entirely contain the shape to draw, recenter and/or expand the map to display it
            If Not shapeCompare.Contains(pShapeOrBuffer) Then
                Dim pEnvelope As IEnvelope = FocusMap.Extent

                Select Case pShapeOrBuffer.GeometryType
                    Case esriGeometryType.esriGeometryMultipoint
                        With DirectCast(pShapeOrBuffer, IGeometryCollection)
                            If .GeometryCount = 1 Then
                                pEnvelope.CenterAt(.Geometry(0))
                            Else
                                pEnvelope = pShape.Envelope
                                pEnvelope.Expand(1.1, 1.1, True)
                            End If
                        End With
                    Case esriGeometryType.esriGeometryPoint
                        pEnvelope.CenterAt(pShapeOrBuffer)
                    Case Else
                        pEnvelope = pShapeOrBuffer.Envelope
                        pEnvelope.Expand(1.1, 1.1, True)
                End Select
                FocusMap.Extent = pEnvelope

                'If Not DirectCast(FocusMap.Extent, IRelationalOperator).Contains(pShape) Then
                DrawPhase = esriViewDrawPhase.esriViewGeography Or esriViewDrawPhase.esriViewGeoSelection Or esriViewDrawPhase.esriViewGraphics Or esriViewDrawPhase.esriViewGraphicSelection
            End If

            'instead of
            'm_pMxDoc.ActiveView.Refresh()
            'the following is much faster:
            FocusMap.PartialRefresh(DrawPhase, Nothing, Nothing)
        Catch ex As Exception
            Logging.Log.Error("Error in DrawShapes", ex)
            MessageBox.Show("An error occurred drawing the analysis feature on the map: " & ex.Message, "Error Drawing Features", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'not critical, so just inform user and keep going.
        End Try
    End Sub

    ''' <summary>
    ''' Deletes any graphic elements from the focus map that were created by this class.
    ''' </summary>
    Public Shared Sub DeleteElements()
        Dim pGC As IGraphicsContainer = FocusMap()
        pGC.Reset()
        Dim pElement As IElement
        pElement = pGC.Next
        Do Until pElement Is Nothing
            If NameIs(pElement, "ERAtoolsGraphicElement") Then
                pGC.DeleteElement(pElement)
            End If
            pElement = pGC.Next
        Loop
        FocusMap.Refresh()
    End Sub

    ''' <summary>
    ''' Determines whether the current focus map has any graphic elements created by this class.
    ''' </summary>
    ''' <returns>
    ''' <c>true</c> if the current focus map has graphic elements created by this class; otherwise, <c>false</c>.
    ''' </returns>
    Public Shared Function HasElements() As Boolean
        Dim pGC As IGraphicsContainer = FocusMap()
        pGC.Reset()
        Dim pElement As IElement = pGC.Next
        While pElement IsNot Nothing
            If NameIs(pElement, "ERAtoolsGraphicElement") Then
                Return True
            End If
            pElement = pGC.Next
        End While
        Return False
    End Function

    ''' <summary>
    ''' Determines whether the referenced graphic element properties has the name specified in sName.
    ''' </summary>
    ''' <param name="pEProps">An interface to element properties to be tested.</param>
    ''' <param name="sName">The name to check for.</param>
    ''' <returns></returns>
    Public Shared Function NameIs(ByVal pEProps As IElementProperties, ByVal sName As String) As Boolean
        Return UCase(pEProps.Name) = UCase(sName)
    End Function

End Class
