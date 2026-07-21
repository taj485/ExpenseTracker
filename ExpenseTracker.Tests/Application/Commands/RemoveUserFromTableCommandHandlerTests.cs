using ExpenseTracker.Application.Commands.RemoveUserFromTable;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class RemoveUserFromTableCommandHandlerTests
    {
        private readonly Mock<IExpenseTableReader> _mockExpenseTableReader;
        private readonly Mock<IExpenseTableWriter> _mockExpenseTableWriter;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly RemoveUserFromTableCommandHandler _handler;
        private const int TableId = 1;

        public RemoveUserFromTableCommandHandlerTests()
        {
            _mockExpenseTableReader = new Mock<IExpenseTableReader>();
            _mockExpenseTableWriter = new Mock<IExpenseTableWriter>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _currentUser.Id = 1;
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            var validator = new RemoveUserFromTableValidator();

            _handler = new RemoveUserFromTableCommandHandler(_mockExpenseTableReader.Object, _mockExpenseTableWriter.Object, _mockCurrentUserProvider.Object, validator);
        }

        [Fact]
        public async Task Handle_WhenAdminRemovesAnotherMember_RemovesThem()
        {
            // Arrange
            var table = ExpenseTable.Create("Household", _currentUser.Id);
            table.AddMember(2, isAdmin: false);
            var command = new RemoveUserFromTableCommand(TableId, 2);

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockExpenseTableReader.Setup(x => x.IsAdminAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockExpenseTableReader.Setup(x => x.GetByIdAsync(TableId, It.IsAny<CancellationToken>())).ReturnsAsync(table);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            table.Members.Should().ContainSingle(m => m.UserId == _currentUser.Id);
            _mockExpenseTableWriter.Verify(x => x.UpdateAsync(table), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNonAdminMemberRemovesSelf_Allowed()
        {
            // Arrange
            var admin = User.Create("auth0|admin");
            admin.Id = 2;
            var table = ExpenseTable.Create("Household", admin.Id);
            table.AddMember(_currentUser.Id, isAdmin: false);
            var command = new RemoveUserFromTableCommand(TableId, _currentUser.Id);

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockExpenseTableReader.Setup(x => x.GetByIdAsync(TableId, It.IsAny<CancellationToken>())).ReturnsAsync(table);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            table.Members.Should().ContainSingle(m => m.UserId == admin.Id);
            _mockExpenseTableWriter.Verify(x => x.UpdateAsync(table), Times.Once);
            _mockExpenseTableReader.Verify(x => x.IsAdminAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenNonAdminMemberTriesToRemoveSomeoneElse_ThrowsForbiddenException()
        {
            // Arrange
            var command = new RemoveUserFromTableCommand(TableId, 2);

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockExpenseTableReader.Setup(x => x.IsAdminAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task Handle_WhenCallerIsNotMember_ThrowsNotFoundException()
        {
            // Arrange
            var command = new RemoveUserFromTableCommand(TableId, 2);

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenRemovingLastAdmin_ThrowsDomainException()
        {
            // Arrange
            var table = ExpenseTable.Create("Household", _currentUser.Id);
            var command = new RemoveUserFromTableCommand(TableId, _currentUser.Id);

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockExpenseTableReader.Setup(x => x.GetByIdAsync(TableId, It.IsAny<CancellationToken>())).ReturnsAsync(table);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
