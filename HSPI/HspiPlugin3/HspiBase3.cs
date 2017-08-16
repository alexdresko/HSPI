using System;
using System.Collections.Generic;
using System.Linq;
using HomeSeerAPI;

namespace Hspi.HspiPlugin3
{
    public abstract class HspiBase3 : HspiBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public TreeNodeCollection<Action> Actions { get; set; } = new TreeNodeCollection<Action>(null);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public TreeNodeCollection<Trigger> Triggers { get; set; } = new TreeNodeCollection<Trigger>(null);

        public string Port { get; set; }

        protected abstract TreeNodeCollection<Action> GetActions();

        protected abstract List<Device> GetDevices();

        public override string get_ActionName(int actionNumber)
        {
            var action = GetActions().SingleOrDefault(p => p.Data.GetId() == actionNumber);

            return action?.Data?.GetName() ?? string.Empty;
        }

        public override int Capabilities()
        {
            return (int) Enums.eCapabilities.CA_IO |
                   (int) (SupportsSecurityCapability() ? Enums.eCapabilities.CA_Security : 0) |
                   (int) (SupportsThermostatCapability() ? Enums.eCapabilities.CA_Thermostat : 0) |
                   (int) (SupportsSourceSwitchCapability() ? Enums.eCapabilities.CA_SourceSwitch : 0) |
                   (int) (SupportsMusicCapability() ? Enums.eCapabilities.CA_Music : 0);
        }

        public override void ShutdownIO()
        {
            foreach (var device in GetDevices())
            {
                device.Shutdown();
            }

            Shutdown();
        }

        protected abstract void Shutdown();

        public override IPlugInAPI.strInterfaceStatus InterfaceStatus()
        {
            var status = GetInterfaceStatus();
            var result = new IPlugInAPI.strInterfaceStatus {intStatus = status};
            return result;
        }

        public override string InitIO(string port)
        {
            Port = port;
            foreach (var device in GetDevices())
            {
                var init = device.Init();
                if (!string.IsNullOrWhiteSpace(init))
                {
                    return init;
                }
            }
            return InitIO();
        }

        protected abstract TreeNodeCollection<Trigger> GetTriggers();

        protected abstract string InitIO();

        protected abstract IPlugInAPI.enumInterfaceStatus GetInterfaceStatus();

        public override int AccessLevel()
        {
            return (int) GetPluginLicense();
        }

        public override void SetIOMulti(List<CAPI.CAPIControl> colSend)
        {
            if (colSend != null)
            {
                foreach (var capiControl in colSend.Where(p => p != null))
                {
                    var device = GetDevices().SingleOrDefault(p => p.GetId() == capiControl.Ref);
                    if (device != null)
                    {
                        try
                        {
                            var result = device.SetDeviceIo(capiControl);
                            if (result)
                            {
                                HS.SetDeviceValueByRef(capiControl.Ref, capiControl.ControlValue, true);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                }
            }
        }

        public override string InstanceFriendlyName()
        {
            return Name;
        }

        protected abstract LicenseLevel GetPluginLicense();

        protected abstract bool SupportsThermostatCapability();

        protected abstract bool SupportsSourceSwitchCapability();

        protected abstract bool SupportsSecurityCapability();

        protected abstract bool SupportsMusicCapability();

        public override bool get_HasConditions(int triggerNumber)
        {
            var trigger = GetTriggers().SingleOrDefault(p => p.Data.GetId() == triggerNumber);

            return trigger?.Data?.HasConditions() ?? false;
        }

        public override string get_TriggerName(int triggerNumber)
        {
            var trigger = GetTriggers().SingleOrDefault(p => p.Data.GetId() == triggerNumber);

            return trigger?.Data?.GetName() ?? string.Empty;
        }

        public override int get_SubTriggerCount(int triggerNumber)
        {
            var trigger = GetTriggers().SingleOrDefault(p => p.Data.GetId() == triggerNumber);

            return trigger?.Children?.Count ?? 0;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public override object PluginFunction(string functionName, object[] parameters)
        {
            try
            {
                var ty = GetType();
                var mi = ty.GetMethod(functionName);
                if (mi == null)
                {
                    Log("Method " + functionName + " does not exist in this plugin.");
                    return null;
                }

                return mi.Invoke(this, parameters);
            }
            catch (Exception ex)
            {
                Log("Error in PluginProc: " + ex.Message);
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public override object PluginPropertyGet(string propertyName, object[] parameters)
        {
            try
            {
                var ty = GetType();
                var mi = ty.GetProperty(propertyName);
                if (mi == null)
                {
                    Log("Property " + propertyName + " does not exist in this plugin.");
                    return null;
                }

                return mi.GetValue(this, parameters);
            }
            catch (Exception ex)
            {
                Log("Error in PluginPropertyGet: " + ex.Message);
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public override void PluginPropertySet(string propertyName, object value)
        {
            try
            {
                var ty = GetType();
                var mi = ty.GetProperty(propertyName);
                if (mi == null)
                {
                    Log("Property " + propertyName + " does not exist in this plugin.");
                }
                if (mi != null)
                {
                    mi.SetValue(this, value, null);
                }
            }
            catch (Exception ex)
            {
                Log("Error in PluginPropertySet: " + ex.Message);
            }
        }

        private void Log(string message)
        {
            HS.WriteLog(GetName(), message);
        }

        public override int ActionCount()
        {
            return GetActions()?.Count() - 1 ?? 0;
        }

        protected override bool GetHasTriggers()
        {
            return GetTriggers()?.Count() - 1 > 0;
        }


        public override void set_Condition(IPlugInAPI.strTrigActInfo actionInfo, bool value)
        {
            var trigger = TriggerFromTriggerActionInfo(actionInfo);

            if (trigger != null)
            {
                trigger.Data.IsCondition = value;
            }
        }

        private TreeNodeCollection<Trigger> TriggerFromTriggerActionInfo(IPlugInAPI.strTrigActInfo actionInfo)
        {
            var trigger = GetTriggers().SingleOrDefault(p => p.Data.Uid == actionInfo.UID);

            if (trigger == null)
            {
            }
            return trigger;
        }

        public override bool get_Condition(IPlugInAPI.strTrigActInfo actionInfo)
        {
            var trigger = TriggerFromTriggerActionInfo(actionInfo);

            return trigger?.Data?.IsCondition ?? false;
        }


        public override bool TriggerTrue(IPlugInAPI.strTrigActInfo actionInfo)
        {
            var trigger = TriggerFromTriggerActionInfo(actionInfo);

            return trigger?.Data?.Test(actionInfo, trigger) ?? false;
        }

        public override string ActionBuildUI(string uniqueControlId, IPlugInAPI.strTrigActInfo actionInfo)
        {
            var action = ActionFromTriggerActionInfo(actionInfo);

            return action?.Data.BuildUi(uniqueControlId, actionInfo, action) ?? string.Empty;

        }

        private TreeNodeCollection<Action> ActionFromTriggerActionInfo(IPlugInAPI.strTrigActInfo actionInfo)
        {
            var action = GetActions().SingleOrDefault(p => p.Data.Uid == actionInfo.UID);

            return action;
        }
    }
}