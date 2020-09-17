
''' <summary>
''' Represents an instance of an SDE workspace. Does not implement all properties of the ArcXML standard.
''' Other possible properties include: encrypted (Boolean), geoindexdir (String), localcodepage (Boolean)
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public Class EraSdeWorkspace
    Inherits EraWorkspace

    ''' <summary>
    ''' Gets the SDE instance property set by the SDEWORKSPACE XML element in a configuration file.
    ''' </summary>
    ''' <value>The SDE instance.</value>
    Public ReadOnly Property Instance() As String
        Get
            Return GetAttributeValue("instance", True)
        End Get
    End Property

    ''' <summary>
    ''' Gets the SDE server property set by the SDEWORKSPACE XML element in a configuration file.
    ''' </summary>
    ''' <value>The server.</value>
    Public ReadOnly Property Server() As String
        Get
            Return GetAttributeValue("server", False)
        End Get
    End Property

    ''' <summary>
    ''' Gets the SDE server property set by the SDEWORKSPACE XML element in a configuration file.
    ''' </summary>
    ''' <value>The database.</value>
    Public ReadOnly Property Database() As String
        Get
            Return GetAttributeValue("database", False)
        End Get
    End Property

    ''' <summary>
    ''' Gets the user property (username used to connect to the SDE instance) set by the SDEWORKSPACE XML element in a configuration file. 
    ''' </summary>
    ''' <value>The user.</value>
    Public ReadOnly Property User() As String
        Get
            Return GetAttributeValue("user", Me.AuthenticationMode = "DBMS")
        End Get
    End Property

    ''' <summary>
    ''' Gets the password property set by the SDEWORKSPACE XML element in a configuration file.
    ''' </summary>
    ''' <value>The password.</value>
    Public ReadOnly Property Password() As String
        Get
            Return GetAttributeValue("password", Me.AuthenticationMode = "DBMS")
        End Get
    End Property

    ''' <summary>
    ''' Gets the version property set by the SDEWORKSPACE XML element in a configuration file.
    ''' </summary>
    ''' <value>The version. If not provided, returns "SDE.DEFAULT".</value>
    Public ReadOnly Property Version() As String
        Get
            Dim strVersion As String = GetAttributeValue("version", False)
            If strVersion = "" Then Return "SDE.DEFAULT"
            Return strVersion
        End Get
    End Property

    ''' <summary>
    ''' Gets the authentication mode property set by the SDEWORKSPACE XML element in a configuration file.
    ''' </summary>
    ''' <value>The authentication mode, either OSA or DBMS. If not provided, DBMS is the default.</value>
    Public ReadOnly Property AuthenticationMode() As String
        Get
            Dim strAuthMode As String = GetAttributeValue("authentication_mode", False)
            If strAuthMode = "" Then Return "DBMS"
            Return strAuthMode
        End Get
    End Property

    ''' <summary>
    ''' Opens an instance of an Iworkspace from an SdeWorkspaceFactory using the server/instance/user/password/database properties.
    ''' </summary>
    Protected Overrides Sub OpenWorkspace()
        Dim pWorkspaceFactory As ESRI.ArcGIS.Geodatabase.IWorkspaceFactory
        Dim pPropset As ESRI.ArcGIS.esriSystem.IPropertySet

        'Each ERAReport includes some property set information
        'This can be passed in and stored in the property set object
        Try
            pPropset = New ESRI.ArcGIS.esriSystem.PropertySet
            With pPropset
                If Me.Server <> "" Then .SetProperty("SERVER", Me.Server)
                .SetProperty("INSTANCE", Me.Instance)
                .SetProperty("AUTHENTICATION_MODE", Me.AuthenticationMode)
                If Me.AuthenticationMode = "DBMS" Then
                    .SetProperty("USER", Me.User)
                    .SetProperty("PASSWORD", Me.Password)
                End If
                .SetProperty("DATABASE", Me.Database)
                .SetProperty("VERSION", Me.Version)
            End With

            pWorkspaceFactory = New ESRI.ArcGIS.DataSourcesGDB.SdeWorkspaceFactory
            m_workspace = pWorkspaceFactory.Open(pPropset, 0)
        Catch ex As System.Runtime.InteropServices.COMException
            'The possible messages returned are 
            '    "Server machine not found"
            '    "Entry for SDE instance not found in services file"
            '    "Bad login user"
            '    "SDE not running on server" 'this can also occur if an invalid port (instance) is provided, but there's no way to determine whether it's a server not running SDE or a server running SDE on a different port.
            '    "Underlying DBMS did not accept username/password"
            '    "Version not found"
            'I think these are fairly self explanatory, so I'll just pass it along
            Throw New EraException("Error opening SDE workspace " & Me.Name & ": " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Gets the name of the tag.
    ''' </summary>
    ''' <value>The name of the tag.</value>
    Public Overrides ReadOnly Property TagName() As String
        Get
            Return "SDEWORKSPACE"
        End Get
    End Property
End Class