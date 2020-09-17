Imports System.IO
Imports System.Xml
Imports ESRI.ArcGIS.Geometry
Imports System.Windows.Forms

<System.Runtime.InteropServices.ComVisible(False)> _
Friend Class frmERAToolsGUI
    Inherits System.Windows.Forms.Form

    Private m_Units As String
    Private m_Distance As Double

    'used only when calling from the point, line or polygon tools
    Private m_pShape As ESRI.ArcGIS.Geometry.IGeometry
    'used only when running saved or selected features
    Private m_colFeatures As New Generic.List(Of ESRI.ArcGIS.Geodatabase.IFeature)


    Private m_RunMode As RunModeEnum
    Friend Enum RunModeEnum
        SingleOrMergedSavedOrSelectedFeature
        MultipleSavedOrSelectedFeatures
        SingleShape
    End Enum
    Public Property RunMode() As RunModeEnum
        Get
            Return m_RunMode
        End Get
        Set(ByVal value As RunModeEnum)
            m_RunMode = value
            If m_RunMode = RunModeEnum.SingleOrMergedSavedOrSelectedFeature Or m_RunMode = RunModeEnum.SingleShape Then
                'Single feature, allow simple naming based on user-supplied text
                Me.cboShapeNameField.Visible = False
                Me.txtShapeName.Visible = True
                Me.lblShapeName.Text = "Shape &Name"
            ElseIf Me.AllowNaming Then
                'We'll be generating multiple reports, so allow features to be named by selecting a field...
                Me.cboShapeNameField.Visible = True
                Me.txtShapeName.Visible = False
                Me.lblShapeName.Text = "&Name Field"
                Try
                    'get fields names from features, should be at least two features, but we'll just get the first
                    For i As Integer = 0 To m_colFeatures.Item(0).Fields.FieldCount - 1
                        Dim pField As ESRI.ArcGIS.Geodatabase.IField = m_colFeatures.Item(0).Fields.Field(i)
                        If pField.Type <> ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeGeometry And _
                           pField.Type <> ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeBlob And _
                           pField.Type <> ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeRaster And _
                           pField.Type <> ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeXML Then
                            cboShapeNameField.Items.Add(pField.Name)
                        End If
                    Next
                Catch ex As Exception
                    MessageBox.Show("Error in obtaining fields to use for naming shapes: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Else
                'AllowNaming property takes over
            End If
        End Set
    End Property

    ''' <summary>
    ''' Called only by the saved shapes form to send the features (not just shapes, but data as well) to this form. The associated data are used for naming purposes.
    ''' </summary>
    ''' <value>A generic.list of IFeature pointers.</value>
    Public WriteOnly Property Features() As Generic.List(Of ESRI.ArcGIS.Geodatabase.IFeature)
        Set(ByVal Value As Generic.List(Of ESRI.ArcGIS.Geodatabase.IFeature))
            'Dim pFeature As ESRI.ArcGIS.Geodatabase.IFeature
            'Dim pShape As ESRI.ArcGIS.Geometry.IGeometry
            Try
                m_colFeatures = Value
                'If m_colShapes Is Nothing Then m_colShapes = New ESRI.ArcGIS.Geometry.GeometryBag
                'DirectCast(m_colShapes, ESRI.ArcGIS.Geometry.IGeometry).SpatialReference = theMxDoc.FocusMap.SpatialReference
                'For i As Int16 = 0 To m_colFeatures.Count - 1
                '    pFeature = m_colFeatures.Item(i)
                '    pShape = pFeature.Shape
                '    pShape.Project(theMxDoc.FocusMap.SpatialReference)
                '    m_colShapes.AddGeometry(pShape)
                'Next
                'm_pShape = CreateUnionedShape()
            Catch ex As Exception
                MessageBox.Show("Error in Features: " & ex.Message, "Error in Features", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Set
    End Property

    ''' <summary>
    ''' Sets the shape. Called only by an inherited class of the ERABaseTool class to set the simple point, line or polygon drawn by the user.
    ''' </summary>
    ''' <value>A pointer to an IGeometry instance.</value>
    Public WriteOnly Property Shape() As ESRI.ArcGIS.Geometry.IGeometry
        Set(ByVal Value As ESRI.ArcGIS.Geometry.IGeometry)
            m_pShape = Value
        End Set
    End Property

    'Public Property NameShapes() As Boolean
    '    Get
    '        NameShapes = m_NameShapes
    '    End Get
    '    Set(ByVal value As Boolean)
    '        'm_NameShapes = value
    '        m_NameShapes = value
    '    End Set
    'End Property

    Public Property ShapeName() As String
        Get
            Return txtShapeName.Text
        End Get
        Set(ByVal value As String)
            txtShapeName.Text = value
        End Set
    End Property

    Private m_AllowSaving As Boolean = True
    Public Property AllowSaving() As Boolean
        Get
            Return m_AllowSaving
        End Get
        Set(ByVal value As Boolean)
            m_AllowSaving = value
        End Set
    End Property

    Public Property AllowNaming() As Boolean
        Get
            Return txtShapeName.Enabled
        End Get
        Set(ByVal value As Boolean)
            txtShapeName.Enabled = value
            lblShapeName.Enabled = value
        End Set
    End Property

    ''' <summary>
    ''' Adds the referenced feature to the forms collection of features, used for running the selected features option, to provide the list with field names
    ''' and the ability to determine shape names from feature attributes.
    ''' </summary>
    ''' <param name="pFeature">A pointer to an instance of an object implementing IFeature.</param>
    Public Sub AddFeature(ByRef pFeature As ESRI.ArcGIS.Geodatabase.IFeature)
        Me.m_colFeatures.Add(pFeature)
    End Sub

    Private Sub cmdCancel_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub SetDisplayBufferChecked()
        Dim d As Double
        If Double.TryParse(Me.txtBuffer.Text, d) AndAlso d > 0 Then
            'real number entered, enable and set checked if that's the preference
            Me.chkDisplay.Enabled = True
            Me.cboUnits.Enabled = True
            Me.chkDisplay.Checked = My.Settings.DisplayBuffer
        Else
            Me.chkDisplay.Checked = False
            Me.chkDisplay.Enabled = False
            Me.cboUnits.Enabled = False
        End If
    End Sub

    Private Sub frmERAToolsGUI_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
        Try

            chkSaveShape.Checked = My.Settings.SaveShape And m_AllowSaving
            chkSaveShape.Enabled = m_AllowSaving
            SetDisplayBufferChecked()

            PopulateReportsListBox()
            PopulateFormatsListBox()

            If My.Settings.LastFormat <> "" Then
                If cboOutputType.Items.Contains(My.Settings.LastFormat) Then _
                    cboOutputType.Text = My.Settings.LastFormat
            End If

            Me.txtTarget.Text = My.Settings.LastOutput
            Me.txtBuffer.Text = My.Settings.LastBufferSize
            Me.cboUnits.Text = My.Settings.LastBufferUnit

            If m_pShape IsNot Nothing Then 'm_pShape will be nothing when the form is opened to run against saved selected features, so there's nothing to draw on the map
                MapDrawing.DrawShape(m_pShape)
            End If
        Catch ex As Exception
            MessageBox.Show("Error opening ERAtools window: " & ex.Message, "Error Opening ERAtools", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub frmERAToolsGUI_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        System.Windows.Forms.Cursor.Current = Cursors.Default
        'CloseLogFile()
    End Sub

    Private Sub txtBuffer_Enter(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles txtBuffer.Enter
        If txtBuffer.Text = "0" Then txtBuffer.Text = ""
    End Sub

    Private Sub txtBuffer_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBuffer.Leave
        If txtBuffer.Text = "" Then txtBuffer.Text = "0"
    End Sub

    Private Sub txtBuffer_KeyPress(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles txtBuffer.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        KeyAscii = ValidateDouble(KeyAscii)
        eventArgs.KeyChar = Chr(KeyAscii)
        If KeyAscii = 0 Then
            eventArgs.Handled = True
        End If
    End Sub

    Private Sub txtBuffer_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBuffer.TextChanged
        Dim d As Double = 0
        If (Double.TryParse(txtBuffer.Text, d) And d >= 0) Or txtBuffer.Text = "" Then
            ep.SetError(txtBuffer, "")
        Else
            ep.SetError(txtBuffer, "Enter a numeric value greater than or equal to 0")
        End If
        SetDisplayBufferChecked()
    End Sub

    Private Function CalcBufferInMeters() As Double
        'get conversion factors
        'inputs are feet, meters, kilometers, or miles
        'Const FEET_TO_METER As Double = 0.3048
        'Const KILOMETER_TO_METER As Double = 1000
        'Const MILE_TO_METER As Double = 1609.344
        'Const METER_TO_FEET As Double = 3.280839895
        'Const METER_TO_KILOMETER As Double = 0.001
        'Const METER_TO_MILE As Double = 0.00062137
        'Const FEET_TO_MILE As Double = 0.000189394
        'Const FEET_TO_KILOMETER As Double = 0.0003048
        'Const KILOMETER_TO_FEET As Double = 3280.839895013
        'Const KILOMETER_TO_MILE As Double = 0.621371192
        'Const MILE_TO_FEET As Double = 5280
        'Const MILE_TO_KILOMETER As Double = 1.609344

        'Convert distance to meters
        If m_Units = "Meters" Then
            Return m_Distance
        ElseIf m_Units = "Feet" Then
            Return m_Distance * 0.3048
        ElseIf m_Units = "Kilometers" Then
            Return m_Distance * 1000
        ElseIf m_Units = "Miles" Then
            Return m_Distance * 1609.344
        Else
            'Choices are limited by pick list in interface
            'If pick list changes, update this function
            Logging.Log.Error("Unable to determine conversion from " & m_Units & " to meters.")
            Return m_Distance
        End If
    End Function

    Dim Reports As New Generic.Dictionary(Of String, IO.FileInfo)
    Dim OutputFormats As New Generic.Dictionary(Of String, EraStyleSheet)

    'this is the version that reads from a configured directory, rather than the "browse/MRU" version deemed to complicated
    Private Sub PopulateReportsListBox(Optional ByVal FromFolder As String = "")
        cboReports.Items.Clear()
        Reports.Clear()
        Dim xmldoc As XmlDocument
        If FromFolder = "" Then FromFolder = My.Settings.LastSource
        'make sure we can read the source folder
        If FromFolder = "" Then Exit Sub

        Dim di As IO.DirectoryInfo = New IO.DirectoryInfo(IO.Path.GetDirectoryName(FromFolder))
        If di.Exists() Then
            'populate the Reports Listbox
            For Each fi As IO.FileInfo In di.GetFiles("*.era", IO.SearchOption.TopDirectoryOnly)
                Try
                    xmldoc = New XmlDocument
                    xmldoc.Load(fi.FullName)
                    If xmldoc.FirstChild.Name = "ERA_ANALYSIS" Then
                        cboReports.Items.Add(fi.Name)
                        Reports.Add(fi.Name, fi)
                    End If
                    xmldoc = Nothing
                Catch ex As Exception
                    Logging.Log.Warn("Unable to read the ERA Analysis Definition File """ & fi.FullName & """: " & ex.Message)
                End Try
            Next fi
            For Each fi As IO.FileInfo In di.GetFiles("*.xml", IO.SearchOption.TopDirectoryOnly)
                Try
                    xmldoc = New XmlDocument
                    xmldoc.Load(fi.FullName)
                    If xmldoc.FirstChild.Name = "ERA_ANALYSIS" Then
                        cboReports.Items.Add(fi.Name)
                        Reports.Add(fi.Name, fi)
                    End If
                    xmldoc = Nothing
                Catch ex As Exception
                    Logging.Log.Warn("Unable to read the ERA Analysis Definition File """ & fi.FullName & """: " & ex.Message)
                End Try
            Next fi
        Else
            'I don't think we really need to show a message box for this
            'MessageBox.Show("Error: The folder " & My.Settings.SourceFolder & " does not exist. Please select the reports source folder from the Settings menu.", "Folder not found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If

        If My.Settings.LastSource <> "" Then
            Dim fi As New IO.FileInfo(My.Settings.LastSource)
            If fi.Exists And cboReports.Items.Contains(fi.Name) Then
                cboReports.SelectedItem = fi.Name
            End If
        End If
    End Sub

    Private Sub PopulateFormatsListBox()
        Me.cboOutputType.Items.Clear()

        'make sure we can read the source folder
        Dim strF As String
        If System.Diagnostics.Debugger.IsAttached Then
            strF = "C:\ERAtools\Extension\ERATools\System"
        Else
            If My.Settings.OutputFormatFolder = "" Then

                'pull from the registry
                Dim k As Microsoft.Win32.RegistryKey = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\ERAtools2")
                If k IsNot Nothing Then
                    strF = k.GetValue("SystemFolder", "C:\myav8\ERAtools2\System")
                Else
                    strF = "C:\myav8\ERAtools2\System"
                End If
            Else
                strF = My.Settings.OutputFormatFolder
            End If
        End If

        Dim di As IO.DirectoryInfo = New IO.DirectoryInfo(strF)
        If di.Exists() Then
            'populate the Reports Listbox
            For Each fi As IO.FileInfo In di.GetFiles("*.xslt", IO.SearchOption.TopDirectoryOnly)
                Try
                    Dim ss As New EraStyleSheet(fi)
                    cboOutputType.Items.Add(ss.DisplayName)
                    OutputFormats.Add(ss.DisplayName, ss)
                Catch ex As ArgumentException
                    'file not found--just ignore it
                Catch ex As XmlException
                    MessageBox.Show("Error reading output format XSLT file " & fi.Name & ": " & ex.Message, "Error Reading XSLT File", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Catch ex As Exception
                    MessageBox.Show("Error reading output format XSLT file " & fi.Name & ": " & ex.Message, "Error Reading XSLT File", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Next fi
        Else
            'MessageBox.Show("Error reading output format XSLT files: the configured directory " & di.Name & " does not exist.", "Error Reading XSLT Files", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Dim fbd As New FolderBrowserDialog
            With fbd
                .Description = "The ERAtool2 folder (" & di.Name & ") was not found. Please browse to this folder."
                .ShowNewFolderButton = False
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    My.Settings.OutputFormatFolder = .SelectedPath
                    My.Settings.Save()
                    PopulateFormatsListBox()
                End If
            End With
        End If
        'If Not cboOutputType.Items.Contains("XML") Then cboOutputType.Items.Add("XML")
    End Sub

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        Dim sfd As New SaveFileDialog
        With sfd
            Dim ss As EraStyleSheet = GetSelectedStyleSheet()
            If ss Is Nothing Then
                MessageBox.Show("Select an output format first.", "Output Format Not Selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If
            .Filter = ss.DisplayName & " Document (*." & ss.Extension & ")|*." & ss.Extension
            .DefaultExt = ss.Extension
            If .ShowDialog = Windows.Forms.DialogResult.OK Then
                Me.txtTarget.Text = .FileName
                ep.SetError(btnBrowse, "")
            End If
        End With
    End Sub

    Private Sub btnBrowseReports_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBrowseReports.Click
        Dim ofd As New OpenFileDialog
        With ofd
            Try
                Dim fi As New IO.FileInfo(My.Settings.LastSource)
                If fi.Exists Then
                    .InitialDirectory = fi.DirectoryName
                End If
            Catch ex As System.ArgumentException
                'sokay, nothing to worry about
            End Try

            .Filter = "ERAtools2 Configuration Files (*.era)|*.era|ERAtools2 Configuration XML Files (*.xml)|*.xml"
            If .ShowDialog = Windows.Forms.DialogResult.OK Then
                My.Settings.LastSource = .FileName
                My.Settings.Save()
                Me.PopulateReportsListBox()
            End If
        End With
    End Sub

    ''' <summary>
    ''' Creates a unioned shape from the collection of features. Used only when running ERAtools with multiple saved or selected features.
    ''' </summary>
    ''' <returns>A pointer to a new IGeometry (multipoint, polyline or polygon)</returns>
    Private Function CreateUnionedShape() As ESRI.ArcGIS.Geometry.IGeometry
        Try
            If m_colFeatures.Count = 0 Then
                Throw New EraException("There are no shapes in the shape collection (m_colShapes.geometryCount = 0 and m_colFeatures.count = 0) in CreateUnionedShape")
            End If

            'nothing to union if it's just a single shape
            If m_colFeatures.Count = 1 Then
                Return m_colFeatures(0).ShapeCopy
            End If

            Dim m_colshapes As ESRI.ArcGIS.Geometry.IGeometryCollection = New GeometryBag
            Dim pFeature As ESRI.ArcGIS.Geodatabase.IFeature
            For Each pFeature In m_colFeatures
                If pFeature.Shape IsNot Nothing Then
                    m_colshapes.AddGeometry(pFeature.ShapeCopy)
                End If
            Next

            Dim pSpatialReference As ESRI.ArcGIS.Geometry.ISpatialReference = m_colFeatures(0).Shape.SpatialReference

            'Initialize the union shape based on the geometry type
            Dim pUnioningShape As ESRI.ArcGIS.Geometry.IGeometry
            Select Case m_colFeatures(0).Shape.GeometryType
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint
                    pUnioningShape = New ESRI.ArcGIS.Geometry.Multipoint
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultipoint
                    pUnioningShape = New ESRI.ArcGIS.Geometry.Multipoint
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryLine
                    pUnioningShape = New ESRI.ArcGIS.Geometry.Line
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline
                    pUnioningShape = New ESRI.ArcGIS.Geometry.Polyline
                Case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon
                    pUnioningShape = New ESRI.ArcGIS.Geometry.Polygon
                Case Else
                    Throw New EraException("Unable to create unioned shape: unsupported geometry type (" & m_colFeatures(0).Shape.GeometryType & ")")
            End Select

            'Construct the union
            Dim pTopoOperator As ESRI.ArcGIS.Geometry.ITopologicalOperator = pUnioningShape
            pTopoOperator.ConstructUnion(m_colshapes)

            pUnioningShape.SpatialReference = pSpatialReference

            'Return the unioned shape
            Return pUnioningShape
        Catch ex As Exception
            Logging.Log.Error("Error in CreateUnionedShape", ex)
            Throw
        End Try
    End Function

    'Validates user input, returns True if there are no problems
    Private Function ValidateUserInput() As Boolean
        Dim blnValid As Boolean = True 'assume true, set to false if a problem is found

        'Validate analysis definition file
        If cboReports.Text = "" Then
            ep.SetError(Me.btnBrowseReports, "Select an Analysis Definition File")
            blnValid = False
        Else
            Dim fiReport As IO.FileInfo = Reports(cboReports.Text)
            If fiReport Is Nothing Then
                ep.SetError(Me.btnBrowseReports, "Select an Analysis Definition File")
                blnValid = False
            End If

            If Not fiReport.Exists Then
                ep.SetError(Me.btnBrowseReports, "The selected analysis definition file could not be found.")
                PopulateReportsListBox()
                blnValid = False
            End If
        End If

        If blnValid Then ep.SetError(Me.btnBrowseReports, "")

        'validate output format
        If GetSelectedStyleSheet() Is Nothing Then
            ep.SetError(Me.cboOutputType, "You must select an output format")
            blnValid = False
        Else
            ep.SetError(cboOutputType, "")
        End If

        'validate buffer size and units
        If txtBuffer.Text = "" Then txtBuffer.Text = 0
        Dim d As Double
        If Double.TryParse(txtBuffer.Text, d) And d >= 0 Then
            ep.SetError(txtBuffer, "")
        Else
            ep.SetError(Me.txtBuffer, "You must enter a positive numeric value for buffer size. Enter 0 (or nothing) to run a report without a buffer.")
            blnValid = False
        End If

        'Validate output file
        'since we're now saving the XML file as well as the target (assuming that the user hasn't chosen XML output directly),
        'we need to make sure we can overwrite the XML file as well as the target.
        If txtTarget.Text = "" Then
            ep.SetError(btnBrowse, "Select an output file")
            blnValid = False
        Else
            ep.SetError(btnBrowse, "")

            Dim fiTarget As New IO.FileInfo(txtTarget.Text)
            With fiTarget
                If .Exists AndAlso My.Settings.WarnReplaceOutput Then
                    If MessageBox.Show("The output report file already exists. Do you want to overwrite it?", "Confirm Overwrite", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.No Then
                        ep.SetError(btnBrowse, "File Already Exists")
                        blnValid = False
                    Else
                        ep.SetError(btnBrowse, "")
                    End If
                End If

                If blnValid Then
                    If Not .Directory.Exists Then
                        ep.SetError(btnBrowse, "The output report directory does not exist. Please browse to another output file directory.")
                        blnValid = False
                    Else
                        'try to write to file
                        Try
                            Dim temp As IO.FileStream = .Create()
                            If Not temp.CanWrite Then
                                blnValid = False
                                ep.SetError(btnBrowse, "Unable to write to output file. Please browse to another output file.")
                            Else
                                ep.SetError(btnBrowse, "")
                            End If
                            temp.Close()
                        Catch ex As UnauthorizedAccessException
                            ep.SetError(btnBrowse, "Unauthorized access error creating output file: " & ex.Message)
                            blnValid = False
                        Catch ex As IO.IOException
                            ep.SetError(btnBrowse, "IO error creating output file: " & ex.Message)
                            blnValid = False
                        End Try
                    End If
                End If
            End With
        End If
        Return blnValid
    End Function

    Private Sub btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRun.Click
        Dim pMousePointer As System.Windows.Forms.Cursor = System.Windows.Forms.Cursor.Current
        Try
            If Not Me.ValidateUserInput Then
                MessageBox.Show("Unable to run report. Please correct the errors indicated on the form.", "Unable to Run Report", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If

            Dim dDistance As Double
            Dim strOutputType As String
            Dim pShape As ESRI.ArcGIS.Geometry.IGeometry
            Dim bSuccess As Boolean = True
            Dim fiTarget As New IO.FileInfo(txtTarget.Text)
            Dim fiReport As IO.FileInfo = Reports(cboReports.Text)

            If Me.chkDisplay.Enabled Then My.Settings.DisplayBuffer = Me.chkDisplay.Checked
            If Me.chkSaveShape.Enabled Then My.Settings.SaveShape = Me.chkSaveShape.Checked

            'Save MRU
            My.Settings.LastSource = fiReport.FullName
            My.Settings.LastOutput = fiTarget.FullName
            My.Settings.LastBufferSize = Me.txtBuffer.Text
            My.Settings.LastBufferUnit = Me.cboUnits.Text
            My.Settings.LastFormat = GetSelectedStyleSheet.DisplayName
            My.Settings.Save()

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor

            'Collect and store file paths
            'sourceReport is from cboReports
            'Stylesheet is determined by output type
            'outputFile is a default value, modifiable by the user
            'LogFile always writes to Local_Directory\System to a file named the same as the output file with a .log extension

            Logging.Log.Debug("Source file: " & fiReport.FullName)
            'strStyleSheet = GetSelectedStyleSheet.FullPath
            Logging.Log.Debug("Stylesheet: " & GetSelectedStyleSheet.FullPath)
            strOutputType = cboOutputType.Text ' VB6.GetItemString(lisOutputType, lisOutputType.SelectedIndex)
            Logging.Log.Debug("Output Type: " & strOutputType)

            'Collect and store Distance
            m_Distance = CDbl(Val(txtBuffer.Text))
            Logging.Log.Debug("Store Distance: " & m_Distance)
            m_Units = cboUnits.Text ' VB6.GetItemString(cboUnits, cboUnits.SelectedIndex)
            Logging.Log.Debug("Store Units: " & m_Units)
            dDistance = CalcBufferInMeters()
            Logging.Log.Debug("Calc Buffer: " & dDistance)

            'Initialize ERA Listener
            Dim m_ERAListener As EraListener = EraListener.Current
            m_ERAListener.DeleteShapes()
            If Me.RunMode = RunModeEnum.SingleShape Or Me.RunMode = RunModeEnum.SingleOrMergedSavedOrSelectedFeature Then
                If Me.RunMode = RunModeEnum.SingleShape Then
                    Logging.Log.Info("Single report for single digitized feature")
                    pShape = m_pShape
                Else
                    Logging.Log.Info("Single report for single or multiple saved or selected features")
                    pShape = CreateUnionedShape()
                End If
                'pShape.SpatialReference = theMxDoc.FocusMap.SpatialReference
                Dim strShapeName As String = Me.getShapeName(pShape)
                If chkSaveShape.Checked Then
                    SystemWorkspace.SaveShape(pShape, strShapeName)
                End If
                bSuccess = m_ERAListener.AnalyzeShape(pShape, strShapeName, fiReport.FullName, GetSelectedStyleSheet, txtTarget.Text, dDistance, chkDisplay.Checked, txtReportTitle.Text)
            Else 'multi shapes
                ' Run one report for each shape
                Logging.Log.Info("Multiple reports, one for each shape")
                Logging.Log.Debug("Looping through the collection of " & m_colFeatures.Count & " shapes")
                'For Each pShape In m_colShapes
                Dim i As Integer = 0
                For Each pFeature As ESRI.ArcGIS.Geodatabase.IFeature In m_colFeatures
                    i += 1
                    pShape = pFeature.Shape
                    'pShape.SpatialReference = Me.m_pMap.SpatialReference 'not necessary, now that we're using the pFeature, feature should have a projection already

                    Dim strShapeName As String
                    If Me.AllowNaming Then
                        If Me.cboShapeNameField.SelectedIndex >= 0 Then
                            'get the values from the feature
                            Dim objValue As Object = pFeature.Value(pFeature.Fields.FindField(Me.cboShapeNameField.Text))
                            If objValue Is Nothing Then
                                objValue = Me.getShapeName(pShape) 'use the default name ("unnamed point at ....")
                            End If
                            Try
                                strShapeName = objValue.ToString
                            Catch ex As Exception
                                strShapeName = "Error obtaining shape name: " & ex.Message
                            End Try
                        Else
                            If Me.cboShapeNameField.Text = "" Then
                                strShapeName = Me.getShapeName(pShape) 'use the default name ("unnamed point at ....")
                            Else
                                strShapeName = Me.cboShapeNameField.Text 'use the text the user has entered
                            End If
                        End If
                    Else
                        'uses the description for the saved shape
                        strShapeName = Nz(pFeature.Value(pFeature.Fields.FindField("ShapeName")), Me.getShapeName(pShape)) 'use the default name ("unnamed point at ....") if null
                    End If

                    Dim fi As New FileInfo(txtTarget.Text)
                    'replace invalid filename characters in strShapename (\/:*?"<>|)
                    Dim strShapeNameforFile As String = strShapeName
                    For Each badchar As Char In New Char() {"\", "\", "/", ":", "*", "?", """", "<", ">", "|"}
                        strShapeNameforFile = strShapeNameforFile.Replace(badchar, "_")
                    Next

                    'For multiple reports, to ensure unique filenames, we append both the shape name the user has provided, and just in case the shape name is not unique, an incremental number as well.
                    Dim strDestinationFile As String = fi.DirectoryName & "\" & fi.Name.Remove(fi.Name.Length - fi.Extension.Length) & "-" & i.ToString.PadLeft(3, "0") & "-" & strShapeNameforFile & fi.Extension
                    If chkSaveShape.Checked Then
                        SystemWorkspace.SaveShape(pShape, strShapeName)
                    End If
                    If Not m_ERAListener.AnalyzeShape(pShape, strShapeName, fiReport.FullName, GetSelectedStyleSheet, strDestinationFile, dDistance, chkDisplay.Checked, txtReportTitle.Text) Then
                        bSuccess = False
                    End If
                Next
            End If
            System.Windows.Forms.Cursor.Current = pMousePointer
            If Not bSuccess Then
                Dim logFilePath As String = Logging.LogFilePath
                If String.IsNullOrWhiteSpace(logFilePath) Then
                    MessageBox.Show("An error occurred processing the report. Please contact technical support. A log file should have been generated with more details regarding the error, but it can not currently be located.", "Error Running Report")
                ElseIf MessageBox.Show("An error occurred processing the report. Please contact technical support. A log file (" & logFilePath & ") has been generated with more details regarding the error. Do you want to view the log file?", "Error Running Report", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                    Try
                        Dim p As New Process
                        With p
                            With .StartInfo
                                .WindowStyle = ProcessWindowStyle.Normal
                                .FileName = logFilePath
                            End With
                            .Start()
                        End With
                        'These exceptions are generally non-critical, so we inform the user, log the exception, but still return true, as the creation of the output was successful, even though the file hasn't already been opened
                    Catch ex As InvalidOperationException
                        'No file name was specified in the Process component's StartInfo.
                        '-or- 
                        'The ProcessStartInfo.UseShellExecute member of the StartInfo property is true while ProcessStartInfo.RedirectStandardInput, ProcessStartInfo.RedirectStandardOutput, or ProcessStartInfo.RedirectStandardError is true. 
                        MessageBox.Show("An InvalidOperationException error occurred opening the file " & logFilePath & ": " & ex.Message, "Error Opening File", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Logging.Log.Error("An InvalidOperationException error occurred opening the file " & logFilePath, ex)
                    Catch ex As System.ComponentModel.Win32Exception
                        'There was an error in opening the associated file. 
                        MessageBox.Show("A Win32Exception error occurred opening the file " & logFilePath & ": " & ex.Message, "Error Opening File", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Logging.Log.Error("An Win32Exception error occurred opening the file " & logFilePath, ex)
                    Catch ex As Exception
                        'There was an error in opening the associated file. 
                        MessageBox.Show("An Error occurred opening the file " & logFilePath & ": " & ex.Message, "Error Opening File", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Logging.Log.Error("An Error occurred opening the file " & logFilePath, ex)
                    End Try
                End If
            End If
            Me.Close()
        Catch ex As Exception
            Logging.Log.Error("Error in btnRun_Click", ex)
            MessageBox.Show("Error running analysis: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        System.Windows.Forms.Cursor.Current = pMousePointer
    End Sub

    Private Function getShapeName(ByRef pShape As IGeometry) As String
        'If use has specified a name, just use that
        If txtShapeName.Text <> "" And txtShapeName.Text <> "<enter shape name>" Then Return txtShapeName.Text

        'If we made it this far, get a default "unnamed" shape name based on the shape geometry
        Try
            'based on the attributes of the shape, create a unique string identifying the shape
            Dim sbShapeName As New System.Text.StringBuilder("Unnamed ")

            Dim pPoint As IPoint
            Dim pArea As IArea

            Select Case pShape.GeometryType
                Case esriGeometryType.esriGeometryPoint
                    sbShapeName.Append("point")
                    pPoint = pShape
                Case esriGeometryType.esriGeometryLine
                    sbShapeName.Append("line")
                    pArea = pShape.Envelope
                    pPoint = pArea.Centroid
                Case esriGeometryType.esriGeometryPolyline
                    sbShapeName.Append("polyline")
                    pArea = pShape.Envelope
                    pPoint = pArea.Centroid
                Case esriGeometryType.esriGeometryPolygon
                    sbShapeName.Append("polygon")
                    pArea = pShape.Envelope
                    pPoint = pArea.Centroid
                Case Else
                    sbShapeName.Append("shape (type " & pShape.GeometryType & ")")
                    pPoint = New ESRI.ArcGIS.Geometry.Point
                    pPoint.X = 0
                    pPoint.Y = 0
            End Select

            Dim pPointClone As IPoint = DirectCast(pPoint, ESRI.ArcGIS.esriSystem.IClone).Clone

            Dim pSpatialRefFact As ISpatialReferenceFactory = New SpatialReferenceEnvironment

            'Dim geoSpatialReference As ISpatialReference = New GeographicCoordinateSystem
            Dim pGeoCooSys As IGeographicCoordinateSystem = pSpatialRefFact.CreateGeographicCoordinateSystem(esriSRGeoCSType.esriSRGeoCS_NAD1983)

            pPointClone.Project(pGeoCooSys)

            If pShape.GeometryType = esriGeometryType.esriGeometryPoint Then
                sbShapeName.Append(" located at ")
            Else
                sbShapeName.Append(" centered at ")
            End If
            sbShapeName.Append(Math.Round(pPointClone.X, 6) & " °, " & Math.Round(pPointClone.Y, 6) & " °")
            Return sbShapeName.ToString
        Catch ex As Exception
            Return "Error obtaining shape name: " & ex.Message
        End Try
    End Function

    Private Function GetSelectedStyleSheet() As EraStyleSheet
        If OutputFormats.ContainsKey(Me.cboOutputType.Text) Then Return OutputFormats.Item(Me.cboOutputType.Text)
        Return Nothing
    End Function

    Private Sub cboOutputType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboOutputType.SelectedIndexChanged
        If cboOutputType.SelectedIndex > -1 Then
            Dim ss As EraStyleSheet = GetSelectedStyleSheet()
            If ss Is Nothing Then Exit Sub
            ep.SetError(cboOutputType, "")

            'automatically rename the output format
            If txtTarget.Text = "" Then Exit Sub

            'change extension automatically
            Try
                txtTarget.Text = IO.Path.ChangeExtension(txtTarget.Text, ss.Extension)
            Catch ex As Exception
                'don't worry about it
            End Try
        End If
    End Sub

    Private Sub txtShapeName_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtShapeName.Enter
        If txtShapeName.Text = "<enter shape name>" Then txtShapeName.Text = ""
    End Sub

    Private Sub txtShapeName_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtShapeName.KeyPress
        If e.KeyChar = """" Then
            e.Handled = True
        End If
    End Sub

    Private Sub txtShapeName_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtShapeName.Leave
        If txtShapeName.Text = "" Then
            txtShapeName.Text = "<enter shape name>"
        End If
    End Sub

    Private Sub txtTarget_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTarget.Leave, txtReportTitle.Leave
        If txtTarget.Text <> "" Then ep.SetError(btnBrowse, "")
    End Sub

    Private Sub cboReports_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboReports.Leave
        If cboReports.Text <> "" Then
            ep.SetError(btnBrowseReports, "")
        End If
    End Sub

    Private Sub cboReports_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboReports.SelectedIndexChanged
        If cboReports.SelectedIndex > -1 Then
            ep.SetError(btnBrowseReports, "")
            UpdateReportTitle()
        End If
    End Sub

    Private Sub cboUnits_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboUnits.SelectedIndexChanged
        If cboUnits.SelectedIndex >= 0 Then ep.SetError(cboUnits, "")
    End Sub

    Private Sub UpdateReportTitle()
        Try
            Dim fi As IO.FileInfo = Reports(cboReports.Text)
            If fi IsNot Nothing Then
                Dim xmldoc As New XmlDocument
                xmldoc.Load(fi.FullName)
                Dim strTitle As String = xmldoc.DocumentElement.GetAttribute("title")
                If strTitle.Length > 0 Then
                    txtReportTitle.Text = strTitle
                End If
            End If
        Catch ex As Exception
            Logging.Log.Error("Failed to get Report Title from the Report file", ex)
            'This may be an indication that there's going to be a problem reading this file,
            'but just wait till we get to that point before we do anything...
        End Try
    End Sub

    Private Sub chkDisplay_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkDisplay.Click
        My.Settings.DisplayBuffer = Me.chkDisplay.Checked
        My.Settings.Save()
    End Sub
End Class
