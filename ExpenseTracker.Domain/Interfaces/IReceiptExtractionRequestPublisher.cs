namespace ExpenseTracker.Domain.Interfaces
{
    public interface IReceiptExtractionRequestPublisher
    {
        Task PublishAsync(Guid jobId, Uri imageUrl, string contentType, CancellationToken cancellationToken = default);
    }
}
