using HomeSeerAPI;

namespace Hspi.HspiPlugin3
{
    public abstract class Device
    {
        public abstract int GetId();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Io")]
        public abstract bool SetDeviceIo(CAPI.CAPIControl capiControl);

        public abstract string Init();

        public abstract void Shutdown();
    }
}