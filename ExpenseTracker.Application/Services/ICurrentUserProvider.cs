using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Services
{
    public interface ICurrentUserProvider
    {
        Task<User> GetOrProvisionAsync(CancellationToken ct);
    }
}