using ExpenseTracker.Domain.Exceptions;
using System;
using System.Collections.Generic;

namespace ExpenseTracker.Domain.Entities
{
    public class Receipt
    {
        public int Id { get; private set; }
        public DateTime UploadedAt { get; private set; }
        public string? ImageReference { get; private set; }

        /// <summary>
        /// Discount on the whole receipt, taken off after item discounts, in pounds. 0 when there is none.
        /// </summary>
        public decimal Discount { get; private set; }

        private readonly List<Expense> _expenses = new();
        public IReadOnlyCollection<Expense> Expenses => _expenses.AsReadOnly();

        private Receipt() { }

        public static Receipt Create(DateTime uploadedAt, string? imageReference = null, decimal discount = 0)
        {
            if (discount < 0)
                throw new DomainException("Receipt discount can't be negative.");

            return new Receipt
            {
                UploadedAt = uploadedAt.Kind == DateTimeKind.Utc
                    ? uploadedAt
                    : DateTime.SpecifyKind(uploadedAt, DateTimeKind.Utc),
                ImageReference = imageReference,
                Discount = discount
            };
        }
    }
}
