namespace ExpenseTracker.Domain.Entities
{
    /// <summary>
    /// Records who requested a receipt extraction so the status endpoint and the SignalR hub can
    /// authorize by owner. A job id alone is unguessable but is not authorization.
    /// </summary>
    public class ReceiptExtractionJob
    {
        public Guid Id { get; private set; }
        public int UserId { get; private set; }
        public int ExpenseTableId { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        private ReceiptExtractionJob() { }

        public static ReceiptExtractionJob Create(Guid id, int userId, int expenseTableId)
        {
            return new ReceiptExtractionJob
            {
                Id = id,
                UserId = userId,
                ExpenseTableId = expenseTableId,
                CreatedAtUtc = DateTime.UtcNow,
            };
        }
    }
}
