using System.ComponentModel.DataAnnotations;

namespace WeatherSubscription.Api.DTOs.Requests
{
    public class CreateSubscriptionRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        public string? ZipCode { get; set; }
    }
}
