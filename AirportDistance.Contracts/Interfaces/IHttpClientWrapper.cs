namespace AirportDistance.Contracts.Interfaces
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> GetAsync(string url);
    }
}
