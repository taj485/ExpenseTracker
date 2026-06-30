using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Queries.GetAllExpenses;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Tests.Application.Queries
{
    public class GetAllExpensesQueryHandlerTests
    {
        private readonly Mock<IExpenseReader> _mockReader;
        private readonly GetAllExpensesQueryHandler _handler;

        public GetAllExpensesQueryHandlerTests()
        {
            _mockReader = new Mock<IExpenseReader>();
            _handler = new GetAllExpensesQueryHandler(_mockReader.Object);
        }

        [Fact]
        public async Task Handle_ReturnsListofDtoExpenses()
        {
            //Arrange
            var expenses = new List<Expense>
            {
                Expense.Create(1, ExpenseCategory.Transport, "Bus Fare"),
                Expense.Create(2, ExpenseCategory.Food, "Lunch"),
                Expense.Create(3, ExpenseCategory.Entertainment, "Movie Ticket")
            };

            _mockReader.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expenses);

            var request = new GetAllExpensesQuery();

            //Act
            var result = await _handler.Handle(request, CancellationToken.None);

            //Assert
            var expected = expenses.Select(x => new ExpenseDto
            {
                Amount = x.Amount.Amount,
                Category = x.Category.ToString(),
                Description = x.Description
            });

            result.Should().HaveCount(3);
        }
    }
}
