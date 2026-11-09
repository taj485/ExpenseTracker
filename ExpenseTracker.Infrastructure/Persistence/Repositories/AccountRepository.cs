using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories
{
    public class AccountRepository : IAccountWriter, IDeletedAccountReader
    {
        private readonly ExpenseTrackerDbContext _context;

        public AccountRepository(ExpenseTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsDeletedAsync(string subject, CancellationToken cancellationToken = default)
        {
            return await _context.DeletedAccounts.AnyAsync(d => d.Subject == subject, cancellationToken);
        }

        public async Task<IReadOnlyList<string>> DeleteAccountDataAsync(string subject, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var imageReferences = new List<string>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Subject == subject, cancellationToken);

            if (user is not null)
            {
                imageReferences.AddRange(await RemoveUserDataAsync(user.Id, cancellationToken));
                _context.Users.Remove(user);
            }

            if (!await IsDeletedAsync(subject, cancellationToken))
                _context.DeletedAccounts.Add(DeletedAccount.Create(subject, DateTime.UtcNow));

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return imageReferences;
        }

        private async Task<List<string>> RemoveUserDataAsync(int userId, CancellationToken cancellationToken)
        {
            var imageReferences = new List<string>();
            var deletedTableIds = new HashSet<int>();

            // Soft-deleted spaces count too: a hard delete leaves nothing personal behind.
            var tables = await _context.ExpenseTables
                .IgnoreQueryFilters()
                .Include(t => t.Members)
                .Where(t => t.CreatedByUserId == userId || t.Members.Any(m => m.UserId == userId))
                .ToListAsync(cancellationToken);

            foreach (var table in tables)
            {
                if (table.Members.All(m => m.UserId == userId))
                {
                    imageReferences.AddRange(await RemoveTableAsync(table, cancellationToken));
                    deletedTableIds.Add(table.Id);
                }
                else
                {
                    table.RemoveDeletedAccount(userId);
                }
            }

            var createdExpenses = await _context.Expenses
                .IgnoreQueryFilters()
                .Where(e => e.CreatedByUserId == userId && !deletedTableIds.Contains(e.ExpenseTableId))
                .ToListAsync(cancellationToken);

            foreach (var expense in createdExpenses)
                expense.ClearCreator();

            return imageReferences;
        }

        private async Task<List<string>> RemoveTableAsync(ExpenseTable table, CancellationToken cancellationToken)
        {
            var tableId = table.Id;

            var expenses = await _context.Expenses
                .IgnoreQueryFilters()
                .Where(e => e.ExpenseTableId == tableId)
                .ToListAsync(cancellationToken);

            var receiptIds = expenses
                .Where(e => e.ReceiptId.HasValue)
                .Select(e => e.ReceiptId!.Value)
                .Distinct()
                .ToList();

            // A receipt still linked to an expense in another space stays.
            var receipts = await _context.Receipts
                .IgnoreQueryFilters()
                .Where(r => receiptIds.Contains(r.Id) && !r.Expenses.Any(e => e.ExpenseTableId != tableId))
                .ToListAsync(cancellationToken);

            _context.Expenses.RemoveRange(expenses);
            _context.Receipts.RemoveRange(receipts);
            _context.ExpenseTables.Remove(table);

            return receipts
                .Where(r => !string.IsNullOrWhiteSpace(r.ImageReference))
                .Select(r => r.ImageReference!)
                .ToList();
        }
    }
}
