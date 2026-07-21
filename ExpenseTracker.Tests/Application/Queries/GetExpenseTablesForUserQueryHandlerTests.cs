using ExpenseTracker.Application.Queries.GetExpenseTablesForUser;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Tests.Application.Queries
{
    public class GetExpenseTablesForUserQueryHandlerTests
    {
        private readonly Mock<IExpenseTableReader> _mockExpenseTableReader;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly GetExpenseTablesForUserQueryHandler _handler;

        public GetExpenseTablesForUserQueryHandlerTests()
        {
            _mockExpenseTableReader = new Mock<IExpenseTableReader>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _currentUser.Id = 1;
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            _handler = new GetExpenseTablesForUserQueryHandler(_mockExpenseTableReader.Object, _mockCurrentUserProvider.Object);
        }

        [Fact]
        public async Task Handle_ReturnsDtosWithAdminFlagAndMemberCount()
        {
            // Arrange
            var adminTable = ExpenseTable.Create("Household", _currentUser.Id);
            var memberTable = ExpenseTable.Create("Work Trip", 2);
            memberTable.AddMember(_currentUser.Id, isAdmin: false);

            _mockExpenseTableReader.Setup(x => x.GetAllForUserAsync(_currentUser.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ExpenseTable> { adminTable, memberTable });

            // Act
            var result = await _handler.Handle(new GetExpenseTablesForUserQuery(), CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Single(t => t.Name == "Household").IsCurrentUserAdmin.Should().BeTrue();
            result.Single(t => t.Name == "Work Trip").IsCurrentUserAdmin.Should().BeFalse();
            result.Single(t => t.Name == "Work Trip").MemberCount.Should().Be(2);
        }
    }
}
