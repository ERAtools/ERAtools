Imports System.Windows.Forms
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.Geometry

Public Class ERAtoolsPolygonTool
    Inherits ERABaseTool

    Protected Overrides Sub OnMouseDown(ByVal arg As ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs)
        Try
            Dim pRubberPolygon As IRubberBand
            pRubberPolygon = New RubberPolygon
            Dim pShape As IPolygon = pRubberPolygon.TrackNew(m_pScreenDisplay, GetSymbol)
            If pShape Is Nothing Then Exit Sub
            Dim pArea As IArea = pShape
            If pArea.Area <= 0 Then
                'user has digitized counterclockwise
                Logging.Log.Warn("User digitized a counter-clockwise feature. This is typically interpreted by ArcGIS as being an inside ring, or delimiter of an out-polygon.")
                Dim pGeomColl As IGeometryCollection = pShape


                If pGeomColl.GeometryCount = 1 Then
                    Dim pRing As IRing = pGeomColl.Geometry(0)
                    pRing.ReverseOrientation()
                    pArea = pRing
                    If pArea.Area < 0 Then
                        MessageBox.Show("An error occurred digitizing the polygon. Please try again.", "Digitizing Area", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Exit Sub
                    Else
                        Dim geoCol As IGeometryCollection
                        geoCol = New Polygon
                        geoCol.AddGeometry(pRing)
                        'pGeomColl = New GeometryBag
                        'pGeomColl.AddGeometry(pRing)
                        pShape = geoCol
                    End If
                Else
                    Logging.Log.Error("Failed to cast user-digitized polygon to a polygon collection.")
                    MessageBox.Show("An error occurred digitizing the polygon. Please try again.", "Digitizing Area", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                End If
            End If

            pShape.SpatialReference = My.ArcMap.Document.FocusMap.SpatialReference()

            ShowERAForm(pShape)

        Catch ex As Exception
            Logging.Log.Error("Error in tracking the polygon", ex)
            MessageBox.Show("An error occurred in tracking the polygon: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Friend Overrides Function GetSymbol() As ISymbol
        Dim pSymbol As ISimpleFillSymbol
        Dim pRGBColor As IRgbColor

        pSymbol = New SimpleFillSymbol
        pRGBColor = New RgbColor
        pRGBColor.Red = 88
        pRGBColor.Green = 160
        pRGBColor.Blue = 50
        pSymbol.Style = esriSimpleFillStyle.esriSFSSolid
        pSymbol.Color = pRGBColor

        Return pSymbol
    End Function

End Class