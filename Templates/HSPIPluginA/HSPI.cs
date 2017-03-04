using System.Collections.Generic;
using System.Collections.Specialized;
using HomeSeerAPI;
using Hspi;

namespace $safeprojectname$
{
    // ReSharper disable once InconsistentNaming
    public class HSPI : HspiBase
    {
        public override string InstanceFriendlyName()
        {
            return string.Empty;
        }

        public override IPlugInAPI.strInterfaceStatus InterfaceStatus()
        {
            return new IPlugInAPI.strInterfaceStatus {intStatus = IPlugInAPI.enumInterfaceStatus.OK};
        }

        public override int Capabilities()
        {
            return (int) Enums.eCapabilities.CA_IO;
        }

        public override int AccessLevel()
        {
            return 1;
        }

        public override bool SupportsAddDevice()
        {
            return false;
        }

        public override bool SupportsConfigDevice()
        {
            return false;
        }

        public override bool SupportsConfigDeviceAll()
        {
            return false;
        }

        public override bool SupportsMultipleInstances()
        {
            return false;
        }

        public override bool SupportsMultipleInstancesSingleEXE()
        {
            return false;
        }

        public override bool RaisesGenericCallbacks()
        {
            return false;
        }

        public override void HSEvent(Enums.HSEvent eventType, object[] parms)
        {
        }

        public override string InitIO(string port)
        {
            // debug
            Hs.WriteLog(Name, "Entering InitIO");

            // initialise everything here, return a blank string only if successful, or an error message


            // debug
            Hs.WriteLog(Name, "Completed InitIO");
            return "";
        }

        public override IPlugInAPI.PollResultInfo PollDevice(int dvref)
        {
            // return the value of a device on demand

            return new IPlugInAPI.PollResultInfo
            {
                Result = IPlugInAPI.enumPollResult.Device_Not_Found,
                Value = 0
            };
        }

        public override void SetIOMulti(List<CAPI.CAPIControl> colSend)
        {
            // homeseer will inform us when the one of our devices has changed.  Push that change through to the field.
        }

        public override void ShutdownIO()
        {
            // debug
            Hs.WriteLog(Name, "Entering ShutdownIO");

            // shut everything down here


            // let our console wrapper know we are finished
            Shutdown = true;

            // debug
            Hs.WriteLog(Name, "Completed ShutdownIO");
        }

        public override SearchReturn[] Search(string searchString, bool regEx)
        {
            return null;
        }

        public override string ActionBuildUI(string sUnique, IPlugInAPI.strTrigActInfo actInfo)
        {
            return "";
        }

        public override bool ActionConfigured(IPlugInAPI.strTrigActInfo actInfo)
        {
            return true;
        }

        public override int ActionCount()
        {
            return 0;
        }

        public override string ActionFormatUI(IPlugInAPI.strTrigActInfo actInfo)
        {
            return "";
        }

        public override IPlugInAPI.strMultiReturn ActionProcessPostUI(NameValueCollection postData,
            IPlugInAPI.strTrigActInfo trigInfoIn)
        {
            return new IPlugInAPI.strMultiReturn();
        }

        public override bool ActionReferencesDevice(IPlugInAPI.strTrigActInfo actInfo, int dvRef)
        {
            return false;
        }

        public override string get_ActionName(int actionNumber)
        {
            return "";
        }

        public override bool get_Condition(IPlugInAPI.strTrigActInfo trigInfo)
        {
            return false;
        }

        public override bool get_HasConditions(int triggerNumber)
        {
            return false;
        }

        public override string TriggerBuildUI(string sUnique, IPlugInAPI.strTrigActInfo trigInfo)
        {
            return "";
        }

        public override string TriggerFormatUI(IPlugInAPI.strTrigActInfo trigInfo)
        {
            return "";
        }

        public override IPlugInAPI.strMultiReturn TriggerProcessPostUI(NameValueCollection postData,
            IPlugInAPI.strTrigActInfo trigInfoIn)
        {
            return new IPlugInAPI.strMultiReturn();
        }

        public override bool TriggerReferencesDevice(IPlugInAPI.strTrigActInfo trigInfo, int dvRef)
        {
            return false;
        }

        public override bool TriggerTrue(IPlugInAPI.strTrigActInfo trigInfo)
        {
            return false;
        }

        public override int get_SubTriggerCount(int triggerNumber)
        {
            return 0;
        }

        public override string get_SubTriggerName(int triggerNumber, int subTriggerNumber)
        {
            return "";
        }

        public override bool get_TriggerConfigured(IPlugInAPI.strTrigActInfo trigInfo)
        {
            return true;
        }

        public override string get_TriggerName(int triggerNumber)
        {
            return "";
        }

        public override bool HandleAction(IPlugInAPI.strTrigActInfo actInfo)
        {
            return false;
        }

        public override void set_Condition(IPlugInAPI.strTrigActInfo trigInfo, bool value)
        {
        }

        public override void SpeakIn(int device, string txt, bool w, string host)
        {
        }

        public override string GenPage(string link)
        {
            return "";
        }

        public override string PagePut(string data)
        {
            return "";
        }

        public override string GetPagePlugin(string page, string user, int userRights, string queryString)
        {
            return "";
        }

        public override string PostBackProc(string page, string data, string user, int userRights)
        {
            return "";
        }

        public override string ConfigDevice(int @ref, string user, int userRights, bool newDevice)
        {
            return "";
        }

        public override Enums.ConfigDevicePostReturn ConfigDevicePost(int @ref, string data, string user, int userRights)
        {
            return Enums.ConfigDevicePostReturn.DoneAndCancel;
        }

        public override object PluginFunction(string procName, object[] parms)
        {
            return null;
        }

        public override object PluginPropertyGet(string procName, object[] parms)
        {
            return null;
        }

        public override void PluginPropertySet(string procName, object value)
        {
        }

        protected override bool GetHasTriggers()
        {
            return false;
        }

        protected override int GetTriggerCount()
        {
            return 0;
        }

        protected override string GetName()
        {
            return "$projectname$";
        }

        protected override bool GetHscomPort()
        {
            return true;
        }

        public override void SetDeviceValue(int refId, double value, bool trigger = true)
        {
            
        }
    }
}