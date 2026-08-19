using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;

namespace ExpenseTracker.Application.Services
{
    public class MerchantResolver : IMerchantResolver
    {
        private readonly IMerchantReader _merchantReader;
        private readonly IMerchantWriter _merchantWriter;

        public MerchantResolver(IMerchantReader merchantReader, IMerchantWriter merchantWriter)
        {
            _merchantReader = merchantReader;
            _merchantWriter = merchantWriter;
        }

        public async Task<Merchant?> FindAsync(string? rawName, CancellationToken cancellationToken)
        {
            var normalized = Merchant.Normalize(rawName);

            if (normalized.Length == 0)
                return null;

            return await _merchantReader.GetByNormalizedNameAsync(normalized, cancellationToken);
        }

        public async Task<int?> ResolveOrCreateAsync(string? rawName, CancellationToken cancellationToken)
        {
            var trimmed = rawName?.Trim();

            if (Merchant.Normalize(trimmed).Length == 0)
                return null;

            var existing = await FindAsync(trimmed, cancellationToken);

            if (existing is not null)
                return existing.Id;

            // Unknown merchant — record the name as the user/receipt spelled it, website unknown.
            return await _merchantWriter.AddAsync(Merchant.Create(trimmed!), cancellationToken);
        }
    }
}
