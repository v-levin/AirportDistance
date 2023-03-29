using AirportDistance.Contracts.Requests;
using AirportDistance.Contracts.Responses;

namespace AirportDistance.Contracts.Interfaces
{
    public interface IDistanceService
    {
        DistanceResponse GetDistance(DistanceRequest request);
    }
}
