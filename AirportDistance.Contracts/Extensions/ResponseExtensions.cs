using AirportDistance.Contracts.Responses;
using FluentValidation.Results;

namespace AirportDistance.Contracts.Extensions
{
    public static class ResponseExtensions
    {
        public static Response ToResponse(this ValidationResult result)
        {
            return new Response
            {
                Result = result.IsValid,
                Errors = result.Errors
                               .Select(e => new ResponseError { Name = e.PropertyName, Error = e.ErrorMessage })
                               .ToList()
            };
        }
    }
}
