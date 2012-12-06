Public Class ColourMapFilter
    Inherits Filter
    Private colourblindnesstype As String 'this determines what type of colourblindness is being emulated
    Private colourmap As Bitmap
    Public Sub New(ByVal pType As String, ByVal pRender As String)
        colourblindnesstype = pType
        Dim pro As String
        Dim tri As String
        Dim deu As String
        Dim rod As String
        'Set the file paths depending on what colourmap set has been chosen
        If pRender = "VCcolourmap" Then
            pro = "Resources/VCpro.png"
            tri = "Resources/VCtri.png"
            deu = "Resources/VCdeu.png"
            rod = "Resources/VCrod.png"
        End If
        If pRender = "EVIcolourmap" Then
            pro = "Resources/EVIpro.png"
            tri = "Resources/EVItri.png"
            deu = "Resources/EVIdeu.png"
            rod = "Resources/EVIrod.png"
        End If
        'Set the colour pallette file depending on what type of colourblindness has been chosen
        Select Case colourblindnesstype
            Case "pro"
                colourmap = New Bitmap(pro)
            Case "deu"
                colourmap = New Bitmap(deu)
            Case "tri"
                colourmap = New Bitmap(tri)
            Case "rod"
                colourmap = New Bitmap(rod)
        End Select
    End Sub

    Public Overrides Function filter(ByVal col As System.Drawing.Color)
        Dim colourOut As Drawing.Color = col
        'Find the x and y co-ordinates of the colour associated with the original RGB colour pallette
        'In the palette, Red increases along the X axis, and Green increases along the Y axis
        'Every time R goes from 255 to 0 again, Blue increases by 1
        'Every time this happens, it forms a visible square
        'There are 16 "colour squares" per row in the pallette
        Dim x, y As Integer
        x = col.B
        y = 0
        While x >= 16 'Find out which colour square to look in, using the value of B as a guideline
            x = x - 16 'Find the X co-ordinate of the top left corner of the colour square
            y += 1 'Find the Y co-ordinate of the top left corner of the colour square
        End While
        x = x * 256 'Scale the X value to correspont to the pixel scale, not the "colour square" scale
        y = y * 256 'Scale the Y value to correspont to the pixel scale, not the "colour square" scale
        x += col.R 'find out the X co-ordinate within the colour square
        y += col.G 'find out the Y co-ordinate within the colour square
        colourOut = colourmap.GetPixel(x, y)
        Return colourOut
    End Function

    Public Overrides Sub flush()
        'destroy the bitmap associated with this filter, to free memory
        colourmap.Dispose()
    End Sub
End Class