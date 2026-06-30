using ExpenseTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IExpenseWriter
    {
        Task<int> AddAsync(Expense expense, CancellationToken cancellationToken);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(int id);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
