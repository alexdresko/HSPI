Imports HSCF
Imports HomeSeerAPI
Imports Scheduler
Imports HSCF.Communication.ScsServices.Service
Imports System.Reflection
Imports System.Text




Public Class HSPI
    Implements IPlugInAPI               ' this API is required for ALL plugins

    Public OurInstanceFriendlyName As String = ""

    Public pluginpage As WebPage            ' a jquery web page
    Public pluginTestPage As WebTestPage
    Dim WebPageName As String = "Sample_Web_Page"
    Public Shared bShutDown As Boolean = False


#Region "Externally Accessible Methods/Procedures - e.g. Script Commands"

    Public Function MyLocalFunction() As Integer
        Return 555
    End Function

    Public Function MySquareFunction(ByVal parms() As Object) As Double
        If parms Is Nothing Then Return 0
        If parms.Count < 1 Then Return 0
        If parms(0) Is Nothing Then Return 0
        If Not IsNumeric(parms(0)) Then Return 0

        Dim NType As System.Type = New Object().GetType 'Default to object.
        Try
            NType = parms(0).GetType
        Catch ex As Exception
        End Try
        Try
            Return parms(0) ^ 2
        Catch ex As Exception
            Log("MySquareFunction returned an exception - bad input perhaps? Type of input=" & NType.ToString & ", Ex=" & ex.Message, LogType.LOG_TYPE_ERROR)
            Return 0
        End Try

    End Function

#End Region

#Region "Common Interface"

    Dim Zone(5) As String   ' For search demonstration purposes only.
    Dim OneOfMyDevices As New Scheduler.Classes.DeviceClass
    Public Function Search(ByVal SearchString As String, ByVal RegEx As Boolean) As HomeSeerAPI.SearchReturn() Implements IPlugInAPI.Search
        ' Not yet implemented in the Sample
        '
        ' Normally we would do a search on plug-in actions, triggers, devices, etc. for the string provided, using
        '   the string as a regular expression if RegEx is True.
        '
        Dim colRET As New Collections.Generic.List(Of SearchReturn)
        Dim RET As SearchReturn = Nothing

        'So let's pretend we searched through all of the plug-in resources (triggers, actions, web pages, perhaps zone names, songs, etc.) 
        ' and found a few matches....  

        '   The matches can be returned as just the string value...:
        RET = New SearchReturn
        RET.RType = eSearchReturn.r_String_Other
        RET.RDescription = "Found in the zone description for zone 4"
        RET.RValue = Zone(4)
        colRET.Add(RET)
        '   The matches can be returned as a URL:
        RET = New SearchReturn
        RET.RType = eSearchReturn.r_URL
        RET.RValue = IFACE_NAME & Instance  ' Could have put something such as /DeviceUtility?ref=12345&edit=1     to take them directly to the device properties of a device.
        colRET.Add(RET)
        '   The matches can be returned as an Object:
        '   This will be VERY infrequently used as it is restricted to object types that can go through the HomeSeer-Plugin interface.
        '   Normal data type objects (Date, String, Integer, Enum, etc.) can go through, but very few complex objects such as the 
        '       HomeSeer DeviceClass will make it through the interface unscathed.
        RET = New SearchReturn
        RET.RType = eSearchReturn.r_Object
        RET.RDescription = "Found in a device."
        RET.RValue = hs.DeviceName(OneOfMyDevices.Ref(hs))      'Returning a string in the RValue is optional since this is an object type return
        RET.RObject = OneOfMyDevices
        colRET.Add(RET)

        Return colRET.ToArray

    End Function


    ' a custom call to call a specific procedure in the plugin
    Public Function PluginFunction(ByVal proc As String, ByVal parms() As Object) As Object Implements IPlugInAPI.PluginFunction
        Try
            Dim ty As Type = Me.GetType
            Dim mi As MethodInfo = ty.GetMethod(proc)
            If mi Is Nothing Then
                Log("Method " & proc & " does not exist in this plugin.", LogType.LOG_TYPE_ERROR)
                Return Nothing
            End If
            Return (mi.Invoke(Me, parms))
        Catch ex As Exception
            Log("Error in PluginProc: " & ex.Message, LogType.LOG_TYPE_ERROR)
        End Try

        Return Nothing
    End Function

    Public Function PluginPropertyGet(ByVal proc As String, parms() As Object) As Object Implements IPlugInAPI.PluginPropertyGet
        Try
            Dim ty As Type = Me.GetType
            Dim mi As PropertyInfo = ty.GetProperty(proc)
            If mi Is Nothing Then
                Log("Method " & proc & " does not exist in this plugin.", LogType.LOG_TYPE_ERROR)
                Return Nothing
            End If
            Return mi.GetValue(Me, Nothing)
        Catch ex As Exception
            Log("Error in PluginProc: " & ex.Message, LogType.LOG_TYPE_ERROR)
        End Try

        Return Nothing
    End Function

    Public Sub PluginPropertySet(ByVal proc As String, value As Object) Implements IPlugInAPI.PluginPropertySet
        Try
            Dim ty As Type = Me.GetType
            Dim mi As PropertyInfo = ty.GetProperty(proc)
            If mi Is Nothing Then
                Log("Property " & proc & " does not exist in this plugin.", LogType.LOG_TYPE_ERROR)
            End If
            mi.SetValue(Me, value, Nothing)
        Catch ex As Exception
            Log("Error in PluginPropertySet: " & ex.Message, LogType.LOG_TYPE_ERROR)
        End Try
    End Sub


    Public ReadOnly Property Name As String Implements HomeSeerAPI.IPlugInAPI.Name
        Get
            Return IFACE_NAME
        End Get
    End Property

    Public Function Capabilities() As Integer Implements HomeSeerAPI.IPlugInAPI.Capabilities
        Return HomeSeerAPI.Enums.eCapabilities.CA_IO
    End Function

    ' return 1 for a free plugin
    ' return 2 for a licensed (for pay) plugin
    Public Function AccessLevel() As Integer Implements HomeSeerAPI.IPlugInAPI.AccessLevel
        Return 1
    End Function

    Public ReadOnly Property HSCOMPort As Boolean Implements HomeSeerAPI.IPlugInAPI.HSCOMPort
        Get
            Return True  'We want HS to give us a com port number for accessing the hardware via a serial port
        End Get
    End Property

    Public Function SupportsMultipleInstances() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsMultipleInstances
        Return False
    End Function

    Public Function SupportsMultipleInstancesSingleEXE() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsMultipleInstancesSingleEXE
        Return False
    End Function

    Public Function InstanceFriendlyName() As String Implements HomeSeerAPI.IPlugInAPI.InstanceFriendlyName
        Return OurInstanceFriendlyName
    End Function

    Private WithEvents test_timer As New System.Timers.Timer
    Private timerVal As Double
    Private Sub test_timer_Elapsed(sender As Object, e As System.Timers.ElapsedEventArgs) Handles test_timer.Elapsed
        timerVal += 1       ' increment our test value so we can see it change on the device and test triggers
        ' find our device, we only have one
        Dim dv As Scheduler.Classes.DeviceClass
        Dim ref As Integer
        Try
            Dim EN As Scheduler.Classes.clsDeviceEnumeration
            EN = hs.GetDeviceEnumerator
            If EN Is Nothing Then Throw New Exception(IFACE_NAME & " failed to get a device enumerator from HomeSeer.")
            Do
                dv = EN.GetNext
                If dv Is Nothing Then Continue Do
                If dv.Interface(Nothing) IsNot Nothing Then
                    If dv.Interface(Nothing).Trim = IFACE_NAME Then
                        If dv.Ref(Nothing) = MyDevice Then
                            ref = dv.Ref(Nothing)
                            Console.WriteLine("Updating device with ref " & ref.ToString & " to value " & timerVal.ToString)
                            'hs.SetDeviceValueByRef(ref, timerVal, True)
                            hs.SetDeviceValueByRef(ref, 2000, True)
                            hs.SetDeviceString(ref, "/images/browser", False)
                        ElseIf dv.Ref(Nothing) = MyTempDevice Then
                            ref = dv.Ref(Nothing)
                            hs.SetDeviceValueByRef(ref, 72, True)   ' simulate setting the thermostat temp
                        End If
                    End If
                End If
            Loop Until EN.Finished
        Catch ex As Exception
            hs.WriteLog(IFACE_NAME & " Error", "Exception in Find_Create_Devices/Enumerator: " & ex.Message)
        End Try

        ' as a test, raise our generic callback here, our HSEvent will then be called.
        callback.RaiseGenericEventCB("sample_type", {"1", "2"}, IFACE_NAME, "")
    End Sub

    Public Function CustomFunction() As Integer
        Return 123
    End Function

    ' this sub demonstrates how to access another plugin by plugin name and instance
    Private Sub AccessPlugin()

        Dim pa As New PluginAccess(hs, "Z-Wave", "")
        If pa.Connected Then
            hs.WriteLog(IFACE_NAME, "Connected to plugin Z-Wave")
            hs.WriteLog(IFACE_NAME, "Interface name: " & pa.Name & " Interface status: " & pa.InterfaceStatus.intStatus.ToString)

            Dim r As Integer
            r = pa.PluginFunction("CustomFunction", Nothing)
            hs.WriteLog(IFACE_NAME, "r:" & r.ToString)
        Else
            hs.WriteLog(IFACE_NAME, "Could not connect to plugin Z-Wave, is it running?")
        End If
    End Sub

#If PlugDLL Then
    ' These 2 functions for internal use only
    Public Property HSObj As HomeSeerAPI.IHSApplication Implements HomeSeerAPI.IPlugInAPI.HSObj
        Get
            Return hs
        End Get
        Set(value As HomeSeerAPI.IHSApplication)
            hs = value
        End Set
    End Property

    Public Property CallBackObj As HomeSeerAPI.IAppCallbackAPI Implements HomeSeerAPI.IPlugInAPI.CallBackObj
        Get
            Return callback
        End Get
        Set(value As HomeSeerAPI.IAppCallbackAPI)
            callback = value
        End Set
    End Property
#End If

    Public Function InitIO(ByVal port As String) As String Implements HomeSeerAPI.IPlugInAPI.InitIO
        Console.WriteLine("InitIO called with parameter port as " & port)

        hs.WriteLog(IFACE_NAME, "Initializing sample plugin on port " & port.ToString)

        Dim plugins() As String = hs.GetPluginsList
        gEXEPath = hs.GetAppPath

        Try
            'AccessPlugin()

            Dim wpd As New WebPageDesc
