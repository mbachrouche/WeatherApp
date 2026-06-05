using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherSubscription.Api.Controllers;
using WeatherSubscription.Api.Domain.Interfaces;
using WeatherSubscription.Api.DTOs.Requests;
using WeatherSubscription.Api.DTOs.Responses;
using WeatherSubscription.Api.Exceptions;
using Xunit;

namespace WeatherSubscription.Api.Tests.Controllers
{
    public class SubscriptionsControllerTests
    {
        [Fact]
        public async Task PostSubscription_WithValidData_Returns201WithBody()
        {
            var serviceMock = new Mock<ISubscriptionService>();
            var request = new CreateSubscriptionRequest
            {
                Email = "user@example.com",
                City = "Berlin",
                Country = "DE",
                ZipCode = "10115"
            };

            var expectedResponse = new SubscriptionCreatedResponse
            {
                Id = 1,
                Email = request.Email,
                City = request.City,
                Country = request.Country,
                ZipCode = request.ZipCode,
                CreatedAt = DateTime.UtcNow
            };

            serviceMock.Setup(s => s.CreateSubscriptionAsync(request.Email, request.City, request.Country, request.ZipCode))
                .ReturnsAsync(expectedResponse);

            var controller = new SubscriptionsController(serviceMock.Object);

            var result = await controller.Create(request);

            var createdResult = result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(201);
            createdResult.Value.Should().BeEquivalentTo(expectedResponse);
            createdResult.ActionName.Should().Be(nameof(SubscriptionsController.GetWeather));
            createdResult.RouteValues.Should().ContainKey("email");
            createdResult.RouteValues["email"].Should().Be(request.Email);
        }

        [Fact]
        public async Task PostSubscription_WithInvalidModel_Returns400()
        {
            var serviceMock = new Mock<ISubscriptionService>();
            var controller = new SubscriptionsController(serviceMock.Object);
            controller.ModelState.AddModelError("Email", "Required");

            var request = new CreateSubscriptionRequest
            {
                Email = string.Empty,
                City = "Berlin",
                Country = "DE"
            };

            var result = await controller.Create(request);

            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetWeather_WithExistingEmail_Returns200WithBody()
        {
            var serviceMock = new Mock<ISubscriptionService>();
            var email = "user@example.com";
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

            serviceMock.Setup(s => s.GetWeatherForEmailAsync(email))
                .ReturnsAsync(weatherResponse);

            var controller = new SubscriptionsController(serviceMock.Object);

            var result = await controller.GetWeather(email);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(weatherResponse);
        }

        [Fact]
        public async Task GetWeather_WithNonExistingEmail_WhenNotFoundExceptionThrown_Returns404()
        {
            var serviceMock = new Mock<ISubscriptionService>();
            var email = "missing@example.com";

            serviceMock.Setup(s => s.GetWeatherForEmailAsync(email))
                .ThrowsAsync(new NotFoundException($"Subscription with email '{email}' not found."));

            var controller = new SubscriptionsController(serviceMock.Object);

            Func<Task> act = async () => await controller.GetWeather(email);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetWeather_WhenWeatherApiExceptionThrown_Returns503()
        {
            var serviceMock = new Mock<ISubscriptionService>();
            var email = "owm-error@example.com";

            serviceMock.Setup(s => s.GetWeatherForEmailAsync(email))
                .ThrowsAsync(new WeatherApiException("OpenWeatherMap unavailable"));

            var controller = new SubscriptionsController(serviceMock.Object);

            Func<Task> act = async () => await controller.GetWeather(email);

            await act.Should().ThrowAsync<WeatherApiException>();
        }
    }
}
