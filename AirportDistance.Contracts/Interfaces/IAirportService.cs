namespace AirportDistance.Contracts.Interfaces;

public interface IAirportService
{
    Task<Airport> GetAirportDetails(string airportCode);
}