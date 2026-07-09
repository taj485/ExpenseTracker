using ExpenseTracker.Application.Commands.UpdateExpense;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class UpdateExpenseCommandHandlerTests
    {
        private readonly Mock<IExpenseReader> _mockExpenseReader;
        private readonly Mock<IExpenseWriter> _mockExpenseWriter;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly UpdateExpenseCommandHandler _handler;

        public UpdateExpenseCommandHandlerTests()
        {
            _mockExpenseReader = new Mock<IExpenseReader>();
            _mockExpenseWriter = new Mock<IExpenseWriter>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _currentUser.Id = 1;
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            var validator = new UpdateExpenseValidator();

            _handler = new UpdateExpenseCommandHandler(_mockExpenseReader.Object, _mockExpenseWriter.Object, _mockCurrentUserProvider.Object, validator);
        }

        [Fact]
        public async Task Handle_WithValidCommand_UpdatesAndSavesExpense()
        {
            // Arrange
            var expense = Expense.Create(100m, ExpenseCategory.Food, "Dinner", DateTime.UtcNow, _currentUser);
            var command = new UpdateExpenseCommand(1, 150m, ExpenseCategory.Transport, "Taxi");

            _mockExpenseReader.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expense);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockExpenseWriter.Verify(x => x.UpdateAsync(It.Is<Expense>(e =>
                e.Amount.Amount == command.Amount &&
                e.Category == command.Category &&
                e.Description == command.Description)), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Handle_WithInvalidAmount_ThrowsValidationException(decimal amount)
        {
            // Arrange
            var command = new UpdateExpenseCommand(1, amount, ExpenseCategory.Food, "Dinner");

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Amount*");

            _mockExpenseWriter.Verify(
                x => x.UpdateAsync(It.IsAny<Expense>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WithEmptyDescription_ThrowsValidationException()
        {
            // Arrange
            var command = new UpdateExpenseCommand(1, 50m, ExpenseCategory.Food, "");

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Description*");

            _mockExpenseWriter.Verify(
                x => x.UpdateAsync(It.IsAny<Expense>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WithInvalidCategory_ThrowsValidationException()
        {
            // Arrange
            var command = new UpdateExpenseCommand(1, 50m, (ExpenseCategory)999, "Dinner");

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Category*");

            _mockExpenseWriter.Verify(
                x => x.UpdateAsync(It.IsAny<Expense>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenExpenseNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var command = new UpdateExpenseCommand(1, 50m, ExpenseCategory.Food, "Dinner");

            _mockExpenseReader.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Expense?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            _mockExpenseWriter.Verify(
                x => x.UpdateAsync(It.IsAny<Expense>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenCurrentUserNotAssociated_ThrowsNotFoundException()
        {
            // Arrange
            var otherUser = User.Create("auth0|other-user");
            otherUser.Id = 2;
            var expense = Expense.Create(100m, ExpenseCategory.Food, "Dinner", DateTime.UtcNow, otherUser);
            var command = new UpdateExpenseCommand(1, 150m, ExpenseCategory.Transport, "Taxi");

            _mockExpenseReader.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expense);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            _mockExpenseWriter.Verify(
                x => x.UpdateAsync(It.IsAny<Expense>()),
                Times.Never);
        }
    }
}