Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports System.Windows.Forms

<System.Runtime.InteropServices.ComVisible(False)> _
Public Class frmSavedShapes
    Private m_SelectedShapeType As ESRI.ArcGIS.Geometry.esriGeometryType
    Private m_CallingTool As ERAtoolsSavedShapeTool

    Public Function View(ByRef CallingTool As ERAtoolsSavedShapeTool) As Windows.Forms.DialogResult
        m_CallingTool = CallingTool
        Return Me.ShowDialog()
    End Function

    Private Sub cboShapeType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboShapeType.SelectedIndexChanged
        If cboShapeType.SelectedIndex = -1 Then
            Me.dgvShapes.Rows.Clear()
            Exit Sub
        End If
        Dim pshapeType As String
        Try
            pshapeType = cboShapeType.Text

            'Open the selected Featureclass in the System Workspace
            'Return all records to the DataGrid and PopulateDataGrid
            Select Case pshapeType
                Case "Point"
                    m_SelectedShapeType = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint
                Case "Line"
                    m_SelectedShapeType = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryLine
                Case "Polygon"
                    m_SelectedShapeType = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon

            End Select

            PopulateDataGrid()
            lblStatus.Text = dgvShapes.RowCount & " " & pshapeType & " features available."
        Catch ex As Exception
            Logging.Log.Error("Error in cboShapeType_SelectedIndexChanged", ex)
        End Try
    End Sub

    Private Populating As Boolean = False

    Private Sub PopulateDataGrid()
        Populating = True
        Dim pFeature As ESRI.ArcGIS.Geodatabase.IFeature
        Dim FeatureValues(3) As String

        Dim nNameField As Long
        Dim nDateField As Long

        dgvShapes.Rows.Clear()

        Try
            Dim pFeatureCursor As IFeatureCursor = SystemWorkspace.GetSavedShapes(m_SelectedShapeType)
            If pFeatureCursor Is Nothing Then Exit Sub

            pFeature = pFeatureCursor.NextFeature
            If pFeature Is Nothing Then Exit Sub

            nNameField = pFeature.Fields.FindField("ShapeName")
            nDateField = pFeature.Fields.FindField("DateCreated")
            Dim strName As String, dateCreated As Date
            While Not pFeature Is Nothing
                'New Row in the datagrid
                'First column is shapename
                'Second column is datacreated
                'Third column is OID
                If pFeature.Value(nNameField) Is DBNull.Value Then
                    strName = "<null>"
                    Logging.Log.Warn("Feature with ObjectID " & pFeature.OID & " in " & Me.cboShapeType.Text & " layer has a null value for ShapeName")
                Else
                    strName = pFeature.Value(nNameField)
                End If
                If pFeature.Value(nDateField) Is DBNull.Value Then
                    dateCreated = Date.MinValue
                    Logging.Log.Warn("Feature with ObjectID " & pFeature.OID & " in " & Me.cboShapeType.Text & " layer has a null value for DateCreated")
                Else
                    If IsDate(pFeature.Value(nDateField)) Then
                        dateCreated = pFeature.Value(nDateField)
                    Else
                        Logging.Log.Warn("Feature with ObjectID " & pFeature.OID & " in " & Me.cboShapeType.Text & " layer has an invalid date value for DateCreated")
                        dateCreated = Date.MinValue
                    End If
                End If
                If pFeature.Shape Is Nothing OrElse pFeature.Shape.IsEmpty Then
                    Logging.Log.Warn("Feature with ObjectID " & pFeature.OID & " in " & Me.cboShapeType.Text & " layer has a null shape and will not be displayed in the grid.")
                Else
                    dgvShapes.Rows.Add(New Object() {strName, dateCreated, pFeature.OID})
                End If

                pFeature = pFeatureCursor.NextFeature
            End While
            Me.dgvShapes.Visible = True
            Me.dgvShapes.Enabled = True
        Catch ex As Exception
            Logging.Log.Error("Error in PopulateDataGrid", ex)
            Throw
        End Try
        dgvShapes.ClearSelection()
        Populating = False
    End Sub

    Private Sub frmSavedShapes_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.btnDelete.Enabled = False
        Me.btnSelect.Enabled = False
    End Sub

    Private Sub btnSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelect.Click
        Dim n As Integer = Me.dgvShapes.SelectedRows.Count
        If Me.optIndividual.Checked Then
            If n > My.Settings.WarnReportsLimit And n <= My.Settings.MaxReportsLimit Then
                If MessageBox.Show("Your current selection will generate " & n & " individual reports. Are you sure you want to do this?", "Confirm Selection", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then Exit Sub
            ElseIf n > My.Settings.MaxReportsLimit Then
                MessageBox.Show("Your current selection of " & n & " features exceeds the maximum number of reports that can be generated in one process (" & My.Settings.MaxReportsLimit & "). Please select fewer features.", "Too Many Selected Features", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                Exit Sub
            End If
        Else
            If n > My.Settings.WarnUnionedShapesLimit And n <= My.Settings.MaxUnionedShapesLimit Then
                If MessageBox.Show("Your current selection will attempt to union " & n & " individual features into a single shape. This may take a considerable amount of time. Are you sure you want to do this?", "Confirm Selection", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then Exit Sub
            ElseIf n > My.Settings.MaxUnionedShapesLimit Then
                MessageBox.Show("Your current selection of " & n & " features exceeds the maximum number of shapes that can be unioned (" & My.Settings.MaxUnionedShapesLimit & "). Please select fewer features.", "Too Many Selected Features", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                Exit Sub
            End If
        End If

        Try
            Dim pFeatureCursor As IFeatureCursor = SystemWorkspace.GetSavedShapes(m_SelectedShapeType, GetWhereClause)
            m_CallingTool.ClearShapes()
            If pFeatureCursor Is Nothing Then
                Logging.Log.Error("The Call to SystemWorkspace.GetSavedShapes returned nothing, expected a reference to an instance of IFeatureCursor")
                DialogResult = Windows.Forms.DialogResult.Cancel
            Else
                m_CallingTool.UnionShapes = Me.optTogether.Checked
                m_CallingTool.SetFeatures(pFeatureCursor)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor)
                DialogResult = Windows.Forms.DialogResult.OK
            End If
        Catch ex As Exception
            Logging.Log.Error("Error in btnSelect_Click", ex)
            Throw
        End Try
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        'm_CallingTool.ClearShapes() 'Do not remove shapes from the map on cancel
        Me.Close()
    End Sub


    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        'confirm with messagebox
        Try
            If MessageBox.Show("Are you sure you want to delete the selected features?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then Exit Sub
            SystemWorkspace.DeleteFeatures(m_SelectedShapeType, GetWhereClause)
            m_CallingTool.ClearShapes()
            PopulateDataGrid()
            lblStatus.Text = dgvShapes.RowCount & " " & cboShapeType.Text & " features available."
        Catch ex As Exception
            Logging.Log.Error("Error in btnDelete_Click", ex)
            Throw
        End Try
    End Sub

    Private Function GetWhereClause() As String
        Dim OIDs As New Generic.List(Of String)

        Try
            For Each dgvRow As DataGridViewRow In dgvShapes.SelectedRows
                OIDs.Add(dgvRow.Cells(2).Value)
            Next
            If OIDs.Count > 0 Then
                Return "ObjectID in (" & Join(OIDs.ToArray, ",") & ")"
            Else
                Return ""
            End If
        Catch ex As Exception
            Throw
        End Try
    End Function

    Private Sub dgvShapes_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvShapes.SelectionChanged
        If Populating Then Exit Sub
        Dim n As Integer = dgvShapes.SelectedRows.Count
        btnSelect.Enabled = (n > 0)
        btnDelete.Enabled = (n > 0)
        Me.optIndividual.Enabled = (n > 1)
        Me.optTogether.Enabled = (n > 1)

        lblStatus.Text = dgvShapes.SelectedRows.Count & " " & cboShapeType.Text & " features selected."
    End Sub

    Private Sub DrawSelectedShapes()
        Try
            'clear user-drawn shapes
            m_CallingTool.ClearShapes()
            If (dgvShapes.SelectedRows.Count > 0) Then
                'draw selected shapes
                Dim pFeatureCursor As IFeatureCursor = SystemWorkspace.GetSavedShapes(m_SelectedShapeType, GetWhereClause)
                If pFeatureCursor Is Nothing Then
                    Logging.Log.Warn("The call to SystemWorkspace.GetSavedShapes returned a null reference, rather than an instance of a feature cursor.")
                Else
                    m_CallingTool.DrawShape(ConsolidateSelectedShapes(pFeatureCursor))
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor)
                End If
            End If
        Catch ex As Exception
            Logging.Log.Error("Error in DrawSelectedShapes", ex)
            Throw
        End Try
    End Sub

    Private Function ConsolidateSelectedShapes(ByVal m_Features As IFeatureCursor) As ESRI.ArcGIS.Geometry.IGeometry
        Dim pShape As IGeometry
        Dim pFeature As IFeature
        Dim pBag As IGeometryCollection
        Dim pTopoOp As ITopologicalOperator
        Dim pUnioningShape As IGeometry
        Dim pSpatialReference As ISpatialReference
        Dim pGeometryType As esriGeometryType
        Try
            pBag = New ESRI.ArcGIS.Geometry.GeometryBag
            pFeature = m_Features.NextFeature
            If pFeature Is Nothing Then Return Nothing
            pSpatialReference = pFeature.Shape.SpatialReference
            pGeometryType = pFeature.Shape.GeometryType
            While Not pFeature Is Nothing
                pShape = pFeature.Shape
                pBag.AddGeometry(pShape)
                pFeature = m_Features.NextFeature
            End While

            'Initialize the union shape based on the geometry type
            Select Case pGeometryType
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
                    MsgBox("Unhandled Shape Type " & pGeometryType)
                    Return Nothing
            End Select

            'Construct the union
            pTopoOp = pUnioningShape
            pTopoOp.ConstructUnion(pBag)

            'Return the unioned shape
            pUnioningShape.SpatialReference = pSpatialReference
            Return pUnioningShape
        Catch ex As Exception
            Throw
        End Try
    End Function

    Private Sub DrawButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DrawButton.Click
        DrawSelectedShapes()
    End Sub
End Class