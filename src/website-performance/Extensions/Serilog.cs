using System;
using Serilog;
using Serilog.Events;

namespace website_performance.Extensions
{
    internal static class Serilog
    {
        internal static LoggerConfiguration ConfigureMinimumLevel(
            this LoggerConfiguration loggerConfiguration,
            string verbosity)
        {
            if (Enum.TryParse(verbosity, out LogEventLevel verbose))
                loggerConfiguration.MinimumLevel.Is(verbose);
            else
                loggerConfiguration.MinimumLevel.Fatal();

            return loggerConfiguration;
        }
    }
}