Imports System
Imports Scheduler
Imports HomeSeerAPI
Imports HSCF.Communication.Scs.Communication.EndPoints.Tcp
Imports HSCF.Communication.ScsServices.Client
Imports HSCF.Communication.ScsServices.Service
Imports System.Reflection

Public Class HSPI
    Implements IPlugInAPI    ' this API is required for ALL plugins

    Dim sConfigPage As String = "Sample_Config"
    Dim sStatusPage As String = "Sample_Status"
    Dim ConfigPage As New web_config(sConfigPage)
    Dim StatusPage As New web_status(sStatusPage)
    Dim WebPage As Object

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

#Disable Warning BC42355 ' Property doesn't return a value on all code paths
        End Get
#Enable Warning BC42355 ' Property doesn't return a value on all code paths
    End Property

    Public Function Capabilities() As Integer Implements HomeSeerAPI.IPlugInAPI.Capabilities
        Return HomeSeerAPI.Enums.eCapabilities.CA_IO
    End Function

    Public Function AccessLevel() As Integer Implements HomeSeerAPI.IPlugInAPI.AccessLevel
        Return 1
    End Function

    Public Function InterfaceStatus() As HomeSeerAPI.IPlugInAPI.strInterfaceStatus Implements HomeSeerAPI.IPlugInAPI.InterfaceStatus
        Dim es As New IPlugInAPI.strInterfaceStatus
        'es.intStatus = IPlugInAPI.enumInterfaceStatus.OK
        'Return es
#Disable Warning BC42105 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42105 ' Function doesn't return a value on all code paths

    Public Function SupportsMultipleInstances() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsMultipleInstances

#Disable Warning BC42353 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42353 ' Function doesn't return a value on all code paths

    Public Function SupportsMultipleInstancesSingleEXE() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsMultipleInstancesSingleEXE

#Disable Warning BC42353 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42353 ' Function doesn't return a value on all code paths

    Public Function InstanceFriendlyName() As String Implements HomeSeerAPI.IPlugInAPI.InstanceFriendlyName

#Disable Warning BC42105 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42105 ' Function doesn't return a value on all code paths

    Public Function InitIO(ByVal port As String) As String Implements HomeSeerAPI.IPlugInAPI.InitIO

#Disable Warning BC42105 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42105 ' Function doesn't return a value on all code paths

    Public Function RaisesGenericCallbacks() As Boolean Implements HomeSeerAPI.IPlugInAPI.RaisesGenericCallbacks

#Disable Warning BC42353 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42353 ' Function doesn't return a value on all code paths

    Public Sub SetIOMulti(colSend As System.Collections.Generic.List(Of HomeSeerAPI.CAPI.CAPIControl)) Implements HomeSeerAPI.IPlugInAPI.SetIOMulti

    End Sub

    Public Sub ShutdownIO() Implements HomeSeerAPI.IPlugInAPI.ShutdownIO
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
        WebPage = SelectPage(pageName)
        Return WebPage.GetPagePlugin(pageName, user, userRights, queryString)
    End Function

    Public Function PostBackProc(ByVal pageName As String, ByVal data As String, ByVal user As String, ByVal userRights As Integer) As String Implements HomeSeerAPI.IPlugInAPI.PostBackProc
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

#Disable Warning BC42105 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42105 ' Function doesn't return a value on all code paths

    Public Function ActionConfigured(ByVal ActInfo As IPlugInAPI.strTrigActInfo) As Boolean Implements HomeSeerAPI.IPlugInAPI.ActionConfigured

#Disable Warning BC42353 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42353 ' Function doesn't return a value on all code paths

    Public Function ActionReferencesDevice(ByVal ActInfo As IPlugInAPI.strTrigActInfo, ByVal dvRef As Integer) As Boolean Implements HomeSeerAPI.IPlugInAPI.ActionReferencesDevice

#Disable Warning BC42353 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42353 ' Function doesn't return a value on all code paths

    Public Function ActionFormatUI(ByVal ActInfo As IPlugInAPI.strTrigActInfo) As String Implements HomeSeerAPI.IPlugInAPI.ActionFormatUI

#Disable Warning BC42105 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42105 ' Function doesn't return a value on all code paths

    Public ReadOnly Property ActionName(ByVal ActionNumber As Integer) As String Implements HomeSeerAPI.IPlugInAPI.ActionName
        Get

#Disable Warning BC42107 ' Property doesn't return a value on all code paths
        End Get
#Enable Warning BC42107 ' Property doesn't return a value on all code paths
    End Property

    Public Function ActionProcessPostUI(ByVal PostData As Collections.Specialized.NameValueCollection, ByVal TrigInfoIN As IPlugInAPI.strTrigActInfo) As IPlugInAPI.strMultiReturn Implements HomeSeerAPI.IPlugInAPI.ActionProcessPostUI

#Disable Warning BC42105 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42105 ' Function doesn't return a value on all code paths

    Public Function ActionCount() As Integer Implements HomeSeerAPI.IPlugInAPI.ActionCount

