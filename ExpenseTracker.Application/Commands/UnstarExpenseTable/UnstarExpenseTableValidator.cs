using FluentValidation;

namespace ExpenseTracker.Application.Commands.UnstarExpenseTable
{
    public class UnstarExpenseTableValidator : AbstractValidator<UnstarExpenseTableCommand>
    {
        public UnstarExpenseTableValidator()
        {
            RuleFor(x => x.ExpenseTableId)
                .GreaterThan(0).WithMessage("ExpenseTableId is required.");
        }
    }
}
