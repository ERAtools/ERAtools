<System.Runtime.InteropServices.ComVisible(False)> _
Module TextValidation

    Public Function Nz(ByVal Value As Object, Optional ByVal ValueIfNull As Object = "") As String
        Try
            If Value Is Nothing OrElse Value Is DBNull.Value OrElse (TypeOf (Value) Is System.String AndAlso Value = "") Then
                Return ValueIfNull.ToString
            Else
                If Value.GetType().Equals(System.TypeCode.String) Then
                    If Value = "" Then
                        Return ValueIfNull.ToString()
                    Else
                        Return Value
                    End If
                Else
                    Return Value.ToString
                End If
            End If

        Catch ex As Exception
            Logging.Log.Error("Error in TextValidation.Nz", ex)
            Throw
        End Try
    End Function

    'Public Function NzByType(ByRef Value As Object, ByVal ValueOfType As Object) As Object
    '    Dim t As Type
    '    t = ValueOfType.GetType
    '    Try
    '        If Value Is Nothing OrElse Value Is DBNull.Value OrElse Value = "" Then
    '            If t.IsPrimitive Then
    '                If t.ToString = "" Then
    '                    Return ""
    '                End If
    '            Elseif t.IsSubclassOf(

    '            End If
    '        Else
    '            Return CType(Value.GetType, t)
    '        End If

    '    Catch ex As Exception
    '        Logging.Log.Error("Error in TextValidation.Nz", ex)
    '        Throw
    '    End Try
    'End Function

    'Public Function ValidateLong(ByRef KeyAscii As Short) As Short
    '	'if keystroke is a digit or - , validation returns KeyAscii
    '	'for all other values validation returns null (Ascii 0)
    '	If (KeyAscii >= 48 And KeyAscii <= 57) Or KeyAscii = 45 Or KeyAscii = System.Windows.Forms.Keys.Delete Or KeyAscii = System.Windows.Forms.Keys.Back Or KeyAscii = System.Windows.Forms.Keys.Tab Or KeyAscii = System.Windows.Forms.Keys.Return Then
    '		'key ascii is a digit, delete, backspace, tab or return
    '		ValidateLong = KeyAscii
    '	Else
    '		ValidateLong = 0
    '	End If

    'End Function


    Public Function ValidateDouble(ByRef KeyAscii As Short) As Short
        'if keystroke is a digit or - , or a decimal, validation returns KeyAscii
        'for all other values validation returns null (Ascii 0)
        If (KeyAscii >= 48 And KeyAscii <= 57) Or KeyAscii = 45 Or KeyAscii = 46 Or KeyAscii = System.Windows.Forms.Keys.Delete Or KeyAscii = System.Windows.Forms.Keys.Back Or KeyAscii = System.Windows.Forms.Keys.Tab Or KeyAscii = System.Windows.Forms.Keys.Return Then
            'key ascii is a digit, delete, backspace, tab or return
            ValidateDouble = KeyAscii
        Else
            ValidateDouble = 0
        End If

    End Function


    'Public Function ValidateText(ByRef KeyAscii As Short) As Short
    '	'if keystroke is a letter or ' or - or . or space, validation returns KeyAscii
    '	'for all other values validation returns null (Ascii 0)
    '	If (KeyAscii >= 65 And KeyAscii <= 90) Or (KeyAscii >= 97 And KeyAscii <= 122) Or KeyAscii = 39 Or KeyAscii = 45 Or KeyAscii = 46 Or KeyAscii = System.Windows.Forms.Keys.Delete Or KeyAscii = System.Windows.Forms.Keys.Back Or KeyAscii = System.Windows.Forms.Keys.Space Or KeyAscii = System.Windows.Forms.Keys.Tab Or KeyAscii = System.Windows.Forms.Keys.Return Then
    '		'key ascii is a letter, dash, apostrophe, dot, space, delete, backspace, tab or return
    '		ValidateText = KeyAscii
    '	Else
    '		ValidateText = 0
    '	End If

    'End Function

    'Public Function FormatNumeric(ByVal strVal As String, Optional ByVal nDigits As Integer = 2) As String
    '       Dim strDigits As String

    '	strDigits = New String("#", 9) & "." & New String("#", nDigits)

    '	If IsNumeric(strVal) Then
    '		strVal = VB6.Format(strVal, strDigits)
    '		If strVal = "." Then
    '			strVal = "0"
    '		End If
    '	End If
    '	FormatNumeric = strVal
    'End Function
End Module