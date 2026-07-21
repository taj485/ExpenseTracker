using ExpenseTracker.Application.Commands.InviteUserToTable;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class InviteUserToTableCommandHandlerTests
    {
        private readonly Mock<IExpenseTableReader> _mockExpenseTableReader;
        private readonly Mock<IExpenseTableWriter> _mockExpenseTableWriter;
        private readonly Mock<IUserReader> _mockUserReader;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly InviteUserToTableCommandHandler _handler;
        private const int TableId = 1;

        public InviteUserToTableCommandHandlerTests()
        {
            _mockExpenseTableReader = new Mock<IExpenseTableReader>();
            _mockExpenseTableWriter = new Mock<IExpenseTableWriter>();
            _mockUserReader = new Mock<IUserReader>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _currentUser.Id = 1;
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            var validator = new InviteUserToTableValidator();

            _handler = new InviteUserToTableCommandHandler(_mockExpenseTableReader.Object, _mockExpenseTableWriter.Object, _mockUserReader.Object, _mockCurrentUserProvider.Object, validator);
        }

        [Fact]
        public async Task Handle_WhenCallerIsAdminAndInviteeExists_AddsMember()
        {
            // Arrange
            var invitee = User.Create("auth0|invitee");
            invitee.Id = 2;
            var table = ExpenseTable.Create("Household", _currentUser.Id);
            var command = new InviteUserToTableCommand(TableId, "invitee@example.com");

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockExpenseTableReader.Setup(x => x.IsAdminAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockUserReader.Setup(x => x.GetAllByEmailAsync("invitee@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User> { invitee });
            _mockExpenseTableReader.Setup(x => x.GetByIdAsync(TableId, It.IsAny<CancellationToken>())).ReturnsAsync(table);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            table.Members.Should().Contain(m => m.UserId == 2 && !m.IsAdmin);
            _mockExpenseTableWriter.Verify(x => x.UpdateAsync(table), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCallerIsNotMember_ThrowsNotFoundException()
        {
            // Arrange
            var command = new InviteUserToTableCommand(TableId, "invitee@example.com");

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenCallerIsMemberButNotAdmin_ThrowsForbiddenException()
        {
            // Arrange
            var command = new InviteUserToTableCommand(TableId, "invitee@example.com");

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockExpenseTableReader.Setup(x => x.IsAdminAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task Handle_WhenNoUserHasThatEmail_ThrowsNotFoundException()
        {
            // Arrange
            var command = new InviteUserToTableCommand(TableId, "nobody@example.com");

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockExpenseTableReader.Setup(x => x.IsAdminAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockUserReader.Setup(x => x.GetAllByEmailAsync("nobody@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>());

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenMultipleUsersShareThatEmail_ThrowsDomainException()
        {
            // Arrange
            var command = new InviteUserToTableCommand(TableId, "duplicate@example.com");

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockExpenseTableReader.Setup(x => x.IsAdminAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockUserReader.Setup(x => x.GetAllByEmailAsync("duplicate@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User> { User.Create("auth0|a"), User.Create("auth0|b") });

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
