using Hspi;

 namespace HSPI_$safeprojectname${
    internal class Program
    {
        private static void Main(string[] args)
        {
            Connector.Connect<HSPI>(args);
        }
    }
}