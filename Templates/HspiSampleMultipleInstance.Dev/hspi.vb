Imports System
Imports Scheduler
Imports HomeSeerAPI
Imports HSCF.Communication.Scs.Communication.EndPoints.Tcp
Imports HSCF.Communication.ScsServices.Client
Imports HSCF.Communication.ScsServices.Service
Imports System.Reflection
Imports System.Web
Imports System.Text


Public Class HSPI
    Implements IPlugInAPI    ' this API is required for ALL plugins
    'Implements IThermostatAPI           ' add this API if this plugin supports thermostats
    Public instance As String = ""
    Const PageName As String = "Events"
    Dim sConfigPage As String = "Sample_Config"
    Dim sStatusPage As String = "Sample_Status"
    Dim ConfigPage As web_config
    Dim StatusPage As web_status
    Dim WebPage As Object

    Dim actions As New hsCollection
    Dim action As New action
    Dim triggers As New hsCollection
    Dim trigger As New trigger

    Dim Commands As New hsCollection

    Sub LoadCommands()
        Commands.Add(CObj("All Lights Off"), CStr(CInt(DEVICE_COMMAND.All_Lights_Off)))
        Commands.Add(CObj("All Lights On"), CStr(CInt(DEVICE_COMMAND.All_Lights_On)))
        Commands.Add(CObj("Device On"), CStr(CInt(DEVICE_COMMAND.UOn)))
        Commands.Add(CObj("Device Off"), CStr(CInt(DEVICE_COMMAND.UOff)))
    End Sub

    Public Function PluginFunction(ByVal proc As String, ByVal parms() As Object) As Object Implements IPlugInAPI.PluginFunction
        Try
            Dim ty As Type = Me.GetType
            Dim mi As MethodInfo = ty.GetMethod(proc)
            If mi Is Nothing Then
                Log("Method " & proc & " does not exist in this plugin.", MessageType.Error_)
                Return Nothing
            End If
            Return (mi.Invoke(Me, parms))
        Catch ex As Exception
            Log("Error in PluginProc: " & ex.Message, MessageType.Error_)
        End Try

        Return Nothing
    End Function

    Public Function PluginPropertyGet(ByVal proc As String, parms() As Object) As Object Implements IPlugInAPI.PluginPropertyGet
        Try
            Dim ty As Type = Me.GetType
            Dim mi As PropertyInfo = ty.GetProperty(proc)
            If mi Is Nothing Then
                Log("Property " & proc & " does not exist in this plugin.", MessageType.Error_)
                Return Nothing
            End If
            Return mi.GetValue(Me, parms)
        Catch ex As Exception
            Log("Error in PluginPropertyGet: " & ex.Message, MessageType.Error_)
        End Try

        Return Nothing
    End Function

    Public Sub PluginPropertySet(ByVal proc As String, value As Object) Implements IPlugInAPI.PluginPropertySet
        Try
            Dim ty As Type = Me.GetType
            Dim mi As PropertyInfo = ty.GetProperty(proc)
            If mi Is Nothing Then
                Log("Property " & proc & " does not exist in this plugin.", MessageType.Error_)
            End If
            mi.SetValue(Me, value, Nothing)
        Catch ex As Exception
            Log("Error in PluginPropertySet: " & ex.Message, MessageType.Error_)
        End Try
    End Sub

    Public ReadOnly Property Name As String Implements HomeSeerAPI.IPlugInAPI.Name
        Get
            Return IFACE_NAME
        End Get
    End Property

    Public ReadOnly Property HSCOMPort As Boolean Implements HomeSeerAPI.IPlugInAPI.HSCOMPort
        Get
            Return False
        End Get
    End Property

    Public Function Capabilities() As Integer Implements HomeSeerAPI.IPlugInAPI.Capabilities
        Return HomeSeerAPI.Enums.eCapabilities.CA_IO
    End Function

    Public Function AccessLevel() As Integer Implements HomeSeerAPI.IPlugInAPI.AccessLevel
        Return 1
    End Function

    Public Function InterfaceStatus() As HomeSeerAPI.IPlugInAPI.strInterfaceStatus Implements HomeSeerAPI.IPlugInAPI.InterfaceStatus
        Dim es As New IPlugInAPI.strInterfaceStatus
        es.intStatus = IPlugInAPI.enumInterfaceStatus.OK
        Return es
    End Function

    Public Function SupportsMultipleInstances() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsMultipleInstances
        Return True
    End Function

    Public Function SupportsMultipleInstancesSingleEXE() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsMultipleInstancesSingleEXE
