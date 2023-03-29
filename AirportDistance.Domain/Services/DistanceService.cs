using AirportDistance.Contracts;
using AirportDistance.Contracts.Extensions;
using AirportDistance.Contracts.Interfaces;
using AirportDistance.Contracts.Messages;
using AirportDistance.Contracts.Requests;
using AirportDistance.Contracts.Responses;
using AirportDistance.Domain.Validators;
using GeoCoordinatePortable;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AirportDistance.Domain.Services
{
    public class DistanceService : IDistanceService
    {
        private readonly IHttpClientWrapper _httpClient;
        private readonly ILogger<DistanceService> _logger;
        private readonly string _airportsUrl;

        public DistanceService(IHttpClientWrapper httpClient, ILogger<DistanceService> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _airportsUrl = "https://places-dev.cteleport.com/airports";
        }

        /// <summary>
        /// Gets the distance between two airports.
        /// </summary>
        /// <param name="request">Distance request.</param>
        /// <returns>Returnes the distance in miles.</returns>
        public DistanceResponse GetDistance(DistanceRequest request)
        {
            var validator = new AirportCodeValidator().Validate(request).ToResponse();

            if (!validator.Result)
                return new DistanceResponse { Result = false, Errors = validator.Errors };

            DistanceResponse response = new();
            try
            {
                var originAirport = GetAirportDetails(request.OriginAirportCode!);
                var destinationAirport = GetAirportDetails(request.DestinationAirportCode!);

                if (originAirport.Location is not null && destinationAirport.Location is not null)
                {
                    response = CalculateDistance(originAirport.Location, destinationAirport.Location);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ExceptionMessages.GetDistanceException, ex);
                response.Result = false;
                response.Errors.Add(new ResponseError { Name = "GetDistanceException", Error = ExceptionMessages.GetDistanceException });
                return response;
            }

            return response;
        }

        private Airport GetAirportDetails(string airportCode)
        {
            using var response = _httpClient.GetAsync($"{_airportsUrl}/{airportCode}").Result;
            response.EnsureSuccessStatusCode();
            var json = response.Content.ReadAsStringAsync().Result;
            var airportDetails = JsonConvert.DeserializeObject<Airport>(json);
            if (airportDetails is not null)
                return airportDetails;
            else
                return new Airport();
        }

        private static DistanceResponse CalculateDistance(Location originAirport, Location destinationAirport)
        {
            var originAirportCoords = new GeoCoordinate(originAirport.Lat, originAirport.Lon);
            var destinationAirportCoords = new GeoCoordinate(destinationAirport.Lat, destinationAirport.Lon);
            var distanceInMeters = originAirportCoords.GetDistanceTo(destinationAirportCoords);
            var distanceInMiles = ConvertMetersToMiles(distanceInMeters);

            return new DistanceResponse { Distance = distanceInMiles };
        }

        private static double ConvertMetersToMiles(double meters)
        {
            return Math.Round(meters / 1609.344, 2);
        }
    }
}
