<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmSelectedFeature
#Region "Windows Form Designer generated code "
	<System.Diagnostics.DebuggerNonUserCode()> Public Sub New()
		MyBase.New()
		'This call is required by the Windows Form Designer.
		InitializeComponent()
	End Sub
	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
		If Disposing Then
			If Not components Is Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(Disposing)
	End Sub
	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer
	Public ToolTip1 As System.Windows.Forms.ToolTip
    Public WithEvents optIndividual As System.Windows.Forms.RadioButton
	Public WithEvents optTogether As System.Windows.Forms.RadioButton
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.optIndividual = New System.Windows.Forms.RadioButton
        Me.optTogether = New System.Windows.Forms.RadioButton
        Me.cboLayer = New System.Windows.Forms.ComboBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnOK = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'optIndividual
        '
        Me.optIndividual.BackColor = System.Drawing.SystemColors.Control
        Me.optIndividual.Cursor = System.Windows.Forms.Cursors.Default
        Me.optIndividual.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optIndividual.Location = New System.Drawing.Point(12, 52)
        Me.optIndividual.Name = "optIndividual"
        Me.optIndividual.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.optIndividual.Size = New System.Drawing.Size(285, 18)
        Me.optIndividual.TabIndex = 2
        Me.optIndividual.TabStop = True
        Me.optIndividual.Text = "Create &Separate Reports for Each Feature"
        Me.ToolTip1.SetToolTip(Me.optIndividual, "Choose this option to run multiple reports, one for each selected feature. ")
        Me.optIndividual.UseVisualStyleBackColor = False
        '
        'optTogether
        '
        Me.optTogether.BackColor = System.Drawing.SystemColors.Control
        Me.optTogether.Checked = True
        Me.optTogether.Cursor = System.Windows.Forms.Cursors.Default
        Me.optTogether.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optTogether.Location = New System.Drawing.Point(12, 76)
        Me.optTogether.Name = "optTogether"
        Me.optTogether.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.optTogether.Size = New System.Drawing.Size(285, 18)
        Me.optTogether.TabIndex = 3
        Me.optTogether.TabStop = True
        Me.optTogether.Text = "Create &One Report for All Features"
        Me.ToolTip1.SetToolTip(Me.optTogether, "Choose this option to union all selected features into a single feature, and run " & _
                "one report on the unioned feature.")
        Me.optTogether.UseVisualStyleBackColor = False
        '
        'cboLayer
        '
        Me.cboLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboLayer.FormattingEnabled = True
        Me.cboLayer.Location = New System.Drawing.Point(12, 25)
        Me.cboLayer.Name = "cboLayer"
        Me.cboLayer.Size = New System.Drawing.Size(285, 21)
        Me.cboLayer.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.cboLayer, "Choose the layer that contains the features you want to run reports on.")
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 7)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(133, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Select Features from &Layer"
        '
        'btnOK
        '
        Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnOK.Location = New System.Drawing.Point(78, 101)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 4
        Me.btnOK.Text = "O&K"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(159, 101)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Text = "&Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'frmSelectedFeature
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(311, 138)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.optTogether)
        Me.Controls.Add(Me.optIndividual)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cboLayer)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Location = New System.Drawing.Point(494, 254)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSelectedFeature"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Selected Features"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cboLayer As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
#End Region 
End Class