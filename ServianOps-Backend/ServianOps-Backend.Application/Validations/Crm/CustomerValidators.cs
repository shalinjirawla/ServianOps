using ServianOps_Backend.Application.CustomerModule.Customer.CustomerDto;
using FluentValidation;
using ServianOps_Backend.Application.CustomerModule.Customer.CustomerDto;

namespace ServianOps_Backend.Application.Validations.Crm
{
    public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
    {
        public CreateCustomerDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.CustomerTypeId).GreaterThan(0);
            RuleFor(x => x.ContactFirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ContactMobile).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ContactEmail).EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail));
        }
    }

    public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
    {
        public UpdateCustomerDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.CustomerTypeId).GreaterThan(0);
            RuleFor(x => x.ContactFirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ContactMobile).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ContactEmail).EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail));
        }
    }
}
