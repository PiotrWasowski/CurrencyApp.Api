using System.Text.Json.Serialization;

namespace CurrencyApp.Application.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CurrencyApiType
    {
        NBP = 0
    }
}
