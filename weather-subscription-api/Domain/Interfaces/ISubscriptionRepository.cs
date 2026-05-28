using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherSubscription.Api.Domain.Entities;

namespace WeatherSubscription.Api.Domain.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task AddAsync(Subscription subscription);
        Task<Subscription?> GetByIdAsync(Guid id);
        Task<IEnumerable<Subscription>> GetAllAsync();
    }
}
