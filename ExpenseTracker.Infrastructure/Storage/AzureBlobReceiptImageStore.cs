using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.ValueObjects;
using Microsoft.Extensions.Options;

namespace ExpenseTracker.Infrastructure.Storage
{
    public class AzureBlobReceiptImageStore : IReceiptImageStore
    {
        private readonly BlobServiceClient _client;
        private readonly AzureBlobStorageOptions _options;

        public AzureBlobReceiptImageStore(BlobServiceClient client, IOptions<AzureBlobStorageOptions> options)
        {
            _client = client;
            _options = options.Value;
        }

        public async Task<string> UploadTempAsync(byte[] content, string contentType, CancellationToken cancellationToken = default)
        {
            var blobName = $"{Guid.NewGuid()}{GetExtension(contentType)}";
            var blob = TempContainer.GetBlobClient(blobName);

            using var stream = new MemoryStream(content);
            await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

            return blobName;
        }

        public Uri GenerateTempReadSasUri(string blobName)
        {
            var blob = TempContainer.GetBlobClient(blobName);

            if (!blob.CanGenerateSasUri)
                throw new InvalidOperationException("Blob storage is not configured with a shared key, so a SAS cannot be issued.");

            return blob.GenerateSasUri(BuildReadSas(blob));
        }

        public async Task<string?> PromoteTempAsync(string blobName, CancellationToken cancellationToken = default)
        {
            var source = TempContainer.GetBlobClient(blobName);

            if (!await source.ExistsAsync(cancellationToken))
                return null;

            // Same name in the permanent container keeps this idempotent: the SPA saves once per selected
            // table, so several requests promote the same blob concurrently and must converge.
            var destination = PermanentContainer.GetBlobClient(blobName);

            if (!source.CanGenerateSasUri)
                throw new InvalidOperationException("Blob storage is not configured with a shared key, so a SAS cannot be issued.");

            var sourceUri = source.GenerateSasUri(BuildReadSas(source));

            // Same storage account, so this is a server-side copy — the bytes never travel through the API.
            await destination.SyncCopyFromUriAsync(sourceUri, cancellationToken: cancellationToken);

            return blobName;
        }

        public async Task<ReceiptImage?> DownloadAsync(string blobReference, CancellationToken cancellation = default)
        {
            var blob = PermanentContainer.GetBlobClient(blobReference);

            if (!await blob.ExistsAsync(cancellation))
                return null;

            var download = await blob.DownloadContentAsync(cancellation);

            return new ReceiptImage
            {
                Content = download.Value.Content.ToArray(),
                ContentType = download.Value.Details.ContentType ?? "application/octet-stream",
            };
        }

        private BlobContainerClient PermanentContainer => _client.GetBlobContainerClient(_options.ContainerName);

        private BlobContainerClient TempContainer => _client.GetBlobContainerClient(_options.TempContainerName);

        private BlobSasBuilder BuildReadSas(BlobClient blob)
        {
            var builder = new BlobSasBuilder
            {
                BlobContainerName = blob.BlobContainerName,
                BlobName = blob.Name,
                Resource = "b",
                // Backdated so a few seconds of clock skew between us and Azure can't invalidate the link.
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = DateTimeOffset.UtcNow.Add(_options.SasLifetime),
            };

            builder.SetPermissions(BlobSasPermissions.Read);

            return builder;
        }

        private static string GetExtension(string contentType) => contentType switch
        {
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => ".jpg"
        };
    }
}
