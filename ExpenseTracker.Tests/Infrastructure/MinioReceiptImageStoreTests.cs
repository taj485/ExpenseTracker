using ExpenseTracker.Infrastructure.Storage;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Moq;

namespace ExpenseTracker.Tests.Infrastructure
{
    public class MinioReceiptImageStoreTests
    {
        private readonly Mock<IMinioClient> _client = new();

        private MinioReceiptImageStore BuildStore(string endpoint = "http://localhost:9000") =>
            new(_client.Object, Options.Create(new MinioStorageOptions { Endpoint = endpoint, BucketName = "receipts" }));

        private void SetupRemove(Exception? throws)
        {
            var setup = _client.Setup(c => c.RemoveObjectAsync(It.IsAny<RemoveObjectArgs>(), It.IsAny<CancellationToken>()));
            if (throws is null) setup.Returns(Task.CompletedTask);
            else setup.ThrowsAsync(throws);
        }

        [Fact]
        public async Task DeleteAsync_RemovesTheObject()
        {
            SetupRemove(null);

            await BuildStore().DeleteAsync("a.jpg");

            _client.Verify(c => c.RemoveObjectAsync(It.IsAny<RemoveObjectArgs>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenTheObjectIsAlreadyGone_Succeeds()
        {
            SetupRemove(new ObjectNotFoundException("a.jpg"));

            var act = () => BuildStore().DeleteAsync("a.jpg");

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteAsync_WhenTheBucketDoesNotExist_Succeeds()
        {
            SetupRemove(new BucketNotFoundException("receipts", "missing"));

            var act = () => BuildStore().DeleteAsync("a.jpg");

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteAsync_WhenNotConfigured_Throws()
        {
            var act = () => BuildStore(endpoint: "").DeleteAsync("a.jpg");

            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
