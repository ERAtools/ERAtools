Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.Geodatabase
Imports System.Windows.Forms

<System.Runtime.InteropServices.ComVisible(False)> _
Friend Class frmSelectedFeature
    Inherits System.Windows.Forms.Form
    Private m_CallingTool As ERAtoolsSelectTool

    Public Function View(ByRef CallingTool As ERAtoolsSelectTool) As DialogResult
        m_CallingTool = CallingTool
        Return Me.ShowDialog()
    End Function

    Private Sub frmSelectedFeature_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
        'this is a workaround to a failure of the AutoScale or AutoSize features of .Net to correctly deal with 
        'a form that is designed at standard 96 DPI being shown on a computer with a DPI of 120. The controls near the 
        'bottom of on non-resizeable forms are partially or entirely outside of the boundary of the form.
        'While setting AutoSize = True also solves this problem, it has the unintended consequence of causing the form
        'to appear shorter on screens when it shouldn't have to resize the form, resulting in an awkward-looking appearance
        'with the controls too close to the bottom of the form. This happens even if AutoSizeMode is set to grow only.
        'If Me.btnOK.Top + Me.btnOK.Height > Me.ClientSize.Height Then
        '    Me.SuspendLayout()

        '    Me.ClientSize = New System.Drawing.Size(Me.ClientSize.Width, Me.btnOK.Top + Me.btnOK.Height + 20)

        '    Me.ResumeLayout(False)
        '    Me.PerformLayout()
        'End If
        Debug.Print(Me.Width & " x " & Me.Height)

        'Read the m_pMxDoc.FocusMap.FeatureSelection
        'IdentifyLayers returns true if there is at least one feature selected
        'since the select command isn't enabled if no features are selected, it should always return true
        'but just in case, we check again.
        If IdentifyLayers() Then
            Me.cboLayer.SelectedIndex = 0 'just to set a default
        Else
            'No features selected. Shouldn't ever happen, but this form won't work without it
            MessageBox.Show("Error in frmSelectedFeature_Load: No features selected.", "No Features Selected Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.cboLayer.Enabled = False
            Me.btnOK.Enabled = False
        End If
    End Sub

    Private Layers As DataTable

    Private Function IdentifyLayers() As Boolean
        'For each feature in the pEnum, get the layer name and add it to the list
        'Count the number of features in each layer
        'returns true if there is at least one selection
        Dim pEnum As IEnumFeature = My.ArcMap.Document.FocusMap.FeatureSelection
        IdentifyLayers = False

        Layers = New DataTable
        Layers.Columns.Add("LayerAlias")
        Layers.Columns.Add("Display")
        Layers.Columns.Add("SelectedFeatureCount")
        Layers.PrimaryKey = New DataColumn() {Layers.Columns("LayerAlias")}

        Dim pFeature As ESRI.ArcGIS.Geodatabase.IFeature = pEnum.Next
        Do While Not pFeature Is Nothing
            IdentifyLayers = True
            Dim key As String = pFeature.Class.AliasName
            If Layers.Rows.Find(key) Is Nothing Then
                Layers.Rows.Add(key, key, 1)
            Else
                Layers.Rows.Find(key)("SelectedFeatureCount") += 1
            End If
            pFeature = pEnum.Next
        Loop

        For Each row As DataRow In Layers.Rows
            row("Display") = row("LayerAlias") & " ( " & row("SelectedFeatureCount") & " selected features)"
        Next

        With cboLayer
            .DisplayMember = "Display"
            .ValueMember = "LayerAlias"
            .DataSource = Layers
        End With

    End Function


    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        'First make sure they're not doing something dumb, like opening 1000 reports.
        Dim r As DataRowView = cboLayer.SelectedItem
        If Me.optIndividual.Checked Then
            If r("SelectedFeatureCount") > My.Settings.WarnReportsLimit And r("SelectedFeatureCount") <= My.Settings.MaxReportsLimit Then
                If MessageBox.Show("Your current selection will generate " & r("SelectedFeatureCount") & " individual reports. Are you sure you want to do this?", "Confirm Selection", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then Exit Sub
            ElseIf r("SelectedFeatureCount") > My.Settings.MaxReportsLimit Then
                MessageBox.Show("Your current selection of " & r("SelectedFeatureCount") & " features exceeds the maximum number of reports that can be generated in one process (" & My.Settings.MaxReportsLimit & "). Please select fewer features.", "Too Many Selected Features", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                Exit Sub
            End If
        Else


            If r("SelectedFeatureCount") > My.Settings.WarnUnionedShapesLimit And r("SelectedFeatureCount") <= My.Settings.MaxUnionedShapesLimit Then
                If MessageBox.Show("Your current selection will attempt to union " & r("SelectedFeatureCount") & " individual features into a single shape. This may take a considerable amount of time. Are you sure you want to do this?", "Confirm Selection", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then Exit Sub
            ElseIf r("SelectedFeatureCount") > My.Settings.MaxUnionedShapesLimit Then
                MessageBox.Show("Your current selection of " & r("SelectedFeatureCount") & " features exceeds the maximum number of shapes that can be unioned (" & My.Settings.MaxUnionedShapesLimit & "). Please select fewer features.", "Too Many Selected Features", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                Exit Sub
            End If

        End If

        Dim pEnum As IEnumFeature = My.ArcMap.Document.FocusMap.FeatureSelection
        'the IEnumFeatureSetup interface allows us to get the actual field values with the feature,
        'otherwise the feature's fields will be null except for the OID and Shape fields.
        Dim pEnumFeatureSetup As IEnumFeatureSetup = pEnum
        pEnumFeatureSetup.AllFields = True

        Dim pFeature As ESRI.ArcGIS.Geodatabase.IFeature = pEnum.Next
        Do While Not pFeature Is Nothing
            If pFeature.Class.AliasName = cboLayer.SelectedValue Then
                m_CallingTool.AddFeature(pFeature)
            End If
            pFeature = pEnum.Next
        Loop

        m_CallingTool.UnionShapes = optTogether.Checked

        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub cboLayer_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboLayer.SelectedIndexChanged
        If cboLayer.SelectedIndex = -1 Then Exit Sub
        If Layers Is Nothing Then Exit Sub
        Dim row As DataRow = Layers.Rows.Find(cboLayer.SelectedValue)
        If row Is Nothing Then Exit Sub
        If row("SelectedFeatureCount") > 1 Then
            Me.optIndividual.Enabled = True
            Me.optTogether.Enabled = True
            Me.optTogether.Checked = True
        Else
            Me.optIndividual.Enabled = False
            Me.optTogether.Enabled = False
        End If
    End Sub

End Class