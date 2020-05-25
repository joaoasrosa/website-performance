using System.Collections.Generic;
using Google.Apis.Pagespeedonline.v1;
using Google.Apis.Services;
using Serilog;
using website_performance.Entities;

namespace website_performance.UseCases
{
    public class ExecutePerformanceTestUseCase
    {
        private readonly string _apiKey;
        private readonly string _applicationName;
        private readonly string _directory;
        private readonly ILogger _logger;

        public ExecutePerformanceTestUseCase(ILogger logger, string directory, string apiKey, string applicationName)
        {
            _logger = logger;
            _directory = directory;
            _apiKey = apiKey;
            _applicationName = applicationName;
        }

        public void ExecutePerformanceTest(IReadOnlyCollection<Sitemap> sitemaps)
        {
            _logger.Debug("Starting the Google Page Speed performance test for {sitemapsCount} sitemap(s).",
                sitemaps.Count);

            var service = new PagespeedonlineService(new BaseClientService.Initializer
            {
                ApiKey = _apiKey,
                ApplicationName = _applicationName
            });

            foreach (var sitemap in sitemaps)
                ExecutePerformanceTestForSitemap(sitemap);

            _logger.Debug("Finished the Google Page Speed performance test for {sitemapsCount} sitemap(s).",
                sitemaps.Count);

            void ExecutePerformanceTestForSitemap(Sitemap sitemap)
            {
                foreach (var page in sitemap.Pages)
                {
                    _logger.Information("Executing the page speed test for {pageUrl}.", page.ToString());
                    
                    var result = service.Pagespeedapi.Runpagespeed(page.ToString()).Execute();
                    
                    _logger.Information("The page result was {score}.", result.Score);
                    _logger.Information("The page report was {@report}", result.FormattedResults);
                }
            }
        }
    }
}