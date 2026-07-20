using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IReceiptWriter
    {
        Task<int> AddAsync(Receipt receipt, CancellationToken cancellationToken);
    }
}
