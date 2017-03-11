Imports System.Text
Imports System.Web
Imports Scheduler
Imports HomeSeerAPI

Public Class web_config
    Inherits clsPageBuilder
    Dim TimerEnabled As Boolean
    Public hspiref As HSPI

    Public Sub New(ByVal pagename As String)
        MyBase.New(pagename)
    End Sub

    Public Overrides Function postBackProc(page As String, data As String, user As String, userRights As Integer) As String

        Dim parts As Collections.Specialized.NameValueCollection
        parts = HttpUtility.ParseQueryString(data)
        Dim sKey As String
        Dim sValue As String

        Dim i As Integer
        For i = 0 To parts.Count - 1
            sKey = parts.GetKey(i)
            sValue = parts(sKey).Trim
            If sKey IsNot Nothing Then
                If sKey.StartsWith("removeinstance_") Then
                    Dim instance_name As String = sKey.Substring(15)
                    If instance_name <> "" Then
                        RemoveInstance(instance_name)
                        Me.pageCommands.Add("refresh", "true")
                    End If
                End If
            End If
        Next
        If parts("new_instance") = "Submit" Then
            Dim instance_name As String = parts("instance_name")
            If instance_name <> "" Then
                AddInstance(instance_name)
                Threading.Thread.Sleep(1000)    ' takes some time for new instance to connect and callbacks for registering pages to complete
                Me.pageCommands.Add("refresh", "true")
            End If
        End If

        Return MyBase.postBackProc(page, data, user, userRights)
    End Function

    Public Function GetPagePlugin(ByVal pageName As String, ByVal user As String, ByVal userRights As Integer, ByVal queryString As String, instance As String) As String
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


            ' specific page starts here
            Dim inst As String
            If hspiref.instance = "" Then
                inst = "Main"
            Else
                inst = hspiref.instance
            End If
            stb.Append("Current Instance: " & inst)
            stb.Append("<br><br>")

            If hspiref.instance = "" Then
                'main instance, list other instances
                Dim de As DictionaryEntry
                For Each de In AllInstances
                    Dim key As String = de.Key
                    Dim but As New clsJQuery.jqButton("removeinstance_" & key, "Remove", pageName, False)
                    stb.Append("<br>Instance: " & de.Key & "&nbsp&nbsp&nbsp" & but.Build)
                Next
            End If

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
        If hspiref.instance = "" Then
            stb.Append(clsPageBuilder.FormStart("form", "form", "Post"))
            stb.Append("<br><br>New instance name:<br>")
            Dim tb As New clsJQuery.jqTextBox("instance_name", "text", "", PageName, 40, False)
            tb.editable = True
            stb.Append(tb.Build)
            stb.Append("<br>")
            Dim but As New clsJQuery.jqButton("new_instance", "Create New Plugin Instance", PageName, True)
            stb.Append(but.Build)
        Else
            stb.Append("<br>Nothing to configure as this is not the main instance")
        End If
        stb.Append(clsPageBuilder.FormEnd)
        Return stb.ToString
    End Function

    


    

    Sub PostMessage(ByVal sMessage As String)
        Me.divToUpdate.Add("message", sMessage)
        Me.pageCommands.Add("starttimer", "")
        TimerEnabled = True
    End Sub

End Class

