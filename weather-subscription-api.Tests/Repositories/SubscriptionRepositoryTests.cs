using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using WeatherSubscription.Api.Domain.Entities;
using WeatherSubscription.Api.Exceptions;
using WeatherSubscription.Api.Infrastructure.Data;
using WeatherSubscription.Api.Infrastructure.Repositories;

namespace WeatherSubscription.Api.Tests.Repositories
{
    public class SubscriptionRepositoryTests
    {
        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_SavesAndReturnsSubscriptionWithId()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var repo = new SubscriptionRepository(context);
            var subscription = new Subscription("test@example.com", "London", "GB", "SW1A 1AA");

            // Act
            var result = await repo.CreateAsync(subscription);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Email.Should().Be("test@example.com");
            result.City.Should().Be("London");
            result.Country.Should().Be("GB");
            result.ZipCode.Should().Be("SW1A 1AA");
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsSubscription_WhenExists()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var repo = new SubscriptionRepository(context);
            var subscription = new Subscription("john@example.com", "Paris", "FR", "75001");
            await repo.CreateAsync(subscription);

            // Act
            var result = await repo.GetByEmailAsync("john@example.com");

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be("john@example.com");
            result.City.Should().Be("Paris");
            result.Country.Should().Be("FR");
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var repo = new SubscriptionRepository(context);

            // Act
            var result = await repo.GetByEmailAsync("nonexistent@example.com");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenEmailExists()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var repo = new SubscriptionRepository(context);
            var subscription = new Subscription("alice@example.com", "Berlin", "DE", "10115");
            await repo.CreateAsync(subscription);

            // Act
            var result = await repo.ExistsAsync("alice@example.com");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenEmailNotExists()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var repo = new SubscriptionRepository(context);

            // Act
            var result = await repo.ExistsAsync("unknown@example.com");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CreateAsync_ThrowsDuplicateEmailException_WhenEmailAlreadyExists()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var repo = new SubscriptionRepository(context);
            var subscription1 = new Subscription("duplicate@example.com", "Madrid", "ES", "28001");
            await repo.CreateAsync(subscription1);

            var subscription2 = new Subscription("duplicate@example.com", "Barcelona", "ES", "08002");

            // Act & Assert
            await FluentActions.Invoking(() => repo.CreateAsync(subscription2))
                .Should()
                .ThrowAsync<DuplicateEmailException>();
        }
    }
}
