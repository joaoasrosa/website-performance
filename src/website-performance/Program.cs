using System;
using System.Collections.Generic;
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

                            var robots = ParseRobots(arguments);
                            var sitemaps = ParseSitemaps(robots);

                            if (arguments.Warmup)
                            {
                                ExecuteWarmup(sitemaps);
                            }
                            else
                            {
                                arguments.Validate();
                                ExecutePerformanceTest(
                                    sitemaps,
                                    arguments.Directory,
                                    arguments.ApiKey,
                                    arguments.ApplicationName);
                            }

                            return 0;
                        }
                        catch (Exception e)
                        {
                            if (Log.Logger == null)
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

            Robots ParseRobots(Arguments arguments)
            {
                using (var parseRobotsTxtUseCase = new ParseRobotsTxtUseCase(
                    arguments.Url,
                    new HttpMessageHandlerFactory(),
                    Log.Logger))
                {
                    return parseRobotsTxtUseCase.ParseRobotsTxt();
                }
            }

            IReadOnlyCollection<SitemapEntity> ParseSitemaps(Robots robots)
            {
                var parseSitemapUseCase = new ParseSitemapUseCase(new HttpMessageHandlerFactory(), Log.Logger);
                return parseSitemapUseCase.ParseSitemaps(robots);
            }

            void ExecuteWarmup(IReadOnlyCollection<SitemapEntity> sitemaps)
            {
                var executeWarmupUseCase = new ExecuteWarmupUseCase(
                    new HttpMessageHandlerFactory(), 
                    Log.Logger);
                executeWarmupUseCase.ExecuteWarmup(sitemaps);
            }

            void ExecutePerformanceTest(
                IReadOnlyCollection<SitemapEntity> sitemaps,
                string directory,
                string apiKey,
                string applicationName)
            {
                var executePerformanceTestUseCase =
                    new ExecutePerformanceTestUseCase(Log.Logger, directory, apiKey, applicationName);
                executePerformanceTestUseCase.ExecutePerformanceTest(sitemaps);
            }
        }
    }
}