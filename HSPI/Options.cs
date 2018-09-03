using CommandLine;

namespace Hspi
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Options
    {
        [Option('p', "port", HelpText = "HomeSeer admin port", Default = 10400)]
        public int Port { get; set; } = 10400;

        [Option('s', "server", HelpText = "HomeSeer IP address", Default = "127.0.0.1")]
        public string Server { get; set; } = "127.0.0.1";
    }
}