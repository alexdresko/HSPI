Imports HomeSeerAPI
Imports Scheduler
Imports System.Reflection
Imports System.Text


Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters


Module Util
   
    ' interface status
    ' for InterfaceStatus function call
    Public Const ERR_NONE = 0
    Public Const ERR_SEND = 1
    Public Const ERR_INIT = 2

    Public hs As HomeSeerAPI.IHSApplication
    Public callback As HomeSeerAPI.IAppCallbackAPI
    Public mediacallback As HomeSeerAPI.IMediaAPI_HS

    Public Const IFACE_NAME As String = "Sample Plugin"
    Public Instance As String = ""                             ' set when SupportMultipleInstances is TRUE
    Public gEXEPath As String = ""
    Public gGlobalTempScaleF As Boolean = True

    Public colTrigs_Sync As System.Collections.SortedList
    Public colTrigs As System.Collections.SortedList
    Public colActs_Sync As System.Collections.SortedList
    Public colActs As System.Collections.SortedList

    Private Demo_ARE As Threading.AutoResetEvent
    Private Demo_Thread As Threading.Thread

    Public Function StringIsNullOrEmpty(ByRef s As String) As Boolean
        If String.IsNullOrEmpty(s) Then Return True
        Return String.IsNullOrEmpty(s.Trim)
    End Function

    Friend Sub Demo_Start()

        Dim StartIt As Boolean = False
        If Demo_ARE Is Nothing Then
            Demo_ARE = New Threading.AutoResetEvent(False)
        End If
        If Demo_Thread Is Nothing Then
            StartIt = True
        ElseIf Not Demo_Thread.IsAlive Then
            StartIt = True
        End If
        If Not StartIt Then Exit Sub

        Demo_Thread = New Threading.Thread(AddressOf Demo_Proc)
        Demo_Thread.Name = "Sample Plug-In Number Generator for Trigger Demonstration"
        Demo_Thread.Priority = Threading.ThreadPriority.BelowNormal
        Demo_Thread.Start()

    End Sub

    Friend Function GetDecimals(ByVal D As Double) As Integer
        Dim s As String = ""
        Dim c(0) As Char
        c(0) = "0"  ' Trailing zeros to be removed.
        D = Math.Abs(D) - Math.Abs(Int(D))  ' Remove the whole number so the result always starts with "0." which is a known quantity.
        s = D.ToString("F30")
        s = s.TrimEnd(c)
        Return s.Length - 2     ' Minus 2 because that is the length of "0."
    End Function

    Friend RNum As New Random(2000)
    Friend Function Demo_Generate_Weight() As Double
        Dim Mult As Integer
        Dim W As Double
        ' The sole purpose of this procedure is to generate random weights
        '   for the purpose of testing the triggers and actions in this plug-in.

        Try
            Do
                Mult = RNum.Next(3)
            Loop Until Mult > 0
            W = (RNum.NextDouble * 2001) * Mult ' Generates a random weight between 0 and 6003 lbs.
        Catch ex As Exception
            Log(IFACE_NAME & " Error: Exception in demo number generation for Trigger 1: " & ex.Message, LogType.LOG_TYPE_WARNING)
        End Try

        Return W

    End Function

    Friend Class Volt_Demo

        Public Sub New(ByVal Euro As Boolean)
            MyBase.New()
            mvarVoltTypeEuro = Euro
        End Sub

        Private mvarVoltTypeEuro As Boolean
        Private mvarVoltage As Double
        Private mvarAverageVoltage As Double
        Private mvarSumVoltage As Double
        Private mvarStart As Date = Date.MinValue
        Private mvarCount As Integer

        Public ReadOnly Property Voltage As Double
            Get
                Return mvarVoltage
            End Get
        End Property
        Public ReadOnly Property AverageVoltage As Double
            Get
                Return mvarAverageVoltage
            End Get
        End Property
        Public ReadOnly Property AverageSince As Date
            Get
                Return mvarStart
            End Get
        End Property
        Public ReadOnly Property AverageCount As Integer
            Get
                Return mvarCount
            End Get
        End Property

        Friend Sub ResetAverage()
            mvarSumVoltage = 0
            mvarAverageVoltage = 0
            mvarCount = 0
        End Sub


        Friend RNum As New Random(IIf(mvarVoltTypeEuro, 220, 110))
        Friend Sub Demo_Generate_Value()

            ' The sole purpose of this procedure is to generate random voltages for 
            '   purposes of testing the triggers and actions in this plug-in.

            Try

                If HSPI.bShutDown Then Exit Sub

                ' Initialize time if it has not been done.
                If mvarStart = Date.MinValue Then
                    mvarStart = Now
                End If

                mvarCount += 1

                If mvarVoltTypeEuro Then
                    Do
                        mvarVoltage = (RNum.NextDouble * 240) + RNum.Next(20)
                    Loop While mvarVoltage < 205
                    Log("Voltage (European) = " & mvarVoltage.ToString, LogType.LOG_TYPE_INFO)
                Else
                    Do
                        mvarVoltage = (RNum.NextDouble * 120) + RNum.Next(10)
                    Loop While mvarVoltage < 100
                    Log("Voltage (North American) = " & mvarVoltage.ToString, LogType.LOG_TYPE_INFO)
                End If
                mvarSumVoltage += mvarVoltage
                mvarAverageVoltage = mvarSumVoltage / mvarCount

                If Double.MaxValue - mvarSumVoltage <= 300 Then
                    mvarStart = Now
                    mvarSumVoltage = mvarVoltage
                    mvarCount = 1
                End If

            Catch ex As Exception
                Log(IFACE_NAME & " Error: Exception in Value generation for Trigger 2: " & ex.Message, LogType.LOG_TYPE_WARNING)
            End Try

        End Sub

    End Class

    Friend Volt As Volt_Demo = Nothing
    Friend VoltEuro As Volt_Demo = Nothing

    Friend Sub Demo_Proc()
        Dim obj As Object = Nothing
        Dim Trig1 As MyTrigger1Ton = Nothing
        Dim Trig2 As MyTrigger2Shoe = Nothing
        Dim RND As New Random(1000)
        Dim T As Integer
        Dim Weight As Double
        Dim WeightEven As Double

        Try
            Do
                If HSPI.bShutDown Then Exit Do

                If colTrigs Is Nothing Then
                    Demo_ARE.WaitOne(10000)
                    If HSPI.bShutDown Then Exit Do
                    Continue Do
                End If
                If colTrigs.Count < 1 Then
                    Demo_ARE.WaitOne(10000)
                    If HSPI.bShutDown Then Exit Do
                    Continue Do
                End If

                T = RND.Next(10000, 30000)
                If HSPI.bShutDown Then Exit Do
                Demo_ARE.WaitOne(T)
                If HSPI.bShutDown Then Exit Do

                Weight = Demo_Generate_Weight()
                Dim i As Integer
                i = Convert.ToInt32(Weight / 2000)
                WeightEven = i * 2000

                Log("----------------------------------------------------------------", LogType.LOG_TYPE_INFO)
                Log("Weight = " & Weight.ToString & ", Even = " & WeightEven.ToString, LogType.LOG_TYPE_INFO)

                Dim TrigsToCheck() As IAllRemoteAPI.strTrigActInfo = Nothing
                Dim TC As IAllRemoteAPI.strTrigActInfo = Nothing
                Dim Trig As strTrigger

                ' We have generated a new Weight, so let's see if we need to trigger events.
                Try
                    ' Step 1: Ask HomeSeer for any triggers that are for this plug-in and are Type 1
                    TrigsToCheck = Nothing
                    TrigsToCheck = callback.TriggerMatches(IFACE_NAME, 1, -1)
                Catch ex As Exception
                End Try
                ' Step 2: If triggers were returned, we need to check them against our trigger value.
                If TrigsToCheck IsNot Nothing AndAlso TrigsToCheck.Count > 0 Then
                    For Each TC In TrigsToCheck
                        If TC.DataIn IsNot Nothing AndAlso TC.DataIn.Length > 10 Then
                            Trig = TriggerFromData(TC.DataIn)
                        Else
                            Trig = TriggerFromInfo(TC)
                        End If
                        If Not Trig.Result Then Continue For
                        If Trig.TrigObj Is Nothing Then Continue For
                        If Trig.WhichTrigger <> eTriggerType.OneTon Then Continue For
                        Try
                            Trig1 = CType(Trig.TrigObj, MyTrigger1Ton)
                        Catch ex As Exception
                            Trig1 = Nothing
                        End Try
                        If Trig1 Is Nothing Then Continue For
                        If Trig1.EvenTon Then
                            Log("Checking Weight Trigger (Even), " & WeightEven.ToString & " against trigger of " & Trig1.TriggerWeight.ToString, LogType.LOG_TYPE_INFO)
                            If WeightEven > Trig1.TriggerWeight Then
                                Log("Weight trigger is TRUE - calling FIRE! for event ID " & TC.evRef.ToString, LogType.LOG_TYPE_WARNING)
                                ' Step 3: If a trigger matches, call FIRE!
                                callback.TriggerFire(IFACE_NAME, TC)
                            End If
                        Else
                            Log("Checking Weight Trigger, " & Weight.ToString & " against trigger of " & Trig1.TriggerWeight.ToString, LogType.LOG_TYPE_INFO)
                            If Weight > Trig1.TriggerWeight Then
                                Log("Weight trigger is TRUE - calling FIRE! for event ID " & TC.evRef.ToString, LogType.LOG_TYPE_WARNING)
                                callback.TriggerFire(IFACE_NAME, TC)
                                ' Step 3: If a trigger matches, call FIRE!
                            End If
                        End If
                    Next
                End If


                If Volt Is Nothing Then Volt = New Volt_Demo(False)
                If VoltEuro Is Nothing Then VoltEuro = New Volt_Demo(True)
                Volt.Demo_Generate_Value()
                VoltEuro.Demo_Generate_Value()


                ' We have generated a new Voltage, so let's see if we need to trigger events.
                Try
                    ' Step 1: Ask HomeSeer for any triggers that are for this plug-in and are Type 2, SubType 1
                    TrigsToCheck = Nothing
                    TrigsToCheck = callback.TriggerMatches(IFACE_NAME, 2, 1)
                Catch ex As Exception
                End Try
                ' Step 2: If triggers were returned, we need to check them against our trigger value.
                If TrigsToCheck IsNot Nothing AndAlso TrigsToCheck.Count > 0 Then
                    For Each TC In TrigsToCheck
                        If TC.DataIn IsNot Nothing AndAlso TC.DataIn.Length > 10 Then
                            Trig = TriggerFromData(TC.DataIn)
                        Else
                            Trig = TriggerFromInfo(TC)
                        End If
                        If Not Trig.Result Then Continue For
                        If Trig.TrigObj Is Nothing Then Continue For
                        If Trig.WhichTrigger <> eTriggerType.TwoVolts Then Continue For
                        Try
                            Trig2 = CType(Trig.TrigObj, MyTrigger2Shoe)
                        Catch ex As Exception
                            Trig2 = Nothing
                        End Try
                        If Trig2 Is Nothing Then Continue For
                        If Trig2.SubTrigger2 Then Continue For ' Only checking SubType 1 right now.
                        If Trig2.EuroVoltage Then
                            Log("Checking Voltage Trigger: " & Math.Round(VoltEuro.Voltage, GetDecimals(Trig2.TriggerValue)).ToString & " vs Trigger Value " & Trig2.TriggerValue.ToString, LogType.LOG_TYPE_INFO)
                            If Math.Round(VoltEuro.Voltage, GetDecimals(Trig2.TriggerValue)) = Trig2.TriggerValue Then
                                ' Step 3: If a trigger matches, call FIRE!
                                Log("Voltage trigger is TRUE - calling FIRE! for event ID " & TC.evRef.ToString, LogType.LOG_TYPE_WARNING)
                                callback.TriggerFire(IFACE_NAME, TC)
                            End If
                        Else
                            Log("Checking Voltage Trigger: " & Math.Round(Volt.Voltage, GetDecimals(Trig2.TriggerValue)).ToString & " vs Trigger Value " & Trig2.TriggerValue.ToString, LogType.LOG_TYPE_INFO)
                            If Math.Round(Volt.Voltage, GetDecimals(Trig2.TriggerValue)) = Trig2.TriggerValue Then
                                ' Step 3: If a trigger matches, call FIRE!
                                Log("Voltage trigger is TRUE - calling FIRE! for event ID " & TC.evRef.ToString, LogType.LOG_TYPE_WARNING)
                                callback.TriggerFire(IFACE_NAME, TC)
                            End If
                        End If
                    Next
                End If


                ' We have generated a new Voltage, so let's see if we need to trigger events.
                Try
                    ' Step 1: Ask HomeSeer for any triggers that are for this plug-in and are Type 2, SubType 2
                    ' (We did Type 2 SubType 1 up above)
                    TrigsToCheck = Nothing
                    TrigsToCheck = callback.TriggerMatches(IFACE_NAME, 2, 2)
                Catch ex As Exception
                End Try
                ' Step 2: If triggers were returned, we need to check them against our trigger value.
                If TrigsToCheck IsNot Nothing AndAlso TrigsToCheck.Count > 0 Then
                    For Each TC In TrigsToCheck
                        If TC.DataIn IsNot Nothing AndAlso TC.DataIn.Length > 10 Then
                            Trig = TriggerFromData(TC.DataIn)
                        Else
                            Trig = TriggerFromInfo(TC)
                        End If
                        If Not Trig.Result Then Continue For
                        If Trig.TrigObj Is Nothing Then Continue For
                        If Trig.WhichTrigger <> eTriggerType.TwoVolts Then Continue For
                        Try
                            Trig2 = CType(Trig.TrigObj, MyTrigger2Shoe)
                        Catch ex As Exception
                            Trig2 = Nothing
                        End Try
                        If Trig2 Is Nothing Then Continue For
                        If Not Trig2.SubTrigger2 Then Continue For ' Only checking SubType 2 right now.
                        If Trig2.EuroVoltage Then
                            Log("Checking Avg Voltage Trigger: " & Math.Round(VoltEuro.AverageVoltage, GetDecimals(Trig2.TriggerValue)).ToString & " vs Trigger Value " & Trig2.TriggerValue.ToString, LogType.LOG_TYPE_INFO)
                            If Math.Round(VoltEuro.AverageVoltage, GetDecimals(Trig2.TriggerValue)) = Trig2.TriggerValue Then
                                ' Step 3: If a trigger matches, call FIRE!
                                Log("Average Voltage trigger is TRUE - calling FIRE! for event ID " & TC.evRef.ToString, LogType.LOG_TYPE_WARNING)
                                callback.TriggerFire(IFACE_NAME, TC)
                            End If
                        Else
                            Log("Checking Avg Voltage Trigger: " & Math.Round(Volt.AverageVoltage, GetDecimals(Trig2.TriggerValue)).ToString & " vs Trigger Value " & Trig2.TriggerValue.ToString, LogType.LOG_TYPE_INFO)
                            If Math.Round(Volt.AverageVoltage, GetDecimals(Trig2.TriggerValue)) = Trig2.TriggerValue Then
                                ' Step 3: If a trigger matches, call FIRE!
                                Log("Average Voltage trigger is TRUE - calling FIRE! for event ID " & TC.evRef.ToString, LogType.LOG_TYPE_WARNING)
                                callback.TriggerFire(IFACE_NAME, TC)
                            End If
                        End If
                    Next
                End If


            Loop
        Catch ex As Exception

        End Try
    End Sub




    Enum LogType
        LOG_TYPE_INFO = 0
        LOG_TYPE_ERROR = 1
        LOG_TYPE_WARNING = 2
    End Enum

    Public Sub Log(ByVal msg As String, ByVal logType As LogType)
        Try
            If msg Is Nothing Then msg = ""
            If Not [Enum].IsDefined(GetType(LogType), logType) Then
                logType = Util.LogType.LOG_TYPE_ERROR
            End If
            Console.WriteLine(msg)
            Select Case logType
                Case logType.LOG_TYPE_ERROR
                    hs.WriteLog(IFACE_NAME & " Error", msg)
                Case logType.LOG_TYPE_WARNING
                    hs.WriteLog(IFACE_NAME & " Warning", msg)
                Case logType.LOG_TYPE_INFO
                    hs.WriteLog(IFACE_NAME, msg)
            End Select
        Catch ex As Exception
            Console.WriteLine("Exception in LOG of " & IFACE_NAME & ": " & ex.Message)
        End Try

    End Sub

    Friend Enum eTriggerType
        OneTon = 1
        TwoVolts = 2
        Unknown = 0
    End Enum
    Friend Enum eActionType
        Unknown = 0
        Weight = 1
        Voltage = 2
    End Enum

    Friend Structure strTrigger
        Public WhichTrigger As eTriggerType
        Public TrigObj As Object
        Public Result As Boolean
    End Structure
    Friend Structure strAction
        Public WhichAction As eActionType
        Public ActObj As Object
        Public Result As Boolean
    End Structure

    Friend Function TriggerFromData(ByRef Data As Byte()) As strTrigger
        Dim ST As New strTrigger
        ST.WhichTrigger = eTriggerType.Unknown
        ST.Result = False
        If Data Is Nothing Then Return ST
        If Data.Length < 1 Then Return ST

        Dim bRes As Boolean = False
        Dim Trig1 As New MyTrigger1Ton
        Dim Trig2 As New MyTrigger2Shoe
        Try
            bRes = DeSerializeObject(Data, Trig1)
        Catch ex As Exception
            bRes = False
        End Try
        If bRes And Trig1 IsNot Nothing Then
            ST.WhichTrigger = eTriggerType.OneTon
            ST.TrigObj = Trig1
            ST.Result = True
            Return ST
        End If
        Try
            bRes = DeSerializeObject(Data, Trig2)
        Catch ex As Exception
            bRes = False
        End Try
        If bRes And Trig2 IsNot Nothing Then
            ST.WhichTrigger = eTriggerType.TwoVolts
            ST.TrigObj = Trig2
            ST.Result = True
            Return ST
        End If
        ST.WhichTrigger = eTriggerType.Unknown
        ST.TrigObj = Nothing
        ST.Result = False
        Return ST
    End Function

    Friend Function ActionFromData(ByRef Data As Byte()) As strAction
        Dim ST As New strAction
        ST.WhichAction = eActionType.Unknown
        ST.Result = False
        If Data Is Nothing Then Return ST
        If Data.Length < 1 Then Return ST

        Dim bRes As Boolean = False
        Dim Act1 As New MyAction1EvenTon
        Dim Act2 As New MyAction2Euro
        Try
            bRes = DeSerializeObject(Data, Act1)
        Catch ex As Exception
            bRes = False
        End Try
        If bRes And Act1 IsNot Nothing Then
            ST.WhichAction = eActionType.Weight
            ST.ActObj = Act1
            ST.Result = True
            Return ST
        End If
        Try
            bRes = DeSerializeObject(Data, Act2)
        Catch ex As Exception
            bRes = False
        End Try
        If bRes And Act2 IsNot Nothing Then
            ST.WhichAction = eActionType.Voltage
            ST.ActObj = Act2
            ST.Result = True
            Return ST
        End If
        ST.WhichAction = eActionType.Unknown
        ST.ActObj = Nothing
        ST.Result = False
        Return ST
    End Function

    Sub Add_Update_Trigger(ByVal Trig As Object)
        If Trig Is Nothing Then Exit Sub
        Dim sKey As String = ""
        If TypeOf Trig Is MyTrigger1Ton Then
            Dim Trig1 As MyTrigger1Ton = Nothing
            Try
                Trig1 = CType(Trig, MyTrigger1Ton)
            Catch ex As Exception
                Trig1 = Nothing
            End Try
            If Trig1 IsNot Nothing Then
                If Trig1.TriggerUID < 1 Then Exit Sub
                sKey = "K" & Trig1.TriggerUID.ToString
                If colTrigs.ContainsKey(sKey) Then
                    SyncLock colTrigs.SyncRoot
                        colTrigs.Remove(sKey)
                    End SyncLock
                End If
                colTrigs.Add(sKey, Trig1)
            End If
        ElseIf TypeOf Trig Is MyTrigger2Shoe Then
            Dim Trig2 As MyTrigger2Shoe = Nothing
            Try
                Trig2 = CType(Trig, MyTrigger2Shoe)
            Catch ex As Exception
                Trig2 = Nothing
            End Try
            If Trig2 IsNot Nothing Then
                If Trig2.TriggerUID < 1 Then Exit Sub
                sKey = "K" & Trig2.TriggerUID.ToString
                If colTrigs.ContainsKey(sKey) Then
                    SyncLock colTrigs.SyncRoot
                        colTrigs.Remove(sKey)
                    End SyncLock
                End If
                colTrigs.Add(sKey, Trig2)
            End If
        End If
    End Sub

    Sub Add_Update_Action(ByVal Act As Object)
        If Act Is Nothing Then Exit Sub
        Dim sKey As String = ""
        If TypeOf Act Is MyAction1EvenTon Then
            Dim Act1 As MyAction1EvenTon = Nothing
            Try
                Act1 = CType(Act, MyAction1EvenTon)
            Catch ex As Exception
                Act1 = Nothing
            End Try
            If Act1 IsNot Nothing Then
                If Act1.ActionUID < 1 Then Exit Sub
                sKey = "K" & Act1.ActionUID.ToString
                If colActs.ContainsKey(sKey) Then
                    SyncLock colActs.SyncRoot
                        colActs.Remove(sKey)
                    End SyncLock
                End If
                colActs.Add(sKey, Act1)
            End If
        ElseIf TypeOf Act Is MyAction2Euro Then
            Dim Act2 As MyAction2Euro = Nothing
            Try
                Act2 = CType(Act, MyAction2Euro)
            Catch ex As Exception
                Act2 = Nothing
            End Try
            If Act2 IsNot Nothing Then
                If Act2.ActionUID < 1 Then Exit Sub
                sKey = "K" & Act2.ActionUID.ToString
                If colActs.ContainsKey(sKey) Then
                    SyncLock colActs.SyncRoot
                        colActs.Remove(sKey)
                    End SyncLock
                End If
                colActs.Add(sKey, Act2)
            End If
        End If
    End Sub

    Public MyDevice As Integer = -1
    Public MyTempDevice As Integer = -1

    Friend Sub Find_Create_Devices()
        Dim col As New Collections.Generic.List(Of Scheduler.Classes.DeviceClass)
        Dim dv As Scheduler.Classes.DeviceClass
        Dim Found As Boolean = False

        Try
            Dim EN As Scheduler.Classes.clsDeviceEnumeration
            EN = hs.GetDeviceEnumerator
            If EN Is Nothing Then Throw New Exception(IFACE_NAME & " failed to get a device enumerator from HomeSeer.")
            Do
                dv = EN.GetNext
                If dv Is Nothing Then Continue Do

                

                If dv.Interface(Nothing) IsNot Nothing Then
                    If dv.Interface(Nothing).Trim = IFACE_NAME Then
                        col.Add(dv)
                    End If
                End If
            Loop Until EN.Finished
        Catch ex As Exception
            hs.WriteLog(IFACE_NAME & " Error", "Exception in Find_Create_Devices/Enumerator: " & ex.Message)
        End Try

        Try
            Dim DT As DeviceTypeInfo = Nothing
            If col IsNot Nothing AndAlso col.Count > 0 Then
                For Each dv In col
                    If dv Is Nothing Then Continue For
                    If dv.Interface(hs) <> IFACE_NAME Then Continue For
                    DT = dv.DeviceType_Get(hs)
                    If DT IsNot Nothing Then
                        If DT.Device_API = DeviceTypeInfo.eDeviceAPI.Thermostat AndAlso DT.Device_Type = DeviceTypeInfo.eDeviceType_Thermostat.Temperature Then
                            ' this is our temp device
                            Found = True
                            MyTempDevice = dv.Ref(Nothing)
                            hs.SetDeviceValueByRef(dv.Ref(Nothing), 72, False)
                        End If

                        If DT.Device_API = DeviceTypeInfo.eDeviceAPI.Thermostat AndAlso DT.Device_Type = DeviceTypeInfo.eDeviceType_Thermostat.Setpoint Then
                            Found = True
                            If DT.Device_SubType = DeviceTypeInfo.eDeviceSubType_Setpoint.Heating_1 Then
                                hs.SetDeviceValueByRef(dv.Ref(Nothing), 68, False)
                            End If
                        End If

                        If DT.Device_API = DeviceTypeInfo.eDeviceAPI.Thermostat AndAlso DT.Device_Type = DeviceTypeInfo.eDeviceType_Thermostat.Setpoint Then
                            Found = True
                            If DT.Device_SubType = DeviceTypeInfo.eDeviceSubType_Setpoint.Cooling_1 Then
                                hs.SetDeviceValueByRef(dv.Ref(Nothing), 75, False)
                            End If
                        End If

                        If DT.Device_API = DeviceTypeInfo.eDeviceAPI.Plug_In AndAlso DT.Device_Type = 69 Then
                            Found = True
                            MyDevice = dv.Ref(Nothing)

                            ' Now (mostly for demonstration purposes) - work with the PlugExtraData object.
                            Dim EDO As HomeSeerAPI.clsPlugExtraData = Nothing
                            EDO = dv.PlugExtraData_Get(Nothing)
                            If EDO IsNot Nothing Then
                                Dim obj As Object = Nothing
                                obj = EDO.GetNamed("My Special Object")
                                If obj IsNot Nothing Then
                                    Log("Plug-In Extra Data Object Retrieved = " & obj.ToString, LogType.LOG_TYPE_INFO)
                                End If
                                obj = EDO.GetNamed("My Count")
                                Dim MC As Integer = 1
                                If obj Is Nothing Then
                                    If Not EDO.AddNamed("My Count", MC) Then
                                        Log("Error adding named data object to plug-in sample device!", LogType.LOG_TYPE_ERROR)
                                        Exit For
                                    End If
                                    dv.PlugExtraData_Set(hs) = EDO
                                    hs.SaveEventsDevices()
                                Else
                                    Try
                                        MC = Convert.ToInt32(obj)
                                    Catch ex As Exception
                                        MC = -1
                                    End Try
                                    If MC < 0 Then Exit For
                                    Log("Retrieved count from plug-in sample device is: " & MC.ToString, LogType.LOG_TYPE_INFO)
                                    MC += 1
                                    ' Now put it back - need to remove the old one first.
                                    EDO.RemoveNamed("My Count")
                                    EDO.AddNamed("My Count", MC)
                                    dv.PlugExtraData_Set(hs) = EDO
                                    hs.SaveEventsDevices()
                                End If
                            End If


                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            hs.WriteLog(IFACE_NAME & " Error", "Exception in Find_Create_Devices/Find: " & ex.Message)
        End Try

        Try
            If Not Found Then
                Dim ref As Integer
                Dim GPair As VGPair = Nothing
                ref = hs.NewDeviceRef("Sample Plugin Device with buttons and slider")
                If ref > 0 Then
                    MyDevice = ref
                    dv = hs.GetDeviceByRef(ref)
                    Dim disp(1) As String
                    disp(0) = "This is a test of the"
                    disp(1) = "Emergency Broadcast Display Data system"
                    dv.AdditionalDisplayData(hs) = disp
                    dv.Address(hs) = "HOME"
                    dv.Code(hs) = "A1"              ' set a code if needed, but not required
                    'dv.Can_Dim(hs) = True
                    dv.Device_Type_String(hs) = "My Sample Device"
                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Plug_In
                    DT.Device_Type = 69                                 ' our own device type
                    dv.DeviceType_Set(hs) = DT
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Last_Change(hs) = #5/21/1929 11:00:00 AM#
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"

                    Dim EDO As New HomeSeerAPI.clsPlugExtraData
                    dv.PlugExtraData_Set(hs) = EDO
                    ' Now just for grins, let's modify it.
                    Dim HW As String = "Hello World"
                    If EDO.GetNamed("My Special Object") IsNot Nothing Then
                        EDO.RemoveNamed("My Special Object")
                    End If
                    EDO.AddNamed("My Special Object", HW)
                    ' Need to re-save it.
                    dv.PlugExtraData_Set(hs) = EDO

                    ' add an ON button and value
                    Dim Pair As VSPair
                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 0
                    Pair.Status = "Off"
                    Pair.Render = Enums.CAPIControlType.Button
                    Pair.Render_Location.Row = 1
                    Pair.Render_Location.Column = 1
                    Pair.ControlUse = ePairControlUse._Off            ' set this for UI apps like HSTouch so they know this is for OFF
                    hs.DeviceVSP_AddPair(ref, Pair)
                    GPair = New VGPair
                    GPair.PairType = VSVGPairType.SingleValue
                    GPair.Set_Value = 0
                    GPair.Graphic = "/images/HomeSeer/status/off.gif"
                    dv.ImageLarge(hs) = "/images/browser.png"
                    hs.DeviceVGP_AddPair(ref, GPair)

                    ' add DIM values
                    Pair = New VSPair(ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.Range
                    Pair.ControlUse = ePairControlUse._Dim            ' set this for UI apps like HSTouch so they know this is for lighting control dimming
                    Pair.RangeStart = 1
                    Pair.RangeEnd = 99
                    Pair.RangeStatusPrefix = "Dim "
                    Pair.RangeStatusSuffix = "%"
                    Pair.Render = Enums.CAPIControlType.ValuesRangeSlider
                    Pair.Render_Location.Row = 2
                    Pair.Render_Location.Column = 1
                    Pair.Render_Location.ColumnSpan = 3
                    hs.DeviceVSP_AddPair(ref, Pair)
                    ' add graphic pairs for the dim levels
                    GPair = New VGPair()
                    GPair.PairType = VSVGPairType.Range
                    GPair.RangeStart = 1
                    GPair.RangeEnd = 5.99999999
                    GPair.Graphic = "/images/HomeSeer/status/dim-00.gif"
                    hs.DeviceVGP_AddPair(ref, GPair)

                    GPair = New VGPair()
                    GPair.PairType = VSVGPairType.Range
                    GPair.RangeStart = 6
                    GPair.RangeEnd = 15.99999999
                    GPair.Graphic = "/images/HomeSeer/status/dim-10.gif"
                    hs.DeviceVGP_AddPair(ref, GPair)

                    GPair = New VGPair()
                    GPair.PairType = VSVGPairType.Range
                    GPair.RangeStart = 16
                    GPair.RangeEnd = 25.99999999
                    GPair.Graphic = "/images/HomeSeer/status/dim-20.gif"
                    hs.DeviceVGP_AddPair(ref, GPair)

                    GPair = New VGPair()
                    GPair.PairType = VSVGPairType.Range
                    GPair.RangeStart = 26
                    GPair.RangeEnd = 35.99999999
                    GPair.Graphic = "/images/HomeSeer/status/dim-30.gif"
                    hs.DeviceVGP_AddPair(ref, GPair)

                    GPair = New VGPair()
                    GPair.PairType = VSVGPairType.Range
                    GPair.RangeStart = 36
                    GPair.RangeEnd = 45.99999999
                    GPair.Graphic = "/images/HomeSeer/status/dim-40.gif"
                    hs.DeviceVGP_AddPair(ref, GPair)

                    GPair = New VGPair()
                    GPair.PairType = VSVGPairType.Range
                    GPair.RangeStart = 46
                    GPair.RangeEnd = 55.99999999
                    GPair.Graphic = "/images/HomeSeer/status/dim-50.gif"
                    hs.DeviceVGP_AddPair(ref, GPair)

                    GPair = New VGPair()
                    GPair.PairType = VSVGPairType.Range
                    GPair.RangeStart = 56
                    GPair.RangeEnd = 65.99999999
                    GPair.Graphic = "/images/HomeSeer/status/dim-60.gif"
                    hs.DeviceVGP_AddPair(ref, GPair)

                    GPair = New VGPair()
                    GPair.PairType = VSVGPairType.Range
                    GPair.RangeStart = 66
                    GPair.RangeEnd = 75.99999999
                    GPair.Graphic = "/images/HomeSeer/status/dim-70.gif"
                    hs.DeviceVGP_AddPair(ref, GPair)

                    GPair = New VGPair()
                    GPair.PairType = VSVGPairType.Range
                    GPair.RangeStart = 76
                    GPair.RangeEnd = 85.99999999
                    GPair.Graphic = "/images/HomeSeer/status/dim-80.gif"
                    hs.DeviceVGP_AddPair(ref, GPair)

                    GPair = New VGPair()
                    GPair.PairType = VSVGPairType.Range
                    GPair.RangeStart = 86
                    GPair.RangeEnd = 95.99999999
                    GPair.Graphic = "/images/HomeSeer/status/dim-90.gif"
                    hs.DeviceVGP_AddPair(ref, GPair)

                    GPair = New VGPair()
                    GPair.PairType = VSVGPairType.Range
                    GPair.RangeStart = 96
                    GPair.RangeEnd = 98.99999999
                    GPair.Graphic = "/images/HomeSeer/status/on.gif"
                    hs.DeviceVGP_AddPair(ref, GPair)

                    ' add an OFF button and value
                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 100
                    Pair.Status = "On"
                    Pair.ControlUse = ePairControlUse._On            ' set this for UI apps like HSTouch so they know this is for lighting control ON
                    Pair.Render = Enums.CAPIControlType.Button
                    Pair.Render_Location.Row = 1
                    Pair.Render_Location.Column = 2
                    hs.DeviceVSP_AddPair(ref, Pair)
                    GPair = New VGPair
                    GPair.PairType = VSVGPairType.SingleValue
                    GPair.Set_Value = 100
                    GPair.Graphic = "/images/HomeSeer/status/on.gif"
                    hs.DeviceVGP_AddPair(ref, GPair)

                    ' add an last level button and value
                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 255
                    Pair.Status = "On Last Level"
                    Pair.Render = Enums.CAPIControlType.Button
                    Pair.Render_Location.Row = 1
                    Pair.Render_Location.Column = 3
                    hs.DeviceVSP_AddPair(ref, Pair)

                    ' add a button that executes a special command but does not actually set any device value, here we will speak something
                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Control)   ' set the type to CONTROL only so that this value will never be displayed as a status
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 1000                           ' we use a value that is not used as a status, this value will be handled in SetIOMult, see that function for the handling
                    Pair.Status = "Speak Hello"
                    Pair.Render = Enums.CAPIControlType.Button
                    Pair.Render_Location.Row = 1
                    Pair.Render_Location.Column = 3
                    hs.DeviceVSP_AddPair(ref, Pair)
                    



                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.MISC_Set(hs, Enums.dvMISC.NO_LOG)
                    'dv.MISC_Set(hs, Enums.dvMISC.STATUS_ONLY)      ' set this for a status only device, no controls, and do not include the DeviceVSP calls above
                    Dim PED As clsPlugExtraData = dv.PlugExtraData_Get(hs)
                    If PED Is Nothing Then PED = New clsPlugExtraData
                    PED.AddNamed("Test", New Boolean)
                    PED.AddNamed("Device", dv)
                    dv.PlugExtraData_Set(hs) = PED
                    dv.Status_Support(hs) = True
                    dv.UserNote(hs) = "This is my user note - how do you like it? This device is version " & dv.Version.ToString
                    'hs.SetDeviceString(ref, "Not Set", False)  ' this will override the name/value pairs
                End If

                ref = hs.NewDeviceRef("Sample Plugin Device with list values")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    'dv.Can_Dim(hs) = True
                    dv.Device_Type_String(hs) = "My Sample Device"
                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Plug_In
                    DT.Device_Type = 70
                    dv.DeviceType_Set(hs) = DT
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Last_Change(hs) = #5/21/1929 11:00:00 AM#
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"
                    Dim Pair As VSPair
                    ' add list values, will appear as drop list control
                    Pair = New VSPair(ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Render = Enums.CAPIControlType.Values
                    Pair.Value = 1
                    Pair.Status = "1"
                    hs.DeviceVSP_AddPair(ref, Pair)

                    Pair = New VSPair(ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Render = Enums.CAPIControlType.Values
                    Pair.Value = 2
                    Pair.Status = "2"
                    hs.DeviceVSP_AddPair(ref, Pair)

                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.MISC_Set(hs, Enums.dvMISC.NO_LOG)
                    'dv.MISC_Set(hs, Enums.dvMISC.STATUS_ONLY)      ' set this for a status only device, no controls, and do not include the DeviceVSP calls above
                    dv.Status_Support(hs) = True
                End If

                ref = hs.NewDeviceRef("Sample Plugin Device with radio type control")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    'dv.Can_Dim(hs) = True
                    dv.Device_Type_String(hs) = "My Sample Device"
                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Plug_In
                    DT.Device_Type = 71
                    dv.DeviceType_Set(hs) = DT
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Last_Change(hs) = #5/21/1929 11:00:00 AM#
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"
                    Dim Pair As VSPair
                    ' add values, will appear as a radio control and only allow one option to be selected at a time
                    Pair = New VSPair(ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Render = Enums.CAPIControlType.Radio_Option
                    Pair.Value = 1
                    Pair.Status = "Value 1"
                    hs.DeviceVSP_AddPair(ref, Pair)
                    Pair.Value = 2
                    Pair.Status = "Value 2"
                    hs.DeviceVSP_AddPair(ref, Pair)
                    Pair.Value = 3
                    Pair.Status = "Value 3"
                    hs.DeviceVSP_AddPair(ref, Pair)

                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.MISC_Set(hs, Enums.dvMISC.NO_LOG)
                    'dv.MISC_Set(hs, Enums.dvMISC.STATUS_ONLY)      ' set this for a status only device, no controls, and do not include the DeviceVSP calls above
                    dv.Status_Support(hs) = True
                End If

                ref = hs.NewDeviceRef("Sample Plugin Device with list text single selection")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    'dv.Can_Dim(hs) = True
                    dv.Device_Type_String(hs) = "My Sample Device"
                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Plug_In
                    DT.Device_Type = 72
                    dv.DeviceType_Set(hs) = DT
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Last_Change(hs) = #5/21/1929 11:00:00 AM#
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"
                    Dim Pair As VSPair
                    ' add list values, will appear as drop list control
                    Pair = New VSPair(ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Render = Enums.CAPIControlType.Single_Text_from_List
                    Pair.Value = 1
                    Pair.Status = "String 1"
                    hs.DeviceVSP_AddPair(ref, Pair)

                    Pair = New VSPair(ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Render = Enums.CAPIControlType.Single_Text_from_List
                    Pair.Value = 2
                    Pair.Status = "String 2"
                    hs.DeviceVSP_AddPair(ref, Pair)



                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.MISC_Set(hs, Enums.dvMISC.NO_LOG)
                    'dv.MISC_Set(hs, Enums.dvMISC.STATUS_ONLY)      ' set this for a status only device, no controls, and do not include the DeviceVSP calls above
                    dv.Status_Support(hs) = True
                End If

                ref = hs.NewDeviceRef("Sample Plugin Device with list text multiple selection")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    'dv.Can_Dim(hs) = True
                    dv.Device_Type_String(hs) = "My Sample Device"
                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Plug_In
                    DT.Device_Type = 73
                    dv.DeviceType_Set(hs) = DT
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Last_Change(hs) = #5/21/1929 11:00:00 AM#
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"
                    Dim Pair As VSPair
                    ' add list values, will appear as drop list control
                    Pair = New VSPair(ePairStatusControl.Control)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Render = Enums.CAPIControlType.List_Text_from_List
                    Pair.StringListAdd = "String 1"
                    Pair.StringListAdd = "String 2"
                    hs.DeviceVSP_AddPair(ref, Pair)

                   



                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.MISC_Set(hs, Enums.dvMISC.NO_LOG)
                    'dv.MISC_Set(hs, Enums.dvMISC.STATUS_ONLY)      ' set this for a status only device, no controls, and do not include the DeviceVSP calls above
                    dv.Status_Support(hs) = True
                End If

                ref = hs.NewDeviceRef("Sample Plugin Device with text box text")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    'dv.Can_Dim(hs) = True
                    dv.Device_Type_String(hs) = "Sample Device with textbox input"
                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Plug_In
                    DT.Device_Type = 74
                    dv.DeviceType_Set(hs) = DT
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Last_Change(hs) = #5/21/1929 11:00:00 AM#
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"
                    Dim Pair As VSPair
                    ' add text value it will appear in an editable text box
                    Pair = New VSPair(ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Render = Enums.CAPIControlType.TextBox_String
                    Pair.Value = 0
                    Pair.Status = "Default Text"
                    hs.DeviceVSP_AddPair(ref, Pair)

                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.MISC_Set(hs, Enums.dvMISC.NO_LOG)
                    'dv.MISC_Set(hs, Enums.dvMISC.STATUS_ONLY)      ' set this for a status only device, no controls, and do not include the DeviceVSP calls above
                    dv.Status_Support(hs) = True
                End If

                ref = hs.NewDeviceRef("Sample Plugin Device with text box number")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    'dv.Can_Dim(hs) = True
                    dv.Device_Type_String(hs) = "Sample Device with textbox input"
                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Plug_In
                    DT.Device_Type = 75
                    dv.DeviceType_Set(hs) = DT
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Last_Change(hs) = #5/21/1929 11:00:00 AM#
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"
                    Dim Pair As VSPair
                    ' add text value it will appear in an editable text box
                    Pair = New VSPair(ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Render = Enums.CAPIControlType.TextBox_Number
                    Pair.Value = 0
                    Pair.Status = "Default Number"
                    Pair.Value = 0
                    hs.DeviceVSP_AddPair(ref, Pair)

                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.MISC_Set(hs, Enums.dvMISC.NO_LOG)
                    'dv.MISC_Set(hs, Enums.dvMISC.STATUS_ONLY)      ' set this for a status only device, no controls, and do not include the DeviceVSP calls above
                    dv.Status_Support(hs) = True
                End If

                ' this demonstrates some controls that are displayed in a pop-up dialog on the device utility page
                ' this device is also set so the values/graphics pairs cannot be edited and no graphics displays for the status
                ref = hs.NewDeviceRef("Sample Plugin Device with pop-up control")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    'dv.Can_Dim(hs) = True
                    dv.Device_Type_String(hs) = "My Sample Device"
                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Plug_In
                    DT.Device_Type = 76
                    dv.DeviceType_Set(hs) = DT
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Last_Change(hs) = #5/21/1929 11:00:00 AM#
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"


                    
                    Dim Pair As VSPair
                    ' add an OFF button and value
                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 0
                    Pair.Status = "Off"
                    Pair.Render = Enums.CAPIControlType.Button
                    hs.DeviceVSP_AddPair(ref, Pair)

                    ' add an ON button and value

                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 100
                    Pair.Status = "On"
                    Pair.Render = Enums.CAPIControlType.Button
                    hs.DeviceVSP_AddPair(ref, Pair)

                    ' add DIM values
                    Pair = New VSPair(ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.Range
                    Pair.RangeStart = 1
                    Pair.RangeEnd = 99
                    Pair.RangeStatusPrefix = "Dim "
                    Pair.RangeStatusSuffix = "%"
                    Pair.Render = Enums.CAPIControlType.ValuesRangeSlider

                    hs.DeviceVSP_AddPair(ref, Pair)

                    dv.MISC_Set(hs, Enums.dvMISC.CONTROL_POPUP)     ' cause control to be displayed in a pop-up dialog
                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.MISC_Set(hs, Enums.dvMISC.NO_LOG)

                    dv.Status_Support(hs) = True
                End If

                ' this is a device that pop-ups and uses row/column attributes to position the controls on the form
                ref = hs.NewDeviceRef("Sample Plugin Device with pop-up control row/column")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    dv.Device_Type_String(hs) = "My Sample Device"
                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Plug_In
                    DT.Device_Type = 77
                    dv.DeviceType_Set(hs) = DT
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Last_Change(hs) = #5/21/1929 11:00:00 AM#
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"


                    ' add an array of buttons formatted like a number pad
                    Dim Pair As VSPair
                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 1
                    Pair.Status = "1"
                    Pair.Render = Enums.CAPIControlType.Button
                    Pair.Render_Location.Column = 1
                    Pair.Render_Location.Row = 1
                    hs.DeviceVSP_AddPair(ref, Pair)

                    Pair.Value = 2 : Pair.Status = "2" : Pair.Render_Location.Column = 2 : Pair.Render_Location.Row = 1 : hs.DeviceVSP_AddPair(ref, Pair)
                    Pair.Value = 3 : Pair.Status = "3" : Pair.Render_Location.Column = 3 : Pair.Render_Location.Row = 1 : hs.DeviceVSP_AddPair(ref, Pair)
                    Pair.Value = 4 : Pair.Status = "4" : Pair.Render_Location.Column = 1 : Pair.Render_Location.Row = 2 : hs.DeviceVSP_AddPair(ref, Pair)
                    Pair.Value = 5 : Pair.Status = "5" : Pair.Render_Location.Column = 2 : Pair.Render_Location.Row = 2 : hs.DeviceVSP_AddPair(ref, Pair)
                    Pair.Value = 6 : Pair.Status = "6" : Pair.Render_Location.Column = 3 : Pair.Render_Location.Row = 2 : hs.DeviceVSP_AddPair(ref, Pair)
                    Pair.Value = 7 : Pair.Status = "7" : Pair.Render_Location.Column = 1 : Pair.Render_Location.Row = 3 : hs.DeviceVSP_AddPair(ref, Pair)
                    Pair.Value = 8 : Pair.Status = "8" : Pair.Render_Location.Column = 2 : Pair.Render_Location.Row = 3 : hs.DeviceVSP_AddPair(ref, Pair)
                    Pair.Value = 9 : Pair.Status = "9" : Pair.Render_Location.Column = 3 : Pair.Render_Location.Row = 3 : hs.DeviceVSP_AddPair(ref, Pair)
                    Pair.Value = 10 : Pair.Status = "*" : Pair.Render_Location.Column = 1 : Pair.Render_Location.Row = 4 : hs.DeviceVSP_AddPair(ref, Pair)
                    Pair.Value = 0 : Pair.Status = "0" : Pair.Render_Location.Column = 2 : Pair.Render_Location.Row = 4 : hs.DeviceVSP_AddPair(ref, Pair)
                    Pair.Value = 11 : Pair.Status = "#" : Pair.Render_Location.Column = 3 : Pair.Render_Location.Row = 4 : hs.DeviceVSP_AddPair(ref, Pair)
                    Pair.Value = 12 : Pair.Status = "Clear" : Pair.Render_Location.Column = 1 : Pair.Render_Location.Row = 5 : Pair.Render_Location.ColumnSpan = 3 : hs.DeviceVSP_AddPair(ref, Pair)

                    dv.MISC_Set(hs, Enums.dvMISC.CONTROL_POPUP)     ' cause control to be displayed in a pop-up dialog
                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.MISC_Set(hs, Enums.dvMISC.NO_LOG)

                    dv.Status_Support(hs) = True
                End If

                ' this device is created so that no graphics are displayed and the value/graphics pairs cannot be edited
                ref = hs.NewDeviceRef("Sample Plugin Device no graphics")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    'dv.Can_Dim(hs) = True
                    dv.Device_Type_String(hs) = "My Sample Device"
                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Plug_In
                    DT.Device_Type = 76
                    dv.DeviceType_Set(hs) = DT
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Last_Change(hs) = #5/21/1929 11:00:00 AM#
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"

                    dv.MISC_Set(hs, Enums.dvMISC.NO_GRAPHICS_DISPLAY)    ' causes no graphics to display and value/graphics pairs cannot be edited
                    dv.MISC_Set(hs, Enums.dvMISC.CONTROL_POPUP)     ' cause control to be displayed in a pop-up dialog
                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.MISC_Set(hs, Enums.dvMISC.NO_LOG)

                    dv.Status_Support(hs) = True
                End If

                ref = hs.NewDeviceRef("Sample Plugin Device with color control")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    'dv.Can_Dim(hs) = True
                    dv.Device_Type_String(hs) = "My Sample Device"
                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Plug_In
                    DT.Device_Type = 76
                    dv.DeviceType_Set(hs) = DT
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Last_Change(hs) = #5/21/1929 11:00:00 AM#
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"

                    Dim Pair As VSPair
                    Pair = New VSPair(ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.Range
                    Pair.RangeStart = 1
                    Pair.RangeEnd = 99
                    Pair.RangeStatusPrefix = ""
                    Pair.RangeStatusSuffix = ""
                    Pair.Render = Enums.CAPIControlType.Color_Picker

                    hs.DeviceVSP_AddPair(ref, Pair)
                    
                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)


                    dv.Status_Support(hs) = True
                End If

                ' build a thermostat device group,all of the following thermostat devices are grouped under this root device
                gGlobalTempScaleF = Convert.ToBoolean(hs.GetINISetting("Settings", "gGlobalTempScaleF", "True").Trim)   ' get the F or C setting from HS setup
                Dim therm_root_dv As Scheduler.Classes.DeviceClass = Nothing
                ref = hs.NewDeviceRef("Sample Plugin Thermostat Root Device")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    therm_root_dv = dv
                    dv.Address(hs) = "HOME"
                    dv.Device_Type_String(hs) = "Z-Wave Thermostat"     ' this device type is set up in the default HSTouch projects so we set it here so the default project displays
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"

                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Thermostat
                    DT.Device_Type = DeviceTypeInfo.eDeviceType_Thermostat.Root
                    DT.Device_SubType = 0
                    DT.Device_SubType_Description = ""
                    dv.DeviceType_Set(hs) = DT
                    dv.MISC_Set(hs, Enums.dvMISC.STATUS_ONLY)
                    dv.Relationship(hs) = Enums.eRelationship.Parent_Root

                    hs.SaveEventsDevices()
                End If

                ref = hs.NewDeviceRef("Sample Plugin Thermostat Fan Device")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    dv.Device_Type_String(hs) = "Thermostat Fan"
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"

                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Thermostat
                    DT.Device_Type = DeviceTypeInfo.eDeviceType_Thermostat.Fan_Mode_Set
                    DT.Device_SubType = 0
                    DT.Device_SubType_Description = ""
                    dv.DeviceType_Set(hs) = DT
                    dv.Relationship(hs) = Enums.eRelationship.Child
                    If therm_root_dv IsNot Nothing Then
                        therm_root_dv.AssociatedDevice_Add(hs, ref)
                    End If
                    dv.AssociatedDevice_Add(hs, therm_root_dv.Ref(hs))

                    Dim Pair As VSPair
                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 0
                    Pair.Status = "Auto"
                    Pair.Render = Enums.CAPIControlType.Button
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)

                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 1
                    Pair.Status = "On"
                    Pair.Render = Enums.CAPIControlType.Button
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)

                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.Status_Support(hs) = True
                    hs.SaveEventsDevices()
                End If

                ref = hs.NewDeviceRef("Sample Plugin Thermostat Mode Device")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    dv.Device_Type_String(hs) = "Thermostat Mode"
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"

                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Thermostat
                    DT.Device_Type = DeviceTypeInfo.eDeviceType_Thermostat.Operating_Mode
                    DT.Device_SubType = 0
                    DT.Device_SubType_Description = ""
                    dv.DeviceType_Set(hs) = DT
                    dv.Relationship(hs) = Enums.eRelationship.Child
                    If therm_root_dv IsNot Nothing Then
                        therm_root_dv.AssociatedDevice_Add(hs, ref)
                    End If
                    dv.AssociatedDevice_Add(hs, therm_root_dv.Ref(hs))

                    Dim Pair As VSPair
                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 0
                    Pair.Status = "Off"
                    Pair.Render = Enums.CAPIControlType.Button
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)

                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 1
                    Pair.Status = "Heat"
                    Pair.Render = Enums.CAPIControlType.Button
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)
                    GPair = New VGPair()
                    GPair.PairType = VSVGPairType.SingleValue
                    GPair.Set_Value = 1
                    GPair.Graphic = "/images/HomeSeer/status/Heat.png"
                    Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 2
                    Pair.Status = "Cool"
                    Pair.Render = Enums.CAPIControlType.Button
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)
                    GPair = New VGPair()
                    GPair.PairType = VSVGPairType.SingleValue
                    GPair.Set_Value = 2
                    GPair.Graphic = "/images/HomeSeer/status/Cool.png"
                    Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Both)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 3
                    Pair.Status = "Auto"
                    Pair.Render = Enums.CAPIControlType.Button
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)
                    GPair = New VGPair()
                    GPair.PairType = VSVGPairType.SingleValue
                    GPair.Set_Value = 3
                    GPair.Graphic = "/images/HomeSeer/status/Auto.png"
                    Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.Status_Support(hs) = True
                    hs.SaveEventsDevices()
                End If

                ref = hs.NewDeviceRef("Sample Plugin Thermostat Heat Setpoint")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    dv.Device_Type_String(hs) = "Thermostat Heat Setpoint"
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"

                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Thermostat
                    DT.Device_Type = DeviceTypeInfo.eDeviceType_Thermostat.Setpoint
                    DT.Device_SubType = DeviceTypeInfo.eDeviceSubType_Setpoint.Heating_1
                    DT.Device_SubType_Description = ""
                    dv.DeviceType_Set(hs) = DT
                    dv.Relationship(hs) = Enums.eRelationship.Child
                    If therm_root_dv IsNot Nothing Then
                        therm_root_dv.AssociatedDevice_Add(hs, ref)
                    End If
                    dv.AssociatedDevice_Add(hs, therm_root_dv.Ref(hs))

                    Dim Pair As VSPair
                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Status)
                    Pair.PairType = VSVGPairType.Range
                    Pair.RangeStart = -2147483648
                    Pair.RangeEnd = 2147483647
                    Pair.RangeStatusPrefix = ""
                    Pair.RangeStatusSuffix = " " & VSPair.ScaleReplace
                    Pair.IncludeValues = True
                    Pair.ValueOffset = 0
                    Pair.RangeStatusDecimals = 0
                    Pair.HasScale = True
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)

                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Control)
                    Pair.PairType = VSVGPairType.Range
                    ' 39F = 4C
                    ' 50F = 10C
                    ' 90F = 32C
                    If gGlobalTempScaleF Then
                        Pair.RangeStart = 50
                        Pair.RangeEnd = 90
                    Else
                        Pair.RangeStart = 10
                        Pair.RangeEnd = 32
                    End If
                    Pair.RangeStatusPrefix = ""
                    Pair.RangeStatusSuffix = " " & VSPair.ScaleReplace
                    Pair.IncludeValues = True
                    Pair.ValueOffset = 0
                    Pair.RangeStatusDecimals = 0
                    Pair.HasScale = True
                    Pair.Render = Enums.CAPIControlType.TextBox_Number
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)

                    ' The scale does not matter because the global temperature scale setting
                    '   will override and cause the temperature to always display in the user's
                    '   selected scale, so use that in setting up the ranges.
                    'If dv.ZWData.Sensor_Scale = 1 Then  ' Fahrenheit
                    If gGlobalTempScaleF Then
                        ' Set up the ranges for Fahrenheit
                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = -50
                        GPair.RangeEnd = 5
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-00.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 5.00000001
                        GPair.RangeEnd = 15.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-10.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 16
                        GPair.RangeEnd = 25.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-20.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 26
                        GPair.RangeEnd = 35.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-30.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 36
                        GPair.RangeEnd = 45.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-40.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 46
                        GPair.RangeEnd = 55.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-50.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 56
                        GPair.RangeEnd = 65.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-60.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 66
                        GPair.RangeEnd = 75.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-70.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 76
                        GPair.RangeEnd = 85.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-80.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 86
                        GPair.RangeEnd = 95.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-90.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 96
                        GPair.RangeEnd = 104.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-100.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 105
                        GPair.RangeEnd = 150.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-110.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                    Else
                        ' Celsius
                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = -45
                        GPair.RangeEnd = -15
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-00.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = -14.999999
                        GPair.RangeEnd = -9.44
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-10.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = -9.43999999
                        GPair.RangeEnd = -3.88
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-20.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = -3.8799999
                        GPair.RangeEnd = 1.66
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-30.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 1.67
                        GPair.RangeEnd = 7.22
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-40.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 7.23
                        GPair.RangeEnd = 12.77
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-50.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 12.78
                        GPair.RangeEnd = 18.33
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-60.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 18.34
                        GPair.RangeEnd = 23.88
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-70.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 23.89
                        GPair.RangeEnd = 29.44
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-80.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 29.45
                        GPair.RangeEnd = 35
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-90.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 35.0000001
                        GPair.RangeEnd = 40
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-100.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 40.0000001
                        GPair.RangeEnd = 75
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-110.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                    End If

                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.Status_Support(hs) = True
                    hs.SaveEventsDevices()
                End If

                ref = hs.NewDeviceRef("Sample Plugin Thermostat Cool Setpoint")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    dv.Device_Type_String(hs) = "Thermostat Cool Setpoint"
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"

                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Thermostat
                    DT.Device_Type = DeviceTypeInfo.eDeviceType_Thermostat.Setpoint
                    DT.Device_SubType = DeviceTypeInfo.eDeviceSubType_Setpoint.Cooling_1
                    DT.Device_SubType_Description = ""
                    dv.DeviceType_Set(hs) = DT
                    dv.Relationship(hs) = Enums.eRelationship.Child
                    If therm_root_dv IsNot Nothing Then
                        therm_root_dv.AssociatedDevice_Add(hs, ref)
                    End If
                    dv.AssociatedDevice_Add(hs, therm_root_dv.Ref(hs))

                    Dim Pair As VSPair
                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Status)
                    Pair.PairType = VSVGPairType.Range
                    Pair.RangeStart = -2147483648
                    Pair.RangeEnd = 2147483647
                    Pair.RangeStatusPrefix = ""
                    Pair.RangeStatusSuffix = " " & VSPair.ScaleReplace
                    Pair.IncludeValues = True
                    Pair.ValueOffset = 0
                    Pair.RangeStatusDecimals = 0
                    Pair.HasScale = True
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)

                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Control)
                    Pair.PairType = VSVGPairType.Range
                    ' 39F = 4C
                    ' 50F = 10C
                    ' 90F = 32C
                    If gGlobalTempScaleF Then
                        Pair.RangeStart = 50
                        Pair.RangeEnd = 90
                    Else
                        Pair.RangeStart = 10
                        Pair.RangeEnd = 32
                    End If
                    Pair.RangeStatusPrefix = ""
                    Pair.RangeStatusSuffix = " " & VSPair.ScaleReplace
                    Pair.IncludeValues = True
                    Pair.ValueOffset = 0
                    Pair.RangeStatusDecimals = 0
                    Pair.HasScale = True
                    Pair.Render = Enums.CAPIControlType.TextBox_Number
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)

                    ' The scale does not matter because the global temperature scale setting
                    '   will override and cause the temperature to always display in the user's
                    '   selected scale, so use that in setting up the ranges.
                    'If dv.ZWData.Sensor_Scale = 1 Then  ' Fahrenheit
                    If gGlobalTempScaleF Then
                        ' Set up the ranges for Fahrenheit
                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = -50
                        GPair.RangeEnd = 5
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-00.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 5.00000001
                        GPair.RangeEnd = 15.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-10.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 16
                        GPair.RangeEnd = 25.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-20.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 26
                        GPair.RangeEnd = 35.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-30.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 36
                        GPair.RangeEnd = 45.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-40.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 46
                        GPair.RangeEnd = 55.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-50.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 56
                        GPair.RangeEnd = 65.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-60.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 66
                        GPair.RangeEnd = 75.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-70.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 76
                        GPair.RangeEnd = 85.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-80.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 86
                        GPair.RangeEnd = 95.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-90.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 96
                        GPair.RangeEnd = 104.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-100.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 105
                        GPair.RangeEnd = 150.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-110.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                    Else
                        ' Celsius
                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = -45
                        GPair.RangeEnd = -15
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-00.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = -14.999999
                        GPair.RangeEnd = -9.44
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-10.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = -9.43999999
                        GPair.RangeEnd = -3.88
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-20.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = -3.8799999
                        GPair.RangeEnd = 1.66
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-30.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 1.67
                        GPair.RangeEnd = 7.22
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-40.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 7.23
                        GPair.RangeEnd = 12.77
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-50.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 12.78
                        GPair.RangeEnd = 18.33
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-60.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 18.34
                        GPair.RangeEnd = 23.88
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-70.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 23.89
                        GPair.RangeEnd = 29.44
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-80.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 29.45
                        GPair.RangeEnd = 35
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-90.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 35.0000001
                        GPair.RangeEnd = 40
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-100.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 40.0000001
                        GPair.RangeEnd = 75
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-110.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                    End If

                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.Status_Support(hs) = True
                    hs.SaveEventsDevices()
                End If
                ref = hs.NewDeviceRef("Sample Plugin Thermostat Temp")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    dv.Device_Type_String(hs) = "Thermostat Temp"
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"

                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Thermostat
                    DT.Device_Type = DeviceTypeInfo.eDeviceType_Thermostat.Temperature
                    DT.Device_SubType = 1   ' temp
                    DT.Device_SubType_Description = ""
                    dv.DeviceType_Set(hs) = DT
                    dv.Relationship(hs) = Enums.eRelationship.Child
                    If therm_root_dv IsNot Nothing Then
                        therm_root_dv.AssociatedDevice_Add(hs, ref)
                    End If
                    dv.AssociatedDevice_Add(hs, therm_root_dv.Ref(hs))

                    Dim Pair As VSPair
                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Status)
                    Pair.PairType = VSVGPairType.Range
                    Pair.RangeStart = -2147483648
                    Pair.RangeEnd = 2147483647
                    Pair.RangeStatusPrefix = ""
                    Pair.RangeStatusSuffix = " " & VSPair.ScaleReplace
                    Pair.IncludeValues = True
                    Pair.ValueOffset = 0
                    Pair.HasScale = True
                    Pair.RangeStatusDecimals = 0
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)

                    If gGlobalTempScaleF Then
                        ' Set up the ranges for Fahrenheit
                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = -50
                        GPair.RangeEnd = 5
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-00.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 5.00000001
                        GPair.RangeEnd = 15.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-10.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 16
                        GPair.RangeEnd = 25.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-20.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 26
                        GPair.RangeEnd = 35.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-30.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range
                        GPair.RangeStart = 36
                        GPair.RangeEnd = 45.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-40.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 46
                        GPair.RangeEnd = 55.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-50.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 56
                        GPair.RangeEnd = 65.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-60.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 66
                        GPair.RangeEnd = 75.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-70.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 76
                        GPair.RangeEnd = 85.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-80.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 86
                        GPair.RangeEnd = 95.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-90.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 96
                        GPair.RangeEnd = 104.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-100.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 105
                        GPair.RangeEnd = 150.99999999
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-110.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                    Else
                        ' Celsius
                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = -45
                        GPair.RangeEnd = -15
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-00.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = -14.999999
                        GPair.RangeEnd = -9.44
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-10.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = -9.43999999
                        GPair.RangeEnd = -3.88
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-20.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = -3.8799999
                        GPair.RangeEnd = 1.66
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-30.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 1.67
                        GPair.RangeEnd = 7.22
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-40.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 7.23
                        GPair.RangeEnd = 12.77
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-50.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 12.78
                        GPair.RangeEnd = 18.33
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-60.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 18.34
                        GPair.RangeEnd = 23.88
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-70.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 23.89
                        GPair.RangeEnd = 29.44
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-80.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 29.45
                        GPair.RangeEnd = 35
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-90.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 35.0000001
                        GPair.RangeEnd = 40
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-100.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        GPair = New VGPair()
                        GPair.PairType = VSVGPairType.Range

                        GPair.RangeStart = 40.0000001
                        GPair.RangeEnd = 75
                        GPair.Graphic = "/images/HomeSeer/status/Thermometer-110.png"
                        Default_VG_Pairs_AddUpdateUtil(ref, GPair)

                        dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                        dv.Status_Support(hs) = True
                        hs.SaveEventsDevices()
                    End If
                End If


                ref = hs.NewDeviceRef("Sample Plugin Thermostat Fan State")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    dv.Device_Type_String(hs) = "Thermostat Fan State"
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"

                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Thermostat
                    DT.Device_Type = DeviceTypeInfo.eDeviceType_Thermostat.Fan_Status
                    DT.Device_SubType = 0
                    DT.Device_SubType_Description = ""
                    dv.DeviceType_Set(hs) = DT
                    dv.Relationship(hs) = Enums.eRelationship.Child
                    If therm_root_dv IsNot Nothing Then
                        therm_root_dv.AssociatedDevice_Add(hs, ref)
                    End If
                    dv.AssociatedDevice_Add(hs, therm_root_dv.Ref(hs))

                    Dim Pair As VSPair
                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Status)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 0
                    Pair.Status = "Off"
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)

                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Status)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 1
                    Pair.Status = "On"
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)

                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.Status_Support(hs) = True
                    hs.SaveEventsDevices()
                End If

                ref = hs.NewDeviceRef("Sample Plugin Thermostat Mode Status")
                If ref > 0 Then
                    dv = hs.GetDeviceByRef(ref)
                    dv.Address(hs) = "HOME"
                    dv.Device_Type_String(hs) = "Thermostat Mode Status"
                    dv.Interface(hs) = IFACE_NAME
                    dv.InterfaceInstance(hs) = ""
                    dv.Location(hs) = IFACE_NAME
                    dv.Location2(hs) = "Sample Devices"

                    Dim DT As New DeviceTypeInfo
                    DT.Device_API = DeviceTypeInfo.eDeviceAPI.Thermostat
                    DT.Device_Type = DeviceTypeInfo.eDeviceType_Thermostat.Operating_State
                    DT.Device_SubType = 0
                    DT.Device_SubType_Description = ""
                    dv.DeviceType_Set(hs) = DT
                    dv.Relationship(hs) = Enums.eRelationship.Child
                    If therm_root_dv IsNot Nothing Then
                        therm_root_dv.AssociatedDevice_Add(hs, ref)
                    End If
                    dv.AssociatedDevice_Add(hs, therm_root_dv.Ref(hs))

                    Dim Pair As VSPair
                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Status)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 0
                    Pair.Status = "Idle"
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)

                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Status)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 1
                    Pair.Status = "Heating"
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)

                    Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Status)
                    Pair.PairType = VSVGPairType.SingleValue
                    Pair.Value = 2
                    Pair.Status = "Cooling"
                    Default_VS_Pairs_AddUpdateUtil(ref, Pair)

                    dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
                    dv.Status_Support(hs) = True
                    hs.SaveEventsDevices()
                End If
            End If

        Catch ex As Exception
            hs.WriteLog(IFACE_NAME & " Error", "Exception in Find_Create_Devices/Create: " & ex.Message)
        End Try

    End Sub

    Private Sub Default_VG_Pairs_AddUpdateUtil(ByVal dvRef As Integer, ByRef Pair As VGPair)
        If Pair Is Nothing Then Exit Sub
        If dvRef < 1 Then Exit Sub
        If Not hs.DeviceExistsRef(dvRef) Then Exit Sub

        Dim Existing As VGPair = Nothing

        ' The purpose of this procedure is to add the protected, default VS/VG pairs WITHOUT overwriting any user added
        '   pairs unless absolutely necessary (because they conflict).

        Try
            Existing = hs.DeviceVGP_Get(dvRef, Pair.Value) 'VGPairs.GetPairByValue(Pair.Value)

            If Existing IsNot Nothing Then
                hs.DeviceVGP_Clear(dvRef, Pair.Value)
                hs.DeviceVGP_AddPair(dvRef, Pair)
            Else
                ' There is not a pair existing, so just add it.
                hs.DeviceVGP_AddPair(dvRef, Pair)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub Default_VS_Pairs_AddUpdateUtil(ByVal dvRef As Integer, ByRef Pair As VSPair)
        If Pair Is Nothing Then Exit Sub
        If dvRef < 1 Then Exit Sub
        If Not hs.DeviceExistsRef(dvRef) Then Exit Sub

        Dim Existing As VSPair = Nothing

        ' The purpose of this procedure is to add the protected, default VS/VG pairs WITHOUT overwriting any user added
        '   pairs unless absolutely necessary (because they conflict).

        Try
            Existing = hs.DeviceVSP_Get(dvRef, Pair.Value, Pair.ControlStatus) 'VSPairs.GetPairByValue(Pair.Value, Pair.ControlStatus)

            If Existing IsNot Nothing Then

                ' This is unprotected, so it is a user's value/status pair.
                If Existing.ControlStatus = HomeSeerAPI.ePairStatusControl.Both And Pair.ControlStatus <> HomeSeerAPI.ePairStatusControl.Both Then
                    ' The existing one is for BOTH, so try changing it to the opposite of what we are adding and then add it.
                    If Pair.ControlStatus = HomeSeerAPI.ePairStatusControl.Status Then
                        If Not hs.DeviceVSP_ChangePair(dvRef, Existing, HomeSeerAPI.ePairStatusControl.Control) Then
                            hs.DeviceVSP_ClearBoth(dvRef, Pair.Value)
                            hs.DeviceVSP_AddPair(dvRef, Pair)
                        Else
                            hs.DeviceVSP_AddPair(dvRef, Pair)
                        End If
                    Else
                        If Not hs.DeviceVSP_ChangePair(dvRef, Existing, HomeSeerAPI.ePairStatusControl.Status) Then
                            hs.DeviceVSP_ClearBoth(dvRef, Pair.Value)
                            hs.DeviceVSP_AddPair(dvRef, Pair)
                        Else
                            hs.DeviceVSP_AddPair(dvRef, Pair)
                        End If
                    End If
                ElseIf Existing.ControlStatus = HomeSeerAPI.ePairStatusControl.Control Then
                    ' There is an existing one that is STATUS or CONTROL - remove it if ours is protected.
                    hs.DeviceVSP_ClearControl(dvRef, Pair.Value)
                    hs.DeviceVSP_AddPair(dvRef, Pair)

                ElseIf Existing.ControlStatus = HomeSeerAPI.ePairStatusControl.Status Then
                    ' There is an existing one that is STATUS or CONTROL - remove it if ours is protected.
                    hs.DeviceVSP_ClearStatus(dvRef, Pair.Value)
                    hs.DeviceVSP_AddPair(dvRef, Pair)

                End If

            Else
                ' There is not a pair existing, so just add it.
                hs.DeviceVSP_AddPair(dvRef, Pair)

            End If

        Catch ex As Exception

        End Try
    End Sub
    Private Sub CreateOneDevice(dev_name As String)
        Dim ref As Integer
        Dim dv As Scheduler.Classes.DeviceClass

        ref = hs.NewDeviceRef(dev_name)
        Console.WriteLine("Creating device named: " & dev_name)
        If ref > 0 Then
            dv = hs.GetDeviceByRef(ref)
            dv.Address(hs) = "HOME"
            'dv.Can_Dim(hs) = True
            dv.Device_Type_String(hs) = "My Sample Device"
            Dim DT As New DeviceTypeInfo
            DT.Device_API = DeviceTypeInfo.eDeviceAPI.Plug_In
            DT.Device_Type = 69
            dv.DeviceType_Set(hs) = DT
            dv.Interface(hs) = IFACE_NAME
            dv.InterfaceInstance(hs) = ""
            dv.Last_Change(hs) = #5/21/1929 11:00:00 AM#
            dv.Location(hs) = IFACE_NAME
            dv.Location2(hs) = "Sample Devices"


            ' add an ON button and value
            Dim Pair As VSPair
            Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Both)
            Pair.PairType = VSVGPairType.SingleValue
            Pair.Value = 100
            Pair.Status = "On"
            Pair.Render = Enums.CAPIControlType.Button
            hs.DeviceVSP_AddPair(ref, Pair)

            ' add an OFF button and value
            Pair = New VSPair(HomeSeerAPI.ePairStatusControl.Both)
            Pair.PairType = VSVGPairType.SingleValue
            Pair.Value = 0
            Pair.Status = "Off"
            Pair.Render = Enums.CAPIControlType.Button
            hs.DeviceVSP_AddPair(ref, Pair)

            ' add DIM values
            Pair = New VSPair(ePairStatusControl.Both)
            Pair.PairType = VSVGPairType.Range
            Pair.RangeStart = 1
            Pair.RangeEnd = 99
            Pair.RangeStatusPrefix = "Dim "
            Pair.RangeStatusSuffix = "%"
            Pair.Render = Enums.CAPIControlType.ValuesRangeSlider

            hs.DeviceVSP_AddPair(ref, Pair)

            'dv.MISC_Set(hs, Enums.dvMISC.CONTROL_POPUP)     ' cause control to be displayed in a pop-up dialog
            dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
            dv.MISC_Set(hs, Enums.dvMISC.NO_LOG)

            dv.Status_Support(hs) = True
        End If
    End Sub

    Friend Function TriggerFromInfo(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As strTrigger
        Dim sKey As String = ""
        sKey = "K" & TrigInfo.UID.ToString
        If colTrigs IsNot Nothing Then
            If colTrigs.ContainsKey(sKey) Then
                Dim obj As Object = Nothing
                obj = colTrigs.Item(sKey)
                If obj IsNot Nothing Then
                    Dim Ret As strTrigger
                    Ret.Result = False
                    If TypeOf obj Is MyTrigger1Ton Then
                        Ret.WhichTrigger = eTriggerType.OneTon
                        Ret.TrigObj = obj
                        Ret.Result = True
                        Return Ret
                    ElseIf TypeOf obj Is MyTrigger2Shoe Then
                        Ret.WhichTrigger = eTriggerType.TwoVolts
                        Ret.TrigObj = obj
                        Ret.Result = True
                        Return Ret
                    End If
                End If
            End If
        End If
        Dim Bad As strTrigger
        Bad.WhichTrigger = eTriggerType.Unknown
        Bad.Result = False
        Bad.TrigObj = Nothing
        Return Bad
    End Function
    Friend Function ActionFromInfo(ByVal ActInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As strAction
        Dim sKey As String = ""
        sKey = "K" & ActInfo.UID.ToString
        If colActs IsNot Nothing Then
            If colActs.ContainsKey(sKey) Then
                Dim obj As Object = Nothing
                obj = colActs.Item(sKey)
                If obj IsNot Nothing Then
                    Dim Ret As strAction
                    Ret.Result = False
                    If TypeOf obj Is MyAction1EvenTon Then
                        Ret.WhichAction = eActionType.Weight
                        Ret.ActObj = obj
                        Ret.Result = True
                        Return Ret
                    ElseIf TypeOf obj Is MyAction2Euro Then
                        Ret.WhichAction = eActionType.Voltage
                        Ret.ActObj = obj
                        Ret.Result = True
                        Return Ret
                    End If
                End If
            End If
        End If
        Dim Bad As strAction
        Bad.WhichAction = eActionType.Unknown
        Bad.Result = False
        Bad.ActObj = Nothing
        Return Bad
    End Function


    Friend Function SerializeObject(ByRef ObjIn As Object, ByRef bteOut() As Byte) As Boolean
        If ObjIn Is Nothing Then Return False
        Dim str As New MemoryStream
        Dim sf As New Binary.BinaryFormatter

        Try
            sf.Serialize(str, ObjIn)
            ReDim bteOut(Convert.ToInt32(str.Length - 1))
            bteOut = str.ToArray
            Return True
        Catch ex As Exception
            Log(IFACE_NAME & " Error: Serializing object " & ObjIn.ToString & " :" & ex.Message, LogType.LOG_TYPE_ERROR)
            Return False
        End Try

    End Function

    Friend Function SerializeObject(ByRef ObjIn As Object, ByRef HexOut As String) As Boolean
        If ObjIn Is Nothing Then Return False
        Dim str As New MemoryStream
        Dim sf As New Binary.BinaryFormatter
        Dim bteOut() As Byte

        Try
            sf.Serialize(str, ObjIn)
            ReDim bteOut(Convert.ToInt32(str.Length - 1))
            bteOut = str.ToArray
            HexOut = ""
            For i As Integer = 0 To bteOut.Length - 1
                HexOut &= bteOut(i).ToString("x2").ToUpper
            Next
            Return True
        Catch ex As Exception
            Log(IFACE_NAME & " Error: Serializing (Hex) object " & ObjIn.ToString & " :" & ex.Message, LogType.LOG_TYPE_ERROR)
            Return False
        End Try

    End Function

    Public Function DeSerializeObject(ByRef bteIn() As Byte, ByRef ObjOut As Object) As Boolean
        ' Almost immediately there is a test to see if ObjOut is NOTHING.  The reason for this
        '   when the ObjOut is suppose to be where the deserialized object is stored, is that 
        '   I could find no way to test to see if the deserialized object and the variable to 
        '   hold it was of the same type.  If you try to get the type of a null object, you get
        '   only a null reference exception!  If I do not test the object type beforehand and 
        '   there is a difference, then the InvalidCastException is thrown back in the CALLING
        '   procedure, not here, because the cast is made when the ByRef object is cast when this
        '   procedure returns, not earlier.  In order to prevent a cast exception in the calling
        '   procedure that may or may not be handled, I made it so that you have to at least 
        '   provide an initialized ObjOut when you call this - ObjOut is set to nothing after it 
        '   is typed.
        If bteIn Is Nothing Then Return False
        If bteIn.Length < 1 Then Return False
        If ObjOut Is Nothing Then Return False
        Dim str As MemoryStream
        Dim sf As New Binary.BinaryFormatter
        Dim ObjTest As Object
        Dim TType As System.Type
        Dim OType As System.Type
        Try
            OType = ObjOut.GetType
            ObjOut = Nothing
            str = New MemoryStream(bteIn)
            ObjTest = sf.Deserialize(str)
            If ObjTest Is Nothing Then Return False
            TType = ObjTest.GetType
            If Not TType.Equals(OType) Then Return False
            ObjOut = ObjTest
            If ObjOut Is Nothing Then Return False
            Return True
        Catch exIC As InvalidCastException
            Return False
        Catch ex As Exception
            Log(IFACE_NAME & " Error: DeSerializing object: " & ex.Message, LogType.LOG_TYPE_ERROR)
            Return False
        End Try

    End Function

    Public Function DeSerializeObject(ByRef HexIn As String, ByRef ObjOut As Object) As Boolean
        ' Almost immediately there is a test to see if ObjOut is NOTHING.  The reason for this
        '   when the ObjOut is suppose to be where the deserialized object is stored, is that 
        '   I could find no way to test to see if the deserialized object and the variable to 
        '   hold it was of the same type.  If you try to get the type of a null object, you get
        '   only a null reference exception!  If I do not test the object type beforehand and 
        '   there is a difference, then the InvalidCastException is thrown back in the CALLING
        '   procedure, not here, because the cast is made when the ByRef object is cast when this
        '   procedure returns, not earlier.  In order to prevent a cast exception in the calling
        '   procedure that may or may not be handled, I made it so that you have to at least 
        '   provide an initialized ObjOut when you call this - ObjOut is set to nothing after it 
        '   is typed.
        If HexIn Is Nothing Then Return False
        If String.IsNullOrEmpty(HexIn.Trim) Then Return False
        If ObjOut Is Nothing Then Return False

        Dim str As MemoryStream
        Dim sf As New Binary.BinaryFormatter
        Dim ObjTest As Object
        Dim TType As System.Type
        Dim OType As System.Type

        Dim bteIn() As Byte
        Dim HowMany As Integer

        Try
            HowMany = Convert.ToInt32((HexIn.Length / 2) - 1)
            ReDim bteIn(HowMany)
            For i As Integer = 0 To HowMany
                'bteIn(i) = CByte("&H" & HexIn.Substring(i * 2, 2))
                bteIn(i) = Byte.Parse(HexIn.Substring(i * 2, 2), Globalization.NumberStyles.HexNumber)
            Next
            OType = ObjOut.GetType
            ObjOut = Nothing
            str = New MemoryStream(bteIn)
            ObjTest = sf.Deserialize(str)
            If ObjTest Is Nothing Then Return False
            TType = ObjTest.GetType
            If Not TType.Equals(OType) Then Return False
            ObjOut = ObjTest
            If ObjOut Is Nothing Then Return False
            Return True
        Catch exIC As InvalidCastException
            Return False
        Catch ex As Exception
            Log(IFACE_NAME & " Error: DeSerializing object: " & ex.Message, LogType.LOG_TYPE_ERROR)
            Return False
        End Try

    End Function
End Module
