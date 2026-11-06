using ExpenseTracker.Application.DTO;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Tests.Application.Queries
{
    public class ExpenseDtoMapperTests
    {
        [Fact]
        public void ToDto_CopiesLineAndDefaultsReceiptDiscountToZero_WhenNoReceipt()
        {
            var expense = Expense.Create(2, ExpenseCategory.Transport, "Bus Fare", DateTime.UtcNow, 1);

            var dto = expense.ToDto(currentUserId: 7);

            Assert.Equal("Bus Fare", dto.Description);
            Assert.Equal(0m, dto.ReceiptDiscount);
            Assert.Equal(0m, dto.Discount);
            Assert.False(dto.CreatedByCurrentUser);
        }

        [Fact]
        public void ToDto_CopiesTheReceiptDiscount_WhenTheExpenseIsOnAReceipt()
        {
            var expense = Expense.Create(2, ExpenseCategory.Food, "Milk", DateTime.UtcNow, 1, receiptId: 5, discount: 0.5m);
            typeof(Expense).GetProperty(nameof(Expense.Receipt))!
                .SetValue(expense, Receipt.Create(DateTime.UtcNow, null, 1.25m));

            var dto = expense.ToDto(currentUserId: 7);

            Assert.Equal(1.25m, dto.ReceiptDiscount);
            Assert.Equal(0.5m, dto.Discount);
        }
    }
}
