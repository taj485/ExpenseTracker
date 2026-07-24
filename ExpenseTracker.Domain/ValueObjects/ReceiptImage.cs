using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Domain.ValueObjects
{
    public class ReceiptImage
    {
        public required byte[] Content { get; init; }
        public required string ContentType { get; init; }
    }
}
