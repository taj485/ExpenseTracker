using ExpenseTracker.Domain.Exceptions;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ExpenseTracker.Domain.Entities
{
    public class Merchant
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string NormalizedName { get; private set; }
        public string? Website { get; private set; }
        public ICollection<MerchantAlias> Aliases { get; private set; } = new List<MerchantAlias>();

        private Merchant() { }

        public static Merchant Create(string name, string? website = null)
        {
            var trimmed = name?.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
                throw new DomainException("Merchant name is required");

            var normalized = Normalize(trimmed);

            if (normalized.Length == 0)
                throw new DomainException("Merchant name must contain at least one letter or digit");

            return new Merchant
            {
                Name = trimmed,
                NormalizedName = normalized,
                Website = string.IsNullOrWhiteSpace(website) ? null : website.Trim()
            };
        }

        public void SetWebsite(string? website)
        {
            Website = string.IsNullOrWhiteSpace(website) ? null : website.Trim();
        }

        /// <summary>
        /// Match key: lowercased, accents stripped, every non-alphanumeric dropped.
        /// Mirrors the frontend slug in Client/src/app/core/utils/merchant.utils.ts and the
        /// backfill SQL in the AddMerchantReferenceTable migration — change all three together.
        /// </summary>
        public static string Normalize(string? name)
        {
            var decomposed = (name ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Normalize(NormalizationForm.FormD);

            var builder = new StringBuilder(decomposed.Length);

            foreach (var ch in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark)
                    continue;

                if (ch is >= 'a' and <= 'z' or >= '0' and <= '9')
                    builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}