#If 1 Then
            ' create our jquery web page
            pluginpage = New WebPage(WebPageName)
            ' register the page with the HS web server, HS will post back to the WebPage class
            ' "pluginpage" is the URL to access this page
            ' comment this out if you are going to use the GenPage/PutPage API istead
            If Instance = "" Then
                hs.RegisterPage(IFACE_NAME, IFACE_NAME, Instance)
            Else
                hs.RegisterPage(IFACE_NAME & Instance, IFACE_NAME, Instance)
            End If


            ' create test page
            pluginTestPage = New WebTestPage("Test Page")
            hs.RegisterPage("Test Page", IFACE_NAME, Instance)

            ' register a configuration link that will appear on the interfaces page

            wpd.link = IFACE_NAME & Instance                ' we add the instance so it goes to the proper plugin instance when selected
            If Instance <> "" Then
                wpd.linktext = "Sample Plugin Config instance " & Instance
            Else
                wpd.linktext = "Sample Plugin Config"
            End If

            wpd.page_title = "Sample Plugin Config"
            wpd.plugInName = IFACE_NAME
            wpd.plugInInstance = Instance
            callback.RegisterConfigLink(wpd)
#End If

            ' register a normal page to appear in the HomeSeer menu
            wpd = New WebPageDesc
            wpd.link = IFACE_NAME & Instance
            If Instance <> "" Then
                wpd.linktext = "Sample Plugin Page instance " & Instance
            Else
                wpd.linktext = "Sample Plugin Page"
            End If
            wpd.page_title = "Sample Plugin Page"
            wpd.plugInName = IFACE_NAME
            wpd.plugInInstance = Instance
            callback.RegisterLink(wpd)
#If 1 Then
            ' register a normal page to appear in the HomeSeer menu
            wpd = New WebPageDesc
            wpd.link = "Test Page" & Instance
            If Instance <> "" Then
                wpd.linktext = "Sample Plugin Test Page instance " & Instance
            Else
                wpd.linktext = "Sample Plugin Test Page"
            End If
            wpd.page_title = "Sample Plugin Test Page"
            wpd.plugInName = IFACE_NAME
            wpd.plugInInstance = Instance
            callback.RegisterLink(wpd)
#End If
            ' init a speak proxy
            'callback.RegisterProxySpeakPlug(IFACE_NAME, "")

            ' register a generic callback for other plugins to raise to use
            callback.RegisterGenericEventCB("sample_type", IFACE_NAME, "")

            hs.WriteLog(IFACE_NAME, "InitIO called, plug-in is being initialized...")

            Dim Arr() As IPlugInAPI.strTrigActInfo
            Dim strTrig As strTrigger = Nothing
            Dim Trig1 As MyTrigger1Ton = Nothing
            Dim Trig2 As MyTrigger2Shoe = Nothing
            Try
                Arr = callback.GetTriggers(IFACE_NAME)
            Catch ex As Exception
                Arr = Nothing
            End Try
            If Arr IsNot Nothing AndAlso Arr.Count > 0 Then
                For Each Info As IPlugInAPI.strTrigActInfo In Arr
                    'hs.WriteLog(IFACE_NAME & " Warning", "Got Trigger: EvRef=" & Info.evRef.ToString & ", Trig/SubTrig=" & Info.TANumber.ToString & "/" & Info.SubTANumber.ToString & ", UID=" & Info.UID.ToString)
                    If ValidTrigInfo(Info) Then
                        strTrig = Nothing
                        strTrig = GetTrigs(Info, Info.DataIn)
                        If strTrig.Result Then
                            If strTrig.WhichTrigger <> eTriggerType.Unknown Then
                                If strTrig.WhichTrigger = eTriggerType.OneTon Then
                                    Try
                                        Trig1 = CType(strTrig.TrigObj, MyTrigger1Ton)
                                    Catch ex As Exception
                                        Trig1 = Nothing
                                    End Try
                                    If Trig1 IsNot Nothing Then
                                        Add_Update_Trigger(Trig1)
                                    End If
                                ElseIf strTrig.WhichTrigger = eTriggerType.TwoVolts Then
                                    Try
                                        Trig2 = CType(strTrig.TrigObj, MyTrigger2Shoe)
                                    Catch ex As Exception
                                        Trig2 = Nothing
                                    End Try
                                    If Trig2 IsNot Nothing Then
                                        Add_Update_Trigger(Trig2)
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next
            End If

            hs.WriteLog(IFACE_NAME, colTrigs.Count.ToString & " triggers were loaded from HomeSeer.")

            hs.WriteLog(IFACE_NAME, "Checking for devices owned by " & IFACE_NAME)
            Find_Create_Devices()

            ' register for events from homeseer if a device changes value
            callback.RegisterEventCB(Enums.HSEvent.VALUE_CHANGE, IFACE_NAME, "")

            Demo_Start()

            test_timer.Interval = 10000
            test_timer.Enabled = True

            hs.SaveINISetting("Settings", "test", Nothing, "hspi_HSTouch.ini")

            ' example of how to save a file to the HS images folder, mainly for use by plugins that are running remotely, album art, etc.
            'SaveImageFileToHS(gEXEPath & "\html\images\browser.png", "sample\browser.png")
            'SaveFileToHS(gEXEPath & "\html\images\browser.png", "sample\browser.png")
        Catch ex As Exception
            bShutDown = True
            Return "Error on InitIO: " & ex.Message
        End Try

        bShutDown = False
        Return ""       ' return no error, or an error message

    End Function


    Private configcalls As Integer
    Public Function ConfigDevice(ref As Integer, user As String, userRights As Integer, newDevice As Boolean) As String Implements HomeSeerAPI.IPlugInAPI.ConfigDevice
        Dim stb As New StringBuilder

        stb.Append("<br>Call: " & configcalls.ToString & "<br><br>")
        Dim but As New clsJQuery.jqButton("button", "Press", "deviceutility", True)
        stb.Append(but.Build)

        stb.Append(clsPageBuilder.DivStart("sample_div", ""))
        stb.Append(clsPageBuilder.DivEnd)


        configcalls += 1

        Return stb.ToString
    End Function

    Public Function ConfigDevicePost(ref As Integer, data As String, user As String, userRights As Integer) As Enums.ConfigDevicePostReturn Implements HomeSeerAPI.IPlugInAPI.ConfigDevicePost
        Console.WriteLine("ConfigDevicePost: " & data)

        ' handle form items:
        Dim parts As Collections.Specialized.NameValueCollection
        parts = System.Web.HttpUtility.ParseQueryString(data)
        ' handle items like:
        ' if parts("id")="mybutton" then
        'callback.ConfigPageCommandsAdd("newpage", "status")
        callback.ConfigDivToUpdateAdd("sample_div", "The div has been updated with this content")
        Return Enums.ConfigDevicePostReturn.DoneAndCancelAndStay
    End Function

    ' Web Page Generation - OLD METHODS
    ' ================================================================================================
    Public Function GenPage(ByVal link As String) As String Implements HomeSeerAPI.IPlugInAPI.GenPage
        Dim st As New StringBuilder
        Dim pageheader As String
        Dim pagefooter As String
        Dim rawpage As String = "This is the page body"

        ' Example raw page return

        st.Append("HTTP/1.0 200 OK" & vbCrLf)
        st.Append("Server: HomeSeer" & vbCrLf)
        st.Append("Expires: Sun, 22 Mar 1993 16:18:35 GMT" & vbCrLf)
        st.Append("Content-Type: text/html" & vbCrLf)
        st.Append("Accep-Ranges: bytes" & vbCrLf)
        pageheader = hs.GetPageHeader("Plug-in Configuration", "page title", "<meta http-equiv=""refresh"" content=""3;url=/plugin_config"">", "", False, False).Trim
        pagefooter = hs.GetPageFooter(False).Trim
        st.Append("Content-Length: " & (rawpage.Length + pageheader.Length + pagefooter.Length) & vbCrLf & vbCrLf)
        st.Append(pageheader)
        st.Append(rawpage)
        st.Append(pagefooter)
        Return st.ToString

        'Return "Generated from GenPage in plugin " & IFACE_NAME
    End Function
    Public Function PagePut(ByVal data As String) As String Implements HomeSeerAPI.IPlugInAPI.PagePut
        Return ""
    End Function
    ' ================================================================================================

    ' Web Page Generation - NEW METHODS
    ' ================================================================================================
    Public Function GetPagePlugin(ByVal pageName As String, ByVal user As String, ByVal userRights As Integer, ByVal queryString As String) As String Implements HomeSeerAPI.IPlugInAPI.GetPagePlugin
        'If you have more than one web page, use pageName to route it to the proper GetPagePlugin
        Console.WriteLine("GetPagePlugin pageName: " & pageName)
        ' get the correct page
        Select Case pageName
            Case "Sample Plugin"
                Return (pluginpage.GetPagePlugin(pageName, user, userRights, queryString))
            Case "Test Page"
                Return (pluginTestPage.GetPagePlugin(pageName, user, userRights, queryString))
        End Select
        Return "page not registered"
    End Function

    Public Function PostBackProc(ByVal pageName As String, ByVal data As String, ByVal user As String, ByVal userRights As Integer) As String Implements HomeSeerAPI.IPlugInAPI.PostBackProc
        'If you have more than one web page, use pageName to route it to the proper postBackProc
        Select Case pageName
            Case "Sample Plugin"
                Return pluginpage.postBackProc(pageName, data, user, userRights)
            Case "Test Page"
                Return pluginTestPage.postBackProc(pageName, data, user, userRights)
        End Select

        Return ""
    End Function

    ' ================================================================================================

    Public Sub HSEvent(ByVal EventType As Enums.HSEvent, ByVal parms() As Object) Implements HomeSeerAPI.IPlugInAPI.HSEvent
        Console.WriteLine("HSEvent: " & EventType.ToString)
        Select Case EventType
            Case Enums.HSEvent.VALUE_CHANGE
        End Select
    End Sub



    Public Function InterfaceStatus() As HomeSeerAPI.IPlugInAPI.strInterfaceStatus Implements HomeSeerAPI.IPlugInAPI.InterfaceStatus
        Dim es As New IPlugInAPI.strInterfaceStatus
        es.intStatus = IPlugInAPI.enumInterfaceStatus.OK
        Return es
    End Function

    Public Function PollDevice(ByVal dvref As Integer) As IPlugInAPI.PollResultInfo Implements HomeSeerAPI.IPlugInAPI.PollDevice
        Console.WriteLine("PollDevice")
        Dim ri As IPlugInAPI.PollResultInfo
        ri.Result = IPlugInAPI.enumPollResult.OK
        Return ri
    End Function

    Public Function RaisesGenericCallbacks() As Boolean Implements HomeSeerAPI.IPlugInAPI.RaisesGenericCallbacks
        Return True
    End Function

    Public Sub SetIOMulti(colSend As System.Collections.Generic.List(Of HomeSeerAPI.CAPI.CAPIControl)) Implements HomeSeerAPI.IPlugInAPI.SetIOMulti
        Dim CC As CAPIControl
        For Each CC In colSend
            Console.WriteLine("SetIOMulti set value: " & CC.ControlValue.ToString & "->ref:" & CC.Ref.ToString)

            If CC.ControlType = Enums.CAPIControlType.List_Text_from_List Then
                Try
                    Console.WriteLine("Selected Strings:" & String.Join(",", CC.ControlStringList))
                    Dim dv As Scheduler.Classes.DeviceClass = hs.GetDeviceByRef(CC.Ref)
                    dv.StringSelected(hs) = CC.ControlStringList    ' for testing, we just set the array back to the device to show the selection

                    If dv IsNot Nothing Then
                        For Each st As String In CC.ControlStringList
                            ' get each string selected and handle any control here
                        Next
                    End If
                    hs.SetDeviceString(CC.Ref, String.Join(",", CC.ControlStringList), True)
                Catch ex As Exception
                End Try
            Else
                ' check if our special control button was pressed, the function here is to speak hello
                If CC.ControlValue = 1000 Then
                    ' this is our button from the sample device that executes a special function but does not set any device value
                    hs.WriteLog(IFACE_NAME, "Special control button pressed, value is " & CC.ControlValue.ToString)
                    hs.Speak("Hello")
                Else
                    ' call function here to control the hardware, then update the actual values
                    ' assume hardware has been set and update HS with new values
                    hs.SetDeviceValueByRef(CC.Ref, CC.ControlValue, True)
                    hs.SetDeviceString(CC.Ref, CC.ControlString, True)

                    Dim dv As Scheduler.Classes.DeviceClass = hs.GetDeviceByRef(CC.Ref)
                    Dim DT As DeviceTypeInfo
                    DT = dv.DeviceType_Get(hs)
                    If DT.Device_Type = DeviceTypeInfo.eDeviceType_Thermostat.Operating_Mode Then
                        Dim list() As Integer = dv.AssociatedDevices(hs)
                        If list IsNot Nothing Then
                            Dim ref As Integer = list(0)    ' child device will only have one associated device which is the root
                            Dim child_dv As Scheduler.Classes.DeviceClass = FindThermChildByType(ref, DeviceTypeInfo.eDeviceType_Thermostat.Operating_State)
                            If child_dv IsNot Nothing Then
                                If CC.ControlValue < 3 Then ' ignore AUTO value for status, status can only be Idle,Heat, or Cool
                                    hs.SetDeviceValueByRef(child_dv.Ref(Nothing), CC.ControlValue, True)
                                End If
                            End If
                        End If
                    ElseIf DT.Device_Type = DeviceTypeInfo.eDeviceType_Thermostat.Fan_Mode_Set Then
                        Dim list() As Integer = dv.AssociatedDevices(hs)
                        If list IsNot Nothing Then
                            Dim ref As Integer = list(0)    ' child device will only have one associated device which is the root
                            Dim child_dv As Scheduler.Classes.DeviceClass = FindThermChildByType(ref, DeviceTypeInfo.eDeviceType_Thermostat.Fan_Status)
                            If child_dv IsNot Nothing Then
                                hs.SetDeviceValueByRef(child_dv.Ref(Nothing), CC.ControlValue, True)
                            End If
                        End If
                    End If
                End If

            End If


        Next
    End Sub

    Private Function FindThermChildByType(root_dv As Integer, dev_type As DeviceTypeInfo.eDeviceType_Thermostat) As Scheduler.Classes.DeviceClass

        ' set therm operating mode
        ' get the root device so we can find associated devices
        Dim dv As Scheduler.Classes.DeviceClass = hs.GetDeviceByRef(root_dv)
        Dim list() As Integer
        Dim DT As DeviceTypeInfo

        If dv IsNot Nothing Then
            ' have the root device, get all associated devices
            List = dv.AssociatedDevices(hs)
            For index As Integer = 0 To List.Count - 1
                Dim childref As Integer = List(index)
                Dim child_dv As Scheduler.Classes.DeviceClass = hs.GetDeviceByRef(childref)
                If child_dv IsNot Nothing Then
                    DT = child_dv.DeviceType_Get(Nothing)
                    If DT.Device_Type = dev_type Then
                        Return child_dv
                        Exit For
                    End If
                End If
            Next
        End If
        Return Nothing
    End Function

    Public Sub ShutdownIO() Implements HomeSeerAPI.IPlugInAPI.ShutdownIO
        ' do your shutdown stuff here

        bShutDown = True ' setting this flag will cause the plugin to disconnect immediately from HomeSeer
    End Sub

    Public Function SupportsConfigDevice() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsConfigDevice
        Return True
    End Function

    Public Function SupportsConfigDeviceAll() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsConfigDeviceAll
        Return False
    End Function

    Public Function SupportsAddDevice() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsAddDevice
        Return False
    End Function


