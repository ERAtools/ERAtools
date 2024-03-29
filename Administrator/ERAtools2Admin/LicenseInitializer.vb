#If MODE = "ArcGIS10" Then
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by ESRI license initialization tool.
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'
' Robby: I removed all code related to checking out Extensions because
' ERAtools2Admin will likely never use any Extensions
'------------------------------------------------------------------------------

Option Explicit On
Imports ESRI.ArcGIS
Imports ESRI.ArcGIS.esriSystem
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Windows.Forms

Partial Friend Class LicenseInitializer

    Private Sub BindingArcGISRuntime(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ResolveBindingEvent
        Try
            ' Modify ArcGIS runtime binding code as needed; for example, 
            ' the list of products and their binding preference order.
            For Each c As ProductCode In New ProductCode() {ProductCode.Desktop, ProductCode.Engine}
                Try
                    If RuntimeManager.Bind(c) Then Exit Sub
                Catch ex As Exception
                    'For some stupid reason, RuntimeManager.Bind(ProductCode.Engine) throws an exception! :P
                End Try
            Next
            ' Modify the code below on how to handle bind failure:
            ' Failed to bind, announce and force exit
            'MsgBox("ArcGIS runtime binding failed. Application will shut down.")
            'End
        Catch ex As Exception
            'Probably just has an earlier version of ArcMap
        End Try
    End Sub

    ''' <summary>
    ''' Raised when ArcGIS runtime binding hasn't been established. 
    ''' </summary>
    Public Event ResolveBindingEvent As EventHandler

    ''' <summary>
    ''' Initialize the application with the specified products and extension license codes.
    ''' </summary>
    ''' <returns>Initialization is successful.</returns>
    ''' <remarks>
    ''' Make sure an active ArcGIS runtime has been bound before license initialization.
    '''</remarks>
    Public Function InitializeApplication(ByVal productCodes As esriLicenseProductCode()) As Boolean
        m_requestedProducts = productCodes.Distinct.ToList
        ' Make sure an active runtime has been bound before calling any ArcObjects code.
        Try
            If RuntimeManager.ActiveRuntime Is Nothing Then
                RaiseEvent ResolveBindingEvent(Me, New EventArgs())
            End If
        Catch ex As Exception
            'Probably just has an earlier version of ArcMap
        End Try
        Return Initialize()
    End Function

    ''' <summary>
    ''' A summary of the status of product and extensions initialization.
    ''' </summary>
    Public Function LicenseMessage() As String
        'If RuntimeManager.ActiveRuntime Is Nothing Then Return MessageNoRuntimeBinding
        If m_AoInit Is Nothing Then Return MessageNoAoInitialize
        Dim prodStatus As String = String.Empty
        Dim licenseInfo As ILicenseInformation = CType(m_AoInit, ILicenseInformation)
        If m_productStatus Is Nothing OrElse m_productStatus.Count = 0 Then
            Return MessageNoLicensesRequested
        ElseIf (m_productStatus.ContainsValue(esriLicenseStatus.esriLicenseAlreadyInitialized) _
            OrElse m_productStatus.ContainsValue(esriLicenseStatus.esriLicenseCheckedOut)) Then
            Return ReportInformation(licenseInfo, m_AoInit.InitializedProduct(), esriLicenseStatus.esriLicenseCheckedOut)
        Else
            'Failed...
            For Each item As KeyValuePair(Of esriLicenseProductCode, esriLicenseStatus) In m_productStatus
                prodStatus += ReportInformation(licenseInfo, item.Key, item.Value) + Environment.NewLine
            Next
        End If
        Return prodStatus.Trim()
    End Function

    ''' <summary>
    ''' Shuts down AoInitialize object and check back in extensions to ensure
    ''' any ESRI libraries that have been used are unloaded in the correct order.
    ''' </summary>
    ''' <remarks>Once Shutdown has been called, you cannot re-initialize the product license
    ''' and should not make any ArcObjects call.</remarks>
    Public Sub ShutdownApplication()
        If m_hasShutDown Then Return

        If m_AoInit IsNot Nothing Then
            m_AoInit.Shutdown()
        End If

        m_requestedProducts.Clear()
        m_productStatus.Clear()
        m_hasShutDown = True
    End Sub

    ''' <summary>
    ''' Get/Set the ordering of product code checking. If true, check from lowest to 
    ''' highest license. True by default.
    ''' </summary>
    Public Property InitializeLowerProductFirst() As Boolean
        Get
            Return m_InitializeLowerProductFirst
        End Get
        Set(ByVal value As Boolean)
            m_InitializeLowerProductFirst = value
        End Set
    End Property

    ''' <summary>
    ''' Retrieves the product code initialized in the ArcObjects application
    ''' </summary>
    Public ReadOnly Property InitializedProduct() As esriLicenseProductCode
        Get
            If m_AoInit IsNot Nothing Then
                Try
                    Return m_AoInit.InitializedProduct()
                Catch
                    Return 0
                End Try
            End If
            Return 0
        End Get
    End Property

    Public ReadOnly Property HasInitializedProduct As Boolean
        Get
            Return m_hasInitializedProduct
        End Get
    End Property

#Region "Helper methods"

    Private Function Initialize() As Boolean
        'If RuntimeManager.ActiveRuntime Is Nothing Then Return False
        Try

            If m_requestedProducts Is Nothing OrElse m_requestedProducts.Count = 0 Then Return False

            m_hasInitializedProduct = False

            If InitializeLowerProductFirst Then
                m_requestedProducts = m_requestedProducts.OrderBy(Function(c) CInt(c)).ToList
            Else
                m_requestedProducts = m_requestedProducts.OrderByDescending(Function(c) CInt(c)).ToList
            End If

            m_AoInit = New AoInitializeClass()
            For Each code As esriLicenseProductCode In m_requestedProducts
                'The following line is actually pointless, but it prevents a warning...
                Dim prod As esriLicenseProductCode = code
                Dim t As Task = Task.Factory.StartNew(
                    Sub()
                        Dim status As esriLicenseStatus = m_AoInit.IsProductCodeAvailable(prod)
                        If (status = esriLicenseStatus.esriLicenseAvailable) Then
                            status = m_AoInit.Initialize(prod)
                            If (status = esriLicenseStatus.esriLicenseAlreadyInitialized OrElse
                                    status = esriLicenseStatus.esriLicenseCheckedOut) Then
                                m_hasInitializedProduct = True
                            End If
                        ElseIf (status = esriLicenseStatus.esriLicenseAlreadyInitialized OrElse
                                status = esriLicenseStatus.esriLicenseCheckedOut) Then
                            m_hasInitializedProduct = True
                        End If
                        m_productStatus.Add(prod, status)
                    End Sub)
                Try
                    t.Wait(20000)
                    If HasInitializedProduct Then Exit For
                Catch ex As Exception
                    'Don't worry about it here, it's probably a timeout. subsequent check might still work
                    LogException(ex, "Exception occurred while trying to check out a license.")
                End Try
            Next
            m_requestedProducts.Clear()
        Catch ex As Exception
            'this is ok-ish means ArcGIS 10.x isn't installed
            'EXCEPTION: Unhandled Exception	Retrieving the COM class factory for component with CLSID {E01BE902-CC85-4B13-A828-02E789E0DDA9} failed due to the following error: 80040154 Class not registered (Exception from HRESULT: 0x80040154 (REGDB_E_CLASSNOTREG)).
            LogException(ex, "Exception occurred while trying to check out a license.")

        End Try

        Return HasInitializedProduct
    End Function

    Private Function ReportInformation(ByVal licInfo As ILicenseInformation, ByVal code As esriLicenseProductCode, ByVal status As esriLicenseStatus) As String
        Dim prodName As String
        Try
            prodName = licInfo.GetLicenseProductName(code)
        Catch
            prodName = code.ToString()
        End Try
        Select Case status
            Case esriLicenseStatus.esriLicenseAlreadyInitialized, esriLicenseStatus.esriLicenseCheckedOut
                Return String.Format(MessageProductAvailable, prodName)
            Case Else
                Return String.Format(MessageProductNotLicensed, prodName)
        End Select
    End Function
#End Region

#Region "Private Contants"
    Private Const MessageNoRuntimeBinding As String = "Invalid ArcGIS runtime binding."
    Private Const MessageNoAoInitialize As String = "ArcObjects initialization failed."
    Private Const MessageNoLicensesRequested As String = "Product: No licenses were requested"
    Private Const MessageProductAvailable As String = "Product: {0}: Available"
    Private Const MessageProductNotLicensed As String = "Product: {0}: Not Licensed"
    Private Const MessageExtensionAvailable As String = " Extension: {0}: Available"
    Private Const MessageExtensionNotLicensed As String = " Extension: {0}: Not Licensed"
    Private Const MessageExtensionFailed As String = " Extension: {0}: Failed"
    Private Const MessageExtensionUnavailable As String = " Extension: {0}: Unavailable"
#End Region

#Region "Private members"
    Private m_AoInit As IAoInitialize = Nothing
    Private m_hasShutDown As Boolean = False
    Private m_hasInitializedProduct As Boolean = False

    Private m_requestedProducts As List(Of esriLicenseProductCode)
    Private m_productStatus As New Dictionary(Of esriLicenseProductCode, esriLicenseStatus)

    Private m_InitializeLowerProductFirst As Boolean = True 'default from low to high  
#End Region

End Class
#End If