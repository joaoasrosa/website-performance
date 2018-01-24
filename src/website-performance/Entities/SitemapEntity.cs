using System;
using System.Collections.Generic;

namespace website_performance.Entities
{
    public class SitemapEntity
    {
        private readonly List<Uri> _pages;

        public SitemapEntity(Uri url)
        {
            Url = url;
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
    }
}