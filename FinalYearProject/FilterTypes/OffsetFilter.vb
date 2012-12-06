Public Class OffsetFilter
    Inherits Filter
    Private r, g, b As Integer

    Public Sub New(ByVal pR As Integer, ByVal pG As Integer, ByVal pB As Integer)
        r = pR
        g = pG
        b = pB
    End Sub

    Public Overrides Function filter(ByVal col As System.Drawing.Color)
        Dim colourOut As System.Drawing.Color = col
        Dim rcol, gcol, bcol As Integer
        rcol = col.R * ((100 - r) / 100) 'remove a percentage of the colour()
        If rcol < 0 Then
            rcol = 0 'but make sure the value can't be below 0
        End If
        gcol = col.G * ((100 - g) / 100) 'remove a percentage of the colour()
        If gcol < 0 Then
            gcol = 0 'but make sure the value can't be below 0
        End If
        bcol = col.B * ((100 - b) / 100) 'remove a percentage of the colour()
        If bcol < 0 Then
            bcol = 0 'but make sure the value can't be below 0
        End If
        colourOut = Color.FromArgb(col.A, rcol, gcol, bcol)
        Return colourOut
    End Function

    Public Overrides Sub flush()
    End Sub
End Class

