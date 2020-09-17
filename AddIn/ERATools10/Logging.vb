Imports System.IO
Imports System.Linq
Imports log4net

<System.Runtime.InteropServices.ComVisible(False)> _
Friend Module Logging
    Private m_log As ILog

    Friend ReadOnly Property Log() As ILog
        Get
            If m_log Is Nothing Then
                log4net.Util.LogLog.InternalDebugging = System.Diagnostics.Debugger.IsAttached
                Dim AssemblyLocation As String = GetType(Logging).Assembly.Location
                Dim ConfigFile As New IO.FileInfo(IO.Path.Combine(IO.Path.GetDirectoryName(AssemblyLocation), "log4net.xml"))
                If ConfigFile.Exists Then log4net.Config.XmlConfigurator.Configure(ConfigFile)
                m_log = log4net.LogManager.GetLogger("ERAtoolsLogger")
            End If
            Return m_log
        End Get
    End Property

    Friend ReadOnly Property LogFilePath() As String
        Get
            Dim rootAppender As log4net.Appender.FileAppender = Log.Logger.Repository.GetAppenders().OfType(Of log4net.Appender.FileAppender).FirstOrDefault()
            If rootAppender Is Nothing Then Return ""
            Return rootAppender.File
        End Get
    End Property
End Module