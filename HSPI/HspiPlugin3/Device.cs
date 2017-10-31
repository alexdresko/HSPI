using HomeSeerAPI;
using Scheduler.Classes;

namespace Hspi.HspiPlugin3
{
    public abstract class Device
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Io")]
        public abstract bool SetDeviceIo(CAPI.CAPIControl capiControl);

        public abstract IPlugInAPI.PollResultInfo Poll();

        public abstract string GetName();

        public abstract string GetAddress();

        public abstract string GetDeviceSubTypeDescription();

        public abstract DeviceType GetDeviceType();

        public abstract string GetLocation();

        public abstract string GetLocation2();

        public abstract bool IsOwnedByThisPlugin();

        public abstract string GetDeviceString();

        public abstract double GetDeviceValue();

        public abstract string GetAttention();

        public abstract bool SupportsStatusPolling();

        public abstract bool CanDim();

        public abstract string GetImage();

        public abstract string GetImageLarge();

        public DeviceClass DeviceClass { get; set; }

        public int Id { get; set; }

        public abstract DeviceConfigPage GetConfigPage();
    }
}