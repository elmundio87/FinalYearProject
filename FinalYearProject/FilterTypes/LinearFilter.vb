Imports System.Collections.Specialized

Public Class linearFilter
    Inherits Filter
    Private type As String
    Private colourmemory As NameValueCollection = New NameValueCollection
    Private lastcol, lastmancol As Color

    Public Sub New(ByVal pType As String)
        type = pType
    End Sub

    Public Overrides Function filter(ByVal col As System.Drawing.Color)
        Dim colourOut As System.Drawing.Color = col
        Dim R, G, B As Integer 'These represent the separate RGB channels()
        Dim L, M, S As Integer 'These represent the LMS cone reactions()
        'RGB2LMS transformation matrix = |L| |17.8824 43.5161 4.11935||R|
        ' |M| = |3.45565 27.1554 3.86714||G|
        ' |S| |0.0299566 0.184309 1.46709||B|
        'Therefore
        'L = 17.8824(R) + 43.5161(G) + 4.11935(B)
        'M = 3.45565(R) + 27.1554(G) + 3.86714(B)
        'S = 0.0299566(R) + 0.184309(G) + 1.46709(B)
        '
        'LMS2RGB matrix = |R| | 0.080944 -0.130504 0.116721||L|
        'inverse(LMS2RGB) |G|= |-0.0102485 0.0540194 -0.113615||M|
        ' |B| |-0.000365294 -0.00412163 0.693513||S|
        'Therefore
        'R = 0.080944(L) + -0.130504(M) + 0.116721(S)
        'G = -0.0102485(L) + 0.0540194(M) -0.113615(S)
        'B = -0.000365294(L) -0.00412163(M) 0.693513(S)
        '|Lp| |0 2.02344 -2.52581 ||L|
        '|Mp| = |0 1 0 ||M|
        '|Sp| |0 0 1 ||S|
        '
        '|Ld| |1 0 0 ||L|
        '|Md| = |0.494207 0 1.24827 ||M|
        '|Sd| |0 0 1 ||S|
        '
        R = colourOut.R
        G = colourOut.G
        B = colourOut.B
        'RGB2LMS
        L = (17.8824 * (R)) + (43.5161 * (G)) + (4.11935 * (B))
        M = (3.45565 * (R)) + (27.1554 * (G)) + (3.86714 * (B))

        S = (0.0299566 * (R)) + (0.184309 * (G)) + (1.46709 * (B))
        'Depending on the type of colour blindness, change a different LMS value
        Select Case type
            Case "pro"
                L = (0 * (L)) + (2.02344 * (M)) - (2.52581 * (S))
            Case "deu"
                M = (0.494207 * (L)) + (0 * (M)) + (1.24827 * (S))
            Case "tri"
                S = (0.05 * (M))
            Case "rod"
                S = (0.05 * (M))
                M = (0.494207 * (L)) + (0 * (M)) + (1.24827 * (S))
                L = (0 * (L)) + (2.02344 * (M)) - (2.52581 * (S))
                S = (0.05 * (M))
                M = (0.494207 * (L)) + (0 * (M)) + (1.24827 * (S))
                L = (0 * (L)) + (2.02344 * (M)) - (2.52581 * (S))
        End Select
        'LMS2RGB
        R = (0.080944 * (L)) - (0.130504 * (M)) + (0.116721 * (S))
        G = (-0.0102485 * (L)) + (0.0540194 * (M)) - (0.113615 * (S))
        B = (-0.000365294 * (L)) - (0.00412163 * (M)) + (0.693513 * (S))
        'Make sure the RGB values can't be greater than 255 or less than(0)
        If R > 255 Then
            R = 255
        End If
        If G > 255 Then
            G = 255
        End If
        If B > 255 Then
            B = 255
        End If
        If R < 0 Then
            R = 0
        End If
        If G < 0 Then
            G = 0
        End If
        If B < 0 Then
            B = 0
        End If
        'Return the resulting colour
        colourOut = Color.FromArgb(colourOut.A, R, G, B)
        Return colourOut
    End Function

    Public Overrides Sub flush()
    End Sub
End Class