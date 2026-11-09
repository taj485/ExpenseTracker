using ExpenseTracker.Domain.Exceptions;
using System;

namespace ExpenseTracker.Domain.Entities
{
    /// <summary>
    /// The identity-provider subject of an account that was deleted. An access token issued before
    /// the deletion stays valid until it expires, so this stops it from provisioning a new user.
    /// </summary>
    public class DeletedAccount
    {
        public string Subject { get; private set; }
        public DateTime DeletedAt { get; private set; }

        private DeletedAccount() { }

        public static DeletedAccount Create(string subject, DateTime deletedAt)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new DomainException("Subject is required");

            return new DeletedAccount
            {
                Subject = subject,
                DeletedAt = deletedAt.Kind == DateTimeKind.Utc ? deletedAt : DateTime.SpecifyKind(deletedAt, DateTimeKind.Utc)
            };
        }
    }
}
