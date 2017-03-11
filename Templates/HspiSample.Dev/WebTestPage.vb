Imports System.Text
Imports System.Web
Imports Scheduler

Public Class WebTestPage
    Inherits clsPageBuilder

    Public Sub New(ByVal pagename As String)
        MyBase.New(pagename)
    End Sub

    Public Overrides Function postBackProc(page As String, data As String, user As String, userRights As Integer) As String
        Console.WriteLine("Sample Plugin test page post: " & data)
        Try
            Dim parts As Collections.Specialized.NameValueCollection
            parts = HttpUtility.ParseQueryString(data)
            If parts("uploadfile") = "true" Then
                ' handle uploading of file
                ' the orig file is ID_OriginalFile
                ' the temp file is ID_TempFile
                ' status is ID_Status
                ' return the proper JSON so the uploader completes
                ' success return=Return "{""success"":""true""}"
                ' if error return=Return "{""error"":""No directory specified""}"
                Return "{""success"":""true""}"
            End If

            Dim id As String = parts("id")
            If id = "btimerstop" Then
                Me.pageCommands.Add("stoptimer", "")
            End If
            If id = "btimerstart" Then
                Me.pageCommands.Add("starttimer", "")
            End If
            If id = "sradd" Then
                Me.propertySet.Add("my_region", "appenddiv=<br>" & Date.Now.ToString & ":added this to the scrolling region value=0 added this to the scrolling region added this to the scrolling region added this to the scrolling region added this to the scrolling region added this to the scrolling region added this to the scrolling region")
            End If
            If id = "spbut" Then
                Me.pageCommands.Add("stopspinner", "spin")
            End If
            If id = "butOpenSlide" Then
                Me.pageCommands.Add("slidingtabopen", "myslide1")
            End If
            If id = "butCloseSlide" Then
                Me.pageCommands.Add("slidingtabclose", "myslide1")
            End If
            If id = "myslide1_ID" Then
                'Me.pageCommands.Add("refresh", True)
            End If


            If id = "b111" Then

                Me.divToUpdate.Add("myslide1_ID_content", "new stuff for the tab")
            End If

            Return MyBase.postBackProc(page, data, user, userRights)
        Catch ex As Exception
            Console.WriteLine("Error Test page postback: " & ex.Message)
        End Try
        Return ""
    End Function

    Public Function GetPagePlugin(ByVal pageName As String, ByVal user As String, ByVal userRights As Integer, ByVal queryString As String) As String
        Dim stb As New StringBuilder
        Dim page As WebTestPage = Me

        Try
            page.reset()

            ' handle queries with special data
            Dim parts As Collections.Specialized.NameValueCollection = Nothing
            If (queryString <> "") Then
                parts = HttpUtility.ParseQueryString(queryString)
            End If
            If parts IsNot Nothing Then
                If parts("myslide1") = "myslide_name_open" Then
                    ' handle a get for tab content
                    Dim name As String = parts("name")
                    Return ("<table><tr><td>cell1</td><td>cell2</td></tr><tr><td>cell row 2</td><td>cell 2 row 2</td></tr></table>")
                    'Return ("<div><b>content data for tab</b><br><b>content data for tab</b><br><b>content data for tab</b><br><b>content data for tab</b><br><b>content data for tab</b><br><b>content data for tab</b><br></div>")
                End If
                If parts("myslide1") = "myslide_name_close" Then
                    Return ""
                End If
            End If


            page.AddHeader(stb.ToString)
            'page.RefreshIntervalMilliSeconds = 5000
            ' handler for our status div
            'stb.Append(page.AddAjaxHandler("/devicestatus?ref=3576", "stat"))
            stb.Append(Me.AddAjaxHandlerPost("action=updatetime", Me.PageName))



            ' page body starts here

            Me.AddHeader(hs.GetPageHeader(pageName, "Sample Plugin Controls Test", "", "", False, True))

            'Dim dv As DeviceClass = GetDeviceByRef(3576)
            'Dim CS As CAPIStatus
            'CS = dv.GetStatus

            stb.Append("This page displays all of the jquery controls availible<br><br>")

            stb.Append("<hr>Timer Test<br>")
            Dim btimerstop As New clsJQuery.jqButton("btimerstop", "Stop Timer", Me.PageName, False)
            stb.Append(btimerstop.Build)
            Dim btimerstart As New clsJQuery.jqButton("btimerstart", "Start Timer", Me.PageName, False)
            stb.Append(btimerstart.Build)

            stb.Append("<hr>Listbox Test<br><br>")
            Dim lb As New clsJQuery.jqListBox("list1", Me.PageName)
            lb.style = "height:100px;width:300px;"
            lb.items.Add("item 1")
            lb.items.Add("item 2")
            stb.Append(lb.Build)

            stb.Append("<hr>Listbox Test<br><br>")
            Dim lb1 As New clsJQuery.jqListBox("list2", Me.PageName)
            lb1.style = "height:100px;width:300px;"
            lb1.items.Add("item 3")
            lb1.items.Add("item 4")
            stb.Append(lb1.Build)


            stb.Append("<hr>Scrolling Region Test<br>")
            Dim srb As New clsJQuery.jqButton("sradd", "Add To Region", Me.PageName, False)

            stb.Append(srb.Build & "<br><br>")
            Dim sr As New clsJQuery.jqScrollingRegion("my_region")
            sr.className = ""
            sr.AddStyle("height:100px;overflow:auto;width: 500px;background: #BCE5FC;")
            sr.content = "Here is the content to scroll=0<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>Here is the content to scroll<br>"
            stb.Append(sr.Build)
            stb.Append("<br>")
            
            stb.Append("<hr>Spinner Test<br><br><br>")
            Dim sp As New clsJQuery.jqDynSpinner("spin")
            sp.left = 0
            stb.Append(sp.Build)
            Dim spbut As New clsJQuery.jqButton("spbut", "Stop Spinner", Me.PageName, False)
            stb.Append("<br><br><br>" & spbut.Build)

            stb.Append("<hr>Tabs Test<br><br>")
            Dim jqtabs As New clsJQuery.jqTabs("tab1id", Me.PageName)
            Dim tab As New clsJQuery.Tab
            tab.tabTitle = "Tab 1"
            tab.tabDIVID = "1234"
            tab.tabContent = "The content of tab 1<br><div id='test'>The content of tab 1</div><br>"
            jqtabs.postOnTabClick = True
            jqtabs.tabs.Add(tab)

            tab = New clsJQuery.Tab
            tab.tabTitle = "Tab 2"
            tab.tabContent = "The content of tab 2"
            jqtabs.tabs.Add(tab)

            stb.Append(jqtabs.Build)

            stb.Append("<br><br>")


            stb.Append("<span title='the tip'><input ></input></span>" & vbCrLf)

            stb.Append("<hr>Progress Bar Test<br><br>")
            Dim pb As New clsJQuery.jqProgressBar("pb1", 50)
            pb.className = "update_progress"
            stb.Append(pb.Build)
            stb.Append("<br>")


            stb.Append("<hr>ToolTip Test<br>")
            stb.Append("There should be a help icon after this text")
            stb.Append(New clsJQuery.jqToolTip("This is the tool tip!").build)

            stb.Append("<hr>Select File Test<br>")
            Dim fs As New clsJQuery.jqLocalFileSelector("fs1", Me.PageName, False)
            fs.label = "HS Folder"
            fs.path = gEXEPath
            fs.AddExtension("*.*")
            stb.Append(fs.Build)

            Dim fs2 As New clsJQuery.jqLocalFileSelector("fs2", Me.PageName, False)
            fs2.toolTip = "This will select a local file<br>This is on the next line.<a href=""/elog"">Log</a>"
            fs2.label = "Root of Drive"
            fs2.path = "C:\"
            fs2.AddExtension(".vb")
            stb.Append(fs2.Build)

            stb.Append("<hr>Overlay Test<br>")

            Dim ol As clsJQuery.jqOverlay = New clsJQuery.jqOverlay("ov1", Me.PageName, False, "events_overlay")
            ol.toolTip = "This will display the overlay"
            ol.label = "Display Overlay"
            'Dim bov1 As New clsJQuery.jqButton("ov1button", "Edit", Me.PageName, False)
            'bov1.includeClickFunction = False


            Dim tbov1 As New clsJQuery.jqTextBox("tbov1", "text", "hello", Me.PageName, 20, False)
            tbov1.toolTip = "This should be the tooltip for this textbox"
            Dim tbut1 As New clsJQuery.jqButton("tbut1", "submit", Me.PageName, True)
            ol.overlayHTML = clsPageBuilder.FormStart("overlayformm", "testpage", "post")
            ol.overlayHTML &= "<div>This is the overlay text<br><br>" & tbov1.Build & tbut1.Build & "</div>"
            ol.overlayHTML &= clsPageBuilder.FormEnd
            stb.Append(ol.Build)

            stb.Append("<hr>Upload File Test<br><br>")
            stb.Append(clsPageBuilder.FormStart("uploadform", "testpage", "post"))
            stb.Append("<input name='uploadaction' value='uploadactionvalue' type='hidden'>")


            Dim ul As New clsJQuery.jqFileUploader("uploader", Me.PageName)
            ul.toolTip = "This will upload a file"
            ul.values.Add("uploadfile", "true")
            ul.AddExtension("*.jpg")
            ul.AddExtension("*.wav")
            ul.AddExtension("*.png")
            ul.acceptFiles = "audio/*"
            ul.label = "My Label for Uploading"
            stb.Append(ul.Build)
            stb.Append("Stuff after the uploader")

            stb.Append(clsPageBuilder.FormEnd)

            stb.Append(HTML_StartTable(1))
            stb.Append(HTML_StartRow)
            stb.Append(HTML_StartCell("", 1))

            stb.Append(clsPageBuilder.FormStart("sliding_tab_form", "testpage", "post"))

            stb.Append("<hr>Button Textbox Test")
            Dim b1 As New clsJQuery.jqButton("b1", "Hide", Me.PageName, False)
            b1.toolTip = "This button will hide the slider"
            Dim b11 As New clsJQuery.jqButton("b11", "Show", Me.PageName, False)
            Dim b111 As New clsJQuery.jqButton("b111", "New Content", Me.PageName, False)
            Dim t1 As New clsJQuery.jqTextBox("t1", "text", "hello", Me.PageName, 20, False)




            stb.Append("<hr>Sliding tab Test<br><br>")
            Dim butOpenSlide As clsJQuery.jqButton = New clsJQuery.jqButton("butOpenSlide", "Open Slider", Me.PageName, False)
            stb.Append(butOpenSlide.Build)
            Dim butCloseSlide As clsJQuery.jqButton = New clsJQuery.jqButton("butCloseSlide", "Close Slider", Me.PageName, False)
            stb.Append(butCloseSlide.Build)
            stb.Append("<br><br>")
            Dim st As New clsJQuery.jqSlidingTab("myslide1", Me.PageName, False)
            st.initiallyOpen = True
            st.toolTip = "This is the tooltip for the sliding tab"
            st.tab.AddContent("the content")
            st.tab.name = "myslide_name"
            st.tab.tabName.Unselected = "Unselected Tab Title"
            st.tab.tabName.Selected = "Selected Tab Title" & t1.Build & b1.Build & b11.Build & b111.Build
            stb.Append(st.Build)

            stb.Append(clsPageBuilder.FormEnd)

            stb.Append(HTML_EndCell)
            stb.Append(HTML_EndRow)
            stb.Append(HTML_EndTable)

            stb.Append(clsPageBuilder.DivStart("demo", ""))
            stb.Append(clsPageBuilder.FormStart("myform1", "testpage", "post"))


            Dim slid As New clsJQuery.jqSlider("sliderid", 0, 100, 25, clsJQuery.jqSlider.jqSliderOrientation.horizontal, 90, Me.PageName, False)
            slid.toolTip = "This is the tooltip for the slider"
            stb.Append("<br><br>" & slid.build)

            stb.Append("<hr>jqTextBox Test<br><br>")
            Dim tb As New clsJQuery.jqTextBox("tb1", "text", "default text", Me.PageName, 10, True)
            tb.promptText = "This is the prompt text for this textbox"
            tb.toolTip = "This is the tooltip for this text box"
            'tb.dialogWidth = 200
            stb.Append(tb.Build)
            stb.Append("<br><br>")
            tb.name = "tb2"
            tb.promptText = "This is the prompt text for this textbox"
            tb.toolTip = "This is the tooltip for this text box"
            'tb.dialogWidth = 200
            stb.Append(tb.Build)
            tb.name = "tb3"
            tb.promptText = "This is the prompt text for this textbox"
            tb.toolTip = "This is the tooltip for this text box"
            'tb.dialogWidth = 200
            stb.Append(tb.Build)

            stb.Append("<hr>jqCheckBox Test")
            stb.Append(clsPageBuilder.FormStart("FormTestForCheckbox", "Something", "post"))
            Dim c As New clsJQuery.jqCheckBox("check1", "CheckLabel", Me.PageName, True, False)
            c.toolTip = "This is the tooltip for the checkbox"
            c.checked = True
            stb.Append("<br>")
            stb.Append(c.Build)
            stb.Append("<br>")

            Dim c2 As New clsJQuery.jqCheckBox("check2", "CheckLabel", Me.PageName, True, True)
            stb.Append("<br>")
            stb.Append(c2.Build)
            stb.Append("<br>")
            stb.Append(clsPageBuilder.FormEnd)

            stb.Append(HTML_StartTable(0))
            stb.Append(HTML_StartRow)
            stb.Append(HTML_StartCell("", 1, HTML_Align.LEFT, True))
            stb.Append(clsPageBuilder.FormStart("FormTestForDrag", "Something", "post"))
            stb.Append(clsPageBuilder.DivStart("SomesortOfDIV", ""))

            stb.Append(clsJQuery.DivStart("d1", "", True, False, "", "", Me.PageName))



            stb.Append("<hr>jqTimeSpanPicker Test<br><br>")

            Dim ts As New clsJQuery.jqTimeSpanPicker("tm1", "Time:", Me.PageName, True)
            ts.toolTip = "This is the tooltip for the timespan picker"
            ts.showSeconds = True
            'ts.formToPostID = "myform1"
            stb.Append(ts.Build)
            stb.Append("<br><br>")

            Dim tsbutt As New clsJQuery.jqButton("tsbutt", "submit timespan picker", Me.PageName, True)
            stb.Append(tsbutt.Build)
            stb.Append(clsPageBuilder.FormEnd)

            stb.Append(HTML_EndCell)
            stb.Append(HTML_EndRow)
            stb.Append(HTML_EndTable)

            stb.Append(clsJQuery.DivEnd)

            stb.Append(clsPageBuilder.FormStart("FormTestForDrag", "Something", "post"))

            stb.Append(clsJQuery.DivStart("d2", "tmdroppable", True, True, ".tmdroppable", "", Me.PageName))
            Dim ts2 As New clsJQuery.jqTimeSpanPicker("tm2", "Time:", Me.PageName, False)
            ts2.toolTip = "Time in line tooltip"
            ts2.showSeconds = False
            stb.Append("Time in line: " & ts2.Build)
            stb.Append("<br><br>")
            stb.Append(clsJQuery.DivEnd)

            stb.Append(clsJQuery.DivStart("d3", "tmdroppable", True, True, ".tmdroppable", "", Me.PageName))



            Dim ts3 As New clsJQuery.jqTimeSpanPicker("tm3", "Time:", Me.PageName, True)
            ts3.showSeconds = False
            ts3.showDays = False
            'ts3.formToPostID = "myform1"
            stb.Append(ts3.Build)
            stb.Append("<br><br>")
            stb.Append(clsJQuery.DivEnd)

            stb.Append(clsPageBuilder.DivEnd)

            stb.Append(clsPageBuilder.FormEnd)

            stb.Append(HTML_EndCell)
            stb.Append(HTML_EndRow)
            stb.Append(HTML_EndTable)





            stb.Append(clsPageBuilder.FormStart("myform3", "testpage", "post"))

            stb.Append("<hr>jqTimePicker Test<br><br>")
            Dim tp As New clsJQuery.jqTimePicker("mytm", "Time:", Me.PageName, False)
            tp.toolTip = "This is the tooltip for the Time Picker"
            tp.ampm = True
            tp.showSeconds = True
            tp.minutesSeconds = False
            tp.defaultValue = "1:30:45"
            stb.Append(tp.Build)
            stb.Append("<br>")
            Dim btimebut As New clsJQuery.jqButton("button", "Button for timepickerpost", Me.PageName, True)
            stb.Append(btimebut.Build)

            stb.Append("<hr>jqDatePicker Test")

            Dim dp As New clsJQuery.jqDatePicker("mydp", "Date:", Me.PageName, False, ",")
            'dp.toolTip = "This is the tooltip for the Date Picker"
            dp.multipleDates = True
            dp.allowWildcards = True
            dp.defaultDate = "1/1/2011,2/1/2012"
            dp.size = 40
            stb.Append(dp.Build)

            stb.Append(clsPageBuilder.TextBox("name", "name", "text", "", 8))
            stb.Append(clsPageBuilder.TextBox("address", "address", "text", "", 8))
            Dim b As New clsJQuery.jqButton("button1", "Button for form1", Me.PageName, True)
            b.primaryIcon = "ui-icon-locked"
            b.toolTip = "This is the tooltip"
            b.imagePathNormal = "images/HomeSeer/ui/Expand.png"

            stb.Append("<DIV id='u1'>DIV U1</DIV>")
            stb.Append("<DIV id='u2'>DIV U2</DIV>")

            stb.Append(b.Build)
            stb.Append(clsPageBuilder.FormEnd)

            stb.Append("<hr>jqTextBox/jqButton Test form post<br>")
            stb.Append(clsPageBuilder.FormStart("myform2", "testpage", "post"))
            Dim tb2 As New clsJQuery.jqTextBox("textbox2", "text", "", Me.PageName, 40, False)
            tb2.editable = True

            stb.Append(tb2.Build)
            'stb.Append(clsPageBuilder.TextBox("textbox2", "textbox2", "text", "", 8))
            Dim b2 As New clsJQuery.jqButton("buttonform2", "Button for form2", Me.PageName, True)
            stb.Append(b2.Build)
            stb.Append(clsPageBuilder.FormEnd)


            stb.Append("<hr>jqDropList Test<br><br>")
            stb.Append(clsPageBuilder.FormStart("mydroplistform", "mydroplistform_name", "post"))
            ' droplist

            Dim dl As New clsJQuery.jqDropList("droplistname", Me.PageName, False)
            dl.toolTip = "This is the tooltip for the Droplist"
            dl.AddItem("first one", "1", False)
            dl.AddItem("second one", "2", True)
            dl.autoPostBack = False
            stb.Append(dl.Build)

            dl.id = "droplistname2"
            dl.name = "droplistname3"
            dl.toolTip = "This is the tooltip for the second Droplist"
            dl.items.Clear()
            dl.AddItem("first one again", "1", False)
            dl.AddItem("second one again", "2", True)
            stb.Append(dl.Build)

            Dim dlbut As New clsJQuery.jqButton("dlbutt", "Submit Droplist Form", Me.PageName, True)
            stb.Append("<br>" & dlbut.Build)

            stb.Append(clsPageBuilder.FormEnd)


            ' selector
            stb.Append("<hr>jqSelector Test")
            stb.Append(clsPageBuilder.FormStart("myform4", "testpage", "post"))

            Dim s As New clsJQuery.jqSelector("sel1", Me.PageName, False)
            s.toolTip = "This is the tooltip for the selector"
            s.AddItem("Red", "1", False)
            s.AddItem("Blue", "2", True)
            s.AddItem("Green", "3", False)
            s.AddItem("Any Color", "4", True)
            s.label = "Edit Colors"
            s.dialogCaption = "Edit Colors 123"
            stb.Append(s.Build)

            Dim s2 As New clsJQuery.jqSelector("sel2", Me.PageName, False)
            s2.AddItem("value1", "1", False)
            s2.AddItem("value2", "2", True)
            s2.AddItem("value3", "3", False)
            s2.AddItem("my big long value here", "3", True)
            s2.label = "Edit Items"
            stb.Append(s2.Build)
            stb.Append(clsPageBuilder.FormEnd)

            stb.Append("<hr>jqRadioButton Test<br><br>")
            Dim rb As New clsJQuery.jqRadioButton("rb1", Me.PageName, False)
            'rb.buttonset = False
            rb.values.Add("Item 1", "1")
            rb.values.Add("Item 2", "2")
            rb.checked = "Item 2"
            stb.Append(rb.Build)

            stb.Append(clsPageBuilder.FormStart("colorpickerpost", "testpage", "post"))
            stb.Append("<hr>jqColorPicker Test<br>")
            Dim cp As New clsJQuery.jqColorPicker("color", "status", 40, "0000")
            stb.Append(cp.Build)
            Dim bcolor As New clsJQuery.jqButton("colorbutton", "Button for color picker submit", Me.PageName, True)
            stb.Append(bcolor.Build)
            stb.Append(clsPageBuilder.FormEnd)

            stb.Append("<hr>jqContainer Test<br><br>")
            stb.Append(clsJQuery.DivStart("container_div", "", False, False, "", "", Me.PageName))
            Dim room As New clsJQuery.jqContainer("parent1", "", "Container Title", "", 0, 0, 200, 100, "This is the container content on the first line<br><b>This is the second line in bold</b>", False, Me.PageName, False)
            room.backgroundColor = "e5e5e5"
            room.positionAbsolute = False

            stb.Append(room.build)
            stb.Append(clsJQuery.DivEnd)

            stb.Append("<hr>jqButton Test<br><br>")
            Dim jqbut As New clsJQuery.jqButton("jqb1", "Normal Button", Me.PageName, False)
            jqbut.toolTip = "Tooltip for the button"
            stb.Append(jqbut.Build)

            Dim jqbut2 As New clsJQuery.jqButton("jqb2", "", Me.PageName, False)
            jqbut2.imagePathNormal = "\images\HomeSeer\ui\Delete.png"
            jqbut2.imagePathPressed = "\images\HomeSeer\ui\Delete.png"
            jqbut2.toolTip = "Tooltip for the image button"
            stb.Append(jqbut2.Build)

            stb.Append("<hr>jqButton URL Test<br><br>")
            Dim urlbut As New clsJQuery.jqButton("urlbut", "Button", Me.PageName, False)
            urlbut.url = "\deviceutility"
            stb.Append(urlbut.Build)

            stb.Append("<hr>jqMultiselect Test<br><br>")
            Dim msel As New clsJQuery.jqMultiSelect("msel1", Me.PageName, False)
            msel.label = "Select test value"
            msel.AddItem("Option Name 1", "val1", False)
            msel.AddItem("Option Name 2", "val2", True)
            msel.AddItem("Option Name 3", "val3", True)
            stb.Append(msel.Build)




            stb.Append(clsPageBuilder.DivStart("", "id='result'"))
            stb.Append(clsPageBuilder.DivEnd)


            ' container test
            'Dim statCont As New clsJQuery.jqContainer("contid", "Office Lamp", "/homeseer/on.gif", 100, 100, "this is the content")
            'stb.Append(statCont.build)


            stb.Append(clsPageBuilder.DivEnd)



        Catch ex As Exception
            stb.Append("Test page error: " & ex.Message)
        End Try
        stb.Append("<br>")


        page.AddBody(stb.ToString)

        Return page.BuildPage
    End Function
End Class
