using ExpenseTracker.Application.DTO;
using MediatR;

namespace ExpenseTracker.Application.Commands.ExtractReceiptExpenses
{
    public record ExtractReceiptExpensesCommand(byte[] ImageBytes, string ContentType) : IRequest<List<ExtractedExpenseDto>>;
}
