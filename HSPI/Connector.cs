using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using CommandLine;

namespace Hspi
{
    public static class Connector
    {
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "The function wouldn't do anything without a plugin.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "I don't know what kinds of exceptions it _could_ throw.")]
        public static void Connect<TPlugin>(string[] args) where TPlugin : HspiBase, new()
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    // create an instance of our plugin.
                    var myPlugin = new TPlugin();
                    Console.WriteLine(myPlugin.Name);

                    if (Environment.UserInteractive)
                    {
                        Console.Title = myPlugin.Name;
                    }

                    // Get our plugin to connect to HomeSeer
                    Console.WriteLine($"\nConnecting to HomeSeer at {options.Server}:{options.Port} ...");
                    try
                    {
                        myPlugin.Connect(options.Server, options.Port);

                        // got this far then success
                        Console.WriteLine("  connection to HomeSeer successful.\n");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  connection to HomeSeer failed: {ex.Message}");
                        return;
                    }

                    // let the plugin do it's thing, wait until it shuts down or the connection to HomeSeer fails.
                    try
                    {
                        while (true)
                        {
                            // do nothing for a bit
                            Thread.Sleep(200);

                            // test the connection to HomeSeer
                            if (!myPlugin.Connected)
                            {
                                Console.WriteLine("Connection to HomeSeer lost, exiting");
                                break;
                            }

                            // test for a shutdown signal
                            if (myPlugin.IsShuttingDown)
                            {
                                Console.WriteLine("Plugin has been shut down, exiting");
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unhandled exception from Plugin: {ex.Message}");
                    }

                });
            Environment.Exit(0);
        }
    }
}