using ExpenseTracker.Application.Commands.UnstarExpenseTable;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class UnstarExpenseTableCommandHandlerTests
    {
        private readonly Mock<IExpenseTableReader> _mockExpenseTableReader;
        private readonly Mock<IExpenseTableWriter> _mockExpenseTableWriter;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly UnstarExpenseTableCommandHandler _handler;
        private const int TableId = 1;

        public UnstarExpenseTableCommandHandlerTests()
        {
            _mockExpenseTableReader = new Mock<IExpenseTableReader>();
            _mockExpenseTableWriter = new Mock<IExpenseTableWriter>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _currentUser.Id = 1;
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            var validator = new UnstarExpenseTableValidator();

            _handler = new UnstarExpenseTableCommandHandler(_mockExpenseTableReader.Object, _mockExpenseTableWriter.Object, _mockCurrentUserProvider.Object, validator);
        }

        [Fact]
        public async Task Handle_WhenCallerIsMember_UnstarsTable()
        {
            // Arrange
            var command = new UnstarExpenseTableCommand(TableId);

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockExpenseTableWriter.Verify(x => x.UnstarTableAsync(_currentUser.Id, TableId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCallerIsNotMember_ThrowsNotFoundException()
        {
            // Arrange
            var command = new UnstarExpenseTableCommand(TableId);

            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            _mockExpenseTableWriter.Verify(x => x.UnstarTableAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
