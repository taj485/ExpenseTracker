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
        public string? Merchant { get; set; }
        public string? MerchantWebsite { get; set; }
    }
}
