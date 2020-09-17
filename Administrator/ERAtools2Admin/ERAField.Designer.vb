<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ERAField
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
        Me.components = New System.ComponentModel.Container()
        Dim Label1 As System.Windows.Forms.Label
        Dim Label2 As System.Windows.Forms.Label
        Dim lblPrefix As System.Windows.Forms.Label
        Dim lblSuffix As System.Windows.Forms.Label
        Dim lblExampleHeader As System.Windows.Forms.Label
        Dim lblUseForURL As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ERAField))
        Me.ddlName = New System.Windows.Forms.ComboBox()
        Me.txtAlias = New System.Windows.Forms.TextBox()
        Me.cbLink = New System.Windows.Forms.CheckBox()
        Me.txtPrefix = New System.Windows.Forms.TextBox()
        Me.txtSuffix = New System.Windows.Forms.TextBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.rbRawData = New System.Windows.Forms.RadioButton()
        Me.rbIndexData = New System.Windows.Forms.RadioButton()
        Me.txtFormat = New System.Windows.Forms.TextBox()
        Me.ddlUseForURL = New System.Windows.Forms.ComboBox()
        Me.gbHyperlink = New System.Windows.Forms.GroupBox()
        Me.lblLinkExample = New System.Windows.Forms.Label()
        Me.ddlFunction = New System.Windows.Forms.ComboBox()
        Me.lblFunction = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Label1 = New System.Windows.Forms.Label()
        Label2 = New System.Windows.Forms.Label()
        lblPrefix = New System.Windows.Forms.Label()
        lblSuffix = New System.Windows.Forms.Label()
        lblExampleHeader = New System.Windows.Forms.Label()
        lblUseForURL = New System.Windows.Forms.Label()
        CType(Me.myErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbHyperlink.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Label1.AutoSize = True
        Label1.Location = New System.Drawing.Point(3, 6)
        Label1.Name = "Label1"
        Label1.Size = New System.Drawing.Size(79, 17)
        Label1.TabIndex = 5
        Label1.Text = "Field Name"
        '
        'Label2
        '
        Label2.AutoSize = True
        Label2.Location = New System.Drawing.Point(3, 33)
        Label2.Name = "Label2"
        Label2.Size = New System.Drawing.Size(95, 17)
        Label2.TabIndex = 6
        Label2.Text = "Display Name"
        '
        'lblPrefix
        '
        lblPrefix.AutoSize = True
        lblPrefix.Location = New System.Drawing.Point(6, 46)
        lblPrefix.Name = "lblPrefix"
        lblPrefix.Size = New System.Drawing.Size(75, 17)
        lblPrefix.TabIndex = 4
        lblPrefix.Text = "URL Prefix"
        '
        'lblSuffix
        '
        lblSuffix.AutoSize = True
        lblSuffix.Location = New System.Drawing.Point(6, 72)
        lblSuffix.Name = "lblSuffix"
        lblSuffix.Size = New System.Drawing.Size(74, 17)
        lblSuffix.TabIndex = 5
        lblSuffix.Text = "URL Suffix"
        '
        'lblExampleHeader
        '
        lblExampleHeader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        lblExampleHeader.Location = New System.Drawing.Point(6, 98)
        lblExampleHeader.Name = "lblExampleHeader"
        lblExampleHeader.Size = New System.Drawing.Size(420, 32)
        lblExampleHeader.TabIndex = 6
        lblExampleHeader.Text = "Example: if the value of the ""Field for URL"" is ""x72x"" then URL would be generate" & _
            "d as "
        '
        'lblUseForURL
        '
        lblUseForURL.AutoSize = True
        lblUseForURL.Location = New System.Drawing.Point(6, 19)
        lblUseForURL.Name = "lblUseForURL"
        lblUseForURL.Size = New System.Drawing.Size(95, 17)
        lblUseForURL.TabIndex = 3
        lblUseForURL.Text = "Field For URL"
        '
        'ddlName
        '
        Me.ddlName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ddlName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.ddlName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.ddlName.FormattingEnabled = True
        Me.ddlName.Location = New System.Drawing.Point(128, 3)
        Me.ddlName.Name = "ddlName"
        Me.ddlName.Size = New System.Drawing.Size(285, 24)
        Me.ddlName.Sorted = True
        Me.ddlName.TabIndex = 0
        '
        'txtAlias
        '
        Me.txtAlias.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtAlias.Location = New System.Drawing.Point(128, 30)
        Me.txtAlias.Name = "txtAlias"
        Me.txtAlias.Size = New System.Drawing.Size(285, 22)
        Me.txtAlias.TabIndex = 1
        '
        'cbLink
        '
        Me.cbLink.AutoSize = True
        Me.cbLink.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cbLink.Location = New System.Drawing.Point(3, 110)
        Me.cbLink.Name = "cbLink"
        Me.cbLink.Size = New System.Drawing.Size(257, 21)
        Me.cbLink.TabIndex = 4
        Me.cbLink.Text = "This field is displayed as a hyperlink"
        Me.cbLink.UseVisualStyleBackColor = True
        '
        'txtPrefix
        '
        Me.txtPrefix.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPrefix.Location = New System.Drawing.Point(84, 43)
        Me.txtPrefix.Name = "txtPrefix"
        Me.txtPrefix.Size = New System.Drawing.Size(326, 22)
        Me.txtPrefix.TabIndex = 1
        '
        'txtSuffix
        '
        Me.txtSuffix.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSuffix.Location = New System.Drawing.Point(84, 69)
        Me.txtSuffix.Name = "txtSuffix"
        Me.txtSuffix.Size = New System.Drawing.Size(326, 22)
        Me.txtSuffix.TabIndex = 2
        '
        'rbRawData
        '
        Me.rbRawData.AutoSize = True
        Me.rbRawData.Location = New System.Drawing.Point(299, 110)
        Me.rbRawData.Name = "rbRawData"
        Me.rbRawData.Size = New System.Drawing.Size(315, 21)
        Me.rbRawData.TabIndex = 6
        Me.rbRawData.TabStop = True
        Me.rbRawData.Text = "Proportional to Resource Feature (Raw Data)"
        Me.ToolTip1.SetToolTip(Me.rbRawData, "Use for raw data that is not independent of the size of the polygon." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & " Examples i" & _
                "nclude the Population within a given area, the total number of houses in a given" & _
                " area, etc.")
        Me.rbRawData.UseVisualStyleBackColor = True
        Me.rbRawData.Visible = False
        '
        'rbIndexData
        '
        Me.rbIndexData.AutoSize = True
        Me.rbIndexData.Location = New System.Drawing.Point(203, 110)
        Me.rbIndexData.Name = "rbIndexData"
        Me.rbIndexData.Size = New System.Drawing.Size(312, 21)
        Me.rbIndexData.TabIndex = 5
        Me.rbIndexData.TabStop = True
        Me.rbIndexData.Text = "Proportional to Analysis Feature (Index Data)"
        Me.ToolTip1.SetToolTip(Me.rbIndexData, resources.GetString("rbIndexData.ToolTip"))
        Me.rbIndexData.UseVisualStyleBackColor = True
        Me.rbIndexData.Visible = False
        '
        'txtFormat
        '
        Me.txtFormat.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFormat.Location = New System.Drawing.Point(128, 56)
        Me.txtFormat.Name = "txtFormat"
        Me.txtFormat.Size = New System.Drawing.Size(285, 22)
        Me.txtFormat.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.txtFormat, "Enter a format string to convert dates or numbers to text. Optional.")
        '
        'ddlUseForURL
        '
        Me.ddlUseForURL.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ddlUseForURL.FormattingEnabled = True
        Me.ddlUseForURL.Location = New System.Drawing.Point(84, 16)
        Me.ddlUseForURL.Name = "ddlUseForURL"
        Me.ddlUseForURL.Size = New System.Drawing.Size(326, 24)
        Me.ddlUseForURL.TabIndex = 0
        '
        'gbHyperlink
        '
        Me.gbHyperlink.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gbHyperlink.Controls.Add(Me.ddlUseForURL)
        Me.gbHyperlink.Controls.Add(Me.txtPrefix)
        Me.gbHyperlink.Controls.Add(Me.txtSuffix)
        Me.gbHyperlink.Controls.Add(lblUseForURL)
        Me.gbHyperlink.Controls.Add(Me.lblLinkExample)
        Me.gbHyperlink.Controls.Add(lblExampleHeader)
        Me.gbHyperlink.Controls.Add(lblSuffix)
        Me.gbHyperlink.Controls.Add(lblPrefix)
        Me.gbHyperlink.Location = New System.Drawing.Point(3, 145)
        Me.gbHyperlink.Name = "gbHyperlink"
        Me.gbHyperlink.Size = New System.Drawing.Size(432, 152)
        Me.gbHyperlink.TabIndex = 7
        Me.gbHyperlink.TabStop = False
        Me.gbHyperlink.Text = "Optional Settings For The Hyperlink"
        Me.gbHyperlink.Visible = False
        '
        'lblLinkExample
        '
        Me.lblLinkExample.AutoSize = True
        Me.lblLinkExample.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLinkExample.ForeColor = System.Drawing.Color.Blue
        Me.lblLinkExample.Location = New System.Drawing.Point(6, 130)
        Me.lblLinkExample.Name = "lblLinkExample"
        Me.lblLinkExample.Size = New System.Drawing.Size(36, 17)
        Me.lblLinkExample.TabIndex = 7
        Me.lblLinkExample.Text = "x72x"
        '
        'ddlFunction
        '
        Me.ddlFunction.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ddlFunction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlFunction.Items.AddRange(New Object() {"Group By / Distinct", "Sum", "Average", "Minimum", "Maximum", "Count", "Link Only (Not displayed or included in output, used to link tables)"})
        Me.ddlFunction.Location = New System.Drawing.Point(128, 83)
        Me.ddlFunction.Name = "ddlFunction"
        Me.ddlFunction.Size = New System.Drawing.Size(285, 24)
        Me.ddlFunction.TabIndex = 3
        Me.ddlFunction.Visible = False
        '
        'lblFunction
        '
        Me.lblFunction.AutoSize = True
        Me.lblFunction.Location = New System.Drawing.Point(3, 86)
        Me.lblFunction.Name = "lblFunction"
        Me.lblFunction.Size = New System.Drawing.Size(159, 17)
        Me.lblFunction.TabIndex = 7
        Me.lblFunction.Text = "Summarization Function"
        Me.lblFunction.Visible = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(3, 59)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(52, 17)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "Format"
        '
        'ERAField
        '
        Me.Controls.Add(Me.rbRawData)
        Me.Controls.Add(Me.rbIndexData)
        Me.Controls.Add(Me.ddlFunction)
        Me.Controls.Add(Me.txtFormat)
        Me.Controls.Add(Me.txtAlias)
        Me.Controls.Add(Me.ddlName)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lblFunction)
        Me.Controls.Add(Me.gbHyperlink)
        Me.Controls.Add(Me.cbLink)
        Me.Controls.Add(Label2)
        Me.Controls.Add(Label1)
        Me.Name = "ERAField"
        Me.Size = New System.Drawing.Size(438, 300)
        CType(Me.myErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbHyperlink.ResumeLayout(False)
        Me.gbHyperlink.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ddlName As System.Windows.Forms.ComboBox
    Friend WithEvents txtAlias As System.Windows.Forms.TextBox
    Friend WithEvents cbLink As System.Windows.Forms.CheckBox
    Friend WithEvents txtPrefix As System.Windows.Forms.TextBox
    Friend WithEvents txtSuffix As System.Windows.Forms.TextBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents ddlUseForURL As System.Windows.Forms.ComboBox
    Friend WithEvents gbHyperlink As System.Windows.Forms.GroupBox
    Friend WithEvents ddlFunction As System.Windows.Forms.ComboBox
    Friend WithEvents lblFunction As System.Windows.Forms.Label
    Friend WithEvents lblLinkExample As System.Windows.Forms.Label
    Friend WithEvents rbRawData As System.Windows.Forms.RadioButton
    Friend WithEvents rbIndexData As System.Windows.Forms.RadioButton
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtFormat As System.Windows.Forms.TextBox

End Class
