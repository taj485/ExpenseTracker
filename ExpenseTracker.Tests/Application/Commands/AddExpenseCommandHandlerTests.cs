using ExpenseTracker.Application.Commands.AddExpense;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class AddExpenseCommandHandlerTests
    {
        private readonly Mock<IExpenseWriter> _mockExpenseWriter;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly AddExpenseCommandHandler _handler;

        public AddExpenseCommandHandlerTests()
        {
            _mockExpenseWriter = new Mock<IExpenseWriter>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            var validator = new AddExpenseValidator();

            _handler = new AddExpenseCommandHandler(_mockExpenseWriter.Object, validator, _mockCurrentUserProvider.Object);
        }

        [Fact]
        public async void Handle_should_createAndSaveExpense()
        {
            // Arrange
            var command = new AddExpenseCommand(100m, ExpenseCategory.Food, "Dinner", DateTime.UtcNow);

            _mockExpenseWriter.Setup(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockExpenseWriter.Verify(x => x.AddAsync(It.Is<Expense>(e =>
                e.Amount.Amount == command.Amount &&
                e.Category == command.Category &&
                e.Description == command.Description &&
                e.Users.Contains(_currentUser)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Handle_WithInvalidAmount_ThrowsValidationException(decimal amount)
        {
            // Arrange
            var command = new AddExpenseCommand(amount, ExpenseCategory.Food, "Dinner", DateTime.UtcNow);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Amount*");

            _mockExpenseWriter.Verify(
                x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WithEmptyDescription_ThrowsValidationException()
        {
            // Arrange
            var command = new AddExpenseCommand(50m, ExpenseCategory.Food, "", DateTime.UtcNow);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Description*");

            _mockExpenseWriter.Verify(
                x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WithInvalidCategory_ThrowsValidationException()
        {
            // Arrange
            var command = new AddExpenseCommand(50m, (ExpenseCategory)999, "Dinner", DateTime.UtcNow);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Category*");

            _mockExpenseWriter.Verify(
                x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

    }
}