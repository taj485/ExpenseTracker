using ExpenseTracker.Application.Queries.GetExpenseTableMembers;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.ValueObjects;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace ExpenseTracker.Tests.Application.Queries
{
    public class GetExpenseTableMembersQueryHandlerTests
    {
        private const int TableId = 10;

        private readonly Mock<IExpenseTableReader> _mockExpenseTableReader;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly GetExpenseTableMembersQueryHandler _handler;

        public GetExpenseTableMembersQueryHandlerTests()
        {
            _mockExpenseTableReader = new Mock<IExpenseTableReader>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user", "me@example.com");
            _currentUser.Id = 1;
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            _handler = new GetExpenseTableMembersQueryHandler(
                _mockExpenseTableReader.Object,
                _mockCurrentUserProvider.Object,
                new GetExpenseTableMembersValidator());
        }

        private void SetupMembership(bool isMember) =>
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(isMember);

        [Fact]
        public async Task Handle_ReturnsMembers_WithCurrentUserFlagged()
        {
            // Arrange
            SetupMembership(true);
            _mockExpenseTableReader.Setup(x => x.GetMembersAsync(TableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ExpenseTableMember>
                {
                    new() { UserId = _currentUser.Id, Email = "me@example.com", IsAdmin = true },
                    new() { UserId = 7, Email = "sarah@example.com", IsAdmin = false }
                });

            // Act
            var result = await _handler.Handle(new GetExpenseTableMembersQuery(TableId), CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            var me = result.Single(m => m.UserId == _currentUser.Id);
            me.IsCurrentUser.Should().BeTrue();
            me.IsAdmin.Should().BeTrue();
            me.Email.Should().Be("me@example.com");
            result.Single(m => m.UserId == 7).IsCurrentUser.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_OrdersAdminsFirst_ThenByEmail()
        {
            // Arrange
            SetupMembership(true);
            _mockExpenseTableReader.Setup(x => x.GetMembersAsync(TableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ExpenseTableMember>
                {
                    new() { UserId = 3, Email = "zoe@example.com", IsAdmin = false },
                    new() { UserId = 4, Email = "amy@example.com", IsAdmin = false },
                    new() { UserId = _currentUser.Id, Email = "me@example.com", IsAdmin = true }
                });

            // Act
            var result = await _handler.Handle(new GetExpenseTableMembersQuery(TableId), CancellationToken.None);

            // Assert
            result.Select(m => m.Email).Should().ContainInOrder("me@example.com", "amy@example.com", "zoe@example.com");
        }

        [Fact]
        public async Task Handle_WhenCurrentUserIsNotAMember_ThrowsNotFoundException_AndDoesNotLoadMembers()
        {
            // Arrange
            SetupMembership(false);

            // Act
            var act = () => _handler.Handle(new GetExpenseTableMembersQuery(TableId), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            _mockExpenseTableReader.Verify(x => x.GetMembersAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenTableIdIsInvalid_ThrowsValidationException()
        {
            // Act
            var act = () => _handler.Handle(new GetExpenseTableMembersQuery(0), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
