Module LoggingModule

    Public Sub LogMessage(ByVal Message As String, Optional ByVal Category As String = "")
        Trace.Write(Now.ToString & vbTab)
        If Category.Length > 0 Then
            Trace.WriteLine(Message, Category)
        Else
            Trace.WriteLine(Message)
        End If
        Trace.Flush()
    End Sub

    Public Sub LogException(ByVal ex As Exception, Optional ByVal AdditionalInfo As String = "")
        Trace.Write(Now.ToString & vbTab)
        If AdditionalInfo.Length > 0 Then
            Trace.WriteLine(AdditionalInfo & vbTab & ex.Message, "EXCEPTION")
        Else
            Trace.WriteLine(ex.Message, "EXCEPTION")
        End If
        Trace.WriteLine(ex.StackTrace)
        Trace.Flush()
    End Sub
End Module
