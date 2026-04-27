using FluentAssertions;

namespace CurrencyApp.Tests
{
    public class SampleTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Should_add_numbers_correctly()
        {
            var result = 2 + 2;

            result.Should().Be(4);
        }
    }
}