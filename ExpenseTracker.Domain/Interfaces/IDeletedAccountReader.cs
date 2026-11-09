namespace ExpenseTracker.Domain.Interfaces
{
    public interface IDeletedAccountReader
    {
        Task<bool> IsDeletedAsync(string subject, CancellationToken cancellationToken = default);
    }
}
