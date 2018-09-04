﻿// Copyright (C) 2016 SRG Technology, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using System.Collections.Specialized;
using HomeSeerAPI;

namespace Hspi
{
    /// <summary>
    ///     Base class for HSPI plugin.
    ///     <para />
    ///     A new class with the name HSPI should be derived from this base class.
    ///     <list type="number">
    ///         <item>
    ///             <description>The namespace of the new class must be the same as the EXE filename (without extension).</description>
    ///         </item>
    ///         <item>
    ///             <description>This new class must be public, named HSPI, and be in the root of the namespace.</description>
    ///         </item>
    ///     </list>
    ///     <para />
    ///     Adapted from C# sample generated by Marcus Szolkowski.
    ///     See thread "Really simple C# sample plugin available here!" http://board.HomeSeer.com/showthread.php?t=178122.
    /// </summary>
    /// <seealso cref="IPlugInAPI" />
    public abstract class HspiBase2 : HspiBase
    {
        public override IPlugInAPI.strInterfaceStatus InterfaceStatus()
        {
            var s = new IPlugInAPI.strInterfaceStatus {intStatus = IPlugInAPI.enumInterfaceStatus.OK};
            return s;
        }

        public override string InstanceFriendlyName()
        {
            return string.Empty;
        }

        public override int Capabilities()
        {
            return (int) Enums.eCapabilities.CA_IO;
        }

        public override int AccessLevel()
        {
            return 1;
        }

        protected override bool GetHscomPort()
        {
            return true;
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

        public override void HSEvent(Enums.HSEvent eventType, object[] parameters)
        {
        }

        public override string InitIO(string port)
        {
            return "";
        }

        public override IPlugInAPI.PollResultInfo PollDevice(int deviceId)
        {
            var pollResult = new IPlugInAPI.PollResultInfo
            {
                Result = IPlugInAPI.enumPollResult.Device_Not_Found,
                Value = 0
            };

            return pollResult;
        }

        protected override bool GetHasTriggers()
        {
            return false;
        }

        public override void SetIOMulti(List<CAPI.CAPIControl> colSend)
        {
            // HomeSeer will inform us when the one of our devices has changed.  Push that change through to the field.
        }

        public override void ShutdownIO()
        {
            // let our console wrapper know we are finished
        }

        public override SearchReturn[] Search(string searchString, bool regEx)
        {
            return null;
        }

        public override string ActionBuildUI(string uniqueControlId, IPlugInAPI.strTrigActInfo actionInfo)
        {
            return "";
        }

        public override bool ActionConfigured(IPlugInAPI.strTrigActInfo actionInfo)
        {
            return true;
        }

        public override int ActionCount()
        {
            return 0;
        }

        public override string ActionFormatUI(IPlugInAPI.strTrigActInfo actionInfo)
        {
            return "";
        }

        public override IPlugInAPI.strMultiReturn ActionProcessPostUI(NameValueCollection postData,
            IPlugInAPI.strTrigActInfo actionInfo)
        {
            return new IPlugInAPI.strMultiReturn();
        }

        public override bool ActionReferencesDevice(IPlugInAPI.strTrigActInfo actionInfo, int deviceId)
        {
            return false;
        }

        public override string get_ActionName(int actionNumber)
        {
            return "";
        }

        public override bool get_Condition(IPlugInAPI.strTrigActInfo actionInfo)
        {
            return false;
        }

        public override void set_Condition(IPlugInAPI.strTrigActInfo actionInfo, bool value)
        {
        }

        public override bool get_HasConditions(int triggerNumber)
        {
            return false;
        }

        public override string TriggerBuildUI(string uniqueControlId, IPlugInAPI.strTrigActInfo triggerInfo)
        {
            return "";
        }

        public override string TriggerFormatUI(IPlugInAPI.strTrigActInfo actionInfo)
        {
            return "";
        }

        public override IPlugInAPI.strMultiReturn TriggerProcessPostUI(NameValueCollection postData,
            IPlugInAPI.strTrigActInfo actionInfo)
        {
            return new IPlugInAPI.strMultiReturn();
        }

        public override bool TriggerReferencesDevice(IPlugInAPI.strTrigActInfo actionInfo, int deviceId)
        {
            return false;
        }

        public override bool TriggerTrue(IPlugInAPI.strTrigActInfo actionInfo)
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

        public override bool get_TriggerConfigured(IPlugInAPI.strTrigActInfo actionInfo)
        {
            return true;
        }

        public override string get_TriggerName(int triggerNumber)
        {
            return "";
        }

        public override bool HandleAction(IPlugInAPI.strTrigActInfo actionInfo)
        {
            return false;
        }

        public override void SpeakIn(int deviceId, string text, bool wait, string host)
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

        public override string ConfigDevice(int deviceId, string user, int userRights, bool newDevice)
        {
            return "";
        }

        public override Enums.ConfigDevicePostReturn ConfigDevicePost(int deviceId,
            string data,
            string user,
            int userRights)
        {
            return Enums.ConfigDevicePostReturn.DoneAndCancel;
        }

        public override object PluginFunction(string functionName, object[] parameters)
        {
            return null;
        }

        public override object PluginPropertyGet(string propertyName, object[] parameters)
        {
            return null;
        }

        public override void PluginPropertySet(string propertyName, object value)
        {
        }
    }
}