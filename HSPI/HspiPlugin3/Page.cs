
using System.Collections.Specialized;
using System.Text;
using HomeSeerAPI;
using Scheduler;

namespace Hspi.HspiPlugin3
{
    public abstract class Page : PageBuilder
    {
        // ReSharper disable once PublicConstructorInAbstractClass
        public Page() : base(string.Empty)
        {
            
        }

        public HspiBase3 Root { get; set; }

        public string GetPagePlugin(string user, int userRights, NameValueCollection queryString)
        {
            reset();
            var s = new StringBuilder();
            s = BuildPage(s, user, userRights, queryString);
            AddBody(s.ToString());

            return BuildPage();
        }

        protected abstract StringBuilder BuildPage(StringBuilder s, string user, int userRights, NameValueCollection queryString);

        public abstract bool RegisterInInterfacesMenu();

        public abstract string GetLinkText();

        public abstract string GetPageTitle();

        public abstract bool RegisterInInterfaceConfigPage();

        public override string postBackProc(string page, string data, string user, int userRights)
        {
            return base.postBackProc(page, data, user, userRights);
        }

        public override string postBackProc(ref StateObject state, string Data)
        {
            return base.postBackProc(ref state, Data);
        }
    }
}