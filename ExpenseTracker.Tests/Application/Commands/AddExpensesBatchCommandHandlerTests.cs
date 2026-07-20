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
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly AddExpensesBatchCommandHandler _handler;

        public AddExpensesBatchCommandHandlerTests()
        {
            _mockExpenseWriter = new Mock<IExpenseWriter>();
            _mockReceiptWriter = new Mock<IReceiptWriter>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);

            var validator = new AddExpenseValidator();
            _handler = new AddExpensesBatchCommandHandler(_mockExpenseWriter.Object, _mockReceiptWriter.Object, validator, _mockCurrentUserProvider.Object);

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
            var command = new AddExpensesBatchCommand(new List<AddExpenseCommand>
            {
                new(10m, ExpenseCategory.Food, "Coffee", DateTime.UtcNow),
                new(20m, ExpenseCategory.Transport, "Taxi", DateTime.UtcNow),
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
            var command = new AddExpensesBatchCommand(new List<AddExpenseCommand>
            {
                new(10m, ExpenseCategory.Food, "Coffee", DateTime.UtcNow),
                new(20m, ExpenseCategory.Transport, "Taxi", DateTime.UtcNow),
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
        public async Task Handle_WithOneInvalidItem_AddsValidOnesAndReportsFailedIndex()
        {
            var command = new AddExpensesBatchCommand(new List<AddExpenseCommand>
            {
                new(10m, ExpenseCategory.Food, "Coffee", DateTime.UtcNow),
                new(0m, ExpenseCategory.Transport, "Bad amount", DateTime.UtcNow),
                new(30m, ExpenseCategory.Health, "Pharmacy", DateTime.UtcNow),
            });

            var result = await _handler.Handle(command, CancellationToken.None);

            result.AddedIds.Should().Equal(1, 2);
            result.Errors.Should().ContainSingle();
            result.Errors[0].Index.Should().Be(1);
            result.Errors[0].Errors.Should().Contain(m => m.Contains("Amount"));
            _mockExpenseWriter.Verify(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _mockReceiptWriter.Verify(x => x.AddAsync(It.IsAny<Receipt>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithAllInvalidItems_AddsNoneAndReportsEveryIndex()
        {
            var command = new AddExpensesBatchCommand(new List<AddExpenseCommand>
            {
                new(0m, ExpenseCategory.Food, "", DateTime.UtcNow),
                new(-5m, ExpenseCategory.Transport, "", DateTime.UtcNow),
            });

            var result = await _handler.Handle(command, CancellationToken.None);

            result.AddedIds.Should().BeEmpty();
            result.Errors.Select(e => e.Index).Should().Equal(0, 1);
            _mockExpenseWriter.Verify(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockReceiptWriter.Verify(x => x.AddAsync(It.IsAny<Receipt>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
