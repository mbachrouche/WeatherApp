using System.Threading.Tasks;
using WeatherSubscription.Api.DTOs.Responses;

namespace WeatherSubscription.Api.Domain.Interfaces
{
    public interface ISubscriptionService
    {
        Task<SubscriptionCreatedResponse> CreateSubscriptionAsync(string email, string city, string country, string? zipCode = null);
        Task<WeatherResponse> GetWeatherForEmailAsync(string email);
    }
}
