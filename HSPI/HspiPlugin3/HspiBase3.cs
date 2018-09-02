using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HomeSeerAPI;
using Hspi.Exceptions;
using Hspi.HspiPlugin3.Events;
using Scheduler.Classes;

namespace Hspi.HspiPlugin3
{
    public abstract class HspiBase3 : HspiBase
    {
        private EventProcessor _eventProcessor;

        protected abstract TreeNodeCollection<Action> GetActions();

        protected abstract List<Device> GetDevices();

        protected abstract EventContainerBase GetEventContainer();

        protected abstract IPlugInAPI.enumInterfaceStatus GetInterfaceStatus();

        protected abstract List<Page> GetPages();

        protected abstract LicenseLevel GetPluginLicense();

        protected abstract TreeNodeCollection<Trigger> GetTriggers();

        protected abstract void InitIO();

        protected abstract bool PluginUsesComPort();

        protected abstract void Shutdown();

        protected abstract bool SupportsMusicCapability();

        protected abstract bool SupportsSecurityCapability();

        protected abstract bool SupportsSourceSwitchCapability();

        protected abstract bool SupportsThermostatCapability();

        public override int AccessLevel()
        {
            return (int) GetPluginLicense();
        }

        public override string ActionBuildUI(string uniqueControlId, IPlugInAPI.strTrigActInfo actionInfo)
        {
            var action = ActionFromTriggerActionInfo(actionInfo);

            return action?.Data.BuildUi(uniqueControlId, actionInfo, action) ?? string.Empty;
        }

        public override bool ActionConfigured(IPlugInAPI.strTrigActInfo actionInfo)
        {
            var action = ActionFromTriggerActionInfo(actionInfo);

            return action?.Data?.ActionConfigured() ?? false;
        }

        public override int ActionCount()
        {
            return GetActions()?.Count() - 1 ?? 0;
        }

        public override string ActionFormatUI(IPlugInAPI.strTrigActInfo actionInfo)
        {
            var action = ActionFromTriggerActionInfo(actionInfo);

            return action?.Data?.FormatUI() ?? string.Empty;
        }

        public override IPlugInAPI.strMultiReturn ActionProcessPostUI(NameValueCollection postData,
            IPlugInAPI.strTrigActInfo actionInfo)
        {
            throw new NotImplementedException();
        }

        public override bool ActionReferencesDevice(IPlugInAPI.strTrigActInfo actionInfo, int deviceId)
        {
            var action = ActionFromTriggerActionInfo(actionInfo);

            return action?.Data?.ReferencesDevice(deviceId) ?? false;
        }

        public override int Capabilities()
        {
            return (int) Enums.eCapabilities.CA_IO |
                   (int) (SupportsSecurityCapability() ? Enums.eCapabilities.CA_Security : 0) |
                   (int) (SupportsThermostatCapability() ? Enums.eCapabilities.CA_Thermostat : 0) |
                   (int) (SupportsSourceSwitchCapability() ? Enums.eCapabilities.CA_SourceSwitch : 0) |
                   (int) (SupportsMusicCapability() ? Enums.eCapabilities.CA_Music : 0);
        }

        public override string ConfigDevice(int deviceId, string user, int userRights, bool newDevice)
        {
            var device = GetDevices().SingleOrDefault(p => p.Id == deviceId);

            var page = device?.GetConfigPage();
            if (page != null)
            {
                return page.GetPagePlugin(user, userRights, newDevice);
            }

            return string.Empty;
        }

        public override Enums.ConfigDevicePostReturn ConfigDevicePost(int deviceId,
            string data,
            string user,
            int userRights)
        {
            throw new NotImplementedException();
        }

