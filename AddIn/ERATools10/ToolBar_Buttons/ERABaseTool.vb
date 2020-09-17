Imports ESRI.ArcGIS.ArcMapUI
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.Geometry

<System.Runtime.InteropServices.ComVisible(False)> _
Public MustInherit Class ERABaseTool
    Inherits ESRI.ArcGIS.Desktop.AddIns.Tool

    Friend MustOverride Function GetSymbol() As ISymbol

    Friend Function m_pScreenDisplay() As IScreenDisplay
        Dim m_pMxDoc As IMxDocument = My.ArcMap.Document
        Return m_pMxDoc.ActiveView.ScreenDisplay
    End Function

    Friend Sub ShowERAForm(ByVal pShape As IGeometry)
        Dim m_form As New frmERAToolsGUI
        m_form.Shape = pShape
        m_form.RunMode = frmERAToolsGUI.RunModeEnum.SingleShape
        m_form.ShowDialog()
    End Sub
End Class
