Imports Scheduler
Imports HomeSeerAPI

Imports HSCF.Communication.Scs.Communication.EndPoints.Tcp
Imports HSCF.Communication.ScsServices.Client
Imports HSCF.Communication.ScsServices.Service


Module Main
    Public WithEvents client As HSCF.Communication.ScsServices.Client.IScsServiceClient(Of IHSApplication)
    Dim WithEvents clientCallback As HSCF.Communication.ScsServices.Client.IScsServiceClient(Of IAppCallbackAPI)
    Dim WithEvents clientMediaCallback As HSCF.Communication.ScsServices.Client.IScsServiceClient(Of IMediaAPI_HS)

    Private host As HomeSeerAPI.IHSApplication
    Public plugin As New HSPI                               ' real plugin functions, user supplied


    Sub Main()
        Dim sIp As String = "127.0.0.1"
        Dim argv As System.Collections.ObjectModel.ReadOnlyCollection(Of String)
        argv = My.Application.CommandLineArgs

        Dim sCmd As String
        For Each sCmd In argv
            Dim ch(0) As String
            ch(0) = "="
            Dim parts() As String = sCmd.Split(ch, StringSplitOptions.None)
            Select Case parts(0).ToLower
                Case "server" : sIp = parts(1)
                Case "instance"
                    Try
                        Instance = parts(1)
                    Catch ex As Exception
                        Instance = ""
                    End Try
            End Select
        Next

        Console.WriteLine("Plugin: " & IFACE_NAME & " Instance: " & Instance & " starting...")
        Console.WriteLine("Connecting to server at " & sIp & "...")
        client = ScsServiceClientBuilder.CreateClient(Of IHSApplication)(New ScsTcpEndPoint(sIp, 10400), plugin)
        clientCallback = ScsServiceClientBuilder.CreateClient(Of IAppCallbackAPI)(New ScsTcpEndPoint(sIp, 10400), plugin)
        clientMediaCallback = ScsServiceClientBuilder.CreateClient(Of IMediaAPI_HS)(New ScsTcpEndPoint(sIp, 10400), plugin)

        Dim Attempts As Integer = 1

TryAgain:
        Try
            client.Connect()
            clientCallback.Connect()
            clientMediaCallback.Connect()

            Dim APIVersion As Double

            Try
                host = client.ServiceProxy
                APIVersion = host.APIVersion  ' will cause an error if not really connected
                Console.WriteLine("Host API Version: " & APIVersion.ToString)
            Catch ex As Exception
                Console.WriteLine("Error getting API version from host object: " & ex.Message & "->" & ex.StackTrace)
                'Return
            End Try

            Try
                callback = clientCallback.ServiceProxy
                APIVersion = callback.APIVersion  ' will cause an error if not really connected
            Catch ex As Exception
                Console.WriteLine("Error getting API version from callback object: " & ex.Message & "->" & ex.StackTrace)
                Return
            End Try

            Try
                ' example to access a media plugin
                mediacallback = clientMediaCallback.ServiceProxy
                mediacallback.SetPlugin("Media Player", "")
                'Dim key As New Lib_Entry_Key
                'mediacallback.Play(key)
            Catch ex As Exception

            End Try


        Catch ex As Exception
            Console.WriteLine("Cannot connect attempt " & Attempts.ToString & ": " & ex.Message)
            If ex.Message.ToLower.Contains("timeout occurred.") Then
                Attempts += 1
                If Attempts < 6 Then GoTo TryAgain
            End If

            If client IsNot Nothing Then
                client.Dispose()
                client = Nothing
            End If
            If clientCallback IsNot Nothing Then
                clientCallback.Dispose()
                clientCallback = Nothing
            End If
            wait(4)
            Return
        End Try

        Try
            ' create the user object that is the real plugin, accessed from the pluginAPI wrapper
            callback = callback
            hs = host
            plugin.OurInstanceFriendlyName = Instance
            ' connect to HS so it can register a callback to us
            host.Connect(IFACE_NAME, Instance)
            Console.WriteLine("Connected, waiting to be initialized...")
            Do
                Threading.Thread.Sleep(10)
            Loop While client.CommunicationState = HSCF.Communication.Scs.Communication.CommunicationStates.Connected And Not HSPI.bShutDown
            Console.WriteLine("Connection lost, exiting")
            ' disconnect from server for good here
            client.Disconnect()
            clientCallback.Disconnect()
            wait(2)
            End
        Catch ex As Exception
            Console.WriteLine("Cannot connect(2): " & ex.Message)
            wait(2)
            End
            Return
        End Try

    End Sub

    Private Sub client_Disconnected(ByVal sender As Object, ByVal e As System.EventArgs) Handles client.Disconnected
        Console.WriteLine("Disconnected from server - client")
    End Sub


    Private Sub wait(ByVal secs As Integer)
        Threading.Thread.Sleep(secs * 1000)
    End Sub

    
End Module
