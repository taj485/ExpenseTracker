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
        public async Task Handle_MapsCreatorEmail_AndFlagsExpensesAddedByCurrentUser()
        {
            //Arrange
            var otherUser = User.Create("auth0|other-user", "sarah@example.com");
            otherUser.Id = 2;
            _currentUser.UpdateEmail("me@example.com");

            var mine = Expense.Create(10, ExpenseCategory.Food, "Lunch", DateTime.UtcNow, TableId, createdByUserId: _currentUser.Id);
            mine.CreatedByUser = _currentUser;
            var theirs = Expense.Create(20, ExpenseCategory.Transport, "Taxi", DateTime.UtcNow, TableId, createdByUserId: otherUser.Id);
            theirs.CreatedByUser = otherUser;
            var unknown = Expense.Create(30, ExpenseCategory.Health, "Pharmacy", DateTime.UtcNow, TableId);

            _mockExpenseTableReader.Setup(x => x.GetByIdAsync(TableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ExpenseTable.Create("My Table", _currentUser.Id));
            _mockReader.Setup(x => x.GetAllForTableAsync(TableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Expense> { mine, theirs, unknown });

            //Act
            var result = await _handler.Handle(new GetAllExpensesQuery { ExpenseTableId = TableId }, CancellationToken.None);

            //Assert
            var lunch = result.Single(e => e.Description == "Lunch");
            lunch.CreatedByEmail.Should().Be("me@example.com");
            lunch.CreatedByCurrentUser.Should().BeTrue();

            var taxi = result.Single(e => e.Description == "Taxi");
            taxi.CreatedByEmail.Should().Be("sarah@example.com");
            taxi.CreatedByCurrentUser.Should().BeFalse();

            var pharmacy = result.Single(e => e.Description == "Pharmacy");
            pharmacy.CreatedByEmail.Should().BeNull();
            pharmacy.CreatedByCurrentUser.Should().BeFalse();
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
