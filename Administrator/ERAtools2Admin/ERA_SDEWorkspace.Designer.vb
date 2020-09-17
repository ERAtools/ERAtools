<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ERA_SDEWorkspace
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
        Dim Label3 As System.Windows.Forms.Label
        Dim txtDatabaseType As System.Windows.Forms.TextBox
        Dim Label1 As System.Windows.Forms.Label
        Dim Label4 As System.Windows.Forms.Label
        Dim Label6 As System.Windows.Forms.Label
        Dim Label5 As System.Windows.Forms.Label
        Dim Label7 As System.Windows.Forms.Label
        Dim NameLabel As System.Windows.Forms.Label
        Dim Label2 As System.Windows.Forms.Label
        Me.txtServer = New System.Windows.Forms.TextBox
        Me.txtName = New System.Windows.Forms.TextBox
        Me.txtInstance = New System.Windows.Forms.TextBox
        Me.txtPassword = New System.Windows.Forms.TextBox
        Me.txtUser = New System.Windows.Forms.TextBox
        Me.txtDatabase = New System.Windows.Forms.TextBox
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.txtVersion = New System.Windows.Forms.TextBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.rbOSA = New System.Windows.Forms.RadioButton
        Me.rbDBMS = New System.Windows.Forms.RadioButton
        Label3 = New System.Windows.Forms.Label
        txtDatabaseType = New System.Windows.Forms.TextBox
        Label1 = New System.Windows.Forms.Label
        Label4 = New System.Windows.Forms.Label
        Label6 = New System.Windows.Forms.Label
        Label5 = New System.Windows.Forms.Label
        Label7 = New System.Windows.Forms.Label
        NameLabel = New System.Windows.Forms.Label
        Label2 = New System.Windows.Forms.Label
        CType(Me.myErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label3
        '
        Label3.AutoSize = True
        Label3.Location = New System.Drawing.Point(3, 58)
        Label3.Name = "Label3"
        Label3.Size = New System.Drawing.Size(63, 13)
        Label3.TabIndex = 9
        Label3.Text = "SDE Server"
        '
        'txtDatabaseType
        '
        txtDatabaseType.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        txtDatabaseType.BackColor = System.Drawing.SystemColors.Control
        txtDatabaseType.Location = New System.Drawing.Point(81, 3)
        txtDatabaseType.Name = "txtDatabaseType"
        txtDatabaseType.ReadOnly = True
        txtDatabaseType.Size = New System.Drawing.Size(248, 20)
        txtDatabaseType.TabIndex = 0
        txtDatabaseType.Text = "SDE Workspace"
        '
        'Label1
        '
        Label1.AutoSize = True
        Label1.Location = New System.Drawing.Point(3, 6)
        Label1.Name = "Label1"
        Label1.Size = New System.Drawing.Size(31, 13)
        Label1.TabIndex = 7
        Label1.Text = "Type"
        '
        'Label4
        '
        Label4.AutoSize = True
        Label4.Location = New System.Drawing.Point(3, 84)
        Label4.Name = "Label4"
        Label4.Size = New System.Drawing.Size(43, 13)
        Label4.TabIndex = 10
        Label4.Text = "Service"
        '
        'Label6
        '
        Label6.AutoSize = True
        Label6.Location = New System.Drawing.Point(6, 72)
        Label6.Name = "Label6"
        Label6.Size = New System.Drawing.Size(53, 13)
        Label6.TabIndex = 13
        Label6.Text = "Password"
        '
        'Label5
        '
        Label5.AutoSize = True
        Label5.Location = New System.Drawing.Point(6, 46)
        Label5.Name = "Label5"
        Label5.Size = New System.Drawing.Size(55, 13)
        Label5.TabIndex = 12
        Label5.Text = "Username"
        '
        'Label7
        '
        Label7.AutoSize = True
        Label7.Location = New System.Drawing.Point(3, 110)
        Label7.Name = "Label7"
        Label7.Size = New System.Drawing.Size(53, 13)
        Label7.TabIndex = 11
        Label7.Text = "Database"
        '
        'NameLabel
        '
        NameLabel.AutoSize = True
        NameLabel.Location = New System.Drawing.Point(3, 32)
        NameLabel.Name = "NameLabel"
        NameLabel.Size = New System.Drawing.Size(72, 13)
        NameLabel.TabIndex = 8
        NameLabel.Text = "Display Name"
        '
        'Label2
        '
        Label2.AutoSize = True
        Label2.Location = New System.Drawing.Point(3, 267)
        Label2.Name = "Label2"
        Label2.Size = New System.Drawing.Size(42, 13)
        Label2.TabIndex = 13
        Label2.Text = "Version"
        '
        'txtServer
        '
        Me.txtServer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtServer.Location = New System.Drawing.Point(81, 55)
        Me.txtServer.Name = "txtServer"
        Me.txtServer.Size = New System.Drawing.Size(248, 20)
        Me.txtServer.TabIndex = 2
        '
        'txtName
        '
        Me.txtName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtName.Location = New System.Drawing.Point(81, 29)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(248, 20)
        Me.txtName.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.txtName, "The user-friendly name of this workspace. Must be unique within the document.")
        '
        'txtInstance
        '
        Me.txtInstance.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtInstance.Location = New System.Drawing.Point(81, 81)
        Me.txtInstance.Name = "txtInstance"
        Me.txtInstance.Size = New System.Drawing.Size(248, 20)
        Me.txtInstance.TabIndex = 3
        '
        'txtPassword
        '
        Me.txtPassword.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPassword.Location = New System.Drawing.Point(75, 69)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.Size = New System.Drawing.Size(250, 20)
        Me.txtPassword.TabIndex = 2
        Me.txtPassword.UseSystemPasswordChar = True
        '
        'txtUser
        '
        Me.txtUser.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtUser.Location = New System.Drawing.Point(75, 43)
        Me.txtUser.Name = "txtUser"
        Me.txtUser.Size = New System.Drawing.Size(250, 20)
        Me.txtUser.TabIndex = 1
        '
        'txtDatabase
        '
        Me.txtDatabase.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDatabase.Location = New System.Drawing.Point(81, 107)
        Me.txtDatabase.Name = "txtDatabase"
        Me.txtDatabase.Size = New System.Drawing.Size(248, 20)
        Me.txtDatabase.TabIndex = 4
        '
        'txtVersion
        '
        Me.txtVersion.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtVersion.Location = New System.Drawing.Point(79, 264)
        Me.txtVersion.Name = "txtVersion"
        Me.txtVersion.Size = New System.Drawing.Size(248, 20)
        Me.txtVersion.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.txtVersion, "Enter the version you wish to connect to. Typically, this is ""SDE.DEFAULT"".")
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.rbOSA)
        Me.GroupBox1.Controls.Add(Me.rbDBMS)
        Me.GroupBox1.Controls.Add(Me.txtUser)
        Me.GroupBox1.Controls.Add(Me.txtPassword)
        Me.GroupBox1.Controls.Add(Label5)
        Me.GroupBox1.Controls.Add(Label6)
        Me.GroupBox1.Location = New System.Drawing.Point(4, 136)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(347, 122)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Account"
        '
        'rbOSA
        '
        Me.rbOSA.AutoSize = True
        Me.rbOSA.Location = New System.Drawing.Point(9, 95)
        Me.rbOSA.Name = "rbOSA"
        Me.rbOSA.Size = New System.Drawing.Size(176, 17)
        Me.rbOSA.TabIndex = 3
        Me.rbOSA.TabStop = True
        Me.rbOSA.Text = "Operating system authentication"
        Me.rbOSA.UseVisualStyleBackColor = True
        '
        'rbDBMS
        '
        Me.rbDBMS.AutoSize = True
        Me.rbDBMS.Location = New System.Drawing.Point(6, 20)
        Me.rbDBMS.Name = "rbDBMS"
        Me.rbDBMS.Size = New System.Drawing.Size(141, 17)
        Me.rbDBMS.TabIndex = 0
        Me.rbDBMS.TabStop = True
        Me.rbDBMS.Text = "Database authentication"
        Me.rbDBMS.UseVisualStyleBackColor = True
        '
        'ERA_SDEWorkspace
        '
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.txtVersion)
        Me.Controls.Add(NameLabel)
        Me.Controls.Add(Label2)
        Me.Controls.Add(Label7)
        Me.Controls.Add(Me.txtDatabase)
        Me.Controls.Add(Label4)
        Me.Controls.Add(Me.txtInstance)
        Me.Controls.Add(Label3)
        Me.Controls.Add(Me.txtServer)
        Me.Controls.Add(Me.txtName)
        Me.Controls.Add(txtDatabaseType)
        Me.Controls.Add(Label1)
        Me.Name = "ERA_SDEWorkspace"
        Me.Size = New System.Drawing.Size(354, 356)
        CType(Me.myErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtServer As System.Windows.Forms.TextBox
    Friend WithEvents txtName As System.Windows.Forms.TextBox
    Friend WithEvents txtInstance As System.Windows.Forms.TextBox
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents txtUser As System.Windows.Forms.TextBox
    Friend WithEvents txtDatabase As System.Windows.Forms.TextBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents txtVersion As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents rbDBMS As System.Windows.Forms.RadioButton
    Friend WithEvents rbOSA As System.Windows.Forms.RadioButton
End Class
