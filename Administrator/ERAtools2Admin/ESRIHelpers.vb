Imports ESRI.ArcGIS.CatalogUI, ESRI.ArcGIS.Catalog, ESRI.ArcGIS.Carto, ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Framework, ESRI.ArcGIS.GeoDatabaseUI


Public Module ESRIHelpers

    'Public Function getIWorkspaceFromEraNode(ByRef eraNode As ERANode, Optional ByRef ErrorStatus As String = Nothing) As ESRI.ArcGIS.Geodatabase.IWorkspace
    '    If My.Application.ESRI_Status <> My.ESRI_Status_Enum.Connected Then
    '        Return Nothing
    '    End If
    '    If eraNode Is Nothing Then
    '        Return Nothing
    '    End If
    '    'My.Application.StartWait()
    '    Try
    '        Return DirectCast(eraNode, WorkspaceNode).GetWorkspace

    '        'Dim pPropset As ESRI.ArcGIS.esriSystem.IPropertySet = New ESRI.ArcGIS.esriSystem.PropertySet
    '        'Dim pFact As IWorkspaceFactory = Nothing

    '        'Select Case eraNode.ERANodeType
    '        '    Case ERAtools2Admin.ERANode.ERANodeTypeEnum.AccessWorkspace
    '        '        'Dim strDatabase As String = eraNode.GetAttributeValue("database")
    '        '        'If IO.File.Exists(strDatabase) Or IO.Directory.Exists(strDatabase) Then
    '        '        '    pPropset.SetProperty("DATABASE", strDatabase)
    '        '        '    If IO.Path.GetExtension(strDatabase) = ".gdb" Then
    '        '        '        pFact = New ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactory
    '        '        '    Else
    '        '        '        pFact = New ESRI.ArcGIS.DataSourcesGDB.AccessWorkspaceFactory
    '        '        '    End If
    '        '        'Else
    '        '        '    My.Application.EndWait()
    '        '        '    ErrorStatus = "File not found"
    '        '        '    Return Nothing
    '        '        'End If
    '        '        'If Not pFact.IsWorkspace(strDatabase) Then
    '        '        '    My.Application.EndWait()
    '        '        '    If Not ErrorStatus Is Nothing Then ErrorStatus = "Could not open geodatabase at " & strDatabase
    '        '        '    Return Nothing
    '        '        'End If
    '        '    Case ERAtools2Admin.ERANode.ERANodeTypeEnum.SDEWorkspace
    '        '        pPropset.SetProperty("Server", eraNode.GetAttributeValue("server"))
    '        '        pPropset.SetProperty("Instance", eraNode.GetAttributeValue("instance"))
    '        '        pPropset.SetProperty("Database", eraNode.GetAttributeValue("database")) ' Ignored with ArcSDE for Oracle 
    '        '        pPropset.SetProperty("user", eraNode.GetAttributeValue("user"))
    '        '        pPropset.SetProperty("password", eraNode.GetAttributeValue("password"))
    '        '        pPropset.SetProperty("version", "sde.DEFAULT")
    '        '        pFact = New ESRI.ArcGIS.DataSourcesGDB.SdeWorkspaceFactory
    '        '    Case ERAtools2Admin.ERANode.ERANodeTypeEnum.ShapeWorkspace
    '        '        Dim strFolder As String = eraNode.GetAttributeValue("directory")
    '        '        If IO.Directory.Exists(strFolder) Then
    '        '            pPropset.SetProperty("DATABASE", strFolder)
    '        '            pFact = New ESRI.ArcGIS.DataSourcesFile.ShapefileWorkspaceFactory
    '        '        Else
    '        '            My.Application.EndWait()
    '        '            ErrorStatus = "Folder not found"
    '        '            Return Nothing
    '        '        End If
    '        '        If Not pFact.IsWorkspace(strFolder) Then
    '        '            My.Application.EndWait()
    '        '            If Not ErrorStatus Is Nothing Then ErrorStatus = "Could not open workspace at " & strFolder
    '        '            Return Nothing
    '        '        End If
    '        '    Case Else 'shouldn't ever happen...
    '        '        My.Application.EndWait()
    '        '        Return Nothing
    '        'End Select
    '        'Dim ws As IWorkspace = pFact.Open(pPropset, Nothing)
    '        'My.Application.EndWait()
    '        'Return ws
    '    Catch ex As Exception
    '        My.Application.EndWait()
    '        If Not ErrorStatus Is Nothing Then ErrorStatus = "Error opening workspace: " & ex.Message
    '        Return Nothing
    '    End Try


    'End Function

    Public Class LayerConnectInfo
        Public LayerID As String
        Public WorkspaceID As String
    End Class

    Public Function BrowseForLayers(ByRef ctrl As ERAControl) As LayerConnectInfo
        If My.Application.ESRI_Status <> My.ESRI_Status_Enum.Connected Then
            If Not My.Application.ConnectToESRI Then
                MessageBox.Show("Unable to browse to layers because no valid ESRI license was found (" & My.Application.GetESRIStatusMessage & ")", "Unable to Browse Layers", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Return Nothing
            End If
        End If
        Dim OwnerDoc As Xml.XmlDocument = ctrl.ParentForm.WorkspacesNode.XMLNode.OwnerDocument
        Dim m_LayerConnectInfo As New LayerConnectInfo ' Xml.XmlElement = ctrl.ParentForm.WorkspacesNode.XMLNode.OwnerDocument.CreateElement("el")
        Try
            'Create an ArcCatalog open dataset dialog
            Dim pGxDialog As IGxDialog = New GxDialog
            pGxDialog.Title = "Select Dataset"
            pGxDialog.ButtonCaption = "Select"
            pGxDialog.RememberLocation = True

            'Filter the type of datasets to display in the dialog
            Dim strDefaultFilter As String = GetSetting(Application.ProductName, "MRU", "DefaultFilter", "Shapefile Feature Class")
            Dim pFilterCol As IGxObjectFilterCollection = pGxDialog
            pFilterCol.AddFilter(New GxFilterShapefiles, strDefaultFilter = "Shapefile Feature Class")
            pFilterCol.AddFilter(New GxFilterSDEFeatureClasses, strDefaultFilter = "SDE Feature Class")
            pFilterCol.AddFilter(New GxFilterPGDBFeatureClasses, strDefaultFilter = "Personal Geodatabase Feature Class")
            If TypeOf (ctrl) Is ERATable Then
                pFilterCol.AddFilter(New GxFilterTables, strDefaultFilter = "Tables")
                'pFilterCol.AddFilter(New GxFilterTablesAndFeatureClasses, strDefaultFilter = "Tables And Feature Classes")
            End If

            'Show the dialog
            Dim pEnumGx As IEnumGxObject = Nothing
            If Not pGxDialog.DoModalOpen(0, pEnumGx) Then
                Return Nothing
            End If

            Dim pGxDataset As IGxDataset = pEnumGx.Next
            If pGxDataset Is Nothing Then
                Return Nothing
            End If

            Try
                SaveSetting(Application.ProductName, "MRU", "DefaultFilter", pGxDataset.Dataset.Category)
            Catch ex As Exception
                MessageBox.Show("Error reading data: " & ex.Message, "Error Reading Data", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return Nothing
            End Try

            'Check to see if the selected dataset's workspace is already in the workspaces collection
            'If not, add it

            Dim m_WSXMLNode As Xml.XmlElement = Nothing

            Select Case pGxDataset.Dataset.Workspace.Type
                Case esriWorkspaceType.esriFileSystemWorkspace
                    'shapefile, dbf table, etc.
                    'Dim sws As New ERA_Shape_IWorkspace(pGxDataset.Dataset.Workspace)
                    m_WSXMLNode = ctrl.ParentForm.WorkspacesNode.XMLNode.SelectSingleNode("SHAPEWORKSPACE[@directory='" & pGxDataset.Dataset.Workspace.PathName & "']")
                    If m_WSXMLNode Is Nothing Then
                        m_WSXMLNode = ctrl.ParentForm.WorkspacesNode.XMLNode.OwnerDocument.CreateElement("SHAPEWORKSPACE")
                        Dim FolderPath As String = pGxDataset.Dataset.Workspace.PathName

                        m_WSXMLNode.SetAttribute("directory", FolderPath)
                        m_WSXMLNode.SetAttribute("name", "shp-ws_" & FolderPath.Replace(" ", "_"))
                        Dim m_EraNode As New ShapeWorkspaceNode
                        m_EraNode.XMLNode = m_WSXMLNode
                        ctrl.ParentForm.WorkspacesNode.Nodes.Add(m_EraNode)
                        ctrl.ParentForm.WorkspacesNode.XMLNode.AppendChild(m_WSXMLNode)
                    End If
                Case esriWorkspaceType.esriLocalDatabaseWorkspace
                    'personal geodatabase layer or table

                    'Dim aws As New ERA_Access_IWorkspace(pGxDataset.Dataset.Workspace)

                    'ooookay, this is a workaround to a very peculiar issue that seems to randomly affect browsing to geodatabases
                    'if I add this line here to dim pWorkspaceFactory and set = new AccessWorkspaceFactory, it solves the 
                    'problem documented in ERANode.AccessWorkspaceNode.GetWorkspaceFactory. It's essentially the same line of code
                    'as the following, but there it throws an invalid cast exception. There's something about this block of code
                    'that causes subsequent calls to similar lines of code to fail.
                    Try
                        Dim pWorkspaceFactory As IWorkspaceFactory = New ESRI.ArcGIS.DataSourcesGDB.AccessWorkspaceFactory
                        Dim pWorkspaceFactory2 As IWorkspaceFactory = New ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactory
                    Catch ex As Exception
                        Trace.WriteLine(ex.Message)
                    End Try

                    'If My.Settings.ConfirmFGDB Then
                    '    If pGxDataset.Dataset.Workspace.WorkspaceFactory.WorkspaceDescription(False) = "File Geodatabase" Then
                    '        Dim c As New ConfirmDialog
                    '        c.PromptLabel.Text = "Feature classes from file-based personal geodatabase are not supported on ArcMap 9.1. If you intend to distribute this .era file to users running 9.1, please be sure to only use Access-based personal geodatabase files."
                    '        c.btnNo.Visible = False
                    '        c.btnYes.Text = "OK"
                    '        c.Text = "9.1 Compatibility Warning"
                    '        c.Height += 30
                    '        c.PromptLabel.Height += 30
                    '        c.btnYes.Left = 109
                    '        c.ShowDialog()
                    '        If c.result = 2 Then
                    '            My.Settings.ConfirmFGDB = False
                    '            My.Settings.Save()
                    '        End If
                    '    End If
                    'End If


                    m_WSXMLNode = ctrl.ParentForm.WorkspacesNode.XMLNode.SelectSingleNode("ACCESSWORKSPACE[@database='" & pGxDataset.Dataset.Workspace.PathName & "']")
                    If m_WSXMLNode Is Nothing Then
                        m_WSXMLNode = OwnerDoc.CreateElement("ACCESSWORKSPACE")
                        Dim MDBPath As String = pGxDataset.Dataset.Workspace.PathName
                        m_WSXMLNode.SetAttribute("database", MDBPath) 'aws.MDBPath)
                        m_WSXMLNode.SetAttribute("name", "gdb-ws_" & MDBPath.Replace(" ", "_"))
                        Dim m_EraNode As New AccessWorkspaceNode
                        m_EraNode.XMLNode = m_WSXMLNode
                        ctrl.ParentForm.WorkspacesNode.Nodes.Add(m_EraNode)
                        ctrl.ParentForm.WorkspacesNode.XMLNode.AppendChild(m_WSXMLNode)
                    End If

                    'if the first "Dim pWOrkspaceFactory" line isn't there, then the invalid cast exception is thrown
                    'here (or in GetWorkspaceFactory, if this line is not here).
                    'Dim pWorkspaceFactory2 As IWorkspaceFactory = New ESRI.ArcGIS.DataSourcesGDB.AccessWorkspaceFactory

                Case esriWorkspaceType.esriRemoteDatabaseWorkspace
                    'sde layer or table
                    'Dim sdeCon As New ERA_Sde_IWorkspace(pGxDataset.Dataset.Workspace)
                    'With sdeCon
                    Dim PropertySet As ESRI.ArcGIS.esriSystem.IPropertySet = pGxDataset.Dataset.Workspace.ConnectionProperties

                    m_WSXMLNode = ctrl.ParentForm.WorkspacesNode.XMLNode.SelectSingleNode("SDEWORKSPACE[@instance='" & PropertySet.GetProperty("instance") & "' and @server='" & PropertySet.GetProperty("server") & "' and @user= '" & PropertySet.GetProperty("user") & "']")
                    If m_WSXMLNode Is Nothing Then
                        m_WSXMLNode = OwnerDoc.CreateElement("SDEWORKSPACE")
                        m_WSXMLNode.SetAttribute("name", "sde-ws_" & PropertySet.GetProperty("server").ToLower & "_" & PropertySet.GetProperty("instance").ToLower & "_" & PropertySet.GetProperty("user"))
                        m_WSXMLNode.SetAttribute("server", PropertySet.GetProperty("server"))
                        m_WSXMLNode.SetAttribute("instance", PropertySet.GetProperty("instance"))
                        'database is optional, and a call to get the property will result in a comexception
                        Try
                            m_WSXMLNode.SetAttribute("database", PropertySet.GetProperty("database"))
                        Catch ex As System.Runtime.InteropServices.COMException
                            m_WSXMLNode.SetAttribute("database", "")
                        End Try
                        m_WSXMLNode.SetAttribute("authentication_mode", PropertySet.GetProperty("authentication_mode"))
                        If m_WSXMLNode.GetAttribute("authentication_mode") = "DBMS" Then
                            m_WSXMLNode.SetAttribute("user", PropertySet.GetProperty("user"))
                            'because the password is encrypted once it makes its way into the propertyset, we can't retrieve it
                            'so we have to prompt the user to enter it again, to store it.
                            Dim strPassword As String = InputBox("Enter the password used to connect to the SDE server", "Enter SDE Password")
                            m_WSXMLNode.SetAttribute("password", strPassword)
                        End If
                        m_WSXMLNode.SetAttribute("version", PropertySet.GetProperty("version"))
                        Dim m_EraNode As New SDEWorkspaceNode
                        m_EraNode.XMLNode = m_WSXMLNode
                        ctrl.ParentForm.WorkspacesNode.Nodes.Add(m_EraNode)
                        ctrl.ParentForm.WorkspacesNode.XMLNode.AppendChild(m_WSXMLNode)
                    End If
                    'End With
            End Select
            ctrl.ParentForm.WorkspacesNode.Expand()

            m_LayerConnectInfo.WorkspaceID = m_WSXMLNode.GetAttribute("name")
            m_LayerConnectInfo.LayerID = pGxDataset.DatasetName.Name
            Return m_LayerConnectInfo
        Catch ex As Exception
            MessageBox.Show("Error browsing for layers: " & ex.Message, "Error Browsing for Layers", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End Try
    End Function

    Public Function WorkspaceContains(ByVal wspace As IFeatureWorkspace, ByVal LayerName As String, Optional ByVal FeatureClassOnly As Boolean = False) As Boolean
        If TypeOf wspace Is IWorkspace2 Then
            Dim pWorkspace2 As IWorkspace2 = wspace

            If FeatureClassOnly Then
                Return pWorkspace2.NameExists(esriDatasetType.esriDTFeatureClass, LayerName)
            Else
                Return pWorkspace2.NameExists(esriDatasetType.esriDTFeatureClass, LayerName) OrElse pWorkspace2.NameExists(esriDatasetType.esriDTTable, LayerName)
            End If
        Else
            'IWorkspace2 is not implemented, so use the brute method...
            Try
                If FeatureClassOnly Then
                    Dim fc As IFeatureClass = wspace.OpenFeatureClass(LayerName)
                    Return fc IsNot Nothing
                Else
                    Dim table As ITable = wspace.OpenTable(LayerName)
                    Return table IsNot Nothing
                End If
            Catch ex As Exception
                Return False
            End Try
        End If
    End Function
End Module
