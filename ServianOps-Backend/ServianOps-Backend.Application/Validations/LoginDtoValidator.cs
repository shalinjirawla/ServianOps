using FluentValidation;
using ServianOps_Backend.Application.AuthModule.Auth.AuthDto;

namespace ServianOps_Backend.Application.Validations
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            // TenancyName is optional because Host (SuperAdmin) has no TenancyName.
            // RuleFor(x => x.TenancyName)
            //     .NotEmpty().WithMessage("Company Code is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid Email is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
