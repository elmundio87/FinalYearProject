Public Class WebRequest
    Private webString As String

    Public Sub New(ByVal s As String)
        webString = s
    End Sub

    Public Function getAttribute(ByVal s As String)
        'Try and parse an attribute value, if it exists
        Try
            Dim tempstring As String
            tempstring = webString.Remove(0, webString.IndexOf(s & ": "))
            tempstring = tempstring.Substring(0, tempstring.IndexOf(Chr(13) & Chr(10)))
            tempstring = tempstring.Remove(0, Len(s & ": "))

            Return tempstring
        Catch
        End Try
        Return webString
    End Function

    Public Function getFile()
        'try and parse the file name to download
        Dim tempstring As String

        tempstring = webString.Remove(0, webString.IndexOf(" "))
        Try
            tempstring = tempstring.Trim()
            tempstring = tempstring.Substring(0, tempstring.IndexOf(" "))
        Catch
        Finally

        End Try
        Return tempstring
    End Function

    Public Function getRequestType()
        'Will return GET, POST etc
        Dim tempstring As String = webString
        tempstring = tempstring.Substring(0, tempstring.IndexOf(" "))
        Return tempstring
    End Function

    Public Function getPostFileName()
        Dim tempstring As String

        tempstring = webString.Remove(0, webString.IndexOf("filename=") + Len("filename= "))
        tempstring = tempstring.Substring(0, tempstring.IndexOf(Chr(34)))
        Return tempstring

    End Function
End Class
