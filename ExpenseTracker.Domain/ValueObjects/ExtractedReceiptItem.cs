using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.ValueObjects
{
    public record ExtractedReceiptItem(decimal UnitPrice, ExpenseCategory Category, string Description, DateOnly Date, decimal Quantity, string? Merchant = null, decimal Discount = 0, QuantityUnit Unit = QuantityUnit.Item);

    /// <summary>What a receipt scan found: the line items and any discount on the whole receipt.</summary>
    public record ExtractedReceipt(IReadOnlyList<ExtractedReceiptItem> Items, decimal ReceiptDiscount = 0)
    {
        public static readonly ExtractedReceipt Empty = new(Array.Empty<ExtractedReceiptItem>());
    }
}
