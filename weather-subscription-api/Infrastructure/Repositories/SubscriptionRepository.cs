using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeatherSubscription.Api.Domain.Entities;
using WeatherSubscription.Api.Domain.Interfaces;
using WeatherSubscription.Api.Exceptions;
using WeatherSubscription.Api.Infrastructure.Data;

namespace WeatherSubscription.Api.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _context;

        public SubscriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Subscription?> GetByEmailAsync(string email)
        {
            var normalizedEmail = email.ToLowerInvariant();
            return await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.Email == normalizedEmail);
        }

        public async Task<bool> ExistsAsync(string email)
        {
            var normalizedEmail = email.ToLowerInvariant();
            return await _context.Subscriptions
                .AnyAsync(s => s.Email == normalizedEmail);
        }

        public async Task<Subscription> CreateAsync(Subscription subscription)
        {
            // Application-level duplicate check
            if (await ExistsAsync(subscription.Email))
            {
                throw new DuplicateEmailException($"Email {subscription.Email} is already subscribed");
            }

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }
    }
}
