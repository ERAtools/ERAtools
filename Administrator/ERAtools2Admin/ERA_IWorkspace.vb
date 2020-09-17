'Public MustInherit Class ERA_IWorkspace
'    Friend m_Workspace As ESRI.ArcGIS.Geodatabase.IWorkspace
'    Private m_Name As String = ""

'    Public Property Name() As String
'        Get
'            If m_Name = "" Then Return GenerateName()
'            Return m_Name
'        End Get
'        Set(ByVal value As String)
'            m_Name = value
'        End Set
'    End Property

'    Friend MustOverride Function GenerateName() As String

'    Public XMLNode As Xml.XmlNode

'End Class

'Public Class ERA_Sde_IWorkspace
'    Inherits ERA_IWorkspace

'    Private m_PropertySet As ESRI.ArcGIS.esriSystem.IPropertySet

'    Public Sub New(ByRef pWorkspace As ESRI.ArcGIS.Geodatabase.IWorkspace)
'        m_Workspace = pWorkspace
'        Dim PropertySet As ESRI.ArcGIS.esriSystem.IPropertySet2 = pWorkspace.ConnectionProperties
'        m_PropertySet = PropertySet
'    End Sub

'    Public Property Instance() As String
'        Get
'            Return GetProperty("instance")
'        End Get
'        Set(ByVal value As String)
'            SetProperty("instance", value)
'        End Set
'    End Property

'    Public Property Server() As String
'        Get
'            Return GetProperty("server")
'        End Get
'        Set(ByVal value As String)
'            SetProperty("server", value)
'        End Set
'    End Property
'    Public Property Database() As String
'        Get
'            Return GetProperty("database")
'        End Get
'        Set(ByVal value As String)
'            SetProperty("database", value)
'        End Set
'    End Property
'    Public Property User() As String
'        Get
'            Return GetProperty("user")
'        End Get
'        Set(ByVal value As String)
'            SetProperty("user", value)
'        End Set
'    End Property

'    Private strPassword As String = ""
'    Public Property Password() As String
'        Get
'            'the saved connections store the password in an encrypted format, so we'll have to prompt 
'            'the user to enter the password so it can be stored as plain text in the output XML
'            If strPassword = "" Then
'                strPassword = InputBox("Enter the password to connect to the SDE server", "Enter Password")
'            End If
'            Return strPassword
'        End Get
'        Set(ByVal value As String)
'            strPassword = value
'            SetProperty("password", value)
'        End Set
'    End Property

'    Private Function GetProperty(ByVal PropertyName As String) As Object
'        Try
'            Return m_PropertySet.GetProperty(PropertyName)
'        Catch ex As Exception
'            Return ""
'        End Try
'    End Function

'    Private Sub SetProperty(ByVal PropertyName As String, ByVal PropertyValue As Object)
'        Try
'            m_PropertySet.SetProperty(PropertyName, PropertyValue)
'        Catch ex As Exception
'            Throw
'        End Try
'    End Sub

'    Friend Overrides Function GenerateName() As String
'        Return "sde-ws_" & Server.ToLower & "_" & Instance.ToLower & "_" & User
'    End Function

'End Class

'Public Class ERA_Shape_IWorkspace
'    Inherits ERA_IWorkspace

'    Public Directory As String

'    Public Sub New(ByRef pWorkspace As ESRI.ArcGIS.Geodatabase.IWorkspace)
'        Directory = pWorkspace.PathName
'    End Sub

'    Friend Overrides Function GenerateName() As String
'        Return "shp-ws_" & Directory.Replace(" ", "_")
'    End Function

'End Class

'Public Class ERA_Access_IWorkspace
'    Inherits ERA_IWorkspace

'    Public MDBPath As String

'    Public Sub New(ByRef pWorkspace As ESRI.ArcGIS.Geodatabase.IWorkspace)
'        MDBPath = pWorkspace.PathName
'    End Sub

'    Friend Overrides Function GenerateName() As String
'        Return "acc-ws_" & MDBPath.Replace(" ", "_")
'    End Function

'End Class

'Public Class ERA_FileGeodatabase_IWorkspace
'    Inherits ERA_IWorkspace

'    Public GDBPath As String

'    Public Sub New(ByVal pWorkspace As ESRI.ArcGIS.Geodatabase.IWorkspace)
'        GDBPath = pWorkspace.PathName
'    End Sub

'    Friend Overrides Function GenerateName() As String
'        Return "gdb-ws_" & GDBPath.Replace(" ", "_")
'    End Function

'End Class
