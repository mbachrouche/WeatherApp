using System.Threading.Tasks;
using WeatherSubscription.Api.Domain.Interfaces;
using WeatherSubscription.Api.DTOs.Responses;

namespace WeatherSubscription.Api.Services
{
    public class WeatherService : IWeatherService
    {
        public WeatherService()
        {
        }

        public Task<WeatherResponse> GetWeatherForCityAsync(string city)
        {
            throw new System.NotImplementedException();
        }
    }
}
