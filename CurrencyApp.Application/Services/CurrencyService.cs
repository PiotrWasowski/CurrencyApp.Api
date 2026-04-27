using CurrencyApp.Application.Configuration;
using CurrencyApp.Application.DTOs;
using CurrencyApp.Application.Enums;
using CurrencyApp.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Runtime;

namespace CurrencyApp.Application.Services
{
    public class CurrencyService
    {
        private readonly ICurrencyProviderFactory _factory;
        private readonly CurrencySettings _settings;

        public CurrencyService(ICurrencyProviderFactory factory, IOptions<CurrencySettings> settings)
        {
            _factory = factory;
            _settings = settings.Value;
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

            var list = await provider.GetCurrenciesAsync();
            return ApplySorting(list);
        }

        private List<CurrencyDto> ApplySorting(List<CurrencyDto> list)
        {
            return _settings.DefaultSort switch
            {
                "CodeAsc" => list.OrderBy(x => x.Code).ToList(),
                "CodeDesc" => list.OrderByDescending(x => x.Code).ToList(),
                "NameAsc" => list.OrderBy(x => x.Name).ToList(),
                "NameDesc" => list.OrderByDescending(x => x.Name).ToList(),
                _ => list
            };
        }
    }
}
