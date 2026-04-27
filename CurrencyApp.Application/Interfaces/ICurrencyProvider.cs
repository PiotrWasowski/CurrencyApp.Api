using CurrencyApp.Application.DTOs;

namespace CurrencyApp.Application.Interfaces
{
    public interface ICurrencyProvider
    {
        Task<List<CurrencyRateDto>> GetRatesAsync(string from, string to, DateTime fromDate, DateTime toDate);
        Task<List<CurrencyDto>> GetCurrenciesAsync();
    }
}
