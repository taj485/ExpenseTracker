using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;

namespace ExpenseTracker.Tests.Infrastructure
{
    /// <summary>
    /// The seed rows go straight into a migration, where a duplicate key or a stale id is painful
    /// to unpick. These guard the invariants the unique indexes rely on.
    /// </summary>
    public class MerchantSeedDataTests
    {
        private static T Get<T>(object row, string property) =>
            (T)row.GetType().GetProperty(property)!.GetValue(row)!;

        private static IEnumerable<string> NormalizedNames() =>
            MerchantSeedData.Merchants.Select(m => Get<string>(m, "NormalizedName"));

        private static IEnumerable<string> NormalizedAliases() =>
            MerchantSeedData.Aliases.Select(a => Get<string>(a, "NormalizedAlias"));

        [Fact]
        public void Seeds_oneThousandMerchants()
        {
            MerchantSeedData.Merchants.Should().HaveCount(1000);
        }

        [Fact]
        public void MerchantIds_occupyTheTwoReservedRanges()
        {
            var ids = MerchantSeedData.Merchants.Select(m => Get<int>(m, "Id")).ToList();

            ids.Should().OnlyHaveUniqueItems();

            // 1-200 is the original batch. The extended batch starts at 1001 rather than 201
            // because ids just above 200 are handed out by the identity sequence to merchants
            // users create at runtime — see the note in MerchantSeedData.
            ids.Should().BeEquivalentTo(
                Enumerable.Range(1, 200).Concat(Enumerable.Range(1001, 800)));
        }

        [Fact]
        public void NoSeededId_fallsInTheRangeReservedForUserCreatedMerchants()
        {
            var ids = MerchantSeedData.Merchants.Select(m => Get<int>(m, "Id"));

            ids.Should().NotContain(id => id > 200 && id < 1001);
        }

        [Fact]
        public void AliasIds_areUniqueAndContiguousFromOne()
        {
            var ids = MerchantSeedData.Aliases.Select(a => Get<int>(a, "Id")).ToList();

            ids.Should().OnlyHaveUniqueItems();
            ids.Should().BeEquivalentTo(Enumerable.Range(1, ids.Count));
        }

        [Fact]
        public void NormalizedNames_areUnique()
        {
            var duplicates = NormalizedNames()
                .GroupBy(n => n)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            duplicates.Should().BeEmpty("the Merchants.NormalizedName index is unique");
        }

        [Fact]
        public void NormalizedAliases_areUnique()
        {
            var duplicates = NormalizedAliases()
                .GroupBy(a => a)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            duplicates.Should().BeEmpty("the MerchantAliases.NormalizedAlias index is unique");
        }

        [Fact]
        public void NoAlias_shadowsAMerchantName()
        {
            // Harmless but pointless: name lookup wins first, so such an alias could never match.
            NormalizedAliases().Intersect(NormalizedNames()).Should().BeEmpty();
        }

        [Fact]
        public void EveryAlias_pointsAtASeededMerchant()
        {
            var merchantIds = MerchantSeedData.Merchants.Select(m => Get<int>(m, "Id")).ToHashSet();
            var orphans = MerchantSeedData.Aliases
                .Select(a => Get<int>(a, "MerchantId"))
                .Where(id => !merchantIds.Contains(id));

            orphans.Should().BeEmpty();
        }

        [Fact]
        public void EveryMerchant_hasANameAndAWebsite()
        {
            foreach (var merchant in MerchantSeedData.Merchants)
            {
                Get<string>(merchant, "Name").Should().NotBeNullOrWhiteSpace();
                Get<string>(merchant, "NormalizedName").Should().NotBeNullOrWhiteSpace();
                Get<string?>(merchant, "Website").Should().NotBeNullOrWhiteSpace(
                    "seeded merchants are the ones we know the domain for");
            }
        }
    }
}
