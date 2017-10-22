using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Serilog;
using website_performance.Infrastructure;
using website_performance.Tests.Unit.Stubs;
using website_performance.UseCases;
using Xunit;

namespace website_performance.Tests.Unit.UseCases
{
    public class WhenParsingRobotsTxt
    {
        private const string VALID_URL = "https://anotherlookontech.wordpress.com";

        private ParseRobotsTxtUseCase CreateSut(string serializedObject)
        {
            var httpMessageHandlerFactoryMock = new Mock<IHttpMessageHandlerFactory>();
            httpMessageHandlerFactoryMock.Setup(x => x.Get())
                .Returns(new HttpMessageHandlerStub(HandleWithOkStatus(serializedObject)));

            return new ParseRobotsTxtUseCase(VALID_URL, httpMessageHandlerFactoryMock.Object,
                new Mock<ILogger>().Object);
        }

        private static Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> HandleWithOkStatus(
            string serializedObject)
        {
            return (message, token) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(serializedObject),
                RequestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri(VALID_URL)
                }
            });
        }

        [Fact]
        public void GivenValidRobotsUrl_ReturnsValidSitemaps()
        {
            var sut = CreateSut($"# Sitemap{Environment.NewLine}" +
                                $"Sitemap: {VALID_URL}/sitemap_pt.xml{Environment.NewLine}" +
                                $"Sitemap: {VALID_URL}/sitemap_en.xml{Environment.NewLine}" +
                                "# Basic Crawler setup. We allow for everything{Environment.NewLine}" +
                                $"User-agent: *{Environment.NewLine}" +
                                "Allow: /");

            var robots = sut.ParseRobotsTxt();

            robots.Sitemaps.Should().HaveCount(2);
        }
    }
}