using Hspi;

namespace $safeprojectname$
namespace HSPIPluginB.Dev
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Connector.Connect<HSPI>(args);
        }
    }
}