namespace ExpenseTracker.Domain.Interfaces
{
    public interface ICurrentUserService
    {
        string GetCurrentUserSubject();
        string? GetCurrentUserEmail();
    }
}