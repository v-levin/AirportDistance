using System.Net;
using AirportDistance.Contracts;
using AirportDistance.Contracts.Interfaces;
using AirportDistance.Contracts.Requests;
using AirportDistance.Contracts.Responses;
using AirportDistance.Domain.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace AirportDistance.Tests
{
    public class DistanceServiceTests
    {
        private readonly DistanceService _distanceService;
        private readonly string _airportsUrl;
        private readonly Mock<IHttpClientWrapper> _httpClientMock;
        private readonly Mock<ILogger<DistanceService>> _loggerMock;
        private readonly Mock<IValidator<DistanceRequest>> _validatorMock;
        private readonly Mock<IAirportService> _airportServiceMock;
        private readonly Mock<ICalculatorService> _calculatorServiceMock;

        public DistanceServiceTests()
        {
            _httpClientMock = new Mock<IHttpClientWrapper>();
            _loggerMock = new Mock<ILogger<DistanceService>>();
            _validatorMock = new Mock<IValidator<DistanceRequest>>();
            _airportServiceMock = new Mock<IAirportService>();
            _calculatorServiceMock = new Mock<ICalculatorService>();
            _distanceService = new DistanceService(
                _loggerMock.Object,
                _validatorMock.Object,
                _airportServiceMock.Object,
                _calculatorServiceMock.Object);
            _airportsUrl = "https://places-dev.cteleport.com/airports";
        }

        [Fact]
        public async Task GetDistance_ValidRequest_ReturnsDistanceResponse()
        {
            // Arrange
            const string originAirportCode = "AMS";
            const string destinationAirportCode = "PRG";
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

            _validatorMock.Setup(v => v.Validate(distanceRequest)).Returns(new ValidationResult());
            _airportServiceMock.Setup(a => a.GetAirportDetails(It.IsAny<string>()))
                .ReturnsAsync(new Airport 
                { 
                    Location = new Location
                    {
                        Lon = 1.443,
                        Lat = 2.321
                    }
                });
            _calculatorServiceMock
                .Setup(s => s.CalculateDistance(It.IsAny<Location>(), It.IsAny<Location>()))
                .Returns(new DistanceResponse { Distance = 100, Errors = new List<ResponseError>(), Result = true });

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
            const string originAirportCode = "INVALID";
            const string destinationAirportCode = "INVALID";
            var distanceRequest = new DistanceRequest 
            { 
                OriginAirportCode = originAirportCode, 
                DestinationAirportCode = destinationAirportCode 
            };
            
            var validationResult = new ValidationResult 
            { 
                Errors = new List<ValidationFailure> 
                {
                    new()
                    {
                        ErrorMessage = "Invalid request" 
                    }
                }
            };
            
            _validatorMock.Setup(v => v.Validate(distanceRequest)).Returns(validationResult);

            // Act
            var response = await _distanceService.GetDistance(distanceRequest);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Result);
            Assert.NotEmpty(response.Errors);
        }
    }
}




