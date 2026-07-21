using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IExpenseTableWriter
    {
        Task<int> AddAsync(ExpenseTable expenseTable, CancellationToken cancellationToken);
        Task UpdateAsync(ExpenseTable expenseTable);
        Task DeleteAsync(int id);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
