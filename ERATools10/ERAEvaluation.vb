<System.Runtime.InteropServices.ComVisible(False)> _
Public Class EraEvaluation
    Inherits EraNode

    'All Code is Copyright 2006, State of Florida, Department of Environmental Protection.
    'Written by Chris Sands

    Private m_ReportTitle As String
    Private m_Author As String
    Private m_strShapeName As String
    Private m_Issues As New Generic.List(Of EraIssue)

    Public Property Author() As String
        Get
            Author = m_Author
        End Get
        Set(ByVal Value As String)
            m_Author = Value
        End Set
    End Property

    Public Property ReportTitle() As String
        Get
            ReportTitle = m_ReportTitle
        End Get
        Set(ByVal Value As String)
            m_ReportTitle = Value
        End Set
    End Property

    Public ReadOnly Property ShapeName() As String
        Get
            Return m_strShapeName
        End Get
    End Property

    Public Sub New(ByVal strAuthorName As String, ByVal strReportTitle As String, ByVal strShapeName As String)
        'Set time/date, author, and title
        Logging.Log.Debug("Eval Title: " & strReportTitle & " Author: " & strAuthorName)
        Me.Author = strAuthorName
        Me.ReportTitle = strReportTitle
        'Me.DateTime = CStr(Now)
        m_strShapeName = strShapeName
        'm_DateTime = String.Format(Date.Now, "mmddyyyy_hh_nn_ss")
    End Sub



    Public Overrides Sub SetXmlNode(ByRef EraXmlElement As System.Xml.XmlElement)
        m_XmlElement = EraXmlElement
        If EraXmlElement Is Nothing Then
            Throw New EraException("Error reading analysis definition file: the root ERA_ANALYSIS element is missing from the analysis defintion file.")
        End If
        If EraXmlElement.Name <> "ERA_ANALYSIS" Then
            Throw New EraException("Error reading analysis definition file: this is not a valid ERA analysis definition file, the root node is named '" & EraXmlElement.FirstChild.Name & "', not 'ERA_ANALYSIS'. ")
        End If

        Dim WorkspacesNode As Xml.XmlNode = EraXmlElement.SelectSingleNode("WORKSPACES")
        If WorkspacesNode Is Nothing Then
            Throw New EraException("The WORKSPACES node is missing from the analysis definition file.")
        Else
            ClearWorkspaces()
            For Each WorkspaceNode As Xml.XmlElement In WorkspacesNode.SelectNodes("*")
                Dim newWorkspace As EraWorkspace
                Select Case WorkspaceNode.Name
                    Case "SDEWORKSPACE"
                        newWorkspace = New EraSdeWorkspace
                    Case "ACCESSWORKSPACE"
                        newWorkspace = New EraAccessWorkspace
                    Case "SHAPEWORKSPACE"
                        newWorkspace = New EraShapeWorkspace
                    Case Else
                        'unknown workspace type
                        Throw New EraException("An unrecognized XML element was found in the WORKSPACES node: " & WorkspaceNode.Name)
                End Select
                newWorkspace.SetXmlNode(WorkspaceNode)
                AddWorkspace(newWorkspace)
            Next WorkspaceNode
        End If

        For Each IssueNode As Xml.XmlElement In EraXmlElement.SelectNodes("ISSUE")
            Dim newIssue As New EraIssue
            newIssue.SetXmlNode(IssueNode)
            m_Issues.Add(newIssue)
        Next IssueNode
    End Sub

    Public Sub WriteEvaluationXmlResults(ByRef xDoc As Xml.XmlDocument)
        Dim strTemp As String
        Dim RootNode As Xml.XmlElement = xDoc.CreateElement("RESULTS", "")
        xDoc.AppendChild(RootNode)


        'Logging.Log.Debug("Append Title attribute")
        RootNode.SetAttribute("title", Me.ReportTitle)

        'Logging.Log.Debug("Append Author attribute")
        RootNode.SetAttribute("author", Me.Author)

        'Logging.Log.Debug("Append DateTime attribute")
        RootNode.SetAttribute("datetime", Date.Now.ToString("MMddyyyy hh:mm:ss"))

        strTemp = ShapeType(EraListener.Current.AnalysisShape)
        'Logging.Log.Debug("Append Analysis Shape Type " & strTemp)
        RootNode.SetAttribute("analysis_shape", strTemp)

        'Logging.Log.Debug("Append Analysis Shape Name " & Me.ShapeName)
        RootNode.SetAttribute("shape_name", Me.ShapeName)

        strTemp = GetAcreage(EraListener.Current.AnalysisShape)
        'Logging.Log.Debug("Append Analysis Shape Acreage " & strTemp)
        RootNode.SetAttribute("analysis_acreage", strTemp)

        strTemp = GetAcreage(EraListener.Current.BufferShape)
        'Logging.Log.Debug("Append Buffer Shape Acreage " & strTemp)
        RootNode.SetAttribute("buffer_acreage", strTemp)

        'Logging.Log.Debug("Append System Path " & My.Settings.OutputFormatFolder)
        RootNode.SetAttribute("system_path", My.Settings.OutputFormatFolder)

        'Logging.Log.Debug("Append Issue Result Nodes")
        Logging.Log.Debug(m_Issues.Count & " issues to process.")
        For Each newIssue As EraIssue In m_Issues
            newIssue.WriteIssueXmlResults(RootNode)
        Next newIssue

        'For Each newIssue In m_alIssue
        '    newIssue.WriteXmlErrors(RootNode)
        'Next newIssue
    End Sub

    Private Function ShapeType(ByRef pShape As ESRI.ArcGIS.Geometry.IGeometry) As String
        If pShape Is Nothing Then Return "Shape not specified"
        Select Case pShape.GeometryType
            Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint
                Return "Point"
            Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryLine
                Return "Line"
            Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline
                Return "Polyline"
            Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon
                Return "Polygon"
            Case Else
                Return "Unknown (" & pShape.GeometryType & ")"
        End Select
    End Function

    Public Overrides ReadOnly Property TagName() As String
        Get
            Return "ERA_ANALYSIS"
        End Get
    End Property


End Class