#End Region

#Region "Actions Interface"

    Public Function ActionCount() As Integer Implements HomeSeerAPI.IPlugInAPI.ActionCount
        Return 2
    End Function

    Private mvarActionAdvanced As Boolean
    Public Property ActionAdvancedMode As Boolean Implements HomeSeerAPI.IPlugInAPI.ActionAdvancedMode
        Set(ByVal value As Boolean)
            mvarActionAdvanced = value
        End Set
        Get
            Return mvarActionAdvanced
        End Get
    End Property

    Public Function ActionBuildUI(ByVal sUnique As String, ByVal ActInfo As IPlugInAPI.strTrigActInfo) As String Implements HomeSeerAPI.IPlugInAPI.ActionBuildUI
        Dim st As New StringBuilder
        Dim strAction As strAction


        If ValidAct(ActInfo.TANumber) Then

            ' This is a valid action number for the sample plug-in which offers 2 

            If ActInfo.TANumber = 1 Then
                strAction = GetActs(ActInfo, ActInfo.DataIn)
                If strAction.Result AndAlso strAction.WhichAction = eActionType.Weight AndAlso strAction.ActObj IsNot Nothing Then
                    Dim Act1 As MyAction1EvenTon = Nothing
                    Act1 = CType(strAction.ActObj, MyAction1EvenTon)
                    If Act1.SetTo = MyAction1EvenTon.eSetTo.Not_Set Then
                        ' NOTE: You must add sUnique to the name of your control!
                        Dim DL As New clsJQuery.jqDropList("Action1TypeList" & sUnique, "Events", True)
                        DL.AddItem("(Not Set)", "0", True)
                        DL.AddItem("Round Tonnage", "1", False)
                        DL.AddItem("Unrounded Tonnage", "2", False)
                        st.Append("Set Weight Option Mode:" & DL.Build)
                    Else
                        Dim CB1 As New clsJQuery.jqCheckBox("Action1Type" & sUnique, "", "Events", True, True)
                        If Act1.SetTo = MyAction1EvenTon.eSetTo.Rounded Then
                            CB1.checked = True
                            st.Append("Uncheck to revert to Unrounded weights:")
                        Else
                            CB1.checked = False
                            st.Append("Check to change to Rounded weights:")
                        End If
                        st.Append(CB1.Build)
                    End If
                End If
            End If

            If ActInfo.TANumber = 2 Then
                Dim DL2 As New clsJQuery.jqDropList("Act2SubActSelect" & sUnique, "Events", True)
                If Not ValidSubAct(ActInfo.TANumber, ActInfo.SubTANumber) Then
                    DL2.AddItem(" ", "-1", True)
                End If
                If ActInfo.SubTANumber < 3 And ValidSubAct(ActInfo.TANumber, ActInfo.SubTANumber) Then
                    mvarActionAdvanced = True
                End If
                If mvarActionAdvanced Then
                    DL2.AddItem("Action 2 SubAction 1 - Set Voltage to European", "1", CBool(IIf(ActInfo.SubTANumber = 1, True, False)))
                    DL2.AddItem("Action 2 SubAction 2 - Set Voltage to North American", "2", CBool(IIf(ActInfo.SubTANumber = 2, True, False)))
                End If
                DL2.AddItem("Action 2 SubAction 3 - Reset Average Voltage", "3", CBool(IIf(ActInfo.SubTANumber = 3, True, False)))
                If Not ValidSubAct(ActInfo.TANumber, ActInfo.SubTANumber) Then
                    st.Append("Choose a Voltage Sub-Action: " & DL2.Build)
                Else
                    st.Append("Change the Voltage Sub-Action: " & DL2.Build)
                End If
            End If

        Else

            Return "Error, Action number for plug-in " & IFACE_NAME & " was not set."

        End If

        Return st.ToString
    End Function

    Public Function ActionConfigured(ByVal ActInfo As IPlugInAPI.strTrigActInfo) As Boolean Implements HomeSeerAPI.IPlugInAPI.ActionConfigured
        Console.WriteLine("ActionConfigured Called")
        If ValidAct(ActInfo.TANumber) Then
            Return ValidSubAct(ActInfo.TANumber, ActInfo.SubTANumber)
        Else
            Return False
        End If
    End Function

    Public Function ActionReferencesDevice(ByVal ActInfo As IPlugInAPI.strTrigActInfo, ByVal dvRef As Integer) As Boolean Implements HomeSeerAPI.IPlugInAPI.ActionReferencesDevice
        Console.WriteLine("ActionReferencesDevice Called")
        '
        ' Actions in the sample plug-in do not reference devices, but for demonstration purposes we will pretend they do, 
        '   and that ALL actions reference our sample devices.
        '
        If dvRef = MyDevice Then Return True
        Return False
    End Function

    Public Function ActionFormatUI(ByVal ActInfo As IPlugInAPI.strTrigActInfo) As String Implements HomeSeerAPI.IPlugInAPI.ActionFormatUI
        Dim st As New StringBuilder

        If ValidAct(ActInfo.TANumber) Then
            Dim strAction As strAction = Nothing
            If ActInfo.TANumber = 1 Then
                strAction = GetActs(ActInfo, ActInfo.DataIn)
                If strAction.Result AndAlso strAction.WhichAction = eActionType.Weight AndAlso strAction.ActObj IsNot Nothing Then
                    Dim Act1 As MyAction1EvenTon = Nothing
                    Act1 = CType(strAction.ActObj, MyAction1EvenTon)
                    If Act1.SetTo = MyAction1EvenTon.eSetTo.Not_Set Then
                        st.Append("The Weight Options Action has not been configured yet.")
                    Else
                        If Act1.SetTo = MyAction1EvenTon.eSetTo.Rounded Then
                            st.Append("Calculated weights will be rounded.")
                        Else
                            st.Append("Calculated weights will not be rounded.")
                        End If
                    End If
                Else
                    st.Append("Error, " & IFACE_NAME & " Weight option action was not recovered.")
                End If
            End If
            If ActInfo.TANumber = 2 Then
                If Not ValidSubAct(ActInfo.TANumber, ActInfo.SubTANumber) Then
                    st.Append("The voltage options action has not been configured yet.")
                Else
                    Select Case ActInfo.SubTANumber
                        Case 1
                            st.Append("Voltages will be European (220 @ 50Hz)")
                        Case 2
                            st.Append("Voltages will be North American (110 @ 60Hz)")
                        Case 3
                            st.Append("The average voltage calculation will be reset to zero.")
                    End Select
                End If
            End If
        Else
            st.Append("This action for plug-in " & IFACE_NAME & " was not properly set by HomeSeer.")
        End If
        Return st.ToString
    End Function

    Public ReadOnly Property ActionName(ByVal ActionNumber As Integer) As String Implements HomeSeerAPI.IPlugInAPI.ActionName
        Get
            If Not ValidAct(ActionNumber) Then Return ""
            Select Case ActionNumber
                Case 1
                    Return IFACE_NAME & ": Set Weight Option"
                Case 2
                    Return IFACE_NAME & ": Voltage Actions"
            End Select
            Return ""
        End Get
    End Property

    Public Function ActionProcessPostUI(ByVal PostData As Collections.Specialized.NameValueCollection, _
                                        ByVal ActInfoIN As IPlugInAPI.strTrigActInfo) As IPlugInAPI.strMultiReturn _
                                        Implements HomeSeerAPI.IPlugInAPI.ActionProcessPostUI

        Dim Ret As New HomeSeerAPI.IPlugInAPI.strMultiReturn
        Ret.sResult = ""
        ' We cannot be passed info ByRef from HomeSeer, so turn right around and return this same value so that if we want, 
        '   we can exit here by returning 'Ret', all ready to go.  If in this procedure we need to change DataOut or TrigInfo,
        '   we can still do that.
        Ret.DataOut = ActInfoIN.DataIn
        Ret.TrigActInfo = ActInfoIN

        If PostData Is Nothing Then Return Ret
        If PostData.Count < 1 Then Return Ret
        Dim st As New System.Text.StringBuilder
        Dim sKey As String
        Dim sValue As String
        Dim e As EventWebControlInfo

        Dim Act1 As MyAction1EvenTon = Nothing
        Dim Act2 As MyAction2Euro = Nothing
        Dim PlugKey As String = ""

        Try
            ' Look for the Action and SubAction selections because if they changed, then 
            '   GetTrigs will create a new Action object before the other changes are applied.
            For i As Integer = 0 To PostData.Count - 1
                sKey = PostData.GetKey(i)
                hs.WriteLog(IFACE_NAME & "Debug", sKey & "potatoes!")
                sValue = PostData(sKey).Trim
                If sKey Is Nothing Then Continue For
                If String.IsNullOrEmpty(sKey.Trim) Then Continue For
                '       hs.WriteLog(IFACE_NAME & " DEBUG", sKey & "=" & sValue)
                If sKey.Trim = "id" Then
                    e = U_Get_Control_Info(sValue.Trim)
                Else
                    e = U_Get_Control_Info(sKey.Trim)
                End If
                If e.Decoded Then

                    If e.TrigActGroup = enumTAG.Group Or e.TrigActGroup = enumTAG.Trigger Then Continue For

                    If (e.EvRef = ActInfoIN.evRef) Then
                        Select Case e.Name_or_ID
                            Case "Act2SubActSelect"
                                Ret.TrigActInfo.SubTANumber = Convert.ToInt32(Val(sValue.Trim))
                            Case "Action1TypeList"
                                Ret.TrigActInfo.SubTANumber = Convert.ToInt32(Val(sValue.Trim))
                            Case "Action1Type"
                                Select Case sValue.Trim.ToLower
                                    Case "checked"
                                        Ret.TrigActInfo.SubTANumber = 1
                                    Case "unchecked"
                                        Ret.TrigActInfo.SubTANumber = 2
                                End Select
                        End Select
                    End If

                End If
            Next

            ' This uses the event information or the data passed to us to get or create our
            '   action object.
            Dim strAct As strAction
            strAct = GetActs(Ret.TrigActInfo, ActInfoIN.DataIn)
            If strAct.Result = False Then
                ' The action object was not found AND there is not enough info (ActionNumber)
                '   to create a new one, so there is really nothing we can do here!  We will 
                '   wipe out the data since it did not lead to recovery of the action object.
                Ret.DataOut = Nothing
                Ret.sResult = "No action object was created by " & IFACE_NAME & " - not enough information provided."
                Return Ret
            End If


            'Check for a sub-Action change:
            If strAct.WhichAction = eActionType.Voltage AndAlso strAct.ActObj IsNot Nothing Then
                Try
                    Act2 = CType(strAct.ActObj, MyAction2Euro)
                Catch ex As Exception
                    Act2 = Nothing
                End Try
                If Act2 IsNot Nothing Then
                    If ValidSubAct(Ret.TrigActInfo.TANumber, Ret.TrigActInfo.SubTANumber) Then
                        Select Case Ret.TrigActInfo.SubTANumber
                            Case 1
                                Act2.ThisAction = MyAction2Euro.eVAction.SetEuro
                            Case 2
                                Act2.ThisAction = MyAction2Euro.eVAction.SetNorthAmerica
                            Case 3
                                Act2.ThisAction = MyAction2Euro.eVAction.ResetAverage
                        End Select
                    End If
                    If Not SerializeObject(Act2, Ret.DataOut) Then
                        Ret.sResult = IFACE_NAME & " Error, Action type 2 was modified but serialization failed."
                        Return Ret
                    End If
                End If
            ElseIf strAct.WhichAction = eActionType.Weight AndAlso strAct.ActObj IsNot Nothing Then
                Try
                    Act1 = CType(strAct.ActObj, MyAction1EvenTon)
                Catch ex As Exception
                    Act1 = Nothing
                End Try
                If ValidSubAct(Ret.TrigActInfo.TANumber, Ret.TrigActInfo.SubTANumber) Then
                    Select Case Ret.TrigActInfo.SubTANumber
                        Case 1
                            Act1.SetTo = MyAction1EvenTon.eSetTo.Rounded
                        Case 2
                            Act1.SetTo = MyAction1EvenTon.eSetTo.Unrounded
                    End Select
                End If
                If Act1 IsNot Nothing Then
                    If Not SerializeObject(Act1, Ret.DataOut) Then
                        Ret.sResult = IFACE_NAME & " Error, Action type 1 was modified but serialization failed."
                        Return Ret
                    End If
                End If
            End If

        Catch ex As Exception
            Ret.sResult = "ERROR, Exception in Action UI of " & IFACE_NAME & ": " & ex.Message
            Return Ret
        End Try

        ' All OK
        Ret.sResult = ""
        Return Ret


    End Function

    Public Function HandleAction(ByVal ActInfo As IPlugInAPI.strTrigActInfo) As Boolean Implements HomeSeerAPI.IPlugInAPI.HandleAction


        Dim strAction As strAction = Nothing
        If ValidAct(ActInfo.TANumber) Then

            If ActInfo.TANumber = 1 Then
                strAction = GetActs(ActInfo, ActInfo.DataIn)
                If strAction.Result AndAlso strAction.WhichAction = eActionType.Weight AndAlso strAction.ActObj IsNot Nothing Then
                    Dim Act1 As MyAction1EvenTon = Nothing
                    Act1 = CType(strAction.ActObj, MyAction1EvenTon)
                    If Act1.SetTo = MyAction1EvenTon.eSetTo.Not_Set Then
                        Log("Error, Handle Action for Type 1: The Weight Options Action has not been configured yet.", LogType.LOG_TYPE_ERROR)
                        Return False
                    Else
                        Dim obj As Object = Nothing
                        Dim Trig1 As MyTrigger1Ton = Nothing
                        If colTrigs IsNot Nothing AndAlso colTrigs.Count > 0 Then
                            Dim Found As Boolean = False
                            For i As Integer = 0 To colTrigs.Count - 1
                                obj = colTrigs.GetByIndex(i)
                                If obj Is Nothing Then Continue For
                                Try
                                    If TypeOf obj Is MyTrigger1Ton Then
                                        Trig1 = Nothing
                                        Try
                                            Trig1 = CType(obj, MyTrigger1Ton)
                                        Catch ex2 As Exception
                                            Trig1 = Nothing
                                        End Try
                                        If Trig1 IsNot Nothing Then
                                            Found = True
                                            If Act1.SetTo = MyAction1EvenTon.eSetTo.Rounded Then
                                                Trig1.EvenTon = True
                                            Else
                                                Trig1.EvenTon = False
                                            End If
                                        End If
                                    End If
                                Catch ex As Exception
                                End Try
                            Next
                            If Found Then
                                Return True
                            Else
                                Log("Warning - No triggers for type 1 action (weight options) were found or were valid - action cannot be carried out.", LogType.LOG_TYPE_WARNING)
                                Return True
                            End If
                        Else
                            Log("Warning - No triggers for type 1 action (weight options) were found - action cannot be carried out.", LogType.LOG_TYPE_WARNING)
                            Return True
                        End If
                    End If
                Else
                    Log("Error, action type 1 (weight options) provided to Handle Action, but Action ID " & ActInfo.UID.ToString & " could not be recovered.", LogType.LOG_TYPE_ERROR)
                    Return False
                End If
            End If

            If ActInfo.TANumber = 2 Then
                strAction = GetActs(ActInfo, ActInfo.DataIn)
                If strAction.Result AndAlso strAction.WhichAction = eActionType.Voltage AndAlso strAction.ActObj IsNot Nothing Then
                    Dim Act2 As MyAction2Euro = Nothing
                    Act2 = CType(strAction.ActObj, MyAction2Euro)
                    If Not ValidSubAct(ActInfo.TANumber, ActInfo.SubTANumber) Then
                        Log("Error, Handle Action for Type 2: The Voltage Options Action has not been configured yet.", LogType.LOG_TYPE_ERROR)
                        Return False
                    Else
                        Dim obj As Object = Nothing
                        Dim Trig2 As MyTrigger2Shoe = Nothing
                        If colTrigs IsNot Nothing AndAlso colTrigs.Count > 0 Then
                            Dim Found As Boolean = False
                            For i As Integer = 0 To colTrigs.Count - 1
                                obj = colTrigs.GetByIndex(i)
                                If obj Is Nothing Then Continue For
                                Try
                                    If TypeOf obj Is MyTrigger2Shoe Then
                                        Trig2 = Nothing
                                        Try
                                            Trig2 = CType(obj, MyTrigger2Shoe)
                                        Catch ex2 As Exception
                                            Trig2 = Nothing
                                        End Try
                                        If Trig2 IsNot Nothing Then
                                            Select Case Act2.ThisAction
                                                Case MyAction2Euro.eVAction.SetEuro
                                                    Found = True
                                                    Trig2.EuroVoltage = True
                                                Case MyAction2Euro.eVAction.SetNorthAmerica
                                                    Found = True
                                                    Trig2.NorthAMVoltage = True
                                                Case MyAction2Euro.eVAction.ResetAverage
                                                    Found = True
                                                    If Volt Is Nothing Then Volt = New Volt_Demo(False)
                                                    If VoltEuro Is Nothing Then VoltEuro = New Volt_Demo(True)
                                                    Volt.ResetAverage()
                                                    VoltEuro.ResetAverage()

                                            End Select
                                        End If
                                    End If
                                Catch ex As Exception
                                End Try
                            Next
                            If Found Then
                                Return True
                            Else
                                Log("Warning - No triggers for type 2 action (voltage options) were found or were valid - action cannot be carried out.", LogType.LOG_TYPE_WARNING)
                                Return True
                            End If
                        Else
                            Log("Warning - No triggers for type 2 action (voltage options) were found - action cannot be carried out.", LogType.LOG_TYPE_WARNING)
                            Return True
                        End If
                    End If
                Else
                    Log("Error, action type 2 (voltage options) provided to Handle Action, but Action ID " & ActInfo.UID.ToString & " could not be recovered.", LogType.LOG_TYPE_ERROR)
                    Return False
                End If
            End If

            Log("Error, Handle Action was provided an invalid action type.", LogType.LOG_TYPE_ERROR)
            Return False

        Else
            Log("Error, Handle Action was provided an invalid action type.", LogType.LOG_TYPE_ERROR)
            Return False
        End If

    End Function

