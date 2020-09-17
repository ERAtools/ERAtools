<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSavedShapes
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.cboShapeType = New System.Windows.Forms.ComboBox
        Me.lblShapeType = New System.Windows.Forms.Label
        Me.dgvShapes = New System.Windows.Forms.DataGridView
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnSelect = New System.Windows.Forms.Button
        Me.btnDelete = New System.Windows.Forms.Button
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.optTogether = New System.Windows.Forms.RadioButton
        Me.optIndividual = New System.Windows.Forms.RadioButton
        Me.DrawButton = New System.Windows.Forms.Button
        Me.lblStatus = New System.Windows.Forms.Label
        Me.ShapeName = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DateCreated = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.OID = New System.Windows.Forms.DataGridViewTextBoxColumn
        CType(Me.dgvShapes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cboShapeType
        '
        Me.cboShapeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboShapeType.FormattingEnabled = True
        Me.cboShapeType.Items.AddRange(New Object() {"Point", "Line", "Polygon"})
        Me.cboShapeType.Location = New System.Drawing.Point(83, 12)
        Me.cboShapeType.Name = "cboShapeType"
        Me.cboShapeType.Size = New System.Drawing.Size(119, 21)
        Me.cboShapeType.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.cboShapeType, "Select Point, Line, or Polygon.")
        '
        'lblShapeType
        '
        Me.lblShapeType.AutoSize = True
        Me.lblShapeType.Location = New System.Drawing.Point(9, 15)
        Me.lblShapeType.Name = "lblShapeType"
        Me.lblShapeType.Size = New System.Drawing.Size(65, 13)
        Me.lblShapeType.TabIndex = 0
        Me.lblShapeType.Text = "Shape &Type"
        '
        'dgvShapes
        '
        Me.dgvShapes.AllowUserToAddRows = False
        Me.dgvShapes.AllowUserToDeleteRows = False
        Me.dgvShapes.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvShapes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvShapes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvShapes.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ShapeName, Me.DateCreated, Me.OID})
        Me.dgvShapes.Location = New System.Drawing.Point(12, 39)
        Me.dgvShapes.Name = "dgvShapes"
        Me.dgvShapes.RowHeadersWidth = 30
        Me.dgvShapes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvShapes.Size = New System.Drawing.Size(393, 246)
        Me.dgvShapes.TabIndex = 2
        '
        'btnCancel
        '
        Me.btnCancel.AccessibleDescription = "Close the Saved Shapes form."
        Me.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(253, 339)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(77, 23)
        Me.btnCancel.TabIndex = 7
        Me.btnCancel.Text = "&Cancel"
        Me.ToolTip1.SetToolTip(Me.btnCancel, "Close this form.")
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnSelect
        '
        Me.btnSelect.AccessibleDescription = "Select shapes for analysis."
        Me.btnSelect.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnSelect.Location = New System.Drawing.Point(87, 339)
        Me.btnSelect.Name = "btnSelect"
        Me.btnSelect.Size = New System.Drawing.Size(77, 23)
        Me.btnSelect.TabIndex = 5
        Me.btnSelect.Text = "&Select"
        Me.ToolTip1.SetToolTip(Me.btnSelect, "Run analysis on selected shapes.")
        Me.btnSelect.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.AccessibleDescription = "Delete Selected Features"
        Me.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnDelete.Location = New System.Drawing.Point(170, 339)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(77, 23)
        Me.btnDelete.TabIndex = 6
        Me.btnDelete.Text = "&Delete"
        Me.ToolTip1.SetToolTip(Me.btnDelete, "Delete saved shapes from the scratch workspace.")
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'optTogether
        '
        Me.optTogether.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.optTogether.BackColor = System.Drawing.SystemColors.Control
        Me.optTogether.Checked = True
        Me.optTogether.Cursor = System.Windows.Forms.Cursors.Default
        Me.optTogether.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optTogether.Location = New System.Drawing.Point(12, 315)
        Me.optTogether.Name = "optTogether"
        Me.optTogether.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.optTogether.Size = New System.Drawing.Size(285, 18)
        Me.optTogether.TabIndex = 4
        Me.optTogether.TabStop = True
        Me.optTogether.Text = "Create &One Report for All Features"
        Me.ToolTip1.SetToolTip(Me.optTogether, "Choose this option to union all selected features into a single feature, and run " & _
                "one report on the unioned feature.")
        Me.optTogether.UseVisualStyleBackColor = False
        '
        'optIndividual
        '
        Me.optIndividual.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.optIndividual.BackColor = System.Drawing.SystemColors.Control
        Me.optIndividual.Cursor = System.Windows.Forms.Cursors.Default
        Me.optIndividual.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optIndividual.Location = New System.Drawing.Point(12, 291)
        Me.optIndividual.Name = "optIndividual"
        Me.optIndividual.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.optIndividual.Size = New System.Drawing.Size(285, 18)
        Me.optIndividual.TabIndex = 3
        Me.optIndividual.TabStop = True
        Me.optIndividual.Text = "Create Separate Reports for &Each Feature"
        Me.ToolTip1.SetToolTip(Me.optIndividual, "Choose this option to run multiple reports, one for each selected feature. ")
        Me.optIndividual.UseVisualStyleBackColor = False
        '
        'DrawButton
        '
        Me.DrawButton.AccessibleDescription = ""
        Me.DrawButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DrawButton.Location = New System.Drawing.Point(328, 291)
        Me.DrawButton.Name = "DrawButton"
        Me.DrawButton.Size = New System.Drawing.Size(77, 23)
        Me.DrawButton.TabIndex = 9
        Me.DrawButton.Text = "&Preview"
        Me.ToolTip1.SetToolTip(Me.DrawButton, "Click to preview the select shapes on the map")
        Me.DrawButton.UseVisualStyleBackColor = True
        '
        'lblStatus
        '
        Me.lblStatus.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(9, 371)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(110, 13)
        Me.lblStatus.TabIndex = 8
        Me.lblStatus.Text = "Select Saved Shapes"
        '
        'ShapeName
        '
        Me.ShapeName.HeaderText = "Shape Name"
        Me.ShapeName.Name = "ShapeName"
        Me.ShapeName.ReadOnly = True
        '
        'DateCreated
        '
        DataGridViewCellStyle1.Format = "g"
        DataGridViewCellStyle1.NullValue = Nothing
        Me.DateCreated.DefaultCellStyle = DataGridViewCellStyle1
        Me.DateCreated.HeaderText = "Date Created"
        Me.DateCreated.Name = "DateCreated"
        Me.DateCreated.ReadOnly = True
        '
        'OID
        '
        Me.OID.HeaderText = "OID"
        Me.OID.Name = "OID"
        Me.OID.ReadOnly = True
        Me.OID.Visible = False
        '
        'frmSavedShapes
        '
        Me.AcceptButton = Me.btnSelect
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(417, 393)
        Me.Controls.Add(Me.DrawButton)
        Me.Controls.Add(Me.optTogether)
        Me.Controls.Add(Me.optIndividual)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.btnSelect)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.dgvShapes)
        Me.Controls.Add(Me.lblShapeType)
        Me.Controls.Add(Me.cboShapeType)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(270, 277)
        Me.Name = "frmSavedShapes"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "Saved Shapes"
        CType(Me.dgvShapes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cboShapeType As System.Windows.Forms.ComboBox
    Friend WithEvents lblShapeType As System.Windows.Forms.Label
    Friend WithEvents dgvShapes As System.Windows.Forms.DataGridView
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnSelect As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Public WithEvents optTogether As System.Windows.Forms.RadioButton
    Public WithEvents optIndividual As System.Windows.Forms.RadioButton
    Friend WithEvents DrawButton As System.Windows.Forms.Button
    Friend WithEvents ShapeName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DateCreated As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents OID As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
