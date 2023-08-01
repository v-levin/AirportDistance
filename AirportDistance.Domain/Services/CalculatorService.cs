using AirportDistance.Contracts;
using AirportDistance.Contracts.Interfaces;
using AirportDistance.Contracts.Responses;
using GeoCoordinatePortable;

namespace AirportDistance.Domain.Services;

public class CalculatorService : ICalculatorService
{
    public DistanceResponse CalculateDistance(Location originAirport, Location destinationAirport)
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