using ExpenseTracker.Application.Commands.DeleteExpenseTable;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class DeleteExpenseTableCommandHandlerTests
    {
        private readonly Mock<IExpenseTableReader> _mockExpenseTableReader;
        private readonly Mock<IExpenseTableWriter> _mockExpenseTableWriter;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly DeleteExpenseTableCommandHandler _handler;
        private const int TableId = 1;

        public DeleteExpenseTableCommandHandlerTests()
        {
            _mockExpenseTableReader = new Mock<IExpenseTableReader>();
            _mockExpenseTableWriter = new Mock<IExpenseTableWriter>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _currentUser.Id = 1;
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            var validator = new DeleteExpenseTableValidator();

            _handler = new DeleteExpenseTableCommandHandler(_mockExpenseTableReader.Object, _mockExpenseTableWriter.Object, _mockCurrentUserProvider.Object, validator);
        }

        [Fact]
        public async Task Handle_WhenCallerIsAdmin_DeletesTable()
        {
            // Arrange
            var command = new DeleteExpenseTableCommand(TableId);

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockExpenseTableReader.Setup(x => x.IsAdminAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockExpenseTableWriter.Verify(x => x.DeleteAsync(TableId), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCallerIsNotMember_ThrowsNotFoundException()
        {
            // Arrange
            var command = new DeleteExpenseTableCommand(TableId);

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            _mockExpenseTableWriter.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenCallerIsMemberButNotAdmin_ThrowsForbiddenException()
        {
            // Arrange
            var command = new DeleteExpenseTableCommand(TableId);

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockExpenseTableReader.Setup(x => x.IsAdminAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();
            _mockExpenseTableWriter.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
