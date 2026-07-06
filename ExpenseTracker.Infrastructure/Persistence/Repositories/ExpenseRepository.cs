using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories
{
    public class ExpenseRepository : IExpenseWriter, IExpenseReader
    {
        private readonly ExpenseTrackerDbContext _context;

        public ExpenseRepository(ExpenseTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Expenses
                .Include(e => e.Users)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<int> AddAsync(Expense expense, CancellationToken cancellationToken)
        {
            await _context.Expenses.AddAsync(expense, cancellationToken);
            await SaveChangesAsync(cancellationToken);
            return expense.Id;
        }

        public async Task<IEnumerable<Expense>> GetAllForUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Expenses
                .Where(e => e.Users.Any(u => u.Id == userId))
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id)
        {
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);

            if (expense is null)
                throw new NotFoundException($"Expense with id {id} was not found.");

            expense.Delete();

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Expense expense)
        {
            _context.Expenses.Update(expense);

            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
