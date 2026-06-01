using System.Threading.Tasks;
using WeatherSubscription.Api.Domain.Interfaces;
using WeatherSubscription.Api.DTOs.Responses;

namespace WeatherSubscription.Api.Services
{
    public class WeatherService : IWeatherService
    {
        public Task<WeatherResponse> GetWeatherAsync(string city, string country)
        {
            throw new NotImplementedException();
        }
    }
}
