using System;

namespace WeatherSubscription.Api.Exceptions
{
    public class WeatherApiException : Exception
    {
        public WeatherApiException(string message) : base(message)
        {
        }
    }
}
