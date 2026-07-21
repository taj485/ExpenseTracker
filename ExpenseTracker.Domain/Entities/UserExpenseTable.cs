namespace ExpenseTracker.Domain.Entities
{
    public class UserExpenseTable
    {
        public int UserId { get; private set; }
        public int ExpenseTableId { get; private set; }
        public bool IsAdmin { get; private set; }

        public User User { get; private set; }
        public ExpenseTable ExpenseTable { get; private set; }

        private UserExpenseTable() { }

        internal static UserExpenseTable Create(int userId, ExpenseTable expenseTable, bool isAdmin)
        {
            return new UserExpenseTable
            {
                UserId = userId,
                ExpenseTable = expenseTable,
                IsAdmin = isAdmin
            };
        }

        public void PromoteToAdmin() => IsAdmin = true;

        public void DemoteToMember() => IsAdmin = false;
    }
}
