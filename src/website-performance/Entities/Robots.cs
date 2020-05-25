using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace website_performance.Entities
{
    public class Robots
    {
        private readonly List<Uri> _sitemapsList = new List<Uri>();
        private readonly HttpClient _httpClient;
        private readonly Sitemaps _sitemaps; 

        public Robots(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var robotsUrl))
                Url = robotsUrl;
            else
                throw new UriFormatException($"Fail to parse URL \"{url}\"");
        }
        
        public Robots(string url, HttpMessageHandler httpMessageHandler)
        {
            _httpClient = httpMessageHandler == null
                ? throw new ArgumentNullException(nameof(httpMessageHandler))
                : new HttpClient(httpMessageHandler);
            
            if (Uri.TryCreate(url, UriKind.Absolute, out var robotsUrl))
                Url = robotsUrl;
            else
                throw new UriFormatException($"Fail to parse URL \"{url}\"");
            
            _sitemaps = new Sitemaps();
        }

        public Uri Url { get; }

        public IReadOnlyCollection<Uri> Sitemaps => _sitemapsList;

        public void AddSitemap(string sitemapUrl)
        {
            if (Uri.TryCreate(sitemapUrl, UriKind.Absolute, out var sitemap))
            {
                if (_sitemapsList.Contains(sitemap) == false)
                    _sitemapsList.Add(sitemap);
            }
            else
            {
                throw new UriFormatException($"Fail to parse URL \"{sitemapUrl}\"");
            }
        }

        // New code
        public void GeneratePerformanceReport()
        {
            using (var stream = _httpClient.GetStreamAsync(Url).Result)
            {
                using (var streamReader = new StreamReader(stream))
                {

                    while (streamReader.Peek() >= 0)
                    {
                        var line = streamReader.ReadLine();
                        if (!line.StartsWith("Sitemap:"))
                            continue;

                        var sitemapStrings = line.Split("Sitemap:", StringSplitOptions.RemoveEmptyEntries);
                        var sitemap = sitemapStrings.SingleOrDefault();

                        _sitemaps.AddSitemap(sitemap);
                    }
                }
            }

            _sitemaps.GeneratePerformanceReport();
        }
    }
}