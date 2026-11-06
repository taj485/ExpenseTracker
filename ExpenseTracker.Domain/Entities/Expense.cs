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

        /// <summary>
        /// Discount taken off this whole line (quantity x unit price), in pounds. 0 when there is none.
        /// </summary>
        public decimal Discount { get; private set; }
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

        /// <summary>
        /// The user who added the expense. Null for expenses added before this was recorded.
        /// </summary>
        public int? CreatedByUserId { get; private set; }
        // internal set so tests can attach the user without a database, like User.Id.
        public User? CreatedByUser { get; internal set; }

        private Expense() { }

        public static Expense Create(decimal unitPrice, ExpenseCategory category, string description, DateTime date, int expenseTableId,
            int? merchantId = null, int? receiptId = null, int quantity = 1, int? createdByUserId = null, decimal discount = 0)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description is required");

            if (quantity < 1)
                throw new DomainException("Quantity must be at least 1");

            var price = Money.Create(unitPrice);

            if (discount < 0)
                throw new DomainException("Discount can't be negative.");

            if (discount > unitPrice * quantity)
                throw new DomainException("Discount can't be more than the item total.");

            return new Expense
            {
                UnitPrice = price,
                Quantity = quantity,
                Discount = discount,
                Category = category,
                Description = description,
                Date = date,
                IsDeleted = false,
                MerchantId = merchantId,
                ReceiptId = receiptId,
                ExpenseTableId = expenseTableId,
                CreatedByUserId = createdByUserId
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
