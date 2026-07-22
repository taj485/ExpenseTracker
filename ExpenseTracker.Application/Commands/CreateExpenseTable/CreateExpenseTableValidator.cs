using FluentValidation;

namespace ExpenseTracker.Application.Commands.CreateExpenseTable
{
    public class CreateExpenseTableValidator : AbstractValidator<CreateExpenseTableCommand>
    {
        public CreateExpenseTableValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name is too long.");
        }
    }
}
