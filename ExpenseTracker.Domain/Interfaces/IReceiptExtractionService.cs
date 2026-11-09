using ExpenseTracker.Domain.ValueObjects;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IReceiptExtractionService
    {
        Task<ExtractedReceipt> ExtractAsync(byte[] imageBytes, string contentType, CancellationToken cancellationToken);
    }
}
