using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WeatherSubscription.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
