using MediatR;

namespace ExpenseTracker.Application.Commands.ExtractReceiptExpenses
{
    public record ExtractReceiptExpensesCommand(int ExpenseTableId, byte[] ImageBytes, string ContentType)
        : IRequest<ExtractReceiptExpensesResult>;

    public record ExtractReceiptExpensesResult(Guid JobId, string TempReference);
}
