using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IReceiptExtractionJobWriter
    {
        Task AddAsync(ReceiptExtractionJob job, CancellationToken cancellationToken = default);
    }
}
