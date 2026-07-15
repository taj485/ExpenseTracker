using ExpenseTracker.Application.Commands.ExtractReceiptExpenses;
using ExpenseTracker.Application.DTO;
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
        private readonly ExtractReceiptExpensesCommandHandler _handler;

        public ExtractReceiptExpensesCommandHandlerTests()
        {
            _mockReceiptExtractionService = new Mock<IReceiptExtractionService>();
            _handler = new ExtractReceiptExpensesCommandHandler(_mockReceiptExtractionService.Object);
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
                .ReturnsAsync(items);

            var result = await _handler.Handle(new ExtractReceiptExpensesCommand(new byte[] { 1, 2, 3 }, "image/jpeg"), CancellationToken.None);

            result.Should().HaveCount(2);
            result[0].Should().BeEquivalentTo(new ExtractedExpenseDto
            {
                Amount = 4.5m,
                Category = "Food",
                Description = "Coffee",
                Date = new DateOnly(2026, 7, 10),
                Quantity = 2,
            });
        }

        [Fact]
        public async Task Handle_WithNoExtractedItems_ReturnsEmptyList()
        {
            _mockReceiptExtractionService
                .Setup(x => x.ExtractAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ExtractedReceiptItem>());

            var result = await _handler.Handle(new ExtractReceiptExpensesCommand(new byte[] { 1 }, "image/jpeg"), CancellationToken.None);

            result.Should().BeEmpty();
        }
    }
}
