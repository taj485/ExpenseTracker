using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories
{
    public class ExpenseTableRepository : IExpenseTableReader, IExpenseTableWriter
    {
        private readonly ExpenseTrackerDbContext _context;

        public ExpenseTableRepository(ExpenseTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<ExpenseTable?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ExpenseTables
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<ExpenseTable>> GetAllForUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.ExpenseTables
                .Include(t => t.Members)
                .Where(t => t.Members.Any(m => m.UserId == userId))
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsMemberAsync(int expenseTableId, int userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserExpenseTables
                .AnyAsync(m => m.ExpenseTableId == expenseTableId && m.UserId == userId, cancellationToken);
        }

        public async Task<bool> IsAdminAsync(int expenseTableId, int userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserExpenseTables
                .AnyAsync(m => m.ExpenseTableId == expenseTableId && m.UserId == userId && m.IsAdmin, cancellationToken);
        }

        public async Task<int> AddAsync(ExpenseTable expenseTable, CancellationToken cancellationToken)
        {
            await _context.ExpenseTables.AddAsync(expenseTable, cancellationToken);
            await SaveChangesAsync(cancellationToken);
            return expenseTable.Id;
        }

        public async Task UpdateAsync(ExpenseTable expenseTable)
        {
            _context.ExpenseTables.Update(expenseTable);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var expenseTable = await _context.ExpenseTables.FirstOrDefaultAsync(t => t.Id == id);

            if (expenseTable is null)
                throw new NotFoundException($"Expense table with id {id} was not found.");

            expenseTable.Delete();

            await _context.SaveChangesAsync();
        }

        public async Task StarTableAsync(int userId, int expenseTableId, CancellationToken cancellationToken)
        {
            await _context.UserExpenseTables
                .Where(m => m.UserId == userId && m.IsStarred && m.ExpenseTableId != expenseTableId)
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsStarred, false), cancellationToken);

            await _context.UserExpenseTables
                .Where(m => m.UserId == userId && m.ExpenseTableId == expenseTableId)
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsStarred, true), cancellationToken);
        }

        public async Task UnstarTableAsync(int userId, int expenseTableId, CancellationToken cancellationToken)
        {
            await _context.UserExpenseTables
                .Where(m => m.UserId == userId && m.ExpenseTableId == expenseTableId)
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsStarred, false), cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
