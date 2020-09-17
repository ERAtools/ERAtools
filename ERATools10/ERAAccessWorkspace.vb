''' <summary>
''' Represents an instance of an Access geodatabase workspace
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public Class EraAccessWorkspace
    Inherits EraWorkspace

    ''' <summary>
    ''' Gets the full path and filename to the database.
    ''' </summary>
    ''' <value>The full path and filename to the database.</value>
    Public ReadOnly Property Database() As String
        Get
            Return GetAttributeValue("database", True)
        End Get
    End Property

    ''' <summary>
    ''' Opens an instance of an IWorkspace from an AccessWorkspaceFactory using the database property.
    ''' </summary>
    Protected Overrides Sub OpenWorkspace()
        Dim pWorkspaceFactory As ESRI.ArcGIS.Geodatabase.IWorkspaceFactory
        Dim pPropset As ESRI.ArcGIS.esriSystem.IPropertySet
        Dim strDatabase As String = Database

        'Each ERAReport includes some property set information
        'This can be passed in and stored in the property set object

        pPropset = New ESRI.ArcGIS.esriSystem.PropertySet
        pPropset.SetProperty("DATABASE", strDatabase)

        If IO.Path.GetExtension(strDatabase) = ".gdb" Then
            pWorkspaceFactory = New ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactory
        Else
            pWorkspaceFactory = New ESRI.ArcGIS.DataSourcesGDB.AccessWorkspaceFactory
        End If

        Try
            m_workspace = pWorkspaceFactory.Open(pPropset, 0)
        Catch ex As System.Runtime.InteropServices.COMException
            If ex.Message = "Exception from HRESULT: 0x80040213" Then
                'thrown when the file is not found, or is not really an MDB
                If IO.File.Exists(strDatabase) Then
                    Try
                        Dim fs As IO.FileStream = IO.File.Open(strDatabase, IO.FileMode.Open)
                        If fs.CanRead Then
                            'what's going on?
                            Throw New EraException("Unable to open the file """ & strDatabase & """. File is not a valid Access database file.")
                        End If
                        fs.Close()
                    Catch exIO As UnauthorizedAccessException
                        Throw New EraException("You do not have permission to open the file """ & strDatabase & """.")
                    End Try
                Else
                    Throw New EraException("Unable to open the file """ & strDatabase & """. File not found.")
                End If
            ElseIf ex.Message = "Exception from HRESULT: 0x80040216" Then
                'file is opened exclusivly
                Throw New EraException("Unable to open the file """ & strDatabase & """. The database file has been opened exclusively by another user.")
            Else
                'have no idea what's going on here, so just send the exception text back 
                Throw New EraException("Unable to open the Access Workspace """ & strDatabase & """. Error message returned: " & ex.Message)
            End If

        End Try
    End Sub

    ''' <summary>
    ''' Gets the name of the tag.
    ''' </summary>
    ''' <value>The name of the tag.</value>
    Public Overrides ReadOnly Property TagName() As String
        Get
            Return "ACCESSWORKSPACE"
        End Get
    End Property
End Class