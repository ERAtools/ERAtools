<System.Runtime.InteropServices.ComVisible(False)> _
Public Class RecordComparer
    Implements Collections.Generic.IComparer(Of Xml.XmlElement)

    Private SortFields As Xml.XmlElement

    Public Sub New(ByVal SortFieldsNode As Xml.XmlElement)
        SortFields = SortFieldsNode
    End Sub

    Public Function Compare(ByVal x As System.Xml.XmlElement, ByVal y As System.Xml.XmlElement) As Integer Implements System.Collections.Generic.IComparer(Of System.Xml.XmlElement).Compare
        For Each SortField As Xml.XmlElement In SortFields.ChildNodes
            Dim strField As String = SortField.GetAttribute("field")
            'If strField.Equals("Feature Comparison Result", StringComparison.CurrentCultureIgnoreCase) Then
            '    strField = "ERAsummarizedvalue"
            'End If
            Dim xStr, yStr As String
            xStr = GetValue(x, strField)
            yStr = GetValue(y, strField)
            If xStr.Length > 0 AndAlso yStr.Length > 0 Then
                'Custom quarter format "Q#-YYYY", sort by YYYY portion first then Q portion
                'TODO: look for pattern mm-yyyy, mm/yyyy, mmm qn-yyyy, qn/yyyy, qn yyyy--
                'not supported for now, users will just have to use formats that put the year first and use numeric months
                'If xStr.StartsWith("Q") And yStr.StartsWith("Q") Then

                '    Dim yearX As Integer = xStr.
                'Else
                Dim xDec, yDec As Decimal
                Dim xDate, yDate As DateTime
                If Decimal.TryParse(xStr, xDec) AndAlso Decimal.TryParse(yStr, yDec) Then
                    If xDec > yDec Then
                        Return ConvertDirection(SortField, 1)
                    ElseIf xDec < yDec Then
                        Return ConvertDirection(SortField, -1)
                    End If
                ElseIf DateTime.TryParse(xStr, xDate) AndAlso DateTime.TryParse(yStr, yDate) Then
                    If xDate > yDate Then
                        Return ConvertDirection(SortField, 1)
                    ElseIf xDate < yDate Then
                        Return ConvertDirection(SortField, -1)
                    End If
                Else
                    xDec = String.Compare(xStr, yStr, True)
                    If xDec <> 0 Then
                        Return ConvertDirection(SortField, xDec)
                    End If
                End If
                'End If

            ElseIf xStr.Length > 0 AndAlso yStr.Length = 0 Then
                Return ConvertDirection(SortField, 1)
            ElseIf xStr.Length = 0 AndAlso yStr.Length > 0 Then
                Return ConvertDirection(SortField, -1)
            End If
        Next
        Return 0
    End Function

    Private Function GetValue(ByRef el As Xml.XmlElement, ByVal name As String) As String
        If name.Length < 1 Then
            Return ""
        End If
        Dim vNode As Xml.XmlElement = el.SelectSingleNode("VALUES/" & name)
        If vNode Is Nothing OrElse vNode.InnerText Is Nothing Then
            Return ""
        Else
            Return vNode.InnerText
        End If
    End Function

    Private Function ConvertDirection(ByRef SortField As Xml.XmlElement, ByVal result As Integer) As Integer
        If SortField.GetAttribute("direction").Equals("descending", StringComparison.CurrentCultureIgnoreCase) Then
            Return result * -1
        Else    'Default to ascending
            Return result
        End If
    End Function
End Class
