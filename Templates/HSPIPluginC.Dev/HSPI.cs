using System.Collections.Generic;
using System.Collections.Specialized;
using HomeSeerAPI;
using Hspi;
using Hspi.HspiPlugin3;
using Hspi.HspiPlugin3.Events;

//TODO: namespace $safeprojectname$

namespace HSPIPluginC.Dev
{
    // ReSharper disable once InconsistentNaming
    public class HSPI : HspiBase3
    {
        public override bool SupportsAddDevice()
        {
            throw new System.NotImplementedException();
        }

        public override bool RaisesGenericCallbacks()
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

        /// <inheritdoc />
        protected override EventContainerBase GetEventContainer() => new EventContainer(this);

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

        protected override bool PluginUsesComPort()
        {
            throw new System.NotImplementedException();
        }

        protected override List<Page> GetPages()
        {
            return new List<Page>
            {
                new Page()
            };
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