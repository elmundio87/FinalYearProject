Public Class NOColourFilter
Inherits Filter
    Public Overrides Function filter(ByVal col As System.Drawing.Color)
        Return col 'simply return the same colour
    End Function
Public Overrides Sub flush()
End Sub
End Class