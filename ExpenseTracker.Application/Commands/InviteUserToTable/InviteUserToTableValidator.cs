using FluentValidation;

namespace ExpenseTracker.Application.Commands.InviteUserToTable
{
    public class InviteUserToTableValidator : AbstractValidator<InviteUserToTableCommand>
    {
        public InviteUserToTableValidator()
        {
            RuleFor(x => x.ExpenseTableId)
                .GreaterThan(0).WithMessage("ExpenseTableId is required.");
            RuleFor(x => x.InviteeEmail)
                .NotEmpty().WithMessage("Invitee email is required")
                .EmailAddress().WithMessage("Invitee email is not a valid email address.");
        }
    }
}
