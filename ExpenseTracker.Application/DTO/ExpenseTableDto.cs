using System;

namespace ExpenseTracker.Application.DTO
{
    public class ExpenseTableDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsCurrentUserAdmin { get; set; }
        public int MemberCount { get; set; }
    }
}