        public override string GenPage(string link)
        {
            //var page = PageByLinkObsoleteMaybe(link);

            //if (page?.UseRedirect() ?? false)
            //{
            //    var aspxFile = page.RedirectUrl();
            //    if (string.IsNullOrWhiteSpace(aspxFile))
            //    {
            //        throw new HspiException("Redirect URL cannot be null or empty");
            //    }

            //    var sb = new StringBuilder();
            //    var data = new StringBuilder();
            //    data.Append("<HEAD>" + Environment.NewLine);
            //    data.Append($"<meta http-equiv=\"refresh\" content=\"0;url={aspxFile}\">" + Environment.NewLine);
            //    data.Append("</HEAD>" + Environment.NewLine);
            //    sb.Append("HTTP/1.0 200 OK" + Environment.NewLine);
            //    sb.Append("Server: HomeSeer" + Environment.NewLine);
            //    sb.Append("Expires: Sun, 22 Mar 1993 16:18:35 GMT" + Environment.NewLine);
            //    sb.Append("Content-Type: text/html" + Environment.NewLine);
            //    sb.Append("Accept-Ranges: bytes" + Environment.NewLine);
            //    sb.Append("Content-Length: " + data.Length + Environment.NewLine + Environment.NewLine);
            //    sb.Append(data);
            //    return sb.ToString();
            //}

            return string.Empty;
        }

        public override string get_ActionName(int actionNumber)
        {
            var action = GetActions().SingleOrDefault(p => p.Data.GetId() == actionNumber);

            return action?.Data?.GetName() ?? string.Empty;
        }

        public override bool get_Condition(IPlugInAPI.strTrigActInfo actionInfo)
        {
            var trigger = TriggerFromTriggerActionInfo(actionInfo);

            return trigger?.Data?.IsCondition ?? false;
        }

        public override bool get_HasConditions(int triggerNumber)
        {
            var trigger = GetTriggers().SingleOrDefault(p => p.Data.GetId() == triggerNumber);

            return trigger?.Data?.HasConditions() ?? false;
        }

        public override int get_SubTriggerCount(int triggerNumber)
        {
            var trigger = GetTriggers().SingleOrDefault(p => p.Data.GetId() == triggerNumber);

            return trigger?.Children?.Count ?? 0;
        }

        public override string get_SubTriggerName(int triggerNumber, int subTriggerNumber)
        {
            throw new NotImplementedException();
        }

        public override bool get_TriggerConfigured(IPlugInAPI.strTrigActInfo actionInfo)
        {
            throw new NotImplementedException();
        }

        public override string get_TriggerName(int triggerNumber)
        {
            var trigger = GetTriggers().SingleOrDefault(p => p.Data.GetId() == triggerNumber);

            return trigger?.Data?.GetName() ?? string.Empty;
        }

        public override string GetPagePlugin(string page, string user, int userRights, string queryString)
        {
            var found = Pages.SingleOrDefault(p => p.GetPageTitle() == page);
            var parts = HttpUtility.ParseQueryString(queryString);
            if (found != null)
            {
                return found.GetPagePlugin(user, userRights, parts) ?? string.Empty;
            }

            return string.Empty;
        }

        public override bool HandleAction(IPlugInAPI.strTrigActInfo actionInfo)
        {
            throw new NotImplementedException();
        }

        public override void HSEvent(Enums.HSEvent eventType, object[] parameters)
        {
            _eventProcessor.Handle(eventType, parameters);
        }

        public override string InitIO(string port)
        {
            Port = port;
            ConfigureEvents();

            ConfigurePages();

            ConfigureDevices();

#pragma warning disable LindhartAnalyserMissingAwaitWarning // Possible missing await keyword
            Task.Run(() => InitIO());
#pragma warning restore LindhartAnalyserMissingAwaitWarning // Possible missing await keyword

            return string.Empty;
        }

        private void ConfigureDevices()
        {
            foreach (var device in GetDevices())
            {
                ConfigureDevice(device);
            }
        }

