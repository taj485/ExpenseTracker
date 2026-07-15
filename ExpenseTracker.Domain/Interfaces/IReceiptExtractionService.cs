using ExpenseTracker.Domain.ValueObjects;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IReceiptExtractionService
    {
        Task<IReadOnlyList<ExtractedReceiptItem>> ExtractAsync(byte[] imageBytes, string contentType, CancellationToken cancellationToken);
    }
}
