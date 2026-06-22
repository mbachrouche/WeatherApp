using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using WeatherSubscription.Api.Exceptions;
using WeatherSubscription.Api.Services;
using Xunit;

namespace WeatherSubscription.Api.Tests.Services
{
    public class WeatherServiceTests
    {
        private readonly IConfiguration _mockConfig;

        public WeatherServiceTests()
        {
            var configBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "OpenWeatherMap:ApiKey", "test-key-123" },
                    { "OpenWeatherMap:BaseUrl", "https://api.openweathermap.org/data/2.5" }
                });
            _mockConfig = configBuilder.Build();
        }

        [Fact]
        public async Task GetWeatherAsync_ReturnsCorrectlyMappedWeather()
        {
            // Arrange
            var mockResponse = new
            {
                name = "Berlin",
                sys = new
                {
                    country = "DE",
                    sunrise = 1622544129L,
                    sunset = 1622601891L
                },
                weather = new[] { new { description = "light rain" } },
                main = new
                {
                    temp = 14.2m,
                    temp_min = 11.0m,
                    temp_max = 16.5m,
                    pressure = 1012,
                    humidity = 78
                },
                wind = new { speed = 5.3m },
                clouds = new { all = 60 },
                timezone = 3600
            };

            var jsonResponse = JsonSerializer.Serialize(mockResponse);
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new WeatherService(httpClient, _mockConfig, NullLogger<WeatherService>.Instance);

            // Act
            var result = await service.GetWeatherAsync("Berlin", "DE");

            // Assert
            result.City.Should().Be("Berlin");
            result.Country.Should().Be("DE");
            result.Description.Should().Be("light rain");
            result.Temperature.Current.Should().Be(14.2m);
            result.Temperature.Min.Should().Be(11.0m);
            result.Temperature.Max.Should().Be(16.5m);
            result.Pressure.Should().Be(1012);
            result.Humidity.Should().Be(78);
            result.WindSpeed.Should().Be(5.3m);
            result.Cloudiness.Should().Be("Partly Cloudy");
            result.Sunrise.Should().NotBeNullOrEmpty();
            result.Sunset.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetWeatherAsync_CloudinessMapping_Clear()
        {
            // Arrange
            var mockResponse = new
            {
                name = "Berlin",
                sys = new { country = "DE", sunrise = 1622544129L, sunset = 1622601891L },
                weather = new[] { new { description = "clear sky" } },
                main = new { temp = 15m, temp_min = 10m, temp_max = 20m, pressure = 1013, humidity = 70 },
                wind = new { speed = 2m },
                clouds = new { all = 10 },
                timezone = 3600
            };

            var jsonResponse = JsonSerializer.Serialize(mockResponse);
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new WeatherService(httpClient, _mockConfig, NullLogger<WeatherService>.Instance);

            // Act
            var result = await service.GetWeatherAsync("Berlin", "DE");

            // Assert
            result.Cloudiness.Should().Be("Clear");
        }

        [Fact]
        public async Task GetWeatherAsync_CloudinessMapping_PartlyCloudy()
        {
            // Arrange
            var mockResponse = new
            {
                name = "Berlin",
                sys = new { country = "DE", sunrise = 1622544129L, sunset = 1622601891L },
                weather = new[] { new { description = "partly cloudy" } },
                main = new { temp = 15m, temp_min = 10m, temp_max = 20m, pressure = 1013, humidity = 70 },
                wind = new { speed = 2m },
                clouds = new { all = 50 },
                timezone = 3600
            };

            var jsonResponse = JsonSerializer.Serialize(mockResponse);
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new WeatherService(httpClient, _mockConfig, NullLogger<WeatherService>.Instance);

            // Act
            var result = await service.GetWeatherAsync("Berlin", "DE");

            // Assert
            result.Cloudiness.Should().Be("Partly Cloudy");
        }

        [Fact]
        public async Task GetWeatherAsync_CloudinessMapping_Overcast()
        {
            // Arrange
            var mockResponse = new
            {
                name = "Berlin",
                sys = new { country = "DE", sunrise = 1622544129L, sunset = 1622601891L },
                weather = new[] { new { description = "overcast" } },
                main = new { temp = 15m, temp_min = 10m, temp_max = 20m, pressure = 1013, humidity = 70 },
                wind = new { speed = 2m },
                clouds = new { all = 80 },
                timezone = 3600
            };

            var jsonResponse = JsonSerializer.Serialize(mockResponse);
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new WeatherService(httpClient, _mockConfig, NullLogger<WeatherService>.Instance);

            // Act
            var result = await service.GetWeatherAsync("Berlin", "DE");

            // Assert
            result.Cloudiness.Should().Be("Overcast");
        }

        [Fact]
        public async Task GetWeatherAsync_CityNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new WeatherService(httpClient, _mockConfig, NullLogger<WeatherService>.Instance);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => service.GetWeatherAsync("NonExistentCity", "XX"));
        }

        [Fact]
        public async Task GetWeatherAsync_ApiUnavailable_ThrowsWeatherApiException()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new WeatherService(httpClient, _mockConfig, NullLogger<WeatherService>.Instance);

            // Act & Assert
            await Assert.ThrowsAsync<WeatherApiException>(
                () => service.GetWeatherAsync("Berlin", "DE"));
        }
    }
}
