using AirportDistance.Contracts.Interfaces;
using AirportDistance.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AirportDistance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DistanceController : ControllerBase
    {
        private readonly IDistanceService _distanceService;

        public DistanceController(IDistanceService distanceService, ILogger<DistanceController> logger)
        {
            _distanceService = distanceService;
        }

        /// <summary>
        /// Get the distance between two airports.
        /// </summary>
        /// <param name="request">Distance request.</param>
        /// <returns>Returns the distance between two airports.</returns>
        [HttpGet]
        public async Task<ActionResult<double>> GetDistance([FromQuery] DistanceRequest request)
        {
            var response = await _distanceService.GetDistance(request);

            if (!response.Result)
                return BadRequest(response.Errors);

            return Ok(response.Distance);
        }
    }
}
