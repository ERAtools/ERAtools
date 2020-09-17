Imports System.Runtime.InteropServices
Imports System.Drawing
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI

Public NotInheritable Class ERAtoolsSettings
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button

    Protected Overrides Sub OnClick()
        Dim m_frmSettings As New frmSettings
        m_frmSettings.ShowDialog()
    End Sub
End Class



