using ExpenseTracker.Domain.Exceptions;

namespace ExpenseTracker.Domain.Entities
{
    /// <summary>
    /// An alternative spelling that receipts print for a merchant — "Tesco Stores Limited" for Tesco.
    /// Stored normalized so lookups use the same key as <see cref="Merchant.NormalizedName"/>.
    /// </summary>
    public class MerchantAlias
    {
        public int Id { get; private set; }
        public int MerchantId { get; private set; }
        public Merchant? Merchant { get; private set; }
        public string NormalizedAlias { get; private set; }

        private MerchantAlias() { }

        public static MerchantAlias Create(int merchantId, string alias)
        {
            var normalized = Entities.Merchant.Normalize(alias);

            if (normalized.Length == 0)
                throw new DomainException("Merchant alias must contain at least one letter or digit");

            return new MerchantAlias
            {
                MerchantId = merchantId,
                NormalizedAlias = normalized
            };
        }
    }
}
