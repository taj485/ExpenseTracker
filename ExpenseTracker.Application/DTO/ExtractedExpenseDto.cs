namespace ExpenseTracker.Application.DTO
{
    public class ExtractedExpenseDto
    {
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public int Quantity { get; set; }
        public string? Merchant { get; set; }
    }
}
