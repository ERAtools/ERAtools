Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Geometry

Public Class ERAtoolsPolylineTool
    Inherits ERABaseTool

    Protected Overrides Sub OnMouseDown(ByVal arg As ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs)
        Dim pRubberLine As IRubberBand = New RubberLine
        Dim pShape As IGeometry = pRubberLine.TrackNew(m_pScreenDisplay, GetSymbol)
        ShowERAForm(pShape)
    End Sub

    Friend Overrides Function GetSymbol() As ISymbol
        Dim pSymbol As ISimpleLineSymbol
        Dim pRGBColor As IRgbColor
        pSymbol = New SimpleLineSymbol
        pRGBColor = New RgbColor
        pRGBColor.Red = 150
        pRGBColor.Green = 150
        pSymbol.Style = esriSimpleLineStyle.esriSLSSolid
        pSymbol.Color = pRGBColor
        Return pSymbol
    End Function
End Class
