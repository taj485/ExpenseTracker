using ExpenseTracker.Application.Commands.AddExpense;
using ExpenseTracker.Application.Commands.AddExpensesBatch;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class AddExpensesBatchCommandHandlerTests
    {
        private readonly Mock<IExpenseWriter> _mockExpenseWriter;
        private readonly Mock<IReceiptWriter> _mockReceiptWriter;
        private readonly Mock<IExpenseTableReader> _mockExpenseTableReader;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly Mock<IMerchantResolver> _mockMerchantResolver;
        private readonly User _currentUser;
        private readonly AddExpensesBatchCommandHandler _handler;
        private const int TableId = 1;

        public AddExpensesBatchCommandHandlerTests()
        {
            _mockExpenseWriter = new Mock<IExpenseWriter>();
            _mockReceiptWriter = new Mock<IReceiptWriter>();
            _mockExpenseTableReader = new Mock<IExpenseTableReader>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockMerchantResolver = new Mock<IMerchantResolver>();

            var validator = new AddExpenseValidator();
            _handler = new AddExpensesBatchCommandHandler(_mockExpenseWriter.Object, _mockReceiptWriter.Object, _mockExpenseTableReader.Object, validator, _mockCurrentUserProvider.Object, _mockMerchantResolver.Object);

            var nextId = 1;
            _mockExpenseWriter.Setup(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => nextId++);

            var nextReceiptId = 1;
            _mockReceiptWriter.Setup(x => x.AddAsync(It.IsAny<Receipt>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => nextReceiptId++);
        }

        [Fact]
        public async Task Handle_WithAllValidItems_AddsAllAndReturnsNoErrors()
        {
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 10m, ExpenseCategory.Food, "Coffee", DateTime.UtcNow),
                new(TableId, 20m, ExpenseCategory.Transport, "Taxi", DateTime.UtcNow),
            });

            var result = await _handler.Handle(command, CancellationToken.None);

            result.AddedIds.Should().Equal(1, 2);
            result.Errors.Should().BeEmpty();
            _mockExpenseWriter.Verify(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _mockReceiptWriter.Verify(x => x.AddAsync(It.IsAny<Receipt>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithAllValidItems_AllShareSameReceiptId()
        {
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 10m, ExpenseCategory.Food, "Coffee", DateTime.UtcNow),
                new(TableId, 20m, ExpenseCategory.Transport, "Taxi", DateTime.UtcNow),
            });

            var capturedReceiptIds = new List<int?>();
            _mockExpenseWriter.Setup(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()))
                .Callback<Expense, CancellationToken>((e, _) => capturedReceiptIds.Add(e.ReceiptId))
                .ReturnsAsync(() => capturedReceiptIds.Count);

            await _handler.Handle(command, CancellationToken.None);

            capturedReceiptIds.Should().HaveCount(2);
            capturedReceiptIds[0].Should().NotBeNull();
            capturedReceiptIds.Distinct().Should().ContainSingle();
        }

        [Fact]
        public async Task Handle_RecordsCurrentUserAsCreatorOfEveryItem()
        {
            _currentUser.Id = 42;
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 10m, ExpenseCategory.Food, "Coffee", DateTime.UtcNow),
                new(TableId, 20m, ExpenseCategory.Transport, "Taxi", DateTime.UtcNow),
            });

            var capturedCreators = new List<int?>();
            _mockExpenseWriter.Setup(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()))
                .Callback<Expense, CancellationToken>((e, _) => capturedCreators.Add(e.CreatedByUserId))
                .ReturnsAsync(() => capturedCreators.Count);

            await _handler.Handle(command, CancellationToken.None);

            capturedCreators.Should().Equal(42, 42);
        }

        [Fact]
        public async Task Handle_WithOneInvalidItem_AddsValidOnesAndReportsFailedIndex()
        {
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 10m, ExpenseCategory.Food, "Coffee", DateTime.UtcNow),
                new(TableId, 0m, ExpenseCategory.Transport, "Bad amount", DateTime.UtcNow),
                new(TableId, 30m, ExpenseCategory.Health, "Pharmacy", DateTime.UtcNow),
            });

            var result = await _handler.Handle(command, CancellationToken.None);

            result.AddedIds.Should().Equal(1, 2);
            result.Errors.Should().ContainSingle();
            result.Errors[0].Index.Should().Be(1);
            result.Errors[0].Errors.Should().Contain(m => m.Contains("Unit price"));
            _mockExpenseWriter.Verify(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _mockReceiptWriter.Verify(x => x.AddAsync(It.IsAny<Receipt>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithAllInvalidItems_AddsNoneAndReportsEveryIndex()
        {
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 0m, ExpenseCategory.Food, "", DateTime.UtcNow),
                new(TableId, -5m, ExpenseCategory.Transport, "", DateTime.UtcNow),
            });

            var result = await _handler.Handle(command, CancellationToken.None);

            result.AddedIds.Should().BeEmpty();
            result.Errors.Select(e => e.Index).Should().Equal(0, 1);
            _mockExpenseWriter.Verify(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockReceiptWriter.Verify(x => x.AddAsync(It.IsAny<Receipt>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithReceiptImageReference_SetsItOnCreatedReceipt()
        {
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 10m, ExpenseCategory.Food, "Coffee", DateTime.UtcNow),
            }, ReceiptImageReference: "abc123.jpg");

            Receipt? capturedReceipt = null;
            _mockReceiptWriter.Setup(x => x.AddAsync(It.IsAny<Receipt>(), It.IsAny<CancellationToken>()))
                .Callback<Receipt, CancellationToken>((r, _) => capturedReceipt = r)
                .ReturnsAsync(1);

            await _handler.Handle(command, CancellationToken.None);

            capturedReceipt.Should().NotBeNull();
            capturedReceipt!.ImageReference.Should().Be("abc123.jpg");
        }

        [Fact]
        public async Task Handle_WhenCurrentUserNotMemberOfTable_ThrowsNotFoundException()
        {
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 10m, ExpenseCategory.Food, "Coffee", DateTime.UtcNow),
            });
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _mockExpenseWriter.Verify(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithDiscounts_SavesItemAndReceiptDiscounts()
        {
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 1.50m, ExpenseCategory.Food, "Milk", DateTime.UtcNow, Quantity: 2, Discount: 0.50m),
                new(TableId, 2m, ExpenseCategory.Food, "Bread", DateTime.UtcNow),
            }, ReceiptDiscount: 1m);
            Receipt? capturedReceipt = null;
            var capturedExpenses = new List<Expense>();
            _mockReceiptWriter.Setup(x => x.AddAsync(It.IsAny<Receipt>(), It.IsAny<CancellationToken>()))
                .Callback<Receipt, CancellationToken>((r, _) => capturedReceipt = r)
                .ReturnsAsync(1);
            _mockExpenseWriter.Setup(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()))
                .Callback<Expense, CancellationToken>((e, _) => capturedExpenses.Add(e))
                .ReturnsAsync(1);

            await _handler.Handle(command, CancellationToken.None);

            capturedReceipt!.Discount.Should().Be(1m);
            capturedExpenses.Select(e => e.Discount).Should().Equal(0.50m, 0m);
        }

        [Fact]
        public async Task Handle_WithReceiptDiscountAboveTotalAfterItemDiscounts_ThrowsAndAddsNothing()
        {
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 1.50m, ExpenseCategory.Food, "Milk", DateTime.UtcNow, Quantity: 2, Discount: 0.50m),
                new(TableId, 2m, ExpenseCategory.Food, "Bread", DateTime.UtcNow),
            }, ReceiptDiscount: 5m);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<ExpenseTracker.Domain.Exceptions.DomainException>()
                .WithMessage("Receipt discount can't be more than the receipt total (£4.50).");
            _mockExpenseWriter.Verify(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockReceiptWriter.Verify(x => x.AddAsync(It.IsAny<Receipt>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(3.50, "Discount can't be more than the item total (£3.00).")]
        [InlineData(-0.50, "Discount can't be negative.")]
        public async Task Handle_WithInvalidItemDiscount_ReturnsItemError(double discount, string message)
        {
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 1.50m, ExpenseCategory.Food, "Milk", DateTime.UtcNow, Quantity: 2, Discount: (decimal)discount),
            });

            var result = await _handler.Handle(command, CancellationToken.None);

            result.AddedIds.Should().BeEmpty();
            result.Errors.Should().ContainSingle().Which.Errors.Should().Equal(message);
        }

        [Fact]
        public async Task Handle_WithNegativeReceiptDiscount_ThrowsAndAddsNothing()
        {
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 2m, ExpenseCategory.Food, "Bread", DateTime.UtcNow),
            }, ReceiptDiscount: -1m);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<ExpenseTracker.Domain.Exceptions.DomainException>()
                .WithMessage("Receipt discount can't be negative.");
            _mockExpenseWriter.Verify(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockReceiptWriter.Verify(x => x.AddAsync(It.IsAny<Receipt>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithReceiptDiscountEqualToTotalAfterItemDiscounts_Saves()
        {
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 1.50m, ExpenseCategory.Food, "Milk", DateTime.UtcNow, Quantity: 2, Discount: 0.50m),
                new(TableId, 2m, ExpenseCategory.Food, "Bread", DateTime.UtcNow),
            }, ReceiptDiscount: 4.50m);
            _mockExpenseWriter.Setup(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.AddedIds.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_WithItemDiscountEqualToLineTotal_SavesTheFreeItem()
        {
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 1.50m, ExpenseCategory.Food, "Free milk", DateTime.UtcNow, Quantity: 2, Discount: 3m),
            });
            _mockExpenseWriter.Setup(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Errors.Should().BeEmpty();
            result.AddedIds.Should().ContainSingle();
        }

        [Fact]
        public async Task Handle_WithOneItemAndReceiptDiscount_MovesTheReceiptDiscountOntoTheItem()
        {
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 1.50m, ExpenseCategory.Food, "Milk", DateTime.UtcNow, Quantity: 2, Discount: 0.50m),
            }, ReceiptDiscount: 1m);
            Receipt? capturedReceipt = null;
            Expense? capturedExpense = null;
            _mockReceiptWriter.Setup(x => x.AddAsync(It.IsAny<Receipt>(), It.IsAny<CancellationToken>()))
                .Callback<Receipt, CancellationToken>((r, _) => capturedReceipt = r)
                .ReturnsAsync(1);
            _mockExpenseWriter.Setup(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()))
                .Callback<Expense, CancellationToken>((e, _) => capturedExpense = e)
                .ReturnsAsync(1);

            await _handler.Handle(command, CancellationToken.None);

            capturedReceipt!.Discount.Should().Be(0m);
            capturedExpense!.Discount.Should().Be(1.50m);
        }

        [Fact]
        public async Task Handle_WithOneItemWithInvalidDiscountAndReceiptDiscount_StillReturnsTheItemError()
        {
            // Validated before the receipt discount moves, so a negative item discount isn't hidden by it.
            var command = new AddExpensesBatchCommand(TableId, new List<AddExpenseCommand>
            {
                new(TableId, 2m, ExpenseCategory.Food, "Bread", DateTime.UtcNow, Discount: -0.50m),
            }, ReceiptDiscount: 1m);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.AddedIds.Should().BeEmpty();
            result.Errors.Should().ContainSingle().Which.Errors.Should().Equal("Discount can't be negative.");
        }
    }
}
