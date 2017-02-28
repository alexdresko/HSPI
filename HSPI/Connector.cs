using System;
using System.Threading;
using CommandLine;

namespace HSPI
{
    public class Connector
    {
        public static void Connect<TPlugin>(string[] args) where TPlugin : HSPIBase, new()
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                Console.WriteLine("Test Plugin");

                // create an instance of our plugin.
                var myPlugin = new TPlugin();

                // Get our plugin to connect to Homeseer
                Console.WriteLine($"\nConnecting to Homeseer at {options.Server}:{options.Port} ...");
                try
                {
                    myPlugin.Connect(options.Server, options.Port);

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
}