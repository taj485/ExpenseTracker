using ExpenseTracker.Domain.Entities;
using FluentValidation;

namespace ExpenseTracker.Application.Commands.UpdateExpense
{
    public class UpdateExpenseValidator : AbstractValidator<UpdateExpenseCommand>
    {
        public UpdateExpenseValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero.");
            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Unit price must be greater than zero.");
            RuleFor(x => x.Unit)
                .IsInEnum().WithMessage("Invalid quantity unit");
            RuleFor(x => x)
                .Must(x => Expense.QuantityError(x.Quantity, x.Unit) is null)
                .WithName(nameof(UpdateExpenseCommand.Quantity))
                .WithMessage(x => Expense.QuantityError(x.Quantity, x.Unit) + ".");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");
            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Invalid expense category");
            RuleFor(x => x.Merchant)
                .MaximumLength(200).WithMessage("Merchant name is too long.");
        }
    }
}
