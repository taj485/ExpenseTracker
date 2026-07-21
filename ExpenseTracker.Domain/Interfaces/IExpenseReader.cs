using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IExpenseReader
    {
        Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Expense>> GetAllForTableAsync(int expenseTableId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Expense>> GetByReceiptIdAsync(int receiptId, int expenseTableId, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
