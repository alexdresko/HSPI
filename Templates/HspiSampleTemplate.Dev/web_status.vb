Imports System.Text
Imports System.Web
Imports Scheduler
Imports HomeSeerAPI

Public Class web_status
    Inherits clsPageBuilder
    Dim TimerEnabled As Boolean
    Dim lbList As New Collection
    Dim ddTable As DataTable = Nothing

    Public Sub New(ByVal pagename As String)
        MyBase.New(pagename)
    End Sub

    Public Overrides Function postBackProc(page As String, data As String, user As String, userRights As Integer) As String

        Dim parts As Collections.Specialized.NameValueCollection
        Dim value As String = ""
        Dim name As String = ""
        parts = HttpUtility.ParseQueryString(data)

        Select Case parts("id")
            Case "oTabLB"
                PostMessage("The Listbox tab was selected.")
            Case "oTabCB"
                PostMessage("The Checkbox tab was selected.")
            Case "oTabDD"
                PostMessage("The Dropdown tab was selected.")
            Case "oButtonLB"
                lbList.Add(parts("TextboxLB"))
                PostMessage("'" & parts("TextboxLB") & "' has been added.")
                'BuildTabLB(True)
            Case "oButtonDD1"
                ddTable.Rows.Add("DD1", parts("tb1"), parts("tb2"))
                PostMessage("'" & parts("tb1") & "' with a value of " & parts("tb2") & " has been added.")
                'BuildTabDD(True)
            Case "oButtonDD2"
                ddTable.Rows.Add("DD2", parts("tb1"), parts("tb2"))
                PostMessage("'" & parts("tb1") & "' with a value of " & parts("tb2") & " has been added.")
                'BuildTabDD(True)
            Case "oCB1", "oCB2"
                name = parts("id")
                name = Right(name, name.Length - 1) 'strip the 'o' off the ID to get the name
                value = parts(name)
                PostMessage("Something was " & value & ".")
            Case "oSlider"
                value = parts("Slider")
                PostMessage("Value is " & value & ".")
            Case "timer" 'this stops the timer and clears the message
                If TimerEnabled Then 'this handles the initial timer post that occurs immediately upon enabling the timer.
                    TimerEnabled = False
                Else
                    Me.pageCommands.Add("stoptimer", "")
                    Me.divToUpdate.Add("message", "&nbsp;")
                End If
        End Select

        Return MyBase.postBackProc(page, data, user, userRights)
    End Function

    Public Function GetPagePlugin(ByVal pageName As String, ByVal user As String, ByVal userRights As Integer, ByVal queryString As String) As String
        Dim stb As New StringBuilder
        Dim instancetext As String = ""
        Try

            Me.reset()

            ' handle any queries like mode=something
            Dim parts As Collections.Specialized.NameValueCollection = Nothing
            If (queryString <> "") Then
                parts = HttpUtility.ParseQueryString(queryString)
            End If
            If Instance <> "" Then instancetext = " - " & Instance
            stb.Append(hs.GetPageHeader(pageName, "Sample" & instancetext, "", "", True, False))

            stb.Append(clsPageBuilder.DivStart("pluginpage", ""))

            ' a message area for error messages from jquery ajax postback (optional, only needed if using AJAX calls to get data)
            stb.Append(clsPageBuilder.DivStart("errormessage", "class='errormessage'"))
            stb.Append(clsPageBuilder.DivEnd)

            Me.RefreshIntervalMilliSeconds = 3000
            stb.Append(Me.AddAjaxHandlerPost("id=timer", pageName))

            ' specific page starts here

            stb.Append(BuildContent)

            stb.Append(clsPageBuilder.DivEnd)

            ' add the body html to the page
            Me.AddBody(stb.ToString)

            ' return the full page
            Return Me.BuildPage()
        Catch ex As Exception
            'WriteMon("Error", "Building page: " & ex.Message)
            Return "error - " & Err.Description
        End Try
    End Function

    Function BuildContent() As String
        Dim stb As New StringBuilder
        stb.Append("<script>function userConfirm() {  confirm(""hey - are you sure??"");  }</script>")
        stb.Append(" <table border='0' cellpadding='0' cellspacing='0' width='1000'>")
        stb.Append(" <tr><td width='1000' align='center' style='color:#FF0000; font-size:14pt; height:30px;'><strong><div id='message'>&nbsp;</div></strong></tr>")
        stb.Append("  <tr><td class='tablecolumn' width='270'>" & "Hello World" & "</td></tr>")
        stb.Append(" </table>")
        Return stb.ToString
    End Function


    Sub PostMessage(ByVal sMessage As String)
        Me.divToUpdate.Add("message", sMessage)
        Me.pageCommands.Add("starttimer", "")
        TimerEnabled = True
    End Sub

End Class
