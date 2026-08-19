using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using FluentAssertions;

namespace ExpenseTracker.Tests.Domain
{
    public class MerchantTests
    {
        [Theory]
        [InlineData("Tesco", "tesco")]
        [InlineData("tesco", "tesco")]
        [InlineData("TESCO", "tesco")]
        [InlineData("  Tesco  ", "tesco")]
        [InlineData("Tesco Express", "tescoexpress")]
        [InlineData("Sainsbury's", "sainsburys")]
        [InlineData("M&S", "ms")]
        [InlineData("B&Q", "bq")]
        [InlineData("Co-op", "coop")]
        [InlineData("E.ON", "eon")]
        [InlineData("Caffé Nero", "caffenero")]  // combining acute
        [InlineData("Caffè Nero", "caffenero")]   // precomposed grave
        [InlineData("Jet2", "jet2")]
        [InlineData("YO! Sushi", "yosushi")]
        public void Normalize_stripsCaseWhitespaceAccentsAndPunctuation(string input, string expected)
        {
            Merchant.Normalize(input).Should().Be(expected);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("!!!")]
        public void Normalize_returnsEmpty_whenThereIsNothingToMatchOn(string? input)
        {
            Merchant.Normalize(input).Should().BeEmpty();
        }

        [Fact]
        public void Normalize_treatsCasingAndSpacingVariantsAsTheSameMerchant()
        {
            var variants = new[] { "Tesco", "tesco", " TESCO ", "Te s co" };

            variants.Select(Merchant.Normalize).Distinct().Should().ContainSingle();
        }

        [Fact]
        public void Create_trimsTheDisplayNameAndDerivesTheKey()
        {
            var merchant = Merchant.Create("  Tesco Express  ", "tesco.com");

            merchant.Name.Should().Be("Tesco Express");
            merchant.NormalizedName.Should().Be("tescoexpress");
            merchant.Website.Should().Be("tesco.com");
        }

        [Fact]
        public void Create_leavesWebsiteNull_whenNotSupplied()
        {
            Merchant.Create("Some Corner Shop").Website.Should().BeNull();
        }

        [Fact]
        public void Create_normalizesBlankWebsiteToNull()
        {
            Merchant.Create("Some Corner Shop", "   ").Website.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_throws_whenNameIsMissing(string? name)
        {
            Action act = () => Merchant.Create(name!);

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Create_throws_whenNameHasNoAlphanumericContent()
        {
            Action act = () => Merchant.Create("!!!");

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void SetWebsite_clearsTheWebsite_whenGivenBlank()
        {
            var merchant = Merchant.Create("Tesco", "tesco.com");

            merchant.SetWebsite("  ");

            merchant.Website.Should().BeNull();
        }
    }
}
