using ExpenseTracker.Domain.Enums;
using MediatR;

namespace ExpenseTracker.Application.Commands.UpdateExpense
{
    public record UpdateExpenseCommand(int Id, decimal UnitPrice, ExpenseCategory Category, string Description, string? Merchant = null, int Quantity = 1) : IRequest<Unit>;
}
