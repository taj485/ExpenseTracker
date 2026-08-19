using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories
{
    public class MerchantRepository : IMerchantReader, IMerchantWriter
    {
        private readonly ExpenseTrackerDbContext _context;

        public MerchantRepository(ExpenseTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Merchant?> GetByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(normalizedName))
                return null;

            var merchant = await _context.Merchants
                .FirstOrDefaultAsync(m => m.NormalizedName == normalizedName, cancellationToken);

            if (merchant is not null)
                return merchant;

            // Fall back to the receipt spellings — "Tesco Stores Limited" resolves to Tesco.
            return await _context.MerchantAliases
                .Where(a => a.NormalizedAlias == normalizedName)
                .Select(a => a.Merchant)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Merchant?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Merchants
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Merchant>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Merchants
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> AddAsync(Merchant merchant, CancellationToken cancellationToken)
        {
            await _context.Merchants.AddAsync(merchant, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return merchant.Id;
        }
    }
}
