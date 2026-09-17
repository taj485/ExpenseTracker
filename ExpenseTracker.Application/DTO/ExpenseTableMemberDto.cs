namespace ExpenseTracker.Application.DTO
{
    public class ExpenseTableMemberDto
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
