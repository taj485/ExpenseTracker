using ExpenseTracker.Domain.Entities;
using System.Collections.Generic;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IMerchantReader
    {
        /// <summary>
        /// Looks up a merchant by its normalized key, matching on the merchant's own name first
        /// and falling back to its aliases.
        /// </summary>
        Task<Merchant?> GetByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default);

        Task<Merchant?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Merchant>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
