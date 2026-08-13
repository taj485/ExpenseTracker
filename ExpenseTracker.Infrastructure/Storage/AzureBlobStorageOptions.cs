namespace ExpenseTracker.Infrastructure.Storage
{
    public class AzureBlobStorageOptions
    {
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>Permanent container, referenced by <c>Receipt.ImageReference</c>.</summary>
        public string ContainerName { get; set; } = "receipt-images";

        /// <summary>
        /// Holding container for images awaiting a keep/abandon decision. Has a lifecycle rule that
        /// deletes after a day, so abandoned uploads clean themselves up.
        /// </summary>
        public string TempContainerName { get; set; } = "receipt-temp";

        /// <summary>
        /// How long the SAS handed to the analyser stays valid. Long enough to survive an analyser
        /// restart, short enough that a leaked topic message ages out quickly.
        /// </summary>
        public TimeSpan SasLifetime { get; set; } = TimeSpan.FromHours(2);
    }
}
