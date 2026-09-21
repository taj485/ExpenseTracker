using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.ValueObjects;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace ExpenseTracker.Infrastructure.Storage
{
    public class MinioReceiptImageStore : IReceiptImageStore
    {
        private readonly IMinioClient _client;
        private readonly MinioStorageOptions _options;

        public MinioReceiptImageStore(IMinioClient client, IOptions<MinioStorageOptions> options)
        {
            _client = client;
            _options = options.Value;
        }

        public async Task<string> UploadAsync(byte[] content, string contentType, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.Endpoint))
                throw new InvalidOperationException("MinIO storage is not configured.");

            await EnsureBucketExistsAsync(cancellationToken);

            var objectName = $"{Guid.NewGuid()}{GetExtension(contentType)}";

            using var stream = new MemoryStream(content);
            await _client.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(content.Length)
                .WithContentType(contentType), cancellationToken);

            return objectName;
        }

        public async Task<ReceiptImage?> DownloadAsync(string blobReference, CancellationToken cancellation = default)
        {
            if (string.IsNullOrWhiteSpace(_options.Endpoint))
                throw new InvalidOperationException("MinIO storage is not configured.");

            using var buffer = new MemoryStream();

            try
            {
                var stat = await _client.GetObjectAsync(new GetObjectArgs()
                    .WithBucket(_options.BucketName)
                    .WithObject(blobReference)
                    .WithCallbackStream((stream, ct) => stream.CopyToAsync(buffer, ct)), cancellation);

                return new ReceiptImage
                {
                    Content = buffer.ToArray(),
                    ContentType = string.IsNullOrWhiteSpace(stat?.ContentType)
                        ? "application/octet-stream"
                        : stat.ContentType,
                };
            }
            catch (ObjectNotFoundException)
            {
                return null;
            }
            catch (BucketNotFoundException)
            {
                return null;
            }
        }

        private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
        {
            var exists = await _client.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(_options.BucketName), cancellationToken);

            if (!exists)
            {
                await _client.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(_options.BucketName), cancellationToken);
            }
        }

        private static string GetExtension(string contentType) => contentType switch
        {
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => ".jpg"
        };
    }
}
