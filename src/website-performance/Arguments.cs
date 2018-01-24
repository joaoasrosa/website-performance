using System;
using CommandLine;

namespace website_performance
{
    public class Arguments
    {
        public Arguments(
            string url,
            string verbosity,
            string directory,
            string apiKey,
            string applicationName,
            bool warmup)
        {
            Url = url;
            Verbosity = verbosity;
            Directory = directory;
            ApiKey = apiKey;
            ApplicationName = applicationName;
            Warmup = warmup;
        }

        [Option('u', "url", Required = true, HelpText = "The robots.txt URL.")]
        public string Url { get; }

        [Option('v', "verbosity", HelpText = "The logs verbosity.")]
        public string Verbosity { get; }

        [Option('d', "directory", HelpText = "The directory where performance reports are stored.")]
        public string Directory { get; }

        [Option('a', "apikey", HelpText = "The Google Page Speed API Key.")]
        public string ApiKey { get; }

        [Option('n', "applicationname", HelpText = "The Application Name.")]
        public string ApplicationName { get; }

        [Option('w', "warmup", HelpText = "Warmup the website.")]
        public bool Warmup { get; }

        internal void Validate()
        {
            if (Warmup) return;

            if (string.IsNullOrWhiteSpace(Directory) ||
                string.IsNullOrWhiteSpace(ApiKey) ||
                string.IsNullOrWhiteSpace(ApplicationName))
                throw new Exception("The Google Page Speed parameters are missing.");
        }
    }
}