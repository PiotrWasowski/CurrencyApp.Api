namespace CurrencyApp.Application.DTOs
{
    public class CurrencyStatsDto
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Avg { get; set; }
        public List<CurrencyRateDto> Rates { get; set; }
    }
}
