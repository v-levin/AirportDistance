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
        private readonly ILogger<DistanceController> _logger;

        public DistanceController(IDistanceService distanceService, ILogger<DistanceController> logger)
        {
            _distanceService = distanceService;
            _logger = logger;
        }

        /// <summary>
        /// Get the distance between two airports.
        /// </summary>
        /// <param name="request">Distance request.</param>
        /// <returns>Returns the distance between two airports.</returns>
        [HttpGet]
        public ActionResult<double> GetDistance([FromQuery] DistanceRequest request)
        {
            _logger.LogInformation($"Getting the distance between airports with IATA codes {request.OriginAirportCode} and {request.DestinationAirportCode}");
            var response = _distanceService.GetDistance(request);

            if (!response.Result)
                return BadRequest(response.Errors);

            return Ok(response.Distance);
        }
    }
}
