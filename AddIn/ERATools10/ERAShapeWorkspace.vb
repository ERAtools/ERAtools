''' <summary>
''' Represents an instance of a shapefile workspace
''' Other possible properties according to ArcXML standard include: 
''' codepage (String), geoindexdir (String), shared (Boolean)
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public Class EraShapeWorkspace
    Inherits EraWorkspace

    ''' <summary>
    ''' Gets the full path to the directory containing shapefiles.
    ''' </summary>
    ''' <value>The full path to the directory.</value>
    Public ReadOnly Property Directory() As String
        Get
            Return GetAttributeValue("directory", True)
        End Get
    End Property

    ''' <summary>
    ''' Opens an instance of an IWorkspace from a ShapefileWorkspaceFactory using the directory property.
    ''' </summary>
    Protected Overrides Sub OpenWorkspace()
        Dim pWorkspaceFactory As ESRI.ArcGIS.Geodatabase.IWorkspaceFactory
        Dim pWorkspace As ESRI.ArcGIS.Geodatabase.IWorkspace
        Dim strDirectory As String = Directory

        pWorkspaceFactory = New ESRI.ArcGIS.DataSourcesFile.ShapefileWorkspaceFactory

        If Not IO.Directory.Exists(strDirectory) Then
            Throw New EraException("The directory " & strDirectory & " does not exist.")
        End If
        Try
            pWorkspace = pWorkspaceFactory.OpenFromFile(strDirectory, 0)
            Dim DatasetNames As ESRI.ArcGIS.Geodatabase.IEnumDatasetName = pWorkspace.DatasetNames(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTAny)
            Dim DatasetName As ESRI.ArcGIS.Geodatabase.IDatasetName = DatasetNames.Next
            If DatasetName Is Nothing Then
                Throw New EraException("The directory " & strDirectory & " does not contain any datasets.")
            End If
            m_workspace = pWorkspace
        Catch ex As System.Runtime.InteropServices.COMException
            Throw New EraException("Error opening shapefile workspace: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Gets the name of the tag.
    ''' </summary>
    ''' <value>The name of the tag.</value>
    Public Overrides ReadOnly Property TagName() As String
        Get
            Return "SHAPEWORKSPACE"
        End Get
    End Property
End Class