using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IUserReader
    {
        Task<User?> GetByUserIdpAsync(string subject, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct);
    }
}