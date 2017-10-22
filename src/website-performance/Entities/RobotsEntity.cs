using System;
using System.Collections.Generic;

namespace website_performance.Entities
{
    public class RobotsEntity
    {
        private readonly List<Uri> _sitemaps = new List<Uri>();

        public IReadOnlyCollection<Uri> Sitemaps => _sitemaps;

        public void AddSitemap(string sitemapUrl)
        {
            if (Uri.TryCreate(sitemapUrl, UriKind.Absolute, out var sitemap))
                _sitemaps.Add(sitemap);
            else
                throw new UriFormatException($"Fail to parse URL \"{sitemapUrl}\"");
        }
    }
}