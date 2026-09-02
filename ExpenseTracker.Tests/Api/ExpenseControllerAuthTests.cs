using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ExpenseTracker.Tests.Api
{
    /// <summary>
    /// Hosts the real Program, so it pins Database:AutoMigrate off — these tests
    /// assert on authentication and must never reach for a database.
    /// </summary>
    public class AuthTestApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("Database:AutoMigrate", "false");
        }
    }

    public class ExpenseControllerAuthTests : IClassFixture<AuthTestApplicationFactory>
    {
        private readonly HttpClient _client;

        public ExpenseControllerAuthTests(AuthTestApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_WithoutToken_Returns401()
        {
            var response = await _client.GetAsync("/api/expensetable/1/expenses");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AddExpense_WithoutToken_Returns401()
        {
            var response = await _client.PostAsJsonAsync("/api/expensetable/1/expenses", new
            {
                expenseTableId = 1,
                amount = 10m,
                category = "Food",
                description = "Test"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AddExpensesBatch_WithoutToken_Returns401()
        {
            var response = await _client.PostAsJsonAsync("/api/expensetable/1/expenses/batch", new[]
            {
                new { expenseTableId = 1, amount = 10m, category = "Food", description = "Test" }
            });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ExtractReceipt_WithoutToken_Returns401()
        {
            using var content = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent, "file", "receipt.jpg");

            var response = await _client.PostAsync("/api/expensetable/1/expenses/extract-receipt", content);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
