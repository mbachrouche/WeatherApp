using Microsoft.AspNetCore.Mvc;
using WeatherSubscription.Api.DTOs.Requests;
using WeatherSubscription.Api.DTOs.Responses;

namespace WeatherSubscription.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        public SubscriptionsController()
        {
        }

        [HttpPost]
        public IActionResult Create(CreateSubscriptionRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
