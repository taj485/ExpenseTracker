using MediatR;

namespace ExpenseTracker.Application.Commands.RemoveUserFromTable
{
    public record RemoveUserFromTableCommand(int ExpenseTableId, int UserId) : IRequest<Unit>;
}
