Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.IO
Imports System.Threading
Imports System.Drawing.Graphics
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Class Main

    Public Shared Sub Main(ByVal args As String())

        Dim port As Integer

        Dim objReader As New System.IO.StreamReader("port.txt")


        port = CInt(objReader.ReadLine())

        'Try
        'port = CInt(args(0)) 'set the port number from the arguments
        'Catch
        'port = 8151
        'End Try

        Dim counter As Integer = 0 'the counter is used to identify which proxy is being referenced in the console
        Dim tcplistener As New TcpListener(port) 'the object listening for incoming connections
        Try
            tcplistener.Start() 'start listening for connections
            Console.WriteLine("Listening on port {0}", +port)
        Catch
            MsgBox("Port " & port & " is being used by another process. Choose another port, or end the process that has reserved this port")
            End 'end the program if there is a port conflict
        End Try

        While True
            Dim socket As Socket = tcplistener.AcceptSocket() 'create a socket from an incoming connection
            Dim webproxy As New WebProxy(socket, counter) 'create a webproxy object using that socket object
            Dim thread As New Thread(AddressOf webproxy.Run) 'put the new webproxy onto a seperate thread
            thread.Start()
            counter += 1 'The counter is so the thread is identifiable in verbose mode
            If counter = 16777215 Then
                counter = 0 'just in case counter goes over max hex!
            End If
        End While


    End Sub

End Class




