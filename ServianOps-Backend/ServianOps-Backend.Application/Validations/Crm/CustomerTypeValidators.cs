using FluentValidation;
using ServianOps_Backend.Application.CustomerTypeModule.CustomerType.CustomerTypeDto;

namespace ServianOps_Backend.Application.Validations.Crm
{
    public class CreateCustomerTypeDtoValidator : AbstractValidator<CreateCustomerTypeDto>
    {
        public CreateCustomerTypeDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        }
    }

    public class UpdateCustomerTypeDtoValidator : AbstractValidator<UpdateCustomerTypeDto>
    {
        public UpdateCustomerTypeDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        }
    }
}
