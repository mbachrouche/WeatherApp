using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WeatherSubscription.Api.Domain.Interfaces;
using WeatherSubscription.Api.DTOs.Responses;
using WeatherSubscription.Api.Exceptions;
using WeatherSubscription.Api.Infrastructure.External;

namespace WeatherSubscription.Api.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<WeatherResponse> GetWeatherAsync(string city, string country)
        {
            var apiKey = _configuration["OpenWeatherMap:ApiKey"];
            var baseUrl = _configuration["OpenWeatherMap:BaseUrl"];

            var url = $"{baseUrl}/weather?q={Uri.EscapeDataString(city)},{Uri.EscapeDataString(country)}&appid={apiKey}&units=metric";

            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException("City not found");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new WeatherApiException("Weather service unavailable");
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var owmResponse = JsonSerializer.Deserialize<OWMResponse>(jsonContent);

            if (owmResponse == null)
            {
                throw new WeatherApiException("Failed to parse weather response");
            }

            var cloudiness = MapCloudiness(owmResponse.Clouds.All);
            var sunrise = FormatUnixTimestamp(owmResponse.Sys.Sunrise, owmResponse.Timezone);
            var sunset = FormatUnixTimestamp(owmResponse.Sys.Sunset, owmResponse.Timezone);

            return new WeatherResponse
            {
                City = owmResponse.Name,
                Country = owmResponse.Sys.Country,
                Description = owmResponse.Weather.Count > 0 ? owmResponse.Weather[0].Description : "",
                Temperature = new TemperatureResponse
                {
                    Current = owmResponse.Main.Temp,
                    Min = owmResponse.Main.TempMin,
                    Max = owmResponse.Main.TempMax
                },
                Pressure = owmResponse.Main.Pressure,
                Humidity = owmResponse.Main.Humidity,
                WindSpeed = owmResponse.Wind.Speed,
                Cloudiness = cloudiness,
                Sunrise = sunrise,
                Sunset = sunset
            };
        }

        private string MapCloudiness(int cloudPercentage)
        {
            if (cloudPercentage <= 25)
                return "Clear";
            if (cloudPercentage <= 75)
                return "Partly Cloudy";
            return "Overcast";
        }

        private string FormatUnixTimestamp(long unixTimestamp, int timezoneOffset)
        {
            var dateTime = UnixTimeStampToDateTime(unixTimestamp);
            var adjustedTime = dateTime.AddSeconds(timezoneOffset);
            return adjustedTime.ToString("hh:mm tt");
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime;
        }
    }
}
