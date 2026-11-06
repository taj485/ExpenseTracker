using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.ValueObjects
{
    public record ExtractedReceiptItem(decimal UnitPrice,ExpenseCategory Category, string Description, DateOnly Date, int Quantity, string? Merchant = null, decimal Discount = 0);

    /// <summary>What a receipt scan found: the line items and any discount on the whole receipt.</summary>
    public record ExtractedReceipt(IReadOnlyList<ExtractedReceiptItem> Items, decimal ReceiptDiscount = 0)
    {
        public static readonly ExtractedReceipt Empty = new(Array.Empty<ExtractedReceiptItem>());
    }
}
