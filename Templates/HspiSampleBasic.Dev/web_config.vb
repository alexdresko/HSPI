Imports System.Text
Imports System.Web
Imports Scheduler
Imports HomeSeerAPI

Public Class web_config
    Inherits clsPageBuilder
    Dim TimerEnabled As Boolean

    Public Sub New(ByVal pagename As String)
        MyBase.New(pagename)
    End Sub

    Public Overrides Function postBackProc(page As String, data As String, user As String, userRights As Integer) As String

        Dim parts As Collections.Specialized.NameValueCollection
        parts = HttpUtility.ParseQueryString(data)

        Select Case parts("id")
            Case "oTextbox1"
                PostMessage(parts("Textbox1"))
                BuildTextBox("Textbox1", True, "tested")
            Case "oButton1"
                Me.pageCommands.Add("newpage", "Sample_Status")
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

            CurrentPage = Me

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
        stb.Append(" <table border='0' cellpadding='0' cellspacing='0' width='1000'>")
        stb.Append(" <tr><td width='1000' align='center' style='color:#FF0000; font-size:14pt; height:30px;'><strong><div id='message'>&nbsp;</div></strong></tr>")
        stb.Append("  <tr><td class='tablecolumn' width='270'>Caption:" & BuildTextBox("Textbox1") & "</td></tr>")
        stb.Append("  <tr><td class='tablecolumn' width='270'>" & BuildButton("Button1") & "</td></tr>")
        stb.Append(" </table>")
        Return stb.ToString
    End Function

    Public Function BuildTextBox(ByVal Name As String, Optional ByVal Rebuilding As Boolean = False, Optional ByVal Text As String = "") As String
        Dim tb As New clsJQuery.jqTextBox(Name, "", Text, Me.PageName, 20, False)
        Dim TextBox As String = ""
        tb.id = "o" & Name

        TextBox = tb.Build

        If Rebuilding Then
            Me.divToUpdate.Add(Name & "_div", TextBox)
        Else
            TextBox = "<div id='" & Name & "_div'>" & tb.Build & "</div>"
        End If

        Return TextBox
    End Function

    Function BuildButton(ByVal Name As String, Optional ByVal Rebuilding As Boolean = False) As String
        Dim Content As String = ""
        Dim ButtonText As String = "Submit"
        Dim Button As String
        Dim b As New clsJQuery.jqButton(Name, "", Me.PageName, True)

        Select Case Name
            Case "Button1"
                ButtonText = "Go To Status Page"
                b.submitForm = False
        End Select

        b.id = "o" & Name
        b.label = ButtonText

        Button = b.Build

        If Rebuilding Then
            Me.divToUpdate.Add(Name & "_div", Button)
        Else
            Button = "<div id='" & Name & "_div'>" & Button & "</div>"
        End If
        Return Button
    End Function

    Sub PostMessage(ByVal sMessage As String)
        Me.divToUpdate.Add("message", sMessage)
        Me.pageCommands.Add("starttimer", "")
        TimerEnabled = True
    End Sub

End Class

