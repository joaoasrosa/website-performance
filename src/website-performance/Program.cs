using System;
using CommandLine;
using Serilog;
using Serilog.Events;
using website_performance.Entities;
using website_performance.Extensions;
using website_performance.Infrastructure;
using website_performance.UseCases;

namespace website_performance
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Arguments>(args)
                .MapResult(arguments =>
                    {
                        try
                        {
                            SetupLogger(arguments.Verbosity);

                            RobotsEntity robots;

                            using (var parseRobotsTxtUseCase = new ParseRobotsTxtUseCase(
                                arguments.Url,
                                new HttpMessageHandlerFactory(),
                                Log.Logger))
                            {
                                robots = parseRobotsTxtUseCase.ParseRobotsTxt();
                            }

                            return 0;
                        }
                        catch (Exception e)
                        {
                            if(Log.Logger == null)
                                SetupLogger(LogEventLevel.Error.ToString());
                            
                            Log.Logger.Error(e, e.Message);
                            return 1;
                        }
                    },
                    errors =>
                    {
                        SetupLogger(string.Empty);

                        Log.Logger.Fatal("An fatal error occured.");

                        return 1;
                    });

            void SetupLogger(string verbosity)
            {
                Log.Logger = new LoggerConfiguration()
                    .ConfigureMinimumLevel(verbosity)
                    .WriteTo.Console()
                    .CreateLogger();
            }
        }
    }
}