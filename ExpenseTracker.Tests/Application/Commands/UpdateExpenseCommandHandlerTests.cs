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
        private readonly Mock<IExpenseTableReader> _mockExpenseTableReader;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly Mock<IMerchantResolver> _mockMerchantResolver;
        private readonly User _currentUser;
        private readonly UpdateExpenseCommandHandler _handler;
        private const int TableId = 1;

        public UpdateExpenseCommandHandlerTests()
        {
            _mockExpenseReader = new Mock<IExpenseReader>();
            _mockExpenseWriter = new Mock<IExpenseWriter>();
            _mockExpenseTableReader = new Mock<IExpenseTableReader>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _currentUser.Id = 1;
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            _mockMerchantResolver = new Mock<IMerchantResolver>();
            var validator = new UpdateExpenseValidator();

            _handler = new UpdateExpenseCommandHandler(_mockExpenseReader.Object, _mockExpenseWriter.Object, _mockExpenseTableReader.Object, _mockCurrentUserProvider.Object, validator, _mockMerchantResolver.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_UpdatesAndSavesExpense()
        {
            // Arrange
            var expense = Expense.Create(100m, ExpenseCategory.Food, "Dinner", DateTime.UtcNow, TableId);
            var command = new UpdateExpenseCommand(1, 150m, ExpenseCategory.Transport, "Taxi");

            _mockExpenseReader.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expense);
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockExpenseWriter.Verify(x => x.UpdateAsync(It.Is<Expense>(e =>
                e.UnitPrice.Amount == command.UnitPrice &&
                e.Category == command.Category &&
                e.Description == command.Description)), Times.Once);
        }

        [Fact]
        public async Task Handle_WithWeighedQuantity_UpdatesWeightAndUnit()
        {
            var expense = Expense.Create(5m, ExpenseCategory.Food, "Chicken", DateTime.UtcNow, TableId);
            var command = new UpdateExpenseCommand(1, 5m, ExpenseCategory.Food, "Chicken", Quantity: 0.75m, Unit: QuantityUnit.Kg);

            _mockExpenseReader.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expense);
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _handler.Handle(command, CancellationToken.None);

            _mockExpenseWriter.Verify(x => x.UpdateAsync(It.Is<Expense>(e =>
                e.Quantity == 0.75m &&
                e.Unit == QuantityUnit.Kg)), Times.Once);
        }

        [Fact]
        public async Task Handle_WithQuantity_UpdatesQuantity()
        {
            // Arrange
            var expense = Expense.Create(100m, ExpenseCategory.Food, "Dinner", DateTime.UtcNow, TableId);
            var command = new UpdateExpenseCommand(1, 2.50m, ExpenseCategory.Food, "Coffee", Quantity: 4);

            _mockExpenseReader.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expense);
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockExpenseWriter.Verify(x => x.UpdateAsync(It.Is<Expense>(e =>
                e.UnitPrice.Amount == 2.50m &&
                e.Quantity == 4)), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Handle_WithInvalidUnitPrice_ThrowsValidationException(decimal unitPrice)
        {
            // Arrange
            var command = new UpdateExpenseCommand(1, unitPrice, ExpenseCategory.Food, "Dinner");

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Unit price*");

            _mockExpenseWriter.Verify(
                x => x.UpdateAsync(It.IsAny<Expense>()),
                Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        public async Task Handle_WithInvalidQuantity_ThrowsValidationException(int quantity)
        {
            // Arrange
            var command = new UpdateExpenseCommand(1, 50m, ExpenseCategory.Food, "Dinner", Quantity: quantity);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Quantity*");

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
        public async Task Handle_WhenCurrentUserNotMemberOfTable_ThrowsNotFoundException()
        {
            // Arrange
            var expense = Expense.Create(100m, ExpenseCategory.Food, "Dinner", DateTime.UtcNow, TableId);
            var command = new UpdateExpenseCommand(1, 150m, ExpenseCategory.Transport, "Taxi");

            _mockExpenseReader.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expense);
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            _mockExpenseWriter.Verify(
                x => x.UpdateAsync(It.IsAny<Expense>()),
                Times.Never);
        }
    
        private Expense SetUpMilk(decimal discount)
        {
            var expense = Expense.Create(1.50m, ExpenseCategory.Food, "Milk", DateTime.UtcNow, TableId, quantity: 2, discount: discount);
            _mockExpenseReader.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(expense);
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            return expense;
        }

        [Fact]
        public async Task Handle_WithDiscount_ReplacesTheStoredDiscount()
        {
            var expense = SetUpMilk(0.50m);

            await _handler.Handle(new UpdateExpenseCommand(1, 1.50m, ExpenseCategory.Food, "Milk", Quantity: 2, Discount: 1.25m), CancellationToken.None);

            expense.Discount.Should().Be(1.25m);
            _mockExpenseWriter.Verify(x => x.UpdateAsync(expense), Times.Once);
        }

        [Fact]
        public async Task Handle_WithZeroDiscount_ClearsTheDiscount()
        {
            var expense = SetUpMilk(0.50m);

            await _handler.Handle(new UpdateExpenseCommand(1, 1.50m, ExpenseCategory.Food, "Milk", Quantity: 2, Discount: 0m), CancellationToken.None);

            expense.Discount.Should().Be(0m);
        }

        [Fact]
        public async Task Handle_WithoutDiscount_KeepsTheStoredDiscount()
        {
            var expense = SetUpMilk(0.50m);

            await _handler.Handle(new UpdateExpenseCommand(1, 1.50m, ExpenseCategory.Food, "Milk", Quantity: 2), CancellationToken.None);

            expense.Discount.Should().Be(0.50m);
        }

        [Fact]
        public async Task Handle_LoweringThePriceBelowTheKeptDiscount_ThrowsAndDoesNotSave()
        {
            SetUpMilk(0.50m);

            var act = async () => await _handler.Handle(new UpdateExpenseCommand(1, 0.20m, ExpenseCategory.Food, "Milk", Quantity: 2), CancellationToken.None);

            await act.Should().ThrowAsync<ExpenseTracker.Domain.Exceptions.DomainException>()
                .WithMessage("Discount can't be more than the item total (£0.40).");
            _mockExpenseWriter.Verify(x => x.UpdateAsync(It.IsAny<Expense>()), Times.Never);
        }

        [Theory]
        [InlineData(-0.50, "Discount can't be negative.")]
        [InlineData(3.50, "Discount can't be more than the item total (£3.00).")]
        public async Task Handle_WithInvalidDiscount_ThrowsValidationException(decimal discount, string message)
        {
            SetUpMilk(0.50m);

            var act = async () => await _handler.Handle(new UpdateExpenseCommand(1, 1.50m, ExpenseCategory.Food, "Milk", Quantity: 2, Discount: discount), CancellationToken.None);

            (await act.Should().ThrowAsync<ValidationException>())
                .Which.Errors.Should().ContainSingle(e => e.ErrorMessage == message);
            _mockExpenseWriter.Verify(x => x.UpdateAsync(It.IsAny<Expense>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithDiscountEqualToTheLineTotal_IsAcceptedAndSaved()
        {
            var expense = SetUpMilk(0m);

            await _handler.Handle(new UpdateExpenseCommand(1, 1.50m, ExpenseCategory.Food, "Milk", Quantity: 2, Discount: 3.00m), CancellationToken.None);

            expense.Discount.Should().Be(3.00m);
            _mockExpenseWriter.Verify(x => x.UpdateAsync(expense), Times.Once);
        }
    }
}
