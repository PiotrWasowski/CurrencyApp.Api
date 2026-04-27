using CurrencyApp.Application.Enums;
using CurrencyApp.Application.Interfaces;

namespace CurrencyApp.Application.Factories
{
    public class CurrencyProviderFactory : ICurrencyProviderFactory
    {
        private readonly Dictionary<CurrencyApiType, ICurrencyProvider> _providers;

        public CurrencyProviderFactory(IEnumerable<ICurrencyProvider> providers)
        {
            _providers = providers.ToDictionary(p => p.GetType().Name switch
            {
                "NbpCurrencyProvider" => CurrencyApiType.NBP,
                _ => throw new NotSupportedException($"Unsupported provider type: {p.GetType().Name}")
            });
        }

        public ICurrencyProvider GetProvider(CurrencyApiType type)
        {
            return _providers[type];
        }
    }
}
