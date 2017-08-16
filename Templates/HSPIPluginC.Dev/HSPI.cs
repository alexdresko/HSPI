using System.Collections.Generic;
using System.Collections.Specialized;
using HomeSeerAPI;
using Hspi;
using Hspi.HspiPlugin3;

//TODO: namespace $safeprojectname$

namespace HSPIPluginB.Dev
{
    // ReSharper disable once InconsistentNaming
    public class HSPI : HspiBase3
    {
        public override bool SupportsAddDevice()
        {
            throw new System.NotImplementedException();
        }

        public override string GenPage(string link)
        {
            throw new System.NotImplementedException();
        }

        public override string PagePut(string data)
        {
            throw new System.NotImplementedException();
        }



        public override bool RaisesGenericCallbacks()
        {
            throw new System.NotImplementedException();
        }

        public override IPlugInAPI.PollResultInfo PollDevice(int deviceId)
        {
            throw new System.NotImplementedException();
        }

        public override bool SupportsConfigDevice()
        {
            throw new System.NotImplementedException();
        }

        public override bool SupportsConfigDeviceAll()
        {
            throw new System.NotImplementedException();
        }

        public override Enums.ConfigDevicePostReturn ConfigDevicePost(int deviceId, string data, string user, int userRights)
        {
            throw new System.NotImplementedException();
        }

        public override string ConfigDevice(int deviceId, string user, int userRights, bool newDevice)
        {
            throw new System.NotImplementedException();
        }

        public override SearchReturn[] Search(string searchString, bool regEx)
        {
            throw new System.NotImplementedException();
        }


        public override void SpeakIn(int deviceId, string text, bool wait, string host)
        {
            throw new System.NotImplementedException();
        }

        public override string GetPagePlugin(string page, string user, int userRights, string queryString)
        {
            throw new System.NotImplementedException();
        }

        public override string PostBackProc(string page, string data, string user, int userRights)
        {
            throw new System.NotImplementedException();
        }

        public override string get_SubTriggerName(int triggerNumber, int subTriggerNumber)
        {
            throw new System.NotImplementedException();
        }

        public override bool get_TriggerConfigured(IPlugInAPI.strTrigActInfo actionInfo)
        {
            throw new System.NotImplementedException();
        }

        public override bool TriggerReferencesDevice(IPlugInAPI.strTrigActInfo actionInfo, int deviceId)
        {
            throw new System.NotImplementedException();
        }

        public override string TriggerFormatUI(IPlugInAPI.strTrigActInfo actionInfo)
        {
            throw new System.NotImplementedException();
        }

        public override IPlugInAPI.strMultiReturn TriggerProcessPostUI(NameValueCollection postData, IPlugInAPI.strTrigActInfo actionInfo)
        {
            throw new System.NotImplementedException();
        }

        public override string TriggerBuildUI(string uniqueControlId, IPlugInAPI.strTrigActInfo triggerInfo)
        {
            throw new System.NotImplementedException();
        }

        public override bool HandleAction(IPlugInAPI.strTrigActInfo actionInfo)
        {
            throw new System.NotImplementedException();
        }

        public override bool ActionReferencesDevice(IPlugInAPI.strTrigActInfo actionInfo, int deviceId)
        {
            throw new System.NotImplementedException();
        }

        public override string ActionFormatUI(IPlugInAPI.strTrigActInfo actionInfo)
        {
            throw new System.NotImplementedException();
        }

        public override IPlugInAPI.strMultiReturn ActionProcessPostUI(NameValueCollection postData, IPlugInAPI.strTrigActInfo actionInfo)
        {
            throw new System.NotImplementedException();
        }

        public override bool ActionConfigured(IPlugInAPI.strTrigActInfo actionInfo)
        {
            throw new System.NotImplementedException();
        }
        
        public override void HSEvent(Enums.HSEvent eventType, object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        protected override void Shutdown()
        {
            throw new System.NotImplementedException();
        }

        protected override TreeNodeCollection<Trigger> GetTriggers()
        {
            return Triggers;
        }

        protected override string InitIO()
        {
            throw new System.NotImplementedException();
        }

        protected override IPlugInAPI.enumInterfaceStatus GetInterfaceStatus()
        {
            throw new System.NotImplementedException();
        }

        protected override LicenseLevel GetPluginLicense()
        {
            throw new System.NotImplementedException();
        }

        protected override bool SupportsThermostatCapability()
        {
            throw new System.NotImplementedException();
        }

        protected override bool SupportsSourceSwitchCapability()
        {
            throw new System.NotImplementedException();
        }

        protected override bool SupportsSecurityCapability()
        {
            throw new System.NotImplementedException();
        }

        protected override bool SupportsMusicCapability()
        {
            throw new System.NotImplementedException();
        }

        public override bool SupportsMultipleInstances()
        {
            throw new System.NotImplementedException();
        }

        public override bool SupportsMultipleInstancesSingleEXE()
        {
            throw new System.NotImplementedException();
        }

        protected override string GetName()
        {
            //TODO: return "$projectname$";
            return "HSPIPluginC.Dev";
        }

        protected override bool GetHscomPort()
        {
            throw new System.NotImplementedException();
        }

        protected override TreeNodeCollection<Action> GetActions()
        {
            return Actions;
        }

        protected override List<Device> GetDevices()
        {
            return new List<Device>
            {
                new SampleDevice()
            };
        }
    }
}