using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using WeatherSubscription.Api.Domain.Entities;

namespace WeatherSubscription.Api.Tests.Repositories
{
    public class SubscriptionRepositoryTests
    {
        [Fact]
        public void Constructor_WithValidArgs_SetsAllProperties()
        {
            // Arrange & Act
            var subscription = new Subscription("test@example.com", "Berlin", "DE", "10115");

            // Assert
            subscription.Email.Should().Be("test@example.com");
            subscription.City.Should().Be("Berlin");
            subscription.Country.Should().Be("DE");
            subscription.ZipCode.Should().Be("10115");
            subscription.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Constructor_ZipCodeIsOptional()
        {
            // Arrange & Act
            var subscription = new Subscription("test@example.com", "Berlin", "DE", zipCode: null);

            // Assert
            subscription.ZipCode.Should().BeNull();
            subscription.Email.Should().Be("test@example.com");
            subscription.City.Should().Be("Berlin");
            subscription.Country.Should().Be("DE");
        }

        [Fact]
        public void Constructor_NullOrEmptyEmail_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Subscription(null, "Berlin", "DE", "10115"));
            ex.ParamName.Should().Be("email");

            var ex2 = Assert.Throws<ArgumentException>(() => new Subscription("", "Berlin", "DE", "10115"));
            ex2.ParamName.Should().Be("email");
        }

        [Fact]
        public void Constructor_NullOrEmptyCity_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Subscription("test@example.com", null, "DE", "10115"));
            ex.ParamName.Should().Be("city");

            var ex2 = Assert.Throws<ArgumentException>(() => new Subscription("test@example.com", "", "DE", "10115"));
            ex2.ParamName.Should().Be("city");
        }

        [Fact]
        public void Constructor_NullOrEmptyCountry_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Subscription("test@example.com", "Berlin", null, "10115"));
            ex.ParamName.Should().Be("country");

            var ex2 = Assert.Throws<ArgumentException>(() => new Subscription("test@example.com", "Berlin", "", "10115"));
            ex2.ParamName.Should().Be("country");
        }
    }
}
