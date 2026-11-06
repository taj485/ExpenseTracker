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
    }
}
