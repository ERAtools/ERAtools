<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ERATable
    Inherits ERAtools2Admin.ERAControl

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
        Dim Label1 As System.Windows.Forms.Label
        Dim Label3 As System.Windows.Forms.Label
        Dim Label4 As System.Windows.Forms.Label
        Dim Label6 As System.Windows.Forms.Label
        Me.lblKeyFields = New System.Windows.Forms.Label
        Me.lblSortFields = New System.Windows.Forms.Label
        Me.txtName = New System.Windows.Forms.TextBox
        Me.ddlWorkspace = New System.Windows.Forms.ComboBox
        Me.ddlLayer = New System.Windows.Forms.ComboBox
        Me.txtWhereExpression = New System.Windows.Forms.TextBox
        Me.BrowseButton = New System.Windows.Forms.Button
        Me.QueryBuilderButton = New System.Windows.Forms.Button
        Me.gvKeyFields = New System.Windows.Forms.DataGridView
        Me.ddlParentKey = New System.Windows.Forms.DataGridViewComboBoxColumn
        Me.ddlChildKey = New System.Windows.Forms.DataGridViewComboBoxColumn
        Me.cbShowHeaders = New System.Windows.Forms.CheckBox
        Me.cbInline = New System.Windows.Forms.CheckBox
        Me.gvSortFields = New System.Windows.Forms.DataGridView
        Me.colSortFieldName = New System.Windows.Forms.DataGridViewComboBoxColumn
        Me.colDirection = New System.Windows.Forms.DataGridViewComboBoxColumn
        Me.cbSummarization = New System.Windows.Forms.CheckBox
        Me.rbAttributeJoin = New System.Windows.Forms.RadioButton
        Me.rbSpatialJoin = New System.Windows.Forms.RadioButton
        Me.cbInnerJoin = New System.Windows.Forms.CheckBox
        Label1 = New System.Windows.Forms.Label
        Label3 = New System.Windows.Forms.Label
        Label4 = New System.Windows.Forms.Label
        Label6 = New System.Windows.Forms.Label
        CType(Me.myErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gvKeyFields, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gvSortFields, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Label1.AutoSize = True
        Label1.Location = New System.Drawing.Point(3, 6)
        Label1.Name = "Label1"
        Label1.Size = New System.Drawing.Size(35, 13)
        Label1.TabIndex = 0
        Label1.Text = "Name"
        '
        'Label3
        '
        Label3.AutoSize = True
        Label3.Location = New System.Drawing.Point(3, 33)
        Label3.Name = "Label3"
        Label3.Size = New System.Drawing.Size(62, 13)
        Label3.TabIndex = 2
        Label3.Text = "Workspace"
        '
        'Label4
        '
        Label4.AutoSize = True
        Label4.Location = New System.Drawing.Point(3, 60)
        Label4.Name = "Label4"
        Label4.Size = New System.Drawing.Size(34, 13)
        Label4.TabIndex = 5
        Label4.Text = "Table"
        '
        'Label6
        '
        Label6.AutoSize = True
        Label6.Location = New System.Drawing.Point(3, 156)
        Label6.Name = "Label6"
        Label6.Size = New System.Drawing.Size(93, 13)
        Label6.TabIndex = 13
        Label6.Text = "Where Expression"
        '
        'lblKeyFields
        '
        Me.lblKeyFields.AutoSize = True
        Me.lblKeyFields.Location = New System.Drawing.Point(3, 240)
        Me.lblKeyFields.Name = "lblKeyFields"
        Me.lblKeyFields.Size = New System.Drawing.Size(55, 13)
        Me.lblKeyFields.TabIndex = 16
        Me.lblKeyFields.Text = "Key Fields"
        '
        'lblSortFields
        '
        Me.lblSortFields.AutoSize = True
        Me.lblSortFields.Location = New System.Drawing.Point(3, 365)
        Me.lblSortFields.Name = "lblSortFields"
        Me.lblSortFields.Size = New System.Drawing.Size(56, 13)
        Me.lblSortFields.TabIndex = 18
        Me.lblSortFields.Text = "Sort Fields"
        '
        'txtName
        '
        Me.txtName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtName.Location = New System.Drawing.Point(95, 3)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(396, 20)
        Me.txtName.TabIndex = 1
        '
        'ddlWorkspace
        '
        Me.ddlWorkspace.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ddlWorkspace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlWorkspace.FormattingEnabled = True
        Me.ddlWorkspace.Location = New System.Drawing.Point(95, 30)
        Me.ddlWorkspace.Name = "ddlWorkspace"
        Me.ddlWorkspace.Size = New System.Drawing.Size(339, 21)
        Me.ddlWorkspace.Sorted = True
        Me.ddlWorkspace.TabIndex = 3
        '
        'ddlLayer
        '
        Me.ddlLayer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ddlLayer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.ddlLayer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.ddlLayer.FormattingEnabled = True
        Me.ddlLayer.Location = New System.Drawing.Point(95, 57)
        Me.ddlLayer.Name = "ddlLayer"
        Me.ddlLayer.Size = New System.Drawing.Size(396, 21)
        Me.ddlLayer.Sorted = True
        Me.ddlLayer.TabIndex = 6
        '
        'txtWhereExpression
        '
        Me.txtWhereExpression.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtWhereExpression.Location = New System.Drawing.Point(95, 153)
        Me.txtWhereExpression.Multiline = True
        Me.txtWhereExpression.Name = "txtWhereExpression"
        Me.txtWhereExpression.Size = New System.Drawing.Size(396, 72)
        Me.txtWhereExpression.TabIndex = 15
        '
        'BrowseButton
        '
        Me.BrowseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BrowseButton.Location = New System.Drawing.Point(440, 28)
        Me.BrowseButton.Name = "BrowseButton"
        Me.BrowseButton.Size = New System.Drawing.Size(51, 23)
        Me.BrowseButton.TabIndex = 4
        Me.BrowseButton.Text = "Browse"
        Me.BrowseButton.UseVisualStyleBackColor = True
        '
        'QueryBuilderButton
        '
        Me.QueryBuilderButton.Location = New System.Drawing.Point(6, 172)
        Me.QueryBuilderButton.Name = "QueryBuilderButton"
        Me.QueryBuilderButton.Size = New System.Drawing.Size(83, 23)
        Me.QueryBuilderButton.TabIndex = 14
        Me.QueryBuilderButton.Text = "Query Builder"
        Me.QueryBuilderButton.UseVisualStyleBackColor = True
        '
        'gvKeyFields
        '
        Me.gvKeyFields.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gvKeyFields.BackgroundColor = System.Drawing.SystemColors.Control
        Me.gvKeyFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.gvKeyFields.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ddlParentKey, Me.ddlChildKey})
        Me.gvKeyFields.Location = New System.Drawing.Point(95, 231)
        Me.gvKeyFields.MultiSelect = False
        Me.gvKeyFields.Name = "gvKeyFields"
        Me.gvKeyFields.Size = New System.Drawing.Size(396, 119)
        Me.gvKeyFields.TabIndex = 17
        '
        'ddlParentKey
        '
        Me.ddlParentKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.ddlParentKey.HeaderText = "Parent Key"
        Me.ddlParentKey.Name = "ddlParentKey"
        '
        'ddlChildKey
        '
        Me.ddlChildKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.ddlChildKey.HeaderText = "Child Key"
        Me.ddlChildKey.Name = "ddlChildKey"
        Me.ddlChildKey.Sorted = True
        '
        'cbShowHeaders
        '
        Me.cbShowHeaders.AutoSize = True
        Me.cbShowHeaders.Location = New System.Drawing.Point(95, 84)
        Me.cbShowHeaders.Name = "cbShowHeaders"
        Me.cbShowHeaders.Size = New System.Drawing.Size(96, 17)
        Me.cbShowHeaders.TabIndex = 7
        Me.cbShowHeaders.Text = "Show Headers"
        Me.cbShowHeaders.UseVisualStyleBackColor = True
        '
        'cbInline
        '
        Me.cbInline.AutoSize = True
        Me.cbInline.Location = New System.Drawing.Point(197, 84)
        Me.cbInline.Name = "cbInline"
        Me.cbInline.Size = New System.Drawing.Size(147, 17)
        Me.cbInline.TabIndex = 8
        Me.cbInline.Text = "Display In-line with Parent"
        Me.cbInline.UseVisualStyleBackColor = True
        '
        'gvSortFields
        '
        Me.gvSortFields.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gvSortFields.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.gvSortFields.BackgroundColor = System.Drawing.SystemColors.Control
        Me.gvSortFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.gvSortFields.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colSortFieldName, Me.colDirection})
        Me.gvSortFields.Location = New System.Drawing.Point(95, 356)
        Me.gvSortFields.Name = "gvSortFields"
        Me.gvSortFields.Size = New System.Drawing.Size(396, 146)
        Me.gvSortFields.TabIndex = 19
        '
        'colSortFieldName
        '
        Me.colSortFieldName.HeaderText = "Field Name"
        Me.colSortFieldName.Name = "colSortFieldName"
        '
        'colDirection
        '
        Me.colDirection.HeaderText = "Direction"
        Me.colDirection.Items.AddRange(New Object() {"Ascending", "Descending"})
        Me.colDirection.Name = "colDirection"
        '
        'cbSummarization
        '
        Me.cbSummarization.AutoSize = True
        Me.cbSummarization.Location = New System.Drawing.Point(95, 107)
        Me.cbSummarization.Name = "cbSummarization"
        Me.cbSummarization.Size = New System.Drawing.Size(253, 17)
        Me.cbSummarization.TabIndex = 11
        Me.cbSummarization.Text = "Enable Data Grouping/Summarization Functions"
        Me.cbSummarization.UseVisualStyleBackColor = True
        '
        'rbAttributeJoin
        '
        Me.rbAttributeJoin.AutoSize = True
        Me.rbAttributeJoin.Checked = True
        Me.rbAttributeJoin.Enabled = False
        Me.rbAttributeJoin.Location = New System.Drawing.Point(377, 83)
        Me.rbAttributeJoin.Name = "rbAttributeJoin"
        Me.rbAttributeJoin.Size = New System.Drawing.Size(86, 17)
        Me.rbAttributeJoin.TabIndex = 9
        Me.rbAttributeJoin.TabStop = True
        Me.rbAttributeJoin.Text = "Attribute Join"
        Me.rbAttributeJoin.UseVisualStyleBackColor = True
        '
        'rbSpatialJoin
        '
        Me.rbSpatialJoin.AutoSize = True
        Me.rbSpatialJoin.Enabled = False
        Me.rbSpatialJoin.Location = New System.Drawing.Point(377, 106)
        Me.rbSpatialJoin.Name = "rbSpatialJoin"
        Me.rbSpatialJoin.Size = New System.Drawing.Size(79, 17)
        Me.rbSpatialJoin.TabIndex = 10
        Me.rbSpatialJoin.Text = "Spatial Join"
        Me.rbSpatialJoin.UseVisualStyleBackColor = True
        '
        'cbInnerJoin
        '
        Me.cbInnerJoin.AutoSize = True
        Me.cbInnerJoin.Location = New System.Drawing.Point(95, 130)
        Me.cbInnerJoin.Name = "cbInnerJoin"
        Me.cbInnerJoin.Size = New System.Drawing.Size(211, 17)
        Me.cbInnerJoin.TabIndex = 12
        Me.cbInnerJoin.Text = "Filter {0} to only show matching records"
        Me.cbInnerJoin.UseVisualStyleBackColor = True
        '
        'ERATable
        '
        Me.Controls.Add(Me.cbInnerJoin)
        Me.Controls.Add(Me.rbSpatialJoin)
        Me.Controls.Add(Me.rbAttributeJoin)
        Me.Controls.Add(Me.cbSummarization)
        Me.Controls.Add(Me.lblSortFields)
        Me.Controls.Add(Me.gvSortFields)
        Me.Controls.Add(Me.cbInline)
        Me.Controls.Add(Me.cbShowHeaders)
        Me.Controls.Add(Me.gvKeyFields)
        Me.Controls.Add(Me.QueryBuilderButton)
        Me.Controls.Add(Me.BrowseButton)
        Me.Controls.Add(Me.txtWhereExpression)
        Me.Controls.Add(Label6)
        Me.Controls.Add(Me.lblKeyFields)
        Me.Controls.Add(Label4)
        Me.Controls.Add(Label3)
        Me.Controls.Add(Me.ddlLayer)
        Me.Controls.Add(Me.ddlWorkspace)
        Me.Controls.Add(Label1)
        Me.Controls.Add(Me.txtName)
        Me.Name = "ERATable"
        Me.Size = New System.Drawing.Size(516, 505)
        CType(Me.myErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gvKeyFields, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gvSortFields, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtName As System.Windows.Forms.TextBox
    Friend WithEvents ddlWorkspace As System.Windows.Forms.ComboBox
    Friend WithEvents ddlLayer As System.Windows.Forms.ComboBox
    Friend WithEvents txtWhereExpression As System.Windows.Forms.TextBox
    Friend WithEvents BrowseButton As System.Windows.Forms.Button
    Friend WithEvents QueryBuilderButton As System.Windows.Forms.Button
    Friend WithEvents gvKeyFields As System.Windows.Forms.DataGridView
    Friend WithEvents ddlParentKey As System.Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents ddlChildKey As System.Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents cbInline As System.Windows.Forms.CheckBox
    Friend WithEvents cbShowHeaders As System.Windows.Forms.CheckBox
    Friend WithEvents gvSortFields As System.Windows.Forms.DataGridView
    Friend WithEvents colSortFieldName As System.Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents colDirection As System.Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents cbSummarization As System.Windows.Forms.CheckBox
    Friend WithEvents rbAttributeJoin As System.Windows.Forms.RadioButton
    Friend WithEvents rbSpatialJoin As System.Windows.Forms.RadioButton
    Private WithEvents lblSortFields As System.Windows.Forms.Label
    Friend WithEvents cbInnerJoin As System.Windows.Forms.CheckBox
    Private WithEvents lblKeyFields As System.Windows.Forms.Label

End Class
