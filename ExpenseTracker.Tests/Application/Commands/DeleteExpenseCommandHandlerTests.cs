using ExpenseTracker.Application.Commands.DeleteExpense;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class DeleteExpenseCommandHandlerTests
    {
        private readonly Mock<IExpenseWriter> _mockExpenseWriter;
        private readonly DeleteExpenseCommandHandler _handler;

        public DeleteExpenseCommandHandlerTests()
        {
            _mockExpenseWriter = new Mock<IExpenseWriter>();
            var validator = new DeleteExpenseValidator();

            _handler = new DeleteExpenseCommandHandler(_mockExpenseWriter.Object, validator);
        }

        [Fact]
        public async Task Handle_WithValidId_CallsDeleteAsync()
        {
            // Arrange
            var command = new DeleteExpenseCommand(1);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockExpenseWriter.Verify(x => x.DeleteAsync(command.Id), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Handle_WithInvalidId_ThrowsValidationException(int id)
        {
            // Arrange
            var command = new DeleteExpenseCommand(id);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Id*");

            _mockExpenseWriter.Verify(
                x => x.DeleteAsync(It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenExpenseNotFound_PropagatesNotFoundException()
        {
            // Arrange
            var command = new DeleteExpenseCommand(999);

            _mockExpenseWriter.Setup(x => x.DeleteAsync(command.Id))
                .ThrowsAsync(new NotFoundException($"Expense with id {command.Id} was not found."));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
