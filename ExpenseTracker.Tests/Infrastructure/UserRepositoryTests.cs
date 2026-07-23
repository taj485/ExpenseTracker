using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Tests.Infrastructure
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ExpenseTrackerDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _context = CreateContext();
            _context.Database.EnsureCreated();

            _repository = new UserRepository(_context);
        }

        private ExpenseTrackerDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ExpenseTrackerDbContext>()
                .UseSqlite(_connection)
                .Options;

            return new ExpenseTrackerDbContext(options);
        }

        [Fact]
        public async Task AddAsync_PersistsUser_AndReturnsGeneratedId()
        {
            var user = User.Create("auth0|user-1", "person@example.com");

            var id = await _repository.AddAsync(user, CancellationToken.None);

            Assert.True(id > 0);

            using var verifyContext = CreateContext();
            var saved = await verifyContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            Assert.NotNull(saved);
            Assert.Equal("auth0|user-1", saved!.Subject);
            Assert.Equal("person@example.com", saved.Email);
        }

        [Fact]
        public async Task GetByUserIdpAsync_ReturnsUser_WhenItExists()
        {
            await _repository.AddAsync(User.Create("auth0|user-1"), CancellationToken.None);

            var result = await _repository.GetByUserIdpAsync("auth0|user-1", CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("auth0|user-1", result!.Subject);
        }

        [Fact]
        public async Task GetByUserIdpAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            var result = await _repository.GetByUserIdpAsync("auth0|does-not-exist", CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ThrowsOnDuplicateSubject()
        {
            await _repository.AddAsync(User.Create("auth0|user-1"), CancellationToken.None);

            await Assert.ThrowsAnyAsync<DbUpdateException>(() =>
                _repository.AddAsync(User.Create("auth0|user-1"), CancellationToken.None));
        }

        [Fact]
        public async Task UpdateAsync_PersistsChanges()
        {
            var user = User.Create("auth0|user-1", "old@example.com");
            await _repository.AddAsync(user, CancellationToken.None);

            user.UpdateEmail("new@example.com");
            await _repository.UpdateAsync(user);

            using var verifyContext = CreateContext();
            var updated = await verifyContext.Users.FirstOrDefaultAsync(u => u.Subject == "auth0|user-1");

            Assert.Equal("new@example.com", updated!.Email);
        }

        [Fact]
        public async Task GetAllByEmailAsync_ReturnsMatchingUsers()
        {
            await _repository.AddAsync(User.Create("auth0|user-1", "person@example.com"), CancellationToken.None);
            await _repository.AddAsync(User.Create("auth0|user-2", "other@example.com"), CancellationToken.None);

            var results = await _repository.GetAllByEmailAsync("person@example.com", CancellationToken.None);

            Assert.Single(results);
            Assert.Equal("auth0|user-1", results.Single().Subject);
        }

        [Fact]
        public async Task GetAllByEmailAsync_ReturnsEmpty_WhenNoMatch()
        {
            var results = await _repository.GetAllByEmailAsync("nobody@example.com", CancellationToken.None);

            Assert.Empty(results);
        }

        [Fact]
        public async Task GetAllByEmailAsync_IsCaseInsensitive()
        {
            await _repository.AddAsync(User.Create("auth0|user-1", "person@example.com"), CancellationToken.None);

            var results = await _repository.GetAllByEmailAsync("Person@Example.COM", CancellationToken.None);

            Assert.Single(results);
            Assert.Equal("auth0|user-1", results.Single().Subject);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}