Imports System.Xml
Imports System.Xml.Xsl

<System.Runtime.InteropServices.ComVisible(False)> _
Public Class EraStyleSheet
    Private m_FullPath As String
    Private m_DisplayName As String
    Private m_Extension As String

    Public ReadOnly Property FullPath() As String
        Get
            Return m_FullPath
        End Get
    End Property
    Public ReadOnly Property DisplayName() As String
        Get
            Return m_DisplayName
        End Get
    End Property
    Public ReadOnly Property Extension() As String
        Get
            Return m_Extension
        End Get
    End Property

    Public ReadOnly Property xslt() As XslCompiledTransform
        Get
            Dim XmlStyleSheet As New Xml.XmlDocument
            XmlStyleSheet.Load(m_FullPath)

            Dim m_xslt As New Xml.Xsl.XslCompiledTransform(System.Diagnostics.Debugger.IsAttached)
            m_xslt.Load(XmlStyleSheet)
            Return m_xslt
        End Get
    End Property

    Public Sub New(ByVal fi As IO.FileInfo)
        If Not fi.Exists Then
            Throw New ArgumentException("Invalid XSLT file info passed to EraStyleSheet.New")
        End If
        m_FullPath = fi.FullName
        m_DisplayName = IO.Path.GetFileNameWithoutExtension(fi.FullName)

        Dim XmlStyleSheet As New Xml.XmlDocument
        XmlStyleSheet.Load(m_FullPath)
        Dim ExtensionComment As Xml.XmlNode = XmlStyleSheet.FirstChild
        If ExtensionComment.NodeType = XmlNodeType.Comment Then
            m_Extension = DirectCast(ExtensionComment, Xml.XmlComment).Value
        Else
            Throw New XmlException("EraTools stylesheet transform documents must have a comment as the first element that defines the extension to give output files (e.g.: <!--doc-->)")
        End If

    End Sub
    'Public Sub New(ByVal Description As String)
    '    If Description Is Nothing Then
    '        Throw New System.ArgumentNullException("Null Description argument passed to StyleSheet.New")
    '    End If
    '    Dim sa As String() = Description.Split("|")
    '    If sa.GetUpperBound(0) <> 2 Then
    '        Throw New System.ArgumentException("Invalid Description argument passed to StyleSheet.New (" & Description & ")")
    '    End If
    '    m_DisplayName = sa(0)
    '    m_FullPath = sa(1)
    '    m_Extension = "." & sa(2)
    'End Sub

End Class


