using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.ValueObjects;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Infrastructure.Storage
{
    public class AzureBlobReceiptImageStore : IReceiptImageStore
    {
        private readonly AzureBlobStorageOptions _options;

        public AzureBlobReceiptImageStore(IOptions<AzureBlobStorageOptions> options)
        {
            _options = options.Value;
        }

        public async Task<string> UploadAsync(byte[] content, string contentType, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.ConnectionString))
                throw new InvalidOperationException("Azure Blob Storage is not configured.");

            var blobName = $"{Guid.NewGuid()}{GetExtension(contentType)}";

            var client = new BlobServiceClient(_options.ConnectionString);
            var container = client.GetBlobContainerClient(_options.ContainerName);
            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blob = container.GetBlobClient(blobName);
            using var stream = new MemoryStream(content);
            await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

            return blobName;
        }

        public async Task<ReceiptImage?> DownloadAsync(string blobReference, CancellationToken cancellation = default)
        {
            if (string.IsNullOrWhiteSpace(_options.ConnectionString))
                throw new InvalidOperationException("Azure Blob Storage is not configured.");

            var client = new BlobServiceClient(_options.ConnectionString);
            var container = client.GetBlobContainerClient(_options.ContainerName);
            var blob = container.GetBlobClient(blobReference);

            if (!await blob.ExistsAsync(cancellation))
                return null;

            var download = await blob.DownloadContentAsync(cancellation);
            var contentType = download.Value.Details.ContentType ?? "application/octet-stream";

            return new ReceiptImage
            {
                Content = download.Value.Content.ToArray(),
                ContentType = contentType,
            };
        }

        private static string GetExtension(string contentType) => contentType switch
        {
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => ".jpg"
        };
    }
}
