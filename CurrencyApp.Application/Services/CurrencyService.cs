using CurrencyApp.Application.DTOs;
using CurrencyApp.Application.Interfaces;

namespace CurrencyApp.Application.Services
{
    public class CurrencyService
    {
        private readonly ICurrencyProvider _provider;
        public CurrencyService(ICurrencyProvider provider)
        {
            _provider = provider;
        }

        public async Task<CurrencyStatsDto> GetStats(string from, string to, DateTime fromDate, DateTime toDate)
        {
            var rates = await _provider.GetRatesAsync(from, to, fromDate, toDate);
            return new CurrencyStatsDto
            {
                Min = rates.Min(r => r.Rate),
                Max = rates.Max(r => r.Rate),
                Avg = rates.Average(r => r.Rate),
                Rates = rates
            };
        }
    }
}
