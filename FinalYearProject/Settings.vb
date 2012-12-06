Imports System.Data.OleDb

Public Class settings

    ' Connection required for application's Access database

    '#If DEBUG Then
    '   Const dblocation As String = "C:\Users\Edmund\Documents\Visual Studio 2008\Projects\FinalYearProject\FinalYearProject\bin\Debug\"
    '#Else
    'Const dblocation As String = "C:\FinalYearProject\"
    Const dblocation As String = ""
'#End If

    Const strConnection As String = _
           "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & dblocation & "settings.accdb;Persist Security Info=False"

    ' Constructor of the OleDbConnection object 
    Private objConnection As New OleDbConnection(strConnection)
    Private htmlfilter As Boolean 'Filter HTML?
    Private cssfilter As Boolean    'Filter CSS?
    Private imagefilter As Boolean 'Filter Images?
    Private animation As Boolean 'Static images only?
    Private flash As Boolean    'Block flash?
    Private filtertype As String 'What colourblindness
    Private rendertype As String 'What filter type
    Private alternateCSS As Boolean 'Use the other CSS parser?
    Private multithreading As Boolean 'Use multithreading when processing images?
    Private rgb(2) As Integer 'RGB offset values
    Private reducedQuality As Boolean 'Reduce image quality?
    Private cachingMode As String 'Cache images?
    Private contentlanguage As String 'What language is the resource
    Private port As Integer
    Private remoteIP As String
    Private authenticatedUser As Boolean = False
    Private username As String

    Public Sub populate()
        Try
            ' Open connection 
            objConnection.Open()
retry:

            Dim strSQL = "SELECT username FROM IPAssociation WHERE ip_address = '" & remoteIP & "';"
            Dim objcommand As New OleDbCommand(strSQL, objConnection)

            Dim objReader As OleDbDataReader
            objReader = objcommand.ExecuteReader

            If objReader.HasRows Then
                objReader.Read()
                authenticatedUser = True
                username = objReader.GetValue(0)
            Else
                Exit Sub
            End If


            ' SQL query
            strSQL = "SELECT FilterType, HTMLFilter, CSSFilter, ImageFilter, Animation, Flash, Rendertype, AlternateCSS, MultiThreading, ReducedImageQuality, CachingMode FROM UserSettings WHERE username = '" & username & "';"

            ' OleDbCommand constructor 
            objcommand = New OleDbCommand(strSQL, objConnection)

            ' Extract individual fields from table 


            objReader = objcommand.ExecuteReader

            If objReader.HasRows Then

                While objReader.Read
                    'load all the settings
                    filtertype = objReader.GetValue(0)
                    htmlfilter = objReader.GetValue(1)
                    cssfilter = objReader.GetValue(2)
                    imagefilter = objReader.GetValue(3)
                    animation = objReader.GetValue(4)
                    flash = objReader.GetValue(5)
                    rendertype = objReader.GetValue(6)
                    alternateCSS = objReader.GetValue(7)
                    multithreading = objReader.GetValue(8)
                    reducedQuality = objReader.GetValue(9)
                    cachingMode = objReader.GetValue(10)

                End While

            End If

            If filtertype = "custom" Then
                strSQL = "SELECT Red,Blue,Green FROM CustomProfiles WHERE Active = -1"

                ' OleDbCommand constructor 
                objcommand = New OleDbCommand(strSQL, objConnection)

                objReader = objcommand.ExecuteReader

                If objReader.HasRows Then

                    While objReader.Read
                        rgb(0) = objReader.GetValue(0)
                        rgb(1) = objReader.GetValue(1)
                        rgb(2) = objReader.GetValue(2)
                    End While

                End If
            End If

            ' Release resources 
            objReader.Close()
            objReader = Nothing
            objcommand.Dispose()
            objConnection.Close()
        Catch
            Dim x As New Random
            Randomize()
            Threading.Thread.Sleep(x.Next(10))
            GoTo retry 'if the DB is busy, then wait
        End Try
    End Sub

    Public Function getFilter()
        'Return a different filter depending on the settings
        Dim filter As Filter

        If rendertype = "offset" Then
            If filtertype = "rod" Then
                filter = New BWColourFilter

            ElseIf filtertype = "custom" Then

                filter = New OffsetFilter(rgb(0), rgb(1), rgb(2))
            Else
                If filtertype = "pro" Then
                    filter = New OffsetFilter(255, 0, 0)
                End If
                If filtertype = "deu" Then
                    filter = New OffsetFilter(0, 255, 0)
                End If
                If filtertype = "tri" Then
                    filter = New OffsetFilter(0, 0, 255)
                End If
            End If
        End If

        If rendertype = "none" Then
            filter = New NOColourFilter
        End If

        If rendertype = "EVIcolourmap" Or rendertype = "VCcolourmap" Then
            filter = New ColourMapFilter(filtertype, rendertype)
        End If

        If rendertype = "algorithm" Then
            filter = New linearFilter(filtertype)
        End If

        Return filter
    End Function

    Public Function getImageSetting()
        Return imagefilter
    End Function

    Public Function getHTMLSetting()
        Return htmlfilter
    End Function

    Public Function getCSSSetting()
        Return cssfilter
    End Function

    Public Function getAltCSS()
        Return alternateCSS
    End Function

    Public Function getFlash()
        Return flash
    End Function

    Public Function getAnimation()
        Return animation
    End Function

    Public Function getMultithreading()
        Return multithreading
    End Function

    Public Function getQuality()
        Return reducedQuality
    End Function

    Public Function getCachingMode()
        Return cachingMode
    End Function

    Public Function getRenderType()
        Return rendertype
    End Function

    Public Function getFilterType()
        Return filtertype
    End Function

    Public Function getPort()
        Return port
    End Function

    Public Sub setEncodingLanguage(ByVal str As String)
        contentlanguage = str
    End Sub

    Public Function getEncodingLanguage()
        Return contentlanguage
    End Function

    Public Sub setRenderType(ByVal s As String)
        rendertype = s
    End Sub

    Public Sub setFilterType(ByVal s As String)
        filtertype = s
    End Sub

    Public Sub setIP(ByVal s As String)
        remoteIP = s
    End Sub

    Public Function isAuthenticated()
        Return authenticatedUser
    End Function

    Public Sub setAuthenticated()
        authenticatedUser = True
    End Sub

    Public Function getIP()
        Return remoteIP
    End Function

    Public Function getUser()
        Return username
    End Function
End Class
