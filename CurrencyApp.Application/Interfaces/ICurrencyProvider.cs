using CurrencyApp.Domain.Entities;

namespace CurrencyApp.Application.Interfaces
{
    public interface ICurrencyProvider
    {
        Task<List<CurrencyRate>> GetRatesAsync(string from, string to, DateTime fromDate, DateTime toDate);
        Task<List<Currency>> GetCurrenciesAsync();
    }
}
