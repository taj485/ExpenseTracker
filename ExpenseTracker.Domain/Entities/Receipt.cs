using System;
using System.Collections.Generic;

namespace ExpenseTracker.Domain.Entities
{
    public class Receipt
    {
        public int Id { get; private set; }
        public DateTime UploadedAt { get; private set; }
        public string? ImageReference { get; private set; }

        private readonly List<Expense> _expenses = new();
        public IReadOnlyCollection<Expense> Expenses => _expenses.AsReadOnly();

        private Receipt() { }

        public static Receipt Create(DateTime uploadedAt, string? imageReference = null)
        {
            return new Receipt
            {
                UploadedAt = uploadedAt.Kind == DateTimeKind.Utc
                    ? uploadedAt
                    : DateTime.SpecifyKind(uploadedAt, DateTimeKind.Utc),
                ImageReference = imageReference
            };
        }
    }
}
