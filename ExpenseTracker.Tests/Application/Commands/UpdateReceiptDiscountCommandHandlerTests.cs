using ExpenseTracker.Application.Commands.UpdateReceiptDiscount;
using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class UpdateReceiptDiscountCommandHandlerTests
    {
        private readonly Mock<IExpenseReader> _mockExpenseReader = new();
        private readonly Mock<IExpenseTableReader> _mockExpenseTableReader = new();
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider = new();
        private readonly User _currentUser;
        private readonly UpdateReceiptDiscountCommandHandler _handler;
        private const int TableId = 1;
        private const int ReceiptId = 7;

        public UpdateReceiptDiscountCommandHandlerTests()
        {
            _currentUser = User.Create("auth0|test-user");
            _currentUser.Id = 1;
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_currentUser);
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _handler = new UpdateReceiptDiscountCommandHandler(_mockExpenseReader.Object, _mockExpenseTableReader.Object, _mockCurrentUserProvider.Object, new UpdateReceiptDiscountValidator());
        }

        // The S3 receipt: Milk 2 x 1.50 (-0.50), Bread 2.00, Coffee 4.00 (-1.00), receipt discount 2.00. £7.50 left after item discounts.
        private List<Expense> SetUpS3Receipt()
        {
            var receipt = Receipt.Create(DateTime.UtcNow, null, 2m);
            var lines = new List<Expense>
            {
                Expense.Create(1.50m, ExpenseCategory.Food, "Milk", DateTime.UtcNow, TableId, receiptId: ReceiptId, quantity: 2, discount: 0.50m),
                Expense.Create(2.00m, ExpenseCategory.Food, "Bread", DateTime.UtcNow, TableId, receiptId: ReceiptId),
                Expense.Create(4.00m, ExpenseCategory.Food, "Coffee", DateTime.UtcNow, TableId, receiptId: ReceiptId, discount: 1.00m),
            };
            foreach (var line in lines)
                typeof(Expense).GetProperty(nameof(Expense.Receipt))!.SetValue(line, receipt);
            _mockExpenseReader.Setup(x => x.GetByReceiptIdAsync(ReceiptId, TableId, It.IsAny<CancellationToken>())).ReturnsAsync(lines);
            return lines;
        }

        [Theory]
        [InlineData(1.00)]
        [InlineData(0)]
        [InlineData(7.50)]
        public async Task Handle_SetsTheReceiptDiscount_AndEveryLineShowsIt(decimal value)
        {
            var lines = SetUpS3Receipt();

            await _handler.Handle(new UpdateReceiptDiscountCommand(value, TableId, ReceiptId), CancellationToken.None);

            lines.Select(l => l.ToDto(_currentUser.Id).ReceiptDiscount).Should().AllBeEquivalentTo(value);
            _mockExpenseReader.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_AboveWhatIsLeftAfterItemDiscounts_ThrowsAndDoesNotSave()
        {
            var lines = SetUpS3Receipt();

            var act = async () => await _handler.Handle(new UpdateReceiptDiscountCommand(8m, TableId, ReceiptId), CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>().WithMessage("Receipt discount can't be more than the receipt total (£7.50).");
            lines[0].Receipt!.Discount.Should().Be(2m);
            _mockExpenseReader.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithNegativeDiscount_ThrowsValidationException()
        {
            SetUpS3Receipt();

            var act = async () => await _handler.Handle(new UpdateReceiptDiscountCommand(-1m, TableId, ReceiptId), CancellationToken.None);

            (await act.Should().ThrowAsync<ValidationException>())
                .Which.Errors.Should().ContainSingle(e => e.ErrorMessage == "Receipt discount can't be negative.");
            _mockExpenseReader.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenNotAMember_ThrowsNotFoundException()
        {
            SetUpS3Receipt();
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, _currentUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var act = async () => await _handler.Handle(new UpdateReceiptDiscountCommand(1m, TableId, ReceiptId), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _mockExpenseReader.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenTheReceiptHasNoLinesInTheSpace_ThrowsNotFoundException()
        {
            _mockExpenseReader.Setup(x => x.GetByReceiptIdAsync(ReceiptId, TableId, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Expense>());

            var act = async () => await _handler.Handle(new UpdateReceiptDiscountCommand(1m, TableId, ReceiptId), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
