using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Tests.Infrastructure
{
    public class AccountRepositoryTests : IDisposable
    {
        private const string AlexSubject = "auth0|alex";

        private readonly SqliteConnection _connection;
        private readonly ExpenseTrackerDbContext _context;
        private readonly AccountRepository _repository;

        public AccountRepositoryTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _context = CreateContext();
            _context.Database.EnsureCreated();

            _repository = new AccountRepository(_context);
        }

        private ExpenseTrackerDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ExpenseTrackerDbContext>()
                .UseSqlite(_connection)
                .Options;

            return new ExpenseTrackerDbContext(options);
        }

        private async Task<User> SeedUserAsync(string subject, string email)
        {
            var user = User.Create(subject, email);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private async Task<ExpenseTable> SeedTableAsync(string name, User creator, params (User user, bool isAdmin)[] members)
        {
            var table = ExpenseTable.Create(name, creator.Id);
            foreach (var (user, isAdmin) in members)
                table.AddMember(user.Id, isAdmin);
            _context.ExpenseTables.Add(table);
            await _context.SaveChangesAsync();
            return table;
        }

        private async Task<Expense> SeedExpenseAsync(ExpenseTable table, string description, decimal amount, User? createdBy, int? receiptId = null, bool deleted = false)
        {
            var expense = Expense.Create(amount, ExpenseCategory.Food, description, DateTime.UtcNow, table.Id,
                receiptId: receiptId, createdByUserId: createdBy?.Id);
            if (deleted)
                expense.Delete();
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return expense;
        }

        private async Task<Receipt> SeedReceiptAsync(string imageReference)
        {
            var receipt = Receipt.Create(DateTime.UtcNow, imageReference);
            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();
            return receipt;
        }

        [Fact]
        public async Task S1_DeletesSpacesWhereTheUserIsTheOnlyMember_WithAllExpensesReceiptsAndPhotos()
        {
            var alex = await SeedUserAsync(AlexSubject, "alex@example.com");
            var home = await SeedTableAsync("Home", alex);
            var trips = await SeedTableAsync("Trips", alex);
            var tesco = await SeedReceiptAsync("tesco.jpg");
            await SeedExpenseAsync(home, "Bread", 1.50m, alex, tesco.Id);
            await SeedExpenseAsync(home, "Milk", 1.20m, alex, tesco.Id);
            await SeedExpenseAsync(trips, "Train", 24.00m, alex);
            await SeedExpenseAsync(trips, "Taxi", 12.00m, alex, deleted: true);

            var imageReferences = await _repository.DeleteAccountDataAsync(AlexSubject, CancellationToken.None);

            imageReferences.Should().Equal("tesco.jpg");
            using var verify = CreateContext();
            (await verify.ExpenseTables.IgnoreQueryFilters().AnyAsync()).Should().BeFalse();
            (await verify.Expenses.IgnoreQueryFilters().AnyAsync()).Should().BeFalse();
            (await verify.Receipts.AnyAsync()).Should().BeFalse();
            (await verify.UserExpenseTables.AnyAsync()).Should().BeFalse();
            (await verify.Users.AnyAsync(u => u.Subject == AlexSubject)).Should().BeFalse();
        }

        [Fact]
        public async Task S1_DeletesSoftDeletedSpacesTheUserWasTheOnlyMemberOf()
        {
            var alex = await SeedUserAsync(AlexSubject, "alex@example.com");
            var old = await SeedTableAsync("Old", alex);
            await SeedExpenseAsync(old, "Coffee", 3m, alex);
            old.Delete();
            await _context.SaveChangesAsync();

            await _repository.DeleteAccountDataAsync(AlexSubject, CancellationToken.None);

            using var verify = CreateContext();
            (await verify.ExpenseTables.IgnoreQueryFilters().AnyAsync()).Should().BeFalse();
            (await verify.Expenses.IgnoreQueryFilters().AnyAsync()).Should().BeFalse();
        }

        [Fact]
        public async Task S3_KeepsASharedSpaceAndItsExpenses_WithoutTheDeletedUserAsCreatorOrMember()
        {
            var alex = await SeedUserAsync(AlexSubject, "alex@example.com");
            var sam = await SeedUserAsync("auth0|sam", "sam@example.com");
            var flat = await SeedTableAsync("Flat", sam, (alex, false));
            var milk = await SeedExpenseAsync(flat, "Milk", 1.20m, alex);
            var bread = await SeedExpenseAsync(flat, "Bread", 1.50m, sam);

            await _repository.DeleteAccountDataAsync(AlexSubject, CancellationToken.None);

            using var verify = CreateContext();
            var savedFlat = await verify.ExpenseTables.Include(t => t.Members).SingleAsync(t => t.Id == flat.Id);
            savedFlat.Members.Select(m => m.UserId).Should().Equal(sam.Id);
            savedFlat.Members.Single().IsAdmin.Should().BeTrue();

            var expenses = await verify.Expenses.Where(e => e.ExpenseTableId == flat.Id).ToListAsync();
            expenses.Should().HaveCount(2);
            expenses.Single(e => e.Id == milk.Id).CreatedByUserId.Should().BeNull();
            expenses.Single(e => e.Id == milk.Id).UnitPrice.Amount.Should().Be(1.20m);
            expenses.Single(e => e.Id == bread.Id).CreatedByUserId.Should().Be(sam.Id);
        }

        [Fact]
        public async Task S3_ClearsTheCreatorOfASharedSpaceTheDeletedUserCreated()
        {
            var alex = await SeedUserAsync(AlexSubject, "alex@example.com");
            var sam = await SeedUserAsync("auth0|sam", "sam@example.com");
            var shared = await SeedTableAsync("Shared", alex, (sam, true));

            await _repository.DeleteAccountDataAsync(AlexSubject, CancellationToken.None);

            using var verify = CreateContext();
            var saved = await verify.ExpenseTables.SingleAsync(t => t.Id == shared.Id);
            saved.CreatedByUserId.Should().BeNull();
        }

        [Fact]
        public async Task S4_WhenTheOnlyAdminLeavesASpaceTheyDidNotCreate_EveryRemainingMemberBecomesAnAdmin()
        {
            var alex = await SeedUserAsync(AlexSubject, "alex@example.com");
            var sam = await SeedUserAsync("auth0|sam", "sam@example.com");
            var flat = await SeedTableAsync("Flat", sam, (alex, true));
            _context.Entry(flat.Members.Single(m => m.UserId == sam.Id)).Property(nameof(UserExpenseTable.IsAdmin)).CurrentValue = false;
            await _context.SaveChangesAsync();

            await _repository.DeleteAccountDataAsync(AlexSubject, CancellationToken.None);

            using var verify = CreateContext();
            var members = await verify.UserExpenseTables.Where(m => m.ExpenseTableId == flat.Id).ToListAsync();
            members.Should().ContainSingle().Which.UserId.Should().Be(sam.Id);
            members.Single().IsAdmin.Should().BeTrue();
        }

        [Fact]
        public async Task S1_DeletesASpaceTheUserCreatedButAlreadyLeft_WhenNobodyIsLeftInIt()
        {
            var alex = await SeedUserAsync(AlexSubject, "alex@example.com");
            var empty = await SeedTableAsync("Empty", alex);
            var membership = await _context.UserExpenseTables.SingleAsync(m => m.ExpenseTableId == empty.Id);
            _context.UserExpenseTables.Remove(membership);
            await _context.SaveChangesAsync();

            await _repository.DeleteAccountDataAsync(AlexSubject, CancellationToken.None);

            using var verify = CreateContext();
            (await verify.ExpenseTables.IgnoreQueryFilters().AnyAsync()).Should().BeFalse();
        }

        [Fact]
        public async Task S1_KeepsAReceiptAndItsPhotoWhileAnExpenseInAnotherSpaceStillUsesIt()
        {
            var alex = await SeedUserAsync(AlexSubject, "alex@example.com");
            var sam = await SeedUserAsync("auth0|sam", "sam@example.com");
            var home = await SeedTableAsync("Home", alex);
            var flat = await SeedTableAsync("Flat", sam);
            var shared = await SeedReceiptAsync("shared.jpg");
            await SeedExpenseAsync(home, "Bread", 1.50m, alex, shared.Id);
            await SeedExpenseAsync(flat, "Milk", 1.20m, sam, shared.Id);

            var imageReferences = await _repository.DeleteAccountDataAsync(AlexSubject, CancellationToken.None);

            imageReferences.Should().BeEmpty();
            using var verify = CreateContext();
            (await verify.Receipts.AnyAsync(r => r.Id == shared.Id)).Should().BeTrue();
        }

        [Fact]
        public async Task S4_WhenTheOnlyAdminLeaves_EveryRemainingMemberBecomesAnAdmin()
        {
            var alex = await SeedUserAsync(AlexSubject, "alex@example.com");
            var sam = await SeedUserAsync("auth0|sam", "sam@example.com");
            var kim = await SeedUserAsync("auth0|kim", "kim@example.com");
            var office = await SeedTableAsync("Office", alex, (sam, false), (kim, false));

            await _repository.DeleteAccountDataAsync(AlexSubject, CancellationToken.None);

            using var verify = CreateContext();
            var members = await verify.UserExpenseTables.Where(m => m.ExpenseTableId == office.Id).ToListAsync();
            members.Select(m => m.UserId).Should().BeEquivalentTo(new[] { sam.Id, kim.Id });
            members.Should().OnlyContain(m => m.IsAdmin);
        }

        [Fact]
        public async Task S4_WhenAnotherAdminRemains_NoMemberIsPromoted()
        {
            var alex = await SeedUserAsync(AlexSubject, "alex@example.com");
            var sam = await SeedUserAsync("auth0|sam", "sam@example.com");
            var kim = await SeedUserAsync("auth0|kim", "kim@example.com");
            var office = await SeedTableAsync("Office", alex, (sam, true), (kim, false));

            await _repository.DeleteAccountDataAsync(AlexSubject, CancellationToken.None);

            using var verify = CreateContext();
            var members = await verify.UserExpenseTables.Where(m => m.ExpenseTableId == office.Id).ToListAsync();
            members.Single(m => m.UserId == sam.Id).IsAdmin.Should().BeTrue();
            members.Single(m => m.UserId == kim.Id).IsAdmin.Should().BeFalse();
        }

        [Fact]
        public async Task S7_RecordsTheSubjectAsDeleted()
        {
            await SeedUserAsync(AlexSubject, "alex@example.com");

            await _repository.DeleteAccountDataAsync(AlexSubject, CancellationToken.None);

            (await _repository.IsDeletedAsync(AlexSubject)).Should().BeTrue();
            (await _repository.IsDeletedAsync("auth0|someone-else")).Should().BeFalse();
        }

        [Fact]
        public async Task S6_SecondCall_SucceedsAndChangesNothing()
        {
            var alex = await SeedUserAsync(AlexSubject, "alex@example.com");
            var sam = await SeedUserAsync("auth0|sam", "sam@example.com");
            await SeedTableAsync("Flat", sam, (alex, false));
            await _repository.DeleteAccountDataAsync(AlexSubject, CancellationToken.None);

            var second = await _repository.DeleteAccountDataAsync(AlexSubject, CancellationToken.None);

            second.Should().BeEmpty();
            using var verify = CreateContext();
            (await verify.DeletedAccounts.CountAsync()).Should().Be(1);
            (await verify.ExpenseTables.CountAsync()).Should().Be(1);
            (await verify.Users.Select(u => u.Subject).ToListAsync()).Should().Equal("auth0|sam");
        }

        [Fact]
        public async Task S5_SignUpWithTheSameEmail_DoesNotSeeTheDeletedAccountsSpaces()
        {
            var alex = await SeedUserAsync(AlexSubject, "alex@example.com");
            await SeedTableAsync("Home", alex);
            await _repository.DeleteAccountDataAsync(AlexSubject, CancellationToken.None);

            var newAlex = await SeedUserAsync("auth0|alex-again", "alex@example.com");

            using var verify = CreateContext();
            (await verify.UserExpenseTables.AnyAsync(m => m.UserId == newAlex.Id)).Should().BeFalse();
            (await verify.ExpenseTables.AnyAsync()).Should().BeFalse();
        }

        [Fact]
        public async Task DeletingTheAccount_RemovesItsAiReceiptConsent()
        {
            var alex = await SeedUserAsync(AlexSubject, "alex@example.com");
            alex.GiveAiReceiptConsent(1, DateTimeOffset.UtcNow);
            await _context.SaveChangesAsync();

            await _repository.DeleteAccountDataAsync(AlexSubject, CancellationToken.None);

            using var verify = CreateContext();
            (await verify.Users.AnyAsync(u => u.AiReceiptConsentVersion != null)).Should().BeFalse();
            (await verify.Users.AnyAsync(u => u.Subject == AlexSubject)).Should().BeFalse();
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}
