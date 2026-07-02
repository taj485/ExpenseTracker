using ExpenseTracker.Domain.Enums;
using MediatR;

namespace ExpenseTracker.Application.Commands.UpdateExpense
{
    public record UpdateExpenseCommand(int Id, decimal Amount, ExpenseCategory Category, string Description) : IRequest<Unit>;
}
