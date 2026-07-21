using ExpenseTracker.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpenseTracker.Domain.Entities
{
    public class ExpenseTable
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int CreatedByUserId { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime DateCreated { get; private set; }
        public DateTime? DateDeleted { get; private set; }
        public DateTime LastModified { get; private set; }

        private readonly List<UserExpenseTable> _members = new();
        public IReadOnlyCollection<UserExpenseTable> Members => _members.AsReadOnly();

        private ExpenseTable() { }

        public static ExpenseTable Create(string name, int createdByUserId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required");

            var now = DateTime.UtcNow;
            var table = new ExpenseTable
            {
                Name = name,
                CreatedByUserId = createdByUserId,
                IsDeleted = false,
                DateCreated = now,
                LastModified = now
            };

            table._members.Add(UserExpenseTable.Create(createdByUserId, table, isAdmin: true));
            return table;
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required");

            Name = name;
            LastModified = DateTime.UtcNow;
        }

        public void AddMember(int userId, bool isAdmin)
        {
            if (_members.Any(m => m.UserId == userId))
                throw new DomainException("User is already a member of this expense table");

            _members.Add(UserExpenseTable.Create(userId, this, isAdmin));
            LastModified = DateTime.UtcNow;
        }

        public void RemoveMember(int userId)
        {
            var member = _members.FirstOrDefault(m => m.UserId == userId);
            if (member is null)
                throw new DomainException("User is not a member of this expense table");

            if (member.IsAdmin && _members.Count(m => m.IsAdmin) == 1)
                throw new DomainException("Cannot remove the last remaining admin");

            _members.Remove(member);
            LastModified = DateTime.UtcNow;
        }

        public void Delete()
        {
            if (IsDeleted)
                throw new DomainException("Expense table is already deleted");

            IsDeleted = true;
            DateDeleted = DateTime.UtcNow;
        }
    }
}
