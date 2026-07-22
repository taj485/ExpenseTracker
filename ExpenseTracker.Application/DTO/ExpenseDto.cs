using System;

namespace ExpenseTracker.Application.DTO
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string? Merchant { get; set; }
        public int? ReceiptId { get; set; }
        public int ExpenseTableId { get; set; }
    }
}
