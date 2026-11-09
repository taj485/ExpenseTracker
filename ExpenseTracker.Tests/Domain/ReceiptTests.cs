using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using FluentAssertions;

namespace ExpenseTracker.Tests.Domain
{
    public class ReceiptTests
    {
        [Fact]
        public void Create_KeepsTheDiscount_AndDefaultsToNone()
        {
            Receipt.Create(DateTime.UtcNow, null, 2.5m).Discount.Should().Be(2.5m);
            Receipt.Create(DateTime.UtcNow).Discount.Should().Be(0m);
        }

        [Fact]
        public void Create_WithNegativeDiscount_Throws()
        {
            Action act = () => Receipt.Create(DateTime.UtcNow, null, -0.01m);

            act.Should().Throw<DomainException>().WithMessage("Receipt discount can't be negative.");
        }
    
        [Fact]
        public void UpdateDiscount_SetsAndClearsTheDiscount_UpToTheTotal()
        {
            var receipt = Receipt.Create(DateTime.UtcNow, null, 2m);

            receipt.UpdateDiscount(7.50m, 7.50m);
            receipt.Discount.Should().Be(7.50m);

            receipt.UpdateDiscount(0m, 7.50m);
            receipt.Discount.Should().Be(0m);
        }

        [Fact]
        public void UpdateDiscount_WithNegativeDiscount_Throws()
        {
            Action act = () => Receipt.Create(DateTime.UtcNow).UpdateDiscount(-1m, 7.50m);

            act.Should().Throw<DomainException>().WithMessage("Receipt discount can't be negative.");
        }

        [Fact]
        public void UpdateDiscount_AboveTheTotal_ThrowsWithTheTotal()
        {
            Action act = () => Receipt.Create(DateTime.UtcNow).UpdateDiscount(8m, 7.50m);

            act.Should().Throw<DomainException>().WithMessage("Receipt discount can't be more than the receipt total (£7.50).");
        }
    }
}
