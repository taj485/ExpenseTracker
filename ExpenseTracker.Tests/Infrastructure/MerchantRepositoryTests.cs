using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Tests.Infrastructure
{
    /// <summary>
    /// Runs against the real seeded reference data, so these also assert that the seed itself
    /// resolves the spellings it is meant to.
    /// </summary>
    public class MerchantRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ExpenseTrackerDbContext _context;
        private readonly MerchantRepository _repository;

        public MerchantRepositoryTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<ExpenseTrackerDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new ExpenseTrackerDbContext(options);
            _context.Database.EnsureCreated();

            _repository = new MerchantRepository(_context);
        }

        [Fact]
        public async Task TheSeedDataIsPresent()
        {
            (await _context.Merchants.CountAsync()).Should().Be(1000);
            (await _context.MerchantAliases.CountAsync()).Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("Tesco", "Tesco")]
        [InlineData("tesco", "Tesco")]
        [InlineData("  TESCO  ", "Tesco")]
        [InlineData("Sainsbury's", "Sainsbury's")]
        [InlineData("B&Q", "B&Q")]
        public async Task GetByNormalizedNameAsync_matchesOnTheMerchantsOwnName(string input, string expected)
        {
            var merchant = await _repository.GetByNormalizedNameAsync(Merchant.Normalize(input));

            merchant.Should().NotBeNull();
            merchant!.Name.Should().Be(expected);
        }

        [Theory]
        [InlineData("TESCO STORES LIMITED", "Tesco")]
        [InlineData("Tesco Express", "Tesco")]
        [InlineData("Sainsburys Local", "Sainsbury's")]
        [InlineData("M&S", "Marks & Spencer")]
        [InlineData("Currys PC World", "Currys")]
        [InlineData("Shell UK Oil", "Shell")]
        [InlineData("Costa", "Costa Coffee")]
        [InlineData("Wetherspoons", "Wetherspoon")]
        public async Task GetByNormalizedNameAsync_fallsBackToTheReceiptSpellings(string printed, string expected)
        {
            var merchant = await _repository.GetByNormalizedNameAsync(Merchant.Normalize(printed));

            merchant.Should().NotBeNull();
            merchant!.Name.Should().Be(expected);
        }

        [Fact]
        public async Task GetByNormalizedNameAsync_returnsNull_forAnUnknownMerchant()
        {
            var merchant = await _repository.GetByNormalizedNameAsync(Merchant.Normalize("Bob's Corner Shop"));

            merchant.Should().BeNull();
        }

        [Fact]
        public async Task GetByNormalizedNameAsync_returnsNull_forAnEmptyKey()
        {
            (await _repository.GetByNormalizedNameAsync("")).Should().BeNull();
        }

        [Fact]
        public async Task SeededMerchants_carryTheirWebsite()
        {
            var tesco = await _repository.GetByNormalizedNameAsync("tesco");

            tesco!.Website.Should().Be("tesco.com");
        }

        [Fact]
        public async Task AddAsync_persistsANewMerchantWithNoWebsite()
        {
            var id = await _repository.AddAsync(Merchant.Create("Bob's Corner Shop"), CancellationToken.None);

            id.Should().BeGreaterThan(0);

            var reloaded = await _repository.GetByNormalizedNameAsync("bobscornershop");
            reloaded.Should().NotBeNull();
            reloaded!.Name.Should().Be("Bob's Corner Shop");
            reloaded.Website.Should().BeNull();
        }

        [Fact]
        public async Task ExpenseReads_eagerLoadTheMerchant_soDtosCanShowNameAndWebsite()
        {
            var user = User.Create("auth0|merchant-test");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var table = ExpenseTable.Create("Test Table", user.Id);
            _context.ExpenseTables.Add(table);
            await _context.SaveChangesAsync();

            var tesco = await _repository.GetByNormalizedNameAsync("tesco");
            var expenseRepository = new ExpenseRepository(_context);
            await expenseRepository.AddAsync(
                Expense.Create(12m, ExpenseCategory.Food, "Groceries", DateTime.UtcNow, table.Id, tesco!.Id),
                CancellationToken.None);

            var loaded = (await expenseRepository.GetAllForTableAsync(table.Id)).Single();

            loaded.Merchant.Should().NotBeNull("the DTO projection reads expense.Merchant.Name");
            loaded.Merchant!.Name.Should().Be("Tesco");
            loaded.Merchant.Website.Should().Be("tesco.com");
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}
