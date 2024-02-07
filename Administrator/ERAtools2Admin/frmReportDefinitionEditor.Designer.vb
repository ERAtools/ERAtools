<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmReportDefinitionEditor
    Inherits System.Windows.Forms.Form

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
        Dim Label2 As System.Windows.Forms.Label
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.ERAAnalysisTreeView = New System.Windows.Forms.TreeView
        Me.AnalysisContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.NewWorkspace = New System.Windows.Forms.ToolStripMenuItem
        Me.NewSDEWorkspace = New System.Windows.Forms.ToolStripMenuItem
        Me.NewShapefileWorkspace = New System.Windows.Forms.ToolStripMenuItem
        Me.NewAccessWorkspace = New System.Windows.Forms.ToolStripMenuItem
        Me.NewIssue = New System.Windows.Forms.ToolStripMenuItem
        Me.NewLayer = New System.Windows.Forms.ToolStripMenuItem
        Me.NewField = New System.Windows.Forms.ToolStripMenuItem
        Me.NewTable = New System.Windows.Forms.ToolStripMenuItem
        Me.Separator1 = New System.Windows.Forms.ToolStripSeparator
        Me.DeleteItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ValidateItem = New System.Windows.Forms.ToolStripMenuItem
        Me.Separator2 = New System.Windows.Forms.ToolStripSeparator
        Me.SortItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MoveUpItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MoveDownItem = New System.Windows.Forms.ToolStripMenuItem
        Me.Separator3 = New System.Windows.Forms.ToolStripSeparator
        Me.MoveToTopItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MoveToBottomItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator
        Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.CutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.PasteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.pnlContent = New System.Windows.Forms.Panel
        Me.btnRevert = New System.Windows.Forms.Button
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.txtReportTitle = New System.Windows.Forms.TextBox
        Label2 = New System.Windows.Forms.Label
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.AnalysisContextMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label2
        '
        Label2.AutoSize = True
        Label2.Location = New System.Drawing.Point(0, 5)
        Label2.Name = "Label2"
        Label2.Size = New System.Drawing.Size(102, 13)
        Label2.TabIndex = 2
        Label2.Text = "Default Report Title:"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer1.Location = New System.Drawing.Point(3, 28)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.GroupBox1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlContent)
        Me.SplitContainer1.Panel2.Controls.Add(Me.btnRevert)
        Me.SplitContainer1.Size = New System.Drawing.Size(739, 467)
        Me.SplitContainer1.SplitterDistance = 236
        Me.SplitContainer1.TabIndex = 1
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.ERAAnalysisTreeView)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(236, 467)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Report Outline"
        '
        'ERAAnalysisTreeView
        '
        Me.ERAAnalysisTreeView.ContextMenuStrip = Me.AnalysisContextMenu
        Me.ERAAnalysisTreeView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ERAAnalysisTreeView.Location = New System.Drawing.Point(3, 16)
        Me.ERAAnalysisTreeView.Name = "ERAAnalysisTreeView"
        Me.ERAAnalysisTreeView.ShowRootLines = False
        Me.ERAAnalysisTreeView.Size = New System.Drawing.Size(230, 448)
        Me.ERAAnalysisTreeView.TabIndex = 0
        '
        'AnalysisContextMenu
        '
        Me.AnalysisContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewWorkspace, Me.NewIssue, Me.NewLayer, Me.NewField, Me.NewTable, Me.Separator1, Me.DeleteItem, Me.ValidateItem, Me.Separator2, Me.SortItem, Me.ToolStripMenuItem1, Me.CopyToolStripMenuItem, Me.CutToolStripMenuItem, Me.PasteToolStripMenuItem})
        Me.AnalysisContextMenu.Name = "AnalysisContextMenu"
        Me.AnalysisContextMenu.ShowImageMargin = False
        Me.AnalysisContextMenu.Size = New System.Drawing.Size(138, 264)
        '
        'NewWorkspace
        '
        Me.NewWorkspace.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewSDEWorkspace, Me.NewShapefileWorkspace, Me.NewAccessWorkspace})
        Me.NewWorkspace.Name = "NewWorkspace"
        Me.NewWorkspace.Size = New System.Drawing.Size(137, 22)
        Me.NewWorkspace.Text = "New &Workspace"
        '
        'NewSDEWorkspace
        '
        Me.NewSDEWorkspace.Name = "NewSDEWorkspace"
        Me.NewSDEWorkspace.Size = New System.Drawing.Size(209, 22)
        Me.NewSDEWorkspace.Text = "New &SDE Workspace"
        '
        'NewShapefileWorkspace
        '
        Me.NewShapefileWorkspace.Name = "NewShapefileWorkspace"
        Me.NewShapefileWorkspace.Size = New System.Drawing.Size(209, 22)
        Me.NewShapefileWorkspace.Text = "New Shape&file Workspace"
        '
        'NewAccessWorkspace
        '
        Me.NewAccessWorkspace.Name = "NewAccessWorkspace"
        Me.NewAccessWorkspace.Size = New System.Drawing.Size(209, 22)
        Me.NewAccessWorkspace.Text = "New &Geodatabase Workspace"
        '
        'NewIssue
        '
        Me.NewIssue.Name = "NewIssue"
        Me.NewIssue.Size = New System.Drawing.Size(137, 22)
        Me.NewIssue.Text = "New &Issue"
        '
        'NewLayer
        '
        Me.NewLayer.Name = "NewLayer"
        Me.NewLayer.Size = New System.Drawing.Size(137, 22)
        Me.NewLayer.Text = "New &Analysis"
        '
        'NewField
        '
        Me.NewField.Name = "NewField"
        Me.NewField.Size = New System.Drawing.Size(137, 22)
        Me.NewField.Text = "New &Field"
        '
        'NewTable
        '
        Me.NewTable.Name = "NewTable"
        Me.NewTable.Size = New System.Drawing.Size(137, 22)
        Me.NewTable.Text = "New &Table"
        '
        'Separator1
        '
        Me.Separator1.Name = "Separator1"
        Me.Separator1.Size = New System.Drawing.Size(134, 6)
        '
        'DeleteItem
        '
        Me.DeleteItem.Name = "DeleteItem"
        Me.DeleteItem.Size = New System.Drawing.Size(137, 22)
        Me.DeleteItem.Text = "&Delete"
        '
        'ValidateItem
        '
        Me.ValidateItem.Name = "ValidateItem"
        Me.ValidateItem.Size = New System.Drawing.Size(137, 22)
        Me.ValidateItem.Text = "&Validate"
        '
        'Separator2
        '
        Me.Separator2.Name = "Separator2"
        Me.Separator2.Size = New System.Drawing.Size(134, 6)
        '
        'SortItem
        '
        Me.SortItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MoveUpItem, Me.MoveDownItem, Me.Separator3, Me.MoveToTopItem, Me.MoveToBottomItem})
        Me.SortItem.Name = "SortItem"
        Me.SortItem.Size = New System.Drawing.Size(137, 22)
        Me.SortItem.Text = "Sort"
        '
        'MoveUpItem
        '
        Me.MoveUpItem.Name = "MoveUpItem"
        Me.MoveUpItem.Size = New System.Drawing.Size(163, 22)
        Me.MoveUpItem.Text = "Move Up"
        '
        'MoveDownItem
        '
        Me.MoveDownItem.Name = "MoveDownItem"
        Me.MoveDownItem.Size = New System.Drawing.Size(163, 22)
        Me.MoveDownItem.Text = "Move Down"
        '
        'Separator3
        '
        Me.Separator3.Name = "Separator3"
        Me.Separator3.Size = New System.Drawing.Size(160, 6)
        '
        'MoveToTopItem
        '
        Me.MoveToTopItem.Name = "MoveToTopItem"
        Me.MoveToTopItem.Size = New System.Drawing.Size(163, 22)
        Me.MoveToTopItem.Text = "Move To Top"
        '
        'MoveToBottomItem
        '
        Me.MoveToBottomItem.Name = "MoveToBottomItem"
        Me.MoveToBottomItem.Size = New System.Drawing.Size(163, 22)
        Me.MoveToBottomItem.Text = "Move To Bottom"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(134, 6)
        '
        'CopyToolStripMenuItem
        '
        Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
        Me.CopyToolStripMenuItem.Size = New System.Drawing.Size(137, 22)
        Me.CopyToolStripMenuItem.Text = "&Copy"
        '
        'CutToolStripMenuItem
        '
        Me.CutToolStripMenuItem.Name = "CutToolStripMenuItem"
        Me.CutToolStripMenuItem.Size = New System.Drawing.Size(137, 22)
        Me.CutToolStripMenuItem.Text = "C&ut"
        '
        'PasteToolStripMenuItem
        '
        Me.PasteToolStripMenuItem.Name = "PasteToolStripMenuItem"
        Me.PasteToolStripMenuItem.Size = New System.Drawing.Size(137, 22)
        Me.PasteToolStripMenuItem.Text = "&Paste"
        '
        'pnlContent
        '
        Me.pnlContent.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlContent.Location = New System.Drawing.Point(4, 4)
        Me.pnlContent.Name = "pnlContent"
        Me.pnlContent.Size = New System.Drawing.Size(495, 427)
        Me.pnlContent.TabIndex = 0
        '
        'btnRevert
        '
        Me.btnRevert.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRevert.AutoSize = True
        Me.btnRevert.CausesValidation = False
        Me.btnRevert.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnRevert.Enabled = False
        Me.btnRevert.Location = New System.Drawing.Point(380, 437)
        Me.btnRevert.Name = "btnRevert"
        Me.btnRevert.Size = New System.Drawing.Size(94, 23)
        Me.btnRevert.TabIndex = 1
        Me.btnRevert.Text = "Revert Changes"
        Me.btnRevert.UseVisualStyleBackColor = True
        '
        'txtReportTitle
        '
        Me.txtReportTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtReportTitle.Location = New System.Drawing.Point(132, 2)
        Me.txtReportTitle.Name = "txtReportTitle"
        Me.txtReportTitle.Size = New System.Drawing.Size(588, 20)
        Me.txtReportTitle.TabIndex = 0
        '
        'frmReportDefinitionEditor
        '
        Me.CancelButton = Me.btnRevert
        Me.ClientSize = New System.Drawing.Size(742, 523)
        Me.Controls.Add(Me.txtReportTitle)
        Me.Controls.Add(Label2)
        Me.Controls.Add(Me.SplitContainer1)
        Me.MinimumSize = New System.Drawing.Size(483, 226)
        Me.Name = "frmReportDefinitionEditor"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.Text = "frmERAAnalysis"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        Me.SplitContainer1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.AnalysisContextMenu.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents ERAAnalysisTreeView As System.Windows.Forms.TreeView
    Friend WithEvents AnalysisContextMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents btnRevert As System.Windows.Forms.Button
    Friend WithEvents pnlContent As System.Windows.Forms.Panel
    Private WithEvents NewWorkspace As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents NewIssue As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents NewLayer As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents DeleteItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewField As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewTable As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewSDEWorkspace As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewShapefileWorkspace As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewAccessWorkspace As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents ValidateItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Separator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents Separator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SortItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MoveUpItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MoveDownItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Separator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents MoveToTopItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MoveToBottomItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents CopyToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PasteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents txtReportTitle As System.Windows.Forms.TextBox
End Class
