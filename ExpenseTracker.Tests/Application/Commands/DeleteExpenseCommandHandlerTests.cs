using ExpenseTracker.Application.Commands.DeleteExpense;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class DeleteExpenseCommandHandlerTests
    {
        private readonly Mock<IExpenseReader> _mockExpenseReader;
        private readonly Mock<IExpenseWriter> _mockExpenseWriter;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly DeleteExpenseCommandHandler _handler;

        public DeleteExpenseCommandHandlerTests()
        {
            _mockExpenseReader = new Mock<IExpenseReader>();
            _mockExpenseWriter = new Mock<IExpenseWriter>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _currentUser.Id = 1;
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            var validator = new DeleteExpenseValidator();

            _handler = new DeleteExpenseCommandHandler(_mockExpenseReader.Object, _mockExpenseWriter.Object, _mockCurrentUserProvider.Object, validator);
        }

        [Fact]
        public async Task Handle_WithValidId_CallsDeleteAsync()
        {
            // Arrange
            var command = new DeleteExpenseCommand(1);
            var expense = Expense.Create(10m, ExpenseCategory.Food, "Coffee", _currentUser);

            _mockExpenseReader.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expense);

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
        public async Task Handle_WhenExpenseNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var command = new DeleteExpenseCommand(999);

            _mockExpenseReader.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Expense?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            _mockExpenseWriter.Verify(
                x => x.DeleteAsync(It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenCurrentUserNotAssociated_ThrowsNotFoundException()
        {
            // Arrange
            var command = new DeleteExpenseCommand(1);
            var otherUser = User.Create("auth0|other-user");
            otherUser.Id = 2;
            var expense = Expense.Create(10m, ExpenseCategory.Food, "Coffee", otherUser);

            _mockExpenseReader.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expense);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            _mockExpenseWriter.Verify(
                x => x.DeleteAsync(It.IsAny<int>()),
                Times.Never);
        }
    }
}