#End Region

#Region "Trigger Interface"

    ''' <summary>
    ''' Indicates (when True) that the Trigger is in Condition mode - it is for triggers that can also operate as a condition
    '''    or for allowing Conditions to appear when a condition is being added to an event.
    ''' </summary>
    ''' <param name="TrigInfo">The event, group, and trigger info for this particular instance.</param>
    ''' <value></value>
    ''' <returns>The current state of the Condition flag.</returns>
    ''' <remarks></remarks>
    Public Property Condition(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As Boolean Implements HomeSeerAPI.IPlugInAPI.Condition
        Get
            Dim strRET As strTrigger
            Dim Trig1 As MyTrigger1Ton = Nothing
            Dim Trig2 As MyTrigger2Shoe = Nothing
            strRET = GetTrigs(TrigInfo, TrigInfo.DataIn)
            If strRET.WhichTrigger <> eTriggerType.Unknown Then
                If strRET.WhichTrigger = eTriggerType.OneTon Then
                    Return False    ' Trigger 1 cannot have a condition
                ElseIf strRET.WhichTrigger = eTriggerType.TwoVolts Then
                    Trig2 = CType(strRET.TrigObj, MyTrigger2Shoe)
                    If Trig2 IsNot Nothing Then
                        Return Trig2.Condition
                    End If
                End If
            End If
            Return False
        End Get
        Set(ByVal value As Boolean)
            Dim strRET As strTrigger
            Dim Trig1 As MyTrigger1Ton = Nothing
            Dim Trig2 As MyTrigger2Shoe = Nothing
            strRET = GetTrigs(TrigInfo, TrigInfo.DataIn)
            If strRET.WhichTrigger <> eTriggerType.Unknown Then
                If strRET.WhichTrigger = eTriggerType.OneTon Then
                    ' Trigger 1 cannot have a condition
                    Exit Property
                ElseIf strRET.WhichTrigger = eTriggerType.TwoVolts Then
                    Trig2 = CType(strRET.TrigObj, MyTrigger2Shoe)
                    If Trig2 IsNot Nothing Then
                        Trig2.Condition = value
                    End If
                End If
            End If
        End Set
    End Property

    Public ReadOnly Property HasConditions(ByVal TriggerNumber As Integer) As Boolean Implements HomeSeerAPI.IPlugInAPI.HasConditions
        Get
            Select Case TriggerNumber
                Case 1
                    Return False
                Case 2
                    Return True
                Case Else
                    Return False
            End Select
        End Get
    End Property

    Public ReadOnly Property HasTriggers() As Boolean Implements HomeSeerAPI.IPlugInAPI.HasTriggers
        Get
            Return True
        End Get
    End Property

    Public ReadOnly Property TriggerCount As Integer Implements HomeSeerAPI.IPlugInAPI.TriggerCount
        Get
            Return 2
        End Get
    End Property

    Public ReadOnly Property TriggerName(ByVal TriggerNumber As Integer) As String Implements HomeSeerAPI.IPlugInAPI.TriggerName
        Get
            Select Case TriggerNumber
                Case 1
                    Return "Trigger 1 Weighs A Ton"
                Case 2
                    Return "Trigger 2 Voltage for You"
                Case Else
                    Return ""
            End Select
        End Get
    End Property

    Public ReadOnly Property SubTriggerCount(ByVal TriggerNumber As Integer) As Integer Implements HomeSeerAPI.IPlugInAPI.SubTriggerCount
        Get
            If TriggerNumber = 2 Then
                Return 2
            Else
                Return 0
            End If
        End Get
    End Property

    Public ReadOnly Property SubTriggerName(ByVal TriggerNumber As Integer, ByVal SubTriggerNumber As Integer) As String Implements HomeSeerAPI.IPlugInAPI.SubTriggerName
        Get
            If TriggerNumber <> 2 Then Return ""
            Select Case SubTriggerNumber
                Case 1
                    Return "Trig 2 SubTrig 1 - Voltage"
                Case 2
                    Return "Trig 2 SubTrig 2 - Average Voltage"
                Case Else
                    Return ""
            End Select
        End Get
    End Property

    Public Function TriggerBuildUI(ByVal sUnique As String, ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As String Implements HomeSeerAPI.IPlugInAPI.TriggerBuildUI
        Dim st As New StringBuilder
        Dim strTrigger As strTrigger


        If ValidTrig(TrigInfo.TANumber) Then

            ' This is a valid trigger number for the sample plug-in which offers 2 triggers (1 and 2)

            If TrigInfo.TANumber = 1 Then
                strTrigger = GetTrigs(TrigInfo, TrigInfo.DataIn)
                If strTrigger.Result AndAlso strTrigger.WhichTrigger = eTriggerType.OneTon AndAlso strTrigger.TrigObj IsNot Nothing Then
                    Dim Trig1 As MyTrigger1Ton = Nothing
                    Trig1 = CType(strTrigger.TrigObj, MyTrigger1Ton)
                    If Trig1 IsNot Nothing Then
                        If Trig1.Condition Then
                            ' This trigger is not valid for a Condition
                        Else
                            Dim TB As New clsJQuery.jqTextBox("TriggerWeight" & sUnique, "number", Trig1.TriggerWeight.ToString, "Events", 8, True)
                            st.Append("&nbsp;&nbsp;")
                            st.Append("Enter weight to be exceeded to trigger: " & TB.Build)
                        End If
                    End If
                End If
            End If

            If TrigInfo.TANumber = 2 Then
                Dim strRET As strTrigger
                Dim Trig2 As MyTrigger2Shoe = Nothing
                strRET = GetTrigs(TrigInfo, TrigInfo.DataIn)
                If strRET.WhichTrigger = eTriggerType.TwoVolts Then
                    Trig2 = CType(strRET.TrigObj, MyTrigger2Shoe)

                    If TrigInfo.SubTANumber = 1 Then
                        ' Voltage
                        Trig2.SubTrigger2 = False
                        Dim TB1 As New clsJQuery.jqTextBox("TriggerVolt" & sUnique, "number", Trig2.TriggerValue.ToString, "Events", 8, True)
                        st.Append("<br>")
                        If Trig2.Condition Then
                            st.Append("Enter the instantaneous voltage (+/- 5V) for the condition to be true: " & TB1.Build)
                        Else
                            st.Append("Enter the instantaneous voltage (exact) for trigger: " & TB1.Build)
                        End If
                    ElseIf TrigInfo.SubTANumber = 2 Then
                        ' Average Voltage
                        Trig2.SubTrigger2 = True
                        Dim TB1 As New clsJQuery.jqTextBox("TriggerAvgVolt" & sUnique, "number", Trig2.TriggerValue.ToString, "Events", 8, True)
                        st.Append("<br>")
                        If Trig2.Condition Then
                            st.Append("Enter the average voltage (+/- 10V) for the condition to be true: " & TB1.Build)
                        Else
                            st.Append("Enter the average voltage (exact) for trigger: " & TB1.Build)
                        End If

                    End If
                End If

            End If


        Else

            Return "Error, Trigger number for plug-in " & IFACE_NAME & " was not set."

        End If

        Return st.ToString

    End Function

    Public ReadOnly Property TriggerConfigured(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As Boolean Implements HomeSeerAPI.IPlugInAPI.TriggerConfigured
        Get
            Dim strRET As strTrigger
            Dim Trig1 As MyTrigger1Ton = Nothing
            Dim Trig2 As MyTrigger2Shoe = Nothing
            strRET = GetTrigs(TrigInfo, TrigInfo.DataIn)
            If strRET.WhichTrigger <> eTriggerType.Unknown Then
                If strRET.WhichTrigger = eTriggerType.OneTon Then
                    Try
                        Trig1 = CType(strRET.TrigObj, MyTrigger1Ton)
                    Catch ex As Exception
                        Trig1 = Nothing
                    End Try
                    If Trig1 IsNot Nothing Then
                        If Trig1.TriggerWeight > 0 Then Return True
                    End If
                    Return False
                ElseIf strRET.WhichTrigger = eTriggerType.TwoVolts Then
                    Try
                        Trig2 = CType(strRET.TrigObj, MyTrigger2Shoe)
                    Catch ex As Exception
                        Trig2 = Nothing
                    End Try
                    If Trig2 IsNot Nothing Then
                        If Trig2.TriggerValue > 0 Then Return True
                    End If
                    Return False
                End If
            End If
            Return False
        End Get
    End Property

    Public Function TriggerReferencesDevice(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo, ByVal dvRef As Integer) As Boolean Implements HomeSeerAPI.IPlugInAPI.TriggerReferencesDevice
        '
        ' Triggers in the sample plug-in do not reference devices, but for demonstration purposes we will pretend they do, 
        '   and that ALL triggers reference our sample devices.
        '
        If dvRef = MyDevice Then Return True
        Return False
    End Function

    Public Function TriggerFormatUI(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As String Implements HomeSeerAPI.IPlugInAPI.TriggerFormatUI
        Dim strRET As strTrigger
        Dim Trig1 As MyTrigger1Ton = Nothing
        Dim Trig2 As MyTrigger2Shoe = Nothing
        strRET = GetTrigs(TrigInfo, TrigInfo.DataIn)
        If strRET.WhichTrigger <> eTriggerType.Unknown Then
            If strRET.WhichTrigger = eTriggerType.OneTon Then
                Try
                    Trig1 = CType(strRET.TrigObj, MyTrigger1Ton)
                Catch ex As Exception
                    Trig1 = Nothing
                End Try
                If Trig1 IsNot Nothing Then
                    If Trig1.Condition Then Return ""
                    Return "The weight exceeds " & Trig1.TriggerWeight.ToString & "lbs."
                Else
                    Return "ERROR (A) - Trigger 1 is not properly built yet."
                End If
            ElseIf strRET.WhichTrigger = eTriggerType.TwoVolts Then
                Try
                    Trig2 = CType(strRET.TrigObj, MyTrigger2Shoe)
                Catch ex As Exception
                    Trig2 = Nothing
                End Try
                If Trig2 IsNot Nothing Then
                    If Trig2.SubTrigger2 Then
                        Dim sRet As String = "The average voltage "
                        If Trig2.Condition Then
                            sRet &= "is within 10V of " & Trig2.TriggerValue.ToString & "V"
                            Return sRet
                        Else
                            sRet &= "is " & Trig2.TriggerValue.ToString & "V"
                            Return sRet
                        End If
                    Else
                        Dim sRet As String = "The current voltage "
                        If Trig2.Condition Then
                            sRet &= "is within 5V of " & Trig2.TriggerValue.ToString & "V"
                            Return sRet
                        Else
                            sRet &= "is " & Trig2.TriggerValue.ToString & "V"
                            Return sRet
                        End If
                    End If
                Else
                    Return "ERROR (B) - Trigger 2 is not properly built yet."
                End If
            End If
        End If
        Return "ERROR - The trigger is not properly built yet."
    End Function

    Public Function TriggerProcessPostUI(ByVal PostData As System.Collections.Specialized.NameValueCollection, _
                                         ByVal TrigInfoIn As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As HomeSeerAPI.IPlugInAPI.strMultiReturn Implements HomeSeerAPI.IPlugInAPI.TriggerProcessPostUI

        Dim Ret As New HomeSeerAPI.IPlugInAPI.strMultiReturn
        Ret.sResult = ""
        ' We cannot be passed info ByRef from HomeSeer, so turn right around and return this same value so that if we want, 
        '   we can exit here by returning 'Ret', all ready to go.  If in this procedure we need to change DataOut or TrigInfo,
        '   we can still do that.
        Ret.DataOut = TrigInfoIn.DataIn
        Ret.TrigActInfo = TrigInfoIn

        If PostData Is Nothing Then Return Ret
        If PostData.Count < 1 Then Return Ret
        Dim st As New System.Text.StringBuilder
        Dim sKey As String
        Dim sValue As String
        Dim e As EventWebControlInfo

        Dim Trig1 As MyTrigger1Ton = Nothing
        Dim Trig2 As MyTrigger2Shoe = Nothing
        Dim PlugKey As String = ""

        Try

            ' This uses the event information or the data passed to us to get or create our
            '   trigger object.
            Dim strTrig As strTrigger
            strTrig = GetTrigs(Ret.TrigActInfo, TrigInfoIn.DataIn)
            If strTrig.Result = False Then
                ' The trigger object was not found AND there is not enough info (TriggerNumber)
                '   to create a new one, so there is really nothing we can do here!  We will 
                '   wipe out the data since it did not lead to recovery of the trigger object.
                Ret.DataOut = Nothing
                Ret.sResult = "No trigger object was created by " & IFACE_NAME & " - not enough information provided."
                Return Ret
            End If

            ' Now go through the data to see what specifics about the trigger may have been set.
            For i As Integer = 0 To PostData.Count - 1
                sKey = PostData.GetKey(i)
                sValue = PostData(sKey).Trim
                If sKey Is Nothing Then Continue For
                If String.IsNullOrEmpty(sKey.Trim) Then Continue For
                If sKey.Trim = "id" Then
                    e = U_Get_Control_Info(sValue.Trim)
                Else
                    e = U_Get_Control_Info(sKey.Trim)
                End If
                If e.Decoded Then

                    If e.TrigActGroup = enumTAG.Group Or e.TrigActGroup = enumTAG.Action Then Continue For

                    If (e.EvRef = TrigInfoIn.evRef) Then
                        Select Case e.Name_or_ID
                            'Case "SubtriggerSelect"

                            Case "TriggerWeight"
                                If strTrig.WhichTrigger = eTriggerType.OneTon AndAlso strTrig.TrigObj IsNot Nothing Then
                                    Try
                                        Trig1 = CType(strTrig.TrigObj, MyTrigger1Ton)
                                    Catch ex As Exception
                                        Ret.sResult = IFACE_NAME & " Error, Conversion of object to Trigger 1 failed: " & ex.Message
                                        Return Ret
                                    End Try
                                    If Trig1 IsNot Nothing Then
                                        Trig1.TriggerWeight = Val(sValue.Trim)
                                    End If
                                End If

                            Case "TriggerVolt"
                                If strTrig.WhichTrigger = eTriggerType.TwoVolts AndAlso strTrig.TrigObj IsNot Nothing Then
                                    Try
                                        Trig2 = CType(strTrig.TrigObj, MyTrigger2Shoe)
                                    Catch ex As Exception
                                        Ret.sResult = IFACE_NAME & " Error, Conversion of object to Trigger 2 failed: " & ex.Message
                                        Return Ret
                                    End Try
                                    If Trig2 IsNot Nothing Then
                                        Trig2.TriggerValue = Val(sValue.Trim)
                                    End If
                                End If

                            Case "TriggerAvgVolt"
                                If strTrig.WhichTrigger = eTriggerType.TwoVolts AndAlso strTrig.TrigObj IsNot Nothing Then
                                    Try
                                        Trig2 = CType(strTrig.TrigObj, MyTrigger2Shoe)
                                    Catch ex As Exception
                                        Ret.sResult = IFACE_NAME & " Error, Conversion of object to Trigger 2 failed: " & ex.Message
                                        Return Ret
                                    End Try
                                    If Trig2 IsNot Nothing Then
                                        Trig2.TriggerValue = Val(sValue.Trim)
                                    End If
                                End If

                            Case Else
                                hs.WriteLog(IFACE_NAME & " Warning", "MyPostData got unhandled key/value of " & e.Name_or_ID & "=" & sValue)
                        End Select
                    End If

                End If
            Next


            'Check for a sub-Trigger change:
            If strTrig.WhichTrigger = eTriggerType.TwoVolts AndAlso strTrig.TrigObj IsNot Nothing Then
                Try
                    Trig2 = CType(strTrig.TrigObj, MyTrigger2Shoe)
                Catch ex As Exception
                    Trig2 = Nothing
                End Try
                If Trig2 IsNot Nothing Then
                    If ValidSubTrig(Ret.TrigActInfo.TANumber, Ret.TrigActInfo.SubTANumber) Then
                        If Ret.TrigActInfo.SubTANumber = 2 Then
                            Trig2.SubTrigger2 = True
                        End If
                    End If
                    If Not SerializeObject(Trig2, Ret.DataOut) Then
                        Ret.sResult = IFACE_NAME & " Error, Trigger type 2 was modified but serialization failed."
                        Return Ret
                    End If
                End If
            ElseIf strTrig.WhichTrigger = eTriggerType.OneTon AndAlso strTrig.TrigObj IsNot Nothing Then
                Try
                    Trig1 = CType(strTrig.TrigObj, MyTrigger1Ton)
                Catch ex As Exception
                    Trig1 = Nothing
                End Try
                If Trig1 IsNot Nothing Then
                    If Not SerializeObject(Trig1, Ret.DataOut) Then
                        Ret.sResult = IFACE_NAME & " Error, Trigger type 1 was modified but serialization failed."
                        Return Ret
                    End If
                End If
            End If

        Catch ex As Exception
            Ret.sResult = "ERROR, Exception in Trigger UI of " & IFACE_NAME & ": " & ex.Message
            Return Ret
        End Try

        ' All OK
        Ret.sResult = ""
        Return Ret

    End Function

    Public Function TriggerTrue(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As Boolean Implements IPlugInAPI.TriggerTrue
        ' 
        ' Since plug-ins tell HomeSeer when a trigger is true via TriggerFire, this procedure is called just to check
        '   conditions.
        '
        Dim strRET As strTrigger
        Dim Trig1 As MyTrigger1Ton = Nothing
        Dim Trig2 As MyTrigger2Shoe = Nothing
        Dim GotIt As Boolean = False
        Dim WhichOne As eTriggerType = eTriggerType.Unknown


        Dim Cur As Double
        Dim TV As Double

        If ValidTrigInfo(TrigInfo) Then
            strRET = TriggerFromInfo(TrigInfo)
            If strRET.WhichTrigger <> eTriggerType.Unknown AndAlso strRET.Result = True Then
                If strRET.WhichTrigger = eTriggerType.OneTon Then
                    ' Trigger type 1 does not support any conditions, so this
                    '   should not even be here and can never be true.
                    Return False
                ElseIf strRET.WhichTrigger = eTriggerType.TwoVolts Then
                    Trig2 = strRET.TrigObj
                    If Trig2 IsNot Nothing Then
                        If Trig2.Condition Then
                            If Trig2.SubTrigger2 Then
                                'Average voltage +/- 10
                                If Trig2.EuroVoltage Then
                                    Cur = VoltEuro.AverageVoltage
                                    TV = Trig2.TriggerValue
                                    If (Cur >= (TV - 10)) And (Cur <= (TV + 10)) Then
                                        Return True
                                    Else
                                        Return False
                                    End If
                                Else
                                    Cur = Volt.AverageVoltage
                                    TV = Trig2.TriggerValue
                                    If (Cur >= (TV - 10)) And (Cur <= (TV + 10)) Then
                                        Return True
                                    Else
                                        Return False
                                    End If
                                End If
                            Else
                                ' Voltage +/- 5
                                If Trig2.EuroVoltage Then
                                    Cur = VoltEuro.Voltage
                                    TV = Trig2.TriggerValue
                                    If (Cur >= (TV - 5)) And (Cur <= (TV + 5)) Then
                                        Return True
                                    Else
                                        Return False
                                    End If
                                Else
                                    Cur = Volt.Voltage
                                    TV = Trig2.TriggerValue
                                    If (Cur >= (TV - 5)) And (Cur <= (TV + 5)) Then
                                        Return True
                                    Else
                                        Return False
                                    End If
                                End If
                            End If
                        Else
                            Return False
                        End If
                    End If
                End If
            End If
        End If

        If ((Not GotIt) Or (WhichOne = eTriggerType.Unknown)) Then
            If TrigInfo.DataIn IsNot Nothing AndAlso TrigInfo.DataIn.Length > 20 Then
                ' Try from the data.
                strRET = TriggerFromData(TrigInfo.DataIn)
                If strRET.WhichTrigger <> eTriggerType.Unknown AndAlso strRET.Result = True Then
                    If strRET.WhichTrigger = eTriggerType.OneTon Then
                        ' Trigger type 1 does not support any conditions, so this
                        '   should not even be here and can never be true.
                        Return False
                    ElseIf strRET.WhichTrigger = eTriggerType.TwoVolts Then
                        Trig2 = strRET.TrigObj
                        If Trig2 IsNot Nothing Then
                            If Trig2.Condition Then
                                If Trig2.SubTrigger2 Then
                                    'Average voltage +/- 10
                                    If Trig2.EuroVoltage Then
                                        Cur = VoltEuro.AverageVoltage
                                        TV = Trig2.TriggerValue
                                        If (Cur >= (TV - 10)) And (Cur <= (TV + 10)) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    Else
                                        Cur = Volt.AverageVoltage
                                        TV = Trig2.TriggerValue
                                        If (Cur >= (TV - 10)) And (Cur <= (TV + 10)) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                Else
                                    ' Voltage +/- 5
                                    If Trig2.EuroVoltage Then
                                        Cur = VoltEuro.Voltage
                                        TV = Trig2.TriggerValue
                                        If (Cur >= (TV - 5)) And (Cur <= (TV + 5)) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    Else
                                        Cur = Volt.Voltage
                                        TV = Trig2.TriggerValue
                                        If (Cur >= (TV - 5)) And (Cur <= (TV + 5)) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                End If
                            Else
                                Return False
                            End If
                        End If
                    End If
                End If
            End If
        End If

        Return False

    End Function

#End Region

    Private Function GetTrigs(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo, ByRef DataIn() As Byte) As strTrigger
        Dim strRET As strTrigger
        Dim Trig1 As MyTrigger1Ton = Nothing
        Dim Trig2 As MyTrigger2Shoe = Nothing

        Dim GotIt As Boolean = False
        Dim WhichOne As eTriggerType = eTriggerType.Unknown

        ' ------------------------------------------------------------------------------------------------------------------
        ' ------------------------------------------------------------------------------------------------------------------
        '
        '  This procedure recovers or creates a trigger object.  It will first try to find the trigger existing in our
        '   own collection of trigger objects (colTrig) using the trigger info provided by HomeSeer.  If it cannot find
        '   the trigger object, then it may just be that since HomeSeer started, that trigger object has not been 
        '   referenced so that we could add it to our collection; in this case we will create the object from the data
        '   passed to us from HomeSeer.   If that fails, and we at least have a valid Trigger number, then we will create
        '   a new trigger object and return it.
        '
        ' ------------------------------------------------------------------------------------------------------------------
        ' ------------------------------------------------------------------------------------------------------------------


        If ValidTrigInfo(TrigInfo) Then
            strRET = TriggerFromInfo(TrigInfo)
            If strRET.WhichTrigger <> eTriggerType.Unknown AndAlso strRET.Result = True Then
                If strRET.WhichTrigger = eTriggerType.OneTon Then
                    Trig1 = strRET.TrigObj
                    If Trig1 IsNot Nothing Then
                        strRET = New strTrigger
                        strRET.TrigObj = Trig1
                        strRET.WhichTrigger = eTriggerType.OneTon
                        strRET.Result = True
                        Return strRET
                    End If
                ElseIf strRET.WhichTrigger = eTriggerType.TwoVolts Then
                    Trig2 = strRET.TrigObj
                    If Trig2 IsNot Nothing Then
                        Trig2.SubTrigger2 = IIf(TrigInfo.SubTANumber = 2, True, False)
                        strRET = New strTrigger
                        strRET.TrigObj = Trig2
                        strRET.WhichTrigger = eTriggerType.TwoVolts
                        strRET.Result = True
                        Return strRET
                    End If
                End If
            End If
        End If

        If ((Not GotIt) Or (WhichOne = eTriggerType.Unknown)) Then
            If DataIn IsNot Nothing AndAlso DataIn.Length > 20 Then
                ' Try from the data.
                strRET = TriggerFromData(DataIn)
                If strRET.WhichTrigger <> eTriggerType.Unknown AndAlso strRET.Result = True Then
                    If strRET.WhichTrigger = eTriggerType.OneTon Then
                        Trig1 = strRET.TrigObj
                        If Trig1 IsNot Nothing Then
                            If Trig1 IsNot Nothing Then
                                strRET = New strTrigger
                                strRET.TrigObj = Trig1
                                strRET.WhichTrigger = eTriggerType.OneTon
                                strRET.Result = True
                                Return strRET
                            End If
                        End If
                    ElseIf strRET.WhichTrigger = eTriggerType.TwoVolts Then
                        Trig2 = strRET.TrigObj
                        If Trig2 IsNot Nothing Then
                            Trig2.SubTrigger2 = IIf(TrigInfo.SubTANumber = 2, True, False)
                            strRET = New strTrigger
                            strRET.TrigObj = Trig2
                            strRET.WhichTrigger = eTriggerType.TwoVolts
                            strRET.Result = True
                            Return strRET
                        End If
                    End If
                End If
            End If
        End If

        If ((Not GotIt) Or (WhichOne = eTriggerType.Unknown)) Then
            If ValidTrigInfo(TrigInfo) Then
                Dim PlugKey As String = ""
                PlugKey = "K" & TrigInfo.UID.ToString
                Select Case TrigInfo.TANumber
                    Case 1
                        Trig1 = New MyTrigger1Ton
                        Trig1.TriggerUID = TrigInfo.UID
                        Try
                            Add_Update_Trigger(Trig1)
                        Catch ex As Exception
                            strRET = New strTrigger
                            strRET.TrigObj = Trig1
                            strRET.WhichTrigger = eTriggerType.OneTon
                            strRET.Result = False
                            Return strRET
                        End Try
                        strRET = New strTrigger
                        strRET.TrigObj = Trig1
                        strRET.WhichTrigger = eTriggerType.OneTon
                        strRET.Result = True
                        Return strRET
                    Case 2
                        Trig2 = New MyTrigger2Shoe
                        Trig2.TriggerUID = TrigInfo.UID
                        If TrigInfo.SubTANumber = 2 Then
                            Trig2.SubTrigger2 = True
                        Else
                            Trig2.SubTrigger2 = False
                        End If
                        Try
                            Add_Update_Trigger(Trig2)
                        Catch ex As Exception
                            strRET = New strTrigger
                            strRET.TrigObj = Trig2
                            strRET.WhichTrigger = eTriggerType.TwoVolts
                            strRET.Result = False
                            Return strRET
                        End Try
                        strRET = New strTrigger
                        strRET.TrigObj = Trig2
                        strRET.WhichTrigger = eTriggerType.TwoVolts
                        strRET.Result = True
                        Return strRET
                End Select
                strRET = New strTrigger
                strRET.TrigObj = Nothing
                strRET.WhichTrigger = eTriggerType.Unknown
                strRET.Result = False
                Return strRET
            ElseIf ValidTrig(TrigInfo.TANumber) Then
                Select Case TrigInfo.TANumber
                    Case 1
                        Trig1 = New MyTrigger1Ton
                        Trig1.TriggerUID = TrigInfo.UID
                        Try
                            Add_Update_Trigger(Trig1)
                        Catch ex As Exception
                        End Try
                        strRET = New strTrigger
                        strRET.TrigObj = Trig1
                        strRET.WhichTrigger = eTriggerType.OneTon
                        strRET.Result = True
                        Return strRET
                    Case 2
                        Trig2 = New MyTrigger2Shoe
                        Trig2.TriggerUID = TrigInfo.UID
                        Try
                            Add_Update_Trigger(Trig2)
                        Catch ex As Exception
                        End Try
                        If TrigInfo.SubTANumber = 2 Then
                            Trig2.SubTrigger2 = True
                        Else
                            Trig2.SubTrigger2 = False
                        End If
                        strRET = New strTrigger
                        strRET.TrigObj = Trig2
                        strRET.WhichTrigger = eTriggerType.TwoVolts
                        strRET.Result = True
                        Return strRET
                End Select
                strRET = New strTrigger
                strRET.TrigObj = Nothing
                strRET.WhichTrigger = eTriggerType.Unknown
                strRET.Result = False
                Return strRET
            End If
        End If


        strRET = New strTrigger
        strRET.TrigObj = Nothing
        strRET.WhichTrigger = eTriggerType.Unknown
        strRET.Result = False
        Return strRET

    End Function

    Private Function GetActs(ByVal ActInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo, ByRef DataIn() As Byte) As strAction
        Dim strRET As strAction
        Dim Act1 As MyAction1EvenTon = Nothing
        Dim Act2 As MyAction2Euro = Nothing

        Dim GotIt As Boolean = False
        Dim WhichOne As eActionType = eActionType.Unknown

        ' ------------------------------------------------------------------------------------------------------------------
        ' ------------------------------------------------------------------------------------------------------------------
        '
        '  This procedure recovers or creates an action object.  It will first try to find the action existing in our
        '   own collection of action objects (colAct) using the action info provided by HomeSeer.  If it cannot find
        '   the action object, then it may just be that since HomeSeer started, that action object has not been 
        '   referenced so that we could add it to our collection; in this case we will create the object from the data
        '   passed to us from HomeSeer.   If that fails, and we at least have a valid Action number, then we will create
        '   a new action object and return it.
        '
        ' ------------------------------------------------------------------------------------------------------------------
        ' ------------------------------------------------------------------------------------------------------------------


        If ValidActInfo(ActInfo) Then
            strRET = ActionFromInfo(ActInfo)
            If strRET.WhichAction <> eActionType.Unknown AndAlso strRET.Result = True Then
                If strRET.WhichAction = eActionType.Weight Then
                    Act1 = strRET.ActObj
                    If Act1 IsNot Nothing Then
                        strRET = New strAction
                        strRET.ActObj = Act1
                        strRET.WhichAction = eActionType.Weight
                        strRET.Result = True
                        Return strRET
                    End If
                ElseIf strRET.WhichAction = eActionType.Voltage Then
                    Act2 = strRET.ActObj
                    If Act2 IsNot Nothing Then
                        strRET = New strAction
                        strRET.ActObj = Act2
                        strRET.WhichAction = eActionType.Voltage
                        strRET.Result = True
                        Return strRET
                    End If
                End If
            End If
        End If

        If ((Not GotIt) Or (WhichOne = eActionType.Unknown)) Then
            If DataIn IsNot Nothing AndAlso DataIn.Length > 20 Then
                ' Try from the data.
                strRET = ActionFromData(DataIn)
                If strRET.WhichAction <> eActionType.Unknown AndAlso strRET.Result = True Then
                    If strRET.WhichAction = eActionType.Weight Then
                        Act1 = strRET.ActObj
                        If Act1 IsNot Nothing Then
                            If Act1 IsNot Nothing Then
                                strRET = New strAction
                                strRET.ActObj = Act1
                                strRET.WhichAction = eActionType.Weight
                                strRET.Result = True
                                Return strRET
                            End If
                        End If
                    ElseIf strRET.WhichAction = eActionType.Voltage Then
                        Act2 = strRET.ActObj
                        If Act2 IsNot Nothing Then
                            strRET = New strAction
                            strRET.ActObj = Act2
                            strRET.WhichAction = eActionType.Voltage
                            strRET.Result = True
                            Return strRET
                        End If
                    End If
                End If
            End If
        End If

        If ((Not GotIt) Or (WhichOne = eActionType.Unknown)) Then
            If ValidActInfo(ActInfo) Then
                Dim PlugKey As String = ""
                PlugKey = "K" & ActInfo.UID.ToString
                Select Case ActInfo.TANumber
                    Case 1
                        Act1 = New MyAction1EvenTon
                        Act1.ActionUID = ActInfo.UID
                        Try
                            Add_Update_Action(Act1)
                        Catch ex As Exception
                            strRET = New strAction
                            strRET.ActObj = Act1
                            strRET.WhichAction = eActionType.Weight
                            strRET.Result = False
                            Return strRET
                        End Try
                        strRET = New strAction
                        strRET.ActObj = Act1
                        strRET.WhichAction = eActionType.Weight
                        strRET.Result = True
                        Return strRET
                    Case 2
                        Act2 = New MyAction2Euro
                        Act2.ActionUID = ActInfo.UID
                        If ActInfo.SubTANumber = 1 Then
                            Act2.ThisAction = MyAction2Euro.eVAction.SetEuro
                        ElseIf ActInfo.SubTANumber = 2 Then
                            Act2.ThisAction = MyAction2Euro.eVAction.SetNorthAmerica
                        ElseIf ActInfo.SubTANumber = 3 Then
                            Act2.ThisAction = MyAction2Euro.eVAction.ResetAverage
                        End If
                        Try
                            Add_Update_Action(Act2)
                        Catch ex As Exception
                            strRET = New strAction
                            strRET.ActObj = Act2
                            strRET.WhichAction = eActionType.Voltage
                            strRET.Result = False
                            Return strRET
                        End Try
                        strRET = New strAction
                        strRET.ActObj = Act2
                        strRET.WhichAction = eActionType.Voltage
                        strRET.Result = True
                        Return strRET
                End Select
                strRET = New strAction
                strRET.ActObj = Nothing
                strRET.WhichAction = eActionType.Unknown
                strRET.Result = False
                Return strRET
            ElseIf ValidAct(ActInfo.TANumber) Then
                Select Case ActInfo.TANumber
                    Case 1
                        Act1 = New MyAction1EvenTon
                        Act1.ActionUID = ActInfo.UID
                        Try
                            Add_Update_Action(Act1)
                        Catch ex As Exception
                        End Try
                        strRET = New strAction
                        strRET.ActObj = Act1
                        strRET.WhichAction = eActionType.Weight
                        strRET.Result = True
                        Return strRET
                    Case 2
                        Act2 = New MyAction2Euro
                        Act2.ActionUID = ActInfo.UID
                        Try
                            Add_Update_Action(Act2)
                        Catch ex As Exception
                        End Try
                        If ActInfo.SubTANumber = 1 Then
                            Act2.ThisAction = MyAction2Euro.eVAction.SetEuro
                        ElseIf ActInfo.SubTANumber = 2 Then
                            Act2.ThisAction = MyAction2Euro.eVAction.SetNorthAmerica
                        ElseIf ActInfo.SubTANumber = 3 Then
                            Act2.ThisAction = MyAction2Euro.eVAction.ResetAverage
                        End If
                        strRET = New strAction
                        strRET.ActObj = Act2
                        strRET.WhichAction = eActionType.Voltage
                        strRET.Result = True
                        Return strRET
                End Select
                strRET = New strAction
                strRET.ActObj = Nothing
                strRET.WhichAction = eActionType.Unknown
                strRET.Result = False
                Return strRET
            End If
        End If


        strRET = New strAction
        strRET.ActObj = Nothing
        strRET.WhichAction = eActionType.Unknown
        strRET.Result = False
        Return strRET

    End Function

    Public Enum enumTAG
        Unknown = 0
        Trigger = 1
        Action = 2
        Group = 3
    End Enum

    Public Structure EventWebControlInfo
        Public Decoded As Boolean
        Public EventTriggerGroupID As Integer
        Public GroupID As Integer
        Public EvRef As Integer
        Public TriggerORActionID As Integer
        Public Name_or_ID As String
        Public Additional As String
        Public TrigActGroup As enumTAG
    End Structure

    Friend Shared Function U_Get_Control_Info(ByVal sIN As String) As EventWebControlInfo
        Dim e As New EventWebControlInfo
        e.EventTriggerGroupID = -1
        e.GroupID = -1
        e.EvRef = -1
        e.Name_or_ID = ""
        e.TriggerORActionID = -1
        e.Decoded = False
        e.Additional = ""
        e.TrigActGroup = enumTAG.Unknown

        If sIN Is Nothing Then Return e
        If String.IsNullOrEmpty(sIN.Trim) Then Return e
        If Not sIN.Contains("_") Then Return e
        Dim s() As String
        Dim ch(0) As String
        ch(0) = "_"
        s = sIN.Split(ch, StringSplitOptions.None)
        If s Is Nothing Then Return e
        If s.Length < 1 Then Return e
        If s.Length = 1 Then
            e.Name_or_ID = s(0).Trim
            e.Decoded = True
            Return e
        End If
        Dim sTemp As String
        For i As Integer = 0 To s.Length - 1
            If s(i) Is Nothing Then Continue For
            If String.IsNullOrEmpty(s(i).Trim) Then Continue For
            If i = 0 Then
                e.Name_or_ID = s(0).Trim
            Else
                If s(i).Trim = "ID" Then Continue For
                If s(i).Trim.StartsWith("G") Then
                    sTemp = s(i).Substring(1).Trim
                    If IsNumeric(sTemp) Then
                        e.EventTriggerGroupID = Convert.ToInt32(Val(sTemp))
                        e.TrigActGroup = enumTAG.Trigger
                    End If
                ElseIf s(i).Trim.StartsWith("L") Then
                    sTemp = s(i).Substring(1).Trim
                    If IsNumeric(sTemp) Then
                        e.GroupID = Convert.ToInt32(Val(sTemp))
                        e.TrigActGroup = enumTAG.Group
                    End If
                ElseIf s(i).Trim.StartsWith("T") Then
                    sTemp = s(i).Substring(1).Trim
                    If IsNumeric(sTemp) Then
                        e.TriggerORActionID = Convert.ToInt32(Val(sTemp))
                        e.TrigActGroup = enumTAG.Trigger
                    End If
                ElseIf s(i).Trim.StartsWith("A") Then
                    sTemp = s(i).Substring(1).Trim
                    If IsNumeric(sTemp) Then
                        e.TriggerORActionID = Convert.ToInt32(Val(sTemp))
                        e.TrigActGroup = enumTAG.Action
                    End If
                Else
                    If IsNumeric(s(i).Trim) Then
                        e.EvRef = Convert.ToInt32(Val(s(i).Trim))
                    Else
                        If String.IsNullOrEmpty(e.Additional) Then
                            e.Additional = s(i).Trim
                        Else
                            e.Additional &= "_" & s(i).Trim
                        End If
                    End If
                End If
            End If
        Next
        e.Decoded = True
        Return e
    End Function
    Friend Function ValidTrigInfo(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As Boolean
        If TrigInfo.evRef > 0 Then
        Else
            Return False
        End If
        If TrigInfo.TANumber > 0 AndAlso TrigInfo.TANumber < 3 Then
            If TrigInfo.TANumber = 1 Then Return True
            If TrigInfo.SubTANumber > 0 AndAlso TrigInfo.SubTANumber < 3 Then Return True
        End If
        Return False
    End Function
    Friend Function ValidActInfo(ByVal ActInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As Boolean
        If ActInfo.evRef > 0 Then
        Else
            Return False
        End If
        If ActInfo.TANumber > 0 AndAlso ActInfo.TANumber < 3 Then
            If ActInfo.TANumber = 1 Then Return True
            If ActInfo.SubTANumber > 0 AndAlso ActInfo.SubTANumber < 4 Then Return True
        End If
        Return False
    End Function
    Friend Function ValidTrig(ByVal TrigIn As Integer) As Boolean
        If TrigIn > 0 AndAlso TrigIn < 3 Then Return True
        Return False
    End Function
    Friend Function ValidAct(ByVal ActIn As Integer) As Boolean
        If ActIn > 0 AndAlso ActIn < 3 Then Return True
        Return False
    End Function
    Friend Function ValidSubTrig(ByVal TrigIn As Integer, ByVal SubTrigIn As Integer) As Boolean
        If TrigIn > 0 AndAlso TrigIn < 3 Then
            If TrigIn = 1 Then Return True
            If SubTrigIn > 0 AndAlso SubTrigIn < 3 Then Return True
        End If
        Return False
    End Function
    Friend Function ValidSubAct(ByVal ActIn As Integer, ByVal SubActIn As Integer) As Boolean
        If ActIn > 0 AndAlso ActIn < 3 Then
            If ActIn = 1 Then
                If SubActIn > 0 AndAlso SubActIn < 3 Then Return True
                Return False
            End If
            If SubActIn > 0 AndAlso SubActIn < 4 Then Return True
        End If
        Return False
    End Function
    Public Sub New()
        MyBase.New()

        ' Create a thread-safe collection by using the .Synchronized wrapper.
        colTrigs_Sync = New System.Collections.SortedList
        colTrigs = System.Collections.SortedList.Synchronized(colTrigs_Sync)

        colActs_Sync = New System.Collections.SortedList
        colActs = System.Collections.SortedList.Synchronized(colActs_Sync)
    End Sub


    ' called if speak proxy is installed
    Public Sub SpeakIn(device As Integer, txt As String, w As Boolean, host As String) Implements HomeSeerAPI.IPlugInAPI.SpeakIn
        Console.WriteLine("Speaking from HomeSeer, txt: " & txt)
        ' speak back
        hs.SpeakProxy(device, txt & " the plugin added this", w, host)
    End Sub

    ' save an image file to HS, images can only be saved in a subdir of html\images so a subdir must be given
    ' save an image object to HS
    Private Sub SaveImageFileToHS(src_filename As String, des_filename As String)
        Dim im As Drawing.Image = Drawing.Image.FromFile(src_filename)
        hs.WriteHTMLImage(im, des_filename, True)
    End Sub

    ' save a file as an array of bytes to HS
    Private Sub SaveFileToHS(src_filename As String, des_filename As String)
        Dim bytes() As Byte = System.IO.File.ReadAllBytes(src_filename)
        If bytes IsNot Nothing Then
            hs.WriteHTMLImageFile(bytes, des_filename, True)
        End If
    End Sub
End Class

