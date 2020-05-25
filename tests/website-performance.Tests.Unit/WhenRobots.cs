using FluentAssertions;
using website_performance.Entities;
using Xunit;

namespace website_performance.Tests.Unit
{
    public class WhenRobots
    {
        [Fact]
        public void GivenInvalidRobots_ThenInvalidRobotsFileEvent()
        {
            var invalidRobotsUrl = Given.A.InvalidRobots.Url;

            var robots = new Robots(invalidRobotsUrl);

            var invalidRobotsFile = Record.Exception(() => robots.GeneratePerformanceReport());

            invalidRobotsFile.Should().BeOfType<InvalidRobotsFile>();
        }
    }
}