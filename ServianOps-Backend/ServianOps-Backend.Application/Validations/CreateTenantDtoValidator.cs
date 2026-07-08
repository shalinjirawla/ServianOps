using FluentValidation;
using ServianOps_Backend.Application.DTOs.Tenant;

namespace ServianOps_Backend.Application.Validations
{
    public class CreateTenantDtoValidator : AbstractValidator<CreateTenantDto>
    {
        public CreateTenantDtoValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company Name is required.")
                .MaximumLength(100).WithMessage("Company Name cannot exceed 100 characters.");

            RuleFor(x => x.TenancyName)
                .NotEmpty().WithMessage("Company Code is required.")
                .Matches("^[a-zA-Z0-9]+$").WithMessage("Company Code must be alphanumeric without spaces.")
                .MaximumLength(50).WithMessage("Company Code cannot exceed 50 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required.")
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required.")
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid Email is required.")
                .MaximumLength(150);

            // Plan ID must be provided
            RuleFor(x => x.PlanId)
                .GreaterThan(0).WithMessage("A valid Plan ID is required.");
        }
    }
}
