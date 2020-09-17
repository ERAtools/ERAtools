<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ERA_ShapeWorkspace
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
        Me.components = New System.ComponentModel.Container
        Dim Label4 As System.Windows.Forms.Label
        Dim txtDatabaseType As System.Windows.Forms.TextBox
        Dim Label1 As System.Windows.Forms.Label
        Dim NameLabel As System.Windows.Forms.Label
        Me.txtDirectory = New System.Windows.Forms.TextBox
        Me.txtName = New System.Windows.Forms.TextBox
        Me.myErrorProvider = New System.Windows.Forms.ErrorProvider(Me.components)
        Label4 = New System.Windows.Forms.Label
        txtDatabaseType = New System.Windows.Forms.TextBox
        Label1 = New System.Windows.Forms.Label
        NameLabel = New System.Windows.Forms.Label
        CType(Me.myErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label4
        '
        Label4.AutoSize = True
        Label4.Location = New System.Drawing.Point(3, 58)
        Label4.Name = "Label4"
        Label4.Size = New System.Drawing.Size(49, 13)
        Label4.TabIndex = 5
        Label4.Text = "Directory"
        '
        'txtDatabaseType
        '
        txtDatabaseType.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        txtDatabaseType.BackColor = System.Drawing.SystemColors.Control
        txtDatabaseType.Location = New System.Drawing.Point(81, 3)
        txtDatabaseType.Name = "txtDatabaseType"
        txtDatabaseType.ReadOnly = True
        txtDatabaseType.Size = New System.Drawing.Size(244, 20)
        txtDatabaseType.TabIndex = 0
        txtDatabaseType.Text = "Shapefile Workspace"
        '
        'Label1
        '
        Label1.AutoSize = True
        Label1.Location = New System.Drawing.Point(3, 6)
        Label1.Name = "Label1"
        Label1.Size = New System.Drawing.Size(31, 13)
        Label1.TabIndex = 3
        Label1.Text = "Type"
        '
        'NameLabel
        '
        NameLabel.AutoSize = True
        NameLabel.Location = New System.Drawing.Point(3, 32)
        NameLabel.Name = "NameLabel"
        NameLabel.Size = New System.Drawing.Size(72, 13)
        NameLabel.TabIndex = 4
        NameLabel.Text = "Display Name"
        '
        'txtDirectory
        '
        Me.txtDirectory.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDirectory.Location = New System.Drawing.Point(81, 55)
        Me.txtDirectory.Name = "txtDirectory"
        Me.txtDirectory.Size = New System.Drawing.Size(244, 20)
        Me.txtDirectory.TabIndex = 2
        '
        'txtName
        '
        Me.txtName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtName.Location = New System.Drawing.Point(81, 29)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(244, 20)
        Me.txtName.TabIndex = 1
        '
        'myErrorProvider
        '
        Me.myErrorProvider.ContainerControl = Me
        '
        'ERA_ShapeWorkspace
        '
        Me.Controls.Add(NameLabel)
        Me.Controls.Add(Label4)
        Me.Controls.Add(Me.txtDirectory)
        Me.Controls.Add(Me.txtName)
        Me.Controls.Add(txtDatabaseType)
        Me.Controls.Add(Label1)
        Me.Name = "ERA_ShapeWorkspace"
        Me.Size = New System.Drawing.Size(350, 250)
        CType(Me.myErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtDirectory As System.Windows.Forms.TextBox
    Friend WithEvents txtName As System.Windows.Forms.TextBox
End Class
