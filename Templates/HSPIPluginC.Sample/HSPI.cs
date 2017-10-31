using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using HomeSeerAPI;
using Hspi;
using Hspi.HspiPlugin3;
using Hspi.HspiPlugin3.Events;

//TODO: namespace $safeprojectname$

namespace HSPIPluginC.Sample
{
    // ReSharper disable once InconsistentNaming
    public class HSPI : HspiBase3
    {
        public override bool SupportsAddDevice()
        {
            return false;
        }

        public override bool RaisesGenericCallbacks()
        {
            return false;
        }

        public override bool SupportsConfigDevice()
        {
            return true;
        }

        public override bool SupportsConfigDeviceAll()
        {
            return false;
        }

        protected override void Shutdown()
        {
        }

        protected override TreeNodeCollection<Trigger> GetTriggers()
        {
            return Triggers;
        }

        protected override void InitIO()
        {
        }

        
        protected override EventContainerBase GetEventContainer() => new EventContainer(this);

        protected override IPlugInAPI.enumInterfaceStatus GetInterfaceStatus()
        {
            return IPlugInAPI.enumInterfaceStatus.OK;
        }

        protected override LicenseLevel GetPluginLicense()
        {
            return LicenseLevel.Free;
        }

        protected override bool SupportsThermostatCapability()
        {
            return false;
        }

        protected override bool SupportsSourceSwitchCapability()
        {
            return false;
        }

        protected override bool SupportsSecurityCapability()
        {
            return false;
        }

        protected override bool SupportsMusicCapability()
        {
            return false;
        }

        protected override bool PluginUsesComPort()
        {
            return false;
        }

        protected override List<Page> GetPages()
        {
            return new List<Page>
            {
                new SamplePage()
            };
        }

        public override bool SupportsMultipleInstances()
        {
            return false;
        }

        public override bool SupportsMultipleInstancesSingleEXE()
        {
            return false;
        }

        protected override string GetName()
        {
            //TODO: return "$projectname$";
            return "HSPIPlugin C Sample";
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