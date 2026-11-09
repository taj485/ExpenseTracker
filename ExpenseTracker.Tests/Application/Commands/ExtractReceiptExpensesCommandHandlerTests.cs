using ExpenseTracker.Application.Commands.ExtractReceiptExpenses;
using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class ExtractReceiptExpensesCommandHandlerTests
    {
        private readonly Mock<IReceiptExtractionService> _mockReceiptExtractionService;
        private readonly Mock<IMerchantResolver> _mockMerchantResolver;
        private readonly ExtractReceiptExpensesCommandHandler _handler;

        public ExtractReceiptExpensesCommandHandlerTests()
        {
            _mockReceiptExtractionService = new Mock<IReceiptExtractionService>();
            _mockMerchantResolver = new Mock<IMerchantResolver>();
            _handler = new ExtractReceiptExpensesCommandHandler(_mockReceiptExtractionService.Object, _mockMerchantResolver.Object);
        }

        [Fact]
        public async Task Handle_MapsExtractedItemsToDtos()
        {
            var items = new List<ExtractedReceiptItem>
            {
                new(4.5m, ExpenseCategory.Food, "Coffee", new DateOnly(2026, 7, 10), 2),
                new(12m, ExpenseCategory.Transport, "Taxi", new DateOnly(2026, 7, 10), 1),
            };
            _mockReceiptExtractionService
                .Setup(x => x.ExtractAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExtractedReceipt(items));

            var result = await _handler.Handle(new ExtractReceiptExpensesCommand(new byte[] { 1, 2, 3 }, "image/jpeg"), CancellationToken.None);

            result.Should().HaveCount(2);
            result[0].Should().BeEquivalentTo(new ExtractedExpenseDto
            {
                UnitPrice = 4.5m,
                Category = "Food",
                Description = "Coffee",
                Date = new DateOnly(2026, 7, 10),
                Quantity = 2,
            });
        }

        [Fact]
        public async Task Handle_WithWeighedItem_ReturnsWeightAndUnit()
        {
            var items = new List<ExtractedReceiptItem>
            {
                new(5m, ExpenseCategory.Food, "Chicken", new DateOnly(2026, 7, 10), 0.612m, null, Unit: QuantityUnit.Kg),
            };
            _mockReceiptExtractionService
                .Setup(x => x.ExtractAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExtractedReceipt(items));

            var result = await _handler.Handle(new ExtractReceiptExpensesCommand(new byte[] { 1 }, "image/jpeg"), CancellationToken.None);

            result.Single().Quantity.Should().Be(0.612m);
            result.Single().Unit.Should().Be("Kg");
            result.Single().UnitPrice.Should().Be(5m);
        }

        [Fact]
        public async Task Handle_WithNoExtractedItems_ReturnsEmptyList()
        {
            _mockReceiptExtractionService
                .Setup(x => x.ExtractAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ExtractedReceipt.Empty);

            var result = await _handler.Handle(new ExtractReceiptExpensesCommand(new byte[] { 1 }, "image/jpeg"), CancellationToken.None);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_MapsItemAndReceiptDiscounts()
        {
            var items = new List<ExtractedReceiptItem>
            {
                new(1.5m, ExpenseCategory.Food, "Milk", new DateOnly(2026, 11, 5), 2, "Tesco", 0.5m),
                new(2m, ExpenseCategory.Food, "Bread", new DateOnly(2026, 11, 5), 1, "Tesco"),
            };
            _mockReceiptExtractionService
                .Setup(x => x.ExtractAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExtractedReceipt(items, 1m));

            var result = await _handler.Handle(new ExtractReceiptExpensesCommand(new byte[] { 1 }, "image/jpeg"), CancellationToken.None);

            result.Select(r => r.Discount).Should().Equal(0.5m, 0m);
            result.Should().OnlyContain(r => r.ReceiptDiscount == 1m);
        }
    }
}
