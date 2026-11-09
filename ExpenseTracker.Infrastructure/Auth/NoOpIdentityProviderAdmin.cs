using ExpenseTracker.Domain.Interfaces;

namespace ExpenseTracker.Infrastructure.Auth
{
    /// <summary>
    /// AI dev mode has no Auth0 user behind its fixed local user, so there is nothing to delete.
    /// </summary>
    public class NoOpIdentityProviderAdmin : IIdentityProviderAdmin
    {
        public Task DeleteUserAsync(string subject, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
