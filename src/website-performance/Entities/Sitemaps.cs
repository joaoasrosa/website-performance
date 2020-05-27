using System;
using System.Collections.Generic;
using System.Linq;
using Value;

namespace website_performance.Entities
{
    public class Sitemaps : ValueType<Sitemaps>
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

            foreach (var sitemap in _sitemaps)
            {
                sitemap.GeneratePerformanceReport();
            }
        }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new[] {new ListByValue<Sitemap>(_sitemaps)};
        }
    }
}