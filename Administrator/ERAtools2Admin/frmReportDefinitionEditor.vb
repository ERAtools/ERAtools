Imports System.IO
Imports System.Xml

Public Class frmReportDefinitionEditor

    Private m_Filename As String
    Private m_XmlDoc As New XmlDocument
    Private m_WorkspacesNode As ERANode
    Private myEraControl As ERAControl = Nothing
    Public Dirty As Boolean = False

    Public Function ContextNode() As ERANode
        Return Me.ERAAnalysisTreeView.SelectedNode
    End Function

    Public Sub New(ByVal fname As String)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

        m_XmlDoc.Schemas.Add("http://tempuri.org/ERA2Schema.xsd", Application.StartupPath & "\ERA2Schema.xsd")
        m_Filename = fname
        If m_Filename <> "" Then
            ReadXMLFile()
        Else
            Dim era_analysis As Xml.XmlElement = m_XmlDoc.CreateElement("ERA_ANALYSIS")
            m_XmlDoc.AppendChild(era_analysis)
            'm_WorkspacesNode = New ERANode(m_XmlDoc.CreateElement("WORKSPACES"))
            m_WorkspacesNode = New WorkspacesNode
            m_WorkspacesNode.XMLNode = m_XmlDoc.CreateElement("WORKSPACES")
            era_analysis.AppendChild(m_WorkspacesNode.XMLNode)
            ERAAnalysisTreeView.Nodes.Add(m_WorkspacesNode)

            Dim newIssue As New IssueNode
            newIssue.XMLNode = m_XmlDoc.CreateElement("ISSUE")
            newIssue.SetAttributeValue("name", "New Issue")

            Dim newAnalysis As New LayerNode
            newAnalysis.XMLNode = m_XmlDoc.CreateElement("LAYER")
            newAnalysis.SetAttributeValue("name", "New Analysis")
            newIssue.XMLNode.AppendChild(newAnalysis.XMLNode)

            newIssue.Nodes.Add(newAnalysis)
            newIssue.ExpandAll()

            era_analysis.AppendChild(newIssue.XMLNode)
            ERAAnalysisTreeView.Nodes.Add(newIssue)
        End If
    End Sub

    Private Sub frmReportDefinitionEditor_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If My.Settings.MaximizeChildren Then
            Me.WindowState = FormWindowState.Maximized
        Else
            If Not My.Settings.ChildrenSize = Nothing Then _
            Me.ClientSize = My.Settings.ChildrenSize
        End If
        If Not My.Settings.TreePaneWidth = Nothing Then _
        Me.SplitContainer1.SplitterDistance = My.Settings.TreePaneWidth
    End Sub

    Private Sub ReadXMLFile()
        'read & validate file, set contents of treeview accordingly
        Try
            'todo: though this is the documented way to validate against a schema, 
            ' it doesn't actually do anything. 
            'Dim settings As XmlReaderSettings = New XmlReaderSettings()
            'settings.Schemas.Add("http://tempuri.org/ERA2Schema.xsd", Application.StartupPath & "\ERA2Schema.xsd")
            'settings.ValidationType = ValidationType.Schema

            'Dim reader As XmlReader = XmlReader.Create(m_Filename, settings)
            'm_XmlDoc = New XmlDocument()
            'm_XmlDoc.Load(reader)

            m_XmlDoc.Load(m_Filename)
        Catch ex As Exception
            MessageBox.Show("An error occurred loading " & m_Filename & ": " & ex.Message, "Error Loading File", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Throw
        End Try

        'Try
        '    Dim eventHandler As Schema.ValidationEventHandler = New Schema.ValidationEventHandler(AddressOf ValidateEventHandler)
        '    m_XmlDoc.Validate(eventHandler)
        'Catch ex As Exception
        '    MessageBox.Show("An error occurred validating " & m_Filename & ": " & ex.Message, "Error Loading File", MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    Throw
        'End Try

        If m_XmlDoc.DocumentElement IsNot Nothing Then
            txtReportTitle.Text = m_XmlDoc.DocumentElement.GetAttribute("title")
        End If

        Try
            AddTreeViewChildNodes(Me.ERAAnalysisTreeView.Nodes, m_XmlDoc.DocumentElement)
        Catch ex As Exception
            MessageBox.Show("Error reading .era file: " & ex.Message, "Error Reading .era File", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Throw
        End Try

        Me.ERAAnalysisTreeView.ExpandAll()
        Dirty = False
    End Sub

    'Private Sub ValidateEventHandler(ByVal sender As Object, ByVal e As System.Xml.Schema.ValidationEventArgs)
    '    Select Case e.Severity
    '        Case Schema.XmlSeverityType.Error
    '            Console.WriteLine("Error: {0}", e.Message)
    '        Case Schema.XmlSeverityType.Warning
    '            Console.WriteLine("Warning {0}", e.Message)
    '    End Select

    'End Sub

    Private Sub AddTreeViewChildNodes(ByVal ParentNodes As TreeNodeCollection, ByVal xml_node As XmlElement)
        ' Add the children of this XML node to this child nodes collection.
        For Each ChildXMLNode As XmlElement In xml_node.ChildNodes
            'Don't add KEYS and SORT_FIELDS
            If Not ChildXMLNode.Name.ToUpper.StartsWith("KEY") AndAlso Not ChildXMLNode.Name.ToUpper.StartsWith("SORT") Then
                ' Make the new TreeView node.
                Dim NewNode As ERANode = EraNodeFactory.CreateEraNode(ChildXMLNode)

                ParentNodes.Add(NewNode)

                'Check for Workspaces node
                If NewNode.ERANodeType = ERANode.ERANodeTypeEnum.Workspaces Then
                    m_WorkspacesNode = NewNode
                End If

                ' Recursively make this node's descendants.
                AddTreeViewChildNodes(NewNode.Nodes, ChildXMLNode)
            End If
        Next ChildXMLNode
    End Sub

    Public Property Filename() As String
        Get
            Return m_Filename
        End Get
        Set(ByVal value As String)
            m_Filename = value
        End Set
    End Property

    Public ReadOnly Property WorkspacesNode() As ERANode
        Get
            If m_WorkspacesNode Is Nothing Then
                'm_WorkspacesNode = New ERANode(m_XmlDoc.CreateElement("WORKSPACES"))
                m_WorkspacesNode = New WorkspacesNode
                m_WorkspacesNode.XMLNode = m_XmlDoc.CreateElement("WORKSPACES")
                ERAAnalysisTreeView.Nodes.Add(m_WorkspacesNode)
            End If
            Return m_WorkspacesNode
        End Get
    End Property

    Public Function getWorkspaceNode(ByVal wsName As String) As ERANode
        For Each wsNode As ERANode In m_WorkspacesNode.Nodes
            If wsNode.GetAttributeValue("name") = wsName Then
                Return wsNode
            End If
        Next
        Return Nothing
    End Function

    Private Sub ERAAnalysisTreeView_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles ERAAnalysisTreeView.AfterSelect
        e.Node.BackColor = Color.DarkBlue
        With DirectCast(e.Node, ERANode)
            If .ErrorText <> "" Then
                .ForeColor = Color.Red
            Else
                .ForeColor = Color.White
            End If
        End With
        If Not myEraControl Is Nothing Then
            If hasChanged Then
                Dirty = True
            End If
            myEraControl.SaveChangesToNode()
            myEraControl = Nothing
            Me.pnlContent.Controls.Clear()
        End If
        Dim SelectedNode As ERANode = e.Node
        Dim m_control As ERAControl = Nothing
        Select Case SelectedNode.ERANodeType
            Case ERANode.ERANodeTypeEnum.Issue
                m_control = New ERAIssue()
            Case ERANode.ERANodeTypeEnum.Layer
                m_control = New ERALayer()
            Case ERANode.ERANodeTypeEnum.SDEWorkspace
                m_control = New ERA_SDEWorkspace()
            Case ERANode.ERANodeTypeEnum.AccessWorkspace
                m_control = New ERA_AccessWorkspace()
            Case ERANode.ERANodeTypeEnum.ShapeWorkspace
                m_control = New ERA_ShapeWorkspace()
            Case ERANode.ERANodeTypeEnum.Field
                m_control = New ERAField()
            Case ERANode.ERANodeTypeEnum.Table
                m_control = New ERATable()
            Case Else
                'do nothing, add nothing, this is ok
        End Select
        If m_control IsNot Nothing Then
            myEraControl = m_control
            m_control.Dock = DockStyle.Fill
            'EraControl.frde_Parent = Me
            With Me.pnlContent.Controls
                .Clear()
                .Add(m_control)
            End With
            m_control.EraNode = SelectedNode
            'm_control.LoadData()


        End If
        'ShowControl(m_control)

    End Sub

    Private Sub ERAAnalysisTreeView_BeforeSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewCancelEventArgs) Handles ERAAnalysisTreeView.BeforeSelect
        If ERAAnalysisTreeView.SelectedNode IsNot Nothing Then
            With DirectCast(ERAAnalysisTreeView.SelectedNode, ERANode)
                If .ErrorText <> "" Then
                    .ForeColor = Color.Red
                Else
                    .ForeColor = Color.Black
                End If
            End With
            ERAAnalysisTreeView.SelectedNode.BackColor = Color.White
        End If
    End Sub

    'Private Sub ShowControl(ByRef EraControl As ERAControl)
    '    myEraControl = EraControl
    '    EraControl.Dock = DockStyle.Fill
    '    'EraControl.frde_Parent = Me
    '    With Me.pnlContent.Controls
    '        .Clear()
    '        .Add(EraControl)
    '    End With
    'End Sub

    Private Sub AnalysisContextMenu_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles AnalysisContextMenu.Opening
        ERAAnalysisTreeView.SelectedNode = ERAAnalysisTreeView.HitTest(ERAAnalysisTreeView.PointToClient(Form.MousePosition)).Node
        DeleteItem.Visible = True
        ValidateItem.Visible = True
        SortItem.Visible = True
        Separator1.Visible = True
        Separator2.Visible = True
        If ContextNode Is Nothing Then
            NewWorkspace.Visible = True
            NewIssue.Visible = True
            NewLayer.Visible = False
            NewField.Visible = False
            NewTable.Visible = False
            DeleteItem.Visible = False
            ValidateItem.Visible = False
            SortItem.Visible = False
            Separator1.Visible = False
            Separator2.Visible = False
        Else
            ERAAnalysisTreeView.SelectedNode = ContextNode
            Select Case ContextNode.ERANodeType
                Case ERANode.ERANodeTypeEnum.Workspaces
                    NewWorkspace.Visible = True
                    NewIssue.Visible = False
                    NewLayer.Visible = False
                    NewField.Visible = False
                    NewTable.Visible = False
                    DeleteItem.Visible = False
                    ValidateItem.Visible = False
                    SortItem.Visible = False
                    Separator1.Visible = False
                    Separator2.Visible = False
                Case ERANode.ERANodeTypeEnum.AccessWorkspace
                    NewWorkspace.Visible = True
                    NewIssue.Visible = False
                    NewLayer.Visible = False
                    NewField.Visible = False
                    NewTable.Visible = False
                Case ERANode.ERANodeTypeEnum.SDEWorkspace
                    NewWorkspace.Visible = True
                    NewIssue.Visible = False
                    NewLayer.Visible = False
                    NewField.Visible = False
                    NewTable.Visible = False
                Case ERANode.ERANodeTypeEnum.ShapeWorkspace
                    NewWorkspace.Visible = True
                    NewIssue.Visible = False
                    NewLayer.Visible = False
                    NewField.Visible = False
                    NewTable.Visible = False
                Case ERANode.ERANodeTypeEnum.Issue
                    NewWorkspace.Visible = False
                    NewIssue.Visible = True
                    NewLayer.Visible = True
                    NewField.Visible = False
                    NewTable.Visible = False
                Case ERANode.ERANodeTypeEnum.Layer
                    NewWorkspace.Visible = False
                    NewIssue.Visible = False
                    NewLayer.Visible = True
                    NewField.Visible = True
                    NewTable.Visible = True
                Case ERANode.ERANodeTypeEnum.Field
                    NewWorkspace.Visible = False
                    NewIssue.Visible = False
                    NewLayer.Visible = False
                    NewField.Visible = True
                    NewTable.Visible = True
                Case ERANode.ERANodeTypeEnum.Table
                    NewWorkspace.Visible = False
                    NewIssue.Visible = False
                    NewLayer.Visible = False
                    NewField.Visible = True
                    NewTable.Visible = True
                Case Else
                    NewWorkspace.Visible = True
                    NewIssue.Visible = True
                    NewLayer.Visible = False
                    NewField.Visible = False
                    NewTable.Visible = False
                    DeleteItem.Visible = False
                    ValidateItem.Visible = False
                    SortItem.Visible = False
                    Separator1.Visible = False
                    Separator2.Visible = False
            End Select
            MoveUpItem.Enabled = True
            MoveDownItem.Enabled = True
            MoveToTopItem.Enabled = True
            MoveToBottomItem.Enabled = True
            If ContextNode.isTop Then
                MoveUpItem.Enabled = False
                MoveToTopItem.Enabled = False
            End If
            If ContextNode.isBottom Then
                MoveDownItem.Enabled = False
                MoveToBottomItem.Enabled = False
            End If
        End If

        Dim m_frmMain As frmMain = Me.MdiParent
        If m_frmMain.ClipboardNode Is Nothing Then
            Me.PasteToolStripMenuItem.Enabled = False
        ElseIf ContextNode() IsNot Nothing Then
            Me.PasteToolStripMenuItem.Enabled = True
            'disable only if pasting a layer onto something other than an issue or child of an issue
            'otherwise the system figures out the right place to put things automatically 

            If ContextNode.ERANodeType = ERANode.ERANodeTypeEnum.Workspaces Or ContextNode.ERANodeType = ERANode.ERANodeTypeEnum.AccessWorkspace Or ContextNode.ERANodeType = ERANode.ERANodeTypeEnum.SDEWorkspace Or ContextNode.ERANodeType = ERANode.ERANodeTypeEnum.ShapeWorkspace Then
                If m_frmMain.ClipboardNode.Name = "LAYER" Or m_frmMain.ClipboardNode.Name = "ISSUE" Then
                    Me.PasteToolStripMenuItem.Enabled = False
                End If
            End If
        End If

        If ContextNode() IsNot Nothing Then
            Me.CutToolStripMenuItem.Enabled = (ContextNode.ERANodeType <> ERANode.ERANodeTypeEnum.Field)
            Me.CopyToolStripMenuItem.Enabled = (ContextNode.ERANodeType <> ERANode.ERANodeTypeEnum.Field)
            Me.Focus()
        End If
    End Sub

    Public Sub NewAccessWorkspace_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NewAccessWorkspace.Click
        AddNode("ACCESSWORKSPACE", "New Access Workspace", WorkspacesNode)
    End Sub

    'Private Sub NewFileGeodatabaseWorkspaceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewFileGeodatabaseWorkspaceToolStripMenuItem.Click
    '    AddNode("FILEGEODATABASEWORKSPACE", "New File Geodatabase Workspace", WorkspacesNode)
    'End Sub

    Public Sub NewSDEWorkspace_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NewSDEWorkspace.Click
        AddNode("SDEWORKSPACE", "New SDE Workspace", WorkspacesNode)
    End Sub

    Public Sub NewShapefileWorkspace_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NewShapefileWorkspace.Click
        AddNode("SHAPEWORKSPACE", "New Shapefile Workspace", WorkspacesNode)
    End Sub

    Public Sub NewIssue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NewIssue.Click
        AddNode("ISSUE", "New Issue")
      End Sub

    Public Sub NewLayer_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NewLayer.Click
        If ERAAnalysisTreeView.SelectedNode Is Nothing Then Exit Sub
        ERAAnalysisTreeView.SelectedNode = GetParentIssue(ContextNode)
        AddNode("LAYER", "New Analysis", ContextNode)
    End Sub

    Public Sub NewField_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NewField.Click
        If ContextNode() Is Nothing Then Exit Sub

        If ContextNode.ERANodeType = ERANode.ERANodeTypeEnum.Field Then _
            Me.ERAAnalysisTreeView.SelectedNode = ContextNode.Parent

        If ContextNode.ERANodeType = ERANode.ERANodeTypeEnum.Layer Or ContextNode.ERANodeType = ERANode.ERANodeTypeEnum.Table Then

            Dim newnode As Xml.XmlElement = m_XmlDoc.CreateElement("FIELD")
            newnode.SetAttribute("alias", "New Field")
            newnode.SetAttribute("visible", "true")
            AddNode(newnode, ContextNode)

        End If

    End Sub

    Public Sub NewTable_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NewTable.Click
        If ContextNode() Is Nothing Then Exit Sub
        If ContextNode.ERANodeType = ERANode.ERANodeTypeEnum.Field Then _
            Me.ERAAnalysisTreeView.SelectedNode = ContextNode.Parent

        AddNode("TABLE", "New Table", ContextNode)

    End Sub

    Private Sub AddNode(ByRef NewXmlElement As Xml.XmlElement, Optional ByRef ParentNode As ERANode = Nothing)
        Dim newnode As ERANode = EraNodeFactory.CreateEraNode(NewXmlElement)
        If ParentNode Is Nothing Then
            ERAAnalysisTreeView.Nodes.Add(newnode)
            m_XmlDoc.FirstChild.AppendChild(newnode.XMLNode)
        Else
            ParentNode.Nodes.Add(newnode)
            ParentNode.XMLNode.AppendChild(newnode.XMLNode)
        End If
        newnode.EnsureVisible()
        ERAAnalysisTreeView.SelectedNode = newnode
        Dirty = True
    End Sub

    Private Sub AddNode(ByVal NewElementName As String, ByVal NewTitle As String, Optional ByRef ParentNode As ERANode = Nothing)
        Dim newnode As Xml.XmlElement = m_XmlDoc.CreateElement(NewElementName)
        newnode.SetAttribute("name", NewTitle)
        AddNode(newnode, ParentNode)
    End Sub

    Private Sub AddEraNode(ByVal ParentNodes As TreeNodeCollection, ByVal EraXmlElement As Xml.XmlElement)
        Dim ParentNode As ERANode = EraNodeFactory.CreateEraNode(EraXmlElement)
        ParentNodes.Add(ParentNode)
        ParentNodes = ParentNode.Nodes
        AddTreeViewChildNodes(ParentNodes, EraXmlElement)
        ParentNode.ExpandAll()
        ParentNode.EnsureVisible()
    End Sub


#Region "Cut/Copy/Paste Subs"
    Private Sub CopyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyToolStripMenuItem.Click
        Dim m_frmMain As frmMain = Me.MdiParent
        m_frmMain.CopyToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub CutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CutToolStripMenuItem.Click
        Dim m_frmMain As frmMain = Me.MdiParent
        m_frmMain.CutToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub PasteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasteToolStripMenuItem.Click
        Dim m_frmMain As frmMain = Me.MdiParent
        m_frmMain.PasteToolStripMenuItem_Click(sender, e)
    End Sub

    Public Function CutNode() As XmlElement
        Dim copyXML As XmlElement = CopyNode()
        Dim n As ERANode = ERAAnalysisTreeView.SelectedNode
        If n.Parent Is Nothing Then
            ERAAnalysisTreeView.Nodes.Remove(n)
        Else
            n.Parent.Nodes.Remove(n)
        End If
        n.XMLNode.ParentNode.RemoveChild(n.XMLNode)
        Me.Dirty = True
        Return copyXML
    End Function

    Public Function CopyNode() As XmlElement
        'This is a workaround to a known but undocumented .Net issue--extended TreeNode objects (like 
        'our ERANode) cannot be retreived from the clipboard because they are not
        'serializable. See:
        'http://groups.google.com/group/microsoft.public.dotnet.framework.windowsforms.controls/browse_thread/thread/57e50ffdd3314ccb/1489486af991ce0e?lnk=st&q=treenode+clipboard&rnum=4&hl=en#1489486af991ce0e
        'Moveover, we have to worry if the node being pasted comes from another instance of this application
        'or from another document within the application, so we have to consider that the node
        'might have a reference to a workspace that is not part of this document.
        'So ultimately we've decided against using the Clipboard class in favor of a simpler, 
        'if limited method of storing it as a variable in the parent form.
        If ERAAnalysisTreeView.SelectedNode Is Nothing Then Return Nothing
        Return DirectCast(ERAAnalysisTreeView.SelectedNode, ERANode).XMLNode.Clone
    End Function

    Public Sub PasteNode(ByVal PasteXML As XmlElement)
        If PasteXML Is Nothing Then Exit Sub
        Dim PasteWorkspacesNode As Xml.XmlElement = Nothing 'will remain nothing unless the pasted node comes from another document
        If Not (PasteXML.OwnerDocument Is m_XmlDoc) Then
            PasteWorkspacesNode = PasteXML.OwnerDocument.SelectSingleNode("ERA_ANALYSIS/WORKSPACES")
            PasteXML = m_XmlDoc.ImportNode(PasteXML, True)
        End If
        Select Case PasteXML.Name
            Case "ISSUE"
                'paste to root, regardless of what someone has selected
                AddEraNode(ERAAnalysisTreeView.Nodes, PasteXML)
                Me.m_XmlDoc.FirstChild.AppendChild(PasteXML)

                For Each LayerXML As XmlElement In PasteXML.SelectNodes("LAYER")
                    'code in PasteLayerTableWorkspaces handles case when PasteWorkspacesNode is nothing
                    PasteLayerTableWorkspaces(LayerXML, PasteWorkspacesNode)
                Next
                Me.Dirty = True
            Case "LAYER"
                Dim targetNode As ERANode = Me.ERAAnalysisTreeView.SelectedNode
                If targetNode Is Nothing Then Exit Sub
                If targetNode.ERANodeType = ERANode.ERANodeTypeEnum.Workspaces Or targetNode.ERANodeType = ERANode.ERANodeTypeEnum.AccessWorkspace Or targetNode.ERANodeType = ERANode.ERANodeTypeEnum.SDEWorkspace Or targetNode.ERANodeType = ERANode.ERANodeTypeEnum.ShapeWorkspace Then
                    MessageBox.Show("You must paste an analysis layer onto an issue.", "Invalid Paste Target", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Exit Sub
                End If
                'next line ensures that we're pasting to the issue if a child of an issue is selected.
                targetNode = GetParentIssue(targetNode)
                'create the ERAnodes from the pasted XML, plus the children
                AddEraNode(targetNode.Nodes, PasteXML)
                targetNode.XMLNode.AppendChild(PasteXML)

                PasteLayerTableWorkspaces(PasteXML, PasteWorkspacesNode)
                Me.Dirty = True
            Case "TABLE"
                Dim targetNode As ERANode = Me.ERAAnalysisTreeView.SelectedNode
                If targetNode Is Nothing Then Exit Sub
                If targetNode.ERANodeType = ERANode.ERANodeTypeEnum.Field Then _
                    targetNode = targetNode.Parent
                AddEraNode(targetNode.Nodes, PasteXML)
                targetNode.XMLNode.AppendChild(PasteXML)
                PasteLayerTableWorkspaces(PasteXML, PasteWorkspacesNode)
                Me.Dirty = True
            Case "WORKSPACES"
                'merge children into this document's workspaces node
                For Each PasteNodeChild As XmlElement In PasteXML.SelectNodes("*")
                    PasteWorkspace(PasteNodeChild, PasteNodeChild.GetAttribute("name"))
                Next
                Me.Dirty = True
            Case "ACCESSWORKSPACE"
                PasteWorkspace(PasteXML, PasteXML.GetAttribute("name"))
            Case "SDEWORKSPACE"
                PasteWorkspace(PasteXML, PasteXML.GetAttribute("name"))
            Case "SHAPEWORKSPACE"
                PasteWorkspace(PasteXML, PasteXML.GetAttribute("name"))
            Case Else
                'not supporting pasting of fields at this point
        End Select
    End Sub

    'Checks to see if the PasteWorkspaceNode already exists in this document's workspaces
    'collection. If it finds a match by the node content (path to folder or gdb, or SDE connection info)
    'then it returns a string representing the workspace ID. If not found, it adds it,
    'ensuring that the name hasn't already been used, and returns the workspace ID.
    Private Function PasteWorkspace(ByVal PasteWorkspacesXML As Xml.XmlElement, ByVal workspacename As String) As String
        If PasteWorkspacesXML Is Nothing Then Return "" '<--this is the case when the node to be pasted comes from the same document
        Dim PasteWorkspaceXML As Xml.XmlElement = PasteWorkspacesXML.SelectSingleNode("*[@name='" & workspacename & "']")
        If PasteWorkspaceXML Is Nothing Then Return ""
        For Each WorkspaceNode As ERANode In WorkspacesNode.Nodes
            If WorkspaceNode.XMLNode.Name = PasteWorkspaceXML.Name Then
                Select Case WorkspaceNode.ERANodeType
                    Case ERANode.ERANodeTypeEnum.AccessWorkspace
                        If WorkspaceNode.GetAttributeValue("database") = _
                        PasteWorkspaceXML.GetAttribute("database") Then
                            Return WorkspaceNode.GetAttributeValue("name")
                        End If
                    Case ERANode.ERANodeTypeEnum.SDEWorkspace
                        If WorkspaceNode.GetAttributeValue("server") = _
                            PasteWorkspaceXML.GetAttribute("server") And _
                            WorkspaceNode.GetAttributeValue("instance") = _
                            PasteWorkspaceXML.GetAttribute("instance") And _
                            WorkspaceNode.GetAttributeValue("user") = _
                            PasteWorkspaceXML.GetAttribute("user") And _
                            WorkspaceNode.GetAttributeValue("database") = _
                            PasteWorkspaceXML.GetAttribute("database") Then
                            Return WorkspaceNode.GetAttributeValue("name")
                        End If
                    Case ERANode.ERANodeTypeEnum.ShapeWorkspace
                        If WorkspaceNode.GetAttributeValue("directory") = _
                        PasteWorkspaceXML.GetAttribute("directory") Then
                            Return WorkspaceNode.GetAttributeValue("name")
                        End If
                End Select
            End If
        Next
        'if we make it this far, no match to an existing workspace has been found
        'so add a new one

        'first, make sure the new name hasn't already been used
        If WorkspacesNode.XMLNode.HasChildNodes AndAlso Not WorkspacesNode.XMLNode.SelectSingleNode("*[@name='" & PasteWorkspaceXML.GetAttribute("name") & "']") Is Nothing Then
            'the name is already in use, so generate a new, hopefully unique name
            'Note: this could be improved by recursively calling a sub that appends a number to the end of the name
            'like data-->data1-->data2 etc. As written it should be good enough.
            Select Case PasteWorkspaceXML.Name
                Case "ACCESSWORKSPACE"
                    PasteWorkspaceXML.SetAttribute("name", "acc-ws_" & PasteWorkspaceXML.GetAttribute("database").Replace(" ", "_"))
                Case "SHAPEWORKSPACE"
                    PasteWorkspaceXML.SetAttribute("name", "shp-ws_" & PasteWorkspaceXML.GetAttribute("directory").Replace(" ", "_"))
                Case "SDEWORKSPACE"
                    PasteWorkspaceXML.SetAttribute("name", "sde-ws_" & PasteWorkspaceXML.GetAttribute("server").ToLower & "_" & PasteWorkspaceXML.GetAttribute("instance").ToLower & "_" & PasteWorkspaceXML.GetAttribute("user").ToLower)
            End Select
        End If

        'make sure this now belongs to our document
        PasteWorkspaceXML = m_XmlDoc.ImportNode(PasteWorkspaceXML, True)
        WorkspacesNode.XMLNode.AppendChild(PasteWorkspaceXML)

        'create a node and add it to the tree
        Dim PasteWorkspaceNode As ERANode = EraNodeFactory.CreateEraNode(PasteWorkspaceXML)
        WorkspacesNode.Nodes.Add(PasteWorkspaceNode)
        PasteWorkspaceNode.EnsureVisible()
        Me.Dirty = True
        Return PasteWorkspaceXML.GetAttribute("name")
    End Function
    'Private Function PasteWorkspace(ByVal PasteWorkspaceNode As ERANode) As String
    '    For Each WorkspaceNode As ERANode In WorkspacesNode.Nodes
    '        If WorkspaceNode.ERANodeType = PasteWorkspaceNode.ERANodeType Then
    '            Select Case WorkspaceNode.ERANodeType
    '                Case ERANode.ERANodeTypeEnum.AccessWorkspace
    '                    If WorkspaceNode.GetAttributeValue("database") = _
    '                    PasteWorkspaceNode.GetAttributeValue("database") Then
    '                        Return WorkspaceNode.GetAttributeValue("name")
    '                    End If
    '                Case ERANode.ERANodeTypeEnum.SDEWorkspace
    '                    If WorkspaceNode.GetAttributeValue("server") = _
    '                        PasteWorkspaceNode.GetAttributeValue("server") And _
    '                        WorkspaceNode.GetAttributeValue("instance") = _
    '                        PasteWorkspaceNode.GetAttributeValue("instance") And _
    '                        WorkspaceNode.GetAttributeValue("user") = _
    '                        PasteWorkspaceNode.GetAttributeValue("user") And _
    '                        WorkspaceNode.GetAttributeValue("database") = _
    '                        PasteWorkspaceNode.GetAttributeValue("database") Then
    '                        Return WorkspaceNode.GetAttributeValue("name")
    '                    End If
    '                Case ERANode.ERANodeTypeEnum.ShapeWorkspace
    '                    If WorkspaceNode.GetAttributeValue("directory") = _
    '                    PasteWorkspaceNode.GetAttributeValue("directory") Then
    '                        Return WorkspaceNode.GetAttributeValue("name")
    '                    End If
    '            End Select
    '        End If
    '    Next
    '    'if we make it this far, no match to an existing workspace has been found
    '    'so add a new one, making sure the new name hasn't already been used
    '    If Not WorkspacesNode.XMLNode.SelectSingleNode("@name='" & PasteWorkspaceNode.GetAttributeValue("name") & "'") Is Nothing Then
    '        Select Case PasteWorkspaceNode.XMLNode.Name
    '            Case "ACCESSWORKSPACE"
    '                PasteWorkspaceNode.SetAttributeValue("name", "acc-ws_" & PasteWorkspaceNode.GetAttributeValue("database").Replace(" ", "_"))
    '            Case "SHAPEWORKSPACE"
    '                PasteWorkspaceNode.SetAttributeValue("name", "shp-ws_" & PasteWorkspaceNode.GetAttributeValue("directory").Replace(" ", "_"))
    '            Case "SDEWORKSPACE"
    '                PasteWorkspaceNode.SetAttributeValue("name", "sde-ws_" & PasteWorkspaceNode.GetAttributeValue("server").ToLower & "_" & PasteWorkspaceNode.GetAttributeValue("instance").ToLower & "_" & PasteWorkspaceNode.GetAttributeValue("user").ToLower)
    '        End Select
    '    End If
    '    WorkspacesNode.Nodes.Add(PasteWorkspaceNode)
    '    PasteWorkspaceNode.EnsureVisible()
    '    Me.Dirty = True
    '    Return PasteWorkspaceNode.GetAttributeValue("name")

    'End Function

    'Recursively run through pastednode and its children to make sure that the
    'layer (and child tables) workspaces are present in the XML document, and 
    'link the nodes xml to this document
    Private Sub PasteLayerTableWorkspaces(ByRef PasteXML As Xml.XmlElement, ByVal PasteWorkspaces As Xml.XmlElement)
        If PasteWorkspaces Is Nothing Then Exit Sub 'will be nothing if pastedXML originally comes from this document
        'in which case there's no need to check the workspaces.
        'Note: if someone does the following: cut a layer, then delete the layer's workspace]
        'then re-paste the layer, the system will just set the pasted layer workspace to "", rather 
        'than replacing the workspace info. As this is an unusual sequence of events, we'll leave it
        'as it is unless and until we hear that it is causing problems.
        If PasteXML.Name = "LAYER" Or PasteXML.Name = "TABLE" Then
            PasteXML.SetAttribute("workspace", PasteWorkspace(PasteWorkspaces, PasteXML.GetAttribute("workspace")))
        End If
        For Each ChildXML As XmlElement In PasteXML.SelectNodes("TABLE")
            PasteLayerTableWorkspaces(ChildXML, PasteWorkspaces)
        Next

    End Sub

    'recursively works its way up through layers, tables and fields until it finds a layer
    'then returns the parent of the layer. NOTE: Will throw an error if passed a workspace or child of workspace
    Private Function GetParentIssue(ByVal ChildNode As ERANode) As ERANode
        Select Case ChildNode.ERANodeType
            Case ERANode.ERANodeTypeEnum.Issue
                Return ChildNode
            Case ERANode.ERANodeTypeEnum.Layer
                Return ChildNode.Parent
            Case Else
                Return GetParentIssue(ChildNode.Parent)
        End Select
    End Function


#End Region

    Private Sub ValidateItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ValidateItem.Click
        If ContextNode() Is Nothing Then Exit Sub

        If My.Application.ESRI_Status <> My.ESRI_Status_Enum.Connected Then
            If Not My.Application.ConnectToESRI Then
                MessageBox.Show("Unable to check out an ArcView, ArcEditor or ArcInfo license. Status returned:" & vbNewLine & My.Application.GetESRIStatusMessage)
                Exit Sub
            End If
        End If
        If Not myEraControl Is Nothing Then
            myEraControl.SaveChangesToNode()
        End If
        'Dim blnError As Boolean = False
        'ESRIHelpers.DoValidation(ContextNode, Me, blnError)
        'ESRIHelpers.ValidateNode(ContextNode, Me)
        ContextNode.ValidateNode()

        If ContextNode.HasError Then
            MessageBox.Show("Validation complete with errors found.", "Errors Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        ElseIf ContextNode.ChildHasError Then
            MessageBox.Show("Validation complete with errors found in child nodes.", "Errors Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            MessageBox.Show("Validation complete with no errors found for the selected report element.", "No Errors Found", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        If Not myEraControl Is Nothing Then myEraControl.ShowErrors()
    End Sub

#Region "Sorting Report Elements"
    Private Sub MoveUpItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MoveUpItem.Click
        If ContextNode() Is Nothing Then Exit Sub
        ContextNode.MoveUp()
    End Sub

    Private Sub MoveDownItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MoveDownItem.Click
        If ContextNode() Is Nothing Then Exit Sub
        ContextNode.MoveDown()
    End Sub

    Private Sub MoveToTopItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MoveToTopItem.Click
        If ContextNode() Is Nothing Then Exit Sub
        ContextNode.MoveToTop()
    End Sub

    Private Sub MoveToBottomItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MoveToBottomItem.Click
        If ContextNode() Is Nothing Then Exit Sub
        ContextNode.MoveToBottom()
    End Sub

#End Region

    Public Sub DeleteItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles DeleteItem.Click
        If ContextNode() Is Nothing Then Exit Sub
        'If Not ERAAnalysisTreeView.Focused Then Exit Sub
        Select Case ContextNode.ERANodeType
            Case ERANode.ERANodeTypeEnum.AccessWorkspace
                For Each node As ERANode In Me.ERAAnalysisTreeView.Nodes
                    If isUsedWorkspace(ContextNode, node) Then
                        MessageBox.Show("Unable Workspace that is being used")
                        Exit Sub
                    End If
                Next
            Case ERANode.ERANodeTypeEnum.SDEWorkspace
                For Each node As ERANode In Me.ERAAnalysisTreeView.Nodes
                    If isUsedWorkspace(ContextNode, node) Then
                        MessageBox.Show("You can't delete a Workspace that is being used")
                        Exit Sub
                    End If
                Next
            Case ERANode.ERANodeTypeEnum.ShapeWorkspace
                For Each node As ERANode In Me.ERAAnalysisTreeView.Nodes
                    If isUsedWorkspace(ContextNode, node) Then
                        MessageBox.Show("You can't delete a Workspace that is being used")
                        Exit Sub
                    End If
                Next
            Case ERANode.ERANodeTypeEnum.Workspaces
                MessageBox.Show("You can't delete the Workspaces item.")
                Exit Sub
        End Select
        If My.Settings.ConfirmDelete Then
            Dim qd As New ConfirmDialog
            qd.StartPosition = FormStartPosition.CenterScreen
            qd.ShowDialog()
            If qd.result = 0 Then
                Exit Sub
            ElseIf qd.result = 2 Then
                My.Settings.ConfirmDelete = False
                My.Settings.Save()
            End If
        End If
        ContextNode.RemoveEraNode()
        Dirty = True
    End Sub

    Private Function isUsedWorkspace(ByRef wsNode As ERANode, ByRef eraNode As ERANode) As Boolean
        If eraNode.GetAttributeValue("workspace") = wsNode.GetMyName Then
            Return True
        End If
        For Each node As ERANode In eraNode.Nodes
            If isUsedWorkspace(wsNode, node) Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function SaveFile() As Boolean
        'Saves the node tree to the XML file
        If Not myEraControl Is Nothing Then
            myEraControl.SaveChangesToNode()
        End If
        'this appears to be about making sure that the XML nodes are ordered
        'correctly in the output, by following the order of the ERANodes in 
        'the treeview. It isn't otherwise necessary, as the ERAControl.SaveChangesToNode
        'calls have already changed the XML. Even deleting or cutting nodes
        'have calls to edit the underlying XML. Only the move up/down/first/last
        'calls only affect the treenodes, but not the underlying XML. It might
        'be better to edit the XML in those handlers.
        'On further reflection, it appears that the New*_Click calls also only
        'affect the ERANodes collection in the treeview. The problem with this 
        'dichotomy in how we handle these situations leads to a problem when
        'pasting nodes, in that the XML may contain things not reflected in the
        'treeview, and vice versa. Pasted nodes are re-added twice, it would appear
        'm_XmlDoc.RemoveAll()
        'Dim root As XmlElement = m_XmlDoc.CreateElement("ERA_ANALYSIS")
        'For Each node As ERANode In ERAAnalysisTreeView.Nodes
        '    root.AppendChild(node.XMLNode)
        '    LinkChildrenXML(node)
        'Next
        'm_XmlDoc.AppendChild(root)
        If Filename = "" Then
            Dim SaveFileDialog As New SaveFileDialog
            SaveFileDialog.Filter = "ERAtools2 Configuration Files (*.era)|*.era|ERAtools2 Configuration XML Files (*.xml)|*.xml"
            If (SaveFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                Filename = SaveFileDialog.FileName
            Else
                Return False
            End If
        End If

        m_XmlDoc.DocumentElement.SetAttribute("title", txtReportTitle.Text)
        Text = Filename

        Try
            m_XmlDoc.Save(Filename)
            Dirty = False
        Catch ex As IOException
            MessageBox.Show("Error saving file: " & ex.Message, "Error Saving File", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
        Return True
    End Function


    Public Sub LinkChildrenXML(ByVal eraNode As ERANode)
        For Each node As ERANode In eraNode.Nodes
            If Not node.XMLNode.ParentNode Is Nothing Then
                node.XMLNode.ParentNode.RemoveChild(node.XMLNode)
            End If
            eraNode.XMLNode.AppendChild(node.XMLNode)
            LinkChildrenXML(node)
        Next
    End Sub

    Private Sub btnRevert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRevert.Click
        If Not myEraControl Is Nothing Then
            myEraControl.LoadData()
        End If
    End Sub

    Public Property hasChanged() As Boolean
        Get
            Return btnRevert.Enabled
        End Get
        Set(ByVal value As Boolean)
            btnRevert.Enabled = value
        End Set
    End Property

    Public Sub RunValidation()
        If My.Application.ESRI_Status <> My.ESRI_Status_Enum.Connected Then
            If Not My.Application.ConnectToESRI Then
                MessageBox.Show("Unable to run validation. Unable to check out an ArcView, ArcEditor or ArcInfo license. Status returned:" & vbNewLine & My.Application.GetESRIStatusMessage, "Unable to Validate", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If
        End If
        If Not myEraControl Is Nothing Then
            myEraControl.SaveChangesToNode()
        End If
        Dim blnError As Boolean = False
        If Me.ERAAnalysisTreeView.Nodes.Count < 2 Then
            MessageBox.Show("This is an empty document.", "Validation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If
        My.Application.StartWait()
        For Each node As ERANode In Me.ERAAnalysisTreeView.Nodes
            node.ValidateNode()
            If node.HasError Or node.ChildHasError Then blnError = True
        Next
        My.Application.EndWait()
        'alert user with messagebox when complete, tell them if there are any errors
        If blnError Then
            MessageBox.Show("Validation complete, errors were found. Errors are highlighted in red.", "Validation Complete", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            MessageBox.Show("Validation complete, no errors found.", "Validation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        If Not myEraControl Is Nothing Then
            myEraControl.ShowErrors()
        End If
    End Sub

    Private Sub frmReportDefinitionEditor_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Me.Dirty Then
            Select Case MessageBox.Show("Do you want to save the changes to " & Me.Text & "?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                Case Windows.Forms.DialogResult.Yes
                    If Not SaveFile() Then e.Cancel = True
                Case Windows.Forms.DialogResult.No
                    'nothing to do here
                Case Windows.Forms.DialogResult.Cancel
                    e.Cancel = True
            End Select
        End If
        My.Settings.TreePaneWidth = Me.SplitContainer1.SplitterDistance
        If Me.WindowState = FormWindowState.Maximized Then
            My.Settings.MaximizeChildren = True
        Else
            My.Settings.MaximizeChildren = False
        End If
        My.Settings.ChildrenSize = Me.ClientSize
        My.Settings.Save()
    End Sub

#Region "Drag/Drop--Currently not supported"
    'Private Sub ERAAnalysisTreeView_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles ERAAnalysisTreeView.ItemDrag
    '    If DirectCast(e.Item, ERANode).ERANodeType = ERANode.ERANodeTypeEnum.Layer Then
    '        Debug.Print("ItemDrag with layer, allowing move")
    '        DoDragDrop(e.Item, DragDropEffects.Move)
    '    Else
    '        Debug.Print("ItemDrag with something else, not allowing move")
    '        'do nothing--only layers can be moved or copied
    '        'todo: allow movement of tables, or workspaces (between documents)
    '    End If

    'End Sub

    'Private Sub ERAAnalysisTreeView_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ERAAnalysisTreeView.DragEnter
    '    If e.Data.GetDataPresent("ERAtools2Admin.ERANode", True) Then
    '        Debug.Print("DragEnter with a TreeNode--allowing move")
    '        ShowEffect(e)
    '    Else
    '        Debug.Print("DragEnter with something else--not allowing move")
    '        e.Effect = DragDropEffects.None
    '    End If
    'End Sub

    'Private Sub ERAAnalysisTreeView_DragOver(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ERAAnalysisTreeView.DragOver
    '    Debug.Print("DragOver")
    '    'Check that there is a TreeNode being dragged 
    '    If e.Data.GetDataPresent("ERAtools2Admin.ERANode", _
    '           True) = False Then Exit Sub

    '    'As the mouse moves over nodes, provide feedback to 
    '    'the user by highlighting the node that is the 
    '    'current drop target
    '    Dim pt As Point = _
    '        CType(sender, TreeView).PointToClient(New Point(e.X, e.Y))
    '    Dim targetNode As ERANode = ERAAnalysisTreeView.GetNodeAt(pt)

    '    'See if the targetNode is currently selected, 
    '    'if so no need to validate again
    '    If Not (ERAAnalysisTreeView.SelectedNode Is targetNode) Then
    '        'Select the    node currently under the cursor
    '        ERAAnalysisTreeView.SelectedNode = targetNode
    '    End If

    '    'Check that the selected node is not the dropNode and
    '    'also that it is not a child of the dropNode and 
    '    'therefore an invalid target
    '    Dim dropNode As ERANode = _
    '        CType(e.Data.GetData("ERAtools2Admin.ERANode"), _
    '        ERANode)

    '    'todo: this doesn't work. We need to ensure that the dropNode is not a child
    '    'of targetNode, and viceversa.
    '    'Do Until targetNode Is Nothing
    '    '    If targetNode Is dropNode Then
    '    '        Debug.Print("dragging onto child of self, not allowing")
    '    '        e.Effect = DragDropEffects.None
    '    '        Exit Sub
    '    '    End If
    '    '    targetNode = targetNode.Parent
    '    'Loop

    '    Debug.Print("comparing types")
    '    'Compare: 
    '    Select Case dropNode.ERANodeType
    '        Case ERANode.ERANodeTypeEnum.AccessWorkspace
    '            If targetNode.ERANodeType = ERANode.ERANodeTypeEnum.Workspaces Then
    '                ShowEffect(e)
    '            Else
    '                e.Effect = DragDropEffects.None
    '            End If
    '        Case ERANode.ERANodeTypeEnum.SDEWorkspace
    '            If targetNode.ERANodeType = ERANode.ERANodeTypeEnum.Workspaces Then
    '                ShowEffect(e)
    '            Else
    '                e.Effect = DragDropEffects.None
    '            End If
    '        Case ERANode.ERANodeTypeEnum.ShapeWorkspace
    '            If targetNode.ERANodeType = ERANode.ERANodeTypeEnum.Workspaces Then
    '                ShowEffect(e)
    '            Else
    '                e.Effect = DragDropEffects.None
    '            End If
    '        Case ERANode.ERANodeTypeEnum.Layer
    '            If targetNode.ERANodeType = ERANode.ERANodeTypeEnum.Issue Then
    '                ShowEffect(e)
    '            Else
    '                e.Effect = DragDropEffects.None
    '            End If
    '        Case Else
    '            e.Effect = DragDropEffects.None
    '    End Select

    'End Sub

    'Private Sub ShowEffect(ByVal e As System.Windows.Forms.DragEventArgs)
    '    Debug.Print("ShowEffect " & e.KeyState)
    '    If (e.KeyState And 8) = 8 Then
    '        Debug.Print("ShowEffect Copy")
    '        e.Effect = DragDropEffects.Copy
    '    Else
    '        Debug.Print("ShowEffect Move")
    '        e.Effect = DragDropEffects.Move
    '    End If
    'End Sub

    'Private Sub ERAAnalysisTreeView_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ERAAnalysisTreeView.DragDrop
    '    'Check that there is a TreeNode being dragged
    '    If e.Data.GetDataPresent("ERAtools2Admin.ERANode", _
    '          True) = False Then Exit Sub

    '    'Get the TreeNode being dragged
    '    Dim dropNode As ERANode = _
    '          CType(e.Data.GetData("ERAtools2Admin.ERANode"), _
    '          ERANode)

    '    'The target node should be selected from the DragOver event
    '    Dim targetNode As ERANode = ERAAnalysisTreeView.SelectedNode

    '    'Remove the drop node from its current location
    '    'todo: if shift pressed, make a copy
    '    dropNode.Remove()

    '    'If there is no targetNode add dropNode to the bottom of
    '    'the TreeView root nodes, otherwise add it to the end of
    '    'the dropNode child nodes
    '    If targetNode Is Nothing Then
    '        ERAAnalysisTreeView.Nodes.Add(dropNode)
    '    Else
    '        targetNode.Nodes.Add(dropNode)
    '    End If

    '    'Ensure the newley created node is visible to
    '    'the user and select it
    '    dropNode.EnsureVisible()
    '    ERAAnalysisTreeView.SelectedNode = dropNode

    '    'still to do: modify the underlying XML to ensure that the dropped node xml
    '    'is added to the document
    '    'not necessary? will have to test
    '    'Dim newXMLNode As Xml.XmlNode = dropNode.XMLNode.CloneNode(True)
    '    'targetNode.XMLNode.AppendChild(newXMLNode)

    '    'dropNode.XMLNode.ParentNode.RemoveChild(dropNode.XMLNode)

    'End Sub

#End Region

    Private Sub ERAAnalysisTreeView_NodeMouseHover(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseHoverEventArgs) Handles ERAAnalysisTreeView.NodeMouseHover
        Dim node As ERANode = e.Node
        If Not node Is Nothing Then
            ToolTip1.SetToolTip(Me.ERAAnalysisTreeView, node.ErrorText)
        End If

    End Sub

  

End Class
