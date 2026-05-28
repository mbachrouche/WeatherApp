using System;
using System.Threading.Tasks;
using WeatherSubscription.Api.Domain.Interfaces;

namespace WeatherSubscription.Api.Services
{
    public class SubscriptionService
    {
        private readonly ISubscriptionRepository _repository;

        public SubscriptionService(ISubscriptionRepository repository)
        {
            _repository = repository;
        }

        public Task CreateSubscriptionAsync(string email, string city)
        {
            throw new NotImplementedException();
        }
    }
}
