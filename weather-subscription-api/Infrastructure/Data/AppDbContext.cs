using Microsoft.EntityFrameworkCore;
using WeatherSubscription.Api.Domain.Entities;

namespace WeatherSubscription.Api.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions => Set<Subscription>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.City).IsRequired();
                entity.Property(e => e.Country).IsRequired();
            });
        }
    }
}
