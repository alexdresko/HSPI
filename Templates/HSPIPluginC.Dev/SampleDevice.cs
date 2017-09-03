using HomeSeerAPI;
using Hspi.HspiPlugin3;

namespace HSPIPluginC.Dev
{
    public class SampleDevice : Device
    {
        public override int GetId()
        {
            throw new System.NotImplementedException();
        }

        public override bool SetDeviceIo(CAPI.CAPIControl capiControl)
        {
            throw new System.NotImplementedException();
        }

        
        public override IPlugInAPI.PollResultInfo Poll()
        {
            throw new System.NotImplementedException();
        }
    }
}