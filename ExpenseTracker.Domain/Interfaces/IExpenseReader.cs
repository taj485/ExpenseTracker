using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IExpenseReader
    {
        Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Expense>> GetAllAsync(CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
