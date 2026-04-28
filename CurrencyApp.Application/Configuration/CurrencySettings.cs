namespace CurrencyApp.Application.Configuration
{
    public class CurrencySettings
    {
        public string NbpApiBaseUrl { get; set; }
        public string DateFormat { get; set; }
        public string DefaultSort { get; set; }
        public int CacheDurationMinutes { get; set; }
    }
}