#Disable Warning BC42353 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42353 ' Function doesn't return a value on all code paths

    Public Property Condition(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As Boolean Implements HomeSeerAPI.IPlugInAPI.Condition
        Set(ByVal value As Boolean)

        End Set
        Get

#Disable Warning BC42355 ' Property doesn't return a value on all code paths
        End Get
#Enable Warning BC42355 ' Property doesn't return a value on all code paths
    End Property

    Public Function HandleAction(ByVal ActInfo As IPlugInAPI.strTrigActInfo) As Boolean Implements HomeSeerAPI.IPlugInAPI.HandleAction

#Disable Warning BC42353 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42353 ' Function doesn't return a value on all code paths

    Public ReadOnly Property HasConditions(ByVal TriggerNumber As Integer) As Boolean Implements HomeSeerAPI.IPlugInAPI.HasConditions
        Get

#Disable Warning BC42355 ' Property doesn't return a value on all code paths
        End Get
#Enable Warning BC42355 ' Property doesn't return a value on all code paths
    End Property

    Public Function TriggerTrue(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As Boolean Implements HomeSeerAPI.IPlugInAPI.TriggerTrue

#Disable Warning BC42353 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42353 ' Function doesn't return a value on all code paths

    Public ReadOnly Property HasTriggers() As Boolean Implements HomeSeerAPI.IPlugInAPI.HasTriggers
        Get

#Disable Warning BC42355 ' Property doesn't return a value on all code paths
        End Get
#Enable Warning BC42355 ' Property doesn't return a value on all code paths
    End Property

    Public ReadOnly Property SubTriggerCount(ByVal TriggerNumber As Integer) As Integer Implements HomeSeerAPI.IPlugInAPI.SubTriggerCount
        Get

#Disable Warning BC42355 ' Property doesn't return a value on all code paths
        End Get
#Enable Warning BC42355 ' Property doesn't return a value on all code paths
    End Property

    Public ReadOnly Property SubTriggerName(ByVal TriggerNumber As Integer, ByVal SubTriggerNumber As Integer) As String Implements HomeSeerAPI.IPlugInAPI.SubTriggerName
        Get

#Disable Warning BC42107 ' Property doesn't return a value on all code paths
        End Get
#Enable Warning BC42107 ' Property doesn't return a value on all code paths
    End Property

    Public Function TriggerBuildUI(ByVal sUnique As String, ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As String Implements HomeSeerAPI.IPlugInAPI.TriggerBuildUI

#Disable Warning BC42105 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42105 ' Function doesn't return a value on all code paths

    Public ReadOnly Property TriggerConfigured(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As Boolean Implements HomeSeerAPI.IPlugInAPI.TriggerConfigured
        Get

#Disable Warning BC42355 ' Property doesn't return a value on all code paths
        End Get
#Enable Warning BC42355 ' Property doesn't return a value on all code paths
    End Property

    Public Function TriggerReferencesDevice(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo, ByVal dvRef As Integer) As Boolean Implements HomeSeerAPI.IPlugInAPI.TriggerReferencesDevice

#Disable Warning BC42353 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42353 ' Function doesn't return a value on all code paths

    Public Function TriggerFormatUI(ByVal TrigInfo As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As String Implements HomeSeerAPI.IPlugInAPI.TriggerFormatUI

#Disable Warning BC42105 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42105 ' Function doesn't return a value on all code paths

    Public ReadOnly Property TriggerName(ByVal TriggerNumber As Integer) As String Implements HomeSeerAPI.IPlugInAPI.TriggerName
        Get

#Disable Warning BC42107 ' Property doesn't return a value on all code paths
        End Get
#Enable Warning BC42107 ' Property doesn't return a value on all code paths
    End Property

    Public Function TriggerProcessPostUI(ByVal PostData As System.Collections.Specialized.NameValueCollection,
                                         ByVal TrigInfoIn As HomeSeerAPI.IPlugInAPI.strTrigActInfo) As HomeSeerAPI.IPlugInAPI.strMultiReturn Implements HomeSeerAPI.IPlugInAPI.TriggerProcessPostUI

#Disable Warning BC42105 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42105 ' Function doesn't return a value on all code paths

    Public ReadOnly Property TriggerCount As Integer Implements HomeSeerAPI.IPlugInAPI.TriggerCount
        Get

#Disable Warning BC42355 ' Property doesn't return a value on all code paths
        End Get
#Enable Warning BC42355 ' Property doesn't return a value on all code paths
    End Property

    Public Function SupportsConfigDevice() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsConfigDevice

#Disable Warning BC42353 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42353 ' Function doesn't return a value on all code paths

    Public Function SupportsConfigDeviceAll() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsConfigDeviceAll

#Disable Warning BC42353 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42353 ' Function doesn't return a value on all code paths

    Public Function SupportsAddDevice() As Boolean Implements HomeSeerAPI.IPlugInAPI.SupportsAddDevice

#Disable Warning BC42353 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42353 ' Function doesn't return a value on all code paths

    Function ConfigDevicePost(ByVal ref As Integer, ByVal data As String, ByVal user As String, ByVal userRights As Integer) As Enums.ConfigDevicePostReturn Implements IPlugInAPI.ConfigDevicePost

#Disable Warning BC42353 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42353 ' Function doesn't return a value on all code paths

    Function ConfigDevice(ByVal ref As Integer, ByVal user As String, ByVal userRights As Integer, newDevice As Boolean) As String Implements IPlugInAPI.ConfigDevice

#Disable Warning BC42105 ' Function doesn't return a value on all code paths
    End Function
#Enable Warning BC42105 ' Function doesn't return a value on all code paths

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

