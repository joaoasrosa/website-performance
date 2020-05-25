using FluentAssertions;
using website_performance.Entities;
using Xunit;

namespace website_performance.Tests.Unit
{
    public class WhenRobots
    {
        [Fact]
        public void GivenRobotsWithNoSitemap_ThenRobotsDoesNotContainSitemapsEvent()
        {
            var invalidRobotsUrl = Given.A.Robots.WithNoSitemaps.Url;
            var invalidHttpMessageHandlerFactory = Given.A.Robots.WithNoSitemaps.HttpMessageHandler;

            var robots = new Robots(
                invalidRobotsUrl,
                invalidHttpMessageHandlerFactory
            );

            var invalidRobotsFile = Record.Exception(() => robots.GeneratePerformanceReport());

            invalidRobotsFile.Should().BeOfType<RobotsDoesNotContainSitemaps>();
        }
    }
}