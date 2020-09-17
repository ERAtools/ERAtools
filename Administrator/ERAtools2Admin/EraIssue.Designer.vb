<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ERAIssue
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
        Dim Label1 As System.Windows.Forms.Label
        Me.txtIssueDescription = New System.Windows.Forms.TextBox
        Me.txtIssueName = New System.Windows.Forms.TextBox
        NameLabel = New System.Windows.Forms.Label
        Label1 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'NameLabel
        '
        NameLabel.AutoSize = True
        NameLabel.Location = New System.Drawing.Point(4, 7)
        NameLabel.Name = "NameLabel"
        NameLabel.Size = New System.Drawing.Size(35, 13)
        NameLabel.TabIndex = 2
        NameLabel.Text = "Name"
        '
        'Label1
        '
        Label1.AutoSize = True
        Label1.Location = New System.Drawing.Point(4, 33)
        Label1.Name = "Label1"
        Label1.Size = New System.Drawing.Size(60, 13)
        Label1.TabIndex = 3
        Label1.Text = "Description"
        '
        'txtIssueDescription
        '
        Me.txtIssueDescription.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtIssueDescription.Location = New System.Drawing.Point(75, 30)
        Me.txtIssueDescription.Multiline = True
        Me.txtIssueDescription.Name = "txtIssueDescription"
        Me.txtIssueDescription.Size = New System.Drawing.Size(250, 346)
        Me.txtIssueDescription.TabIndex = 1
        '
        'txtIssueName
        '
        Me.txtIssueName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtIssueName.Location = New System.Drawing.Point(75, 4)
        Me.txtIssueName.Name = "txtIssueName"
        Me.txtIssueName.Size = New System.Drawing.Size(250, 20)
        Me.txtIssueName.TabIndex = 0
        '
        'ERAIssue
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.txtIssueName)
        Me.Controls.Add(Me.txtIssueDescription)
        Me.Controls.Add(Label1)
        Me.Controls.Add(NameLabel)
        Me.Name = "ERAIssue"
        Me.Size = New System.Drawing.Size(350, 379)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtIssueDescription As System.Windows.Forms.TextBox
    Friend WithEvents txtIssueName As System.Windows.Forms.TextBox

End Class
