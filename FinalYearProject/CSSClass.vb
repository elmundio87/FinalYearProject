
Public Class CSSClass
    Public Name As String 'The name of this class
    Public Rules As ArrayList = New ArrayList() 'The rules associated with it

    Public Function count()
        Return Rules.Count
    End Function

End Class

