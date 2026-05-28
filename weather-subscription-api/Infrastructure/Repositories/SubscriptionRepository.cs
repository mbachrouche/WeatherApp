using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherSubscription.Api.Domain.Entities;
using WeatherSubscription.Api.Domain.Interfaces;

namespace WeatherSubscription.Api.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        public Task AddAsync(Subscription subscription)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Subscription>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Subscription?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
