Public Class ValuePair
    Public Text As String
    Public Value As Object

    Public Sub New(ByVal val As Object, ByVal txt As String)
        Value = val
        Text = txt
    End Sub

    Public Overrides Function ToString() As String
        Return Text
    End Function
End Class

Public Class ValueCollection
    Inherits System.Collections.ObjectModel.KeyedCollection(Of String, ValuePair)

    Protected Overrides Function GetKeyForItem(ByVal item As ValuePair) As String
        Return item.Text
    End Function

    Public Sub AddPair(ByVal val As Object, ByVal txt As String)
        Add(New ValuePair(val, txt))
    End Sub

    Public Function FindKeyForValue(ByVal val As Object) As String
        For Each item As ValuePair In Me.Items
            If item.Value.Equals(val) Then
                Return item.Text
            End If
        Next
        Return ""
    End Function
End Class