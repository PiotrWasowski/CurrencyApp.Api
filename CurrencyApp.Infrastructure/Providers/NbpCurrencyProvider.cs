using CurrencyApp.Application.DTOs;
using CurrencyApp.Application.Exceptions;
using CurrencyApp.Application.Interfaces;
using System.Net.Http.Json;

namespace CurrencyApp.Infrastructure.Providers
{
    public class NbpCurrencyProvider: ICurrencyProvider
    {
        private readonly HttpClient _httpClient;

        public NbpCurrencyProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CurrencyRateDto>> GetRatesAsync(string from, string to, DateTime fromDate, DateTime toDate)
        {
            var url = $"exchangerates/rates/A/{from}/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/";

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
