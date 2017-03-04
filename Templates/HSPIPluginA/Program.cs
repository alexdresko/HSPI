using Hspi;

namespace $safeprojectname$
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Connector.Connect<HSPI>(args);
        }
    }
}