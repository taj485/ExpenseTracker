using MediatR;

namespace ExpenseTracker.Application.Commands.DeleteExpense
{
    public record DeleteExpenseCommand(int Id) : IRequest<Unit>;
}
