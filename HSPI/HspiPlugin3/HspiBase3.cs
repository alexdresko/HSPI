using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using HomeSeerAPI;
using Hspi.Exceptions;
using Hspi.HspiPlugin3.Events;

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
            throw new NotImplementedException();
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
            _eventProcessor = new EventProcessor(GetEventContainer(), Callback, GetName());
            _eventProcessor.Configure();

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

            ThreadPool.QueueUserWorkItem(InitIOWaitCallback);

            return string.Empty;
        }

        public List<Page> Pages { get; set; }

        private void InitIOWaitCallback(object state)
        {
            InitIO();
        }

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
            return GetDevices().SingleOrDefault(p => p.GetId() == deviceId);
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