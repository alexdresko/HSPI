Imports System.Text
Imports System.IO
Imports System.Threading
Imports System.Web
Imports System
Imports HomeSeerAPI
Imports Scheduler

Public Class plugin
    Dim sConfigPage As String = "Sample_Config"
    Dim sStatusPage As String = "Sample_Status"
    Dim ConfigPage As New web_config(sConfigPage)
    Dim StatusPage As New web_status(sStatusPage)
    Dim WebPage As Object

    Dim actions As New hsCollection
    Dim action As New action
    Dim triggers As New hsCollection
    Dim trigger As New trigger

    Dim Commands As New hsCollection

    Const Pagename = "Events"

#Region "Action/Trigger/Device Processes"
#Region "Device Interface"

    Public Function ConfigDevice(ref As Integer, user As String, userRights As Integer, newDevice As Boolean) As String
        Dim dv As Scheduler.Classes.DeviceClass = Nothing
        Dim stb As New StringBuilder
        Dim ddHouseCode As New clsJQuery.jqDropList("HouseCode", "", False)
        Dim ddUnitCode As New clsJQuery.jqDropList("DeviceCode", "", False)
        Dim bSave As New clsJQuery.jqButton("Save", "Done", "DeviceUtility", True)
        Dim HouseCode As String = ""
        Dim Devicecode As String = ""
        Dim Sample As SampleClass

        dv = hs.GetDeviceByRef(ref)

        Dim PED As clsPlugExtraData = dv.PlugExtraData_Get(hs)

        Sample = PEDGet(PED, "Sample")

        If Sample Is Nothing Then
            ' Set the defaults
            Sample = New SampleClass
            InitHSDevice(dv)
            Sample.Housecode = "A"
            Sample.DeviceCode = "1"
            PEDAdd(PED, "Sample", Sample)
            dv.PlugExtraData_Set(hs) = PED
        End If

        HouseCode = Sample.Housecode
        Devicecode = Sample.DeviceCode

        For Each l In "ABCDEFGHIJKLMNOP"
            ddHouseCode.AddItem(l, l, l = HouseCode)
        Next

        For i = 1 To 16
            ddUnitCode.AddItem(i.ToString, i.ToString, i.ToString = Devicecode)
        Next

        Try
            stb.Append("<form id='frmSample' name='SampleTab' method='Post'>")
            stb.Append(" <table border='0' cellpadding='0' cellspacing='0' width='610'>")
            stb.Append("  <tr><td colspan='4' align='Center' style='font-size:10pt; height:30px;' nowrap>Select a Housecode and Unitcode that matches one of the devices HomeSeer will be communicating with.</td></tr>")
            stb.Append("  <tr>")
            stb.Append("   <td nowrap class='tablecolumn' align='center' width='70'>House<br>Code</td>")
            stb.Append("   <td nowrap class='tablecolumn' align='center' width='70'>Unit<br>Code</td>")
            stb.Append("   <td nowrap class='tablecolumn' align='center' width='200'>&nbsp;</td>")
            stb.Append("  </tr>")
            stb.Append("  <tr>")
            stb.Append("   <td class='tablerowodd' align='center'>" & ddHouseCode.Build & "</td>")
            stb.Append("   <td class='tablerowodd' align='center'>" & ddUnitCode.Build & "</td>")
            stb.Append("   <td class='tablerowodd' align='left'>" & bSave.Build & "</td>")
            stb.Append("  </tr>")
            stb.Append(" </table>")
            stb.Append("</form>")
            Return stb.ToString
        Catch
            Return Err.Description
        End Try
    End Function

    Public Function ConfigDevicePost(ref As Integer, data As String, user As String, userRights As Integer) As Enums.ConfigDevicePostReturn
        Dim dv As Scheduler.Classes.DeviceClass = Nothing
        Dim parts As Collections.Specialized.NameValueCollection
        Dim PED As clsPlugExtraData
        Dim ReturnValue As Integer = Enums.ConfigDevicePostReturn.DoneAndCancel
        Dim Sample As SampleClass

        Try
            parts = HttpUtility.ParseQueryString(data)

            dv = hs.GetDeviceByRef(ref)
            PED = dv.PlugExtraData_Get(hs)
            Sample = PEDGet(PED, "Sample")
            If Sample Is Nothing Then
                InitHSDevice(dv)
            End If

            Sample.Housecode = parts("HouseCode")
            Sample.DeviceCode = parts("DeviceCode")

            PED = dv.PlugExtraData_Get(hs)
            PEDAdd(PED, "Sample", Sample)
            dv.PlugExtraData_Set(hs) = PED
            hs.SaveEventsDevices()

            Return ReturnValue
        Catch ex As Exception

        End Try
        Return ReturnValue
    End Function

    Public Sub SetIOMulti(colSend As System.Collections.Generic.List(Of HomeSeerAPI.CAPI.CAPIControl))
        Dim CC As CAPIControl
        Dim dv As Scheduler.Classes.DeviceClass = Nothing
        Dim PED As clsPlugExtraData
        Dim Sample As SampleClass
        Dim Housecode As String = ""
        Dim Devicecode As String = ""

        For Each CC In colSend
            dv = hs.GetDeviceByRef(CC.Ref)
            PED = dv.PlugExtraData_Get(hs)
            Sample = PEDGet(PED, "Sample")

            If Sample IsNot Nothing Then
                Housecode = Sample.Housecode
                Devicecode = Sample.DeviceCode
            End If
            SendCommand(Housecode, Devicecode, CC.ControlValue)
        Next
    End Sub

