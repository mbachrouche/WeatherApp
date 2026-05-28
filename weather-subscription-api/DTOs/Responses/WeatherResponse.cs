namespace WeatherSubscription.Api.DTOs.Responses
{
    public class WeatherResponse
    {
        public string City { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal TemperatureCelsius { get; set; }
    }
}
