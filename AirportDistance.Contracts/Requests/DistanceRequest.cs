using System.ComponentModel.DataAnnotations;

namespace AirportDistance.Contracts.Requests
{
    public class DistanceRequest
    {
        [Required]
        public string? OriginAirportCode { get; set; }
        [Required]
        public string? DestinationAirportCode { get; set; }
    }
}
