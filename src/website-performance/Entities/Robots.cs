using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Value;

namespace website_performance.Entities
{
    public class Robots : ValueType<Robots>
    {
        private readonly HttpClient _httpClient;
        private readonly Sitemaps _sitemaps;
        private readonly Uri _url;

        public Robots(string url, HttpMessageHandler httpMessageHandler)
        {
            _httpClient = httpMessageHandler == null
                ? throw new ArgumentNullException(nameof(httpMessageHandler))
                : new HttpClient(httpMessageHandler);

            if (Uri.TryCreate(url, UriKind.Absolute, out var robotsUrl))
                _url = robotsUrl;
            else
                throw new UriFormatException($"Fail to parse URL \"{url}\"");

            _sitemaps = new Sitemaps();
        }

        public void GeneratePerformanceReport()
        {
            using (var stream = _httpClient.GetStreamAsync(_url).Result)
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

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new[] {_url};
        }
    }
}