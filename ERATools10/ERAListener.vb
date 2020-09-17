Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports System.IO
Imports System.Windows.Forms

<System.Runtime.InteropServices.ComVisible(False)> _
Friend Class EraListener
    'All Code is Copyright 2006, State of Florida, Department of Environmental Protection.
    'Written by Chris Sands



    Private m_AnalysisShape As IGeometry = Nothing
    Private m_BufferShape As IGeometry = Nothing
    Private m_AnalysisPlusBufferShape As IGeometry = Nothing
    Private Shared m_StaticInstance As EraListener

    Public Shared ReadOnly Property Current() As EraListener
        Get
            If m_StaticInstance Is Nothing Then
                m_StaticInstance = New EraListener
            End If
            Return m_StaticInstance
        End Get
    End Property

    Public ReadOnly Property AnalysisShape() As IGeometry
        Get
            Return m_AnalysisShape
        End Get
    End Property

    Public ReadOnly Property BufferShape() As IGeometry
        Get
            Return m_BufferShape
        End Get
    End Property

    Public ReadOnly Property AnalysisPlusBufferShape() As IGeometry
        Get
            Return m_AnalysisPlusBufferShape
        End Get
    End Property

    Private Sub New() 'frmTool As frmERAToolsGUI)
        Logging.Log.Debug("ERAListener Initialized")
    End Sub

    Private Sub CalculateBuffer(ByVal dBufferDistance As Double, ByVal bDisplay As Boolean)
        'Get the buffer
        Try
            If dBufferDistance = 0 Then
                m_AnalysisPlusBufferShape = m_AnalysisShape
                m_BufferShape = Nothing
                'buffer shape will be nothing, which is OK and is handled by the individual analyses
                Exit Sub
            End If
            'buffer distance > 0, so buffer the shape
            Dim pTopoOperator As ITopologicalOperator
            Dim pDiffTopoOp As ITopologicalOperator

            'We first clone the shape, as we may need to project it in order to determine the
            'appropriate buffer size in the coordinate system's map units
            Dim pShapeClone As ESRI.ArcGIS.Geometry.IGeometry = DirectCast(m_AnalysisShape, ESRI.ArcGIS.esriSystem.IClone).Clone

            'Determine the actual buffer distance to use in the units of the feature being analyzed.
            'If it's a feature in a geographic projection, project it to albers and use the
            'dBufferDistance (which has already been set to meters).
            'If it's a projected feature, get the meters per unit of the feature's coordinate system and 
            'use that to adjust the dBufferDistance.
            If m_AnalysisShape.SpatialReference IsNot Nothing Then
                If TypeOf (m_AnalysisShape.SpatialReference) Is ESRI.ArcGIS.Geometry.IGeographicCoordinateSystem Then
                    ProjectShapeToAlbers(pShapeClone)
                ElseIf TypeOf (m_AnalysisShape.SpatialReference) Is ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem Then
                    dBufferDistance /= DirectCast(m_AnalysisShape.SpatialReference, ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem).CoordinateUnit.MetersPerUnit
                End If
            End If

            If m_AnalysisShape.GeometryType = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultipoint Then
                Try
                    Dim pGeoCol As ESRI.ArcGIS.Geometry.IGeometryCollection = pShapeClone
                    Dim pPolygon As ESRI.ArcGIS.Geometry.IPolygon
                    Dim pGeoBag As ESRI.ArcGIS.Geometry.IGeometryCollection = New ESRI.ArcGIS.Geometry.GeometryBag
                    Dim iRow As Integer

                    DirectCast(pGeoBag, ESRI.ArcGIS.Geometry.IGeometry).SpatialReference = pShapeClone.SpatialReference
                    For iRow = 0 To pGeoCol.GeometryCount - 1
                        pTopoOperator = pGeoCol.Geometry(iRow)
                        pPolygon = pTopoOperator.Buffer(dBufferDistance)
                        pGeoBag.AddGeometry(pPolygon)
                    Next iRow

                    '2006 08 02 This resets pPolygon to an empty shape
                    'Without resetting the polygon, one of the shapes in the GeoBag gets dropped.
                    pPolygon = New ESRI.ArcGIS.Geometry.Polygon

                    pTopoOperator = pPolygon
                    pTopoOperator.ConstructUnion(pGeoBag)

                    m_BufferShape = pPolygon
                    Me.m_AnalysisPlusBufferShape = pPolygon

                Catch ex As Exception
                    Logging.Log.Error("Error in ERAListener.CalculateBuffer (Buffer Multipoint)", ex)
                    Throw
                End Try

            Else 'not a multi-point
                pTopoOperator = pShapeClone
                Me.m_AnalysisPlusBufferShape = pTopoOperator.Buffer(dBufferDistance)

                If m_AnalysisShape.GeometryType = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon Then
                    'Return the buffer only, minus the pShape::the donut, not the hole
                    pDiffTopoOp = m_AnalysisPlusBufferShape
                    m_BufferShape = pDiffTopoOp.Difference(pShapeClone)
                Else 'when the feature type is point or line, then the 
                    'buffer shape will be the same as the buffer plus feature shape
                    m_BufferShape = m_AnalysisPlusBufferShape
                End If
            End If

            'return the shape back to the original projection.
            m_BufferShape.Project(m_AnalysisShape.SpatialReference)
            m_AnalysisPlusBufferShape.Project(m_AnalysisShape.SpatialReference)
            'Re-draw the shapes on the map, now that we have a buffer
            'DisplayShapesOnMap(bDisplay)
            'I've changed this, it does it at the end now bb 3-12-08
        Catch ex As Exception
            Logging.Log.Error("Error in ERAListener.CalculateBuffer", ex)
            Throw
        End Try
    End Sub

    Public Sub DeleteShapes()
        MapDrawing.DeleteElements()
    End Sub


    Public Function AnalyzeShape(ByRef pFeatureShape As ESRI.ArcGIS.Geometry.IGeometry, ByVal FeatureShapeName As String, ByVal strSourceFile As String, ByVal SelectedStyleSheet As EraStyleSheet, ByVal strDestinationFile As String, ByVal dBufferDistance As Double, ByVal bDisplay As Boolean, ByVal strReportTitle As String) As Boolean

        Try
            Logging.Log.Debug("AnalyzeShape Called")

            If pFeatureShape Is Nothing Then
                Throw New EraException("The analysis feature has not been initialized.")
            End If
            If pFeatureShape.IsEmpty Then
                Throw New EraException("The analysis feature is an empty shape.")
            End If

            If pFeatureShape.Dimension = esriGeometryDimension.esriGeometry2Dimension Then
                Dim pArea As IArea = pFeatureShape
                If pArea.Area <= 0 Then
                    'is this an "EraException?"
                    Throw New EraException("Bad polygon passed to AnalyzeShape. Area is <= 0")
                End If
            End If

            Logging.Log.Debug("Passed analysis feature validation check.")

            m_AnalysisShape = pFeatureShape
            CalculateBuffer(dBufferDistance, bDisplay)

            'Get report author automatically from username
            Dim strAuthorName As String = My.Settings.AuthorName

            If strAuthorName = "" Then
                Try
                    AppDomain.CurrentDomain.SetPrincipalPolicy(Security.Principal.PrincipalPolicy.WindowsPrincipal)
                    Dim p As Security.Principal.WindowsPrincipal = System.Threading.Thread.CurrentPrincipal
                    Dim wi As Security.Principal.WindowsIdentity = p.Identity
                    strAuthorName = wi.Name
                Catch ex As Exception
                    Logging.Log.Error("Error obtaining username", ex)
                    'This isn't critical, so just let the user know and move on
                    strAuthorName = "Err: " & ex.Message
                End Try
            End If

            Dim m_Eval As New EraEvaluation(strAuthorName, strReportTitle, FeatureShapeName)
            Try
                'open and read the analysis definition file 
                Dim ReportConfigXmlDoc As New Xml.XmlDocument
                Logging.Log.Debug("Attempting to load " & strSourceFile)
                If IO.File.Exists(strSourceFile) Then
                    Logging.Log.Debug("The file exists")
                Else
                    Throw New IO.FileNotFoundException("The analysis definition file " & strSourceFile & " does not exist")
                End If

                ReportConfigXmlDoc.Load(strSourceFile)

                'this will recursively work its way all the way through the document
                'exceptions are thrown by the EraNode child classes that call SetXmlNode
                'each will look for conditions that can cause problems running the report
                '(invalid workspaces, missing layers, fields, etc.), and throw EraConfigurationExceptions
                'which are caught and reported below.
                'The only problem with doing this is that it bails at the first sign of trouble, and so does 
                'not report every error in the file if there's only one, but that's what the validation
                'function in the EraToolsAdmin is for.
                m_Eval.SetXmlNode(ReportConfigXmlDoc.FirstChild)
            Catch ex As EraException
                Logging.Log.Error("Error reading analysis definition file: a configuration error occurred reading the analysis definition file", ex)
                MessageBox.Show("Error reading analysis definition file: a configuration error occurred reading the analysis definition file: " & ex.Message, "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            Catch ex As Exception
                Logging.Log.Error("Unable to run report: a problem occurred reading the analysis definition file: ", ex)
                MessageBox.Show("Unable to run report: a problem occurred reading the analysis definition file: " & ex.Message, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End Try


            'Alrighty, now we're actually doing the analysis
            Logging.Log.Debug("Generate output file")
            'output results to results.xml
            'AddToLog("Before evaluation shapetypes are Feature::Buffer " & m_FeatureShape.GeometryType & "::" & m_BufferShape.GeometryType)
            Dim EraXmlResults As New Xml.XmlDocument
            EraXmlResults.AppendChild(EraXmlResults.CreateProcessingInstruction("xml-stylesheet", "href=""" & SelectedStyleSheet.FullPath & """ type=""text/xsl"""))
            EraXmlResults.AppendChild(EraXmlResults.CreateProcessingInstruction("html", "version='1.0' encoding='ISO-8859-1'"))
            m_Eval.WriteEvaluationXmlResults(EraXmlResults)

            'Now we're always going to save the XML

            Try
                EraXmlResults.Save(IO.Path.ChangeExtension(strDestinationFile, "xml"))
            Catch ex As IOException
                'We're just going to swallow this one, the import thing is the transform, below
                'unless of course the user has selected XML as the output type, then throw an error here
                If SelectedStyleSheet.DisplayName = "XML" Then
                    Throw 'New IOException("Error writing results file: " & ex.Message)
                Else
                    Logging.Log.Warn("Non-critical IO Exception writing results file to interim XML file: " & ex.ToString)
                End If
            End Try

            'Apply the stylesheet transformation as appropriate
            If SelectedStyleSheet.DisplayName <> "XML" Then
                'Transform the XMLResults document to an html document using the output stylesheet
                'write the transformed string out to a file
                Dim xslt As Xml.Xsl.XslCompiledTransform = SelectedStyleSheet.xslt
                Try
                    Dim ts As New Xml.XmlTextWriter(strDestinationFile, System.Text.Encoding.GetEncoding("iso-8859-1"))
                    xslt.Transform(EraXmlResults, ts)
                    ts.Close()
                Catch ex As IOException
                    Logging.Log.Error("IO Exception creating " & strDestinationFile, ex)
                    Throw
                Catch ex As Xml.Xsl.XsltException
                    Logging.Log.Error("XSL Transorm Exception transforming XML", ex)
                    Throw
                End Try
            End If

            'Draw the shape(s)
            MapDrawing.DrawShape(m_AnalysisShape, m_BufferShape)

            'Open the report in whatever application is configured to open the document within the user's operating system
            Try
                Dim p As New Process
                With p
                    With .StartInfo
                        .WindowStyle = ProcessWindowStyle.Normal
                        .FileName = strDestinationFile
                    End With
                    .Start()
                End With
                'The above method is now being used in place of the commented line that follows, in an attempt
                'to resolve an issue that occurs sporadically only on some PCs, where the IE window displays briefly
                'then loses focus and slips behind the ArcMap window.
                'p.StartInfo.WindowStyle = ProcessWindowStyle.Normal

                'These exceptions are generally non-critical, so we inform the user, log the exception, but still return true, as the creation of the output was successful, even though the file hasn't already been opened
            Catch ex As InvalidOperationException
                'No file name was specified in the Process component's StartInfo.
                '-or- 
                'The ProcessStartInfo.UseShellExecute member of the StartInfo property is true while ProcessStartInfo.RedirectStandardInput, ProcessStartInfo.RedirectStandardOutput, or ProcessStartInfo.RedirectStandardError is true. 
                MessageBox.Show("An InvalidOperationException error occurred opening the file " & strDestinationFile & ": " & ex.Message, "Error Opening File", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Logging.Log.Error("An InvalidOperationException error occurred opening the file " & strDestinationFile, ex)
            Catch ex As System.ComponentModel.Win32Exception
                'There was an error in opening the associated file. 
                MessageBox.Show("A Win32Exception error occurred opening the file " & strDestinationFile & ": " & ex.Message, "Error Opening File", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Logging.Log.Error("An Win32Exception error occurred opening the file " & strDestinationFile, ex)
            End Try

            Return True
        Catch ex As Exception
            Logging.Log.Error("Error in ERAListner.AnalyzeShape", ex)
            Return False
        End Try
    End Function
End Class
