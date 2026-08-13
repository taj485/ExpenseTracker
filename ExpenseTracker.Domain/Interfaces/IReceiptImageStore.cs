using ExpenseTracker.Domain.ValueObjects;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IReceiptImageStore
    {
        /// <summary>Uploads to the temp container, which has a lifecycle rule that deletes after 24h.</summary>
        Task<string> UploadTempAsync(byte[] content, string contentType, CancellationToken cancellationToken = default);

        /// <summary>A short-lived, read-only SAS scoped to one temp blob, safe to hand to the analyser.</summary>
        Uri GenerateTempReadSasUri(string blobName);

        /// <summary>
        /// Server-side copies a temp blob into the permanent container under the same name and returns
        /// that reference, or null when the source no longer exists. Idempotent by design.
        /// </summary>
        Task<string?> PromoteTempAsync(string blobName, CancellationToken cancellationToken = default);

        Task<ReceiptImage?> DownloadAsync(string blobReference, CancellationToken cancellation = default);
    }
}