using CurrencyApp.Application.Configuration;
using CurrencyApp.Application.DTOs;
using CurrencyApp.Application.Enums;
using CurrencyApp.Application.Interfaces;
using CurrencyApp.Domain.Entities;
using CurrencyApp.Domain.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

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

        public async Task<CurrencyStatsDto?> GetStats(CurrencyApiType apiType, string from, string to, DateTime fromDate, DateTime toDate)
        {
            var provider = GetProvider(apiType);
            var rates = await provider.GetRatesAsync(from, to, fromDate, toDate);

            if (rates == null || !rates.Any())
            {
                return null;
            }

            var stats = CurrencyStatisticsCalculator.Calculate(rates);

            return new CurrencyStatsDto
            {
                Min = stats.Min,
                Max = stats.Max,
                Avg = stats.Average,
                Rates = rates.Select(r => MapRate(r)).ToList()
            };
        }

        public async Task<List<CurrencyDto>> GetCurrencies(CurrencyApiType apiType)
        {
            var cacheKey = $"currencies:{apiType}";

            if (_cache.TryGetValue(cacheKey, out List<CurrencyDto> cached))
            {
                return cached;
            }

            var provider = GetProvider(apiType);

            var list = await provider.GetCurrenciesAsync();
            var sortedList = ApplySorting(
                 list.Select(x => new CurrencyDto
                 {
                     Code = x.Code,
                     Name = x.Name
                 }).ToList());

            _cache.Set(cacheKey, sortedList, TimeSpan.FromMinutes(_settings.CacheDurationMinutes));
            return sortedList;
        }

        private List<CurrencyDto> ApplySorting(List<CurrencyDto> list)
        {
            return _settings.DefaultSort switch
            {
                CurrencySort.CodeAsc => list.OrderBy(x => x.Code).ToList(),
                CurrencySort.CodeDesc => list.OrderByDescending(x => x.Code).ToList(),
                CurrencySort.NameAsc => list.OrderBy(x => x.Name).ToList(),
                CurrencySort.NameDesc => list.OrderByDescending(x => x.Name).ToList(),
                _ => list
            };
        }

        private ICurrencyProvider GetProvider(CurrencyApiType apiType)
                 => _factory.GetProvider(apiType);

        private CurrencyRateDto MapRate(CurrencyRate rate)
        {
            return new CurrencyRateDto
            {
                Date = rate.Date.ToString(_settings.DateFormat),
                Rate = rate.Rate
            };
        }
    }
}
