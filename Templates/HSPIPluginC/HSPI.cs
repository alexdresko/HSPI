using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using HomeSeerAPI;
using Hspi;
using Hspi.HspiPlugin3;
using Hspi.HspiPlugin3.Events;
using Action = Hspi.HspiPlugin3.Action;

//TODO: namespace $safeprojectname$

namespace HSPIPluginC.Dev
{
    // ReSharper disable once InconsistentNaming
    public class HSPI : HspiBase3
    {
        public override bool SupportsAddDevice()
        {
            throw new NotImplementedException();
        }

        public override bool RaisesGenericCallbacks()
        {
            throw new NotImplementedException();
        }

        public override bool SupportsConfigDevice()
        {
            throw new NotImplementedException();
        }

        public override bool SupportsConfigDeviceAll()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        
        protected override EventContainerBase GetEventContainer() => new EventContainer(this);

        protected override IPlugInAPI.enumInterfaceStatus GetInterfaceStatus()
        {
            throw new NotImplementedException();
        }

        protected override LicenseLevel GetPluginLicense()
        {
            throw new NotImplementedException();
        }

        protected override bool SupportsThermostatCapability()
        {
            throw new NotImplementedException();
        }

        protected override bool SupportsSourceSwitchCapability()
        {
            throw new NotImplementedException();
        }

        protected override bool SupportsSecurityCapability()
        {
            throw new NotImplementedException();
        }

        protected override bool SupportsMusicCapability()
        {
            throw new NotImplementedException();
        }

        protected override bool PluginUsesComPort()
        {
            throw new NotImplementedException();
        }

        protected override List<Page> GetPages()
        {
            throw new NotImplementedException();
        }

        public override bool SupportsMultipleInstances()
        {
            throw new NotImplementedException();
        }

        public override bool SupportsMultipleInstancesSingleEXE()
        {
            throw new NotImplementedException();
        }

        protected override string GetName()
        {
            //TODO: return "$projectname$";
            return "HSPIPlugin C Dev";
        }

        protected override TreeNodeCollection<Action> GetActions()
        {
            return Actions;
        }

        protected override List<Device> GetDevices()
        {
            throw new NotImplementedException();
        }
    }
}