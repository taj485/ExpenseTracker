
using ExpenseTracker.Domain.Exceptions;

namespace ExpenseTracker.Domain.ValueObjects
{
    public class Money
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        private Money() { }

        public Money Add(Money money)
        {
            return new Money
            {
                Amount = this.Amount + money.Amount,
                Currency = this.Currency
            };  
        }

        public static Money Create(decimal amount, string currency = "GBP")
        {
            if(amount <= 0)
            {
                throw new DomainException("Amount must be greater than zero.");
            }

            return new Money
            {
                Amount = amount,
                Currency = currency
            };

        }
    }
}
