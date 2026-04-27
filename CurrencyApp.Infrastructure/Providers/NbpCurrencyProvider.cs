using CurrencyApp.Application.DTOs;
using CurrencyApp.Application.Exceptions;
using CurrencyApp.Application.Interfaces;
using System.Net.Http.Json;
using CurrencyApp.Application.Configuration;
using Microsoft.Extensions.Options;

namespace CurrencyApp.Infrastructure.Providers
{
    public class NbpCurrencyProvider: ICurrencyProvider
    {
        private readonly HttpClient _httpClient;
        private readonly CurrencySettings _settings;

        public NbpCurrencyProvider(HttpClient httpClient, IOptions<CurrencySettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<List<CurrencyDto>> GetCurrenciesAsync()
        {
            var response = await _httpClient.GetAsync("exchangerates/tables/A/");

            if (!response.IsSuccessStatusCode)
                throw new ExternalApiException("NBP API error");

            var data = await response.Content.ReadFromJsonAsync<List<NbpTableResponse>>();

            return data.First().rates.Select(r => new CurrencyDto
            {
                Code = r.code,
                Name = r.currency
            }).ToList();
        }

        public async Task<List<CurrencyRateDto>> GetRatesAsync(string from, string to, DateTime fromDate, DateTime toDate)
        {
            if (from == to)
                return new List<CurrencyRateDto>();

            // PLN -> X
            if (from == "PLN")
            {
                var toRates = await GetRates(to, fromDate, toDate);

                return toRates.Select(r => new CurrencyRateDto
                {
                    Date = r.Date,
                    Rate = 1 / r.Rate
                }).ToList();
            }

            // X -> PLN
            if (to == "PLN")
            {
                return await GetRates(from, fromDate, toDate);
            }

            // X -> PLN
            if (to == "PLN")
            {
                return await GetRates(from, fromDate, toDate);
            }

            // X -> Y
            var fromRates = await GetRates(from, fromDate, toDate);
            var toRates2 = await GetRates(to, fromDate, toDate);

            return fromRates.Join(toRates2,
                f => f.Date,
                t => t.Date,
                (f, t) => new CurrencyRateDto
                {
                    Date = f.Date,
                    Rate = f.Rate / t.Rate
                }).ToList();
        }

        private async Task<List<CurrencyRateDto>> GetRates(
            string currency,
            DateTime fromDate,
            DateTime toDate)
        {
            var format = _settings.DateFormat;

            var url = $"exchangerates/rates/A/{currency}/{fromDate.ToString(format)}/{toDate.ToString(format)}/";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return new List<CurrencyRateDto>();

                throw new ExternalApiException("NBP API failed");
            }

            var data = await response.Content.ReadFromJsonAsync<NbpResponse>();

            return data.rates.Select(r => new CurrencyRateDto
            {
                Date = r.effectiveDate,
                Rate = r.mid
            }).ToList();
        }
    }
}
