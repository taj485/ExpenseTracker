using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;


namespace ExpenseTracker.Tests.Infrastructure
{
    public class ExpenseRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ExpenseTrackerDbContext _context;
        private readonly ExpenseRepository _repository;

        public ExpenseRepositoryTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _context = CreateContext();
            _context.Database.EnsureCreated();

            _repository = new ExpenseRepository(_context);
        }

        private ExpenseTrackerDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ExpenseTrackerDbContext>()
                .UseSqlite(_connection)
                .Options;

            return new ExpenseTrackerDbContext(options);
        }

        private async Task<ExpenseTable> SeedExpenseTableAsync(string subject)
        {
            var user = User.Create(subject);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var table = ExpenseTable.Create("Test Table", user.Id);
            _context.ExpenseTables.Add(table);
            await _context.SaveChangesAsync();

            return table;
        }

        [Fact]
        public async Task AddAsync_PersistsExpense_AndReturnsGeneratedId()
        {
            var table = await SeedExpenseTableAsync("auth0|user-1");
            var expense = Expense.Create(50.00m, ExpenseCategory.Food, "Groceries", DateTime.UtcNow, table.Id);

            var id = await _repository.AddAsync(expense, CancellationToken.None);

            Assert.True(id > 0);

            using var verifyContext = CreateContext();
            var saved = await verifyContext.Expenses.FirstOrDefaultAsync(e => e.Id == id);

            Assert.NotNull(saved);
            Assert.Equal("Groceries", saved!.Description);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsExpense_WhenItExists()
        {
            var table = await SeedExpenseTableAsync("auth0|user-1");
            var id = await _repository.AddAsync(Expense.Create(20m, ExpenseCategory.Transport, "Bus fare", DateTime.UtcNow, table.Id), CancellationToken.None);

            var result = await _repository.GetByIdAsync(id, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("Bus fare", result!.Description);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenExpenseDoesNotExist()
        {
            var result = await _repository.GetByIdAsync(999, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsExpenseWithCorrectExpenseTableId()
        {
            var table = await SeedExpenseTableAsync("auth0|user-1");
            var id = await _repository.AddAsync(Expense.Create(10m, ExpenseCategory.Food, "Snacks", DateTime.UtcNow, table.Id), CancellationToken.None);

            using var verifyContext = CreateContext();
            var verifyRepository = new ExpenseRepository(verifyContext);
            var result = await verifyRepository.GetByIdAsync(id, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(table.Id, result!.ExpenseTableId);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenExpenseIsSoftDeleted()
        {
            var table = await SeedExpenseTableAsync("auth0|user-1");
            var id = await _repository.AddAsync(Expense.Create(10m, ExpenseCategory.Food, "Snacks", DateTime.UtcNow, table.Id), CancellationToken.None);
            await _repository.DeleteAsync(id);

            var result = await _repository.GetByIdAsync(id, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllForTableAsync_ReturnsOnlyThatTablesNonDeletedExpenses()
        {
            var table1 = await SeedExpenseTableAsync("auth0|user-1");
            var table2 = await SeedExpenseTableAsync("auth0|user-2");

            await _repository.AddAsync(Expense.Create(10m, ExpenseCategory.Food, "Lunch", DateTime.UtcNow, table1.Id), CancellationToken.None);
            await _repository.AddAsync(Expense.Create(20m, ExpenseCategory.Transport, "Taxi", DateTime.UtcNow, table1.Id), CancellationToken.None);
            await _repository.AddAsync(Expense.Create(30m, ExpenseCategory.Food, "Dinner", DateTime.UtcNow, table2.Id), CancellationToken.None);

            var results = await _repository.GetAllForTableAsync(table1.Id, CancellationToken.None);

            Assert.Equal(2, results.Count());
        }

        [Fact]
        public async Task GetAllForTableAsync_ExcludesSoftDeletedExpenses()
        {
            var table = await SeedExpenseTableAsync("auth0|user-1");
            var keepId = await _repository.AddAsync(Expense.Create(10m, ExpenseCategory.Food, "Lunch", DateTime.UtcNow, table.Id), CancellationToken.None);
            var deleteId = await _repository.AddAsync(Expense.Create(20m, ExpenseCategory.Transport, "Taxi", DateTime.UtcNow, table.Id), CancellationToken.None);

            await _repository.DeleteAsync(deleteId);

            var results = await _repository.GetAllForTableAsync(table.Id, CancellationToken.None);

            Assert.Single(results);
            Assert.Equal(keepId, results.Single().Id);
        }

        [Fact]
        public async Task GetAllForTableAsync_DoesNotReturnOtherTablesExpenses()
        {
            var table1 = await SeedExpenseTableAsync("auth0|user-1");
            var table2 = await SeedExpenseTableAsync("auth0|user-2");

            await _repository.AddAsync(Expense.Create(10m, ExpenseCategory.Food, "Lunch", DateTime.UtcNow, table2.Id), CancellationToken.None);

            var results = await _repository.GetAllForTableAsync(table1.Id, CancellationToken.None);

            Assert.Empty(results);
        }

        [Fact]
        public async Task DeleteAsync_SoftDeletesExpense()
        {
            var table = await SeedExpenseTableAsync("auth0|user-1");
            var id = await _repository.AddAsync(Expense.Create(15m, ExpenseCategory.Food, "Coffee", DateTime.UtcNow, table.Id), CancellationToken.None);

            await _repository.DeleteAsync(id);

            using var verifyContext = CreateContext();
            var deleted = await verifyContext.Expenses
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Id == id);

            Assert.NotNull(deleted);
            Assert.True(deleted!.IsDeleted);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsNotFoundException_WhenExpenseDoesNotExist()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _repository.DeleteAsync(999));
        }

        [Fact]
        public async Task UpdateAsync_PersistsChanges()
        {
            var table = await SeedExpenseTableAsync("auth0|user-1");
            var id = await _repository.AddAsync(Expense.Create(30m, ExpenseCategory.Food, "Dinner", DateTime.UtcNow, table.Id), CancellationToken.None);
            var expense = await _repository.GetByIdAsync(id, CancellationToken.None);

            expense!.UpdateDescription("Dinner with friends");
            await _repository.UpdateAsync(expense);

            using var verifyContext = CreateContext();
            var updated = await verifyContext.Expenses.FirstOrDefaultAsync(e => e.Id == id);

            Assert.Equal("Dinner with friends", updated!.Description);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}
