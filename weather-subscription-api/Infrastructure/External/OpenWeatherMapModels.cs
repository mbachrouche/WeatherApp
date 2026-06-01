using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WeatherSubscription.Api.Infrastructure.External
{
    public class OWMResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("sys")]
        public OWMSys Sys { get; set; } = new();

        [JsonPropertyName("weather")]
        public List<OWMWeather> Weather { get; set; } = new();

        [JsonPropertyName("main")]
        public OWMMain Main { get; set; } = new();

        [JsonPropertyName("wind")]
        public OWMWind Wind { get; set; } = new();

        [JsonPropertyName("clouds")]
        public OWMClouds Clouds { get; set; } = new();

        [JsonPropertyName("timezone")]
        public int Timezone { get; set; }
    }

    public class OWMSys
    {
        [JsonPropertyName("country")]
        public string Country { get; set; } = "";

        [JsonPropertyName("sunrise")]
        public long Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public long Sunset { get; set; }
    }

    public class OWMWeather
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = "";
    }

    public class OWMMain
    {
        [JsonPropertyName("temp")]
        public decimal Temp { get; set; }

        [JsonPropertyName("temp_min")]
        public decimal TempMin { get; set; }

        [JsonPropertyName("temp_max")]
        public decimal TempMax { get; set; }

        [JsonPropertyName("pressure")]
        public int Pressure { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }

    public class OWMWind
    {
        [JsonPropertyName("speed")]
        public decimal Speed { get; set; }
    }

    public class OWMClouds
    {
        [JsonPropertyName("all")]
        public int All { get; set; }
    }
}
