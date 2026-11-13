namespace ExpenseTracker.Application.DTO
{
    /// <summary>GivenAt is UTC, so it serialises with a trailing Z.</summary>
    public record AiReceiptConsentDto(bool Given, int? Version, DateTime? GivenAt);
}
