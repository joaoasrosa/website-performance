using System;
using System.Collections.Generic;
using Value;

namespace website_performance.Entities
{
    public class Sitemap : ValueType<Sitemap>
    {
        private readonly List<Uri> _pages;

        public Sitemap(Uri url)
        {
            Url = url;
            _pages = new List<Uri>();
        }

        private Sitemap(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var sitemapUrl))
                Url = sitemapUrl;
            else
                throw new UriFormatException($"Fail to parse URL \"{url}\"");

            _pages = new List<Uri>();
        }

        public Uri Url { get; }

        public IReadOnlyCollection<Uri> Pages => _pages;

        public void AddPage(string page)
        {
            if (Uri.TryCreate(page, UriKind.Absolute, out var pageUri))
            {
                if (_pages.Contains(pageUri) == false)
                    _pages.Add(pageUri);
            }
            else
            {
                throw new UriFormatException($"Fail to parse URL \"{page}\"");
            }
        }

        public static implicit operator Sitemap(string sitemapUrl) => new Sitemap(sitemapUrl);

        internal void GeneratePerformanceReport()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new[] {Url};
        }
    }
}