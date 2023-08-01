using AirportDistance.Contracts.Extensions;
using AirportDistance.Contracts.Interfaces;
using AirportDistance.Contracts.Requests;
using AirportDistance.Contracts.Responses;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AirportDistance.Domain.Services
{
    public class DistanceService : IDistanceService
    {
        private readonly ILogger<DistanceService> _logger;
        private readonly IValidator<DistanceRequest> _validator;
        private readonly IAirportService _airportService;
        private readonly ICalculatorService _calculatorService;

        public DistanceService(
            ILogger<DistanceService> logger,
            IValidator<DistanceRequest> validator,
            IAirportService airportService,
            ICalculatorService calculatorService)
        {
            _logger = logger;
            _validator = validator;
            _airportService = airportService;
            _calculatorService = calculatorService;
        }

        /// <summary>
        /// Gets the distance between two airports.
        /// </summary>
        /// <param name="request">Distance request.</param>
        /// <returns>Returns the distance in miles.</returns>
        public async Task<DistanceResponse> GetDistance(DistanceRequest request)
        {
            _logger.LogInformation("Getting distance between airports with IATA codes {OriginAirport} and {DestinationAirport} started..", 
                request.OriginAirportCode, request.DestinationAirportCode);
            
            var validator = _validator.Validate(request).ToResponse();

            if (!validator.Result)
                return new DistanceResponse { Result = false, Errors = validator.Errors };

            DistanceResponse response = new();
            try
            {
                var originAirport = await _airportService.GetAirportDetails(request.OriginAirportCode!);
                var destinationAirport = await _airportService.GetAirportDetails(request.DestinationAirportCode!);

                if (originAirport.Location is not null && destinationAirport.Location is not null)
                {
                    response = _calculatorService.CalculateDistance(originAirport.Location, destinationAirport.Location);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{Exception}", ex);
                response.Result = false;
                response.Errors.Add(new ResponseError
                {
                    Name = "GetDistanceException", 
                    Error = "An exception occured while getting the distance"
                });
            }

            return response;
        }
    }
}
