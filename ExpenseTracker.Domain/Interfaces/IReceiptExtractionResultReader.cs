using ExpenseTracker.Domain.ValueObjects;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IReceiptExtractionResultReader
    {
        /// <summary>Returns null when the analyser does not know this job id.</summary>
        Task<ReceiptExtractionResult?> GetAsync(Guid jobId, CancellationToken cancellationToken = default);
    }
}
