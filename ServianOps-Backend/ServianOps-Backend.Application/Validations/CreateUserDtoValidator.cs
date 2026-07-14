using FluentValidation;
using ServianOps_Backend.Application.UserModule.User.UserDto;

namespace ServianOps_Backend.Application.Validations
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
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

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        }
    }
}
