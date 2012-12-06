Imports System.Net
Imports System.Text
Imports System.Net.Sockets
Imports System.IO
Imports System.Data.OleDb

Class WebProxy
    Dim clientSocket As Socket 'The socket between the browser and the webproxy
    Dim read As [Byte]() = New Byte(4095) {} 'buffer to read the intitial HTTP request
    Dim request As WebRequest 'an object to encapsulate the HTTP request
    Dim file As File 'The file to process
    Dim clientmessage As String 'the HTTP request
    Dim settings As New settings 'controls the behaviour of the webproxy and filters
    Dim ID As String 'the ID of this webproxy, for use by the console
    Dim begin, downloaded, processed, ended As DateTime 'for measuring how long processing takes
    Dim myWebHeaderCollection As WebHeaderCollection '
    Dim fileToDownload As String 'the url being downloaded
    Dim bytearray() As Byte = New Byte() {} 'contains the file information as bytes
    Dim clientmessagebytes() As Byte = New Byte() {}

    Public Sub New(ByVal socket As Sockets.Socket, ByVal counter As Integer)
        Me.clientSocket = socket
        ID = String.Format("[{0:X6}] ", counter)
        settings.setIP(IPAddress.Parse(CType(clientSocket.RemoteEndPoint, IPEndPoint).Address.ToString()).ToString)
        settings.populate()

    End Sub

    Private Function ReadMessage(ByVal ByteArray As Byte(), ByRef s As Socket, ByRef clientmessage As String) As Integer
        Dim bytes As Integer = s.Receive(ByteArray, 4096, 0) 'read the HTTP request

        clientmessage = Encoding.UTF8.GetString(ByteArray) 'turn it into a readable string



        If clientmessage.ToLower.Contains("post ") Then
            Dim stream As System.IO.MemoryStream = New MemoryStream
            Dim BinaryWriter As BinaryWriter = New BinaryWriter(Stream)
            Dim readBinary As New BinaryReader(stream)

            BinaryWriter.Write(ByteArray, 0, bytes)
            Dim headerlength As Integer = clientmessage.IndexOf(vbNewLine & vbNewLine, clientmessage.IndexOf(vbNewLine & vbNewLine) + 1) + 4
            'headerlength = 0
            Try
                s.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1)

                s.ReceiveBufferSize = 4096
                s.ReceiveTimeout = 100
                While s.Receive(ByteArray, 4096, SocketFlags.None) > 0 'read the HTTP request
                    clientmessage += Encoding.UTF8.GetString(ByteArray) 'turn it into a readable string
                    BinaryWriter.Write(ByteArray)
                    'Threading.Thread.Sleep(100) '?????????????????
                    Console.WriteLine(s.Available)
                End While


            Catch

            End Try

            stream.Seek(headerlength, SeekOrigin.Begin)
            clientmessagebytes = readBinary.ReadBytes(stream.Length - 1)

            'Dim oFileStream As System.IO.FileStream

            'oFileStream = New System.IO.FileStream("C:/users/edmund/desktop/lol.jpg", System.IO.FileMode.Create)
            'oFileStream.Write(clientmessagebytes, 0, clientmessagebytes.Length) 'download the file to local disk
            'oFileStream.Close()

            readBinary.Close()
            BinaryWriter.Close()
            stream.Dispose()


        End If




        Return bytes
    End Function

    Public Sub Run()

        Dim bytes As Integer = ReadMessage(read, clientSocket, clientmessage)
        If bytes = 0 Then
            Exit Sub 'nothing downloaded
        End If

        begin = Now 'set a time marker for later time calculations

        Try
            request = New WebRequest(clientmessage)
            'get the URL to download
            fileToDownload = request.getFile()

            ' Console.WriteLine(clientmessage.Substring(0, 4096))

            If clientmessage.ToLower.Contains("get /webui/register.htm") Then
                showCreateUser("")
                clientSocket.Shutdown(SocketShutdown.Both) 'close the connection between the browser and proxy
                clientSocket.Close()
                Exit Sub
            End If

            If clientmessage.ToLower.Contains("post") And clientmessage.ToLower.Contains("filename=") And clientmessage.ToLower.Contains("/webui/") Then
                Dim x As Integer = request.getAttribute("Content-Length")

                bytearray = clientmessagebytes
       
                fileToDownload = "/" & request.getPostFileName()

            End If

            If clientmessage.ToLower.Contains("post /webui/register.htm") Then

                If validateNewUser() = "" Then
                    Dim username = parsePost("username")
                    Dim password = parsePost("password")
                    CreateUser(username, password)

                    Try
                        Dim line1, line2, line3, line4, httpresponse, html As String  'create the HTTP response string
                        html = "<html>" & _
                "<head>" & _
                "<script language='javascript'>" & _
                "location.replace('../webui/')" & _
                "</script>" & _
                "</title>Log in to EVI</title>" & _
                "</head>" & _
                "<body>" & _
                "</body>" & _
                "</html>"
                        line1 = "HTTP/1.0 200 OK"
                        line2 = "Date: " & Now.ToLongDateString
                        line3 = "Content-Type: text/html"
                        line4 = "Content-Length:" & Len(html)


                        httpresponse = line1 & vbCrLf & line2 & vbCrLf & line3 & vbCrLf & line4 & vbCrLf & vbCrLf
                        sendHTML(httpresponse, html)
                    Catch
                    End Try
                Else
                    showCreateUser(validateNewUser())
                End If
                clientSocket.Shutdown(SocketShutdown.Both) 'close the connection between the browser and proxy
                clientSocket.Close()
                Exit Sub
            End If

            If clientmessage.ToLower.Contains("logout=log+out") Then
                tryLogOut()
                clientSocket.Shutdown(SocketShutdown.Both) 'close the connection between the browser and proxy
                clientSocket.Close()
                Exit Sub
            End If

            If clientmessage.ToLower.Contains("username=") And clientmessage.ToLower.Contains("password=") Then
                tryLogin()
                clientSocket.Shutdown(SocketShutdown.Both) 'close the connection between the browser and proxy
                clientSocket.Close()
                Exit Sub
            End If


            If settings.isAuthenticated = False Then
                showLogin(request.getAttribute("User-Agent").tolower.contains("iphone"), "")
                clientSocket.Shutdown(SocketShutdown.Both) 'close the connection between the browser and proxy
                clientSocket.Close()
                Exit Sub
            End If


            If clientmessage.ToLower.Contains("get /webui/") Or (clientmessage.ToLower.Contains("get") And clientmessage.ToLower.Contains(settings.getPort & "/webui/")) Then
                If request.getAttribute("User-Agent").tolower.contains("iphone") Then
                    showWebUIphone(False)
                Else
                    showWebUI(False)
                End If
                clientSocket.Shutdown(SocketShutdown.Both) 'close the connection between the browser and proxy
                clientSocket.Close()
                Exit Sub
            End If

            If clientmessage.ToLower.Contains("post") And clientmessage.ToLower.Contains("eviwebpage=true") And clientmessage.ToLower.Contains("save=save") Then
                updateSettings()
                If request.getAttribute("User-Agent").tolower.contains("iphone") Then
                    showWebUIphone(True)
                Else
                    showWebUI(True)
                End If
                clientSocket.Shutdown(SocketShutdown.Both) 'close the connection between the browser and proxy
                clientSocket.Close()
                Exit Sub
            End If

            If request.getRequestType = "CONNECT" Then
                Exit Sub 'Don't know how to handle this request type
            End If


            Console.WriteLine(ID & "Connecting to host " & request.getAttribute("Host"))
            Console.Write(ID & "Requesting file: " & request.getFile.Substring(fileToDownload.LastIndexOf("/") + 1))

            If GetMediaType(fileToDownload) = "FLASH" And settings.getFlash = True Then
                Console.WriteLine(ID & "Flash file rejected")
                Exit Try
            End If

            If request.getFile.Substring(fileToDownload.LastIndexOf("/") + 1) = "" Then
                Console.WriteLine(" (likely the index file)")
            Else
                Console.WriteLine()
            End If

            If request.getRequestType = "GET" Then
                bytearray = DownloadFileFromServer(fileToDownload) 'download the requested file
            End If

            downloaded = Now 'set a time marker for later time calculations

            FilterFile() 'filter the file

            processed = Now 'set a time marker for later time calculations

            SendFileDownSocket(bytearray) 'send the file to the browser


            ended = Now 'set a time marker for later time calculations



        Catch ex As WebException
            Console.WriteLine(ID & "404 - File not found")
            FileNotFound() 'send a file not found response to the browser
        Catch ex As Exception
            Console.WriteLine(ID & "Error processing file, not changed")
            FileError() 'send a server error response to the browser
        End Try

        settings = Nothing 'dispose of the settings file

        Try
            clientSocket.Shutdown(SocketShutdown.Both) 'close the connection between the browser and proxy
            clientSocket.Close()
        Catch
        Finally
            Console.WriteLine(ID & "Socket closed")
            CalculateTime()
        End Try

    End Sub

    Private Sub FilterFile()
        Select Case GetMediaType(fileToDownload) 'use the file extension to try and find the media type
            Case "Image"
                Console.WriteLine(ID & "File is an image")
                If settings.getImageSetting = True Then
                    Console.WriteLine(ID & "Image processing beginning")
                    file = New ImageFile(bytearray)
                    file.FilterData(settings)
                    bytearray = file.getFilteredData
                    Console.WriteLine(ID & "Image processing completed")
                End If
            Case "HTML"
                Console.WriteLine(ID & "File is HTML")
                If settings.getHTMLSetting = True Then
                    Console.WriteLine(ID & "HTML processing beginning")
                    file = New htmlfile(bytearray)
                    file.FilterData(settings)
                    bytearray = file.getFilteredData
                    Console.WriteLine(ID & "HTML processing completed")
                End If
            Case "CSS"
                Console.WriteLine(ID & "File is CSS")
                If settings.getCSSSetting = True Then
                    file = New CSSFile(bytearray)
                    file.FilterData(settings)
                    bytearray = file.getFilteredData()
                    Console.WriteLine(ID & "CSS processing completed")
                End If
        End Select
    End Sub

    Private Sub CalculateTime() 'Debug sub, returns how long it took to process a file
        Dim timetodownload, timetoprocess, totaltime As Long
        timetoprocess = processed.Ticks - downloaded.Ticks
        timetodownload = downloaded.Ticks - begin.Ticks
        totaltime = ended.Ticks - begin.Ticks
        Console.WriteLine(ID & "took " & timetodownload / 10000000 & " seconds to download. ")
        Console.WriteLine(ID & "took " & timetoprocess / 10000000 & " seconds to process.")
        Console.WriteLine(ID & "took " & totaltime / 10000000 & " seconds to complete the operation.")
    End Sub

    Private Sub SendFileDownSocket(ByRef file As Byte()) 'send a file to the browser, with the correct HTTP response headers
        Console.WriteLine(ID & "Sending file to browser")
        Dim line1, line2, line3, line4, httpresponse As String
        line1 = "HTTP/1.0 200 OK"
        line2 = "Date: " & Now.ToLongDateString
        Try
            line3 = "Content-Type:" & myWebHeaderCollection.Item("Content-Type") 'if the file was recently downloaded
        Catch 'if the file was loaded from cache (IE. not web response to copy)
            Select Case GetMediaType(fileToDownload)
                Case "Image"
                    line3 = "Content-Type: image/png"
                Case "HTML"
                    line3 = "Content-Type: text/html"
                Case "CSS"
                    line3 = "Content-Type: text/css"
            End Select
        End Try
        line4 = "Content-Length:" & file.Length

        httpresponse = line1 & vbCrLf & line2 & vbCrLf & line3 & vbCrLf & line4 & vbCrLf & vbCrLf
        clientSocket.Send(System.Text.UTF8Encoding.UTF8.GetBytes(httpresponse), System.Text.UTF8Encoding.UTF8.GetBytes(httpresponse).Length, 0)
        clientSocket.Send(file, file.Length, 0)
    End Sub

    Private Sub FileNotFound() 'if the requested resource is not found, return a 404 error page
        Dim imglist As ArrayList = New ArrayList()
        Dim ra As Random = New Random 'choose a random 404 pic
        imglist.Add("http://www.ihasafunny.com/wp-content/uploads/2009/11/lolcat-4.10.08.0.0.0x0.500x375.jpeg")
        imglist.Add("http://cdn0.knowyourmeme.com/i/28257/original/error0kn.png")
        imglist.Add("http://icanhascheezburger.wordpress.com/files/2009/03/funny-pictures-cat-searches-for-a-file.jpg")

        Randomize()


        Try
            Dim line1, line2, line3, line4, httpresponse As String 'create the HTTP response string
            Dim html As String = "<html><head></head><body><H1>Error 404 - File Not Found</H1>Either the resource does not exist, or access was denied.<br/><br/> <div><img src='" & imglist(ra.Next(0, imglist.Count)) & "'></div></body></html>"
            line1 = "HTTP/1.0 404 Not Found"
            line2 = "Date: " & Now.ToLongDateString
            line3 = "Content-Type: text/html"
            line4 = "Content-Length:" & Len(html)

            httpresponse = line1 & vbCrLf & line2 & vbCrLf & line3 & vbCrLf & line4 & vbCrLf & vbCrLf
            sendHTML(httpresponse, html)
        Catch
        End Try
    End Sub

    Private Sub FileError()
        Try
            Dim line1, line2, line3, line4, httpresponse As String  'create the HTTP response string
            Dim html As String = "<html><head></head><body><H1>Error 500 - Internal Server Error</H1>Due to a an irregularity in this resource, it could not be manipulated.</body></html>"
            line1 = "HTTP/1.0 500 Internal Server Error"
            line2 = "Date: " & Now.ToLongDateString
            line3 = "Content-Type: text/html"
            line4 = "Content-Length:" & Len(html)

            httpresponse = line1 & vbCrLf & line2 & vbCrLf & line3 & vbCrLf & line4 & vbCrLf & vbCrLf
            sendHTML(httpresponse, html)
        Catch
        End Try
    End Sub

    Private Sub forbiddenError()
        Try
            Dim line1, line2, line3, line4, httpresponse As String  'create the HTTP response string
            Dim html As String = "<html><head></head><body><H1>Error 403 - Forbidden</H1>You are not on the list of approved IP addresses.</body></html>"
            line1 = "HTTP/1.0 403 Forbidden"
            line2 = "Date: " & Now.ToLongDateString
            line3 = "Content-Type: text/html"
            line4 = "Content-Length:" & Len(html)

            httpresponse = line1 & vbCrLf & line2 & vbCrLf & line3 & vbCrLf & line4 & vbCrLf & vbCrLf
            sendHTML(httpresponse, html)
        Catch
        End Try
    End Sub



    Private Function DownloadFileFromServer(ByVal url As String) 'download a URL and pass back to the WebProxy as bytes

        Dim filebytes() As Byte = New Byte() {}
        Dim wc As New System.Net.WebClient
        Dim data As String

        Dim folder As String = System.AppDomain.CurrentDomain.BaseDirectory & "cache\" & request.getAttribute("Host") 'the folder to check in the cache
        Dim file As String = folder & "\cached" & url.Remove(0, url.LastIndexOf("/") + 1) 'the filename of the file to check for
        wc.Proxy = GlobalProxySelection.GetEmptyWebProxy 'ignore local IE proxy settings, otherwise will get into an infinite loop if IE6/Chrome/Safari uses localhost as the proxy

        Try
            wc.Headers.Add("user-agent", request.getAttribute("User-Agent")) 'make sure the webclient downloads the file intended for the client browser
        Catch
            wc.Headers.Add("user-agent", "Mozilla/5.0(Windows; U; Windows NT 5.2; rv:1.9.2) Gecko/20100101 Firefox/3.6") 'JUST in case user agent isn't read properly
        End Try


        'if the file is an image, caching is on and the file has already been downloaded
        If settings.getCachingMode = "learnandrecall" And System.IO.File.Exists(file) And GetMediaType(fileToDownload) = "Image" Then

            Dim oFile As System.IO.FileInfo
            oFile = New System.IO.FileInfo(file)

            Dim oFileStream As System.IO.FileStream = oFile.OpenRead()
            Dim lBytes As Long = oFileStream.Length

            If (lBytes > 0) Then
                Dim fileData(lBytes - 1) As Byte
                ReDim filebytes(lBytes)
                ' Read the file into a byte array
                oFileStream.Read(filebytes, 0, lBytes)
                oFileStream.Close()
            End If
            Console.WriteLine(ID & "Retrieved file from cache")
        Else
            'file not found in cache, or cache not enabled
            If request.getRequestType = "POST" Then
                data = clientmessage.Remove(0, clientmessage.IndexOf(vbCrLf & vbCrLf)).Trim
                filebytes = wc.UploadData(url.Substring(0, url.IndexOf("/", 8)), Encoding.UTF8.GetBytes(data)) 'upload data that the browser has sent
            Else
                filebytes = wc.DownloadData(url) 'else, just download a file
            End If

            myWebHeaderCollection = wc.ResponseHeaders 'get a collection of the HTTP response headers for later use
            settings.setEncodingLanguage(myWebHeaderCollection.Item("Content-Language")) 'set the unicode language of the response in the settings
            Console.WriteLine(ID & "Downloaded file from " & request.getAttribute("Host"))
        End If

        'if caching turned on, the file doesn't exist and the file is an image, download it into the cache
        If (settings.getCachingMode = "learningonly" Or settings.getCachingMode = "learnandrecall") And Not System.IO.File.Exists(file) And GetMediaType(fileToDownload) = "Image" Then
            Dim oFileStream As System.IO.FileStream

            If Not Directory.Exists(folder) Then
                Directory.CreateDirectory(folder) 'create the directory if it doesn't exist
            End If

            Try
                oFileStream = New System.IO.FileStream(folder & "\cached" & url.Remove(0, url.LastIndexOf("/") + 1), System.IO.FileMode.Create)
                oFileStream.Write(filebytes, 0, filebytes.Length) 'download the file to local disk
                oFileStream.Close()
            Catch
            End Try
        End If


        Return filebytes

    End Function

    Private Function GetMediaType(ByVal filename As String)

        Try 'use file extensions to get the file type
            Select Case GetFileType(filename.ToLower)
                Case ".jpeg", ".jpg", ".gif", ".tif", ".png", ".bmp"
                    Return "Image"
                Case ".html", ".htm"
                    Return "HTML"
                Case ".css"
                    Return "CSS"
            End Select

            If filename.Chars(filename.Length - 1) = "/" Then
                Return "HTML"
            End If

            If filename.Contains(".php") Then
                Return "HTML"
            End If

            If filename.Contains(".swf") Then
                Return "FLASH"
            End If
        Catch
        End Try
        Return "HTML"
    End Function

    Private Function GetFileType(ByVal fileName) 'get the file extension
        Dim s As String = fileName.Substring(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf("."))
        Try
            s = s.Substring(0, s.IndexOf("?"))
            'in case of a weird file extension
            'EG. http://newsimg.bbc.co.uk/nol/shared/img/v4/banner.jpg?v1
        Catch ex As Exception
        End Try

        Return s
    End Function

    Private Sub showWebUIphone(ByVal showupdated As Boolean)
        Dim html As String
        Dim render1 As String = ""
        Dim render2 As String = ""
        Dim render3 As String = ""
        Dim render4 As String = ""
        Dim render5 As String = ""
        Dim effect1 As String = ""
        Dim effect2 As String = ""
        Dim effect3 As String = ""
        Dim effect4 As String = ""
        Dim updated As String = ""

        If showupdated = True Then
            updated = "Settings updated </BR> (" & Now & ")"
        End If

        Select Case (settings.getRenderType)
            Case "none"
                render1 = "selected='selected'"
            Case "offset"
                render2 = "selected='selected'"
            Case "VCcolourmap"
                render3 = "selected='selected'"
            Case "EVIcolourmap"
                render4 = "selected='selected'"
            Case "algorithm"
                render5 = "selected='selected'"
        End Select

        Select Case (settings.getFilterType)
            Case "pro"
                effect1 = "selected='selected'"
            Case "deu"
                effect2 = "selected='selected'"
            Case "tri"
                effect3 = "selected='selected'"
            Case "rod"
                effect4 = "selected='selected'"
            Case "custom"
                effect1 = "selected='selected'"
        End Select


        html = "<head>" & _
                "<title>iPhone interface</title>" & _
                "<style>body {  font-family: Helvetica, Arial, sans-serif;  margin: 0;  padding: 0;}.edgeToEdge {  background-color: #fff;  border-spacing: 0;  border-width: 0;  font-size: 20px;  font-weight: bold;  margin: 0;  padding: 0;}.edgeToEdge.formButtons {  margin: 0;  padding: 10px;}.edgeToEdge .formList {  font-weight: normal;}.edgeToEdge.formFields textarea,.roundedRect.formFields textarea,.roundedRect .formFields textarea {  clear: left;  display: block;  width: 296px;}dl.edgeToEdge.formFields dd,dl.roundedRect.formFields dd,.roundedRect .formFields dd {  border-bottom: 1px solid rgb(217,217,217);  padding: 10px;  margin: 0;  text-align: right;}dl.edgeToEdge.formFields dd.last,dl.roundedRect.formFields dd.last,.roundedRect .formFields dd.last {  border-bottom-width: 0;}dl.edgeToEdge.formFields dt,dl.roundedRect.formFields dt,.roundedRect .formFields dt {  clear: left;  float: left;  padding: 10px 10px 0 10px;  margin: 0;}p.edgeToEdgeLast {  border-bottom-width: 0;}form {  margin: 0;}.formButtons {  text-align: right;}.formList {  list-style-type: none;  margin: 0 0 -.33em 0;  padding: 0;}.formList li {  margin-bottom: .33em;}.formResults {  background-color: #fff;  -webkit-border-radius: 8px;  color: #900;  font: normal normal bold 17px/normal Helvetica, Arial, sans-serif; margin: 1em 10px;  padding: 10px;  text-align: center;}.iphone-preview-landscape,.iphone-preview-portrait {  background-color: #A3A3A3;  border: 1px solid #000;  margin: 1em auto;  min-height: 320px;  width: 356px;}.iphone-preview-portrait {  min-height: 356px;  width: 320px;}.roundedRect {  background-color: #fff;  border-width: 0;  font-family: Helvetica, Arial, sans-serif;  font-size: 17px;  font-weight: bold;margin: 10px;  -webkit-border-radius: 8px 8px;" & _
                "}body.roundedRect,body.roundedRect .iphone-preview-landscape {  background-color: #C5CCD3;}body.roundedRect {  font: normal normal bold 17px/normal Helvetica, Arial, sans-serif;}div.roundedRect {  margin: .6em 10px 1em 10px;  padding: 1px 10px;}p.roundedRect {  padding: 10px;}.roundedRectHead,.roundedRect legend {  color: rgb(76,86,108);  font: normal normal bold 17px/normal Helvetica, Arial, sans-serif;  margin: 0;}.roundedRectIntHead {  font: normal normal normal 21px/normal Helvetica, Arial, sans-serif;  margin: 1em 0 -.75em 0;}fieldset.roundedRect {  margin: 2em 10px 1em 10px;  padding: 0 0 1px 0;}fieldset.roundedRect p {  margin: 1em 10px;}.roundedRect legend {  padding-left: 10px;  position: relative;  top: -.8em;}.roundedRectHead {  margin: 1em 20px -.5em 20px;}" & _
                ".logout {background: transparent;border-top: 0;border-right: 0;border-bottom: 1px solid #00F;border-left: 0;color: #00F;display: inline;margin: 0;padding: 0;}</style>" & _
                "<meta name='viewport' content='width=150, initial-scale=1.5, user-scalable=no'/>" & _
                "</head>" & _
                "<body>" & _
                "<FORM ACTION='' method='post'>" & _
                "<div>" & _
                "You are logged in as '" & settings.getUser & "' <input type='submit' class='logout' name='logout' value='Log Out' align='right'> " & _
                "</br>" & _
                "<b>Render Type</b></br>" & _
                "<select name='render'> " & _
                "<option value='none'" & render1 & ">None</option>" & _
                "<option value='offset'" & render2 & ">Colour Offset</option>" & _
                "<option value='VCcolourmap'" & render3 & ">Vischeck Colourmap</option>" & _
                "<option value='EVIcolourmap'" & render4 & ">EVI Colourmap</option>" & _
                "<option value='algorithm'" & render5 & ">Linear Algorithm</option>" & _
                "</select>" & _
                "</div>" & _
                "<div>" & _
                "<b>Filter Type</b></br>" & _
                "<select name='effect'>" & _
                "<option value='pro'" & effect1 & ">Protoanopia</option>" & _
                "<option value='deu'" & effect2 & ">Deuteranopia</option>" & _
                "<option value='tri'" & effect3 & ">Tritanopia</option>" & _
                "<option value='rod'" & effect4 & ">Rod Monochromacy</option>" & _
                "</select>" & _
                "</div>" & _
                "<input type = 'hidden' value='true' name='eviwebpage'>" & _
                "<input type='submit' name='save' value='Save'>" & _
                "</FORM>  </td>" & _
                "<br/>" & updated & _
                "</FORM>" & _
                "</body>" & _
                "</html>"

        Dim line1, line2, line3, line4, httpresponse As String  'create the HTTP response string

        line1 = "HTTP/1.0 200 OK"
        line2 = "Date: " & Now.ToLongDateString
        line3 = "Content-Type: text/html"
        line4 = "Content-Length:" & Len(html)

        httpresponse = line1 & vbCrLf & line2 & vbCrLf & line3 & vbCrLf & line4 & vbCrLf & vbCrLf
        sendHTML(httpresponse, html)
    End Sub

    Private Sub showWebUI(ByVal showupdated As Boolean)
        Dim html As String
        Dim render1 As String = ""
        Dim render2 As String = ""
        Dim render3 As String = ""
        Dim render4 As String = ""
        Dim render5 As String = ""
        Dim effect1 As String = ""
        Dim effect2 As String = ""
        Dim effect3 As String = ""
        Dim effect4 As String = ""
        Dim updated As String = ""

        If showupdated = True Then
            updated = "Settings updated </BR> (" & Now & ")"
        End If

        Select Case (settings.getRenderType)
            Case "none"
                render1 = "checked"
            Case "offset"
                render2 = "checked"
            Case "VCcolourmap"
                render3 = "checked"
            Case "EVIcolourmap"
                render4 = "checked"
            Case "algorithm"
                render5 = "checked"
        End Select

        Select Case (settings.getFilterType)
            Case "pro"
                effect1 = "checked"
            Case "deu"
                effect2 = "checked"
            Case "tri"
                effect3 = "checked"
            Case "rod"
                effect4 = "checked"
            Case "custom"
                effect1 = "checked"
        End Select

        html = "<html>" & _
            "<head>" & _
            "<title>EVI Web interface</title>" & _
            "<style type='text/css'> * {font-family : verdana;font-size : 8pt;} .logout {background: transparent;border-top: 0;border-right: 0;border-bottom: 1px solid #00F;border-left: 0;color: #00F;display: inline;margin: 0;padding: 0;}</style>" & _
            "</head>" & _
            "<body>" & _
            "<FORM ACTION='' method='post'>" & _
            "<div>" & _
            "You are logged in as '" & settings.getUser & "' <input class='logout' type='submit' name='logout' value='Log Out'> " & _
            "</br>" & _
            "<table width='600'>" & _
            "<tr><td><b>Render Type</b></td><td><b>Visual Impairment</b></td><td><b>Image Settings</b></td><td><b>HTML/CSS Settings</b></td></tr>" & _
            "<tr><td><input type='radio' name='render' value='none' " & render1 & " > None<br>" & _
            "<input type='radio' name='render' value='offset'" & render2 & "> Colour Offset<br>" & _
            "<input type='radio' name='render' value='VCcolourmap' " & render3 & "> Vischeck Colour Mapping<br>" & _
            "<input type='radio' name='render' value='EVIcolourmap'" & render4 & "> EVI Colour Mapping<br>" & _
            "<input type='radio' name='render' value='algorithm'" & render5 & "> Linear Algorithm</td><td>" & _
            "<input type='radio' name='effect' value='pro'" & effect1 & "> Protoanopia<br>" & _
            "<input type='radio' name='effect' value='deu'" & effect2 & "> Deuteranopia<br>" & _
            "<input type='radio' name='effect' value='tri'" & effect3 & "> Tritanopia<br>" & _
            "<input type='radio' name='effect' value='rod'" & effect4 & "> Rod Monochromacy<br>" & _
            "<br></td></tr>" & _
            "<tr><td></td><td></td></tr>" & _
            "<tr><td></td><td></td></tr>" & _
            "<tr><td>" & _
            "<input type = 'hidden' value='true' name='eviwebpage'>" & _
            "<input type='submit' name='save' value='Save'>" & _
            updated & _
            "</td>" & _
            "</FORM></div>" & _
            "<FORM ENCTYPE='multipart/form-data' ACTION='' METHOD='POST'>" & _
            "Local file upload<br>" & _
            "<input type='file' name='datafile' size='40'>" & _
            "<input type='submit'value='Send'>" & _
            "</body>" & _
            "</html>"


        Try
            Dim line1, line2, line3, line4, httpresponse As String  'create the HTTP response string

            line1 = "HTTP/1.0 200 OK"
            line2 = "Date: " & Now.ToLongDateString
            line3 = "Content-Type: text/html"
            line4 = "Content-Length:" & Len(html)


            httpresponse = line1 & vbCrLf & line2 & vbCrLf & line3 & vbCrLf & line4 & vbCrLf & vbCrLf
            sendHTML(httpresponse, html)
        Catch
        End Try

    End Sub

    Private Sub sendHTML(ByVal httpresponse As String, ByVal html As String)
        Try
            clientSocket.Send(System.Text.UTF8Encoding.UTF8.GetBytes(httpresponse), System.Text.UTF8Encoding.UTF8.GetBytes(httpresponse).Length, 0) 'send the HTTP headers
            clientSocket.Send(System.Text.UTF8Encoding.UTF8.GetBytes(html), System.Text.UTF8Encoding.UTF8.GetBytes(html).Length, 0) 'send HTML
        Catch
        End Try
    End Sub

    Private Sub updateSettings()

        '#If DEBUG Then
        'Const dblocation As String = "C:\Users\Edmund\Documents\Visual Studio 2008\Projects\FinalYearProject\FinalYearProject\bin\Debug\"
        '#Else
        Const dblocation As String = ""
        '#End If


        Const strConnection As String = _
    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & dblocation & "settings.accdb;Persist Security Info=False"


        ' Constructor of the OleDbConnection object 
        Dim objConnection As New OleDbConnection(strConnection)

        objConnection.Open()
        'for all values on the UI, create an UPDATE statement and commit
        Dim strSQL As String = "UPDATE UserSettings SET FilterType = '" & parsePost("effect") & "" & "',RenderType = '" & parsePost("render") & "" & "' WHERE username = '" & settings.getUser & "';"
        Dim objcommand As New OleDbCommand(strSQL, objConnection)

        Try
            objcommand.ExecuteNonQuery()
        Catch
        Finally
            objcommand.Dispose()
            objConnection.Close()
        End Try

        settings.setFilterType(parsePost("effect"))
        settings.setRenderType(parsePost("render"))

    End Sub

    Private Sub showLogin(ByVal isIPhone As Boolean, ByVal message As String)
        Dim html As String
        Dim iphone As String = "<meta name='viewport' content='width=150, initial-scale=1.5, user-scalable=no'/>"


        If isIPhone = False Then
            iphone = ""
        End If



        html = "<html>" & _
               "<head>" & _
               "<title>Log in to EVI</title>" & _
               "<style type='text/css'> .col1 {text-align: right; width: '100px';} .col2 {text-align: left; width: '180px';} .button {text-align: right; width: '200px';}</style>" & _
               iphone & _
               "</head>" & _
               "<body>" & _
               "<H1>Emulating Visual Impairment Login Page</H1>" & _
               "<FORM ACTION='' METHOD='post'>" & _
               "<TABLE>" & _
               "<input type = 'hidden' value='true' name='eviwebpage'>" & _
               "<input type='hidden' value='" & request.getAttribute("Host") & "' NAME='redirect'>" & _
                "<tr>" & _
                "<td class='col1'>" & _
                "Username:</td>" & _
               "<td class='col2'><input type='text' name='username'></input></td>" & _
               "</tr><tr><td class='col1'>" & _
               "Password: </td>" & _
               "<td class='col2'><input type='password' name='password'></input></td>" & _
               "</tr>" & _
               "<tr>" & _
               "<td>" & _
               "<a href='register.htm'>Register</a>" & _
                "</td>" & _
            "<td class='col1'>" & _
                        "<input type='submit' name='login' value='Log In'></input>" & _
        "</td>" & _
            "</TABLE>" & _
               "</FORM>" & _
                "<div>" & message & "</div>" & _
               "</body>" & _
               "</html>"


        Try
            Dim line1, line2, line3, line4, httpresponse As String  'create the HTTP response string

            line1 = "HTTP/1.0 200 OK"
            line2 = "Date: " & Now.ToLongDateString
            line3 = "Content-Type: text/html"
            line4 = "Content-Length:" & Len(html)


            httpresponse = line1 & vbCrLf & line2 & vbCrLf & line3 & vbCrLf & line4 & vbCrLf & vbCrLf
            sendHTML(httpresponse, html)
        Catch
        End Try

    End Sub

    Private Sub showCreateUser(ByVal err As String)
        Dim html As String

        html = "<html>" & _
               "<head>" & _
               "<title>Create new user</title>" & _
               "<style type='text/css'> .col1 {text-align: right; width: '100px';} .col2 {text-align: left; width: '180px';} .button {text-align: right; width: '200px';}</style>" & _
               "</head>" & _
               "<body>" & _
               "<H1>Create new user</H1>" & _
               "<FORM ACTION='' METHOD='post'>" & _
               "<table width='280'><tr>" & _
               "<input type = 'hidden' value='true' name='eviwebpage'>" & _
               "<td class='col1'>Username:</td>" & _
               "<td class='col2'><input type='text' name='username'></input></td>" & _
               "</tr>" & _
               "<tr>" & _
               "<td class='col1'>" & _
               "Password:</td>" & _
               "<td class='col2'><input type='password' name='password'></input></td>" & _
               "</tr>" & _
               "<tr>" & _
               "<td class='col1'>Repeat Password:</td>" & _
               "<td class='col2'><input type='password' name='password2'></input></td>" & _
                "</tr>" & _
                "<tr><td>" & _
                "<a href='../webui/'>Back</a>" & _
                "</td>" & _
                "<td class='button'>" & _
               "<input type='submit'  name='register' value='Register'></input></body>" & _
               "</td>" & _
               "</tr>" & _
               "</table>" & _
               "</FORM>" & _
               "<div></br>" & err & "</div>" & _
               "</body>" & _
               "</html>"

        Try
            Dim line1, line2, line3, line4, httpresponse As String  'create the HTTP response string

            line1 = "HTTP/1.0 200 OK"
            line2 = "Date: " & Now.ToLongDateString
            line3 = "Content-Type: text/html"
            line4 = "Content-Length:" & Len(html)


            httpresponse = line1 & vbCrLf & line2 & vbCrLf & line3 & vbCrLf & line4 & vbCrLf & vbCrLf
            sendHTML(httpresponse, html)
        Catch
        End Try

    End Sub

    Private Function parsePost(ByVal attribute As String)
        Dim tempstring As String

        tempstring = clientmessage.Remove(0, clientmessage.LastIndexOf(attribute & "=") + Len(attribute & "="))
        Return tempstring.Substring(0, tempstring.IndexOf("&"))
    End Function

    Private Sub tryLogin()
        '#If DEBUG Then
        '       Const dblocation As String = "C:\Users\Edmund\Documents\Visual Studio 2008\Projects\FinalYearProject\FinalYearProject\bin\Debug\"
        '#Else
    Const dblocation As String = ""
        '#End If


        Const strConnection As String = _
    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & dblocation & "settings.accdb;Persist Security Info=False"


        ' Constructor of the OleDbConnection object 
        Dim objConnection As New OleDbConnection(strConnection)

        objConnection.Open()
        'for all values on the UI, create an UPDATE statement and commit
        Dim strSQL As String = "SELECT username from UserSettings WHERE username ='" & parsePost("username") & "' and password='" & parsePost("password") & "';"
        Dim objcommand As New OleDbCommand(strSQL, objConnection)
        Dim objReader As OleDbDataReader
        Try

            objReader = objcommand.ExecuteReader


            If objReader.HasRows Then
                objReader.Read()
                strSQL = "INSERT INTO IPAssociation(username,ip_address) VALUES('" & objReader.GetValue(0) & "','" & settings.getIP().ToString & "');"
                objcommand = New OleDbCommand(strSQL, objConnection)
                objcommand.ExecuteNonQuery()
                settings.setAuthenticated()
            Else
                showLogin(request.getAttribute("User-Agent").tolower.contains("iphone"), "Invalid username/password combination")
                Exit Sub
            End If


            Dim html As String



            html = "<html>" & _
                "<head>" & _
