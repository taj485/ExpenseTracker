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
        public decimal Quantity { get; private set; }
        public QuantityUnit Unit { get; private set; }
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
            int? merchantId = null, int? receiptId = null, decimal quantity = 1, int? createdByUserId = null,
            QuantityUnit unit = QuantityUnit.Item)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description is required");

            EnsureValidQuantity(quantity, unit);

            return new Expense
            {
                UnitPrice = Money.Create(unitPrice),
                Quantity = quantity,
                Unit = unit,
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

        public void UpdateQuantity(decimal quantity, QuantityUnit unit = QuantityUnit.Item)
        {
            EnsureValidQuantity(quantity, unit);

            Quantity = quantity;
            Unit = unit;
        }

        /// <summary>
        /// Why a quantity is not allowed for its unit, or null when it is. Items are counted in whole
        /// numbers; weights and volumes may be fractional, to at most 3 decimal places.
        /// </summary>
        public static string? QuantityError(decimal quantity, QuantityUnit unit)
        {
            if (unit == QuantityUnit.Item && (quantity < 1 || quantity % 1 != 0))
                return "Quantity must be a whole number of at least 1";

            if (quantity <= 0)
                return "Quantity must be greater than 0";

            if (decimal.Round(quantity, 3) != quantity)
                return "Quantity can have at most 3 decimal places";

            return null;
        }

        private static void EnsureValidQuantity(decimal quantity, QuantityUnit unit)
        {
            var error = QuantityError(quantity, unit);
            if (error is not null)
                throw new DomainException(error);
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
