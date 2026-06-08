using System;
using System.Threading.Tasks;
using WeatherSubscription.Api.Domain.Entities;
using WeatherSubscription.Api.Domain.Interfaces;
using WeatherSubscription.Api.DTOs.Responses;
using WeatherSubscription.Api.Exceptions;

namespace WeatherSubscription.Api.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _repository;
        private readonly IWeatherService _weatherService;

        public SubscriptionService(ISubscriptionRepository repository, IWeatherService weatherService)
        {
            _repository = repository;
            _weatherService = weatherService;
        }

        public async Task<SubscriptionCreatedResponse> CreateSubscriptionAsync(
            string email, string city, string country, string? zipCode = null)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City is required.", nameof(city));
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country is required.", nameof(country));

            // Check for duplicate
            if (await _repository.ExistsAsync(email))
                throw new DuplicateEmailException($"Email '{email}' already exists.");

            // Create and save entity
            var subscription = new Subscription(email, city, country, zipCode);
            var savedSubscription = await _repository.CreateAsync(subscription);

            // Map to DTO
            return new SubscriptionCreatedResponse
            {
                Id = savedSubscription.Id,
                Email = savedSubscription.Email,
                City = savedSubscription.City,
                Country = savedSubscription.Country,
                ZipCode = savedSubscription.ZipCode,
                CreatedAt = savedSubscription.CreatedAt
            };
        }

        public async Task<WeatherResponse> GetWeatherForEmailAsync(string email)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            // Look up subscription
            var subscription = await _repository.GetByEmailAsync(email);
            if (subscription == null)
                throw new NotFoundException($"Subscription with email '{email}' not found.");

            // Fetch weather
            var weather = await _weatherService.GetWeatherAsync(subscription.City, subscription.Country);
            return weather;
        }
    }
}
