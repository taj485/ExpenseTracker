using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Tests.Infrastructure
{
    public class ExpenseTableRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ExpenseTrackerDbContext _context;
        private readonly ExpenseTableRepository _repository;

        public ExpenseTableRepositoryTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _context = CreateContext();
            _context.Database.EnsureCreated();

            _repository = new ExpenseTableRepository(_context);
        }

        private ExpenseTrackerDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ExpenseTrackerDbContext>()
                .UseSqlite(_connection)
                .Options;

            return new ExpenseTrackerDbContext(options);
        }

        private async Task<User> SeedUserAsync(string subject)
        {
            var user = User.Create(subject);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        [Fact]
        public async Task AddAsync_PersistsTable_AndReturnsGeneratedId()
        {
            var user = await SeedUserAsync("auth0|user-1");
            var table = ExpenseTable.Create("Household", user.Id);

            var id = await _repository.AddAsync(table, CancellationToken.None);

            Assert.True(id > 0);

            using var verifyContext = CreateContext();
            var saved = await verifyContext.ExpenseTables.FirstOrDefaultAsync(t => t.Id == id);

            Assert.NotNull(saved);
            Assert.Equal("Household", saved!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_IncludesMembers()
        {
            var user = await SeedUserAsync("auth0|user-1");
            var table = ExpenseTable.Create("Household", user.Id);
            var id = await _repository.AddAsync(table, CancellationToken.None);

            using var verifyContext = CreateContext();
            var verifyRepository = new ExpenseTableRepository(verifyContext);
            var result = await verifyRepository.GetByIdAsync(id, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result!.Members);
            Assert.Equal(user.Id, result.Members.Single().UserId);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenTableIsSoftDeleted()
        {
            var user = await SeedUserAsync("auth0|user-1");
            var table = ExpenseTable.Create("Household", user.Id);
            var id = await _repository.AddAsync(table, CancellationToken.None);
            await _repository.DeleteAsync(id);

            var result = await _repository.GetByIdAsync(id, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllForUserAsync_ReturnsOnlyTablesTheUserBelongsTo()
        {
            var user1 = await SeedUserAsync("auth0|user-1");
            var user2 = await SeedUserAsync("auth0|user-2");

            await _repository.AddAsync(ExpenseTable.Create("User 1 Table", user1.Id), CancellationToken.None);
            await _repository.AddAsync(ExpenseTable.Create("User 2 Table", user2.Id), CancellationToken.None);

            var results = await _repository.GetAllForUserAsync(user1.Id, CancellationToken.None);

            Assert.Single(results);
            Assert.Equal("User 1 Table", results.Single().Name);
        }

        [Fact]
        public async Task IsMemberAsync_ReturnsTrue_ForMember_AndFalse_ForNonMember()
        {
            var user1 = await SeedUserAsync("auth0|user-1");
            var user2 = await SeedUserAsync("auth0|user-2");
            var table = ExpenseTable.Create("Household", user1.Id);
            var id = await _repository.AddAsync(table, CancellationToken.None);

            Assert.True(await _repository.IsMemberAsync(id, user1.Id, CancellationToken.None));
            Assert.False(await _repository.IsMemberAsync(id, user2.Id, CancellationToken.None));
        }

        [Fact]
        public async Task IsAdminAsync_ReturnsTrue_ForAdmin_AndFalse_ForNonAdminMember()
        {
            var admin = await SeedUserAsync("auth0|admin");
            var member = await SeedUserAsync("auth0|member");
            var table = ExpenseTable.Create("Household", admin.Id);
            table.AddMember(member.Id, isAdmin: false);
            var id = await _repository.AddAsync(table, CancellationToken.None);

            Assert.True(await _repository.IsAdminAsync(id, admin.Id, CancellationToken.None));
            Assert.False(await _repository.IsAdminAsync(id, member.Id, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteAsync_SoftDeletesTable()
        {
            var user = await SeedUserAsync("auth0|user-1");
            var table = ExpenseTable.Create("Household", user.Id);
            var id = await _repository.AddAsync(table, CancellationToken.None);

            await _repository.DeleteAsync(id);

            using var verifyContext = CreateContext();
            var deleted = await verifyContext.ExpenseTables
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == id);

            Assert.NotNull(deleted);
            Assert.True(deleted!.IsDeleted);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsNotFoundException_WhenTableDoesNotExist()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _repository.DeleteAsync(999));
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}
