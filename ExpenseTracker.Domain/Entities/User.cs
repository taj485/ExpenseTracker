using ExpenseTracker.Domain.Exceptions;

namespace ExpenseTracker.Domain.Entities
{
    public class User
    {
        public int Id { get; internal set; }
        public string Subject { get; private set; }
        public string? Email { get; private set; }

        private User() { }

        public static User Create(string subject, string? email = null)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new DomainException("Subject is required");

            return new User { Subject = subject, Email = email };
        }

        public void UpdateEmail(string? email)
        {
            Email = email;
        }
    }
}