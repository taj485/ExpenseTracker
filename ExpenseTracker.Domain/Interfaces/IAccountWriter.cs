namespace ExpenseTracker.Domain.Interfaces
{
    public interface IAccountWriter
    {
        /// <summary>
        /// In one transaction: deletes the spaces where the user with this subject is the only member
        /// (with all their expenses and receipts), removes the user from shared spaces (promoting every
        /// remaining member when the user was the last admin), clears the user as creator of anything
        /// that remains, deletes the user and records the subject as deleted.
        /// Safe to call again when the user is already gone.
        /// </summary>
        /// <returns>The image references of the deleted receipts, to remove from storage.</returns>
        Task<IReadOnlyList<string>> DeleteAccountDataAsync(string subject, CancellationToken cancellationToken);
    }
}
