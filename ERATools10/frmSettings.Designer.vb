<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSettings
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtOutputFormatsFolder = New System.Windows.Forms.TextBox
        Me.BrowseOutputFormatsButton = New System.Windows.Forms.Button
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.txtScratchWorkspacePath = New System.Windows.Forms.TextBox
        Me.txtAuthorName = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.BrowseSystemWorkspaceButton = New System.Windows.Forms.Button
        Me.Label3 = New System.Windows.Forms.Label
        Me.OKButton = New System.Windows.Forms.Button
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.ep = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.cbWarnReplaceOutput = New System.Windows.Forms.CheckBox
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.TabPage1 = New System.Windows.Forms.TabPage
        Me.TabPage2 = New System.Windows.Forms.TabPage
        CType(Me.ep, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(111, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Output &Formats Folder"
        '
        'txtOutputFormatsFolder
        '
        Me.txtOutputFormatsFolder.Location = New System.Drawing.Point(129, 6)
        Me.txtOutputFormatsFolder.Name = "txtOutputFormatsFolder"
        Me.txtOutputFormatsFolder.Size = New System.Drawing.Size(258, 20)
        Me.txtOutputFormatsFolder.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.txtOutputFormatsFolder, "Enter the path where the XSLT files that define the output format options are sto" & _
                "red.")
        '
        'BrowseOutputFormatsButton
        '
        Me.BrowseOutputFormatsButton.CausesValidation = False
        Me.BrowseOutputFormatsButton.Location = New System.Drawing.Point(393, 6)
        Me.BrowseOutputFormatsButton.Name = "BrowseOutputFormatsButton"
        Me.BrowseOutputFormatsButton.Size = New System.Drawing.Size(54, 23)
        Me.BrowseOutputFormatsButton.TabIndex = 2
        Me.BrowseOutputFormatsButton.Text = "&Browse"
        Me.BrowseOutputFormatsButton.UseVisualStyleBackColor = True
        '
        'txtScratchWorkspacePath
        '
        Me.txtScratchWorkspacePath.Location = New System.Drawing.Point(108, 6)
        Me.txtScratchWorkspacePath.Name = "txtScratchWorkspacePath"
        Me.txtScratchWorkspacePath.Size = New System.Drawing.Size(380, 20)
        Me.txtScratchWorkspacePath.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.txtScratchWorkspacePath, "Enter the filename of the ERAtool geodatabase file where saved shapes are stored")
        '
        'txtAuthorName
        '
        Me.txtAuthorName.Location = New System.Drawing.Point(129, 32)
        Me.txtAuthorName.Name = "txtAuthorName"
        Me.txtAuthorName.Size = New System.Drawing.Size(258, 20)
        Me.txtAuthorName.TabIndex = 7
        Me.ToolTip1.SetToolTip(Me.txtAuthorName, "Enter the name to use as the author of the report")
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(96, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "&Geodatabase Path"
        '
        'BrowseSystemWorkspaceButton
        '
        Me.BrowseSystemWorkspaceButton.Location = New System.Drawing.Point(494, 3)
        Me.BrowseSystemWorkspaceButton.Name = "BrowseSystemWorkspaceButton"
        Me.BrowseSystemWorkspaceButton.Size = New System.Drawing.Size(54, 23)
        Me.BrowseSystemWorkspaceButton.TabIndex = 5
        Me.BrowseSystemWorkspaceButton.Text = "B&rowse"
        Me.BrowseSystemWorkspaceButton.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 35)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(69, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "&Author Name"
        '
        'OKButton
        '
        Me.OKButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.OKButton.Location = New System.Drawing.Point(196, 126)
        Me.OKButton.Name = "OKButton"
        Me.OKButton.Size = New System.Drawing.Size(75, 23)
        Me.OKButton.TabIndex = 9
        Me.OKButton.Text = "&OK"
        Me.OKButton.UseVisualStyleBackColor = True
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Cancel_Button.CausesValidation = False
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(295, 126)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(73, 23)
        Me.Cancel_Button.TabIndex = 10
        Me.Cancel_Button.Text = "&Cancel"
        Me.Cancel_Button.UseVisualStyleBackColor = True
        '
        'ep
        '
        Me.ep.ContainerControl = Me
        '
        'cbWarnReplaceOutput
        '
        Me.cbWarnReplaceOutput.AutoSize = True
        Me.cbWarnReplaceOutput.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cbWarnReplaceOutput.Location = New System.Drawing.Point(3, 58)
        Me.cbWarnReplaceOutput.Name = "cbWarnReplaceOutput"
        Me.cbWarnReplaceOutput.Size = New System.Drawing.Size(180, 17)
        Me.cbWarnReplaceOutput.TabIndex = 8
        Me.cbWarnReplaceOutput.Text = "&Warn before replacing output file"
        Me.cbWarnReplaceOutput.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(0, 2)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(562, 118)
        Me.TabControl1.TabIndex = 11
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.txtScratchWorkspacePath)
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Controls.Add(Me.BrowseSystemWorkspaceButton)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(554, 92)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Saved Shapes"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.txtOutputFormatsFolder)
        Me.TabPage2.Controls.Add(Me.cbWarnReplaceOutput)
        Me.TabPage2.Controls.Add(Me.Label1)
        Me.TabPage2.Controls.Add(Me.BrowseOutputFormatsButton)
        Me.TabPage2.Controls.Add(Me.txtAuthorName)
        Me.TabPage2.Controls.Add(Me.Label3)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(554, 92)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Output Report Options"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'frmSettings
        '
        Me.AcceptButton = Me.OKButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(564, 161)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.Cancel_Button)
        Me.Controls.Add(Me.OKButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSettings"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "ERAtools2 Settings"
        CType(Me.ep, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtOutputFormatsFolder As System.Windows.Forms.TextBox
    Friend WithEvents BrowseOutputFormatsButton As System.Windows.Forms.Button
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtScratchWorkspacePath As System.Windows.Forms.TextBox
    Friend WithEvents BrowseSystemWorkspaceButton As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtAuthorName As System.Windows.Forms.TextBox
    Friend WithEvents OKButton As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents ep As System.Windows.Forms.ErrorProvider
    Friend WithEvents cbWarnReplaceOutput As System.Windows.Forms.CheckBox
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
End Class
