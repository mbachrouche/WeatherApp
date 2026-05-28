using System.Threading.Tasks;
using WeatherSubscription.Api.DTOs.Responses;

namespace WeatherSubscription.Api.Domain.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherResponse> GetWeatherForCityAsync(string city);
    }
}
