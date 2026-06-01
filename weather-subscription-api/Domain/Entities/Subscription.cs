using System;

namespace WeatherSubscription.Api.Domain.Entities
{
    public class Subscription
    {
        public int Id { get; private set; }
        public string Email { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }
        public string? ZipCode { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Subscription(string email, string city, string country, string? zipCode = null)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City is required.", nameof(city));
            if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country is required.", nameof(country));

            Email = email.Trim().ToLowerInvariant();
            City = city.Trim();
            Country = country.Trim().ToUpperInvariant();
            ZipCode = zipCode?.Trim();
            CreatedAt = DateTime.UtcNow;
        }

        protected Subscription() { } // required by EF Core
    }
}
