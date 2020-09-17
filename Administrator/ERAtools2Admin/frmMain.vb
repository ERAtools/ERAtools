Imports System.Windows.Forms

Public Class frmMain

    Private Const m_FileDialogFilter As String = "ERAtools2 Configuration Files (*.era)|*.era|ERAtools2 Configuration XML Files (*.xml)|*.xml"

    Private Sub ShowNewForm(ByVal sender As Object, ByVal e As EventArgs) Handles NewToolStripMenuItem.Click, NewToolStripButton.Click
        'Create a new instance of the child form.
        OpenAnalysis("")
    End Sub

    Private Sub OpenAnalysis(ByVal Filename As String)
        Try
            Dim m_frmERAAnalysis As New frmReportDefinitionEditor(Filename)
            With m_frmERAAnalysis
                .MdiParent = Me
                If Filename = "" Then
                    .Text = "New Report Definiton"
                Else
                    .Text = Filename
                End If
                .Show()
            End With
        Catch ex As Exception
            'Failed to open the form...Probably because it couldn't read the xml file
            'The user should have been notified already, so don't do anything here...
        End Try
    End Sub

    Private Sub OpenFile(ByVal sender As Object, ByVal e As EventArgs) Handles OpenToolStripMenuItem.Click, OpenToolStripButton.Click
        Dim OpenFileDialog As New OpenFileDialog
        'OpenFileDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        OpenFileDialog.Filter = m_FileDialogFilter
        If (OpenFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            Dim FileName As String = OpenFileDialog.FileName

            'Check through all open files and see if it's already open in a child window, if so, focus that window.
            Dim blnFound As Boolean = False
            For Each Childform As Form In Me.MdiChildren
                If TypeOf (Childform) Is frmReportDefinitionEditor Then
                    With DirectCast(Childform, frmReportDefinitionEditor)
                        If .Filename = FileName Then
                            .Show()
                            .Focus()
                            blnFound = True
                            Exit For
                        End If
                    End With
                End If
            Next

            If Not blnFound Then OpenAnalysis(FileName)
        End If
    End Sub

    Private Sub ExitToolsStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ExitToolStripMenuItem.Click
        Global.System.Windows.Forms.Application.Exit()
    End Sub

    'The computer.clipboard is just not reliable enough--so we store the cut/copied element as a variable here
    Public ClipboardNode As Xml.XmlElement

    Public Sub CutToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CutToolStripMenuItem.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            With DirectCast(ActiveMdiChild, frmReportDefinitionEditor)
                ClipboardNode = .CutNode()
                'clipboard is just too problematic My.Computer.Clipboard.SetData("Xml.XmlElement", .CutNode)
            End With
        End If
    End Sub

    Public Sub CopyToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CopyToolStripMenuItem.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            With DirectCast(ActiveMdiChild, frmReportDefinitionEditor)
                ClipboardNode = .CopyNode()
                'clipboard is just too problematic My.Computer.Clipboard.SetData("Xml.XmlElement", .CopyNode)
            End With
        End If
    End Sub

    Public Sub PasteToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles PasteToolStripMenuItem.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            With DirectCast(ActiveMdiChild, frmReportDefinitionEditor)
                .PasteNode(ClipboardNode)
                'clipboard is just too problematic .PasteNode(My.Computer.Clipboard.GetData("Xml.XmlElement"))
            End With
        End If
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteToolStripMenuItem.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            With DirectCast(ActiveMdiChild, frmReportDefinitionEditor)
                .DeleteItem_Click(sender, e)
                'clipboard is just too problematic .PasteNode(My.Computer.Clipboard.GetData("Xml.XmlElement"))
            End With
        End If
    End Sub

    Private Sub CascadeToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CascadeToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.Cascade)
    End Sub

    Private Sub TileVerticleToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TileVerticalToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.TileVertical)
    End Sub

    Private Sub TileHorizontalToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TileHorizontalToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.TileHorizontal)
    End Sub

    Private Sub ArrangeIconsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ArrangeIconsToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.ArrangeIcons)
    End Sub

    Private Sub CloseAllToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CloseAllToolStripMenuItem.Click
        ' Close all child forms of the parent.
        For Each ChildForm As Form In Me.MdiChildren
            ChildForm.Close()
        Next
    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Me.WindowState = FormWindowState.Maximized Then
            My.Settings.WindowMaximize = True
        Else
            My.Settings.WindowMaximize = False
            My.Settings.WindowPosition = Me.DesktopLocation
            My.Settings.WindowSize = Me.ClientSize
            My.Settings.Save()
        End If
    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        LogMessage("ERAtools Administrator Started")

        My.Application.myFrmMain = Me

        Me.Show()
        If Not My.Settings.WindowPosition = Nothing Then
            Me.DesktopLocation = My.Settings.WindowPosition
            Me.ClientSize = My.Settings.WindowSize
        End If
        If My.Settings.WindowMaximize Then
            Me.WindowState = FormWindowState.Maximized
        End If

        Application.DoEvents()

        '12/14/2007: Changed this so that it always checks for the license at startup and
        'hid the UI features that indicate the connection status/options

        'check license
        If True Then 'My.Settings.Startup_ESRI_Connect Then
            Options_ESRI_Startup.Checked = True
            My.Application.StartWait()
            If Not My.Application.ConnectToESRI Then
                My.Application.EndWait()
                MessageBox.Show("Unable to check out an ArcView, ArcEditor or ArcInfo license. Data browsing functionality will be disabled. Status returned:" & vbNewLine & My.Application.GetESRIStatusMessage)
            End If
            My.Application.EndWait()
        Else
            Options_ESRI_Startup.Checked = False
        End If

        ConfirmDeleteToolStripMenuItem.Checked = My.Settings.ConfirmDelete
    End Sub

    Private Sub frmMain_MdiChildActivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MdiChildActivate
        Dim b As Boolean = TypeOf (Me.ActiveMdiChild) Is frmReportDefinitionEditor
        Me.SaveAsToolStripMenuItem.Enabled = b
        Me.SaveToolStripButton.Enabled = b
        Me.SaveToolStripMenuItem.Enabled = b
        Me.AddDropDownButton.Enabled = b
        Me.ValidateToolStripMenuItem.Enabled = b
    End Sub

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub SaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripButton.Click, SaveToolStripMenuItem.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            DirectCast(ActiveMdiChild, frmReportDefinitionEditor).SaveFile()
        End If
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles SaveAsToolStripMenuItem.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            With DirectCast(ActiveMdiChild, frmReportDefinitionEditor)
                Dim SaveFileDialog As New SaveFileDialog
                'SaveFileDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
                SaveFileDialog.Filter = m_FileDialogFilter
                If (SaveFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                    .Filename = SaveFileDialog.FileName
                    .SaveFile()
                End If
            End With
        End If
    End Sub

    Friend Sub UpdateESRIStatusBar()
        Me.ESRI_Connect_Button.Enabled = True
        ESRI_Connect_Button.BorderStyle = Border3DStyle.Raised
        With Me.ESRI_StatusBar
            Select Case My.Application.ESRI_Status
                Case My.ESRI_Status_Enum.Unknown
                    .Text = "Not Connected"
                    .BackColor = Color.FromArgb(255, 255, 50)
                    Me.ESRI_Connect_Button.Text = "Connect"
                Case My.ESRI_Status_Enum.Connected
                    .Text = "Successfully Connected"
                    .BackColor = Color.FromArgb(50, 255, 50)
                    Me.ESRI_Connect_Button.Text = "Disconnect"
                Case My.ESRI_Status_Enum.Connecting
                    .Text = "Connecting..."
                    .BackColor = Color.FromArgb(255, 255, 50)
                    Me.ESRI_Connect_Button.Text = "Connecting..."
                    ESRI_Connect_Button.BorderStyle = Border3DStyle.Sunken
                    Me.ESRI_Connect_Button.Enabled = False
                Case My.ESRI_Status_Enum.Failed
                    .Text = "Failed to Connect"
                    .BackColor = Color.FromArgb(255, 50, 50)
                    Me.ESRI_Connect_Button.Text = "Connect"
            End Select
            .ToolTipText = My.Application.GetESRIStatusMessage
            Application.DoEvents()
        End With
    End Sub

    Private Sub ESRI_Connect_Button_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ESRI_Connect_Button.Click
        If My.Application.ESRI_Status = My.ESRI_Status_Enum.Connected Then
            My.Application.DisconnectESRI()
        ElseIf Not My.Application.ConnectToESRI Then
            MessageBox.Show("Unable to check out an ArcView, ArcEditor or ArcInfo license. Data browsing functionality will be disabled. Status returned:" & vbNewLine & My.Application.GetESRIStatusMessage)
        End If
    End Sub

    Private Sub ESRI_StatusBar_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ESRI_StatusBar.DoubleClick
        MessageBox.Show(Me.ESRI_StatusBar.ToolTipText, "Connection Status", MessageBoxButtons.OK)
    End Sub

    Private Sub Options_ESRI_Startup_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Options_ESRI_Startup.CheckedChanged
        My.Settings.Startup_ESRI_Connect = Options_ESRI_Startup.Checked
    End Sub


    Private Sub ValidateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ValidateToolStripMenuItem.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            With DirectCast(ActiveMdiChild, frmReportDefinitionEditor)
                .RunValidation()
            End With
        End If
    End Sub

    Private Sub AddDropDownButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddDropDownButton.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            Me.NewFieldToolStripMenuItem.Enabled = True
            Me.NewTableToolStripMenuItem.Enabled = True
            Me.NewLayerToolStripMenuItem.Enabled = True
            Dim frde As frmReportDefinitionEditor = ActiveMdiChild
            If frde.ERAAnalysisTreeView.SelectedNode Is Nothing Then
                Me.NewFieldToolStripMenuItem.Enabled = False
                Me.NewTableToolStripMenuItem.Enabled = False
                Me.NewLayerToolStripMenuItem.Enabled = False
                Exit Sub
            End If
            With DirectCast(frde.ERAAnalysisTreeView.SelectedNode, ERANode)
                Select Case .ERANodeType
                    Case ERANode.ERANodeTypeEnum.Workspaces
                        Me.NewFieldToolStripMenuItem.Enabled = False
                        Me.NewTableToolStripMenuItem.Enabled = False
                        Me.NewLayerToolStripMenuItem.Enabled = False
                    Case ERANode.ERANodeTypeEnum.SDEWorkspace
                        Me.NewFieldToolStripMenuItem.Enabled = False
                        Me.NewTableToolStripMenuItem.Enabled = False
                        Me.NewLayerToolStripMenuItem.Enabled = False
                    Case ERANode.ERANodeTypeEnum.AccessWorkspace
                        Me.NewFieldToolStripMenuItem.Enabled = False
                        Me.NewTableToolStripMenuItem.Enabled = False
                        Me.NewLayerToolStripMenuItem.Enabled = False
                    Case ERANode.ERANodeTypeEnum.ShapeWorkspace
                        Me.NewFieldToolStripMenuItem.Enabled = False
                        Me.NewTableToolStripMenuItem.Enabled = False
                        Me.NewLayerToolStripMenuItem.Enabled = False
                    Case ERANode.ERANodeTypeEnum.Issue
                        Me.NewFieldToolStripMenuItem.Enabled = False
                        Me.NewTableToolStripMenuItem.Enabled = False
                        Me.NewLayerToolStripMenuItem.Enabled = True
                    Case ERANode.ERANodeTypeEnum.Layer
                        Me.NewFieldToolStripMenuItem.Enabled = True
                        Me.NewTableToolStripMenuItem.Enabled = True
                        Me.NewLayerToolStripMenuItem.Enabled = True
                    Case ERANode.ERANodeTypeEnum.Table
                        Me.NewFieldToolStripMenuItem.Enabled = True
                        Me.NewTableToolStripMenuItem.Enabled = True
                        Me.NewLayerToolStripMenuItem.Enabled = False
                    Case ERANode.ERANodeTypeEnum.Field
                        Me.NewFieldToolStripMenuItem.Enabled = True
                        Me.NewLayerToolStripMenuItem.Enabled = False
                        Me.NewTableToolStripMenuItem.Enabled = True
                End Select
            End With
        End If
    End Sub

    Private Sub NewSDEWorkspaceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewSDEWorkspaceToolStripMenuItem.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            With DirectCast(ActiveMdiChild, frmReportDefinitionEditor)
                .NewSDEWorkspace_Click(sender, e)
            End With
        End If
    End Sub

    Private Sub NewShapefileWorkspaceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewShapefileWorkspaceToolStripMenuItem.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            With DirectCast(ActiveMdiChild, frmReportDefinitionEditor)
                .NewShapefileWorkspace_Click(sender, e)
            End With
        End If
    End Sub

    Private Sub NewAccessWorkspaceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewAccessWorkspaceToolStripMenuItem.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            With DirectCast(ActiveMdiChild, frmReportDefinitionEditor)
                .NewAccessWorkspace_Click(sender, e)
            End With
        End If
    End Sub

    Private Sub NewIssueToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewIssueToolStripMenuItem.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            With DirectCast(ActiveMdiChild, frmReportDefinitionEditor)
                .NewIssue_Click(sender, e)
            End With
        End If
    End Sub

    Private Sub NewLayerToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewLayerToolStripMenuItem.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            With DirectCast(ActiveMdiChild, frmReportDefinitionEditor)
                .NewLayer_Click(sender, e)
            End With
        End If
    End Sub

    Private Sub NewTableToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewTableToolStripMenuItem.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            With DirectCast(ActiveMdiChild, frmReportDefinitionEditor)
                .NewTable_Click(sender, e)
            End With
        End If
    End Sub

    Private Sub NewFieldToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewFieldToolStripMenuItem.Click
        If ActiveMdiChild Is Nothing Then
            Exit Sub
        ElseIf TypeOf (ActiveMdiChild) Is frmReportDefinitionEditor Then
            With DirectCast(ActiveMdiChild, frmReportDefinitionEditor)
                .NewField_Click(sender, e)
            End With
        End If
    End Sub

 
    Private Sub EditMenu_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles EditMenu.DropDownOpening
        If ActiveMdiChild Is Nothing Then
            PasteToolStripMenuItem.Enabled = False
            CopyToolStripMenuItem.Enabled = False
            CutToolStripMenuItem.Enabled = False
            DeleteToolStripMenuItem.Enabled = False
            Exit Sub
        End If

        Dim ContextNode As ERANode = DirectCast(ActiveMdiChild, frmReportDefinitionEditor).ContextNode
        If ContextNode Is Nothing Then
            PasteToolStripMenuItem.Enabled = False
            CopyToolStripMenuItem.Enabled = False
            CutToolStripMenuItem.Enabled = False
            DeleteToolStripMenuItem.Enabled = False
            Exit Sub
        Else
            DeleteToolStripMenuItem.Enabled = (ContextNode.ERANodeType <> ERANode.ERANodeTypeEnum.Workspaces)
        End If

        If ClipboardNode Is Nothing Then
            Me.PasteToolStripMenuItem.Enabled = False
        Else
            Me.PasteToolStripMenuItem.Enabled = True

            'disable only if pasting a layer onto something other than an issue or child of an issue
            'otherwise the system figures out the right place to put things automatically 
            If ContextNode.ERANodeType = ERANode.ERANodeTypeEnum.Workspaces Or ContextNode.ERANodeType = ERANode.ERANodeTypeEnum.AccessWorkspace Or ContextNode.ERANodeType = ERANode.ERANodeTypeEnum.SDEWorkspace Or ContextNode.ERANodeType = ERANode.ERANodeTypeEnum.ShapeWorkspace Then
                If ClipboardNode.Name = "LAYER" Or ClipboardNode.Name = "ISSUE" Then
                    Me.PasteToolStripMenuItem.Enabled = False
                End If
            End If
        End If

        Me.CutToolStripMenuItem.Enabled = (ContextNode.ERANodeType <> ERANode.ERANodeTypeEnum.Field)
        Me.CopyToolStripMenuItem.Enabled = (ContextNode.ERANodeType <> ERANode.ERANodeTypeEnum.Field)

    End Sub


    Private Sub ConfirmDeleteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConfirmDeleteToolStripMenuItem.Click
        My.Settings.ConfirmDelete = Not My.Settings.ConfirmDelete
        ConfirmDeleteToolStripMenuItem.Checked = My.Settings.ConfirmDelete
        My.Settings.Save()
    End Sub

    Private Sub OptionsToolStripMenuItem1_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles OptionsToolStripMenuItem1.DropDownOpening
        ConfirmDeleteToolStripMenuItem.Checked = My.Settings.ConfirmDelete
    End Sub

    Private Sub ContentsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ContentsToolStripMenuItem.Click
        Dim filename As String = IO.Path.Combine(Application.StartupPath, "ERAtools2AdminHelp.pdf")
        Try
            If IO.File.Exists(filename) Then
                'Shell(filename, AppWinStyle.NormalFocus)
                Process.Start(filename)
            Else
                MessageBox.Show("Unable to display help. Help file ERAtools2AdminHelp.pdf not found in expected path (" & Application.StartupPath & ")", "Help File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show("Error displaying help: " & ex.Message, "Error Displaying Help", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        Dim ab As New AboutDialog
        ab.ShowDialog()
    End Sub
End Class
