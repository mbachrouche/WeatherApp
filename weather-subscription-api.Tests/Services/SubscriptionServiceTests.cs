using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using WeatherSubscription.Api.Domain.Entities;
using WeatherSubscription.Api.Domain.Interfaces;
using WeatherSubscription.Api.DTOs.Responses;
using WeatherSubscription.Api.Exceptions;
using WeatherSubscription.Api.Services;
using Xunit;

namespace WeatherSubscription.Api.Tests.Services
{
    public class SubscriptionServiceTests
    {
        [Fact]
        public async Task CreateSubscriptionAsync_WithValidData_ReturnsSubscriptionCreatedResponse()
        {
            // Arrange
            var repositoryMock = new Mock<ISubscriptionRepository>();
            var weatherServiceMock = new Mock<IWeatherService>();
            
            var email = "user@example.com";
            var city = "Berlin";
            var country = "DE";
            var zipCode = "10115";

            var savedSubscription = new Subscription(email, city, country, zipCode);
            // Simulate that EF Core set the Id
            var subscriptionWithId = new Subscription(email, city, country, zipCode);
            var idField = typeof(Subscription).GetProperty("Id");
            idField?.SetValue(subscriptionWithId, 1);

            repositoryMock.Setup(r => r.ExistsAsync(email))
                .ReturnsAsync(false);
            repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Subscription>()))
                .ReturnsAsync(subscriptionWithId);

            var service = new SubscriptionService(repositoryMock.Object, weatherServiceMock.Object, NullLogger<SubscriptionService>.Instance);

            // Act
            var response = await service.CreateSubscriptionAsync(email, city, country, zipCode);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(1);
            response.Email.Should().Be(email);
            response.City.Should().Be(city);
        }

        [Fact]
        public async Task CreateSubscriptionAsync_WithDuplicateEmail_ThrowsDuplicateEmailException()
        {
            // Arrange
            var repositoryMock = new Mock<ISubscriptionRepository>();
            var weatherServiceMock = new Mock<IWeatherService>();

            var email = "user@example.com";
            var city = "Berlin";
            var country = "DE";

            repositoryMock.Setup(r => r.ExistsAsync(email))
                .ReturnsAsync(true);

            var service = new SubscriptionService(repositoryMock.Object, weatherServiceMock.Object, NullLogger<SubscriptionService>.Instance);

            // Act & Assert
            await Assert.ThrowsAsync<DuplicateEmailException>(() =>
                service.CreateSubscriptionAsync(email, city, country, null));
        }

        [Fact]
        public async Task CreateSubscriptionAsync_WithMissingEmail_ThrowsArgumentException()
        {
            // Arrange
            var repositoryMock = new Mock<ISubscriptionRepository>();
            var weatherServiceMock = new Mock<IWeatherService>();

            var service = new SubscriptionService(repositoryMock.Object, weatherServiceMock.Object, NullLogger<SubscriptionService>.Instance);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateSubscriptionAsync("", "Berlin", "DE", null));
        }

        [Fact]
        public async Task CreateSubscriptionAsync_WithMissingCity_ThrowsArgumentException()
        {
            // Arrange
            var repositoryMock = new Mock<ISubscriptionRepository>();
            var weatherServiceMock = new Mock<IWeatherService>();

            var service = new SubscriptionService(repositoryMock.Object, weatherServiceMock.Object, NullLogger<SubscriptionService>.Instance);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateSubscriptionAsync("user@example.com", "", "DE", null));
        }

        [Fact]
        public async Task CreateSubscriptionAsync_WithMissingCountry_ThrowsArgumentException()
        {
            // Arrange
            var repositoryMock = new Mock<ISubscriptionRepository>();
            var weatherServiceMock = new Mock<IWeatherService>();

            var service = new SubscriptionService(repositoryMock.Object, weatherServiceMock.Object, NullLogger<SubscriptionService>.Instance);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateSubscriptionAsync("user@example.com", "Berlin", "", null));
        }

        [Fact]
        public async Task GetWeatherForEmailAsync_WithExistingEmail_ReturnsWeatherResponse()
        {
            // Arrange
            var repositoryMock = new Mock<ISubscriptionRepository>();
            var weatherServiceMock = new Mock<IWeatherService>();

            var email = "user@example.com";
            var subscription = new Subscription(email, "Berlin", "DE", null);

            var weatherResponse = new WeatherResponse
            {
                City = "Berlin",
                Country = "DE",
                Description = "light rain",
                Temperature = new TemperatureResponse
                {
                    Current = 14.2m,
                    Min = 11.0m,
                    Max = 16.5m
                },
                Pressure = 1012,
                Humidity = 78,
                WindSpeed = 5.3m,
                Cloudiness = "Partly Cloudy",
                Sunrise = "05:42 AM",
                Sunset = "09:11 PM"
            };

            repositoryMock.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(subscription);
            weatherServiceMock.Setup(w => w.GetWeatherAsync("Berlin", "DE"))
                .ReturnsAsync(weatherResponse);

            var service = new SubscriptionService(repositoryMock.Object, weatherServiceMock.Object, NullLogger<SubscriptionService>.Instance);

            // Act
            var response = await service.GetWeatherForEmailAsync(email);

            // Assert
            response.Should().NotBeNull();
            response.City.Should().Be("Berlin");
            response.Country.Should().Be("DE");
            response.Description.Should().Be("light rain");
        }

        [Fact]
        public async Task GetWeatherForEmailAsync_WithNonExistingEmail_ThrowsNotFoundException()
        {
            // Arrange
            var repositoryMock = new Mock<ISubscriptionRepository>();
            var weatherServiceMock = new Mock<IWeatherService>();

            var email = "nonexistent@example.com";

            repositoryMock.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync((Subscription?)null);

            var service = new SubscriptionService(repositoryMock.Object, weatherServiceMock.Object, NullLogger<SubscriptionService>.Instance);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.GetWeatherForEmailAsync(email));
        }

        [Fact]
        public async Task GetWeatherForEmailAsync_WhenOWMUnavailable_ThrowsWeatherApiException()
        {
            // Arrange
            var repositoryMock = new Mock<ISubscriptionRepository>();
            var weatherServiceMock = new Mock<IWeatherService>();

            var email = "user@example.com";
            var subscription = new Subscription(email, "Berlin", "DE", null);

            repositoryMock.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(subscription);
            weatherServiceMock.Setup(w => w.GetWeatherAsync("Berlin", "DE"))
                .ThrowsAsync(new WeatherApiException("OpenWeatherMap unavailable"));

            var service = new SubscriptionService(repositoryMock.Object, weatherServiceMock.Object, NullLogger<SubscriptionService>.Instance);

            // Act & Assert
            await Assert.ThrowsAsync<WeatherApiException>(() =>
                service.GetWeatherForEmailAsync(email));
        }
    }
}
