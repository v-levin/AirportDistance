namespace AirportDistance.Contracts
{
    public class Airport
    {
        public string? Iata { get; set; }
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public Location? Location { get; set; }
    }
}
