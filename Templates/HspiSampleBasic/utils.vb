Imports System.IO
Imports System.Runtime.Serialization.Formatters
Imports HomeSeerAPI

Module utils
    Public plugin As New HSPI
    Public IFACE_NAME As String = "Sample-Basic"
    Public callback As HomeSeerAPI.IAppCallbackAPI
    Public hs As HomeSeerAPI.IHSApplication
    Public Instance As String = ""
    Public InterfaceVersion As Integer
    Public bShutDown As Boolean = False
    Public gEXEPath As String = System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory)
    Public Const INIFILE As String = "Sample-Basic.ini"
    Public CurrentPage As Object

    Public Structure pair
        Dim name As String
        Dim value As String
    End Structure

    Enum LogLevel As Integer
        Normal = 1
        Debug = 2
    End Enum

    Enum MessageType
        Normal = 0
        Warning = 1
        Error_ = 2
    End Enum

    Public Enum DEVICE_COMMAND
        All_Lights_Off = 0
        All_Lights_On = 1
        UOn = 2
        UOff = 3
    End Enum

    Sub PEDAdd(ByRef PED As clsPlugExtraData, ByVal PEDName As String, ByVal PEDValue As Object)
        Dim ByteObject() As Byte = Nothing
        If PED Is Nothing Then PED = New clsPlugExtraData
        SerializeObject(PEDValue, ByteObject)
        If Not PED.AddNamed(PEDName, ByteObject) Then
            PED.RemoveNamed(PEDName)
            PED.AddNamed(PEDName, ByteObject)
        End If
    End Sub

    Function PEDGet(ByRef PED As clsPlugExtraData, ByVal PEDName As String) As Object
        Dim ByteObject() As Byte
        Dim ReturnValue As New Object
        ByteObject = PED.GetNamed(PEDName)
        If ByteObject Is Nothing Then Return Nothing
        DeSerializeObject(ByteObject, ReturnValue)
        Return ReturnValue
    End Function

    Public Function SerializeObject(ByRef ObjIn As Object, ByRef bteOut() As Byte) As Boolean
        If ObjIn Is Nothing Then Return False
        Dim str As New MemoryStream
        Dim sf As New Binary.BinaryFormatter

        Try
            sf.Serialize(str, ObjIn)
            ReDim bteOut(CInt(str.Length - 1))
            bteOut = str.ToArray
            Return True
        Catch ex As Exception
            Log(LogLevel.Debug, IFACE_NAME & " Error: Serializing object " & ObjIn.ToString & " :" & ex.Message)
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
            'If Not TType.Equals(OType) Then Return False
            ObjOut = ObjTest
            If ObjOut Is Nothing Then Return False
            Return True
        Catch exIC As InvalidCastException
            Return False
        Catch ex As Exception
            Log(LogLevel.Debug, IFACE_NAME & " Error: DeSerializing object: " & ex.Message)
            Return False
        End Try

    End Function

    Private Sub DeleteDevices()
        Dim en As Object
        Dim dv As Object

        Try
            en = hs.GetDeviceEnumerator
            Do While Not en.Finished
                dv = en.GetNext
                If dv IsNot Nothing Then
                    If dv.interface = IFACE_NAME Then
                        Try
                            hs.DeleteDevice(dv.ref)
                        Catch ex As Exception
                        End Try
                    End If
                End If
            Loop
            hs.SaveEventsDevices()
        Catch ex As Exception
        End Try
    End Sub

    Sub DeleteModule(ByVal n As Integer)
        Dim i As Integer
        Log("Module to Delete is " & n)
        For i = 1 To 16
            hs.DeleteDevice(hs.GetINISetting("Module " & n, "ref-" & i.ToString, "", INIFILE))
        Next

        hs.ClearINISection("Module " & n.ToString, INIFILE)

        Log("Finished deleting module.")
    End Sub

    Function InitDevice(ByVal PName As String, ByVal modNum As Integer, ByVal counter As Integer, Optional ByVal ref As Integer = 0) As Boolean
        Dim dv As Scheduler.Classes.DeviceClass = Nothing
        Log("Initiating Device " & PName, LogLevel.Debug)

        Try
            If Not hs.DeviceExistsRef(ref) Then
                ref = hs.NewDeviceRef(PName)

                hs.SaveINISetting("Module " & modNum, "ref-" & counter.ToString, ref, INIFILE)
                Try
                    dv = hs.GetDeviceByRef(ref)
                    InitHSDevice(dv, PName)
                    Return True
                Catch ex As Exception
                    Log("Error initializing device " & PName & ": " & ex.Message)
                    Return False
                End Try
            End If
        Catch ex As Exception
            Log("Error getting RefID from DeviceCode within InitDevice. " & ex.Message)
        End Try
        Return False
    End Function

    Sub InitHSDevice(ByRef dv As Scheduler.Classes.DeviceClass, Optional ByVal Name As String = "Sample")
        Dim test As Object = Nothing

        dv.Address(hs) = "HOME"
        Dim DT As New DeviceTypeInfo
        DT.Device_Type = DeviceTypeInfo.eDeviceAPI.Plug_In
        dv.DeviceType_Set(hs) = DT
        dv.Interface(hs) = IFACE_NAME
        dv.InterfaceInstance(hs) = Instance
        dv.Last_Change(hs) = Now
        dv.Name(hs) = Name
        dv.Location(hs) = "Sample"
        dv.Device_Type_String(hs) = "Sample"
        dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
        dv.MISC_Set(hs, Enums.dvMISC.NO_LOG)
        dv.Status_Support(hs) = False
    End Sub

    Public Sub SendCommand(ByVal Housecode As String, ByVal Devicecode As String, ByVal Action As Integer)
        'Send a command somewhere
    End Sub

    Public Sub RegisterCallback(ByRef frm As Object)
        ' call back into HS and get a reference to the HomeSeer ActiveX interface
        ' this can be used make calls back into HS like hs.SetDeviceValue, etc.
        ' The callback object is a different interface reserved for plug-ins.
        callback = frm
        hs = frm.GetHSIface
        If hs Is Nothing Then
            MsgBox("Unable to access HS interface", MsgBoxStyle.Critical)
        Else
            Log("Register callback completed", LogLevel.Debug)
            InterfaceVersion = hs.InterfaceVersion
        End If
    End Sub

    Public Sub RegisterWebPage(ByVal link As String, Optional linktext As String = "", Optional page_title As String = "")
        Try
            hs.RegisterPage(link, IFACE_NAME, Instance)
            If linktext = "" Then linktext = link
            linktext = linktext.Replace("_", " ")
            If page_title = "" Then page_title = linktext
            Dim wpd As New HomeSeerAPI.webPageDesc
            wpd.plugInName = IFACE_NAME
            wpd.link = link
            wpd.linktext = linktext & Instance
            wpd.page_title = page_title & Instance
            callback.RegisterLink(wpd)
        Catch ex As Exception
            Log(LogLevel.Debug, "Error - Registering Web Links: " & ex.Message)
        End Try
    End Sub

    Public Sub Log(ByVal Message As String, Optional ByVal Log_Level As LogLevel = LogLevel.Normal)
        If Log_Level = LogLevel.Normal Then
            hs.WriteLog(IFACE_NAME, Message)
        End If
        If Log_Level = LogLevel.Debug Then
            If IO.Directory.Exists(gEXEPath & "\Debug Logs") Then
                IO.File.AppendAllText(gEXEPath & "\Debug Logs\CurrentCost.log", Now.ToString & " ~ " & Message & vbCrLf)
            ElseIf IO.Directory.Exists(gEXEPath & "\Logs") Then
                IO.File.AppendAllText(gEXEPath & "\Logs\CurrentCost.log", Now.ToString & " ~ " & Message & vbCrLf)
            Else
                IO.File.AppendAllText(gEXEPath & "\CurrentCost.log", Now.ToString & " ~ " & Message & vbCrLf)
            End If
        End If
    End Sub

End Module