#End Region

#Region "Action Properties"

    Sub SetActions()
        Dim o As Object = Nothing
        If actions.Count = 0 Then
            actions.Add(o, "Send Command")
        End If
    End Sub

    Function ActionCount() As Integer
        SetActions()
        Return actions.Count
    End Function

    ReadOnly Property ActionName(ByVal ActionNumber As Integer) As String
        Get
            SetActions()
            If ActionNumber > 0 AndAlso ActionNumber <= actions.Count Then
                Return IFACE_NAME & ": " & actions.Keys(ActionNumber - 1)
            Else
                Return ""
            End If
        End Get
    End Property

#End Region

#Region "Trigger Proerties"

    Sub SetTriggers()
        Dim o As Object = Nothing
        If triggers.Count = 0 Then
            triggers.Add(o, "Recieve Command")
        End If
    End Sub

    Public ReadOnly Property HasTriggers() As Boolean
        Get
            SetTriggers()
            Return IIf(triggers.Count > 0, True, False)
        End Get
    End Property

    Public Function TriggerCount() As Integer
        SetTriggers()
        Return triggers.Count
    End Function

    Public ReadOnly Property SubTriggerCount(ByVal TriggerNumber As Integer) As Integer
        Get
            Dim trigger As trigger
            If ValidTrig(TriggerNumber) Then
                trigger = triggers(TriggerNumber)
                If Not (trigger Is Nothing) Then
                    Return trigger.Count
                Else
                    Return 0
                End If
            Else
                Return 0
            End If
        End Get
    End Property

    Public ReadOnly Property TriggerName(ByVal TriggerNumber As Integer) As String
        Get
            If Not ValidTrig(TriggerNumber) Then
                Return ""
            Else
                Return IFACE_NAME & ": " & triggers.Keys(TriggerNumber - 1)
            End If
        End Get
    End Property

    Public ReadOnly Property SubTriggerName(ByVal TriggerNumber As Integer, ByVal SubTriggerNumber As Integer) As String
        Get
            Dim trigger As trigger
            If ValidSubTrig(TriggerNumber, SubTriggerNumber) Then
                trigger = triggers(TriggerNumber)
                Return IFACE_NAME & ": " & trigger.Keys(SubTriggerNumber - 1)
            Else
                Return ""
            End If
        End Get
    End Property

    Friend Function ValidTrig(ByVal TrigIn As Integer) As Boolean
        SetTriggers()
        If TrigIn > 0 AndAlso TrigIn <= triggers.Count Then
            Return True
        End If
        Return False
    End Function

    Public Function ValidSubTrig(ByVal TrigIn As Integer, ByVal SubTrigIn As Integer) As Boolean
        Dim trigger As trigger = Nothing
        If TrigIn > 0 AndAlso TrigIn <= triggers.Count Then
            trigger = triggers(TrigIn)
            If Not (trigger Is Nothing) Then
                If SubTrigIn > 0 AndAlso SubTrigIn <= trigger.Count Then Return True
            End If
        End If
        Return False
    End Function

