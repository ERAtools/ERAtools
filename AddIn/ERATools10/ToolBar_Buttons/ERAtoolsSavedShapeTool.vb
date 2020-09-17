Imports System.Runtime.InteropServices
Imports System.Drawing
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Geodatabase
Imports System.Windows.Forms

Public NotInheritable Class ERAtoolsSavedShapeTool
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button

    Private m_colFeatures As New Generic.List(Of ESRI.ArcGIS.Geodatabase.IFeature)
    Private m_bUnion As Boolean

    Protected Overrides Sub OnClick()
        Dim m_frmSavedShapes As New frmSavedShapes
        Dim pCount As Int16
        Try
            ' Open frmSavedShapes as Dialog
            ' frmSavedShapes should call SetFeatures to populate m_colFeatures
            m_colFeatures = New Generic.List(Of ESRI.ArcGIS.Geodatabase.IFeature)
            If m_frmSavedShapes.View(Me) = DialogResult.Cancel Then Exit Sub
            'During View, frmSavedShapes populates m_colFeatures
            'After the form returns, code contines from here
            'If features are selected, frmERAToolsGUI is launched and analysis is run

            pCount = m_colFeatures.Count

            If pCount > 0 Then
                Dim m_form As New frmERAToolsGUI
                If pCount = 1 Then
                    'set the name to the ShapeName property of the single saved shape, disable saving
                    m_form.ShapeName = m_FeatureName
                    m_form.AllowNaming = False
                Else 'pcount must be > 1
                    If m_bUnion Then
                        'default name to "Multiple Saved Shapes", allow user to change this, disable saving
                        m_form.AllowNaming = True
                        m_form.ShapeName = "Multiple saved shapes"
                    Else
                        'use the name of the individual features for each individual report, do not allow user to change value, disable saving
                        m_form.AllowNaming = False
                    End If
                End If

                'pass the List of features selected in frmSavedShapes to frmERAToolsGUI
                m_form.Features = m_colFeatures
                If m_bUnion Then
                    m_form.RunMode = frmERAToolsGUI.RunModeEnum.SingleOrMergedSavedOrSelectedFeature
                Else
                    m_form.RunMode = frmERAToolsGUI.RunModeEnum.MultipleSavedOrSelectedFeatures
                End If
                m_form.AllowSaving = False
                m_form.ShowDialog()
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Logging.Log.Error("Error in ERAtoolsSavedShapeTool.OnClick", ex)
        End Try

    End Sub

    Private m_FeatureName As String

    Public Sub SetFeatures(ByRef pFeatureCursor As ESRI.ArcGIS.Geodatabase.IFeatureCursor)
        Dim pFeature As ESRI.ArcGIS.Geodatabase.IFeature
        Try
            'FeatureCursor is passed in from frmSavedShapes
            'Features are put into a List m_colFeatures that can be used for analysis by frmERAToolsGUI
            If pFeatureCursor Is Nothing Then
                MessageBox.Show("No Features Selected from Stored Shapes", "No Features Selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Else
                pFeature = pFeatureCursor.NextFeature
                Dim FeatureCount As Integer = 0
                While Not pFeature Is Nothing
                    m_colFeatures.Add(pFeature)
                    m_FeatureName = Nz(pFeature.Value(pFeature.Fields.FindField("ShapeName")))
                    pFeature = pFeatureCursor.NextFeature
                End While
            End If
        Catch ex As Exception
            Logging.Log.Error("Error setting features in in ERAtoolsSavedShapeTool.SetFeatures", ex)
        End Try
    End Sub

    Public Sub DrawShape(ByVal pShape As IGeometry)
        Dim pClone As IGeometry
        Try
            If pShape IsNot Nothing Then
                'Get SpatialReference from the map
                'Project the shape to the map's spatialreference
                pClone = DirectCast(pShape, ESRI.ArcGIS.esriSystem.IClone).Clone
                pClone.Project(getMapSpatRef())
                MapDrawing.DrawShape(pClone)
            End If
        Catch ex As Exception
            Logging.Log.Error("Error in ERAtoolsSavedShapeTool.DrawShape", ex)
        End Try
    End Sub

    Public Sub ClearShapes()
        MapDrawing.DeleteElements()
    End Sub

    Public WriteOnly Property UnionShapes() As Boolean
        Set(ByVal Value As Boolean)
            m_bUnion = Value
        End Set
    End Property

    Private Function getMapSpatRef() As ESRI.ArcGIS.Geometry.ISpatialReference
        Try
            Return My.ArcMap.Document.FocusMap.SpatialReference
        Catch ex As Exception
            Logging.Log.Error("Error in ERAtoolsSavedShapeTool.getMapSpatRef", ex)
            Throw
        End Try
    End Function

End Class



