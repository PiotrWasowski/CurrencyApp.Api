namespace CurrencyApp.Infrastructure.Providers
{
    public class NbpTableResponse
    {
        public List<NbpCurrency> rates { get; set; }
    }

    public class NbpCurrency
    {
        public string code { get; set; }
        public string currency { get; set; }
    }
}
