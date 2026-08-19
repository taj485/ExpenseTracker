using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Services
{
    /// <summary>
    /// Turns the free-text merchant name that arrives from a receipt or a form into a row in the
    /// merchant reference table.
    /// </summary>
    public interface IMerchantResolver
    {
        /// <summary>
        /// Looks up a merchant without ever writing. Used on the receipt-extraction path, where the
        /// user may still cancel — nothing should be persisted until they save.
        /// </summary>
        Task<Merchant?> FindAsync(string? rawName, CancellationToken cancellationToken);

        /// <summary>
        /// Looks up a merchant and creates it (with a null website) when it isn't known yet.
        /// Returns null when the name is empty or has no alphanumeric content.
        /// </summary>
        Task<int?> ResolveOrCreateAsync(string? rawName, CancellationToken cancellationToken);
    }
}
