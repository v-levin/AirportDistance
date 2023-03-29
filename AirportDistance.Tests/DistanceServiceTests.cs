using System.Net;
using AirportDistance.Contracts;
using AirportDistance.Contracts.Interfaces;
using AirportDistance.Contracts.Requests;
using AirportDistance.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

namespace AirportDistance.Tests
{
    public class DistanceServiceTests
    {
        private readonly DistanceService _distanceService;
        private readonly string _airportsUrl;
        private readonly Mock<IHttpClientWrapper> _httpClientMock;
        private readonly Mock<ILogger<DistanceService>> _loggerMock;

        public DistanceServiceTests()
        {
            _httpClientMock = new Mock<IHttpClientWrapper>();
            _loggerMock = new Mock<ILogger<DistanceService>>();
            _distanceService = new DistanceService(_httpClientMock.Object, _loggerMock.Object);
            _airportsUrl = "https://places-dev.cteleport.com/airports";
        }

        [Fact]
        public async Task GetDistance_ValidRequest_ReturnsDistanceResponse()
        {
            // Arrange
            var originAirportCode = "AMS";
            var destinationAirportCode = "PRG";
            var distanceRequest = new DistanceRequest 
            { 
                OriginAirportCode = originAirportCode, 
                DestinationAirportCode = destinationAirportCode 
            };

            var originAirport = new Airport
            {
                Location = new Location { Lon = 4.763385, Lat = 52.309069 }
            };

            var destinationAirport = new Airport
            {
                Location = new Location { Lon = 14.266638, Lat = 50.106188 }
            };

            _httpClientMock
                .Setup(c => c.GetAsync($"{_airportsUrl}/{originAirportCode}"))
                .ReturnsAsync(new HttpResponseMessage 
                { 
                    StatusCode = HttpStatusCode.OK, 
                    Content = new StringContent(JsonConvert.SerializeObject(originAirport)) 
                });

            _httpClientMock
                .Setup(c => c.GetAsync($"{_airportsUrl}/{destinationAirportCode}"))
                .ReturnsAsync(new HttpResponseMessage 
                { 
                    StatusCode = HttpStatusCode.OK, 
                    Content = new StringContent(JsonConvert.SerializeObject(destinationAirport)) 
                });

            // Act
            var response = await _distanceService.GetDistance(distanceRequest);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Result);
            Assert.Empty(response.Errors);
            Assert.True(response.Distance > 0);
        }

        [Fact]
        public async Task GetDistance_InvalidAirportCodes_ReturnsErrorResponse()
        {
            // Arrange
            var originAirportCode = "INVALID";
            var destinationAirportCode = "INVALID";
            var distanceRequest = new DistanceRequest 
            { 
                OriginAirportCode = originAirportCode, 
                DestinationAirportCode = destinationAirportCode 
            };

            // Act
            var response = await _distanceService.GetDistance(distanceRequest);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Result);
            Assert.NotEmpty(response.Errors);
        }
    }
}




