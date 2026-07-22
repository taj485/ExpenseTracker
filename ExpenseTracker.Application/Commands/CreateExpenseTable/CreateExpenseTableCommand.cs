using MediatR;

namespace ExpenseTracker.Application.Commands.CreateExpenseTable
{
    public record CreateExpenseTableCommand(string Name) : IRequest<int>;
}
