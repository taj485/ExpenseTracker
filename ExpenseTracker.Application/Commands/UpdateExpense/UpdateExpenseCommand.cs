using ExpenseTracker.Domain.Enums;
using MediatR;

namespace ExpenseTracker.Application.Commands.UpdateExpense
{
    public record UpdateExpenseCommand(int Id, decimal UnitPrice, ExpenseCategory Category, string Description, string? Merchant = null, decimal Quantity = 1, decimal? Discount = null, QuantityUnit Unit = QuantityUnit.Item) : IRequest<Unit>;
}
