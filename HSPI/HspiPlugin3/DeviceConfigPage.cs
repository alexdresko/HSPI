using System.Collections.Specialized;
using System.Web;

namespace Hspi.HspiPlugin3
{
    public abstract class DeviceConfigPage : Page
    {
        public string GetPagePlugin(string user, int userRights, bool newDevice)
        {
            IsNewDevice = newDevice;
            return GetPagePlugin(user, userRights, new NameValueCollection());
        }

        public bool IsNewDevice { get; set; }
    }
}