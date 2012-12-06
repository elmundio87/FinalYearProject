Imports System.Collections.Specialized
Imports System.Text.RegularExpressions

Public Class htmlfile
    Inherits File
    Private colournames As NameValueCollection = New NameValueCollection

    Public Sub New(ByVal b As Byte())
        MyBase.new(b)

        colournames.Add("AliceBlue", "#F0F8FF")
        colournames.Add("AntiqueWhite", "#FAEBD7")
        colournames.Add("Aqua", "#00FFFF")
        colournames.Add("Aquamarine", "#7FFFD4")
        colournames.Add("Azure", "#F0FFFF")
        colournames.Add("Beige", "#F5F5DC")
        colournames.Add("Bisqu", "#FFE4C4")
        colournames.Add("Black", "#000000")
        colournames.Add("BlanchedAlmond", "#FFEBCD")
        colournames.Add("Blue", "#0000FF")
        colournames.Add("BlueViolet", "#8A2BE2")
        colournames.Add("Brown", "#A52A2A")
        colournames.Add("BurlyWood", "#DEB887")
        colournames.Add("CadetBlue", "#5F9EA0")
        colournames.Add("Chartreuse", "#7FFF00")
        colournames.Add("Chocolate", "#D2691E")
        colournames.Add("Coral", "#FF7F50")
        colournames.Add("CornflowerBlue", "#6495ED")
        colournames.Add("Cornsilk", "#FFF8DC")
        colournames.Add("Crimson", "#DC143C")
        colournames.Add("Cyan", "#00FFFF")
        colournames.Add("DarkBlue", "#00008B")
        colournames.Add("DarkCyan", "#008B8B")
        colournames.Add("DarkGoldenRod", "#B8860B")
        colournames.Add("DarkGray", "#A9A9A9")
        colournames.Add("DarkGreen", "#006400")
        colournames.Add("DarkKhaki", "#BDB76B")
        colournames.Add("DarkMagenta", "#8B008B")
        colournames.Add("DarkOliveGreen", "#556B2F")
        colournames.Add("Darkorange", "#FF8C00")
        colournames.Add("DarkOrchid", "#9932CC")
        colournames.Add("DarkRed", "#8B0000")
        colournames.Add("DarkSalmon", "#E9967A")
        colournames.Add("DarkSeaGreen", "#8FBC8F")
        colournames.Add("DarkSlateBlue", "#483D8B")
        colournames.Add("DarkSlateGray", "#2F4F4F")
        colournames.Add("DarkTurquoise", "#00CED1")
        colournames.Add("DarkViolet", "#9400D3")
        colournames.Add("DeepPink", "#FF1493")
        colournames.Add("DeepSkyBlue", "#00BFFF")
        colournames.Add("DimGray", "#696969")
        colournames.Add("DodgerBlue", "#1E90FF")
        colournames.Add("FireBrick", "#B22222")
        colournames.Add("FloralWhite", "#FFFAF0")
        colournames.Add("ForestGreen", "#228B22")
        colournames.Add("Fuchsia", "#FF00FF")
        colournames.Add("Gainsboro", "#DCDCDC")
        colournames.Add("GhostWhite", "#F8F8FF")
        colournames.Add("Gold", "#FFD700")
        colournames.Add("GoldenRod", "#DAA520")
        colournames.Add("Gray", "#808080")
        colournames.Add("Green", "#008000")
        colournames.Add("GreenYellow", "#ADFF2F")
        colournames.Add("HoneyDew", "#F0FFF0")
        colournames.Add("HotPink", "#FF69B4")
        colournames.Add("IndianRed", "#CD5C5C")
        colournames.Add("Indigo", "#4B0082")
        colournames.Add("Ivory", "#FFFFF0")
        colournames.Add("Khaki", "#F0E68C")
        colournames.Add("Lavender", "#E6E6FA")
        colournames.Add("LavenderBlush", "#FFF0F5")
        colournames.Add("LawnGreen", "#7CFC00")
        colournames.Add("LemonChiffon", "#FFFACD")
        colournames.Add("LightBlue", "#ADD8E6")
        colournames.Add("LightCoral", "#F08080")
        colournames.Add("LightCyan", "#E0FFFF")
        colournames.Add("LightGoldenRodYellow", "#FAFAD2")
        colournames.Add("LightGrey", "#D3D3D3")
        colournames.Add("LightGreen", "#90EE90")
        colournames.Add("LightPink", "#FFB6C1")
        colournames.Add("LightSalmon", "#FFA07A")
        colournames.Add("LightSeaGreen", "#20B2AA")
        colournames.Add("LightSkyBlue", "#87CEFA")
        colournames.Add("LightSlateGray", "#778899")
        colournames.Add("LightSteelBlue", "#B0C4DE")
        colournames.Add("LightYellow", "#FFFFE0")
        colournames.Add("Lime", "#00FF00")
        colournames.Add("LimeGreen", "#32CD32")
        colournames.Add("Linen", "#FAF0E6")
        colournames.Add("Magenta", "#FF00FF")
        colournames.Add("Maroon", "#800000")
        colournames.Add("MediumAquaMarine", "#66CDAA")
        colournames.Add("MediumBlue", "#0000CD")
        colournames.Add("MediumOrchid", "#BA55D3")
        colournames.Add("MediumPurple", "#9370D8")
        colournames.Add("MediumSeaGreen", "#3CB371")
        colournames.Add("MediumSlateBlue", "#7B68EE")
        colournames.Add("MediumSpringGreen", "#00FA9A")
        colournames.Add("MediumTurquoise", "#48D1CC")
        colournames.Add("MediumVioletRed", "#C71585")
        colournames.Add("MidnightBlue", "#191970")
        colournames.Add("MintCream", "#F5FFFA")
        colournames.Add("MistyRose", "#FFE4E1")
        colournames.Add("Moccasin", "#FFE4B5")
        colournames.Add("NavajoWhite", "#FFDEAD")
        colournames.Add("Navy", "#000080")
        colournames.Add("OldLace", "#FDF5E6")
        colournames.Add("Olive", "#808000")
        colournames.Add("OliveDrab", "#6B8E23")
        colournames.Add("Orange", "#FFA500")
        colournames.Add("OrangeRed", "#FF4500")
        colournames.Add("Orchid", "#DA70D6")
        colournames.Add("PaleGoldenRod", "#EEE8AA")
        colournames.Add("PaleGreen", "#98FB98")
        colournames.Add("PaleTurquoise", "#AFEEEE")
        colournames.Add("PaleVioletRed", "#D87093")
        colournames.Add("PapayaWhip", "#FFEFD5")
        colournames.Add("PeachPuff", "#FFDAB9")
        colournames.Add("Peru", "#CD853F")
        colournames.Add("Pink", "#FFC0CB")
        colournames.Add("Plum", "#DDA0DD")
        colournames.Add("PowderBlue", "#B0E0E6")
        colournames.Add("Purple", "#800080")
        colournames.Add("Red", "#FF0000")
        colournames.Add("RosyBrown", "#BC8F8F")
        colournames.Add("RoyalBlue", "#4169E1")
        colournames.Add("SaddleBrown", "#8B4513")
        colournames.Add("Salmon", "#FA8072")
        colournames.Add("SandyBrown", "#F4A460")
        colournames.Add("SeaGreen", "#2E8B57")
        colournames.Add("SeaShell", "#FFF5EE")
        colournames.Add("Sienna", "#A0522D")
        colournames.Add("Silver", "#C0C0C0")
        colournames.Add("SkyBlue", "#87CEEB")
        colournames.Add("SlateBlue", "#6A5ACD")
        colournames.Add("SlateGray", "#708090")
        colournames.Add("Snow", "#FFFAFA")
        colournames.Add("SpringGreen", "#00FF7F")
        colournames.Add("SteelBlue", "#4682B4")
        colournames.Add("Tan", "#D2B48C")
        colournames.Add("Teal", "#008080")
        colournames.Add("Thistle", "#D8BFD8")
        colournames.Add("Tomato", "#FF6347")
        colournames.Add("Turquoise", "#40E0D0")
        colournames.Add("Violet", "#EE82EE")
        colournames.Add("Wheat", "#F5DEB3")
        colournames.Add("White", "#FFFFFF")
        colournames.Add("WhiteSmoke", "#F5F5F5")
        colournames.Add("Yellow", "#FFFF00")
        colournames.Add("YellowGreen", "#9ACD32")

    End Sub

    Public Overrides Sub filterdata(ByVal f As settings)

        Dim html As String
        Dim colstring As String
        Dim newcol As Color
        Dim fil As Filter = f.getFilter
        Dim charset As String
        Dim encode As System.Text.Encoding

        html = System.Text.Encoding.UTF8.GetString(data)

        If html.Contains("charset=") Then 'check for the unicode language of the HTML
            charset = html.Remove(0, html.IndexOf("charset=") + Len("charset="))
            charset = charset.Substring(0, charset.IndexOf(Chr(34)))
            Try
                encode = System.Text.Encoding.GetEncoding(charset)
                html = encode.GetString(data)
            Catch
                encode = System.Text.Encoding.UTF8
            End Try
        ElseIf f.getEncodingLanguage = "ru" Then
            encode = System.Text.Encoding.GetEncoding("windows-1251")
            html = encode.GetString(data)
        Else
            encode = System.Text.Encoding.UTF8
        End If

  
        'add in the default link colours
        html = Regex.Replace(html, "<body", "<body link=#0000FF vlink=#FF0000 alink=#800080", RegexOptions.IgnoreCase)

        'replace all instances of colour names with their hex equiv's
        For Each key As String In colournames
            If html.ToLower.Contains(key.ToLower) Then
                html = Regex.Replace(html, Chr(34) & key & Chr(34), Chr(34) & colournames.Item(key) & Chr(34), RegexOptions.IgnoreCase)
            End If
        Next

        Dim x As Integer = 0
        Do Until x = html.Length - 1
            Try 'find all instances of colour values in the html page

                If html(x) = "#" Then
                    Dim temp As String
                    temp = html.Substring(x, 7)
                    If isColour(temp) Then

                        colstring = temp
                        newcol = System.Drawing.Color.FromArgb(System.Drawing.ColorTranslator.FromHtml(colstring).ToArgb)
                        newcol = fil.filter(newcol)
                        colstring = System.Drawing.ColorTranslator.ToHtml(newcol)
                        html = html.Remove(x, temp.Length)
                        html = html.Insert(x, colstring)
                    End If
                End If


                If html.Substring(x, 4).ToLower = "rgb(" Then
                    Dim length As Integer
                    Dim temp, r, g, b As String
                    length = html.IndexOf(")", x) - x + 1

                    colstring = html.Substring(x, length)

                    r = colstring.ToLower.Split(",")(0).Replace("rgb(", "")
                    g = colstring.Split(",")(1).Trim
                    b = colstring.Split(",")(2).Trim.Replace(")", "")


                    newcol = System.Drawing.Color.FromArgb(r, g, b)
                    newcol = fil.filter(newcol)
                    colstring = "rgb(" & newcol.R & ", " & newcol.G & ", " & newcol.B & ")"
                    html = html.Remove(x, length)
                    html = html.Insert(x, colstring)

                End If
            Catch
            End Try
            x += 1
        Loop

        filteredData = encode.GetBytes(html)
        fil.Flush()
    End Sub

    Private Function isColour(ByRef s As String)
        'try and figure out if the supplied string is a colour value
        If Not (Char.IsLetterOrDigit(s(5)) And Char.ToLower(s(5)) <= "f") And Not IsNumeric(s(5)) Then
            Try 'check for a #FFF format colour
                Dim x As Color
                x = System.Drawing.Color.FromArgb(System.Drawing.ColorTranslator.FromHtml(s.Substring(0, 4)).ToArgb)
                s = s.Substring(0, 4)
            Catch ex As Exception
                Return False
            End Try
        Else 'check for a #FFFFFF format colour
            Try
                Dim x As Color
                x = System.Drawing.Color.FromArgb(System.Drawing.ColorTranslator.FromHtml(s).ToArgb)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End If


    End Function
End Class