        private void ConfigureDevice(Device device)
        {
            var deviceRef = HS.NewDeviceRef(device.GetName());
            if (deviceRef > 0)
            {
                var deviceClass = (DeviceClass) HS.GetDeviceByRef(deviceRef);

                if (deviceClass == null)
                {
                    throw new NullReferenceException($"Could not obtain device class for {device.GetName()}");
                }

                device.DeviceClass = deviceClass;
                
                var address = device.GetAddress();
                if (!string.IsNullOrWhiteSpace(address))
                {
                    deviceClass.set_Address(HS, address);
                }

                var deviceTypeInfo = GetDeviceTypeInfo(device);

                deviceClass.set_DeviceType_Set(HS, deviceTypeInfo);

                if (device.IsOwnedByThisPlugin())
                {
                    deviceClass.set_Interface(HS, GetName());
                    deviceClass.set_InterfaceInstance(HS, string.Empty);
                }

                deviceClass.set_Last_Change(HS, DateTime.Now);
                deviceClass.set_Location(HS, device.GetLocation());
                deviceClass.set_Location2(HS, device.GetLocation2());

                var deviceString = device.GetDeviceString();

                if (!string.IsNullOrWhiteSpace(deviceString))
                {
                    HS.SetDeviceString(deviceRef, deviceString, true);
                }

                var deviceValue = device.GetDeviceValue();

                HS.SetDeviceValueByRef(deviceRef, deviceValue, false);

                deviceClass.set_Attention(HS, device.GetAttention());

                deviceClass.set_Status_Support(HS, device.SupportsStatusPolling());

                deviceClass.set_Can_Dim(HS, device.CanDim());

                deviceClass.set_Image(HS, device.GetImage());

                deviceClass.set_ImageLarge(HS, device.GetImageLarge());

                if (device is ScriptDevice scriptDevice)
                {
                    deviceClass.set_ScriptName(HS, scriptDevice.GetScriptName());
                    deviceClass.set_ScriptFunc(HS, scriptDevice.GetScriptFunction());
                }
                else if (deviceTypeInfo.Device_API == DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Script)
                {
                    throw new ApplicationException($"{device.GetName()} must derive from ScriptDevice");
                }
            }
        }

        private void ConfigurePages()
        {
            Pages = GetPages();

            foreach (var page in Pages)
            {
                page.PageName = page.GetPageTitle();
                page.Root = this;

                HS.RegisterPage(page.GetPageTitle(), GetName(), InstanceFriendlyName());

                var webPageDesc = new WebPageDesc
                {
                    linktext = page.GetLinkText(),
                    link = page.GetPageTitle(),
                    page_title = page.GetPageTitle(),
                    plugInName = GetName()
                };

                if (page.RegisterInInterfacesMenu())
                {
                    Callback.RegisterLink(webPageDesc);
                }

                if (page.RegisterInInterfaceConfigPage())
                {
                    Callback.RegisterConfigLink(webPageDesc);
                }
            }
        }

        private void ConfigureEvents()
        {
            _eventProcessor = new EventProcessor(GetEventContainer(), Callback, GetName());
            _eventProcessor.Configure();
        }

