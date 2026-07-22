using MediatR;

namespace ExpenseTracker.Application.Commands.InviteUserToTable
{
    public record InviteUserToTableCommand(int ExpenseTableId, string InviteeEmail, bool IsAdmin = false) : IRequest<Unit>;
}
