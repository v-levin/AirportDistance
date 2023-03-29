using AirportDistance.Contracts.Interfaces;
using AirportDistance.Contracts.Requests;
using AirportDistance.Contracts.Responses;
using AirportDistance.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AirportDistance.Tests
{
    public class DistanceControllerTests
    {
        private readonly Mock<IDistanceService> _distanceServiceMock;
        private readonly Mock<ILogger<DistanceController>> _loggerMock;
        private readonly DistanceController _controller;

        public DistanceControllerTests()
        {
            _distanceServiceMock = new Mock<IDistanceService>();
            _loggerMock = new Mock<ILogger<DistanceController>>();

            _controller = new DistanceController(_distanceServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetDistance_ReturnsOkWithDistance()
        {
            // Arrange
            var request = new DistanceRequest { OriginAirportCode = "AMS", DestinationAirportCode = "PRG" };
            var expectedDistance = 438.6;
            var response = new DistanceResponse { Result = true, Distance = expectedDistance };
            _distanceServiceMock.Setup(x => x.GetDistance(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetDistance(request);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            if (okResult?.Value is not null)
                Assert.Equal(expectedDistance, (double)okResult.Value);
        }

        [Fact]
        public async Task GetDistance_ReturnsBadRequestWithErrors()
        {
            // Arrange
            var request = new DistanceRequest { OriginAirportCode = "QQW", DestinationAirportCode = "PRG" };
            var errors = new List<ResponseError> { new ResponseError { Error = "Invalid airport code" } };
            var response = new DistanceResponse { Result = false, Errors = errors };
            _distanceServiceMock.Setup(x => x.GetDistance(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetDistance(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            if (badRequestResult?.Value is not null)
                Assert.Equal(errors, badRequestResult.Value);
        }
    }
}