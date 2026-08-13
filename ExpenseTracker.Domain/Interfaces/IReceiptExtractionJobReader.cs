using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IReceiptExtractionJobReader
    {
        Task<ReceiptExtractionJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
