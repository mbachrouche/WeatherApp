namespace WeatherSubscription.Api.DTOs.Requests
{
    public class CreateSubscriptionRequest
    {
        public string Email { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
}
