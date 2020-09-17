Imports System.Drawing
Imports System.Windows.Forms

Public NotInheritable Class ERAtoolsSelectTool
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button

    Private m_colFeatures As New Generic.List(Of ESRI.ArcGIS.Geodatabase.IFeature)
    Private m_bUnion As Boolean

    ''' <summary>
    ''' Called when user clicks the ERAtoolsSelectTool. Assumes that the user has selected at least one feature from layers loaded in the map. 
    ''' More than one feature may be selected, from more than one layer. If so, frmSelectedFeature is displayed to prompt user to choose which 
    ''' layer to use (only selected feature(s) from a single layer can actually be run), and whether to union the shapes into a single feature
    ''' to run a single report, or whether to run multiple reports for each selected feature.
    ''' </summary>
    Protected Overrides Sub OnClick()

        Dim pCount As Integer
        m_colFeatures.Clear()

        pCount = My.ArcMap.Document.FocusMap.SelectionCount
        If pCount = 0 Then
            MessageBox.Show("Please select features before using this tool", "Nothing Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        ElseIf pCount = 1 Then
            Dim pEnum As ESRI.ArcGIS.Geodatabase.IEnumFeature = My.ArcMap.Document.FocusMap.FeatureSelection
            Dim pFeature As ESRI.ArcGIS.Geodatabase.IFeature = pEnum.Next

            m_bUnion = True 'probably not necessary to do this, but it works, so let's not mess with it
            m_colFeatures.Add(pFeature)
        Else
            Me.AddFeatureWarnings.Clear()
            Dim frmFeatures As New frmSelectedFeature
            If frmFeatures.View(Me) = DialogResult.Cancel Then Exit Sub
            If Me.AddFeatureWarnings.Count > 0 Then
                If MessageBox.Show(Me.AddFeatureWarnings.Count & " warning(s) regarding selected features with null or empty shapes were logged. Do you want to attempt to run the report anyway?", "Warnings Logged", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) = DialogResult.No Then
                    Exit Sub
                End If
            End If
        End If

        'Prepare the Analysis Form
        Dim m_form As New frmERAToolsGUI
        For Each pFeature As ESRI.ArcGIS.Geodatabase.IFeature In m_colFeatures
            m_form.AddFeature(pFeature)
        Next
        If m_bUnion Then
            m_form.RunMode = frmERAToolsGUI.RunModeEnum.SingleOrMergedSavedOrSelectedFeature
        Else
            m_form.RunMode = frmERAToolsGUI.RunModeEnum.MultipleSavedOrSelectedFeatures
        End If
        m_form.ShowDialog()
    End Sub

    Private AddFeatureWarnings As New Generic.List(Of String)

    ''' <summary>
    ''' Used solely by frmSelectedFeature to add features (not just shapes, but the data as well) to the feature collection, as well as the shapes collection
    ''' that will be passed along to frmERAToolsGUI
    ''' </summary>
    ''' <param name="pFeature">A pointer to an IFeature instance.</param>
    Public Sub AddFeature(ByVal pFeature As ESRI.ArcGIS.Geodatabase.IFeature)
        Try
            Dim strWarning As String = ""

            If pFeature.Shape Is Nothing Then
                If pFeature.HasOID Then
                    strWarning = "Attempt to add feature with ObjectID " & pFeature.OID & " failed: shape is nothing"
                Else
                    strWarning = "Attempt to add feature failed: shape is nothing"
                End If
            ElseIf pFeature.Shape.IsEmpty Then
                If pFeature.HasOID Then
                    strWarning = "Attempt to add feature with ObjectID " & pFeature.OID & " failed: shape is empty"
                Else
                    strWarning = "Attempt to add feature failed: shape is empty"
                End If
            End If

            If strWarning <> "" Then
                AddFeatureWarnings.Add(strWarning)
                Logging.Log.Warn(strWarning)
                Exit Sub
            End If

            m_colFeatures.Add(pFeature)
            'Dim pClone As ESRI.ArcGIS.esriSystem.IClone = pFeature.Shape
            'Dim pCloneShape As ESRI.ArcGIS.Geometry.IGeometry = pClone.Clone

            'm_colShapes.AddGeometry(pCloneShape) 'Note that unlike the way this runs when there is just a single feature selected, we don't have to bother setting the spatial reference.
            'It still has the problem of the m_colShapes.geometry(x) having nothing for the spatial reference property, but it doesn't matter because in this instance
            'ERAtools is using the m_colFeatures. m_colShapes is used only for drawing the geometry in any case.
            'seems a bit confusing, but after extensive testing it works, and we shouldn't change it.
        Catch ex As Exception
            MessageBox.Show("Error using selected feature: " & ex.Message, "Error Using Selected Feature", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public WriteOnly Property UnionShapes() As Boolean
        Set(ByVal Value As Boolean)
            m_bUnion = Value
        End Set
    End Property

    Protected Overrides Sub OnUpdate()
        MyBase.Enabled = My.ArcMap.Document.FocusMap.SelectionCount > 0
    End Sub

    'There is no way to do this...
    'Public Overrides ReadOnly Property ToolTip() As String
    '    Get
    '        If Me.Enabled Then
    '            Return "Click to Analyze selected features"
    '        Else
    '            Return "Select features to enable"
    '        End If
    '    End Get
    'End Property

End Class



