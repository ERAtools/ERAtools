#If MODE = "ArcGIS9" Then

Imports ESRI.ArcGIS.esriSystem
Imports System.Threading

Public Class ESRILicenseChecker

    Private m_pAoInitialize As IAoInitialize
    Private m_sbStatus As New System.Text.StringBuilder
    Private m_ProductCode As esriLicenseProductCode = esriLicenseProductCode.esriLicenseProductCodeArcView
    Private m_LicenseStatus As esriLicenseStatus = esriLicenseStatus.esriLicenseNotInitialized
    Shared LicenseManagerCheckComplete As Boolean = False

#Region "Public Properties to report the license manager status"
    Public ReadOnly Property StatusText() As String
        Get
            Return m_sbStatus.ToString
        End Get
    End Property

    Public ReadOnly Property LastStatusText() As String
        Get
            Select Case m_LicenseStatus
                Case esriLicenseStatus.esriLicenseAlreadyInitialized
                    Return "License Already Initialized"
                Case esriLicenseStatus.esriLicenseAvailable
                    Return "License Available"
                Case esriLicenseStatus.esriLicenseCheckedIn
                    Return "License Checked In"
                Case esriLicenseStatus.esriLicenseCheckedOut
                    Return "License Checked Out"
                Case esriLicenseStatus.esriLicenseFailure
                    Return "License Failure"
                Case esriLicenseStatus.esriLicenseNotInitialized
                    Return "License Not Initialized"
                Case esriLicenseStatus.esriLicenseNotLicensed
                    Return "Not Licensed"
                Case esriLicenseStatus.esriLicenseUnavailable
                    Return "License Unavailable"
                Case Else
                    Return "unknown"
            End Select
        End Get
    End Property

    Public ReadOnly Property StatusCode() As esriLicenseStatus
        Get
            Return m_LicenseStatus
        End Get
    End Property

    Public ReadOnly Property OK() As Boolean
        Get
            Return m_LicenseStatus = esriLicenseStatus.esriLicenseCheckedOut Or m_LicenseStatus = esriLicenseStatus.esriLicenseAlreadyInitialized
        End Get
    End Property

    Public ReadOnly Property ProductText() As String
        Get
            Select Case m_ProductCode
                Case esriLicenseProductCode.esriLicenseProductCodeArcEditor
                    Return "ArcEditor"
                Case esriLicenseProductCode.esriLicenseProductCodeArcInfo
                    Return "ArcInfo"
                Case esriLicenseProductCode.esriLicenseProductCodeArcServer
                    Return "ArcServer"
                Case esriLicenseProductCode.esriLicenseProductCodeArcView
                    Return "ArcView"
                Case esriLicenseProductCode.esriLicenseProductCodeEngine
                    Return "ArcEngine"
                Case esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB
                    Return "ArcEngineGeoDB"
                Case Else
                    Return "unknown"
            End Select
        End Get
    End Property
#End Region

#Region "Initialization Subs"
    
    Sub New()
        Try
            'Create a new AoInitialize object
            m_pAoInitialize = New AoInitialize
            If m_pAoInitialize Is Nothing Then
                m_LicenseStatus = esriLicenseStatus.esriLicenseFailure
                Exit Sub
            End If

            'make sure that we can connect to a license manager
            'we do this in a separate thread, as it hangs on the call to IsProductAvailable
            'if configured for a license manager that isn't available
            LicenseManagerCheckComplete = False

            LicenseCheckThread = New Thread(AddressOf LicenseCheckThreadStart)
            Application.DoEvents()
            LicenseCheckThread.Start()

            Dim TimeStart As Date = Date.Now
            Do While Not LicenseManagerCheckComplete
                If DateDiff(DateInterval.Second, TimeStart, Date.Now) > 20 Then
                    m_sbStatus.AppendLine("Timed out waiting for license manager to respond.")
                    LicenseCheckThread.Abort()
                    m_LicenseStatus = esriLicenseStatus.esriLicenseFailure
                    Exit Sub
                End If
                Application.DoEvents()
            Loop
        Catch ex As Exception
            m_sbStatus.AppendLine("ESRI Software might not be installed correctly. Error returned: " & ex.Message)
            m_LicenseStatus = esriLicenseStatus.esriLicenseFailure
        End Try


    End Sub

    'thread used to initially check status of license manager
    Private LicenseCheckThread As Thread

    Private Sub LicenseCheckThreadStart()
        'system hangs on m_pAoInitialize.IsProductCodeAvailable when license manager isn't available, so 
        'we run it in this thread and check the state in the calling main thread
        If Not CheckAndInitialize(esriLicenseProductCode.esriLicenseProductCodeArcView) Then
            If Not CheckAndInitialize(esriLicenseProductCode.esriLicenseProductCodeArcEditor) Then
                If Not CheckAndInitialize(esriLicenseProductCode.esriLicenseProductCodeArcInfo) Then
                    m_LicenseStatus = esriLicenseStatus.esriLicenseUnavailable
                    Exit Sub
                End If
            End If
        End If
        LicenseManagerCheckComplete = True
    End Sub

    Private Function CheckAndInitialize(ByVal Product As esriLicenseProductCode) As Boolean
        m_ProductCode = Product

        m_LicenseStatus = m_pAoInitialize.IsProductCodeAvailable(m_ProductCode)

        If m_LicenseStatus = esriLicenseStatus.esriLicenseAvailable Then
            m_LicenseStatus = m_pAoInitialize.Initialize(m_ProductCode)
        End If

        m_sbStatus.AppendLine(" Product " & Me.ProductText & ": " & Me.LastStatusText)

        Return m_LicenseStatus = esriLicenseStatus.esriLicenseCheckedOut Or m_LicenseStatus = esriLicenseStatus.esriLicenseAlreadyInitialized

    End Function

#End Region

    Public Sub ShutDown()
        m_pAoInitialize.Shutdown()
        My.Application.ESRI_Status = My.ESRI_Status_Enum.Unknown
    End Sub



End Class

#End If