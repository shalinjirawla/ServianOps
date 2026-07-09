using FluentValidation;
using ServianOps_Backend.Application.DTOs.Crm;

namespace ServianOps_Backend.Application.Validations.Crm
{
    public class CreateSiteDtoValidator : AbstractValidator<CreateSiteDto>
    {
        public CreateSiteDtoValidator()
        {
            RuleFor(x => x.SiteName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.CustomerId).GreaterThan(0);
            RuleFor(x => x.ContactFirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ContactMobile).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ContactEmail).EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail));
        }
    }

    public class UpdateSiteDtoValidator : AbstractValidator<UpdateSiteDto>
    {
        public UpdateSiteDtoValidator()
        {
            RuleFor(x => x.SiteName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.CustomerId).GreaterThan(0);
            RuleFor(x => x.ContactFirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ContactMobile).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ContactEmail).EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail));
        }
    }
}
