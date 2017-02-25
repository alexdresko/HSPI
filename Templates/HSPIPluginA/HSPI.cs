using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using HomeSeerAPI;
using HSPI;

namespace $safeprojectname$
{
    public class HSPI : HSPIBase, IPlugInAPI
{
    public override string Name => "$projectname$";

    public override string InstanceFriendlyName()
    {
        return string.Empty;
    }

    public IPlugInAPI.strInterfaceStatus InterfaceStatus()
    {
        return new IPlugInAPI.strInterfaceStatus { intStatus = IPlugInAPI.enumInterfaceStatus.OK };
    }

    public int Capabilities()
    {
        return (int)Enums.eCapabilities.CA_IO;
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
        Hs.WriteLog(Name, "Entering InitIO");

        // initialise everything here, return a blank string only if successful, or an error message


        // debug
        Hs.WriteLog(Name, "Completed InitIO");
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
        Hs.WriteLog(Name, "Entering ShutdownIO");

        // shut everything down here


        // let our console wrapper know we are finished
        Shutdown = true;

        // debug
        Hs.WriteLog(Name, "Completed ShutdownIO");
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
}
}
