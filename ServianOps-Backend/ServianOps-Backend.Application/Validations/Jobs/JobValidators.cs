using System.IO;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using ServianOps_Backend.Application.DTOs.Jobs;

namespace ServianOps_Backend.Application.Validations.Jobs
{
    public class CreateJobDtoValidator : AbstractValidator<CreateJobDto>
    {
        public CreateJobDtoValidator()
        {
            RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("Client is required.");
            RuleFor(x => x.SiteId).GreaterThan(0).WithMessage("Site is required.");
            RuleFor(x => x.TradeId).GreaterThan(0).WithMessage("Trade is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
            RuleFor(x => x.Priority).IsInEnum().WithMessage("Invalid Priority.");
            RuleFor(x => x.Budget).GreaterThanOrEqualTo(0).WithMessage("Budget must be greater than or equal to 0.");
            RuleFor(x => x.NTE).GreaterThanOrEqualTo(0).WithMessage("NTE must be greater than or equal to 0.");

            RuleForEach(x => x.Attachments).SetValidator(new FormFileValidator());
        }
    }

    public class UpdateJobDtoValidator : AbstractValidator<UpdateJobDto>
    {
        public UpdateJobDtoValidator()
        {
            RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("Client is required.");
            RuleFor(x => x.SiteId).GreaterThan(0).WithMessage("Site is required.");
            RuleFor(x => x.TradeId).GreaterThan(0).WithMessage("Trade is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
            RuleFor(x => x.Priority).IsInEnum().WithMessage("Invalid Priority.");
            RuleFor(x => x.Budget).GreaterThanOrEqualTo(0).WithMessage("Budget must be greater than or equal to 0.");
            RuleFor(x => x.NTE).GreaterThanOrEqualTo(0).WithMessage("NTE must be greater than or equal to 0.");

            RuleForEach(x => x.Attachments).SetValidator(new FormFileValidator());
        }
    }

    public class FormFileValidator : AbstractValidator<IFormFile>
    {
        public FormFileValidator()
        {
            RuleFor(x => x.Length)
                .LessThanOrEqualTo(10 * 1024 * 1024) // 10MB limit
                .WithMessage("File size must not exceed 10 MB.");

            RuleFor(x => x.FileName)
                .Must(HaveValidExtension)
                .WithMessage("Only .jpg, .jpeg, .png, and .pdf files are allowed.");
        }

        private bool HaveValidExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return allowedExtensions.Contains(extension);
        }
    }
}
