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

        [Fact]
        public async Task AddAsync_PersistsExpense_AndReturnsGeneratedId()
        {
            var expense = Expense.Create(50.00m, ExpenseCategory.Food, "Groceries");

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
            var id = await _repository.AddAsync(Expense.Create(20m, ExpenseCategory.Transport, "Bus fare"), CancellationToken.None);

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
        public async Task GetByIdAsync_ReturnsNull_WhenExpenseIsSoftDeleted()
        {
            var id = await _repository.AddAsync(Expense.Create(10m, ExpenseCategory.Food, "Snacks"), CancellationToken.None);
            await _repository.DeleteAsync(id);

            var result = await _repository.GetByIdAsync(id, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllNonDeletedExpenses()
        {
            await _repository.AddAsync(Expense.Create(10m, ExpenseCategory.Food, "Lunch"), CancellationToken.None);
            await _repository.AddAsync(Expense.Create(20m, ExpenseCategory.Transport, "Taxi"), CancellationToken.None);

            var results = await _repository.GetAllAsync(CancellationToken.None);

            Assert.Equal(2, results.Count());
        }

        [Fact]
        public async Task GetAllAsync_ExcludesSoftDeletedExpenses()
        {
            var keepId = await _repository.AddAsync(Expense.Create(10m, ExpenseCategory.Food, "Lunch"), CancellationToken.None);
            var deleteId = await _repository.AddAsync(Expense.Create(20m, ExpenseCategory.Transport, "Taxi"), CancellationToken.None);

            await _repository.DeleteAsync(deleteId);

            var results = await _repository.GetAllAsync(CancellationToken.None);

            Assert.Single(results);
            Assert.Equal(keepId, results.Single().Id);
        }

        [Fact]
        public async Task DeleteAsync_SoftDeletesExpense()
        {
            var id = await _repository.AddAsync(Expense.Create(15m, ExpenseCategory.Food, "Coffee"), CancellationToken.None);

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
            var id = await _repository.AddAsync(Expense.Create(30m, ExpenseCategory.Food, "Dinner"), CancellationToken.None);
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
