using HomeSeerAPI;
using Hspi.HspiPlugin3;

namespace HSPIPluginC.Sample
{
    public class SampleDevice : Device
    {
        public override bool SetDeviceIo(CAPI.CAPIControl capiControl)
        {
            throw new System.NotImplementedException();
        }

        
        public override IPlugInAPI.PollResultInfo Poll()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override string GetName()
        {
            return "TAco";
        }

        /// <inheritdoc />
        public override string GetAddress()
        {
            return "the address";
        }

        /// <inheritdoc />
        public override string GetDeviceSubTypeDescription()
        {
            return "my sub type description";
        }

        /// <inheritdoc />
        public override DeviceType GetDeviceType()
        {
            return DeviceType.No_API;
        }

        /// <inheritdoc />
        public override string GetLocation()
        {
            return "the location";
        }

        /// <inheritdoc />
        public override string GetLocation2()
        {
            return "location 2";
        }

        /// <inheritdoc />
        public override bool IsOwnedByThisPlugin()
        {
            return true;
        }

        /// <inheritdoc />
        public override string GetDeviceString()
        {
            return null;
        }

        /// <inheritdoc />
        public override double GetDeviceValue()
        {
            return 0;
        }

        /// <inheritdoc />
        public override string GetAttention()
        {
            return "Atteennnttion!";
        }

        /// <inheritdoc />
        public override bool SupportsStatusPolling()
        {
            return true;
        }

        /// <inheritdoc />
        public override bool CanDim()
        {
            return false;
        }

        /// <inheritdoc />
        public override string GetImage()
        {
            return "some image.gif";
        }

        /// <inheritdoc />
        public override string GetImageLarge()
        {
            return "some large image.gif";
        }

        /// <inheritdoc />
        public override DeviceConfigPage GetConfigPage()
        {
            throw new System.NotImplementedException();
        }
    }
}