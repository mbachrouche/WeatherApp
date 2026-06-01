namespace WeatherSubscription.Api.DTOs.Responses
{
    public class WeatherResponse
    {
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TemperatureResponse Temperature { get; set; } = new();
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public decimal WindSpeed { get; set; }
        public string Cloudiness { get; set; } = string.Empty;
        public string Sunrise { get; set; } = string.Empty;
        public string Sunset { get; set; } = string.Empty;
    }

    public class TemperatureResponse
    {
        public decimal Current { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
    }
}
