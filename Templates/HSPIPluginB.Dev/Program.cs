using Hspi;

//TODO: namespace HSPI_$safeprojectname$

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