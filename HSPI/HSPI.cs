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
    public class HSPI : IPlugInAPI
    {
        private IAppCallbackAPI _callback;
        private IScsServiceClient<IAppCallbackAPI> _callbackClient;
        private IHSApplication _hs;
        // our homeseer objects
        private IScsServiceClient<IHSApplication> _hsClient;
        internal string InstanceName = "";

        // our plugin identity
        internal string InterfaceName = "Test Plugin";

        // our plugin status
        internal bool Shutdown;

        public bool Connected => _hsClient.CommunicationState == CommunicationStates.Connected;

        public string Name => InterfaceName;

        public string InstanceFriendlyName()
        {
            return InstanceName;
        }

        public IPlugInAPI.strInterfaceStatus InterfaceStatus()
        {
            return new IPlugInAPI.strInterfaceStatus {intStatus = IPlugInAPI.enumInterfaceStatus.OK};
        }

        public int Capabilities()
        {
            return (int) Enums.eCapabilities.CA_IO;
        }

        public int AccessLevel()
        {
            return 1;
        }

        public bool HSCOMPort => true;

        public bool SupportsAddDevice()
        {
            return false;
        }

        public bool SupportsConfigDevice()
        {
            return false;
        }

        public bool SupportsConfigDeviceAll()
        {
            return false;
        }

        public bool SupportsMultipleInstances()
        {
            return false;
        }

        public bool SupportsMultipleInstancesSingleEXE()
        {
            return false;
        }

        public bool RaisesGenericCallbacks()
        {
            return false;
        }

        public void HSEvent(Enums.HSEvent eventType, object[] parms)
        {
        }

        public string InitIO(string port)
        {
            // debug
            _hs.WriteLog(InterfaceName, "Entering InitIO");

            // initialise everything here, return a blank string only if successful, or an error message


            // debug
            _hs.WriteLog(InterfaceName, "Completed InitIO");
            return "";
        }

        public IPlugInAPI.PollResultInfo PollDevice(int dvref)
        {
            // return the value of a device on demand

            return new IPlugInAPI.PollResultInfo
            {
                Result = IPlugInAPI.enumPollResult.Device_Not_Found,
                Value = 0
            };
        }

        public void SetIOMulti(List<CAPI.CAPIControl> colSend)
        {
            // homeseer will inform us when the one of our devices has changed.  Push that change through to the field.
        }

        public void ShutdownIO()
        {
            // debug
            _hs.WriteLog(InterfaceName, "Entering ShutdownIO");

            // shut everything down here


            // let our console wrapper know we are finished
            Shutdown = true;

            // debug
            _hs.WriteLog(InterfaceName, "Completed ShutdownIO");
        }

        public SearchReturn[] Search(string searchString, bool regEx)
        {
            return null;
        }

        public bool ActionAdvancedMode
        {
            get { return false; }

            set
            {
                // do nothing
            }
        }

        public bool HasTriggers => false;

        public int TriggerCount => 0;

        public string ActionBuildUI(string sUnique, IPlugInAPI.strTrigActInfo actInfo)
        {
            return "";
        }

        public bool ActionConfigured(IPlugInAPI.strTrigActInfo actInfo)
        {
            return true;
        }

        public int ActionCount()
        {
            return 0;
        }

        public string ActionFormatUI(IPlugInAPI.strTrigActInfo actInfo)
        {
            return "";
        }

        public IPlugInAPI.strMultiReturn ActionProcessPostUI(NameValueCollection postData,
            IPlugInAPI.strTrigActInfo trigInfoIn)
        {
            return new IPlugInAPI.strMultiReturn();
        }

        public bool ActionReferencesDevice(IPlugInAPI.strTrigActInfo actInfo, int dvRef)
        {
            return false;
        }

        public string get_ActionName(int actionNumber)
        {
            return "";
        }

        public bool get_Condition(IPlugInAPI.strTrigActInfo trigInfo)
        {
            return false;
        }

        public bool get_HasConditions(int triggerNumber)
        {
            return false;
        }

        public string TriggerBuildUI(string sUnique, IPlugInAPI.strTrigActInfo trigInfo)
        {
            return "";
        }

        public string TriggerFormatUI(IPlugInAPI.strTrigActInfo trigInfo)
        {
            return "";
        }

        public IPlugInAPI.strMultiReturn TriggerProcessPostUI(NameValueCollection postData,
            IPlugInAPI.strTrigActInfo trigInfoIn)
        {
            return new IPlugInAPI.strMultiReturn();
        }

        public bool TriggerReferencesDevice(IPlugInAPI.strTrigActInfo trigInfo, int dvRef)
        {
            return false;
        }

        public bool TriggerTrue(IPlugInAPI.strTrigActInfo trigInfo)
        {
            return false;
        }

        public int get_SubTriggerCount(int triggerNumber)
        {
            return 0;
        }

        public string get_SubTriggerName(int triggerNumber, int subTriggerNumber)
        {
            return "";
        }

        public bool get_TriggerConfigured(IPlugInAPI.strTrigActInfo trigInfo)
        {
            return true;
        }

        public string get_TriggerName(int triggerNumber)
        {
            return "";
        }

        public bool HandleAction(IPlugInAPI.strTrigActInfo actInfo)
        {
            return false;
        }

        public void set_Condition(IPlugInAPI.strTrigActInfo trigInfo, bool value)
        {
        }

        public void SpeakIn(int device, string txt, bool w, string host)
        {
        }

        public string GenPage(string link)
        {
            return "";
        }

        public string PagePut(string data)
        {
            return "";
        }

        public string GetPagePlugin(string page, string user, int userRights, string queryString)
        {
            return "";
        }

        public string PostBackProc(string page, string data, string user, int userRights)
        {
            return "";
        }

        public string ConfigDevice(int @ref, string user, int userRights, bool newDevice)
        {
            return "";
        }

        public Enums.ConfigDevicePostReturn ConfigDevicePost(int @ref, string data, string user, int userRights)
        {
            return Enums.ConfigDevicePostReturn.DoneAndCancel;
        }

        public object PluginFunction(string procName, object[] parms)
        {
            return null;
        }

        public object PluginPropertyGet(string procName, object[] parms)
        {
            return null;
        }

        public void PluginPropertySet(string procName, object value)
        {
        }

        public void Connect(string serverAddress, int serverPort)
        {
            // This method is called by our console wrapper at launch time

            // Create our main connection to the homeseer TCP communication framework
            // part 1 - hs object Proxy
            try
            {
                _hsClient =
                    ScsServiceClientBuilder.CreateClient<IHSApplication>(new ScsTcpEndPoint(serverAddress, serverPort),
                        this);
                _hsClient.Connect();
                _hs = _hsClient.ServiceProxy;
                // ReSharper disable once UnusedVariable
                var apiVersion = _hs.APIVersion; // just to make sure our connection is valid
            }
            catch (Exception ex)
            {
                throw new Exception("Error connecting homeseer SCS client: " + ex.Message, ex);
            }

            // part 2 - callback object Proxy
            try
            {
                _callbackClient =
                    ScsServiceClientBuilder.CreateClient<IAppCallbackAPI>(
                        new ScsTcpEndPoint(serverAddress, serverPort), this);
                _callbackClient.Connect();
                _callback = _callbackClient.ServiceProxy;
                // ReSharper disable once UnusedVariable
                var apiVersion = _callback.APIVersion; // just to make sure our connection is valid
            }
            catch (Exception ex)
            {
                throw new Exception("Error connecting callback SCS client: " + ex.Message, ex);
            }

            // Establish the reverse connection from homeseer back to our plugin
            try
            {
                _hs.Connect(InterfaceName, InstanceName);
            }
            catch (Exception ex)
            {
                throw new Exception("Error connecting homeseer to our plugin: " + ex.Message, ex);
            }
        }
    }
}