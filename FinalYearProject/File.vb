Public MustInherit Class File
    Public data As Byte()
    Public filteredData As Byte()
    Public MustOverride Sub FilterData(ByVal f As settings)

    Public Sub New(ByVal b() As Byte)
        data = b
    End Sub

    Public Function getData() As Byte()
        Return data
    End Function

    Public Function getFilteredData() As Byte()
        Return filteredData
    End Function
End Class

