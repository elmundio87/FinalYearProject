Imports System.Collections.Specialized
Imports System.Text.RegularExpressions

Public Class CSSFile
    Inherits File
    Private importlist As ArrayList = New ArrayList() ' stores @import statements
    Private classList As ArrayList = New ArrayList() 'stores normal classes
    Private colournames As NameValueCollection = New NameValueCollection 'a list of colour names, and the associated hex value

    Public Sub New(ByVal b() As Byte)
        MyBase.new(b)
        Dim state As Integer = 0
        Dim tempclassname As String = ""
        Dim temprule As String = ""
        Dim tempvalue As String = ""
        Dim tempimport As String = ""
        Dim classGroup As ArrayList = New ArrayList()
        Dim cssString As String

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

        cssString = System.Text.Encoding.ASCII.GetString(b)

        While cssString.Contains("/*")
            cssString = cssString.Remove(cssString.IndexOf("/*"), cssString.IndexOf("*/") - cssString.IndexOf("/*") + Len("/*")) 'REMOVE COMMENTS they interfere with the parsing!
        End While


        'State 0 = Parsing class names
        'State 1 = Parsing rule property
        'State 2 = Parsing rule value
        'State 3 = Parsing an import statement

        For Each ch As Char In cssString

            If state = 0 And ch = "@" Then 'found an import statement
                state = 3
                tempimport = tempimport & ch 'parse the import statement
                GoTo skip
            End If

            If state = 3 And ch = ";" Then
                state = 0 'try starting to read class names
                tempimport = tempimport & ch 'finish parsing the import statement
                importlist.Add(tempimport) 'add the import statement to the list
                tempimport = "" 'clear the temporary variable
            End If

            If ch = "}" And state = 2 Then 'in case someone forgets ";"
                'if was reading a rule value and reaches the end of a class...

                'create a new rule, and add it to every class included in this statement
                Dim tempCSSRule As CSSRule = New CSSRule(temprule, tempvalue)
                For Each csscls As CSSClass In classGroup
                    csscls.Rules.Add(tempCSSRule)
                Next

                'for every class name in this statement, add to the main class list
                For Each Val As CSSClass In classGroup
                    classList.Add(Val)
                Next

                'clear the temporary variables
                classGroup.Clear()
                tempclassname = ""
                temprule = ""
                tempvalue = ""
                state = 0

                GoTo skip
            End If


            If ch = ":" And state = 1 Then 'indicates that a rule name has ended, and a value will begin
                state = 2
                GoTo skip
            End If

            If ch = ";" And state = 2 Then 'indicates that a value has finished
                'create a rule object from the latest parsed rule:value pair
                Dim tempCSSRule As CSSRule = New CSSRule(temprule, tempvalue)
                'add the new rule to all classes included in this statement
                For Each csscls As CSSClass In classGroup
                    csscls.Rules.Add(tempCSSRule)
                Next
                'clear the temporary variables
                temprule = ""
                tempvalue = ""
                state = 1
                GoTo skip
            End If

            If ch = "{" And state = 0 Then 'indicates class name declarations are ending
                Dim css As CSSClass = New CSSClass
                css.Name = tempclassname.Trim
                classGroup.Add(css) 'add the latest class name to the group
                tempclassname = ""
                state = 1 'start parsing a rule name
                GoTo skip
            End If

            If ch = "," And state = 0 Then 'indicates a class name has finished, and another will be declared
                Dim css As CSSClass = New CSSClass
                css.Name = tempclassname.Trim
                classGroup.Add(css) 'add the last class name to the group list
                tempclassname = ""
                GoTo skip
            End If

            If ch = "}" And state = 1 Then
                'indicates that the parser expected a rule name
                'but found the end of the class statement
                For Each Val As CSSClass In classGroup
                    classList.Add(Val)
                Next
                classGroup.Clear()
                tempclassname = ""
                temprule = ""
                tempvalue = ""
                state = 0
                GoTo skip
            End If

            If state = 0 Then 'parse a class name
                tempclassname = tempclassname & ch
            End If

            If state = 1 Then 'parse a rule name
                temprule = temprule & ch
            End If

            If state = 2 Then 'parse a rule value
                tempvalue = tempvalue & ch
            End If

            If state = 3 Then 'parse an import statement
                tempimport = tempimport & ch
            End If

