using ExpenseTracker.Domain.Exceptions;
using System.Collections.Generic;

namespace ExpenseTracker.Domain.Entities
{
    public class User
    {
        public int Id { get; internal set; }
        public string Subject { get; private set; }
        public string? Email { get; private set; }

        private readonly List<Expense> _expenses = new();
        public IReadOnlyCollection<Expense> Expenses => _expenses.AsReadOnly();

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