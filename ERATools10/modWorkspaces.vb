<System.Runtime.InteropServices.ComVisible(False)> _
Module modWorkspaces
    'modWorkspaces

    'The sole purpose of this module is to make
    'a collection of workspaces a global object
    'so code can access workspaces by name
    Friend m_colWorkspaces As New Generic.Dictionary(Of String, EraWorkspace)

    Public Sub AddWorkspace(ByRef ws As EraWorkspace)
        If Not m_colWorkspaces.ContainsKey(ws.Name) Then
            m_colWorkspaces.Add(ws.Name, ws)
        Else
            Throw New EraException("A workspace with the ID " & ws.Name & " already exists in the global workspace collection. Workspace IDs must be unique within an analysis definition file.")
        End If
    End Sub

    Public Function GetWorkspace(ByRef strName As String) As ESRI.ArcGIS.Geodatabase.Workspace
        If m_colWorkspaces Is Nothing Then
            Throw New EraException("The global collection of workspaces has not been initialized.")
        End If
        If Not m_colWorkspaces.ContainsKey(strName) Then
            Throw New EraException("A workspace with the id " & strName & " was not found in the global workspaces collection.")
        End If
        Return m_colWorkspaces.Item(strName).Workspace
    End Function

    Public Sub ClearWorkspaces()
        m_colWorkspaces.Clear()
    End Sub
End Module