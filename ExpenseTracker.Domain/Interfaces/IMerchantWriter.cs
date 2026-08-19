using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IMerchantWriter
    {
        Task<int> AddAsync(Merchant merchant, CancellationToken cancellationToken);
    }
}
