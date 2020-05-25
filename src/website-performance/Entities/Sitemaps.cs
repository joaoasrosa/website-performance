using System.Collections.Generic;
using System.Linq;

namespace website_performance.Entities
{
    internal class Sitemaps
    {
        private readonly List<Sitemap> _sitemaps;

        internal Sitemaps()
        {
            _sitemaps = new List<Sitemap>();
        }

        internal void AddSitemap(Sitemap sitemap)
        {
            _sitemaps.Add(sitemap);
        }

        internal void GeneratePerformanceReport()
        {
            if (!_sitemaps.Any())
                throw new RobotsDoesNotContainSitemaps();
        }
    }
}