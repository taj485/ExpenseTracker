using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.ValueObjects
{
    public record ExtractedReceiptItem(decimal UnitPrice, ExpenseCategory Category, string Description, DateOnly Date, decimal Quantity, string? Merchant = null, QuantityUnit Unit = QuantityUnit.Item);
}
