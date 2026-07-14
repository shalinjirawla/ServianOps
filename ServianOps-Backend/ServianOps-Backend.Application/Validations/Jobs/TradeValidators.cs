using ServianOps_Backend.Application.TradeModule.Trade.TradeDto;
using FluentValidation;

namespace ServianOps_Backend.Application.Validations.Jobs
{
    public class CreateTradeDtoValidator : AbstractValidator<CreateTradeDto>
    {
        public CreateTradeDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Trade Name is required.")
                .MaximumLength(150).WithMessage("Trade Name must not exceed 150 characters.");
        }
    }

    public class UpdateTradeDtoValidator : AbstractValidator<UpdateTradeDto>
    {
        public UpdateTradeDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Trade Name is required.")
                .MaximumLength(150).WithMessage("Trade Name must not exceed 150 characters.");
        }
    }
}
