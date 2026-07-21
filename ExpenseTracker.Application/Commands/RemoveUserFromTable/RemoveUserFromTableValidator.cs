using FluentValidation;

namespace ExpenseTracker.Application.Commands.RemoveUserFromTable
{
    public class RemoveUserFromTableValidator : AbstractValidator<RemoveUserFromTableCommand>
    {
        public RemoveUserFromTableValidator()
        {
            RuleFor(x => x.ExpenseTableId)
                .GreaterThan(0).WithMessage("ExpenseTableId is required.");
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId is required.");
        }
    }
}
