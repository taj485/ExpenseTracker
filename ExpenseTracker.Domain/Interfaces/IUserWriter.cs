using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IUserWriter
    {
        Task<int> AddAsync(User user, CancellationToken ct);
        Task UpdateAsync(User user);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
