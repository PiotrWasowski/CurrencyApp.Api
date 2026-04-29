using CurrencyApp.Domain.Entities;
using CurrencyApp.Domain.Services;
using FluentAssertions;

namespace CurrencyApp.Tests.Domain
{
    [TestFixture]
    public class CurrencyStatisticsCalculatorTests
    {
        [Test]
        public void Should_calculate_min_max_avg_correctly()
        {
            var rates = new List<CurrencyRate>
            {
                new() { Rate = 4m },
                new() { Rate = 5m },
                new() { Rate = 6m }
            };

            var result = CurrencyStatisticsCalculator.Calculate(rates);

            result.Min.Should().Be(4m);
            result.Max.Should().Be(6m);
            result.Average.Should().Be(5m);
        }

        [Test]
        public void Should_return_zero_values_when_no_rates()
        {
            var rates = new List<CurrencyRate>();

            var result = CurrencyStatisticsCalculator.Calculate(rates);

            result.Min.Should().Be(0);
            result.Max.Should().Be(0);
            result.Average.Should().Be(0);
        }
    }
}