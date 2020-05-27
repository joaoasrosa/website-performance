//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.IO;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Xml;
//using Serilog;
//using website_performance.Entities;
//using website_performance.Infrastructure;
//
//namespace website_performance.UseCases
//{
//    public class ParseSitemapUseCase
//    {
//        private readonly IHttpMessageHandlerFactory _httpMessageHandlerFactory;
//        private readonly ILogger _logger;
//
//        public ParseSitemapUseCase(
//            IHttpMessageHandlerFactory httpMessageHandlerFactory,
//            ILogger logger)
//        {
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//            _httpMessageHandlerFactory = httpMessageHandlerFactory ??
//                                         throw new ArgumentNullException(nameof(httpMessageHandlerFactory));
//        }
//
//
//        public IReadOnlyCollection<Sitemap> ParseSitemaps(Robots robots)
//        {
//            _logger.Debug("Starting parse sitemaps from {robotsUrl}.", robots.Url);
//
//            var sitemaps = new ConcurrentBag<Sitemap>();
//
//            var tasks = robots
//                .Sitemaps
//                .Select(
//                    sitemapUrl => Task.Factory.StartNew(
//                        () => sitemaps.Add(ParseSitemap(sitemapUrl))))
//                .ToArray();
//
//            Task.WaitAll(tasks);
//
//            _logger.Debug("Parse sitemaps from \"{robotsUrl}\" has finished.", robots.Url);
//
//            return sitemaps;
//
//            Sitemap ParseSitemap(Uri url)
//            {
//                _logger.Debug("Starting parse sitemap from \"{sitemapUrl}.\"", url);
//
//                var sitemap = new Sitemap(url);
//
//                using (var httpClient = new HttpClient(_httpMessageHandlerFactory.Get()))
//                {
//                    using (var stream = httpClient.GetStreamAsync(url).Result)
//                    {
//                        using (var streamReader = new StreamReader(stream))
//                        {
//                            var xmlDocument = new XmlDocument();
//                            xmlDocument.Load(streamReader);
//
//                            var urlNodes = xmlDocument.GetElementsByTagName("url");
//
//                            foreach (XmlNode urlNode in urlNodes)
//                            {
//                                var pageUrl = urlNode["loc"].InnerText;
//                                _logger.Information("Adding page \"{pageUrl}\" URL.", pageUrl);
//                                sitemap.AddPage(pageUrl);
//                            }
//                        }
//                    }
//                }
//
//                _logger.Debug("Parse sitemap from \"{sitemapUrl}\" has finished.", url);
//
//                return sitemap;
//            }
//        }
//    }
//}