using Hspi;

//TODO: namespace $safeprojectname$
namespace HSPIPluginA.Dev
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Connector.Connect<HSPI>(args);
        }
    }
}