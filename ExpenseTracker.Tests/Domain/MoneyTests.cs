using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.ValueObjects;
using FluentAssertions;

namespace ExpenseTracker.Tests.Domain
{
    public class MoneyTests
    {
        [Fact]
        public void Create_shouldReturn_MoneyWithAmountInGDP()
        {
            //arrange
            var amount = 100;
            //act
            var money = Money.Create(amount);
            //assert
            money.Amount.Should().Be(amount);
            money.Currency.Should().Be("GBP");
        }

        //- When I create money with zero, it should throw an error
        [Fact]
        public void Create_shouldThrowError_WhenAmountIsZero()
        {
            //arrange
            var amount = 0;
            //act
            Action act = () => Money.Create(amount);
            //assert
            act.Should().Throw<DomainException>().WithMessage("Amount must be greater than zero.");
        }

        //- When I add two amounts of the same currency together, it should return the correct total
        [Fact]
        public void Add_shouldReturn_CorrectTotal_WhenCurrenciesAreTheSame()
        {
            //arrange
            var money1 = Money.Create(100);
            var money2 = Money.Create(50);
            //act
            var total = money2.Add(money1);
            //assert
            total.Amount.Should().Be(150);
            total.Currency.Should().Be("GBP");
        }
    }
}
