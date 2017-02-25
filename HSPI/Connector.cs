using System;
using System.Threading;

namespace HSPI
{
    public class Connector
    {
        // our homeseer connection details - we can get these from the console arguments too
        private const string ServerAddress = "127.0.0.1";
        private const int ServerPort = 10400;

        // ReSharper disable once UnusedParameter.Local
        public static void Connect<TPlugin>() where TPlugin : HSPIBase, new()
        {
            Console.WriteLine("Test Plugin");

            // create an instance of our plugin.
            var myPlugin = new TPlugin();

            // Get our plugin to connect to Homeseer
            Console.WriteLine("\nConnecting to Homeseer at " + ServerAddress + ":" + ServerPort + " ...");
            try
            {
                myPlugin.Connect(ServerAddress, ServerPort);

                // got this far then success
                Console.WriteLine("  connection to homeseer successful.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("  connection to homeseer failed: " + ex.Message);
                return;
            }

            // let the plugin do it's thing, wait until it shuts down or the connection to homeseer fails.
            try
            {
                while (true)
                {
                    // do nothing for a bit
                    Thread.Sleep(200);

                    // test the connection to homeseer
                    if (!myPlugin.Connected)
                    {
                        Console.WriteLine("Connection to homeseer lost, exiting");
                        break;
                    }

                    // test for a shutdown signal
                    if (myPlugin.Shutdown)
                    {
                        Console.WriteLine("Plugin has been shut down, exiting");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception from Plugin: " + ex.Message);
            }
        }
    }
}