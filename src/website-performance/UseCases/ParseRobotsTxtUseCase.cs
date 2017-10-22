using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Serilog;
using website_performance.Entities;
using website_performance.Infrastructure;

namespace website_performance.UseCases
{
    public class ParseRobotsTxtUseCase : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _robotsUrl;

        public ParseRobotsTxtUseCase(
            string robotsUrl,
            IHttpMessageHandlerFactory httpMessageHandlerFactory,
            ILogger logger)
        {
            _robotsUrl = string.IsNullOrWhiteSpace(robotsUrl)
                ? throw new ArgumentNullException(nameof(robotsUrl))
                : robotsUrl;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpMessageHandlerFactory == null
                ? throw new ArgumentNullException(nameof(httpMessageHandlerFactory))
                : new HttpClient(httpMessageHandlerFactory.Get());
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public RobotsEntity ParseRobotsTxt()
        {
            _logger.Debug("Starting parse robots.txt from {robotsUrl}.", _robotsUrl);

            RobotsEntity robots = null;

            using (var stream = _httpClient.GetStreamAsync(_robotsUrl).Result)
            {
                robots = new RobotsEntity();

                using (var streamReader = new StreamReader(stream))
                {
                    _logger.Debug("Reading robots.txt from \"{robotsUrl}\".", _robotsUrl);

                    while (streamReader.Peek() >= 0)
                    {
                        var line = streamReader.ReadLine();
                        if (!line.StartsWith("Sitemap:"))
                            continue;

                        _logger.Debug("Parsing sitemap \"{sitemap}\" line.", line);

                        var sitemapStrings = line.Split("Sitemap:", StringSplitOptions.RemoveEmptyEntries);
                        var sitemap = sitemapStrings.SingleOrDefault();

                        _logger.Information("Adding sitemap \"{sitemapUrl}\" URL.", sitemap);

                        robots.AddSitemap(sitemap);
                    }
                }
            }

            _logger.Debug("Parse robots.txt \"{robotsUrl}\" has finished.", _robotsUrl);
            
            return robots;
        }
    }
}