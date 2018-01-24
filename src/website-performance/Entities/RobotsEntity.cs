using System;
using System.Collections.Generic;

namespace website_performance.Entities
{
    public class RobotsEntity
    {
        private readonly List<Uri> _sitemaps = new List<Uri>();

        public RobotsEntity(string url)
        {
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
    }
}