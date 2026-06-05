using System;

namespace WeatherSubscription.Api.DTOs.Responses
{
    public class SubscriptionCreatedResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? ZipCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
