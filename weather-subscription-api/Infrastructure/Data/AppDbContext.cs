using Microsoft.EntityFrameworkCore;
using WeatherSubscription.Api.Domain.Entities;

namespace WeatherSubscription.Api.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
