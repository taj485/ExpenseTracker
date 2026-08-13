using ExpenseTracker.Application.Queries.GetReceiptExtractionStatus;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Tests.Application.Queries
{
    public class GetReceiptExtractionStatusQueryHandlerTests
    {
        private static readonly Guid JobId = Guid.NewGuid();

        private readonly Mock<IReceiptExtractionJobReader> _mockJobReader = new();
        private readonly Mock<IReceiptExtractionResultReader> _mockResultReader = new();
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider = new();
        private readonly User _currentUser;
        private readonly GetReceiptExtractionStatusQueryHandler _handler;

        public GetReceiptExtractionStatusQueryHandlerTests()
        {
            _currentUser = User.Create("auth0|test-user");
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);

            // User.Id defaults to 0 for an unsaved entity, which is what the owning job records too.
            _mockJobReader.Setup(x => x.GetByIdAsync(JobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ReceiptExtractionJob.Create(JobId, _currentUser.Id, 1));

            _handler = new GetReceiptExtractionStatusQueryHandler(
                _mockJobReader.Object,
                _mockResultReader.Object,
                _mockCurrentUserProvider.Object);
        }

        private void AnalyserReturns(ReceiptExtractionResult? result) =>
            _mockResultReader.Setup(x => x.GetAsync(JobId, It.IsAny<CancellationToken>())).ReturnsAsync(result);

        [Fact]
        public async Task Handle_WhenCompleted_MapsItemsToDtos()
        {
            AnalyserReturns(new ReceiptExtractionResult(JobId, ReceiptExtractionStatus.Completed, new List<ExtractedReceiptItem>
            {
                new(4.5m, ExpenseCategory.Food, "Coffee", new DateOnly(2026, 7, 10), 2, "Tesco"),
            }, null));

            var result = await _handler.Handle(new GetReceiptExtractionStatusQuery(JobId), CancellationToken.None);

            result.Status.Should().Be("Completed");
            result.Items.Should().ContainSingle();
            result.Items[0].Amount.Should().Be(4.5m);
            result.Items[0].Category.Should().Be("Food");
            result.Items[0].Description.Should().Be("Coffee");
            result.Items[0].Quantity.Should().Be(2);
            result.Items[0].Merchant.Should().Be("Tesco");
        }

        [Fact]
        public async Task Handle_WhenProcessing_ReturnsNoItems()
        {
            AnalyserReturns(new ReceiptExtractionResult(JobId, ReceiptExtractionStatus.Processing, Array.Empty<ExtractedReceiptItem>(), null));

            var result = await _handler.Handle(new GetReceiptExtractionStatusQuery(JobId), CancellationToken.None);

            result.Status.Should().Be("Processing");
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WhenAnalyserHasNotSeenJobYet_ReportsPending()
        {
            // The request is still in flight on the topic — not an error.
            AnalyserReturns(null);

            var result = await _handler.Handle(new GetReceiptExtractionStatusQuery(JobId), CancellationToken.None);

            result.Status.Should().Be("Pending");
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WhenFailed_SurfacesTheError()
        {
            AnalyserReturns(new ReceiptExtractionResult(JobId, ReceiptExtractionStatus.Failed, Array.Empty<ExtractedReceiptItem>(), "Receipt image link expired."));

            var result = await _handler.Handle(new GetReceiptExtractionStatusQuery(JobId), CancellationToken.None);

            result.Status.Should().Be("Failed");
            result.Error.Should().Be("Receipt image link expired.");
        }

        [Fact]
        public async Task Handle_WhenJobUnknown_ThrowsNotFoundException()
        {
            _mockJobReader.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ReceiptExtractionJob?)null);

            Func<Task> act = async () => await _handler.Handle(new GetReceiptExtractionStatusQuery(JobId), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenJobBelongsToAnotherUser_ThrowsNotFoundExceptionAndNeverCallsAnalyser()
        {
            _mockJobReader.Setup(x => x.GetByIdAsync(JobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ReceiptExtractionJob.Create(JobId, userId: 999, expenseTableId: 1));

            Func<Task> act = async () => await _handler.Handle(new GetReceiptExtractionStatusQuery(JobId), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _mockResultReader.Verify(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithExplicitUserId_SkipsTheHttpPrincipal()
        {
            // The Kafka completion consumer runs off-request and supplies the owner directly.
            _mockJobReader.Setup(x => x.GetByIdAsync(JobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ReceiptExtractionJob.Create(JobId, userId: 42, expenseTableId: 1));
            AnalyserReturns(new ReceiptExtractionResult(JobId, ReceiptExtractionStatus.Completed, Array.Empty<ExtractedReceiptItem>(), null));

            var result = await _handler.Handle(new GetReceiptExtractionStatusQuery(JobId, UserId: 42), CancellationToken.None);

            result.Status.Should().Be("Completed");
            _mockCurrentUserProvider.Verify(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
