using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Infrastructure.Storage
{
    public class AzureBlobStorageOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string ContainerName { get; set; } = "receipts";
    }
}