#If MULTIPLE_EXE Then
        Return False
#Else
        Return True
#End If
    End Function

    Public Function InstanceFriendlyName() As String Implements HomeSeerAPI.IPlugInAPI.InstanceFriendlyName
        Return ""
    End Function

    Public Function InitIO(ByVal port As String) As String Implements HomeSeerAPI.IPlugInAPI.InitIO

        If instance <> "" Then
            ConfigPage = New web_config(sConfigPage & ":" & instance)
        Else
            ConfigPage = New web_config(sConfigPage)
        End If

        ConfigPage.hspiref = Me
        If instance <> "" Then
            StatusPage = New web_status(sStatusPage & ":" & instance)
        Else
            StatusPage = New web_status(sStatusPage)
        End If



        'Dim o As Object = nothing
        RegisterWebPage(sConfigPage, "", "", instance)
        RegisterWebPage(sStatusPage, "", "", instance)
        'if more than 1 action/trigger is needed, or if subactions/triggers are needed, then add them all here
        'actions.Add(o, "Action1")
        'actions.Add(o, "Action2")
        'triggers.Add(o, "Trigger1")
        'triggers.Add(o, "Trigger2")

        Dim wpd As New HomeSeerAPI.WebPageDesc
        wpd.plugInName = IFACE_NAME
        wpd.plugInInstance = instance
        wpd.link = sConfigPage
        wpd.linktext = "SampleMIConfig" & instance
        wpd.page_title = "Sample MI Config" & instance
        callback.RegisterConfigLink(wpd)

        LoadCommands()


        Return ""
    End Function

    Public Function RaisesGenericCallbacks() As Boolean Implements HomeSeerAPI.IPlugInAPI.RaisesGenericCallbacks
        Return False
    End Function

    Public Sub SetIOMulti(colSend As System.Collections.Generic.List(Of HomeSeerAPI.CAPI.CAPIControl)) Implements HomeSeerAPI.IPlugInAPI.SetIOMulti
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

    Public Sub ShutdownIO() Implements HomeSeerAPI.IPlugInAPI.ShutdownIO
        Try
            Try
                hs.SaveEventsDevices()
            Catch ex As Exception
                Log("could not save devices")
            End Try
#If SINGLE_EXE Then
            If instance = "" Then
                bShutDown = True
            End If
#End If

#If MULTIPLE_EXE Then
            bShutDown = True
#End If

        Catch ex As Exception
            Log("Error ending " & IFACE_NAME & " Plug-In")
        End Try
    End Sub

    Public Sub HSEvent(ByVal EventType As Enums.HSEvent, ByVal parms() As Object) Implements HomeSeerAPI.IPlugInAPI.HSEvent
        Console.WriteLine("HSEvent: " & EventType.ToString)
        Select Case EventType
            Case Enums.HSEvent.VALUE_CHANGE
        End Select
    End Sub

    Public Function PollDevice(ByVal dvref As Integer) As IPlugInAPI.PollResultInfo Implements HomeSeerAPI.IPlugInAPI.PollDevice

    End Function

    Public Function GenPage(ByVal link As String) As String Implements HomeSeerAPI.IPlugInAPI.GenPage
        Return Nothing
    End Function

    Public Function PagePut(ByVal data As String) As String Implements HomeSeerAPI.IPlugInAPI.PagePut
        Return Nothing
    End Function

    Public Function GetPagePlugin(ByVal pageName As String, ByVal user As String, ByVal userRights As Integer, ByVal queryString As String) As String Implements HomeSeerAPI.IPlugInAPI.GetPagePlugin
        ' build and return the actual page
        Console.WriteLine("GetPagePlugin INSTANCE:" & instance)

        Dim page_name As String = pageName
        If instance <> "" Then page_name = pageName & ":" & instance
        WebPage = SelectPage(page_name)
        Return WebPage.GetPagePlugin(page_name, user, userRights, queryString, instance)
    End Function

    Public Function PostBackProc(ByVal pageName As String, ByVal data As String, ByVal user As String, ByVal userRights As Integer) As String Implements HomeSeerAPI.IPlugInAPI.PostBackProc
        Console.WriteLine("PostBackProc INSTANCE:" & instance)
        WebPage = SelectPage(pageName)
        Return WebPage.postBackProc(pageName, data, user, userRights)
    End Function

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