"<meta http-equiv='refresh' content='1' > " & _
"</head>" & _
"<body>" & _
"Logged in successfully, will redirect in 3 seconds" & _
"</body>"

            Try
                Dim line1, line2, line3, line4, httpresponse As String  'create the HTTP response string

                line1 = "HTTP/1.0 200 OK"
                line2 = "Date: " & Now.ToLongDateString
                line3 = "Content-Type: text/html"
                line4 = "Content-Length:" & Len(html)


                httpresponse = line1 & vbCrLf & line2 & vbCrLf & line3 & vbCrLf & line4 & vbCrLf & vbCrLf
                sendHTML(httpresponse, html)
            Catch
            End Try



        Catch
        Finally
            objcommand.Dispose()
            objConnection.Close()
        End Try



    End Sub

    Private Sub tryLogOut()
        '#If DEBUG Then
        '       Const dblocation As String = "C:\Users\Edmund\Documents\Visual Studio 2008\Projects\FinalYearProject\FinalYearProject\bin\Debug\"
        '#Else
    Const dblocation As String = ""
        '#End If


        Const strConnection As String = _
    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & dblocation & "settings.accdb;Persist Security Info=False"


        ' Constructor of the OleDbConnection object 
        Dim objConnection As New OleDbConnection(strConnection)

        objConnection.Open()
        'for all values on the UI, create an UPDATE statement and commit
        Dim strSQL As String
        Dim objReader As OleDbDataReader


        strSQL = "DELETE FROM IPAssociation WHERE ip_address = '" & settings.getIP().ToString & "';"
        Dim objcommand As New OleDbCommand(strSQL, objConnection)
        objcommand.ExecuteNonQuery()



        Dim html As String = "<html>" & _
            "<head>" & _
            "<meta http-equiv='refresh' content='1' > " & _
            "</head>" & _
            "<body>" & _
            "Logged out successfully, will redirect in 3 seconds" & _
            "</body>"

        Try
            Dim line1, line2, line3, line4, httpresponse As String  'create the HTTP response string

            line1 = "HTTP/1.0 200 OK"
            line2 = "Date: " & Now.ToLongDateString
            line3 = "Content-Type: text/html"
            line4 = "Content-Length:" & Len(html)


            httpresponse = line1 & vbCrLf & line2 & vbCrLf & line3 & vbCrLf & line4 & vbCrLf & vbCrLf
            sendHTML(httpresponse, html)
        Catch
        End Try

        objcommand.Dispose()
        objConnection.Close()

    End Sub

    Public Sub CreateUser(ByVal pUsername As String, ByVal pPassword As String)
        '#If DEBUG Then
        '       Const dblocation As String = "C:\Users\Edmund\Documents\Visual Studio 2008\Projects\FinalYearProject\FinalYearProject\bin\Debug\"
        '#Else
    Const dblocation As String = ""
        '#End If


        Const strConnection As String = _
    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & dblocation & "settings.accdb;Persist Security Info=False"


        ' Constructor of the OleDbConnection object 
        Dim objConnection As New OleDbConnection(strConnection)

        objConnection.Open()
        'for all values on the UI, create an UPDATE statement and commit
        Dim strSQL As String = "INSERT INTO UserSettings VALUES('" & pUsername & "','" & pPassword & "',-1,-1,-1,0,-1,'pro','algorithm',-1,-1,0,'none');"
        Dim objcommand As New OleDbCommand(strSQL, objConnection)
        Dim objReader As OleDbDataReader

        objcommand.ExecuteNonQuery()

        objcommand.Dispose()
        objConnection.Close()
    End Sub

    Public Function validateNewUser()
        '#If DEBUG Then
        '       Const dblocation As String = "C:\Users\Edmund\Documents\Visual Studio 2008\Projects\FinalYearProject\FinalYearProject\bin\Debug\"
        '#Else
    Const dblocation As String = ""
        '#End If


        Const strConnection As String = _
    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & dblocation & "settings.accdb;Persist Security Info=False"


        ' Constructor of the OleDbConnection object 
        Dim objConnection As New OleDbConnection(strConnection)

        objConnection.Open()
        'for all values on the UI, create an UPDATE statement and commit
        Dim strSQL As String = "SELECT username from UserSettings WHERE username ='" & parsePost("username") & "' and password='" & parsePost("password") & "';"
        Dim objcommand As New OleDbCommand(strSQL, objConnection)
        Dim objReader As OleDbDataReader
        Try

            objReader = objcommand.ExecuteReader

            If objReader.HasRows Then
                Return "That username already exists"
            End If

        Catch
        Finally
            objcommand.Dispose()
            objConnection.Close()
        End Try


        If parsePost("password") <> parsePost("password2") Then
            Return "Password entries must match"
        End If

        If parsePost("username").trim.length < 5 Or parsePost("password").trim.length < 5 Then
            Return "Username and password must be at least 5 characters long"
        End If

        Return ""

    End Function
End Class