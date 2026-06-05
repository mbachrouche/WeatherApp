using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeatherSubscription.Api.Domain.Interfaces;
using WeatherSubscription.Api.DTOs.Requests;
using WeatherSubscription.Api.DTOs.Responses;

namespace WeatherSubscription.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSubscriptionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _subscriptionService.CreateSubscriptionAsync(
                request.Email,
                request.City,
                request.Country,
                request.ZipCode);

            return CreatedAtAction(nameof(GetWeather), new { email = response.Email }, response);
        }

        [HttpGet("{email}/weather")]
        public async Task<IActionResult> GetWeather(string email)
        {
            var response = await _subscriptionService.GetWeatherForEmailAsync(email);
            return Ok(response);
        }
    }
}
