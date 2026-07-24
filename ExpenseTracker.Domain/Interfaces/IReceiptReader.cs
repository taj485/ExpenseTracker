using ExpenseTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IReceiptReader
    {
        Task<Receipt?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
