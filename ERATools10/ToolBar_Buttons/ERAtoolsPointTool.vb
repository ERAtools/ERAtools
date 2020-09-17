Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.Geometry

Public Class ERAtoolsPointTool
    Inherits ERABaseTool

    Protected Overrides Sub OnMouseDown(ByVal arg As ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs)
        Logging.Log.Debug("ERAtoolsPointTool.OnMouseDown called")
        Dim pRubberPoint As IRubberBand = New RubberPoint
        Dim pShape As IGeometry = pRubberPoint.TrackNew(m_pScreenDisplay, GetSymbol)
        ShowERAForm(pShape)
    End Sub

    Friend Overrides Function GetSymbol() As ISymbol
        Dim pSymbol As ISimpleMarkerSymbol
        Dim pRGBColor As IRgbColor
        pSymbol = New SimpleMarkerSymbol
        pRGBColor = New RgbColor
        pRGBColor.Green = 255
        pSymbol.Style = esriSimpleMarkerStyle.esriSMSDiamond
        pSymbol.Color = pRGBColor
        Return pSymbol
    End Function
End Class