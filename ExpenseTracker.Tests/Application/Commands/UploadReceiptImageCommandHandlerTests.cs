using ExpenseTracker.Application.Commands.UploadReceiptImage;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class UploadReceiptImageCommandHandlerTests
    {
        private readonly Mock<IExpenseTableReader> _mockExpenseTableReader;
        private readonly Mock<IReceiptImageStore> _mockReceiptImageStore;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly UploadReceiptImageCommandHandler _handler;
        private const int TableId = 1;

        public UploadReceiptImageCommandHandlerTests()
        {
            _mockExpenseTableReader = new Mock<IExpenseTableReader>();
            _mockReceiptImageStore = new Mock<IReceiptImageStore>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _handler = new UploadReceiptImageCommandHandler(_mockExpenseTableReader.Object, _mockReceiptImageStore.Object, _mockCurrentUserProvider.Object);
        }

        [Fact]
        public async Task Handle_UploadsImageAndReturnsBlobReference()
        {
            var bytes = new byte[] { 1, 2, 3 };
            _mockReceiptImageStore.Setup(x => x.UploadAsync(bytes, "image/jpeg", It.IsAny<CancellationToken>()))
                .ReturnsAsync("generated-blob-name.jpg");

            var result = await _handler.Handle(new UploadReceiptImageCommand(TableId, bytes, "image/jpeg"), CancellationToken.None);

            result.Should().Be("generated-blob-name.jpg");
        }

        [Fact]
        public async Task Handle_WhenCurrentUserNotMemberOfTable_ThrowsNotFoundException()
        {
            _mockExpenseTableReader.Setup(x => x.IsMemberAsync(TableId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            Func<Task> act = async () => await _handler.Handle(new UploadReceiptImageCommand(TableId, new byte[] { 1 }, "image/jpeg"), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _mockReceiptImageStore.Verify(x => x.UploadAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
