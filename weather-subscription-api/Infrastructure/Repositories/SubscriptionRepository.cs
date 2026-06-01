using System.Threading.Tasks;
using WeatherSubscription.Api.Domain.Entities;
using WeatherSubscription.Api.Domain.Interfaces;

namespace WeatherSubscription.Api.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        public Task<Subscription?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Subscription> CreateAsync(Subscription subscription)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}
