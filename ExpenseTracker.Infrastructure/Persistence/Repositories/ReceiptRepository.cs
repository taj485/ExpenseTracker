using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories
{
    public class ReceiptRepository : IReceiptWriter
    {
        private readonly ExpenseTrackerDbContext _context;

        public ReceiptRepository(ExpenseTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(Receipt receipt, CancellationToken cancellationToken)
        {
            await _context.Receipts.AddAsync(receipt, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return receipt.Id;
        }
    }
}
