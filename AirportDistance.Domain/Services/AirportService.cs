using AirportDistance.Contracts;
using AirportDistance.Contracts.Interfaces;
using Newtonsoft.Json;

namespace AirportDistance.Domain.Services;

public class AirportService : IAirportService
{
    private readonly IHttpClientWrapper _httpClient;
    private readonly string _airportsUrl;
    
    public AirportService(IHttpClientWrapper httpClient)
    {
        _httpClient = httpClient;
        _airportsUrl = "https://places-dev.cteleport.com/airports";
    }
    
    public async Task<Airport> GetAirportDetails(string airportCode)
    {
        using var response = await _httpClient.GetAsync($"{_airportsUrl}/{airportCode}");
        response.EnsureSuccessStatusCode();
        var json = response.Content.ReadAsStringAsync().Result;
        var airportDetails = JsonConvert.DeserializeObject<Airport>(json);

        return airportDetails ?? new Airport();
    }
}