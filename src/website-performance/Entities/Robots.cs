using System;
using System.Collections.Generic;
using website_performance.Infrastructure;

namespace website_performance.Entities
{
    public class Robots
    {
        private readonly IHttpMessageHandlerFactory _httpMessageHandlerFactory;
        private readonly List<Uri> _sitemaps = new List<Uri>();

        public Robots(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var robotsUrl))
                Url = robotsUrl;
            else
                throw new UriFormatException($"Fail to parse URL \"{url}\"");
        }
        
        public Robots(string url, IHttpMessageHandlerFactory httpMessageHandlerFactory)
        {
            _httpMessageHandlerFactory = httpMessageHandlerFactory ?? throw new ArgumentNullException(nameof(httpMessageHandlerFactory));
            
            if (Uri.TryCreate(url, UriKind.Absolute, out var robotsUrl))
                Url = robotsUrl;
            else
                throw new UriFormatException($"Fail to parse URL \"{url}\"");
        }

        public Uri Url { get; }

        public IReadOnlyCollection<Uri> Sitemaps => _sitemaps;

        public void AddSitemap(string sitemapUrl)
        {
            if (Uri.TryCreate(sitemapUrl, UriKind.Absolute, out var sitemap))
            {
                if (_sitemaps.Contains(sitemap) == false)
                    _sitemaps.Add(sitemap);
            }
            else
            {
                throw new UriFormatException($"Fail to parse URL \"{sitemapUrl}\"");
            }
        }

        // New code
        public void GeneratePerformanceReport()
        {
            throw new NotImplementedException();
        }
    }
}