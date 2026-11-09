using FluentValidation;

namespace ExpenseTracker.Application.Commands.UpdateReceiptDiscount
{
    public class UpdateReceiptDiscountValidator : AbstractValidator<UpdateReceiptDiscountCommand>
    {
        public UpdateReceiptDiscountValidator()
        {
            RuleFor(x => x.ReceiptDiscount)
                .GreaterThanOrEqualTo(0).WithMessage("Receipt discount can't be negative.");
        }
    }
}
