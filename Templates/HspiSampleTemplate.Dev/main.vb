Imports Scheduler
Imports HomeSeerAPI
Imports HSCF.Communication.Scs.Communication.EndPoints.Tcp
Imports HSCF.Communication.ScsServices.Client
Imports HSCF.Communication.ScsServices.Service

Module Main
    Public WithEvents client As HSCF.Communication.ScsServices.Client.IScsServiceClient(Of IHSApplication)
    Dim WithEvents clientCallback As HSCF.Communication.ScsServices.Client.IScsServiceClient(Of IAppCallbackAPI)

    Private host As HomeSeerAPI.IHSApplication
    Private gAppAPI As HSPI = Nothing

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

        gAppAPI = New HSPI


        Console.WriteLine("Connecting to server at " & sIp & "...")
        client = ScsServiceClientBuilder.CreateClient(Of IHSApplication)(New ScsTcpEndPoint(sIp, 10400), gAppAPI)
        clientCallback = ScsServiceClientBuilder.CreateClient(Of IAppCallbackAPI)(New ScsTcpEndPoint(sIp, 10400), gAppAPI)

        Dim Attempts As Integer = 1

TryAgain:
        Try
            client.Connect()
            clientCallback.Connect()

            host = client.ServiceProxy
            Dim APIVersion As Double = host.APIVersion  ' will cause an error if not really connected

            callback = clientCallback.ServiceProxy
            APIVersion = callback.APIVersion  ' will cause an error if not really connected
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
            ' connect to HS so it can register a callback to us
            host.Connect(IFACE_NAME, "")
            Console.WriteLine("Connected, waiting to be initialized...")
            Do
                Threading.Thread.Sleep(30)
            Loop While client.CommunicationState = HSCF.Communication.Scs.Communication.CommunicationStates.Connected And Not bShutDown
            If Not bShutDown Then
                gAppAPI.ShutdownIO()
                Console.WriteLine("Connection lost, exiting")
            Else
                Console.WriteLine("Shutting down plugin")
            End If
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
