using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.ValueObjects;
using System;

namespace ExpenseTracker.Domain.Entities
{
    public class Expense
    {
        public int Id { get; private set; }
        public Money UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public ExpenseCategory Category { get; private set; }
        public string Description { get; private set; }
        public DateTime Date { get; private set; }
        public bool IsDeleted { get; private set; }
        public int? MerchantId { get; private set; }
        public Merchant? Merchant { get; private set; }
        public int? ReceiptId { get; private set; }
        public Receipt? Receipt { get; private set; }
        public int ExpenseTableId { get; private set; }
        public ExpenseTable? ExpenseTable { get; private set; }

        private Expense() { }

        public static Expense Create(decimal unitPrice, ExpenseCategory category, string description, DateTime date, int expenseTableId,
            int? merchantId = null, int? receiptId = null, int quantity = 1)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description is required");

            if (quantity < 1)
                throw new DomainException("Quantity must be at least 1");

            return new Expense
            {
                UnitPrice = Money.Create(unitPrice),
                Quantity = quantity,
                Category = category,
                Description = description,
                Date = date,
                IsDeleted = false,
                MerchantId = merchantId,
                ReceiptId = receiptId,
                ExpenseTableId = expenseTableId
            };
        }

        public void UpdateUnitPrice(decimal newUnitPrice)
        {
            UnitPrice = Money.Create(newUnitPrice);
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity < 1)
                throw new DomainException("Quantity must be at least 1");

            Quantity = quantity;
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description cannot be empty");

            Description = description;
        }

        public void UpdateCategory(ExpenseCategory category)
        {
            Category = category;
        }

        public void UpdateMerchant(int? merchantId)
        {
            MerchantId = merchantId;
        }

        public void Delete()
        {
            if (IsDeleted)
                throw new DomainException("Expense is already deleted");

            IsDeleted = true;
        }
    }
}
