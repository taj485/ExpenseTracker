using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.AI;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Tests.Infrastructure
{
    public class ReceiptQuantityNormalizerTests
    {
        [Theory]
        [InlineData("Kg", QuantityUnit.Kg)]
        [InlineData(" kg ", QuantityUnit.Kg)]
        [InlineData("g", QuantityUnit.G)]
        [InlineData("LB", QuantityUnit.Lb)]
        [InlineData("l", QuantityUnit.L)]
        [InlineData("Item", QuantityUnit.Item)]
        public void ParseUnit_WithKnownUnit_ReturnsIt(string raw, QuantityUnit expected)
        {
            ReceiptQuantityNormalizer.ParseUnit(raw).Should().Be(expected);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("bunch")]
        [InlineData("7")]
        public void ParseUnit_WithMissingOrUnknownUnit_ReturnsItem(string? raw)
        {
            ReceiptQuantityNormalizer.ParseUnit(raw).Should().Be(QuantityUnit.Item);
        }

        [Theory]
        [InlineData(0.612, 0.612)]
        [InlineData(0.6125, 0.613)]
        [InlineData(1.25, 1.25)]
        [InlineData(0, 1)]
        [InlineData(-0.4, 1)]
        public void Normalize_ForWeighedItem_KeepsFractionToThreePlaces(double raw, double expected)
        {
            ReceiptQuantityNormalizer.Normalize((decimal)raw, QuantityUnit.Kg).Should().Be((decimal)expected);
        }

        [Theory]
        [InlineData(3, 3)]
        [InlineData(2.5, 3)]
        [InlineData(0.4, 1)]
        [InlineData(0, 1)]
        [InlineData(-2, 1)]
        public void Normalize_ForCountedItem_ReturnsWholeNumberOfAtLeastOne(double raw, double expected)
        {
            ReceiptQuantityNormalizer.Normalize((decimal)raw, QuantityUnit.Item).Should().Be((decimal)expected);
        }
    }
}
