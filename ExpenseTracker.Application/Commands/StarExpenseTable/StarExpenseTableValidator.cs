using FluentValidation;

namespace ExpenseTracker.Application.Commands.StarExpenseTable
{
    public class StarExpenseTableValidator : AbstractValidator<StarExpenseTableCommand>
    {
        public StarExpenseTableValidator()
        {
            RuleFor(x => x.ExpenseTableId)
                .GreaterThan(0).WithMessage("ExpenseTableId is required.");
        }
    }
}
