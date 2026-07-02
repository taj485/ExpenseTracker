using FluentValidation;

namespace ExpenseTracker.Application.Commands.DeleteExpense
{
    public class DeleteExpenseValidator : AbstractValidator<DeleteExpenseCommand>
    {
        public DeleteExpenseValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero.");
        }
    }
}
