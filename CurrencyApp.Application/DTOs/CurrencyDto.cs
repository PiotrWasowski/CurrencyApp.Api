namespace CurrencyApp.Application.DTOs
{
    public class CurrencyDto
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public string Display => $"{Code} - {Name}";
    }
}
