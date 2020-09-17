Imports ESRI.ArcGIS.Geometry

''' <summary>
''' Abstract class from which all ERAIssue, ERALayerTable, ERATable, ERAField and all of the Analysis Classes inherit.
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public MustInherit Class EraNode

    ''' <summary>
    ''' Sets the properties of an instance of an inherited class of ERANode based on the XML element passed by reference.
    ''' </summary>
    ''' <param name="EraXmlElement">The XML element containing all of the properties to be used when creating an instance of this class.</param>
    Public MustOverride Sub SetXmlNode(ByRef EraXmlElement As Xml.XmlElement)

    ''' <summary>
    ''' Gets the name of the tag.
    ''' </summary>
    ''' <value>The name of the tag.</value>
    Public MustOverride ReadOnly Property TagName() As String

    Protected m_Name As String
    'Protected m_ErrorList As New ErrorList
    'Protected m_WarningList As New ErrorList
    'Protected ChildNodes As New Generic.List(Of EraNode)
    Protected m_XmlElement As Xml.XmlElement

    ''' <summary>
    ''' Gets or sets the name attribute of the node.
    ''' </summary>
    ''' <value>The name of the node.</value>
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(ByVal Value As String)
            m_Name = Value
        End Set
    End Property

    ''' <summary>
    ''' Logs an error encountered during initializing an instance of the inherited node (during SetXmlNode) or "run-time", during the processing of an analysis.
    ''' </summary>
    ''' <param name="ex">The exception encountered.</param>
    ''' <param name="SubOrFunctionName">Name of the sub or function where the error occurred.</param>
    Friend Sub AddError(ByRef ex As Exception, ByVal SubOrFunctionName As String)
        Logging.Log.Error("Error in " & SubOrFunctionName, ex)
        If m_XmlElement IsNot Nothing Then
            Logging.Log.Error("XML for node with error: " & m_XmlElement.OuterXml)
        Else
            Logging.Log.Error("XML for node is not set")
        End If
        'Me.m_ErrorList.Add(ErrorMessage, SubOrFunctionName)
    End Sub

    ''' <summary>
    ''' Logs a warning encountered during initializing an instance of the inherited node (during SetXmlNode) or "run-time", during the processing of an analysis.
    ''' </summary>
    ''' <param name="WarningMessage">The warning message to log.</param>
    ''' <param name="SubOrFunctionName">Name of the sub, function or other identifying information calling this sub.</param>
    Friend Sub AddWarning(ByVal WarningMessage As String, ByVal SubOrFunctionName As String)
        Logging.Log.Warn("Warning in " & SubOrFunctionName & ": " & WarningMessage)
        'Me.m_WarningList.Add(WarningMessage, SubOrFunctionName)
    End Sub

    ''' <summary>
    ''' Gets the value of the XML attribute named by AttributeName, and optionally throws an exception if the attribute is not found within this EraNode's XML element.
    ''' </summary>
    ''' <param name="AttributeName">Name of the XML attribute for which the value is sought.</param>
    ''' <param name="Required">if set to <c>true</c> and the value for the XML attribute is null, an EraException is thrown.</param>
    ''' <returns></returns>
    Protected Function GetAttributeValue(ByVal AttributeName As String, ByVal Required As Boolean) As String
        If Required Then
            If m_XmlElement.GetAttributeNode(AttributeName) Is Nothing Then
                Throw New EraException("The required attribute " & AttributeName & " is missing from a " & m_XmlElement.Name & " XmlNode. Xml: " & m_XmlElement.OuterXml)
            ElseIf m_XmlElement.GetAttributeNode(AttributeName).Value = "" Then
                Throw New EraException("The required attribute " & AttributeName & " is null in a " & m_XmlElement.Name & " XmlNode. Xml: " & m_XmlElement.OuterXml)
            End If
        ElseIf m_XmlElement.GetAttributeNode(AttributeName) Is Nothing Then
            Return ""
        End If
        Return m_XmlElement.GetAttributeNode(AttributeName).Value
    End Function


End Class


''' <summary>
''' ERAtools exception class
''' </summary>
Public Class EraException
    Inherits Exception

    ''' <summary>
    ''' Initializes a new instance of the <see cref="EraException" /> class.
    ''' </summary>
    ''' <param name="Message">The exception message. The message will automatically be pre-pended with "ERAtools error: ".</param>
    Sub New(ByVal Message As String)
        MyBase.New("ERAtools error: " & Message)
    End Sub
End Class


