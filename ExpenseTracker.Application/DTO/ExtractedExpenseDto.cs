using ExpenseTracker.Domain.Enums;
namespace ExpenseTracker.Application.DTO
{
    public class ExtractedExpenseDto
    {
        public decimal UnitPrice { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = nameof(QuantityUnit.Item);
        /// <summary>Discount the scan found on this line, in pounds. 0 when there is none.</summary>
        public decimal Discount { get; set; }
        /// <summary>Discount the scan found on the whole receipt, repeated on every item like Merchant. 0 when there is none.</summary>
        public decimal ReceiptDiscount { get; set; }
        public string? Merchant { get; set; }
        public string? MerchantWebsite { get; set; }
    }
}
