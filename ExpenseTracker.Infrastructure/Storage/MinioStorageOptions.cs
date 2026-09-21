namespace ExpenseTracker.Infrastructure.Storage
{
    public class MinioStorageOptions
    {
        public string Endpoint { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string BucketName { get; set; } = "receipt-images";
        public bool UseSsl { get; set; }
        public string? Region { get; set; }
    }
}
