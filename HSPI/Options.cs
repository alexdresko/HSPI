using CommandLine;
using CommandLine.Text;

namespace Hspi
{
    public class Options
    {
        [Option('p', "port", HelpText = "HomeSeer admin port", DefaultValue = 10400)]
        public int Port { get; set; } = 10400;

        [Option('s', "server", HelpText = "HomeSeer IP address", DefaultValue = "127.0.0.1")]
        public string Server { get; set; } = "127.0.0.1";

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}