using System.Threading.Tasks;
using WeatherSubscription.Api.Domain.Entities;

namespace WeatherSubscription.Api.Domain.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<Subscription?> GetByEmailAsync(string email);
        Task<Subscription> CreateAsync(Subscription subscription);
        Task<bool> ExistsAsync(string email);
    }
}
