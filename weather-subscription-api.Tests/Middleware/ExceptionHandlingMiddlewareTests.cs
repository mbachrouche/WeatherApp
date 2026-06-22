using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using WeatherSubscription.Api.Exceptions;
using WeatherSubscription.Api.Middleware;
using Xunit;

namespace WeatherSubscription.Api.Tests.Middleware
{
    public class ExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task Middleware_ArgumentException_Returns400WithErrorBody()
        {
            // Arrange
            var exception = new ArgumentException("Email is required");
            var requestDelegate = new Mock<RequestDelegate>();
            requestDelegate.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            var middleware = new ExceptionHandlingMiddleware(requestDelegate.Object, NullLogger<ExceptionHandlingMiddleware>.Instance);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(400);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(body);
            json.GetProperty("error").GetString().Should().Be("Email is required");
        }

        [Fact]
        public async Task Middleware_DuplicateEmailException_Returns409WithErrorBody()
        {
            // Arrange
            var exception = new DuplicateEmailException("Email already exists");
            var requestDelegate = new Mock<RequestDelegate>();
            requestDelegate.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            var middleware = new ExceptionHandlingMiddleware(requestDelegate.Object, NullLogger<ExceptionHandlingMiddleware>.Instance);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(409);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(body);
            json.GetProperty("error").GetString().Should().Be("Email already exists");
        }

        [Fact]
        public async Task Middleware_NotFoundException_Returns404WithErrorBody()
        {
            // Arrange
            var exception = new NotFoundException("Email not found");
            var requestDelegate = new Mock<RequestDelegate>();
            requestDelegate.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            var middleware = new ExceptionHandlingMiddleware(requestDelegate.Object, NullLogger<ExceptionHandlingMiddleware>.Instance);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(404);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(body);
            json.GetProperty("error").GetString().Should().Be("Email not found");
        }

        [Fact]
        public async Task Middleware_WeatherApiException_Returns503WithErrorBody()
        {
            // Arrange
            var exception = new WeatherApiException("OpenWeatherMap unavailable");
            var requestDelegate = new Mock<RequestDelegate>();
            requestDelegate.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            var middleware = new ExceptionHandlingMiddleware(requestDelegate.Object, NullLogger<ExceptionHandlingMiddleware>.Instance);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(503);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(body);
            json.GetProperty("error").GetString().Should().Be("OpenWeatherMap unavailable");
        }

        [Fact]
        public async Task Middleware_UnhandledException_Returns500WithErrorBody()
        {
            // Arrange
            var exception = new Exception("unexpected");
            var requestDelegate = new Mock<RequestDelegate>();
            requestDelegate.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            var middleware = new ExceptionHandlingMiddleware(requestDelegate.Object, NullLogger<ExceptionHandlingMiddleware>.Instance);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(500);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(body);
            json.GetProperty("error").GetString().Should().Be("An unexpected error occurred.");
        }
    }
}