        private static DeviceTypeInfo_m.DeviceTypeInfo GetDeviceTypeInfo(Device device)
        {
            var typeString = device.GetDeviceType().ToString();

            var deviceTypeInfo =
                new DeviceTypeInfo_m.DeviceTypeInfo {Device_SubType_Description = device.GetDeviceSubTypeDescription()};

            if (typeString.StartsWith("No_API"))
            {
                deviceTypeInfo.Device_API = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.No_API;
            }

            if (typeString.StartsWith("Plug_In"))
            {
                deviceTypeInfo.Device_API = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Plug_In;
                deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Plugin.Root;
            }

            if (typeString.StartsWith("Security"))
            {
                deviceTypeInfo.Device_API = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Security;

                if (typeString.StartsWith("Security_Alarm"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Alarm;
                }

                if (typeString.StartsWith("Security_Arming"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Arming;
                }

                if (typeString.StartsWith("Security_Communicator"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Communicator;
                }

                if (typeString.StartsWith("Security_Keypad"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Keypad;
                }

                if (typeString.StartsWith("Security_Output_Other"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Output_Other;
                }

                if (typeString.StartsWith("Security_Output_Relay"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Output_Relay;
                }

                if (typeString.StartsWith("Security_Root"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Root;
                }

                if (typeString.StartsWith("Security_Siren"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Siren;
                }

                if (typeString.StartsWith("Security_Zone_Auxiliary"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Zone_Auxiliary;
                }

                if (typeString.StartsWith("Security_Zone_Interior"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Zone_Interior;
                }

                if (typeString.StartsWith("Security_Zone_Interior_Delay"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Zone_Interior_Delay;
                }

                if (typeString.StartsWith("Security_Zone_Other"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Zone_Other;
                }

                if (typeString.StartsWith("Security_Zone_Perimeter"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Zone_Perimeter;
                }

                if (typeString.StartsWith("Security_Zone_Perimeter_Delay"))
                {
                    deviceTypeInfo.Device_Type =
                        (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Zone_Perimeter_Delay;
                }

                if (typeString.StartsWith("Security_Zone_Safety_CO"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Zone_Safety_CO;
                }

                if (typeString.StartsWith("Security_Zone_Safety_CO2"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Zone_Safety_CO2;
                }

                if (typeString.StartsWith("Security_Zone_Safety_Other"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Zone_Safety_Other;
                }

                if (typeString.StartsWith("Security_Zone_Safety_Smoke"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Security.Zone_Safety_Smoke;
                }
            }

            if (typeString.StartsWith("Thermostat"))
            {
                deviceTypeInfo.Device_API = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Thermostat;

                if (typeString.StartsWith("Thermostat_Additional_Temperature"))
                {
                    deviceTypeInfo.Device_Type =
                        (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Additional_Temperature;
                }

                if (typeString.StartsWith("Thermostat_Fan_Mode_Set"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Fan_Mode_Set;
                }

                if (typeString.StartsWith("Thermostat_Fan_Status"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Fan_Status;
                }

                if (typeString.StartsWith("Thermostat_Filter_Remind"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Filter_Remind;
                }

                if (typeString.StartsWith("Thermostat_Hold_Mode"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Hold_Mode;
                }

                if (typeString.StartsWith("Thermostat_Mode_Set"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Mode_Set;
                }

                if (typeString.StartsWith("Thermostat_Operating_Mode"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Operating_Mode;
                }

                if (typeString.StartsWith("Thermostat_Operating_State"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Operating_State;
                }

                if (typeString.StartsWith("Thermostat_Root"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Root;
                }

                if (typeString.StartsWith("Thermostat_RunTime"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.RunTime;
                }

                if (typeString.StartsWith("Thermostat_Setback"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Setback;
                }

                if (typeString.StartsWith("Thermostat_Setpoint"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Setpoint;
                }

                if (typeString.StartsWith("Thermostat_Setpoint_Auto_Changeover"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Setpoint.Auto_Changeover;
                }

                if (typeString.StartsWith("Thermostat_Setpoint_Away_Heating"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Setpoint.Away_Heating;
                }

                if (typeString.StartsWith("Thermostat_Setpoint_Cooling_1"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Setpoint.Cooling_1;
                }

                if (typeString.StartsWith("Thermostat_Setpoint_Dry_Air"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Setpoint.Dry_Air;
                }

                if (typeString.StartsWith("Thermostat_Setpoint_Energy_Save_Cool"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Setpoint.Energy_Save_Cool;
                }

                if (typeString.StartsWith("Thermostat_Setpoint_Energy_Save_Heat"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Setpoint.Energy_Save_Heat;
                }

                if (typeString.StartsWith("Thermostat_Setpoint_Furnace"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Setpoint.Furnace;
                }

                if (typeString.StartsWith("Thermostat_Setpoint_Heating_1"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Setpoint.Heating_1;
                }

                if (typeString.StartsWith("Thermostat_Setpoint_Invalid"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Setpoint.Invalid;
                }

                if (typeString.StartsWith("Thermostat_Setpoint_Moist_Air"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Setpoint.Moist_Air;
                }

                if (typeString.StartsWith("Thermostat_Temperature"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Thermostat.Temperature;
                }

                if (typeString.StartsWith("Thermostat_Temperature__Unused_3"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Temperature._Unused_3;
                }

                if (typeString.StartsWith("Thermostat_Temperature__Unused_4"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Temperature._Unused_4;
                }

                if (typeString.StartsWith("Thermostat_Temperature_Humidity"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Temperature.Humidity;
                }

                if (typeString.StartsWith("Thermostat_Temperature_Other_Temperature"))
                {
                    deviceTypeInfo.Device_Type =
                        (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Temperature.Other_Temperature;
                }

                if (typeString.StartsWith("Thermostat_Temperature_Temperature"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Temperature.Temperature;
                }

                if (typeString.StartsWith("Thermostat_Temperature_Temperature_1"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceSubType_Temperature.Temperature_1;
                }
            }

            if (typeString.StartsWith("Media"))
            {
                deviceTypeInfo.Device_API = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Media;

                if (typeString.StartsWith("Media_Media_Album"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Media.Media_Album;
                }

                if (typeString.StartsWith("Media_Media_Artist"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Media.Media_Artist;
                }

                if (typeString.StartsWith("Media_Media_Genre"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Media.Media_Genre;
                }

                if (typeString.StartsWith("Media_Media_Playlist"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Media.Media_Playlist;
                }

                if (typeString.StartsWith("Media_Media_Selector_Control"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Media.Media_Selector_Control;
                }

                if (typeString.StartsWith("Media_Media_Track"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Media.Media_Track;
                }

                if (typeString.StartsWith("Media_Media_Type"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Media.Media_Type;
                }

                if (typeString.StartsWith("Media_Player_Control"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Media.Player_Control;
                }

                if (typeString.StartsWith("Media_Player_Repeat"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Media.Player_Repeat;
                }

                if (typeString.StartsWith("Media_Player_Shuffle"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Media.Player_Shuffle;
                }

                if (typeString.StartsWith("Media_Player_Status"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Media.Player_Status;
                }

                if (typeString.StartsWith("Media_Player_Status_Additional"))
                {
                    deviceTypeInfo.Device_Type =
                        (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Media.Player_Status_Additional;
                }

                if (typeString.StartsWith("Media_Player_Volume"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Media.Player_Volume;
                }

                if (typeString.StartsWith("Media_Root"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Media.Root;
                }
            }

            if (typeString.StartsWith("SourceSwitch"))
            {
                deviceTypeInfo.Device_API = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.SourceSwitch;
                if (typeString.StartsWith("SourceSwitch_Invalid"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_SourceSwitch.Invalid;
                }

                if (typeString.StartsWith("SourceSwitch_Root"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_SourceSwitch.Root;
                }

                if (typeString.StartsWith("SourceSwitch_Source"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_SourceSwitch.Source;
                }

                if (typeString.StartsWith("SourceSwitch_Source_Extended"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_SourceSwitch.Source_Extended;
                }

                if (typeString.StartsWith("SourceSwitch_System"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_SourceSwitch.System;
                }

                if (typeString.StartsWith("SourceSwitch_Zone"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_SourceSwitch.Zone;
                }

                if (typeString.StartsWith("SourceSwitch_Zone_Extended"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_SourceSwitch.Zone_Extended;
                }
            }

            if (typeString.StartsWith("Script"))
            {
                deviceTypeInfo.Device_API = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Script;

                if (typeString.StartsWith("Script_Disabled"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Script.Disabled;
                }

                if (typeString.StartsWith("Script_Run_On_Any_Change"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Script.Run_On_Any_Change;
                }

                if (typeString.StartsWith("Script_Run_On_String_Change"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Script.Run_On_String_Change;
                }

                if (typeString.StartsWith("Script_Run_On_Value_Change"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Script.Run_On_Value_Change;
                }
            }

            if (typeString.StartsWith("Energy"))
            {
                deviceTypeInfo.Device_API = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Energy;

                if (typeString.StartsWith("Energy_Amps"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Energy.Amps;
                }

                if (typeString.StartsWith("Energy_Graphing"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Energy.Graphing;
                }

                if (typeString.StartsWith("Energy_KWH"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Energy.KWH;
                }

                if (typeString.StartsWith("Energy_Volts"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Energy.Volts;
                }

                if (typeString.StartsWith("Energy_Watts"))
                {
                    deviceTypeInfo.Device_Type = (int) DeviceTypeInfo_m.DeviceTypeInfo.eDeviceType_Energy.Watts;
                }
            }
            return deviceTypeInfo;
        }

        public List<Page> Pages { get; set; }

        public override string InstanceFriendlyName()
        {
            return string.Empty;
        }

        public override IPlugInAPI.strInterfaceStatus InterfaceStatus()
        {
            var status = GetInterfaceStatus();
            var result = new IPlugInAPI.strInterfaceStatus {intStatus = status};
            return result;
        }

        public override string PagePut(string data)
        {
            return null;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
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

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
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

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
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

        public override IPlugInAPI.PollResultInfo PollDevice(int deviceId)
        {
            var device = DeviceFromDeviceId(deviceId);
            return device?.Poll() ?? new IPlugInAPI.PollResultInfo { Result = IPlugInAPI.enumPollResult.Unknown };
        }

        private Device DeviceFromDeviceId(int deviceId)
        {
            return GetDevices().SingleOrDefault(p => p.Id == deviceId);
        }

        public override string PostBackProc(string page, string data, string user, int userRights)
        {
            var found = Pages.SingleOrDefault(p => p.GetPageTitle() == page);

            return found?.postBackProc(page, data, user, userRights) ?? string.Empty;
        }

        public override SearchReturn[] Search(string searchString, bool regEx)
        {
            throw new NotImplementedException();
        }

        public override void set_Condition(IPlugInAPI.strTrigActInfo actionInfo, bool value)
        {
            var trigger = TriggerFromTriggerActionInfo(actionInfo);

            if (trigger != null)
            {
                trigger.Data.IsCondition = value;
            }
        }

        public override void SetIOMulti(List<CAPI.CAPIControl> colSend)
        {
            if (colSend != null)
            {
                foreach (var capiControl in colSend.Where(p => p != null))
                {
                    var device = GetDevices().SingleOrDefault(p => p.Id == capiControl.Ref);
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

        public override void ShutdownIO()
        {
            Shutdown();
        }

        public override void SpeakIn(int deviceId, string text, bool wait, string host)
        {
            throw new NotImplementedException();
        }

        public override string TriggerBuildUI(string uniqueControlId, IPlugInAPI.strTrigActInfo triggerInfo)
        {
            throw new NotImplementedException();
        }

        public override string TriggerFormatUI(IPlugInAPI.strTrigActInfo actionInfo)
        {
            throw new NotImplementedException();
        }

        public override IPlugInAPI.strMultiReturn TriggerProcessPostUI(NameValueCollection postData,
            IPlugInAPI.strTrigActInfo actionInfo)
        {
            throw new NotImplementedException();
        }

        public override bool TriggerReferencesDevice(IPlugInAPI.strTrigActInfo actionInfo, int deviceId)
        {
            throw new NotImplementedException();
        }

        public override bool TriggerTrue(IPlugInAPI.strTrigActInfo actionInfo)
        {
            var trigger = TriggerFromTriggerActionInfo(actionInfo);

            return trigger?.Data?.Test(actionInfo, trigger) ?? false;
        }

        protected override bool GetHasTriggers()
        {
            return GetTriggers()?.Count() - 1 > 0;
        }

        protected override bool GetHscomPort()
        {
            return PluginUsesComPort();
        }

        private TreeNodeCollection<Action> ActionFromTriggerActionInfo(IPlugInAPI.strTrigActInfo actionInfo)
        {
            var action = GetActions().SingleOrDefault(p => p.Data.Uid == actionInfo.UID);

            return action;
        }

        private void Log(string message)
        {
            HS.WriteLog(GetName(), message);
        }

        private TreeNodeCollection<Trigger> TriggerFromTriggerActionInfo(IPlugInAPI.strTrigActInfo actionInfo)
        {
            var trigger = GetTriggers().SingleOrDefault(p => p.Data.Uid == actionInfo.UID);

            if (trigger == null)
            {
            }
            return trigger;
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public TreeNodeCollection<Action> Actions { get; set; } = new TreeNodeCollection<Action>(null);

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public TreeNodeCollection<Trigger> Triggers { get; set; } = new TreeNodeCollection<Trigger>(null);

        public string Port { get; set; }
    }
}