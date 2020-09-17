Imports ESRI.ArcGIS.Geometry

<System.Runtime.InteropServices.ComVisible(False)> _
Module modProjections

    ''' <summary>
    ''' Projects the shape to Albers.
    ''' </summary>
    ''' <param name="pGeometry">A pointer to an instance of an IGeometry.</param>
    Public Sub ProjectShapeToAlbers(ByRef pGeometry As IGeometry)
        Dim pSpatialRefFact As ISpatialReferenceFactory2 = New SpatialReferenceEnvironment
        Dim pSpatialReference As ISpatialReference = pSpatialRefFact.CreateSpatialReference(3086)
        pGeometry.Project(pSpatialReference)
    End Sub

    ''' <summary>
    ''' Clones the referenced shape.
    ''' </summary>
    ''' <param name="pGeometry">A pointer to an IGeometry instance, which will be cloned.</param>
    ''' <returns></returns>
    Public Function CloneShape(ByRef pGeometry As IGeometry) As IGeometry
        Dim ShapeCloner As ESRI.ArcGIS.esriSystem.IClone
        ShapeCloner = pGeometry
        Return ShapeCloner.Clone
    End Function


    ''' <summary>
    ''' Gets the area of the referenced geometry in acres.
    ''' </summary>
    ''' <param name="pGeometry">A pointer to an IGeometry instance. If a point or line, will return 0. Throws an exception if the geometry has no spatial reference, or if the spatial reference is IUnknownCoordinateSystem.</param>
    ''' <returns>The acreage of the referenced geometry.</returns>
    Public Function GetAcreage(ByRef pGeometry As IGeometry) As Double
        'Assumes that a null reference or null geometry is 0 acreage
        If pGeometry Is Nothing OrElse pGeometry.IsEmpty Then Return 0

        If pGeometry.Dimension <> esriGeometryDimension.esriGeometry2Dimension Then
            Return 0
        End If

        'Conversion factor is the conversion factor between whatever coordinates the shape is based in and square meters
        'We assume it to be 1 unless we determine otherwise based on the spatial reference
        Dim ConversionFactor As Double = 1

        Try
            Dim pAreaShape As IGeometry = CloneShape(pGeometry)
            If pGeometry.SpatialReference Is Nothing Then
                Throw New EraException("Error in GetAcreage function. Geometry has no spatial reference.")
            Else
                If TypeOf (pGeometry.SpatialReference) Is ESRI.ArcGIS.Geometry.IUnknownCoordinateSystem Then
                    Throw New EraException("Error in GetAcreage function. Geometry has an ""unknown"" coordinate system.")
                ElseIf TypeOf (pGeometry.SpatialReference) Is ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem Then
                    Dim pcs As IProjectedCoordinateSystem = pGeometry.SpatialReference
                    Dim u As ILinearUnit = pcs.CoordinateUnit
                    ConversionFactor = u.MetersPerUnit
                Else
                    'By Process of elimination, it's a geographic coordinate system, which must be projected to calculate the acreage
                    ProjectShapeToAlbers(pAreaShape)
                End If
            End If

            Dim pArea As ESRI.ArcGIS.Geometry.IArea = pAreaShape
            '0.000247105 is the conversion factor between square meters and acres
            Return pArea.Area * 0.000247105 * ConversionFactor
        Catch ex As System.Runtime.InteropServices.COMException
            Logging.Log.Error("Error in modProjections.GetAcreage", ex)
            Throw
        End Try

    End Function


    ''' <summary>
    ''' Compares the spatial references and returns true if they are identical.
    ''' </summary>
    ''' <param name="pSourceSR">A pointer to one instance of an ISpatialReference.</param>
    ''' <param name="pTargetSR">A pointer to one instance of an ISpatialReference.</param>
    ''' <returns>True if the two spatial references are identical.</returns>
    Public Function CompareSpatialReferences(ByRef pSourceSR As ISpatialReference, ByRef pTargetSR As ISpatialReference) As Boolean

        'If one or the other lacks a spatial reference, don't assume them to be the same
        If pSourceSR Is Nothing Or pTargetSR Is Nothing Then Return False

        Dim pSourceClone As ESRI.ArcGIS.esriSystem.IClone = pSourceSR
        Dim pTargetClone As ESRI.ArcGIS.esriSystem.IClone = pTargetSR



        'Compare the coordinate system component of the spatial reference
        'If the comparison failed, return false and exit
        If Not pSourceClone.IsEqual(pTargetClone) Then Return False

        'We can also compare the XY precision to ensure the spatial references are equal
        Dim pSourceSR2 As ISpatialReference2 = pSourceSR

        'If the comparison failed, return false and exit
        If Not pSourceSR2.IsXYPrecisionEqual(pTargetSR) Then Return False

        'By process of elimination, they must be equivalent
        Return True
    End Function

End Module
