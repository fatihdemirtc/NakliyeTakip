using FluentValidation;

namespace NakliyeTakip.Location.API.Features.Location.InsertCurrentLocation;

public class InsertCurrentLocationCommandValidator : AbstractValidator<InsertCurrentLocationCommand>
{
    public InsertCurrentLocationCommandValidator()
    {
        RuleFor(x => x.Longitude)
            .NotEmpty().WithMessage("Longitude (L) cannot be empty.")
            .InclusiveBetween(-180, 180).WithMessage("Longitude (L) must be between -180 and 180.");
        RuleFor(x => x.Latitude)
            .NotEmpty().WithMessage("Latitude (A) cannot be empty.")
            .InclusiveBetween(-90, 90).WithMessage("Latitude (A) must be between -90 and 90.");
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId cannot be empty.");
    }
}
