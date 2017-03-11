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
                BuildTabLB(True)
            Case "oButtonDD1"
                ddTable.Rows.Add("DD1", parts("tb1"), parts("tb2"))
                PostMessage("'" & parts("tb1") & "' with a value of " & parts("tb2") & " has been added.")
                BuildTabDD(True)
            Case "oButtonDD2"
                ddTable.Rows.Add("DD2", parts("tb1"), parts("tb2"))
                PostMessage("'" & parts("tb1") & "' with a value of " & parts("tb2") & " has been added.")
                BuildTabDD(True)
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
        stb.Append("  <tr><td class='tablecolumn' width='270'>" & BuildTabs() & "</td></tr>")
        stb.Append(" </table>")
        Return stb.ToString
    End Function

    Public Function BuildTabs() As String
        Dim stb As New StringBuilder
        Dim tabs As clsJQuery.jqTabs = New clsJQuery.jqTabs("oTabs", Me.PageName)
        Dim tab As New clsJQuery.Tab

        tabs.postOnTabClick = True
        tab.tabTitle = "Listbox"
        tab.tabDIVID = "oTabLB"
        tab.tabContent = "<div id='TabLB_div'>" & BuildTabLB() & "</div>"
        tabs.tabs.Add(tab)
        tab = New clsJQuery.Tab
        tab.tabTitle = "Checkbox"
        tab.tabDIVID = "oTabCB"
        tab.tabContent = "<div id='TabCB_div'>" & BuildTabCB() & "</div>"
        tabs.tabs.Add(tab)
        tab = New clsJQuery.Tab
        tab.tabTitle = "Drop Down"
        tab.tabDIVID = "oTabDD"
        tab.tabContent = "<div id='TabDD_div'>" & BuildTabDD() & "</div>"
        tabs.tabs.Add(tab)
        tab = New clsJQuery.Tab
        tab.tabTitle = "Slider"
        tab.tabDIVID = "oTabSL"
        tab.tabContent = "<div id='TabSL_div'>" & BuildTabSL() & "</div>"
        tabs.tabs.Add(tab)
        tab = New clsJQuery.Tab
        tab.tabTitle = "Sliding Tab"
        tab.tabDIVID = "oTabST"
        tab.tabContent = "<div id='TabST_div'>" & BuildTabST() & "</div>"
        tabs.tabs.Add(tab)

        Return tabs.Build
    End Function

    Function BuildTabLB(Optional ByVal Rebuilding As Boolean = False) As String
        Dim stb As New StringBuilder
        Dim lb As New clsJQuery.jqListBox("lb", Me.PageName)
        Dim ms1 As New clsJQuery.jqMultiSelect("ms1", Me.PageName, True)
        Dim ms2 As New clsJQuery.jqMultiSelect("ms2", Me.PageName, False)
        Dim sel As New clsJQuery.jqSelector("sel", Me.PageName, False)
        Dim sel2 As New clsJQuery.jqSelector("sel2", Me.PageName, False)
        ms2.width = 300
        ms2.Size = 6
        lb.style = "height:100px;width:300px;"
        Dim item As Object
        For Each item In lbList
            lb.items.Add(item)
            ms1.AddItem(item, item, False)
            ms2.AddItem(item, item, False)
            sel.AddItem(item, item, False)
        Next
        stb.Append(clsPageBuilder.FormStart("frmTab1", "ListBox", "Post"))
        stb.Append(sel.Build & ms1.Build & ms2.Build & "<br>")
        stb.Append(lb.Build & " " & BuildTextbox("TextboxLB") & " " & BuildButton("ButtonLB"))
        stb.Append(sel2.Build & "<br>")
        stb.Append(clsPageBuilder.FormEnd())
        If Rebuilding Then Me.divToUpdate.Add("TabLB_div", stb.ToString)
        Return stb.ToString
    End Function

    Function BuildTabCB() As String
        Dim stb As New StringBuilder
        Dim cb1 As New clsJQuery.jqCheckBox("CB1", "check 1", Me.PageName, True, False)
        Dim cb2 As New clsJQuery.jqCheckBox("CB2", "check 2", Me.PageName, True, True)
        Dim cb3 As New clsJQuery.jqCheckBox("CB3", "check 3", Me.PageName, False, True)
        Dim cb4 As New clsJQuery.jqCheckBox("CB4", "check 4", Me.PageName, False, False)
        
        cb1.id = "oCB1"
        cb2.id = "oCB2"
        cb3.id = "oCB3"
        cb4.id = "oCB4"

        stb.Append(clsPageBuilder.FormStart("frmTab2", "checkbox", "Post"))
        stb.Append(cb1.Build())
        stb.Append(cb2.Build())
        stb.Append(cb3.Build())
        stb.Append(cb4.Build())
        stb.Append(BuildButton("ButtonCB1") & " ")
        stb.Append(clsPageBuilder.FormEnd())
        Return stb.ToString
    End Function

    Function BuildTabDD(Optional ByVal Rebuilding As Boolean = False) As String
        Dim stb As New StringBuilder
        stb.Append(clsPageBuilder.FormStart("frmTab3", "DropDown", "Post"))
        stb.Append(BuildDD("dd1") & " ")
        stb.Append(BuildDD("dd2") & " ")
        stb.Append("Name:" & BuildTextbox("tb1") & " ")
        stb.Append("Value:" & BuildTextbox("tb2") & " ")
        stb.Append(BuildButton("ButtonDD1") & " ")
        stb.Append(BuildButton("ButtonDD2") & " ")
        stb.Append(clsPageBuilder.FormEnd())
        If Rebuilding Then Me.divToUpdate.Add("TabDD_div", stb.ToString)
        Return stb.ToString
    End Function

    Function BuildTabSL() As String
        Dim stb As New StringBuilder
        stb.Append(BuildSlider("Slider"))
        Return stb.ToString
    End Function


    Function BuildTabST(Optional ByVal Rebuilding As Boolean = False) As String
        Dim stb As New StringBuilder
        Dim st As New clsJQuery.jqSlidingTab("myslide1ID", Me.PageName, False)
        st.initiallyOpen = True
        st.callGetOnOpenClose = False
        st.tab.AddContent("my sliding tab")
        st.tab.name = "myslide_name"
        st.tab.tabName.Unselected = "Unselected Tab Title"
        st.tab.tabName.Selected = "Selected Tab Title"


        stb.Append(st.Build)
        If Rebuilding Then Me.divToUpdate.Add("TabST_div", stb.ToString)
        Return stb.ToString
    End Function

    Function BuildSlider(ByVal Name As String, Optional ByVal value As Integer = 0) As String
        Dim slider As New clsJQuery.jqSlider(Name, 0, 100, value, clsJQuery.jqSlider.jqSliderOrientation.horizontal, 200, Me.PageName, True)
        slider.id = "o" & Name
        Return slider.build
    End Function

    Function BuildTextbox(ByVal Name As String) As String
        Dim tb As New clsJQuery.jqTextBox(Name, "", "", Me.PageName, 20, True)
        tb.id = "o" & Name
        tb.editable = True
        Return tb.Build
    End Function

    Function BuildDD(Name As String, Optional ByVal SelectedValue As String = "")
        Dim dd As New clsJQuery.jqDropList("dd", Me.PageName, False)
        Dim sel As Boolean
        Dim Rows() As DataRow
        Dim Row As DataRow

        dd.id = "o" & Name
        dd.autoPostBack = True

        dd.AddItem("", "", False)

        'save the visible text of the options for later use
        If (ddTable Is Nothing) Then
            ddTable = New DataTable
            ddTable.Columns.Add("ObjectName", GetType(String))
            ddTable.Columns.Add("OptionName", GetType(String))
            ddTable.Columns.Add("OptionValue", GetType(String))
        End If

        Rows = ddTable.Select("ObjectName='" & Name & "'")

        For Each Row In Rows
            If Row.Item("OptionValue") = SelectedValue Then
                sel = True
            Else
                sel = False
            End If
            dd.AddItem(Row.Item("OptionName"), Row.Item("OptionValue"), sel)
        Next

        ddTable.AcceptChanges()

        Return dd.Build
    End Function

    Function BuildButton(ByVal Name As String) As String
        Dim ButtonText As String = "Submit"
        Dim b As New clsJQuery.jqButton(Name, "", Me.PageName, True)

        Select Case Name
            Case "ButtonLB"
                ButtonText = "Add to Listbox"
            Case "ButtonDD1"
                ButtonText = "Add to Dropdown 1"
            Case "ButtonDD2"
                ButtonText = "Add to Dropdown 2"
        End Select

        b.functionToCallOnClick = "userConfirm()"
        b.includeClickFunction = True
        b.id = "o" & Name
        b.label = ButtonText

        Return b.Build()
    End Function

    Public Function DDText(ByVal DDName As String, DDValue As String) As String
        Dim Rows() As DataRow
        Dim Row As DataRow
        Dim ReturnValue As String = ""
        Rows = ddTable.Select("ObjectName='" & DDName & "' AND OptionValue='" & DDValue & "'")
        For Each Row In Rows
            ReturnValue = Row.Item("OptionName")
        Next
        Return ReturnValue
    End Function

    Public Function DDValue(ByVal DDName As String, DDText As String) As String
        Dim Rows() As DataRow
        Dim Row As DataRow
        Dim ReturnValue As String = ""
        Rows = ddTable.Select("ObjectName='" & DDName & "' AND OptionName='" & DDText & "'")
        For Each Row In Rows
            ReturnValue = Row.Item("OptionValue")
        Next
        Return ReturnValue
    End Function

    Sub PostMessage(ByVal sMessage As String)
        Me.divToUpdate.Add("message", sMessage)
        Me.pageCommands.Add("starttimer", "")
        TimerEnabled = True
    End Sub

End Class
