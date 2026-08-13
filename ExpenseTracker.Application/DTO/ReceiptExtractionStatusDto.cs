namespace ExpenseTracker.Application.DTO
{
    public class ReceiptExtractionStatusDto
    {
        public Guid JobId { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ExtractedExpenseDto> Items { get; set; } = new();
        public string? Error { get; set; }
    }
}
