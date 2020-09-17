<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ERALayer
    Inherits ERAControl

    'UserControl overrides dispose to clean up the component list.
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
        Dim NameLabel As System.Windows.Forms.Label
        Dim Label2 As System.Windows.Forms.Label
        Dim Label3 As System.Windows.Forms.Label
        Dim Label4 As System.Windows.Forms.Label
        Dim Label1 As System.Windows.Forms.Label
        Dim Label5 As System.Windows.Forms.Label
        Me.txtName = New System.Windows.Forms.TextBox
        Me.BrowseButton = New System.Windows.Forms.Button
        Me.ddlAnalysisType = New System.Windows.Forms.ComboBox
        Me.txtWhereExpression = New System.Windows.Forms.TextBox
        Me.ddlWorkspace = New System.Windows.Forms.ComboBox
        Me.ddlLayer = New System.Windows.Forms.ComboBox
        Me.QueryBuilderButton = New System.Windows.Forms.Button
        Me.cbShowHeaders = New System.Windows.Forms.CheckBox
        Me.gvSortFields = New System.Windows.Forms.DataGridView
        Me.colSortFieldName = New System.Windows.Forms.DataGridViewComboBoxColumn
        Me.colDirection = New System.Windows.Forms.DataGridViewComboBoxColumn
        Me.cbSummarization = New System.Windows.Forms.CheckBox
        NameLabel = New System.Windows.Forms.Label
        Label2 = New System.Windows.Forms.Label
        Label3 = New System.Windows.Forms.Label
        Label4 = New System.Windows.Forms.Label
        Label1 = New System.Windows.Forms.Label
        Label5 = New System.Windows.Forms.Label
        CType(Me.myErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gvSortFields, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'NameLabel
        '
        NameLabel.AutoSize = True
        NameLabel.Location = New System.Drawing.Point(3, 6)
        NameLabel.Name = "NameLabel"
        NameLabel.Size = New System.Drawing.Size(72, 13)
        NameLabel.TabIndex = 10
        NameLabel.Text = "Display Name"
        '
        'Label2
        '
        Label2.AutoSize = True
        Label2.Location = New System.Drawing.Point(3, 59)
        Label2.Name = "Label2"
        Label2.Size = New System.Drawing.Size(62, 13)
        Label2.TabIndex = 12
        Label2.Text = "Workspace"
        '
        'Label3
        '
        Label3.AutoSize = True
        Label3.Location = New System.Drawing.Point(3, 30)
        Label3.Name = "Label3"
        Label3.Size = New System.Drawing.Size(72, 13)
        Label3.TabIndex = 11
        Label3.Text = "Analysis Type"
        '
        'Label4
        '
        Label4.AutoSize = True
        Label4.Location = New System.Drawing.Point(3, 134)
        Label4.Name = "Label4"
        Label4.Size = New System.Drawing.Size(93, 13)
        Label4.TabIndex = 14
        Label4.Text = "Where Expression"
        '
        'Label1
        '
        Label1.AutoSize = True
        Label1.Location = New System.Drawing.Point(3, 84)
        Label1.Name = "Label1"
        Label1.Size = New System.Drawing.Size(33, 13)
        Label1.TabIndex = 13
        Label1.Text = "Layer"
        '
        'Label5
        '
        Label5.AutoSize = True
        Label5.Location = New System.Drawing.Point(3, 218)
        Label5.Name = "Label5"
        Label5.Size = New System.Drawing.Size(56, 13)
        Label5.TabIndex = 15
        Label5.Text = "Sort Fields"
        '
        'txtName
        '
        Me.txtName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtName.Location = New System.Drawing.Point(95, 3)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(354, 20)
        Me.txtName.TabIndex = 0
        '
        'BrowseButton
        '
        Me.BrowseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BrowseButton.Location = New System.Drawing.Point(398, 52)
        Me.BrowseButton.Name = "BrowseButton"
        Me.BrowseButton.Size = New System.Drawing.Size(51, 23)
        Me.BrowseButton.TabIndex = 3
        Me.BrowseButton.Text = "Browse"
        Me.BrowseButton.UseVisualStyleBackColor = True
        '
        'ddlAnalysisType
        '
        Me.ddlAnalysisType.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ddlAnalysisType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.ddlAnalysisType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.ddlAnalysisType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlAnalysisType.FormattingEnabled = True
        Me.ddlAnalysisType.Items.AddRange(New Object() {"Comparison Resource", "Distance", "Distance and Direction", "Feature Comparison", "Generic Resource", "Pinpoint", "Weighted Average"})
        Me.ddlAnalysisType.Location = New System.Drawing.Point(95, 27)
        Me.ddlAnalysisType.Name = "ddlAnalysisType"
        Me.ddlAnalysisType.Size = New System.Drawing.Size(354, 21)
        Me.ddlAnalysisType.TabIndex = 1
        '
        'txtWhereExpression
        '
        Me.txtWhereExpression.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtWhereExpression.Location = New System.Drawing.Point(95, 131)
        Me.txtWhereExpression.Multiline = True
        Me.txtWhereExpression.Name = "txtWhereExpression"
        Me.txtWhereExpression.Size = New System.Drawing.Size(354, 72)
        Me.txtWhereExpression.TabIndex = 8
        '
        'ddlWorkspace
        '
        Me.ddlWorkspace.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ddlWorkspace.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.ddlWorkspace.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.ddlWorkspace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlWorkspace.FormattingEnabled = True
        Me.ddlWorkspace.Location = New System.Drawing.Point(95, 54)
        Me.ddlWorkspace.Name = "ddlWorkspace"
        Me.ddlWorkspace.Size = New System.Drawing.Size(297, 21)
        Me.ddlWorkspace.Sorted = True
        Me.ddlWorkspace.TabIndex = 2
        '
        'ddlLayer
        '
        Me.ddlLayer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ddlLayer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.ddlLayer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.ddlLayer.FormattingEnabled = True
        Me.ddlLayer.Location = New System.Drawing.Point(95, 81)
        Me.ddlLayer.Name = "ddlLayer"
        Me.ddlLayer.Size = New System.Drawing.Size(354, 21)
        Me.ddlLayer.Sorted = True
        Me.ddlLayer.TabIndex = 4
        '
        'QueryBuilderButton
        '
        Me.QueryBuilderButton.Location = New System.Drawing.Point(6, 150)
        Me.QueryBuilderButton.Name = "QueryBuilderButton"
        Me.QueryBuilderButton.Size = New System.Drawing.Size(83, 23)
        Me.QueryBuilderButton.TabIndex = 7
        Me.QueryBuilderButton.Text = "Query Builder"
        Me.QueryBuilderButton.UseVisualStyleBackColor = True
        '
        'cbShowHeaders
        '
        Me.cbShowHeaders.AutoSize = True
        Me.cbShowHeaders.Location = New System.Drawing.Point(95, 108)
        Me.cbShowHeaders.Name = "cbShowHeaders"
        Me.cbShowHeaders.Size = New System.Drawing.Size(96, 17)
        Me.cbShowHeaders.TabIndex = 5
        Me.cbShowHeaders.Text = "Show Headers"
        Me.cbShowHeaders.UseVisualStyleBackColor = True
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
        Me.gvSortFields.Location = New System.Drawing.Point(95, 209)
        Me.gvSortFields.Name = "gvSortFields"
        Me.gvSortFields.Size = New System.Drawing.Size(354, 236)
        Me.gvSortFields.TabIndex = 9
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
        Me.cbSummarization.Location = New System.Drawing.Point(197, 108)
        Me.cbSummarization.Name = "cbSummarization"
        Me.cbSummarization.Size = New System.Drawing.Size(253, 17)
        Me.cbSummarization.TabIndex = 6
        Me.cbSummarization.Text = "Enable Data Grouping/Summarization Functions"
        Me.cbSummarization.UseVisualStyleBackColor = True
        Me.cbSummarization.Visible = False
        '
        'ERALayer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.Controls.Add(Me.cbSummarization)
        Me.Controls.Add(Label5)
        Me.Controls.Add(Me.gvSortFields)
        Me.Controls.Add(Me.cbShowHeaders)
        Me.Controls.Add(Me.QueryBuilderButton)
        Me.Controls.Add(Label1)
        Me.Controls.Add(Me.ddlLayer)
        Me.Controls.Add(Me.ddlWorkspace)
        Me.Controls.Add(Me.ddlAnalysisType)
        Me.Controls.Add(Me.BrowseButton)
        Me.Controls.Add(Label3)
        Me.Controls.Add(Label2)
        Me.Controls.Add(Me.txtWhereExpression)
        Me.Controls.Add(Me.txtName)
        Me.Controls.Add(NameLabel)
        Me.Controls.Add(Label4)
        Me.Name = "ERALayer"
        Me.Size = New System.Drawing.Size(474, 448)
        CType(Me.myErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gvSortFields, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtName As System.Windows.Forms.TextBox
    Friend WithEvents BrowseButton As System.Windows.Forms.Button
    Friend WithEvents ddlAnalysisType As System.Windows.Forms.ComboBox
    Friend WithEvents txtWhereExpression As System.Windows.Forms.TextBox
    Friend WithEvents ddlWorkspace As System.Windows.Forms.ComboBox
    Friend WithEvents ddlLayer As System.Windows.Forms.ComboBox
    Friend WithEvents QueryBuilderButton As System.Windows.Forms.Button
    Friend WithEvents cbShowHeaders As System.Windows.Forms.CheckBox
    Friend WithEvents gvSortFields As System.Windows.Forms.DataGridView
    Friend WithEvents colSortFieldName As System.Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents colDirection As System.Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents cbSummarization As System.Windows.Forms.CheckBox

End Class
