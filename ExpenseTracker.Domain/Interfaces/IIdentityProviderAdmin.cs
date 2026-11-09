namespace ExpenseTracker.Domain.Interfaces
{
    /// <summary>
    /// Administrative access to the identity provider that holds the users' logins.
    /// </summary>
    public interface IIdentityProviderAdmin
    {
        /// <summary>
        /// Deletes the login with this subject. A user that no longer exists is not an error.
        /// </summary>
        Task DeleteUserAsync(string subject, CancellationToken cancellationToken = default);
    }
}
