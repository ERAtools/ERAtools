Imports ESRI.ArcGIS.esriSystem

Namespace My

    Partial Friend Class MyApplication
        Public myFrmMain As frmMain
        Private m_ESRI_Status As ESRI_Status_Enum = ESRI_Status_Enum.Unknown
#If MODE = "ArcGIS10" Then
        Private m_LicenseInitializer As LicenseInitializer
        Public Property ESRI_Status() As ESRI_Status_Enum
            Get
                Return m_ESRI_Status
            End Get
            Set(ByVal value As ESRI_Status_Enum)
                m_ESRI_Status = value
                myFrmMain.UpdateESRIStatusBar()
            End Set
        End Property

        Public Function ConnectToESRI() As Boolean
            If m_LicenseInitializer Is Nothing OrElse ESRI_Status = ESRI_Status_Enum.Failed Then
                Me.ESRI_Status = ESRI_Status_Enum.Connecting
                m_LicenseInitializer = New ERAtools2Admin.LicenseInitializer
                'esriLicenseProductCode.esriLicenseProductCodeArcView = 40
                'esriLicenseProductCode.esriLicenseProductCodeArcEditor = 50
                'esriLicenseProductCode.esriLicenseProductCodeArcInfo = 60
                m_LicenseInitializer.InitializeApplication(New esriLicenseProductCode() {40, 50, 60})
            End If
            If m_LicenseInitializer.HasInitializedProduct Then
                ESRI_Status = ESRI_Status_Enum.Connected
                Return True
            Else
                LogMessage(m_LicenseInitializer.LicenseMessage, "ESRI LICENSE WARNING")
                ESRI_Status = ESRI_Status_Enum.Failed
                Return False
            End If
        End Function

        Public Sub DisconnectESRI()
            If m_LicenseInitializer Is Nothing Then Return
            m_LicenseInitializer.ShutdownApplication()
            m_LicenseInitializer = Nothing
            ESRI_Status = ESRI_Status_Enum.Unknown
        End Sub

        Public Function GetESRIStatusMessage() As String
            If m_LicenseInitializer Is Nothing Then
                Return "Not Connected"
            End If
            Return m_LicenseInitializer.LicenseMessage
        End Function
#Else
        Public Property ESRI_Status() As ESRI_Status_Enum
            Get
                Return m_ESRI_Status
            End Get
            Set(ByVal value As ESRI_Status_Enum)
                m_ESRI_Status = value
                myFrmMain.UpdateESRIStatusBar()
            End Set
        End Property

        Public Function GetESRIStatusMessage() As String
            Return "Foo"
        End Function
        Public Function ConnectToESRI() As Boolean
            Return True
        End Function
        Public Sub DisconnectESRI()
            ESRI_Status = ESRI_Status_Enum.Unknown
        End Sub

#End If
        Private WaitCount As Integer

        Public Sub StartWait()
            Me.myFrmMain.Cursor = Cursors.WaitCursor
            WaitCount += 1
        End Sub

        Public Sub EndWait()
            WaitCount -= 1
            If WaitCount <= 0 Then
                Me.myFrmMain.Cursor = Cursors.Default
            End If
        End Sub

        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            'ESRI License Initializer generated code.
            'The following is actually handled in frmMain_Load()
            'ConnectToESRI()
        End Sub

        Private Sub MyApplication_Shutdown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shutdown
            'ESRI License Initializer generated code.
            'Do not make any call to ArcObjects after ShutDownApplication()
#If MODE = "ArcGIS10" Then
            DisconnectESRI()
#End If
        End Sub

        Private Sub MyApplication_UnhandledException(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException
            LogException(e.Exception, "Unhandled Exception")
            If MessageBox.Show("An unexpected error has occurred in the application: " & e.Exception.Message & vbNewLine & "This event has been logged. Do you want to view the log?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = DialogResult.Yes Then
                Process.Start(My.Application.Log.DefaultFileLogWriter.FullLogFileName)
            End If

            e.ExitApplication = MessageBox.Show("Do you want to continue using the application? (Click Yes to continue, No to quit)", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = DialogResult.No
            If e.ExitApplication Then
                LogMessage("User has chosen to quit application due to unhandled exception.""Unhandled Exception")
            Else
                LogMessage("User has chosen to continue application after unhandled exception.""Unhandled Exception")
            End If
        End Sub
    End Class

    Enum ESRI_Status_Enum
        Unknown
        Connected
        Failed
        Connecting
    End Enum
End Namespace

