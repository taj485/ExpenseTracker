using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories
{
    public class ReceiptExtractionJobRepository : IReceiptExtractionJobWriter, IReceiptExtractionJobReader
    {
        private readonly ExpenseTrackerDbContext _context;

        public ReceiptExtractionJobRepository(ExpenseTrackerDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReceiptExtractionJob job, CancellationToken cancellationToken = default)
        {
            await _context.ReceiptExtractionJobs.AddAsync(job, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ReceiptExtractionJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.ReceiptExtractionJobs
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
        }
    }
}
