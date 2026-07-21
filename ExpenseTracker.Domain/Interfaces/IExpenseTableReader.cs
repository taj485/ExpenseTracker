using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IExpenseTableReader
    {
        Task<ExpenseTable?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ExpenseTable>> GetAllForUserAsync(int userId, CancellationToken cancellationToken = default);
        Task<bool> IsMemberAsync(int expenseTableId, int userId, CancellationToken cancellationToken = default);
        Task<bool> IsAdminAsync(int expenseTableId, int userId, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
