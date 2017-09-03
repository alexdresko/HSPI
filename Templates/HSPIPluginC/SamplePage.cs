using System.Collections.Specialized;
using System.Text;
using Hspi.HspiPlugin3;

namespace HSPIPluginC.Dev
{
    public class SamplePage : Page
    {
        public override string GetName()
        {
            return "Test";
        }

        protected override StringBuilder BuildPage(StringBuilder s, string user, int userRights, NameValueCollection queryString)
        {
            s.Append("Neato");
            return s;
        }

        public override bool RegisterInInterfacesMenu()
        {
            return true;
        }

        public override string GetLinkText()
        {
            return "Moo cow";
        }

        public override string GetPageTitle()
        {
            return "King Jaffar!";
        }

        public override bool RegisterInInterfaceConfigPage()
        {
            return true;
        }
    }
}