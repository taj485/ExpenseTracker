using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpenseTracker.Domain.Entities
{
    public class Expense
    {
        public int Id { get; private set; }
        public Money Amount { get; private set; }
        public ExpenseCategory Category { get; private set; }
        public string Description { get; private set; }
        public DateTime Date { get; private set; }
        public bool IsDeleted { get; private set; }

        private readonly List<User> _users = new();
        public IReadOnlyCollection<User> Users => _users.AsReadOnly();

        private Expense() { }

        public static Expense Create(decimal amount, ExpenseCategory category, string description, User owner)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description is required");

            var expense = new Expense
            {
                Amount = Money.Create(amount),
                Category = category,
                Description = description,
                Date = DateTime.UtcNow,
                IsDeleted = false
            };

            expense._users.Add(owner);
            return expense;
        }

        public void AddUser(User user)
        {
            if (_users.Any(u => u.Id == user.Id))
                throw new DomainException("User already has access to this expense");

            _users.Add(user);
        }

        public void UpdateAmount(decimal newAmount)
        {
            Amount = Money.Create(newAmount);
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description cannot be empty");

            Description = description;
        }

        public void UpdateCategory(ExpenseCategory category)
        {
            Category = category;
        }

        public void Delete()
        {
            if (IsDeleted)
                throw new DomainException("Expense is already deleted");

            IsDeleted = true;
        }
    }
}
