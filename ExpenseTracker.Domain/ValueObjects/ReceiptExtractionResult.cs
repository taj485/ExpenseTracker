using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.ValueObjects
{
    public record ReceiptExtractionResult(
        Guid JobId,
        ReceiptExtractionStatus Status,
        IReadOnlyList<ExtractedReceiptItem> Items,
        string? Error);
}