#End Region

#Region "Action Interface"

    Public Function HandleAction(ByVal ActInfo As IPlugInAPI.strTrigActInfo) As Boolean
        Dim Housecode As String = ""
        Dim DeviceCode As String = ""
        Dim Command As String = ""
        Dim UID As String
        UID = ActInfo.UID.ToString

        Try
            If Not (ActInfo.DataIn Is Nothing) Then
                DeSerializeObject(ActInfo.DataIn, action)
            Else
                Return False
            End If
            For Each sKey In action.Keys
                Select Case True
                    Case InStr(sKey, "Housecodes_" & UID) > 0
                        Housecode = action(sKey)
                    Case InStr(sKey, "DeviceCodes_" & UID) > 0
                        DeviceCode = action(sKey)
                    Case InStr(sKey, "Commands_" & UID) > 0
                        Command = action(sKey)
                End Select
            Next

            SendCommand(Housecode, DeviceCode, Command)

        Catch ex As Exception
            hs.WriteLog(IFACE_NAME, "Error executing action: " & ex.Message)
        End Try
        Return True
    End Function

    Public Function ActionConfigured(ByVal ActInfo As IPlugInAPI.strTrigActInfo) As Boolean
        Dim Configured As Boolean = False
        Dim sKey As String
        Dim itemsConfigured As Integer = 0
        Dim itemsToConfigure As Integer = 3
        Dim UID As String
        UID = ActInfo.UID.ToString

        If Not (ActInfo.DataIn Is Nothing) Then
            DeSerializeObject(ActInfo.DataIn, action)
            For Each sKey In action.Keys
                Select Case True
                    Case InStr(sKey, "Housecodes_" & UID) > 0 AndAlso action(sKey) <> ""
                        itemsConfigured += 1
                    Case InStr(sKey, "DeviceCodes_" & UID) > 0 AndAlso action(sKey) <> ""
                        itemsConfigured += 1
                    Case InStr(sKey, "Commands_" & UID) > 0 AndAlso action(sKey) <> ""
                        itemsConfigured += 1
                End Select
            Next
            If itemsConfigured = itemsToConfigure Then Configured = True
        End If
        Return Configured
    End Function

    Public Function ActionBuildUI(ByVal sUnique As String, ByVal ActInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As String
        Dim UID As String
        UID = ActInfo.UID.ToString
        Dim stb As New StringBuilder
        Dim Housecode As String = ""
        Dim DeviceCode As String = ""
        Dim Command As String = ""
        Dim dd As New clsJQuery.jqDropList("Housecodes_" & UID & sUnique, Pagename, True)
        Dim dd1 As New clsJQuery.jqDropList("DeviceCodes_" & UID & sUnique, Pagename, True)
        Dim dd2 As New clsJQuery.jqDropList("Commands_" & UID & sUnique, Pagename, True)
        Dim sKey As String


        dd.autoPostBack = True
        dd.AddItem("--Please Select--", "", False)
        dd1.autoPostBack = True
        dd1.AddItem("--Please Select--", "", False)
        dd2.autoPostBack = True
        dd2.AddItem("--Please Select--", "", False)

        If Not (ActInfo.DataIn Is Nothing) Then
            DeSerializeObject(ActInfo.DataIn, action)
        Else 'new event, so clean out the action object
            action = New action
        End If

        For Each sKey In action.Keys
            Select Case True
                Case InStr(sKey, "Housecodes_" & UID) > 0
                    Housecode = action(sKey)
                Case InStr(sKey, "DeviceCodes_" & UID) > 0
                    DeviceCode = action(sKey)
                Case InStr(sKey, "Commands_" & UID) > 0
                    Command = action(sKey)
            End Select
        Next

        For Each C In "ABCDEFGHIJKLMNOP"
            dd.AddItem(C, C, (C = Housecode))
        Next

        stb.Append("Select House Code:")
        stb.Append(dd.Build)

        dd1.AddItem("All", "All", ("All" = DeviceCode))
        For i = 1 To 16
            dd1.AddItem(i.ToString, i.ToString, (i.ToString = DeviceCode))
        Next

        stb.Append("Select Unit Code:")
        stb.Append(dd1.Build)

        For Each item In Commands.Keys
            dd2.AddItem(Commands(item), item, (item = Command))
        Next

        stb.Append("Select Command:")
        stb.Append(dd2.Build)


        Return stb.ToString
    End Function

    Public Function ActionProcessPostUI(ByVal PostData As Collections.Specialized.NameValueCollection, _
                                        ByVal ActInfo As IPlugInAPI.strTrigActInfo) As IPlugInAPI.strMultiReturn

        Dim Ret As New HomeSeerAPI.IPlugInAPI.strMultiReturn
        Dim UID As String
        UID = ActInfo.UID.ToString

        Ret.sResult = ""
        ' We cannot be passed info ByRef from HomeSeer, so turn right around and return this same value so that if we want, 
        '   we can exit here by returning 'Ret', all ready to go.  If in this procedure we need to change DataOut or TrigInfo,
        '   we can still do that.
        Ret.DataOut = ActInfo.DataIn
        Ret.TrigActInfo = ActInfo

        If PostData Is Nothing Then Return Ret
        If PostData.Count < 1 Then Return Ret

        If Not (ActInfo.DataIn Is Nothing) Then
            DeSerializeObject(ActInfo.DataIn, action)
        End If

        Dim parts As Collections.Specialized.NameValueCollection

        Dim sKey As String

        parts = PostData

        Try
            For Each sKey In parts.Keys
                If sKey Is Nothing Then Continue For
                If String.IsNullOrEmpty(sKey.Trim) Then Continue For
                Select Case True
                    Case InStr(sKey, "Housecodes_" & UID) > 0, InStr(sKey, "DeviceCodes_" & UID) > 0, InStr(sKey, "Commands_" & UID) > 0
                        action.Add(CObj(parts(sKey)), sKey)
                End Select
            Next
            If Not SerializeObject(action, Ret.DataOut) Then
                Ret.sResult = IFACE_NAME & " Error, Serialization failed. Signal Action not added."
                Return Ret
            End If
        Catch ex As Exception
            Ret.sResult = "ERROR, Exception in Action UI of " & IFACE_NAME & ": " & ex.Message
            Return Ret
        End Try

        ' All OK
        Ret.sResult = ""
        Return Ret
    End Function

    Public Function ActionFormatUI(ByVal ActInfo As IPlugInAPI.strTrigActInfo) As String
        Dim stb As New StringBuilder
        Dim sKey As String
        Dim Housecode As String = ""
        Dim DeviceCode As String = ""
        Dim Command As String = ""
        Dim UID As String
        UID = ActInfo.UID.ToString

        If Not (ActInfo.DataIn Is Nothing) Then
            DeSerializeObject(ActInfo.DataIn, action)
        End If

        For Each sKey In action.Keys
            Select Case True
                Case InStr(sKey, "Housecodes_" & UID) > 0
                    Housecode = action(sKey)
                Case InStr(sKey, "DeviceCodes_" & UID) > 0
                    DeviceCode = action(sKey)
                Case InStr(sKey, "Commands_" & UID) > 0
                    Command = action(sKey)
            End Select
        Next

        stb.Append(" the system will execute the " & Commands(Command) & " command ")
        stb.Append("on Housecode " & Housecode & " ")
        If DeviceCode = "ALL" Then
            stb.Append("for all Unitcodes")
        Else
            stb.Append("for Unitcode " & DeviceCode)
        End If

        Return stb.ToString
    End Function

#End Region

#Region "Trigger Interface"

    Public ReadOnly Property TriggerConfigured(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As Boolean
        Get
            Dim Configured As Boolean = False
            Dim sKey As String
            Dim itemsConfigured As Integer = 0
            Dim itemsToConfigure As Integer = 3
            Dim UID As String
            UID = TrigInfo.UID.ToString

            If Not (TrigInfo.DataIn Is Nothing) Then
                DeSerializeObject(TrigInfo.DataIn, trigger)
                For Each sKey In trigger.Keys
                    Select Case True
                        Case InStr(sKey, "Housecodes_" & UID) > 0 AndAlso trigger(sKey) <> ""
                            itemsConfigured += 1
                        Case InStr(sKey, "DeviceCodes_" & UID) > 0 AndAlso trigger(sKey) <> ""
                            itemsConfigured += 1
                        Case InStr(sKey, "Commands_" & UID) > 0 AndAlso trigger(sKey) <> ""
                            itemsConfigured += 1
                    End Select
                Next
                If itemsConfigured = itemsToConfigure Then Configured = True
            End If
            Return Configured
        End Get
    End Property

    Public Function TriggerBuildUI(ByVal sUnique As String, ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As String
        Dim UID As String
        UID = TrigInfo.UID.ToString
        Dim stb As New StringBuilder
        Dim Housecode As String = ""
        Dim DeviceCode As String = ""
        Dim Command As String = ""
        Dim dd As New clsJQuery.jqDropList("Housecodes_" & UID & sUnique, Pagename, True)
        Dim dd1 As New clsJQuery.jqDropList("DeviceCodes_" & UID & sUnique, Pagename, True)
        Dim dd2 As New clsJQuery.jqDropList("Commands_" & UID & sUnique, Pagename, True)
        Dim sKey As String

        dd.autoPostBack = True
        dd.AddItem("--Please Select--", "", False)
        dd1.autoPostBack = True
        dd1.AddItem("--Please Select--", "", False)
        dd2.autoPostBack = True
        dd2.AddItem("--Please Select--", "", False)

        If Not (TrigInfo.DataIn Is Nothing) Then
            DeSerializeObject(TrigInfo.DataIn, trigger)
        Else 'new event, so clean out the trigger object
            trigger = New trigger
        End If

        For Each sKey In trigger.Keys
            Select Case True
                Case InStr(sKey, "Housecodes_" & UID) > 0
                    Housecode = trigger(sKey)
                Case InStr(sKey, "DeviceCodes_" & UID) > 0
                    DeviceCode = trigger(sKey)
                Case InStr(sKey, "Commands_" & UID) > 0
                    Command = trigger(sKey)
            End Select
        Next

        For Each C In "ABCDEFGHIJKLMNOP"
            dd.AddItem(C, C, (C = Housecode))
        Next

        stb.Append("Select House Code:")
        stb.Append(dd.Build)

        dd1.AddItem("All", "All", ("All" = DeviceCode))
        For i = 1 To 16
            dd1.AddItem(i.ToString, i.ToString, (i.ToString = DeviceCode))
        Next

        stb.Append("Select Unit Code:")
        stb.Append(dd1.Build)

        For Each item In Commands.Keys
            dd2.AddItem(Commands(item), item, (item = Command))
        Next

        stb.Append("Select Command:")
        stb.Append(dd2.Build)


        Return stb.ToString
    End Function

    Public Function TriggerProcessPostUI(ByVal PostData As System.Collections.Specialized.NameValueCollection, _
                                                     ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As HomeSeerAPI.IPlugInAPI.strMultiReturn
        Dim Ret As New HomeSeerAPI.IPlugInAPI.strMultiReturn
        Dim UID As String
        UID = TrigInfo.UID.ToString

        Ret.sResult = ""
        ' We cannot be passed info ByRef from HomeSeer, so turn right around and return this same value so that if we want, 
        '   we can exit here by returning 'Ret', all ready to go.  If in this procedure we need to change DataOut or TrigInfo,
        '   we can still do that.
        Ret.DataOut = TrigInfo.DataIn
        Ret.TrigActInfo = TrigInfo

        If PostData Is Nothing Then Return Ret
        If PostData.Count < 1 Then Return Ret

        If Not (TrigInfo.DataIn Is Nothing) Then
            DeSerializeObject(TrigInfo.DataIn, trigger)
        End If

        Dim parts As Collections.Specialized.NameValueCollection

        Dim sKey As String

        parts = PostData
        Try
            For Each sKey In parts.Keys
                If sKey Is Nothing Then Continue For
                If String.IsNullOrEmpty(sKey.Trim) Then Continue For
                Select Case True
                    Case InStr(sKey, "Housecodes_" & UID) > 0, InStr(sKey, "DeviceCodes_" & UID) > 0, InStr(sKey, "Commands_" & UID) > 0
                        trigger.Add(CObj(parts(sKey)), sKey)
                End Select
            Next
            If Not SerializeObject(trigger, Ret.DataOut) Then
                Ret.sResult = IFACE_NAME & " Error, Serialization failed. Signal Trigger not added."
                Return Ret
            End If
        Catch ex As Exception
            Ret.sResult = "ERROR, Exception in Trigger UI of " & IFACE_NAME & ": " & ex.Message
            Return Ret
        End Try

        ' All OK
        Ret.sResult = ""
        Return Ret
    End Function

    Public Function TriggerFormatUI(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As String
        Dim stb As New StringBuilder
        Dim sKey As String
        Dim Housecode As String = ""
        Dim DeviceCode As String = ""
        Dim Command As String = ""
        Dim UID As String
        UID = TrigInfo.UID.ToString

        If Not (TrigInfo.DataIn Is Nothing) Then
            DeSerializeObject(TrigInfo.DataIn, trigger)
        End If

        For Each sKey In trigger.Keys
            Select Case True
                Case InStr(sKey, "Housecodes_" & UID) > 0
                    Housecode = trigger(sKey)
                Case InStr(sKey, "DeviceCodes_" & UID) > 0
                    DeviceCode = trigger(sKey)
                Case InStr(sKey, "Commands_" & UID) > 0
                    Command = trigger(sKey)
            End Select
        Next

        stb.Append(" the system detected the " & Commands(Command) & " command ")
        stb.Append("on Housecode " & Housecode & " ")
        If DeviceCode = "ALL" Then
            stb.Append("from a Unitcode")
        Else
            stb.Append("from Unitcode " & DeviceCode)
        End If

        Return stb.ToString
    End Function

#End Region

#End Region

#Region "HomeSeer-Required Functions"

    Function name() As String
        name = IFACE_NAME
    End Function

    Public Function AccessLevel() As Integer
        AccessLevel = 1
    End Function

#End Region

#Region "Init"

    Public Function InitIO(ByVal port As String) As String
        'Dim o As Object = nothing
        RegisterWebPage(sConfigPage)
        RegisterWebPage(sStatusPage)
        'if more than 1 action/trigger is needed, or if subactions/triggers are needed, then add them all here
        'actions.Add(o, "Action1")
        'actions.Add(o, "Action2")
        'triggers.Add(o, "Trigger1")
        'triggers.Add(o, "Trigger2")

        LoadCommands()

        Return ""
    End Function

    Public Sub ShutdownIO()
        Try
            Try
                hs.SaveEventsDevices()
            Catch ex As Exception
                Log("could not save devices")
            End Try
            bShutDown = True
        Catch ex As Exception
            Log("Error ending " & IFACE_NAME & " Plug-In")
        End Try

    End Sub

    Sub LoadCommands()
        Commands.Add(CObj("All Lights Off"), CStr(CInt(DEVICE_COMMAND.All_Lights_Off)))
        Commands.Add(CObj("All Lights On"), CStr(CInt(DEVICE_COMMAND.All_Lights_On)))
        Commands.Add(CObj("Device On"), CStr(CInt(DEVICE_COMMAND.UOn)))
        Commands.Add(CObj("Device Off"), CStr(CInt(DEVICE_COMMAND.UOff)))
    End Sub

#End Region

#Region "Web Page Processing"

    Private Function SelectPage(ByVal pageName As String) As Object
        SelectPage = Nothing
        Select Case pageName
            Case ConfigPage.PageName
                SelectPage = ConfigPage
            Case StatusPage.PageName
                SelectPage = StatusPage
            Case Else
                SelectPage = ConfigPage
        End Select
    End Function

    Public Function postBackProc(page As String, data As String, user As String, userRights As Integer) As String
        WebPage = SelectPage(page)
        Return WebPage.postBackProc(page, data, user, userRights)
    End Function

    Public Function GetPagePlugin(ByVal pageName As String, ByVal user As String, ByVal userRights As Integer, ByVal queryString As String) As String
        ' build and return the actual page
        WebPage = SelectPage(pageName)
        Return WebPage.GetPagePlugin(pageName, user, userRights, queryString)
    End Function

#End Region

End Class
