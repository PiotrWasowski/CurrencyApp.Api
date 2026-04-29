using CurrencyApp.Application.Enums;

namespace CurrencyApp.Application.Configuration
{
    public class CurrencySettings
    {
        public string NbpApiBaseUrl { get; set; }
        public string DateFormat { get; set; }
        public CurrencySort DefaultSort { get; set; }
        public int CacheDurationMinutes { get; set; }
    }
}
