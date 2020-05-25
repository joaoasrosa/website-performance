using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Xml;
using Serilog;
using website_performance.Entities;
using website_performance.Infrastructure;

namespace website_performance.UseCases
{
    public class ExecuteWarmupUseCase
    {
        private readonly IHttpMessageHandlerFactory _httpMessageHandlerFactory;
        private readonly ILogger _logger;

        public ExecuteWarmupUseCase(
            IHttpMessageHandlerFactory httpMessageHandlerFactory,
            ILogger logger)
        {
            _httpMessageHandlerFactory = httpMessageHandlerFactory;
            _logger = logger;
        }

        public void ExecuteWarmup(IReadOnlyCollection<Sitemap> sitemaps)
        {
            _logger.Debug("Starting the warmup for {sitemapsCount} sitemap(s).",
                sitemaps.Count);

            foreach (var sitemap in sitemaps)
                ExecuteWarmupForSitemap(sitemap);

            _logger.Debug("Finished the warmup for {sitemapsCount} sitemap(s).",
                sitemaps.Count);

            void ExecuteWarmupForSitemap(Sitemap sitemap)
            {
                _logger.Debug("Starting parse sitemap from \"{sitemapUrl}\".", sitemap.Url);

                foreach (var sitemapPage in sitemap.Pages)
                {
                    _logger.Debug("Fetching page \"{page}\".", sitemapPage.ToString());

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
                    using (var httpClient = new HttpClient(_httpMessageHandlerFactory.Get()))
                    {
                        using (var stream = httpClient.GetStreamAsync(sitemapPage).Result)
                        {
                            stopWatch.Stop();
                            _logger.Debug("Finishing fetch page \"{page}\". Took {ms} ms.", 
                                sitemapPage.ToString(),
                                stopWatch.ElapsedMilliseconds);

                        }
                    }
                }
                
                _logger.Debug("Parse sitemap from \"{sitemapUrl}\" has finished.", sitemap.Url);
            }
        }
    }
}