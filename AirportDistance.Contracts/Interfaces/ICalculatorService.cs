using AirportDistance.Contracts.Responses;

namespace AirportDistance.Contracts.Interfaces;

public interface ICalculatorService
{
    DistanceResponse CalculateDistance(Location originAirport, Location destinationAirport);
}