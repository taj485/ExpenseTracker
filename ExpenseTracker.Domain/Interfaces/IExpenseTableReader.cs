using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.ValueObjects;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IExpenseTableReader
    {
        Task<ExpenseTable?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ExpenseTable>> GetAllForUserAsync(int userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ExpenseTableMember>> GetMembersAsync(int expenseTableId, CancellationToken cancellationToken = default);
        Task<bool> IsMemberAsync(int expenseTableId, int userId, CancellationToken cancellationToken = default);
        Task<bool> IsAdminAsync(int expenseTableId, int userId, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
