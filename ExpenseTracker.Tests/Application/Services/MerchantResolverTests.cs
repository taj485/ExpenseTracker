using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Tests.Application.Services
{
    public class MerchantResolverTests
    {
        private readonly Mock<IMerchantReader> _mockReader = new();
        private readonly Mock<IMerchantWriter> _mockWriter = new();
        private readonly MerchantResolver _resolver;

        public MerchantResolverTests()
        {
            _resolver = new MerchantResolver(_mockReader.Object, _mockWriter.Object);
        }

        private static Merchant KnownMerchant(int id, string name, string? website = null)
        {
            var merchant = Merchant.Create(name, website);
            // Id is set by the database in production; assign it here for assertion purposes.
            typeof(Merchant).GetProperty(nameof(Merchant.Id))!.SetValue(merchant, id);
            return merchant;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("???")]
        public async Task ResolveOrCreateAsync_returnsNull_whenThereIsNoUsableName(string? raw)
        {
            var result = await _resolver.ResolveOrCreateAsync(raw, CancellationToken.None);

            result.Should().BeNull();
            _mockWriter.Verify(x => x.AddAsync(It.IsAny<Merchant>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ResolveOrCreateAsync_reusesTheExistingMerchant_withoutWriting()
        {
            _mockReader.Setup(x => x.GetByNormalizedNameAsync("tesco", It.IsAny<CancellationToken>()))
                .ReturnsAsync(KnownMerchant(1, "Tesco", "tesco.com"));

            var result = await _resolver.ResolveOrCreateAsync("  TESCO  ", CancellationToken.None);

            result.Should().Be(1);
            _mockWriter.Verify(x => x.AddAsync(It.IsAny<Merchant>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ResolveOrCreateAsync_resolvesThroughAnAlias()
        {
            // The reader is responsible for the alias fallback, so "Tesco Stores Limited"
            // arrives here normalized and comes back as Tesco itself.
            _mockReader.Setup(x => x.GetByNormalizedNameAsync("tescostoreslimited", It.IsAny<CancellationToken>()))
                .ReturnsAsync(KnownMerchant(1, "Tesco", "tesco.com"));

            var result = await _resolver.ResolveOrCreateAsync("TESCO STORES LIMITED", CancellationToken.None);

            result.Should().Be(1);
            _mockWriter.Verify(x => x.AddAsync(It.IsAny<Merchant>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ResolveOrCreateAsync_createsAnUnknownMerchantWithNoWebsite()
        {
            _mockReader.Setup(x => x.GetByNormalizedNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Merchant?)null);

            Merchant? created = null;
            _mockWriter.Setup(x => x.AddAsync(It.IsAny<Merchant>(), It.IsAny<CancellationToken>()))
                .Callback<Merchant, CancellationToken>((m, _) => created = m)
                .ReturnsAsync(42);

            var result = await _resolver.ResolveOrCreateAsync("  Bob's Corner Shop  ", CancellationToken.None);

            result.Should().Be(42);
            created.Should().NotBeNull();
            created!.Name.Should().Be("Bob's Corner Shop");
            created.NormalizedName.Should().Be("bobscornershop");
            created.Website.Should().BeNull();
        }

        [Fact]
        public async Task FindAsync_neverWrites_evenWhenTheMerchantIsUnknown()
        {
            _mockReader.Setup(x => x.GetByNormalizedNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Merchant?)null);

            var result = await _resolver.FindAsync("Totally New Shop", CancellationToken.None);

            result.Should().BeNull();
            _mockWriter.Verify(x => x.AddAsync(It.IsAny<Merchant>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("  ")]
        public async Task FindAsync_doesNotEvenQuery_whenThereIsNoUsableName(string? raw)
        {
            var result = await _resolver.FindAsync(raw, CancellationToken.None);

            result.Should().BeNull();
            _mockReader.Verify(x => x.GetByNormalizedNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
