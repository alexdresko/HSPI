using System;
using HomeSeerAPI;
using HSCF.Communication.Scs.Communication;
using HSCF.Communication.Scs.Communication.EndPoints.Tcp;
using HSCF.Communication.ScsServices.Client;

namespace HSPI
{
    // ReSharper disable once InconsistentNaming
    public abstract class HSPIBase
    {
        protected IScsServiceClient<IHSApplication> HsClient;
        protected IHSApplication Hs;
        protected IScsServiceClient<IAppCallbackAPI> CallbackClient;
        protected IAppCallbackAPI Callback;
        public bool Shutdown;
        public abstract string Name { get; }
        public bool Connected => HsClient.CommunicationState == CommunicationStates.Connected;
        public abstract string InstanceFriendlyName();

        public void Connect(string serverAddress, int serverPort)
        {
            // This method is called by our console wrapper at launch time

            // Create our main connection to the homeseer TCP communication framework
            // part 1 - hs object Proxy
            try
            {
                HsClient =
                    ScsServiceClientBuilder.CreateClient<IHSApplication>(new ScsTcpEndPoint(serverAddress, serverPort),
                        this);
                HsClient.Connect(); 
                Hs = HsClient.ServiceProxy;
                // ReSharper disable once UnusedVariable
                var apiVersion = Hs.APIVersion; // just to make sure our connection is valid
            }
            catch (Exception ex)
            {
                throw new Exception("Error connecting homeseer SCS client: " + ex.Message, ex);
            }

            // part 2 - callback object Proxy
            try
            {
                CallbackClient =
                    ScsServiceClientBuilder.CreateClient<IAppCallbackAPI>(
                        new ScsTcpEndPoint(serverAddress, serverPort), this);
                CallbackClient.Connect();
                Callback = CallbackClient.ServiceProxy;
                // ReSharper disable once UnusedVariable
                var apiVersion = Callback.APIVersion; // just to make sure our connection is valid
            }
            catch (Exception ex)
            {
                throw new Exception("Error connecting callback SCS client: " + ex.Message, ex);
            }

            // Establish the reverse connection from homeseer back to our plugin
            try
            {
                Hs.Connect(Name, InstanceFriendlyName());
            }
            catch (Exception ex)
            {
                throw new Exception("Error connecting homeseer to our plugin: " + ex.Message, ex);
            }
        }
    }
}