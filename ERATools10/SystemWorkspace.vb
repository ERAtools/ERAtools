Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.DataSourcesGDB
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.esriSystem
Imports System.Windows.Forms

''' <summary>
''' SystemWorkspace connects to the ERAtools.mdb in the ERAtools System folder
''' This object makes the connection to the mdb, creates it if necessary,
''' sorts objects into their proper featureclasses
''' saves and retrieves features
''' projects features as necessary
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public Class SystemWorkspace
    'This is a singleton

    Private Shared m_Workspace As IFeatureWorkspace

    Private Sub New()
        'just to shut up FxCop, which complains because VS adds a public contstructor to this class.
    End Sub

    ''' <summary>
    ''' Gets or sets the database path.
    ''' </summary>
    ''' <value>The database path.</value>
    Shared Property DatabasePath() As String
        Get
            'read the location of the system workspace from app.config
            Dim strDatabase As String = My.Settings.ScratchWorkspace 'changed to just store the full path to the workspace
            If strDatabase = "" Then
                'use default, stored in the eratools system folder 
                Dim k As Microsoft.Win32.RegistryKey = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\ERAtools2")
                If k IsNot Nothing Then
                    strDatabase = IO.Path.Combine(k.GetValue("SystemFolder", "C:\myav8\ERAtools2\System"), "ERAtools.mdb")
                Else
                    strDatabase = "C:\myav8\ERAtools2\System\ERAtools.mdb"
                End If
            End If
            Return strDatabase
        End Get
        Set(ByVal value As String)
            My.Settings.ScratchWorkspace = value
            My.Settings.Save()
            m_Workspace = Nothing 'forces a reconnect/recreate in Workspace property
        End Set
    End Property

    ''' <summary>
    ''' Gets the workspace, creating it if necessary.
    ''' </summary>
    ''' <value>The system workspace.</value>
    Protected Shared ReadOnly Property Workspace() As IFeatureWorkspace
        Get
            If m_Workspace Is Nothing Then
                Dim strDatabase As String = DatabasePath
                Try
                    'If Not IO.File.Exists(strDatabase) Then
                    '    Dim ofd As New OpenFileDialog
                    '    With ofd
                    '        .Title = "Browse to the ERAtool2 scratch workspace geodatabase"
                    '        .AddExtension = True
                    '        .Filter = "Personal Geodatabase Files (*.mdb)|*.mdb"
                    '        Try
                    '            If IO.Directory.Exists(IO.Path.GetDirectoryName(strDatabase)) Then
                    '                .InitialDirectory = IO.Path.GetDirectoryName(strDatabase)
                    '            End If
                    '        Catch
                    '            'no worries
                    '        End Try


                    '        .Multiselect = False
                    '        .ShowReadOnly = False
                    '        .CheckPathExists = True
                    '        .CheckFileExists = False

                    '        If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    '            strDatabase = .FileName
                    '            DatabasePath = .FileName
                    '        Else
                    '            Throw New EraException("The scratch geodatabase file """ & strDatabase & """ was not found.")
                    '        End If
                    '    End With

                    'End If

                    If IO.File.Exists(strDatabase) Then
                        Dim pWorkspaceFactory As IWorkspaceFactory = New ESRI.ArcGIS.DataSourcesGDB.AccessWorkspaceFactory
                        Try
                            m_Workspace = pWorkspaceFactory.OpenFromFile(strDatabase, 0)
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
                    Else
                        'try to create the workspace
                        Try
                            Dim pWSF As IWorkspaceFactory = New AccessWorkspaceFactory
                            Dim pPropSet As IPropertySet = New PropertySet
                            Dim pWorkspaceName As IWorkspaceName = pWSF.Create(IO.Path.GetDirectoryName(strDatabase), IO.Path.GetFileName(strDatabase), pPropSet, Nothing)
                            Dim pName As IName = pWorkspaceName
                            m_Workspace = pName.Open

                        Catch ex As Exception
                            MessageBox.Show("Unable to create the scratch workspace used for storing saved shapes: " & ex.Message, "Unable to Create Workspace", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Try
                    End If
                Catch ex As EraException
                    MessageBox.Show("Unable to open the scratch workspace used for storing saved shapes: " & ex.Message, "Unable to Open Workspace", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
            Return m_Workspace
        End Get
    End Property

    ''' <summary>
    ''' Gets the default spatial reference.
    ''' </summary>
    ''' <value>The default spatial reference.</value>
    Public Shared ReadOnly Property DefaultSpatialReference() As ISpatialReference
        Get
            Dim pSpatialRefFact As ISpatialReferenceFactory2 = New SpatialReferenceEnvironment
            Dim srID As Integer = ESRI.ArcGIS.Geometry.esriSRGeoCSType.esriSRGeoCS_WGS1984
            Dim pSpatialReference As ISpatialReference = pSpatialRefFact.CreateSpatialReference(srID)
            pSpatialReference.SetDomain(-180, 180, -90, 90)
            Return pSpatialReference
        End Get
    End Property

    ''' <summary>
    ''' Gets the spatial reference currently used by feature classes in the system workspace, or those do not exist, the default spatial reference.
    ''' </summary>
    ''' <value>The spatial reference used by feature classes in the system workspace, or those do not exist, the default spatial reference..</value>
    Public Shared ReadOnly Property SpatialReference() As ISpatialReference
        Get
            'first we check to see if the scratch workspace exists. We call it this way, rather than the method in the Workspace function
            'because we don't want to create it yet if it doesn't exist.

            If Not IO.File.Exists(DatabasePath) Then
                'No scratch workspace exists yet, return default
                Return DefaultSpatialReference()
            End If

            'workspace file exists, try to get it from existing feature class
            If GetSpatialReferenceOfFeatureClass(esriGeometryType.esriGeometryPoint) IsNot Nothing Then Return GetSpatialReferenceOfFeatureClass(esriGeometryType.esriGeometryPoint)
            If GetSpatialReferenceOfFeatureClass(esriGeometryType.esriGeometryPolyline) IsNot Nothing Then Return GetSpatialReferenceOfFeatureClass(esriGeometryType.esriGeometryPoint)
            If GetSpatialReferenceOfFeatureClass(esriGeometryType.esriGeometryPolygon) IsNot Nothing Then Return GetSpatialReferenceOfFeatureClass(esriGeometryType.esriGeometryPoint)

            'if we've made it this far, the workspace exists, but has no scratch feature classes in it
            Return DefaultSpatialReference()

        End Get
    End Property

    ''' <summary>
    ''' Gets the spatial reference of one of the saved shape feature classes.
    ''' </summary>
    ''' <param name="shapeType">Indicates whether this will be the point, line or polygon feature class.</param>
    ''' <returns>The spatial reference of specified feature class feature class.</returns>
    Public Shared Function GetSpatialReferenceOfFeatureClass(ByVal shapeType As ESRI.ArcGIS.Geometry.esriGeometryType) As ISpatialReference
        Dim fc As IGeoDataset = GetFeatureClass(shapeType)
        If fc Is Nothing Then Return Nothing
        Return fc.SpatialReference
    End Function

    Public Shared Function GetFeatureClass(ByVal shapeType As ESRI.ArcGIS.Geometry.esriGeometryType) As ESRI.ArcGIS.Geodatabase.IFeatureClass
        Try
            Dim pFeatureClass As IFeatureClass = Nothing

            Dim ws As IWorkspace2 = Workspace

            Select Case shapeType
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint
                    If ws.NameExists(esriDatasetType.esriDTFeatureClass, "Point") Then
                        pFeatureClass = Workspace.OpenFeatureClass("Point")
                    Else
                        pFeatureClass = CreateFeatureClass("Point", esriGeometryType.esriGeometryPoint)
                    End If
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryLine
                    If ws.NameExists(esriDatasetType.esriDTFeatureClass, "Line") Then
                        pFeatureClass = Workspace.OpenFeatureClass("Line")
                    Else
                        pFeatureClass = CreateFeatureClass("Line", esriGeometryType.esriGeometryPolyline)
                    End If
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline
                    If ws.NameExists(esriDatasetType.esriDTFeatureClass, "Line") Then
                        pFeatureClass = Workspace.OpenFeatureClass("Line")
                    Else
                        pFeatureClass = CreateFeatureClass("Line", esriGeometryType.esriGeometryPolyline)
                    End If
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon
                    If ws.NameExists(esriDatasetType.esriDTFeatureClass, "Polygon") Then
                        pFeatureClass = Workspace.OpenFeatureClass("Polygon")
                    Else
                        pFeatureClass = CreateFeatureClass("Polygon", esriGeometryType.esriGeometryPolygon)
                    End If
            End Select
            Return pFeatureClass
        Catch ex As Exception
            MessageBox.Show("Error getting featureclass for " & shapeType.ToString & ": " & ex.ToString)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Creates one of the saved shape feature classes.
    ''' </summary>
    ''' <param name="FCName">Name of the feature class.</param>
    ''' <param name="shapeType">Geometry type of the feature class.</param>
    ''' <returns>Pointer to an IFeatureClass interface of the created feature class</returns>
    Private Shared Function CreateFeatureClass(ByVal FCName As String, ByVal shapeType As esriGeometryType) As ESRI.ArcGIS.Geodatabase.IFeatureClass
        Const ShapeFieldname As String = "SHAPE"
        Const OIDFieldname As String = "OBJECTID"
        Dim pFields As IFieldsEdit
        Dim pField As IFieldEdit
        Dim pFeatureClass As IFeatureClass
        Dim pGeoDefEdit As IGeometryDefEdit

        Try
            pGeoDefEdit = New GeometryDef()
            pGeoDefEdit.SpatialReference_2 = DefaultSpatialReference()
            pGeoDefEdit.GeometryType_2 = shapeType

            pFields = New Fields
            pField = New Field
            pField.Name_2 = OIDFieldname
            pField.Type_2 = esriFieldType.esriFieldTypeOID
            pFields.AddField(pField)
            pField = New Field
            pField.Name_2 = ShapeFieldname
            pField.Type_2 = esriFieldType.esriFieldTypeGeometry
            pField.GeometryDef_2 = pGeoDefEdit
            pFields.AddField(pField)

            'Update Fields
            pField = New Field
            pField.Name_2 = "ShapeName"
            pField.Type_2 = esriFieldType.esriFieldTypeString
            pFields.AddField(pField)
            pField = New Field
            pField.Name_2 = "DateCreated"
            pField.Type_2 = esriFieldType.esriFieldTypeDate
            pFields.AddField(pField)

            pFeatureClass = Workspace.CreateFeatureClass(FCName, pFields, Nothing, Nothing, esriFeatureType.esriFTSimple, ShapeFieldname, FCName & "FeatureClass")
            Return pFeatureClass
        Catch ex As Exception
            Logging.Log.Error("Error creating featureclass " & FCName, ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Saves the shape.
    ''' </summary>
    ''' <param name="pShape">A pointer to the shape to save.</param>
    ''' <param name="shapeName">User-specified name of the shape.</param>
    Public Shared Sub SaveShape(ByRef pShape As IGeometry, ByVal shapeName As String)
        Dim pFeatureClass As IFeatureClass
        Dim pFeature As IFeature
        Dim nField As Long
        Try
            Dim SpatRef As ISpatialReference = SpatialReference
            Dim pShapeClone As IGeometry = DirectCast(pShape, ESRI.ArcGIS.esriSystem.IClone).Clone
            If Not CompareSpatialReferences(pShape.SpatialReference, SpatRef) Then
                pShapeClone.Project(SpatRef)
            End If

            pFeatureClass = GetFeatureClass(pShapeClone.GeometryType)
            pFeature = pFeatureClass.CreateFeature()
            pFeature.Shape = pShapeClone

            'Store values in fields
            nField = pFeatureClass.FindField("ShapeName")
            If shapeName.Length > 255 Then
                pFeature.Value(nField) = shapeName.Substring(0, 255)
            Else
                pFeature.Value(nField) = shapeName
            End If
            nField = pFeatureClass.FindField("DateCreated")
            pFeature.Value(nField) = System.DateTime.Now
            pFeature.Store()
        Catch ex As Exception
            Logging.Log.Error("Error saving shape " & shapeName, ex)
            MessageBox.Show("An error occurred saving the shape: " & ex.Message, "Error Saving Shape", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Gets the saved shapes of the specified feature class
    ''' </summary>
    ''' <param name="shapeType">Type of the shape. Indicates whether this will be the point, line or polygon feature class.</param>
    ''' <returns>An pointer to a feature cursor of all features in the feature class.</returns>
    Public Shared Function GetSavedShapes(ByVal shapeType As esriGeometryType) As IFeatureCursor
        'open the featureclass indicated by strShapeType
        'select all features from the featureclass
        'return them in IEnumFeature
        Dim pFeatureClass As IFeatureClass
        Dim pFeatureList As IFeatureCursor

        Try
            pFeatureClass = GetFeatureClass(shapeType)
            'pFeatureList = pFeatureClass.Select(pQFilt, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, m_Workspace)
            pFeatureList = pFeatureClass.Search(Nothing, False)
            Dim pFeature As IFeature = pFeatureList.NextFeature
            Do While Not pFeature Is Nothing
                Debug.Print(pFeature.Value(0))
                pFeature = pFeatureList.NextFeature
            Loop
            pFeatureList = pFeatureClass.Search(Nothing, False)

            Return pFeatureList
        Catch ex As Exception
            Logging.Log.Error("Error getting saved shapes " & shapeType, ex)
            Return Nothing
        End Try

    End Function

    ''' <summary>
    ''' Gets the saved shapes of the specified feature class
    ''' </summary>
    ''' <param name="shapeType">Type of the shape. Indicates whether this will be the point, line or polygon feature class.</param>
    ''' <param name="whereClause">The where clause to filter the feature class by.</param>
    ''' <returns>An pointer to a feature cursor of all features in the feature class that match the where clause.</returns>
    Public Shared Function GetSavedShapes(ByVal shapeType As esriGeometryType, ByVal whereClause As String) As IFeatureCursor
        'open the featureclass indicated by strShapeType
        'select all features from the featureclass
        'return them in IEnumFeature
        Dim pFeatureClass As IFeatureClass
        Dim pFeatureList As IFeatureCursor
        Dim pQFilt As IQueryFilter

        Try
            pQFilt = New QueryFilter
            pQFilt.WhereClause = whereClause
            pFeatureClass = GetFeatureClass(shapeType)
            'pFeatureList = pFeatureClass.Select(pQFilt, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, m_Workspace)
            pFeatureList = pFeatureClass.Search(pQFilt, False)

            Return pFeatureList
        Catch ex As Exception
            Logging.Log.Error("Error getting saved shapes " & shapeType, ex)
            Return Nothing
        End Try

    End Function

    ''' <summary>
    ''' Deletes features of the referenced feature class.
    ''' </summary>
    ''' <param name="shapeType">Type of the shape. Indicates whether shapes will be deleted from the point, line or polygon feature class</param>
    ''' <param name="whereClause">The where clause. Features matching the where clause will be deleted.</param>
    Public Shared Sub DeleteFeatures(ByVal shapeType As esriGeometryType, ByVal whereClause As String)
        Dim pFeatureClass As IFeatureClass
        Dim pQFilt As IQueryFilter
        Dim pTable As ITable

        Try
            pQFilt = New QueryFilter
            pQFilt.WhereClause = whereClause
            pFeatureClass = GetFeatureClass(shapeType)
            pTable = pFeatureClass
            pTable.DeleteSearchedRows(pQFilt)

        Catch ex As Exception
            MessageBox.Show("Error deleting features: " & ex.Message, "Error Deleting Features", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Logging.Log.Error("Error deleting features", ex)
        End Try
    End Sub

End Class
