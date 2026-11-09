using MediatR;

namespace ExpenseTracker.Application.Commands.DeleteAccount
{
    /// <summary>
    /// Deletes the caller's account and personal data. Safe to repeat: a retry finishes whatever
    /// a previous attempt left undone.
    /// </summary>
    public record DeleteAccountCommand : IRequest<Unit>;
}
