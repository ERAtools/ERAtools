Imports System.Text.RegularExpressions

''' <summary>
''' This is a simplified version of the AliasNameFormatter from SaMP.
''' </summary>
Public Class AliasNameFormatter

    ''' <summary>
    ''' This simply takes the input string and finds individual words and puts it in ProperCase and removes any underscores.
    ''' </summary>
    ''' <param name="InString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function FormatShortNameAlias(ByVal InString As String) As String
        If InString.Length < 2 Then Return InString.ToUpper

        'if CamelCased, split it with regex  ([A-Z]+|[0-9]+)
        InString = Regex.Replace(InString, "([A-Z]+|[0-9]+)", " $1", RegexOptions.Compiled).TrimStart

        'initcap
        InString = StrConv(InString, VbStrConv.ProperCase)

        'remove underscores (spaces will have already been added between underscores and other characters by the regex)
        Return InString.Replace("_", "")
    End Function
End Class
