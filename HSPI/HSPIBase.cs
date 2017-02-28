using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using HomeSeerAPI;
using HSCF.Communication.Scs.Communication;
using HSCF.Communication.Scs.Communication.EndPoints.Tcp;
using HSCF.Communication.ScsServices.Client;

namespace HSPI
{
    // ReSharper disable once InconsistentNaming
    public abstract class HSPIBase : IPlugInAPI
    {
        protected IAppCallbackAPI Callback;
        protected IScsServiceClient<IAppCallbackAPI> CallbackClient;
        protected IHSApplication Hs;
        protected IScsServiceClient<IHSApplication> HsClient;
        public bool Shutdown;

        public bool Connected => HsClient.CommunicationState == CommunicationStates.Connected;

        public string Name => GetName();
        public abstract string InstanceFriendlyName();

        public abstract int Capabilities();
        public abstract int AccessLevel();
        public abstract bool SupportsMultipleInstances();
        public abstract bool SupportsMultipleInstancesSingleEXE();
        public abstract bool SupportsAddDevice();
        public abstract IPlugInAPI.strInterfaceStatus InterfaceStatus();
        public abstract void HSEvent(Enums.HSEvent eventType, object[] parms);
        public abstract string GenPage(string link);
        public abstract string PagePut(string data);
        public abstract void ShutdownIO();
        public abstract bool RaisesGenericCallbacks();
        public abstract void SetIOMulti(List<CAPI.CAPIControl> colSend);
        public abstract string InitIO(string port);
        public abstract IPlugInAPI.PollResultInfo PollDevice(int dvref);
        public abstract bool SupportsConfigDevice();
        public abstract bool SupportsConfigDeviceAll();
        public abstract Enums.ConfigDevicePostReturn ConfigDevicePost(int @ref, string data, string user, int userRights);
        public abstract string ConfigDevice(int @ref, string user, int userRights, bool newDevice);
        public abstract SearchReturn[] Search(string searchString, bool regEx);
        public abstract object PluginFunction(string procName, object[] parms);
        public abstract object PluginPropertyGet(string procName, object[] parms);
        public abstract void PluginPropertySet(string procName, object value);
        public abstract void SpeakIn(int device, string txt, bool w, string host);
        public abstract int ActionCount();
        public abstract bool ActionConfigured(IPlugInAPI.strTrigActInfo actInfo);
        public abstract string ActionBuildUI(string sUnique, IPlugInAPI.strTrigActInfo actInfo);

        public abstract IPlugInAPI.strMultiReturn ActionProcessPostUI(NameValueCollection postData,
            IPlugInAPI.strTrigActInfo trigInfoIn);

        public abstract string ActionFormatUI(IPlugInAPI.strTrigActInfo actInfo);
        public abstract bool ActionReferencesDevice(IPlugInAPI.strTrigActInfo actInfo, int dvRef);
        public abstract bool HandleAction(IPlugInAPI.strTrigActInfo actInfo);
        public abstract string TriggerBuildUI(string sUnique, IPlugInAPI.strTrigActInfo trigInfo);

        public abstract IPlugInAPI.strMultiReturn TriggerProcessPostUI(NameValueCollection postData,
            IPlugInAPI.strTrigActInfo trigInfoIn);

        public abstract string TriggerFormatUI(IPlugInAPI.strTrigActInfo trigInfo);
        public abstract bool TriggerTrue(IPlugInAPI.strTrigActInfo trigInfo);
        public abstract bool TriggerReferencesDevice(IPlugInAPI.strTrigActInfo trigInfo, int dvRef);
        public abstract string GetPagePlugin(string page, string user, int userRights, string queryString);
        public abstract string PostBackProc(string page, string data, string user, int userRights);

        public bool HSCOMPort => GetHscomPort();

        public abstract string get_ActionName(int actionNumber);
        public abstract bool get_HasConditions(int triggerNumber);
        public bool HasTriggers => GetHasTriggers();
        protected abstract bool GetHasTriggers();
        public int TriggerCount => GetTriggerCount();
        public bool ActionAdvancedMode { get; set; }
        protected abstract int GetTriggerCount();
        public abstract string get_TriggerName(int triggerNumber);
        public abstract int get_SubTriggerCount(int triggerNumber);
        public abstract string get_SubTriggerName(int triggerNumber, int subTriggerNumber);
        public abstract bool get_TriggerConfigured(IPlugInAPI.strTrigActInfo trigInfo);
        public abstract bool get_Condition(IPlugInAPI.strTrigActInfo trigInfo);
        public abstract void set_Condition(IPlugInAPI.strTrigActInfo trigInfo, bool value);

        protected abstract string GetName();

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

        protected abstract bool GetHscomPort();
    }
}