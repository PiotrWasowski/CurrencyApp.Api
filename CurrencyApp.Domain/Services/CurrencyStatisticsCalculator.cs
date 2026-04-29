using CurrencyApp.Domain.Entities;

namespace CurrencyApp.Domain.Services
{
    public static class CurrencyStatisticsCalculator
    {
        public static CurrencyStats Calculate(IEnumerable<CurrencyRate> rates)
        {
            var list = rates.ToList();

            if (!list.Any())
                return new CurrencyStats();

            return new CurrencyStats
            {
                Min = list.Min(x => x.Rate),
                Max = list.Max(x => x.Rate),
                Average = list.Average(x => x.Rate)
            };
        }
    }
}
