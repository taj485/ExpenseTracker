using ExpenseTracker.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Domain.Interfaces
{
    public interface IReceiptImageStore
    {
        Task<string> UploadAsync(byte[] content, string contentType, CancellationToken cancellationToken = default);
        Task<ReceiptImage?> DownloadAsync(string blobReference, CancellationToken cancellation = default);
    }
}
