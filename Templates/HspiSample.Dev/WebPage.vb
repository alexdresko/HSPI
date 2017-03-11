Imports System.Text
Imports System.Web
Imports Scheduler

Public Class WebPage
    Inherits clsPageBuilder

    Public Sub New(ByVal pagename As String)
        MyBase.New(pagename)
    End Sub

    Public Overrides Function postBackProc(page As String, data As String, user As String, userRights As Integer) As String
        Dim parts As Collections.Specialized.NameValueCollection
        parts = HttpUtility.ParseQueryString(data)
        ' handle postbacks here
        ' update DIV'S with:
        'Me.divToUpdate.Add(DIV_ID, HTML_FOR_DIV)
        ' refresh a page (this page or other page):
        'Me.pageCommands.Add("newpage", url)
        ' open a dialog
        'Me.pageCommands.Add("opendialog", "dyndialog")
        If parts("id") = "b1" Then
            Me.divToUpdate.Add("current_time", "This div was just updated with this")
        End If
        If parts("action") = "updatetime" Then
            ' ajax timer has expired and posted back to us, update the time
            Me.divToUpdate.Add("current_time", DateTime.Now.ToString)
            If DateTime.Now.Second = 0 Then
                Me.divToUpdate.Add("updatediv", "job complete")
            ElseIf DateTime.Now.Second = 30 Then
                Me.divToUpdate.Add("updatediv", "working...")
            End If
        End If

        Return MyBase.postBackProc(page, data, user, userRights)
    End Function



    ' build and return the actual page
    Public Function GetPagePlugin(ByVal pageName As String, ByVal user As String, ByVal userRights As Integer, ByVal queryString As String) As String
        Dim stb As New StringBuilder

        Try
            Me.reset()

            ' handle any queries like mode=something
            Dim parts As Collections.Specialized.NameValueCollection = Nothing
            If (queryString <> "") Then
                parts = HttpUtility.ParseQueryString(queryString)
            End If

            ' add any custom menu items

            ' add any special header items
            'page.AddHeader(header_string)

            ' add the normal title
            Me.AddHeader(hs.GetPageHeader(pageName, "Sample Plugin", "", "", False, False))

            stb.Append(clsPageBuilder.DivStart("pluginpage", ""))

            ' a message area for error messages from jquery ajax postback (optional, only needed if using AJAX calls to get data)
            stb.Append(clsPageBuilder.DivStart("errormessage", "class='errormessage'"))
            stb.Append(clsPageBuilder.DivEnd)

            ' specific page starts here

            stb.Append("<div id='current_time'>" & DateTime.Now.ToString & "</div>" & vbCrLf)

            Dim b As New clsJQuery.jqButton("b1", "Button", IFACE_NAME, False)
            stb.Append(b.Build)
            stb.Append(clsPageBuilder.DivEnd)
            stb.Append("<br>pagename: " & pageName & "<br>")
            stb.Append("<br>This is instance: " & Instance & "<br>")

            Me.RefreshIntervalMilliSeconds = 2000
            stb.Append(Me.AddAjaxHandlerPost("action=updatetime", IFACE_NAME))

            ' add the body html to the page
            Me.AddBody(stb.ToString)

            Me.AddFooter(hs.GetPageFooter)
            Me.suppressDefaultFooter = True

            ' return the full page
            Return Me.BuildPage()
        Catch ex As Exception
            'WriteMon("Error", "Building page: " & ex.Message)
            Return "error"
        End Try
    End Function


End Class
