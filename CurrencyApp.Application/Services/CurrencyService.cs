using CurrencyApp.Application.DTOs;
using CurrencyApp.Application.Enums;
using CurrencyApp.Application.Interfaces;

namespace CurrencyApp.Application.Services
{
    public class CurrencyService
    {
        private readonly ICurrencyProviderFactory _factory;
        public CurrencyService(ICurrencyProviderFactory factory)
        {
            _factory = factory;
        }

        public async Task<CurrencyStatsDto> GetStats(CurrencyApiType apiType, string from, string to, DateTime fromDate, DateTime toDate)
        {
            var provider = _factory.GetProvider(apiType);
            var rates = await provider.GetRatesAsync(from, to, fromDate, toDate);

            if (rates == null || !rates.Any())
            {
                return null;
            }

            return new CurrencyStatsDto
            {
                Min = rates.Min(r => r.Rate),
                Max = rates.Max(r => r.Rate),
                Avg = rates.Average(r => r.Rate),
                Rates = rates
            };
        }

        public async Task<List<CurrencyDto>> GetCurrencies(CurrencyApiType apiType)
        {
            var provider = _factory.GetProvider(apiType);

            return await provider.GetCurrenciesAsync();
        }
    }
}
