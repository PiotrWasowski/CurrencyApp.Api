using CurrencyApp.Application.Enums;

namespace CurrencyApp.Application.Interfaces
{
    public interface ICurrencyProviderFactory
    {
        ICurrencyProvider GetProvider(CurrencyApiType type);
    }
}
