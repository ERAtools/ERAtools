Public Class ERAtoolsClearButton
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button

    Protected Overrides Sub OnClick()
        MapDrawing.DeleteElements()
    End Sub

    Protected Overrides Sub OnUpdate()
        Me.Enabled = MapDrawing.HasElements
    End Sub
End Class
