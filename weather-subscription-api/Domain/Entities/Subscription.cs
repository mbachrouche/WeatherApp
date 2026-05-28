using System;

namespace WeatherSubscription.Api.Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
