using FluentValidation;

namespace ExpenseTracker.Application.Commands.DeleteExpenseTable
{
    public class DeleteExpenseTableValidator : AbstractValidator<DeleteExpenseTableCommand>
    {
        public DeleteExpenseTableValidator()
        {
            RuleFor(x => x.ExpenseTableId)
                .GreaterThan(0).WithMessage("ExpenseTableId is required.");
        }
    }
}
