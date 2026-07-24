using ExpenseTracker.Application.Queries.GetReceiptImage;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Tests.Application.Queries
{
    public class GetReceiptImageQueryHandlerTests
    {
        private readonly Mock<IExpenseReader> _mockExpenseReader;
        private readonly Mock<IExpenseTableReader> _mockExpenseTableReader;
        private readonly Mock<IReceiptReader> _mockReceiptReader;
        private readonly Mock<IReceiptImageStore> _mockReceiptImageStore;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly GetReceiptImageQueryHandler _handler;
        private const int TableId = 1;
        private const int ReceiptId = 1;

        public GetReceiptImageQueryHandlerTests()
        {
            _mockExpenseReader = new Mock<IExpenseReader>();
            _mockExpenseTableReader = new Mock<IExpenseTableReader>();
            _mockReceiptReader = new Mock<IReceiptReader>();
            _mockReceiptImageStore = new Mock<IReceiptImageStore>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mockExpenseReader.Setup(x => x.GetByReceiptIdAsync(ReceiptId, TableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Expense> { Expense.Create(10m, ExpenseCategory.Food, "Coffee", DateTime.UtcNow, TableId, receiptId: ReceiptId) });

            _handler = new GetReceiptImageQueryHandler(
                _mockExpenseReader.Object,
                _mockExpenseTableReader.Object,
                _mockReceiptReader.Object,
                _mockReceiptImageStore.Object,
                _mockCurrentUserProvider.Object);
        }

        private static readonly GetReceiptImageQuery Query = new() { ReceiptId = ReceiptId, ExpenseTableId = TableId };

        [Fact]
        public async Task Handle_ReturnsImage_WhenReceiptAndBlobExist()
        {
            var receipt = Receipt.Create(DateTime.UtcNow, "blob-name.jpg");
            _mockReceiptReader.Setup(x => x.GetByIdAsync(ReceiptId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(receipt);
            _mockReceiptImageStore.Setup(x => x.DownloadAsync("blob-name.jpg", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReceiptImage { Content = new byte[] { 1, 2, 3 }, ContentType = "image/jpeg" });

            var result = await _handler.Handle(Query, CancellationToken.None);

            result.Content.Should().Equal(1, 2, 3);
            result.ContentType.Should().Be("image/jpeg");
            result.FileName.Should().Be($"receipt-{ReceiptId}.jpg");
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenCurrentUserNotAMember()
        {
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            Func<Task> act = async () => await _handler.Handle(Query, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenReceiptNotLinkedToTable()
        {
            _mockExpenseReader.Setup(x => x.GetByReceiptIdAsync(ReceiptId, TableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Expense>());

            Func<Task> act = async () => await _handler.Handle(Query, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenReceiptHasNoImageReference()
        {
            _mockReceiptReader.Setup(x => x.GetByIdAsync(ReceiptId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Receipt.Create(DateTime.UtcNow));

            Func<Task> act = async () => await _handler.Handle(Query, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _mockReceiptImageStore.Verify(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenBlobMissing()
        {
            _mockReceiptReader.Setup(x => x.GetByIdAsync(ReceiptId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Receipt.Create(DateTime.UtcNow, "missing-blob.jpg"));
            _mockReceiptImageStore.Setup(x => x.DownloadAsync("missing-blob.jpg", It.IsAny<CancellationToken>()))
                .ReturnsAsync((ReceiptImage?)null);

            Func<Task> act = async () => await _handler.Handle(Query, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
