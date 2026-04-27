using CurrencyApp.Application.DTOs;
using CurrencyApp.Application.Interfaces;
using CurrencyApp.Application.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CurrencyApp.Tests
{
    public class CurrencyServiceTests
    {
        private Mock<ICurrencyProvider> _providerMock;

        [SetUp]
        public void Setup()
        {
            _providerMock = new Mock<ICurrencyProvider>();
        }

        [Test]
        public async Task Should_Calculate_min_max_avg_correctly()
        {
            var data = new List<CurrencyRateDto>() {
                new() { Date = DateTime.Today, Rate = 4 },
                new() { Date = DateTime.Today, Rate = 5 },
                new() { Date = DateTime.Today, Rate = 6 }
            };

            _providerMock
                .Setup(x => x.GetRatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(data);

            var service = new CurrencyService(_providerMock.Object);

            var result = await service.GetStats("EUR", "PLN", DateTime.Today, DateTime.Today);

            result.Min.Should().Be(4);
            result.Max.Should().Be(6);
            result.Avg.Should().Be(5);
        }
    }
}
