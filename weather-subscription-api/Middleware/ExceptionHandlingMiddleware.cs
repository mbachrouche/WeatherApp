using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WeatherSubscription.Api.Exceptions;

namespace WeatherSubscription.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (ex is not ArgumentException and not DuplicateEmailException
                    and not NotFoundException and not WeatherApiException)
                {
                    _logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                        context.Request.Method, context.Request.Path);
                }

                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex switch
            {
                ArgumentException => 400,
                DuplicateEmailException => 409,
                NotFoundException => 404,
                WeatherApiException => 503,
                _ => 500
            };

            var message = ex is Exception e and not ArgumentException
                and not DuplicateEmailException
                and not NotFoundException
                and not WeatherApiException
                ? "An unexpected error occurred."
                : ex.Message;

            var body = JsonSerializer.Serialize(new { error = message });
            return context.Response.WriteAsync(body);
        }
    }
}
