using CurrencyApp.Application.Configuration;
using CurrencyApp.Application.DTOs;
using CurrencyApp.Application.Enums;
using CurrencyApp.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyApp.Application.Services
{
    public class CurrencyService
    {
        private readonly ICurrencyProviderFactory _factory;
        private readonly CurrencySettings _settings;
        private readonly IMemoryCache _cache;

        public CurrencyService(
            ICurrencyProviderFactory factory, 
            IOptions<CurrencySettings> settings,
            IMemoryCache memoryCache)
        {
            _factory = factory;
            _settings = settings.Value;
            _cache = memoryCache;
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
            var cacheKey = $"currencies_{apiType}";

            if (_cache.TryGetValue(cacheKey, out List<CurrencyDto> cached))
            {
                return cached;
            }

            var provider = _factory.GetProvider(apiType);

            var list = await provider.GetCurrenciesAsync();
            var sortedList = ApplySorting(list);

            _cache.Set(cacheKey, sortedList, TimeSpan.FromHours(1));
            return sortedList;
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
