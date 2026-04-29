namespace CurrencyApp.Application.DTOs
{
    public class CurrencyRateDto
    {
        public string Date { get; set; } = default!;
        public decimal Rate { get; set; }
    }
}
