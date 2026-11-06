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
    }
}
