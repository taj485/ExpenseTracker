using ExpenseTracker.Application.Queries.GetAllExpenses;
using ExpenseTracker.Application.Services;
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
        private readonly Mock<IExpenseTableReader> _mockExpenseTableReader;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly GetAllExpensesQueryHandler _handler;
        private const int TableId = 1;

        public GetAllExpensesQueryHandlerTests()
        {
            _mockReader = new Mock<IExpenseReader>();
            _mockExpenseTableReader = new Mock<IExpenseTableReader>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _currentUser.Id = 1;
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            _handler = new GetAllExpensesQueryHandler(_mockReader.Object, _mockExpenseTableReader.Object, _mockCurrentUserProvider.Object);
        }

        [Fact]
        public async Task Handle_ReturnsListofDtoExpenses()
        {
            //Arrange
            var expenses = new List<Expense>
            {
                Expense.Create(1, ExpenseCategory.Transport, "Bus Fare", DateTime.UtcNow, TableId),
                Expense.Create(2, ExpenseCategory.Food, "Lunch", DateTime.UtcNow, TableId),
                Expense.Create(3, ExpenseCategory.Entertainment, "Movie Ticket", DateTime.UtcNow, TableId)
            };

            _mockExpenseTableReader.Setup(x => x.GetByIdAsync(TableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ExpenseTable.Create("My Table", _currentUser.Id));
            _mockReader.Setup(x => x.GetAllForTableAsync(TableId, It.IsAny<CancellationToken>())).ReturnsAsync(expenses);

            var request = new GetAllExpensesQuery { ExpenseTableId = TableId };

            //Act
            var result = await _handler.Handle(request, CancellationToken.None);

            //Assert
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenCurrentUserNotAMember()
        {
            //Arrange
            _mockExpenseTableReader.Setup(x => x.GetByIdAsync(TableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ExpenseTable?)null);

            var request = new GetAllExpensesQuery { ExpenseTableId = TableId };

            //Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
