using CurrencyApp.Application.Configuration;
using CurrencyApp.Application.Enums;
using CurrencyApp.Application.Interfaces;
using CurrencyApp.Application.Services;
using CurrencyApp.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace CurrencyApp.Tests
{
    [TestFixture]
    public class CurrencyServiceTests
    {
        private Mock<ICurrencyProvider> _providerMock;
        private Mock<ICurrencyProviderFactory> _factoryMock;
        private IMemoryCache _memoryCache;
        IOptions<CurrencySettings> _settings;

        [SetUp]
        public void Setup()
        {
            _factoryMock = new Mock<ICurrencyProviderFactory>();
            _providerMock = new Mock<ICurrencyProvider>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _settings = Options.Create(new CurrencySettings { CacheDurationMinutes = 60 });
        }

        [Test]
        public async Task Should_Calculate_min_max_avg_correctly()
        {
            var data = new List<CurrencyRate>() {
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

            var service = new CurrencyService(_factoryMock.Object, _settings, _memoryCache);

            var result = await service.GetStats(CurrencyApiType.NBP, "EUR", "PLN", DateTime.Today, DateTime.Today);
            result.Value.Should().NotBeNull();
            result.Value.Min.Should().Be(4);
            result.Value.Max.Should().Be(6);
            result.Value.Avg.Should().Be(5);
        }

        [Test]
        public async Task Should_Return_null_if_no_data()
        {
            _providerMock
                .Setup(x => x.GetRatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<CurrencyRate>());

            _factoryMock
                .Setup(x => x.GetProvider(CurrencyApiType.NBP))
                .Returns(_providerMock.Object);

            var service = new CurrencyService(_factoryMock.Object, _settings, _memoryCache);
            var result = await service.GetStats(CurrencyApiType.NBP, "EUR", "PLN", DateTime.Today, DateTime.Today);
            result.IsSuccess.Should().BeFalse();
        }

        [TearDown]
        public void TearDown()
        {
            _memoryCache.Dispose();
        }

        [Test]
        public async Task Should_use_cache_and_not_call_provider_twice()
        {
            var data = new List<Currency>
            {
                new() { Code = "EUR", Name = "Euro" },
                new() { Code = "USD", Name = "US Dollar" }
            };

            _providerMock
                .Setup(x => x.GetCurrenciesAsync())
                .ReturnsAsync(data);

            _factoryMock
                .Setup(x => x.GetProvider(It.IsAny<CurrencyApiType>()))
                .Returns(_providerMock.Object);

            var service = new CurrencyService(
                _factoryMock.Object,
                _settings,
                _memoryCache);

            var result1 = await service.GetCurrencies(CurrencyApiType.NBP);
            var result2 = await service.GetCurrencies(CurrencyApiType.NBP);

            result1.Should().BeEquivalentTo(result2);

            _providerMock.Verify(
                x => x.GetCurrenciesAsync(),
                Times.Once);
        }

        [Test]
        public async Task Should_sort_currencies_by_code_asc_when_configured()
        {
            // ARRANGE
            var data = new List<Currency>
            {
                new() { Code = "USD", Name = "US Dollar" },
                new() { Code = "EUR", Name = "Euro" },
                new() { Code = "GBP", Name = "British Pound" }
            };

            _providerMock
                .Setup(x => x.GetCurrenciesAsync())
                .ReturnsAsync(data);

            _factoryMock
                .Setup(x => x.GetProvider(It.IsAny<CurrencyApiType>()))
                .Returns(_providerMock.Object);

            var settings = Options.Create(new CurrencySettings
            {
                CacheDurationMinutes = 60,
                DefaultSort = CurrencySort.CodeAsc
            });

            var service = new CurrencyService(
                _factoryMock.Object,
                settings,
                _memoryCache);

            var result = await service.GetCurrencies(CurrencyApiType.NBP);

            result.Select(x => x.Code)
                  .Should()
                  .BeInAscendingOrder();
        }

        [Test]
        public async Task Should_sort_currencies_by_code_desc_when_configured()
        {
            var data = new List<Currency>
            {
                new() { Code = "A", Name = "A" },
                new() { Code = "C", Name = "C" },
                new() { Code = "B", Name = "B" }
            };

            _providerMock
                .Setup(x => x.GetCurrenciesAsync())
                .ReturnsAsync(data);

            _factoryMock
                .Setup(x => x.GetProvider(It.IsAny<CurrencyApiType>()))
                .Returns(_providerMock.Object);

            var settings = Options.Create(new CurrencySettings
            {
                CacheDurationMinutes = 60,
                DefaultSort = CurrencySort.CodeDesc
            });

            var service = new CurrencyService(
                _factoryMock.Object,
                settings,
                _memoryCache);

            var result = await service.GetCurrencies(CurrencyApiType.NBP);

            result.Select(x => x.Code)
                  .Should()
                  .BeInDescendingOrder();
        }

        [Test]
        public async Task Should_sort_currencies_by_name_asc_when_configured()
        {
            var data = new List<Currency>
            {
                new() { Code = "USD", Name = "Zebra" },
                new() { Code = "EUR", Name = "Apple" },
                new() { Code = "GBP", Name = "Monkey" }
            };

            _providerMock
                .Setup(x => x.GetCurrenciesAsync())
                .ReturnsAsync(data);

            _factoryMock
                .Setup(x => x.GetProvider(It.IsAny<CurrencyApiType>()))
                .Returns(_providerMock.Object);

            var settings = Options.Create(new CurrencySettings
            {
                CacheDurationMinutes = 60,
                DefaultSort = CurrencySort.NameAsc
            });

            var service = new CurrencyService(
                _factoryMock.Object,
                settings,
                _memoryCache);

            var result = await service.GetCurrencies(CurrencyApiType.NBP);

            result.Select(x => x.Name)
                  .Should()
                  .BeInAscendingOrder();
        }
    }
}
