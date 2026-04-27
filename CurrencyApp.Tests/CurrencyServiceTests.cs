using CurrencyApp.Application.DTOs;
using CurrencyApp.Application.Enums;
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
        private Mock<ICurrencyProviderFactory> _factoryMock;

        [SetUp]
        public void Setup()
        {
            _factoryMock = new Mock<ICurrencyProviderFactory>();
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

            _factoryMock
                .Setup(x => x.GetProvider(CurrencyApiType.NBP))
                .Returns(_providerMock.Object);

            var service = new CurrencyService(_factoryMock.Object);

            var result = await service.GetStats(CurrencyApiType.NBP, "EUR", "PLN", DateTime.Today, DateTime.Today);

            result.Min.Should().Be(4);
            result.Max.Should().Be(6);
            result.Avg.Should().Be(5);
        }

        [Test]
        public async Task Should_Return_null_if_no_data()
        {
            _providerMock
                .Setup(x => x.GetRatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<CurrencyRateDto>());

            _factoryMock
                .Setup(x => x.GetProvider(CurrencyApiType.NBP))
                .Returns(_providerMock.Object);

            var service = new CurrencyService(_factoryMock.Object);
            var result = await service.GetStats(CurrencyApiType.NBP, "EUR", "PLN", DateTime.Today, DateTime.Today);
            result.Should().BeNull();
        }
    }
}
