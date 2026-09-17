namespace ExpenseTracker.Domain.ValueObjects
{
    /// <summary>
    /// Read model for one member of an expense table: the membership joined to the user's email.
    /// </summary>
    public class ExpenseTableMember
    {
        public required int UserId { get; init; }
        public string? Email { get; init; }
        public required bool IsAdmin { get; init; }
    }
}
