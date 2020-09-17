<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmERAToolsGUI
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
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.chkDisplay = New System.Windows.Forms.CheckBox()
        Me.cboUnits = New System.Windows.Forms.ComboBox()
        Me.cboShapeNameField = New System.Windows.Forms.ComboBox()
        Me.txtTarget = New System.Windows.Forms.TextBox()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.btnRun = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.chkSaveShape = New System.Windows.Forms.CheckBox()
        Me.txtBuffer = New System.Windows.Forms.TextBox()
        Me.txtShapeName = New System.Windows.Forms.TextBox()
        Me.txtReportTitle = New System.Windows.Forms.TextBox()
        Me.cboReports = New System.Windows.Forms.ComboBox()
        Me.lblTarget = New System.Windows.Forms.Label()
        Me.lblOutput = New System.Windows.Forms.Label()
        Me.lblReports = New System.Windows.Forms.Label()
        Me.lblBuffer = New System.Windows.Forms.Label()
        Me.lblUnits = New System.Windows.Forms.Label()
        Me.cboOutputType = New System.Windows.Forms.ComboBox()
        Me.btnBrowseReports = New System.Windows.Forms.Button()
        Me.ep = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblShapeName = New System.Windows.Forms.Label()
        CType(Me.ep, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'chkDisplay
        '
        Me.chkDisplay.Location = New System.Drawing.Point(475, 74)
        Me.chkDisplay.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.chkDisplay.Name = "chkDisplay"
        Me.chkDisplay.Size = New System.Drawing.Size(100, 23)
        Me.chkDisplay.TabIndex = 9
        Me.chkDisplay.Text = "&Display"
        Me.ToolTip1.SetToolTip(Me.chkDisplay, "Check to display the buffer on the map after the report has been run")
        '
        'cboUnits
        '
        Me.cboUnits.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cboUnits.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cboUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboUnits.Items.AddRange(New Object() {"Feet", "Kilometers", "Meters", "Miles"})
        Me.cboUnits.Location = New System.Drawing.Point(336, 73)
        Me.cboUnits.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.cboUnits.Name = "cboUnits"
        Me.cboUnits.Size = New System.Drawing.Size(129, 24)
        Me.cboUnits.TabIndex = 8
        Me.ToolTip1.SetToolTip(Me.cboUnits, "Units used to define buffer")
        '
        'cboShapeNameField
        '
        Me.cboShapeNameField.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cboShapeNameField.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cboShapeNameField.FormattingEnabled = True
        Me.cboShapeNameField.Location = New System.Drawing.Point(176, 138)
        Me.cboShapeNameField.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.cboShapeNameField.Name = "cboShapeNameField"
        Me.cboShapeNameField.Size = New System.Drawing.Size(289, 24)
        Me.cboShapeNameField.TabIndex = 13
        Me.ToolTip1.SetToolTip(Me.cboShapeNameField, "Select a field that contains values to use as analysis shape names.")
        Me.cboShapeNameField.Visible = False
        '
        'txtTarget
        '
        Me.txtTarget.AccessibleDescription = "Name of output report file."
        Me.txtTarget.BackColor = System.Drawing.SystemColors.ControlLight
        Me.txtTarget.Location = New System.Drawing.Point(176, 106)
        Me.txtTarget.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtTarget.Name = "txtTarget"
        Me.txtTarget.ReadOnly = True
        Me.txtTarget.Size = New System.Drawing.Size(289, 22)
        Me.txtTarget.TabIndex = 99
        Me.txtTarget.TabStop = False
        Me.ToolTip1.SetToolTip(Me.txtTarget, "Use the browse button at right to specify the output report file.")
        '
        'btnBrowse
        '
        Me.btnBrowse.Location = New System.Drawing.Point(475, 103)
        Me.btnBrowse.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(100, 28)
        Me.btnBrowse.TabIndex = 11
        Me.btnBrowse.Text = "Br&owse"
        Me.ToolTip1.SetToolTip(Me.btnBrowse, "Click to specify the output report file.")
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'btnRun
        '
        Me.btnRun.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.btnRun.Location = New System.Drawing.Point(183, 215)
        Me.btnRun.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnRun.Name = "btnRun"
        Me.btnRun.Size = New System.Drawing.Size(100, 28)
        Me.btnRun.TabIndex = 17
        Me.btnRun.Text = "&Run Report"
        Me.ToolTip1.SetToolTip(Me.btnRun, "Click to run the analysis")
        '
        'btnCancel
        '
        Me.btnCancel.AccessibleDescription = "Close this form."
        Me.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.btnCancel.Location = New System.Drawing.Point(308, 215)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 28)
        Me.btnCancel.TabIndex = 18
        Me.btnCancel.Text = "&Cancel"
        Me.ToolTip1.SetToolTip(Me.btnCancel, "Click to close this form without running the analysis")
        '
        'chkSaveShape
        '
        Me.chkSaveShape.AutoSize = True
        Me.chkSaveShape.Checked = True
        Me.chkSaveShape.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSaveShape.Location = New System.Drawing.Point(475, 143)
        Me.chkSaveShape.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.chkSaveShape.Name = "chkSaveShape"
        Me.chkSaveShape.Size = New System.Drawing.Size(107, 21)
        Me.chkSaveShape.TabIndex = 14
        Me.chkSaveShape.Text = "Sa&ve Shape"
        Me.ToolTip1.SetToolTip(Me.chkSaveShape, "Check this box to save a copy of the digitized or selected features in a local ge" & _
                "odatabase")
        Me.chkSaveShape.UseVisualStyleBackColor = True
        '
        'txtBuffer
        '
        Me.txtBuffer.AccessibleDescription = ""
        Me.txtBuffer.Location = New System.Drawing.Point(176, 74)
        Me.txtBuffer.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtBuffer.MaxLength = 0
        Me.txtBuffer.Name = "txtBuffer"
        Me.txtBuffer.Size = New System.Drawing.Size(91, 22)
        Me.txtBuffer.TabIndex = 6
        Me.txtBuffer.Text = "0"
        Me.ToolTip1.SetToolTip(Me.txtBuffer, "Enter a positive number to use in creating a buffer shape around the analysis sha" & _
                "pe. Enter 0 to run the report with no buffer (note that some analysis types requ" & _
                "ire a buffer).")
        '
        'txtShapeName
        '
        Me.txtShapeName.Location = New System.Drawing.Point(176, 139)
        Me.txtShapeName.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtShapeName.Name = "txtShapeName"
        Me.txtShapeName.Size = New System.Drawing.Size(289, 22)
        Me.txtShapeName.TabIndex = 13
        Me.txtShapeName.Text = "<enter shape name>"
        Me.ToolTip1.SetToolTip(Me.txtShapeName, "Enter a name for the analysis shape. The shape will be saved to the local geodata" & _
                "base with this name, and the name will appear on the output report.")
        '
        'txtReportTitle
        '
        Me.txtReportTitle.AccessibleDescription = "Title to be printed at the top of the report."
        Me.txtReportTitle.BackColor = System.Drawing.SystemColors.Window
        Me.txtReportTitle.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtReportTitle.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtReportTitle.Location = New System.Drawing.Point(176, 171)
        Me.txtReportTitle.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtReportTitle.MaxLength = 0
        Me.txtReportTitle.Name = "txtReportTitle"
        Me.txtReportTitle.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtReportTitle.Size = New System.Drawing.Size(289, 22)
        Me.txtReportTitle.TabIndex = 16
        Me.ToolTip1.SetToolTip(Me.txtReportTitle, "Enter a title for the report")
        '
        'cboReports
        '
        Me.cboReports.AccessibleDescription = "File in ERAtools2 System folder with .era extension."
        Me.cboReports.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboReports.Location = New System.Drawing.Point(176, 7)
        Me.cboReports.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.cboReports.Name = "cboReports"
        Me.cboReports.Size = New System.Drawing.Size(289, 24)
        Me.cboReports.TabIndex = 1
        '
        'lblTarget
        '
        Me.lblTarget.AutoSize = True
        Me.lblTarget.Location = New System.Drawing.Point(16, 110)
        Me.lblTarget.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTarget.Name = "lblTarget"
        Me.lblTarget.Size = New System.Drawing.Size(61, 17)
        Me.lblTarget.TabIndex = 10
        Me.lblTarget.Text = "&Save To"
        '
        'lblOutput
        '
        Me.lblOutput.AutoSize = True
        Me.lblOutput.Location = New System.Drawing.Point(16, 44)
        Me.lblOutput.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblOutput.Name = "lblOutput"
        Me.lblOutput.Size = New System.Drawing.Size(142, 17)
        Me.lblOutput.TabIndex = 3
        Me.lblOutput.Text = "Select Output &Format"
        '
        'lblReports
        '
        Me.lblReports.AutoSize = True
        Me.lblReports.Location = New System.Drawing.Point(16, 11)
        Me.lblReports.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblReports.Name = "lblReports"
        Me.lblReports.Size = New System.Drawing.Size(149, 17)
        Me.lblReports.TabIndex = 0
        Me.lblReports.Text = "&Analysis Definition File"
        '
        'lblBuffer
        '
        Me.lblBuffer.AutoSize = True
        Me.lblBuffer.Location = New System.Drawing.Point(16, 78)
        Me.lblBuffer.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblBuffer.Name = "lblBuffer"
        Me.lblBuffer.Size = New System.Drawing.Size(115, 17)
        Me.lblBuffer.TabIndex = 5
        Me.lblBuffer.Text = "Enter &Buffer Size"
        '
        'lblUnits
        '
        Me.lblUnits.AutoSize = True
        Me.lblUnits.Location = New System.Drawing.Point(279, 78)
        Me.lblUnits.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblUnits.Name = "lblUnits"
        Me.lblUnits.Size = New System.Drawing.Size(48, 17)
        Me.lblUnits.TabIndex = 7
        Me.lblUnits.Text = "&Units: "
        '
        'cboOutputType
        '
        Me.cboOutputType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboOutputType.Location = New System.Drawing.Point(176, 41)
        Me.cboOutputType.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.cboOutputType.Name = "cboOutputType"
        Me.cboOutputType.Size = New System.Drawing.Size(289, 24)
        Me.cboOutputType.TabIndex = 4
        '
        'btnBrowseReports
        '
        Me.btnBrowseReports.Location = New System.Drawing.Point(475, 5)
        Me.btnBrowseReports.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnBrowseReports.Name = "btnBrowseReports"
        Me.btnBrowseReports.Size = New System.Drawing.Size(100, 28)
        Me.btnBrowseReports.TabIndex = 2
        Me.btnBrowseReports.Text = "Bro&wse"
        '
        'ep
        '
        Me.ep.ContainerControl = Me
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(16, 175)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(82, 17)
        Me.Label1.TabIndex = 15
        Me.Label1.Text = "Report &Title"
        '
        'lblShapeName
        '
        Me.lblShapeName.AutoSize = True
        Me.lblShapeName.Location = New System.Drawing.Point(16, 143)
        Me.lblShapeName.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblShapeName.Name = "lblShapeName"
        Me.lblShapeName.Size = New System.Drawing.Size(90, 17)
        Me.lblShapeName.TabIndex = 12
        Me.lblShapeName.Text = "Shape &Name"
        '
        'frmERAToolsGUI
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(591, 256)
        Me.Controls.Add(Me.lblShapeName)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtShapeName)
        Me.Controls.Add(Me.chkSaveShape)
        Me.Controls.Add(Me.btnBrowseReports)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnRun)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.cboOutputType)
        Me.Controls.Add(Me.txtReportTitle)
        Me.Controls.Add(Me.txtTarget)
        Me.Controls.Add(Me.cboReports)
        Me.Controls.Add(Me.chkDisplay)
        Me.Controls.Add(Me.txtBuffer)
        Me.Controls.Add(Me.cboUnits)
        Me.Controls.Add(Me.lblTarget)
        Me.Controls.Add(Me.lblOutput)
        Me.Controls.Add(Me.lblReports)
        Me.Controls.Add(Me.lblBuffer)
        Me.Controls.Add(Me.lblUnits)
        Me.Controls.Add(Me.cboShapeNameField)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Location = New System.Drawing.Point(3, 49)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmERAToolsGUI"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ERAtools2"
        CType(Me.ep, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents btnRun As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnBrowseReports As System.Windows.Forms.Button
    Friend WithEvents chkSaveShape As System.Windows.Forms.CheckBox
    Friend WithEvents txtShapeName As System.Windows.Forms.TextBox
    Friend WithEvents cboShapeNameField As System.Windows.Forms.ComboBox
    Friend WithEvents ep As System.Windows.Forms.ErrorProvider
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents txtTarget As System.Windows.Forms.TextBox
    Friend WithEvents cboReports As System.Windows.Forms.ComboBox
    Friend WithEvents chkDisplay As System.Windows.Forms.CheckBox
    Friend WithEvents txtBuffer As System.Windows.Forms.TextBox
    Friend WithEvents cboUnits As System.Windows.Forms.ComboBox
    Friend WithEvents lblTarget As System.Windows.Forms.Label
    Friend WithEvents lblOutput As System.Windows.Forms.Label
    Friend WithEvents lblBuffer As System.Windows.Forms.Label
    Friend WithEvents lblUnits As System.Windows.Forms.Label
    Friend WithEvents cboOutputType As System.Windows.Forms.ComboBox
    Friend WithEvents lblReports As System.Windows.Forms.Label
    Friend WithEvents txtReportTitle As System.Windows.Forms.TextBox
    Friend WithEvents lblShapeName As System.Windows.Forms.Label
#End Region
End Class