#Region "Actions"
    Sub SetActions()
        Dim o As Object = Nothing
        If actions.Count = 0 Then
            actions.Add(o, "Send Command")
        End If
    End Sub

    Public Property ActionAdvancedMode As Boolean Implements HomeSeerAPI.IPlugInAPI.ActionAdvancedMode
        Set(ByVal value As Boolean)

        End Set
        Get

#Disable Warning BC42355 ' Property doesn't return a value on all code paths
        End Get
#Enable Warning BC42355 ' Property doesn't return a value on all code paths
    End Property

    Public Function ActionBuildUI(ByVal sUnique As String, ByVal ActInfo As IPlugInAPI.strTrigActInfo) As String Implements HomeSeerAPI.IPlugInAPI.ActionBuildUI
        Dim stb As New StringBuilder
        Dim Housecode As String = ""
        Dim DeviceCode As String = ""
        Dim Command As String = ""
        Dim dd As New clsJQuery.jqDropList("HousecodesA1" & sUnique, PageName, True)
        Dim dd1 As New clsJQuery.jqDropList("DeviceCodesA1" & sUnique, PageName, True)
        Dim dd2 As New clsJQuery.jqDropList("CommandsA1" & sUnique, PageName, True)
        Dim sKey As String
        Console.WriteLine("ActionBuildUI Instance:" & instance)
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
                Case InStr(sKey, "HousecodesA1") > 0
                    Housecode = action(sKey)
                Case InStr(sKey, "DeviceCodesA1") > 0
                    DeviceCode = action(sKey)
                Case InStr(sKey, "CommandsA1") > 0
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

    Public Function ActionConfigured(ByVal ActInfo As IPlugInAPI.strTrigActInfo) As Boolean Implements HomeSeerAPI.IPlugInAPI.ActionConfigured
        Dim Configured As Boolean = False
        Dim sKey As String
        Dim itemsConfigured As Integer = 0
        Dim itemsToConfigure As Integer = 3

        If Not (ActInfo.DataIn Is Nothing) Then
            DeSerializeObject(ActInfo.DataIn, action)
            For Each sKey In action.Keys
                Select Case True
                    Case InStr(sKey, "HousecodesA1") > 0 AndAlso action(sKey) <> ""
                        itemsConfigured += 1
                    Case InStr(sKey, "DeviceCodesA1") > 0 AndAlso action(sKey) <> ""
                        itemsConfigured += 1
                    Case InStr(sKey, "CommandsA1") > 0 AndAlso action(sKey) <> ""
                        itemsConfigured += 1
                End Select
            Next
            If itemsConfigured = itemsToConfigure Then Configured = True
        End If
        Return Configured
    End Function

    Public Function ActionReferencesDevice(ByVal ActInfo As IPlugInAPI.strTrigActInfo, ByVal dvRef As Integer) As Boolean Implements HomeSeerAPI.IPlugInAPI.ActionReferencesDevice
        Return False
    End Function

    Public Function ActionFormatUI(ByVal ActInfo As IPlugInAPI.strTrigActInfo) As String Implements HomeSeerAPI.IPlugInAPI.ActionFormatUI
        Dim stb As New StringBuilder
        Dim sKey As String
        Dim Housecode As String = ""
        Dim DeviceCode As String = ""
        Dim Command As String = ""

        If Not (ActInfo.DataIn Is Nothing) Then
            DeSerializeObject(ActInfo.DataIn, action)
        End If

        For Each sKey In action.Keys
            Select Case True
                Case InStr(sKey, "HousecodesA1") > 0
                    Housecode = action(sKey)
                Case InStr(sKey, "DeviceCodesA1") > 0
                    DeviceCode = action(sKey)
                Case InStr(sKey, "CommandsA1") > 0
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

    Public ReadOnly Property ActionName(ByVal ActionNumber As Integer) As String Implements HomeSeerAPI.IPlugInAPI.ActionName
        Get
            SetActions()
            If ActionNumber > 0 AndAlso ActionNumber <= actions.Count Then
                Return IFACE_NAME & ": " & instance & ": " & actions.Keys(ActionNumber - 1)
            Else
                Return ""
            End If
        End Get
    End Property

    Public Function ActionProcessPostUI(ByVal PostData As Collections.Specialized.NameValueCollection, ByVal ActInfoIN As IPlugInAPI.strTrigActInfo) As IPlugInAPI.strMultiReturn Implements HomeSeerAPI.IPlugInAPI.ActionProcessPostUI
        Dim Ret As New HomeSeerAPI.IPlugInAPI.strMultiReturn

        Ret.sResult = ""
        ' We cannot be passed info ByRef from HomeSeer, so turn right around and return this same value so that if we want, 
        '   we can exit here by returning 'Ret', all ready to go.  If in this procedure we need to change DataOut or TrigInfo,
        '   we can still do that.
        Ret.DataOut = ActInfoIN.DataIn
        Ret.TrigActInfo = ActInfoIN

        If PostData Is Nothing Then Return Ret
        If PostData.Count < 1 Then Return Ret

        If Not (ActInfoIN.DataIn Is Nothing) Then
            DeSerializeObject(ActInfoIN.DataIn, action)
        End If

        Dim parts As Collections.Specialized.NameValueCollection

        Dim sKey As String

        parts = PostData

        Try
            For Each sKey In parts.Keys
                If sKey Is Nothing Then Continue For
                If String.IsNullOrEmpty(sKey.Trim) Then Continue For
                Select Case True
                    Case InStr(sKey, "HousecodesA1") > 0, InStr(sKey, "DeviceCodesA1") > 0, InStr(sKey, "CommandsA1") > 0
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

    Public Function ActionCount() As Integer Implements HomeSeerAPI.IPlugInAPI.ActionCount
        Console.WriteLine("Action count for instance: " & instance)
        SetActions()
        Return actions.Count
    End Function


    Public Property Condition(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As Boolean Implements HomeSeerAPI.IPlugInAPI.Condition
        Set(ByVal value As Boolean)

        End Set
        Get
            Return False
        End Get
    End Property

    Public Function HandleAction(ByVal ActInfo As IPlugInAPI.strTrigActInfo) As Boolean Implements HomeSeerAPI.IPlugInAPI.HandleAction
        Dim Housecode As String = ""
        Dim DeviceCode As String = ""
        Dim Command As String = ""
        Try
            If Not (ActInfo.DataIn Is Nothing) Then
                DeSerializeObject(ActInfo.DataIn, action)
            Else
                Return False
            End If

            For Each sKey In action.Keys
                Select Case True
                    Case InStr(sKey, "HousecodesA1") > 0
                        Housecode = action(sKey)
                    Case InStr(sKey, "DeviceCodesA1") > 0
                        DeviceCode = action(sKey)
                    Case InStr(sKey, "CommandsA1") > 0
                        Command = action(sKey)
                End Select
            Next

            SendCommand(Housecode, DeviceCode, Command)

        Catch ex As Exception
            hs.WriteLog(IFACE_NAME, "Error executing action: " & ex.Message)
        End Try
        Return True
    End Function
#End Region

    Public ReadOnly Property HasConditions(ByVal TriggerNumber As Integer) As Boolean Implements HomeSeerAPI.IPlugInAPI.HasConditions
        Get
            Return False
        End Get
    End Property

    Sub SetTriggers()
        Dim o As Object = Nothing
        If triggers.Count = 0 Then
            triggers.Add(o, "Recieve Command")
        End If
    End Sub

    Public Function TriggerTrue(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As Boolean Implements HomeSeerAPI.IPlugInAPI.TriggerTrue
        Return True
    End Function

    Public ReadOnly Property HasTriggers() As Boolean Implements HomeSeerAPI.IPlugInAPI.HasTriggers
        Get
            SetTriggers()
            Return IIf(triggers.Count > 0, True, False)
        End Get
    End Property

    Public ReadOnly Property SubTriggerCount(ByVal TriggerNumber As Integer) As Integer Implements HomeSeerAPI.IPlugInAPI.SubTriggerCount
        Get
            Dim trigger As trigger
            If ValidTrig(TriggerNumber) Then
                trigger = triggers(TriggerNumber - 1)
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

    Friend Function ValidTrig(ByVal TrigIn As Integer) As Boolean
        SetTriggers()
        If TrigIn > 0 AndAlso TrigIn <= triggers.Count Then
            Return True
        End If
        Return False
    End Function

    Private Function ValidSubTrig(ByVal TrigIn As Integer, ByVal SubTrigIn As Integer) As Boolean
        Dim trigger As trigger = Nothing
        If TrigIn > 0 AndAlso TrigIn <= triggers.Count Then
            trigger = triggers(TrigIn)
            If Not (trigger Is Nothing) Then
                If SubTrigIn > 0 AndAlso SubTrigIn <= trigger.Count Then Return True
            End If
        End If
        Return False
    End Function

    Public ReadOnly Property SubTriggerName(ByVal TriggerNumber As Integer, ByVal SubTriggerNumber As Integer) As String Implements HomeSeerAPI.IPlugInAPI.SubTriggerName
        Get
            Dim trigger As trigger
            If ValidSubTrig(TriggerNumber, SubTriggerNumber) Then
                trigger = triggers(TriggerNumber)
                Return IFACE_NAME & ": " & trigger.Keys(SubTriggerNumber)
            Else
                Return ""
            End If
        End Get
    End Property

    Public Function TriggerBuildUI(ByVal sUnique As String, ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As String Implements HomeSeerAPI.IPlugInAPI.TriggerBuildUI
        Dim stb As New StringBuilder
        Dim Housecode As String = ""
        Dim DeviceCode As String = ""
        Dim Command As String = ""
        Dim dd As New clsJQuery.jqDropList("HousecodesT1" & sUnique, PageName, True)
        Dim dd1 As New clsJQuery.jqDropList("DeviceCodesT1" & sUnique, PageName, True)
        Dim dd2 As New clsJQuery.jqDropList("CommandsT1" & sUnique, PageName, True)
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
                Case InStr(sKey, "HousecodesT1") > 0
                    Housecode = trigger(sKey)
                Case InStr(sKey, "DeviceCodesT1") > 0
                    DeviceCode = trigger(sKey)
                Case InStr(sKey, "CommandsT1") > 0
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

    Public ReadOnly Property TriggerConfigured(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As Boolean Implements HomeSeerAPI.IPlugInAPI.TriggerConfigured
        Get
            Dim Configured As Boolean = False
            Dim sKey As String
            Dim itemsConfigured As Integer = 0
            Dim itemsToConfigure As Integer = 3

            If Not (TrigInfo.DataIn Is Nothing) Then
                DeSerializeObject(TrigInfo.DataIn, trigger)
                For Each sKey In trigger.Keys
                    Select Case True
                        Case InStr(sKey, "HousecodesT1") > 0 AndAlso trigger(sKey) <> ""
                            itemsConfigured += 1
                        Case InStr(sKey, "DeviceCodesT1") > 0 AndAlso trigger(sKey) <> ""
                            itemsConfigured += 1
                        Case InStr(sKey, "CommandsT1") > 0 AndAlso trigger(sKey) <> ""
                            itemsConfigured += 1
                    End Select
                Next
                If itemsConfigured = itemsToConfigure Then Configured = True
            End If
            Return Configured
        End Get
    End Property

    Public Function TriggerReferencesDevice(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo, ByVal dvRef As Integer) As Boolean Implements HomeSeerAPI.IPlugInAPI.TriggerReferencesDevice
        Return False
    End Function

    Public Function TriggerFormatUI(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As String Implements HomeSeerAPI.IPlugInAPI.TriggerFormatUI
        Dim stb As New StringBuilder
        Dim sKey As String
        Dim Housecode As String = ""
        Dim DeviceCode As String = ""
        Dim Command As String = ""

        If Not (TrigInfo.DataIn Is Nothing) Then
            DeSerializeObject(TrigInfo.DataIn, trigger)
        End If

        For Each sKey In trigger.Keys
            Select Case True
                Case InStr(sKey, "HousecodesT1") > 0
                    Housecode = trigger(sKey)
                Case InStr(sKey, "DeviceCodesT1") > 0
                    DeviceCode = trigger(sKey)
                Case InStr(sKey, "CommandsT1") > 0
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

    Public ReadOnly Property TriggerName(ByVal TriggerNumber As Integer) As String Implements HomeSeerAPI.IPlugInAPI.TriggerName
        Get
            If Not ValidTrig(TriggerNumber) Then
                Return ""
            Else
                Return IFACE_NAME & ": " & instance & ": " & triggers.Keys(TriggerNumber - 1)
            End If
        End Get
    End Property

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

        If Not (TrigInfoIn.DataIn Is Nothing) Then
            DeSerializeObject(TrigInfoIn.DataIn, trigger)
        End If

        Dim parts As Collections.Specialized.NameValueCollection

        Dim sKey As String

        parts = PostData
        Try
            For Each sKey In parts.Keys
                If sKey Is Nothing Then Continue For
                If String.IsNullOrEmpty(sKey.Trim) Then Continue For
                Select Case True
                    Case InStr(sKey, "HousecodesT1") > 0, InStr(sKey, "DeviceCodesT1") > 0, InStr(sKey, "CommandsT1") > 0
                        trigger.Add(CObj(parts(sKey)), sKey)
                End Select
            Next
            If Not SerializeObject(trigger, Ret.DataOut) Then
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

    Public ReadOnly Property TriggerCount As Integer Implements HomeSeerAPI.IPlugInAPI.TriggerCount
        Get
            SetTriggers()
            Return triggers.Count
        End Get
    End Property

    Public Function SupportsConfigDevice() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsConfigDevice
        Return True
    End Function

    Public Function SupportsConfigDeviceAll() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsConfigDeviceAll
        Return False
    End Function

    Public Function SupportsAddDevice() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsAddDevice
        Return True
    End Function

    Function ConfigDevicePost(ByVal ref As Integer, ByVal data As String, ByVal user As String, ByVal userRights As Integer) As Enums.ConfigDevicePostReturn Implements IPlugInAPI.ConfigDevicePost
        Dim dv As Scheduler.Classes.DeviceClass = Nothing
        Dim parts As Collections.Specialized.NameValueCollection
        Dim PED As clsPlugExtraData
        Dim ReturnValue As Integer = Enums.ConfigDevicePostReturn.DoneAndCancel
        Dim Sample As SampleClass

        Console.WriteLine("Sample basic mi ConfigDevicePost data:" & data)

        Try
            parts = HttpUtility.ParseQueryString(data)

            dv = hs.GetDeviceByRef(ref)
            PED = dv.PlugExtraData_Get(hs)
            Sample = PEDGet(PED, "Sample")
            If Sample Is Nothing Then
                InitHSDevice(dv)
            End If

            Sample.Housecode = parts("HouseCode_" & instance)
            Sample.DeviceCode = parts("DeviceCode_" & instance)

            PED = dv.PlugExtraData_Get(hs)
            PEDAdd(PED, "Sample", Sample)
            dv.PlugExtraData_Set(hs) = PED
            ' this device is now part of this instance, set it in device
            dv.InterfaceInstance(hs) = instance
            hs.SaveEventsDevices()

            Return ReturnValue
        Catch ex As Exception

        End Try
        Return ReturnValue
    End Function

    Function ConfigDevice(ByVal ref As Integer, ByVal user As String, ByVal userRights As Integer, newDevice As Boolean) As String Implements IPlugInAPI.ConfigDevice
        Dim dv As Scheduler.Classes.DeviceClass = Nothing
        Dim stb As New StringBuilder
        Dim ddHouseCode As New clsJQuery.jqDropList("HouseCode_" & instance, "", False)
        Dim ddUnitCode As New clsJQuery.jqDropList("DeviceCode_" & instance, "", False)
        Dim bSave As New clsJQuery.jqButton("Save_" & instance, "Done", "DeviceUtility", True)
        Dim HouseCode As String = ""
        Dim Devicecode As String = ""
        Dim Sample As SampleClass

        Console.WriteLine("ConfigDevice called in MI plugin, ref:" & ref.ToString)
        dv = hs.GetDeviceByRef(ref)

        Dim PED As clsPlugExtraData = dv.PlugExtraData_Get(hs)

        Sample = PEDGet(PED, "Sample")

        If Sample Is Nothing Then
            ' Set the defaults
            Sample = New SampleClass
            InitHSDevice(dv, "Sample", instance)
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
            stb.Append("  <tr><td colspan='4' align='Center' style='font-size:10pt; height:30px;' nowrap>Select a Housecode and Unitcode that matches one of the devices HomeSeer will be communicating with." & " Instance: " & instance & "</td></tr>")
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

    Public Function Search(SearchString As String, RegEx As Boolean) As HomeSeerAPI.SearchReturn() Implements HomeSeerAPI.IPlugInAPI.Search

#Disable Warning BC42105 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42105 ' Function doesn't return a value on all code paths

    Public Sub SpeakIn(device As Integer, txt As String, w As Boolean, host As String) Implements HomeSeerAPI.IPlugInAPI.SpeakIn

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
End Class

