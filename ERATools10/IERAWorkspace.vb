''' <summary>
''' Abstract class from which all workspace nodes inherit (EraAccessWorkspace, EraShapeWorkspace and EraSDEWorkspace).
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public MustInherit Class EraWorkspace
    Inherits EraNode

    ''' <summary>
    ''' Pointer to an ESRI IWorkspace
    ''' </summary>
    Protected m_workspace As ESRI.ArcGIS.Geodatabase.IWorkspace

    ''' <summary>
    ''' Public property to get the workspace, calling the overriden OpenWorkspace
    ''' </summary>
    ''' <value>A pointer to an IWorkspace instance.</value>
    Public ReadOnly Property Workspace() As ESRI.ArcGIS.Geodatabase.IWorkspace
        Get
            If m_workspace Is Nothing Then
                Logging.Log.Debug("In " & Me.TagName & ".Workspace (Get) for " & Me.Name & " workspace is Nothing")
                Logging.Log.Debug("Attempting to open workspace " & Me.Name)
                OpenWorkspace()
                If m_workspace Is Nothing Then
                    Logging.Log.Error("In " & Me.TagName & ".Workspace (Get) for " & Me.Name & " workspace is STILL Nothing")
                End If
                Return m_workspace
            Else
                'Logging.Log.Debug("In " & Me.TagName & ".Workspace (Get) for " & Me.Name & " workspace is set")
                Return m_workspace
            End If
        End Get
    End Property

    ''' <summary>
    ''' Opens the workspace. Inherited classes must override this sub to set the m_workspace instance using methods specific to the type of workspace.
    ''' </summary>
    Protected MustOverride Sub OpenWorkspace()

    Public Overrides Sub SetXmlNode(ByRef Value As Xml.XmlElement)
        m_XmlElement = Value
        Me.Name = GetAttributeValue("name", True)
    End Sub

End Class

