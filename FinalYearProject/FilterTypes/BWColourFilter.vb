Public Class BWColourFilter
Inherits Filter
    Public Overrides Function filter(ByVal col As System.Drawing.Color)
        Dim clr As Integer
        'greyscale
        clr = (CInt(col.R) + CInt(col.G) + CInt(col.B)) \ 3 'The greyscale colour can be obtained by averaging the RGB values
        Return Color.FromArgb(col.A, clr, clr, clr)
    End Function
Public Overrides Sub flush()
End Sub
End Class