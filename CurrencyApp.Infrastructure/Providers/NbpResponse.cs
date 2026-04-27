namespace CurrencyApp.Infrastructure.Providers
{
    public class NbpResponse
    {
        public List<NbpRate> rates { get; set; }
    }

    public class NbpRate
    {
        public DateTime effectiveDate { get; set; }
        public decimal mid { get; set; }
    }
}
