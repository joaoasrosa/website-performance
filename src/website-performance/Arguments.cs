using CommandLine;

namespace website_performance
{
    public class Arguments
    {
        public Arguments(string url, string verbosity)
        {
            Url = url;
            Verbosity = verbosity;
        }

        [Option('u', "url", Required = true, HelpText = "The robots.txt URL.")]

        public string Url { get; }
        
        [Option('v', "verbosity", HelpText = "The logs verbosity.")]

        public string Verbosity { get; }
        
        
    }
}