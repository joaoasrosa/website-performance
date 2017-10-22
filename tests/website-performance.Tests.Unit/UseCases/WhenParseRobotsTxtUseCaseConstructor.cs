using System;
using FluentAssertions;
using Moq;
using Serilog;
using website_performance.Infrastructure;
using website_performance.Tests.Unit.Helpers;
using website_performance.UseCases;
using Xunit;

namespace website_performance.Tests.Unit.UseCases
{
    public class WhenParseRobotsTxtUseCaseConstructor
    {
        private const string VALID_URL = "https://anotherlookontech.wordpress.com";
        
        public static readonly TestTheoryData<string> GivenInvalidRobotsUrl =
            new TestTheoryData<string>(new[] {(string)null, "", " "});

        [Theory]
        [MemberData(nameof(GivenInvalidRobotsUrl))]
        public void GivenInvalidRobotsUrl_ThrowsArgumentNullException(string robotsUrl)
        {
            var exception = Record.Exception(() =>
                new ParseRobotsTxtUseCase(robotsUrl, new HttpMessageHandlerFactory(), new Mock<ILogger>().Object));
            
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GivenNullHttpMessageHandlerFactory_ThrowsArgumentNullException()
        {
            var exception = Record.Exception(() =>
                new ParseRobotsTxtUseCase(VALID_URL, null, new Mock<ILogger>().Object));
            
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GivenNullLogger_ThrowsArgumentNullException()
        {
            var exception = Record.Exception(() =>
                new ParseRobotsTxtUseCase(VALID_URL, new HttpMessageHandlerFactory(), null));
            
            exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}