using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.ValueObjects
{
    public record ExtractedReceiptItem(decimal Amount, ExpenseCategory Category, string Description, DateOnly Date, int Quantity);
}