skip:       'so that the same char can't be parsed twice








        Next
    End Sub

    Public Sub changeClass(ByRef className As String, ByVal rule As CSSRule)

        Try
            For x = 0 To classList.Count - 1
                If classList(x).Name = className Then 'find the class to change
                    For y = 0 To classList(x).Rules.Count
                        If classList(x).rules(y).name = rule.name Then 'find the rule to change
                            classList(x).rules(y).value = rule.value 'change the value
                            Exit Sub
                        End If
                    Next
                End If
            Next
        Catch
        End Try
    End Sub

    Public Function CSSClassToByteArray()
        'convert the CSSFile object into a string

        Dim cssString As String = ""

        'write all the import statements
        For Each Str As String In importlist
            cssString += Str
            cssString += vbNewLine
        Next

        'write out each class with its rules
        For Each Csl As CSSClass In classList
            cssString += Csl.Name
            cssString += " {"
            For Each rl As CSSRule In Csl.Rules
                cssString += rl.name
                cssString += ":"
                cssString += rl.value
                cssString += ";"
            Next
            cssString += "} " & vbNewLine
        Next


        Dim oEncoder As New System.Text.ASCIIEncoding()
        Dim bytes As Byte() = oEncoder.GetBytes(cssString)

        Return bytes
    End Function

    Public Overrides Sub FilterData(ByVal f As settings)

        Dim fil As Filter = f.getFilter()

        If f.getAltCSS = False Then 'if using the object oriented approach
            Dim rulestochange As ArrayList = New ArrayList
            Dim newcolstring As String = ""
            Try
                For Each csl As CSSClass In classList
                    For Each csr As CSSRule In csl.Rules


                        If csr.name.ToLower.Contains("color") Or csr.name.ToLower.Contains("background") Then
                            'find rules that contain colour values
                            Dim colString As String = csr.value
                            Dim newCol As Color

                            'try and find the colour value
                            Try
                                colString = colString.Replace("!important", "")
                                colString = colString.Remove(0, colString.IndexOf("#"))
                            Catch ex As Exception

                            End Try

                            Try
                                'run the colour value through the filter
                                newCol = System.Drawing.Color.FromArgb(System.Drawing.ColorTranslator.FromHtml(colString).ToArgb)
                                newCol = fil.filter(newCol)
                                newcolstring = Drawing.ColorTranslator.ToHtml(newCol)
                            Catch
                                newcolstring = colString
                            End Try

                            'add the rule and new value of a list of rules to change
                            Dim temparr(1) As Object
                            temparr(0) = csl.Name
                            temparr(1) = New CSSRule(csr.name, csr.value.Replace(colString, newcolstring))
                            rulestochange.Add(temparr)

                        End If
                    Next
                Next

                'apply the changes
                For x = 0 To rulestochange.Count - 1
                    changeClass(rulestochange(x)(0), rulestochange(x)(1))
                Next

            Catch
            End Try

            'add in information for the default link colour
            Dim col As Color = System.Drawing.ColorTranslator.FromHtml("#333333")

            Dim cls As CSSClass = New CSSClass
            Dim rl As CSSRule = New CSSRule("color", System.Drawing.ColorTranslator.ToHtml(fil.filter(col)))

            cls.Name = "a:link"
            cls.Rules.Add(rl)
            classList.Add(cls)

            filteredData = Me.CSSClassToByteArray
        Else
            'alternate string based parsing (like the HTML version)
            Dim css As String = System.Text.Encoding.UTF8.GetString(data) + "              " 'add some buffer space for my parsing
            Dim colstring As String
            Dim newcol As Color

            'replace colour names with their hex versions
            For Each key As String In colournames
                If css.ToLower.Contains(Chr(34) & key.ToLower & Chr(34)) Then
                    css = Regex.Replace(css, Chr(34) & key & Chr(34), Chr(34) & colournames.Item(key) & Chr(34), RegexOptions.IgnoreCase)
                End If
            Next


            Dim x As Integer = 0
            Do Until x = css.Length - 1 'for all the characters in the CSS file

                Try
                    If css.Substring(x, 1) = "#" Then 'Might indicate a colour?
                        Dim temp As String
                        temp = css.Substring(x, 7)
                        If isColour(temp) Then 'check that the value IS a colour

                            colstring = temp
                            newcol = System.Drawing.Color.FromArgb(System.Drawing.ColorTranslator.FromHtml(colstring).ToArgb)
                            newcol = fil.filter(newcol) 'filter the colour
                            colstring = System.Drawing.ColorTranslator.ToHtml(newcol)
                            css = css.Remove(x, temp.Length)
                            css = css.Insert(x, colstring) 'replace the existing colour value
                        End If
                    End If


                    If css.Substring(x, 4).ToLower = "rgb(" Then 'Might indicate a colour?
                        Dim length As Integer
                        Dim temp, r, g, b As String
                        length = css.IndexOf(")", x) - x + 1 'find the end of the RGB value

                        colstring = css.Substring(x, length) 'cut the RGB value out and put into a variable

                        r = colstring.ToLower.Split(",")(0).Replace("rgb(", "") 'get R
                        g = colstring.Split(",")(1).Trim 'get G
                        b = colstring.Split(",")(2).Trim.Replace(")", "") 'get B


                        newcol = System.Drawing.Color.FromArgb(r, g, b)
                        newcol = fil.filter(newcol) 'filter the colour
                        colstring = "rgb(" & newcol.R & ", " & newcol.G & ", " & newcol.B & ")"
                        css = css.Remove(x, length) 'rewrite the rgb value
                        css = css.Insert(x, colstring)

                    End If
                Catch
                End Try
                x += 1
            Loop



            Dim oencoder As New System.Text.UTF8Encoding()
            filteredData = oencoder.GetBytes(css) 'turn the string back into bytes

            fil.Flush()

        End If
    End Sub

    Private Function isColour(ByRef s As String)
        'try and figure out if the supplied string is a colour value
        If Not ((Char.IsLetterOrDigit(s(4)) And Char.ToLower(s(4)) <= "f") Or IsNumeric(s(4))) Then
            Try 'check for a #FFF format colour
                Dim x As Color
                x = System.Drawing.Color.FromArgb(System.Drawing.ColorTranslator.FromHtml(s.Substring(0, 4)).ToArgb)
                s = s.Substring(0, 4)
                Return True
            Catch ex As Exception
                Return False
            End Try
        Else
            Try 'check for a #FFFFFF format string
                Dim x As Color
                x = System.Drawing.Color.FromArgb(System.Drawing.ColorTranslator.FromHtml(s).ToArgb)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End If


    End Function

End Class


