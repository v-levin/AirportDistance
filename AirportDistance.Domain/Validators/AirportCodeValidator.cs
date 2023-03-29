using AirportDistance.Contracts.Requests;
using FluentValidation;

namespace AirportDistance.Domain.Validators
{
    public class AirportCodeValidator : AbstractValidator<DistanceRequest>
    {
        public AirportCodeValidator() 
        {
            RuleFor(x => x.OriginAirportCode)
                .NotEmpty().WithMessage("IATA code is required")
                .Length(3).WithMessage("IATA code must be 3 characters long")
                .Matches("^[A-Z]{3}$").WithMessage("IATA code must consist of 3 uppercase letters");

            RuleFor(x => x.DestinationAirportCode)
                .NotEmpty().WithMessage("IATA code is required")
                .Length(3).WithMessage("IATA code must be 3 characters long")
                .Matches("^[A-Z]{3}$").WithMessage("IATA code must consist of 3 uppercase letters");
        }
    }
}
