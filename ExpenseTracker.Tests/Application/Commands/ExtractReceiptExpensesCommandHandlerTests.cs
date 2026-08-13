using ExpenseTracker.Application.Commands.ExtractReceiptExpenses;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class ExtractReceiptExpensesCommandHandlerTests
    {
        private const int TableId = 1;
        private static readonly byte[] ImageBytes = { 1, 2, 3 };
        private static readonly Uri SasUri = new("https://storage.example/receipt-temp/abc.jpg?sig=x");

        private readonly Mock<IExpenseTableReader> _mockExpenseTableReader = new();
        private readonly Mock<IReceiptImageStore> _mockReceiptImageStore = new();
        private readonly Mock<IReceiptExtractionRequestPublisher> _mockPublisher = new();
        private readonly Mock<IReceiptExtractionJobWriter> _mockJobWriter = new();
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider = new();
        private readonly ExtractReceiptExpensesCommandHandler _handler;

        public ExtractReceiptExpensesCommandHandlerTests()
        {
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(User.Create("auth0|test-user"));
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mockReceiptImageStore.Setup(x => x.UploadTempAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("abc.jpg");
            _mockReceiptImageStore.Setup(x => x.GenerateTempReadSasUri(It.IsAny<string>()))
                .Returns(SasUri);

            _handler = new ExtractReceiptExpensesCommandHandler(
                _mockExpenseTableReader.Object,
                _mockReceiptImageStore.Object,
                _mockPublisher.Object,
                _mockJobWriter.Object,
                _mockCurrentUserProvider.Object);
        }

        private static ExtractReceiptExpensesCommand Command() => new(TableId, ImageBytes, "image/jpeg");

        [Fact]
        public async Task Handle_UploadsToTempContainerAndPublishesSasUrl()
        {
            var result = await _handler.Handle(Command(), CancellationToken.None);

            result.JobId.Should().NotBeEmpty();
            result.TempReference.Should().Be("abc.jpg");

            _mockReceiptImageStore.Verify(x => x.UploadTempAsync(ImageBytes, "image/jpeg", It.IsAny<CancellationToken>()), Times.Once);
            _mockPublisher.Verify(x => x.PublishAsync(result.JobId, SasUri, "image/jpeg", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_RecordsJobOwnershipBeforePublishing()
        {
            ReceiptExtractionJob? captured = null;
            _mockJobWriter.Setup(x => x.AddAsync(It.IsAny<ReceiptExtractionJob>(), It.IsAny<CancellationToken>()))
                .Callback<ReceiptExtractionJob, CancellationToken>((j, _) => captured = j)
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(Command(), CancellationToken.None);

            captured.Should().NotBeNull();
            captured!.Id.Should().Be(result.JobId);
            captured.ExpenseTableId.Should().Be(TableId);
        }

        [Fact]
        public async Task Handle_WhenCurrentUserNotMemberOfTable_ThrowsNotFoundException()
        {
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            Func<Task> act = async () => await _handler.Handle(Command(), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenCurrentUserNotMemberOfTable_DoesNotUploadOrPublish()
        {
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Command(), CancellationToken.None));

            _mockReceiptImageStore.Verify(x => x.UploadTempAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockPublisher.Verify(x => x.PublishAsync(It.IsAny<Guid>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockJobWriter.Verify(x => x.AddAsync(It.IsAny<ReceiptExtractionJob>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
