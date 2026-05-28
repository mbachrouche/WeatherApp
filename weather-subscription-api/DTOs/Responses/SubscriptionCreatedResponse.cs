namespace WeatherSubscription.Api.DTOs.Responses
{
    public class SubscriptionCreatedResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
}
