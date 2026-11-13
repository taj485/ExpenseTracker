using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ExpenseTracker.Tests.Api
{
    /// <summary>
    /// PL-006 acceptance tests for the scan and the batch save. The AI model is replaced by what it
    /// reads from each scenario's receipt; the persistence ports are mocks that record what is saved.
    /// </summary>
    public class SingleItemReceiptDiscountApiTests
    {
        private const int HomeId = 1;
        private static readonly DateOnly Today = new(2026, 11, 12);

        private readonly Mock<IReceiptExtractionService> _extraction = new();
        private readonly Mock<IExpenseWriter> _expenseWriter = new();
        private readonly Mock<IReceiptWriter> _receiptWriter = new();
        private readonly Mock<IExpenseTableReader> _expenseTableReader = new();
        private readonly Mock<ICurrentUserProvider> _currentUserProvider = new();
        private readonly Mock<IMerchantResolver> _merchantResolver = new();
        private readonly List<Expense> _savedExpenses = new();
        private readonly List<Receipt> _savedReceipts = new();

        public SingleItemReceiptDiscountApiTests()
        {
            _currentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(User.Create("auth0|alex"));
            _expenseTableReader.Setup(x => x.IsMemberAsync(HomeId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _expenseWriter.Setup(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()))
                .Callback<Expense, CancellationToken>((e, _) => _savedExpenses.Add(e))
                .ReturnsAsync(() => _savedExpenses.Count);
            _receiptWriter.Setup(x => x.AddAsync(It.IsAny<Receipt>(), It.IsAny<CancellationToken>()))
                .Callback<Receipt, CancellationToken>((r, _) => _savedReceipts.Add(r))
                .ReturnsAsync(() => _savedReceipts.Count);
        }

        private HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Database:AutoMigrate", "false");
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication()
                        .AddScheme<AuthenticationSchemeOptions, TestSubjectAuthenticationHandler>(TestSubjectAuthenticationHandler.SchemeName, _ => { });
                    services.PostConfigure<AuthenticationOptions>(options =>
                    {
                        options.DefaultScheme = TestSubjectAuthenticationHandler.SchemeName;
                        options.DefaultAuthenticateScheme = TestSubjectAuthenticationHandler.SchemeName;
                        options.DefaultChallengeScheme = TestSubjectAuthenticationHandler.SchemeName;
                    });

                    services.AddScoped(_ => _extraction.Object);
                    services.AddScoped(_ => _expenseWriter.Object);
                    services.AddScoped(_ => _receiptWriter.Object);
                    services.AddScoped(_ => _expenseTableReader.Object);
                    services.AddScoped(_ => _currentUserProvider.Object);
                    services.AddScoped(_ => _merchantResolver.Object);
                });
            });

            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Add(TestSubjectAuthenticationHandler.HeaderName, "auth0|alex");
            return client;
        }

        private static ExtractedReceiptItem Line(string description, decimal unitPrice, decimal discount = 0) =>
            new(unitPrice, ExpenseCategory.Food, description, Today, 1, "Tesco", discount);

        /// <summary>Scans a photo the model reads as <paramref name="receipt"/>; returns the items, discount and receipt discount.</summary>
        private async Task<List<(string Description, decimal Discount, decimal ReceiptDiscount)>> Scan(ExtractedReceipt receipt)
        {
            _extraction.Setup(x => x.ExtractAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(receipt);

            var photo = new ByteArrayContent(new byte[] { 1, 2, 3 });
            photo.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            var form = new MultipartFormDataContent { { photo, "file", "receipt.jpg" } };

            var response = await CreateClient().PostAsync($"/api/expensetable/{HomeId}/expenses/extract-receipt", form);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return json.RootElement.EnumerateArray()
                .Select(e => (e.GetProperty("description").GetString()!, e.GetProperty("discount").GetDecimal(), e.GetProperty("receiptDiscount").GetDecimal()))
                .ToList();
        }

        private static object BatchItem(string description, decimal unitPrice, decimal discount = 0) => new
        {
            expenseTableId = HomeId,
            unitPrice,
            category = "Food",
            description,
            date = "2026-11-12T00:00:00Z",
            merchant = "Tesco",
            quantity = 1,
            discount,
            unit = "Item",
        };

        private Task<HttpResponseMessage> SendBatch(decimal receiptDiscount, params object[] items) =>
            CreateClient().PostAsJsonAsync($"/api/expensetable/{HomeId}/expenses/batch", new
            {
                expenseTableId = HomeId,
                items,
                receiptDiscount,
            });

        [Fact]
        public async Task S1_scan_of_one_item_with_only_an_item_discount_keeps_it_on_the_item()
        {
            var items = await Scan(new ExtractedReceipt(new[] { Line("Coffee Beans", 6.00m, 0.50m) }));

            items.Should().ContainSingle();
            items[0].Should().Be(("Coffee Beans", 0.50m, 0m));
        }

        [Fact]
        public async Task S2_scan_of_one_item_moves_a_coupon_onto_the_item()
        {
            var items = await Scan(new ExtractedReceipt(new[] { Line("Coffee Beans", 6.00m) }, 1.00m));

            items.Should().ContainSingle();
            items[0].Should().Be(("Coffee Beans", 1.00m, 0m));
        }

        [Fact]
        public async Task S3_scan_of_one_item_adds_the_item_discount_and_the_coupon()
        {
            var items = await Scan(new ExtractedReceipt(new[] { Line("Coffee Beans", 6.00m, 0.50m) }, 1.00m));

            items.Should().ContainSingle();
            items[0].Should().Be(("Coffee Beans", 1.50m, 0m));
        }

        [Fact]
        public async Task S4_scan_of_one_item_never_combines_discounts_beyond_the_line_total()
        {
            // The parser already caps the £5.00 coupon at what is left after the £1.00 price cut.
            var items = await Scan(new ExtractedReceipt(new[] { Line("Bread", 3.00m, 1.00m) }, 2.00m));
            items[0].Should().Be(("Bread", 3.00m, 0m));

            // Even if a reading somehow exceeds the line total, the combined discount is capped.
            var overRead = await Scan(new ExtractedReceipt(new[] { Line("Bread", 3.00m, 1.00m) }, 5.00m));
            overRead[0].Should().Be(("Bread", 3.00m, 0m));
        }

        [Fact]
        public async Task S8_scan_of_two_items_keeps_the_receipt_discount_separate()
        {
            var items = await Scan(new ExtractedReceipt(new[] { Line("Milk", 1.50m, 0.25m), Line("Eggs", 2.80m) }, 1.00m));

            items.Should().Equal(("Milk", 0.25m, 1.00m), ("Eggs", 0m, 1.00m));
        }

        [Fact]
        public async Task S9_batch_moves_a_receipt_discount_onto_a_lone_item()
        {
            var response = await SendBatch(1.00m, BatchItem("Coffee Beans", 6.00m, 0.50m));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            _savedExpenses.Should().ContainSingle().Which.Discount.Should().Be(1.50m);
            _savedReceipts.Should().ContainSingle().Which.Discount.Should().Be(0m);
        }

        [Fact]
        public async Task S9_batch_with_one_item_still_rejects_a_receipt_discount_over_the_receipt_total()
        {
            var response = await SendBatch(2.00m, BatchItem("Bread", 2.00m, 0.50m));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            json.RootElement.GetProperty("error").GetString().Should().Be("Receipt discount can't be more than the receipt total (£1.50).");
            _savedExpenses.Should().BeEmpty();
            _savedReceipts.Should().BeEmpty();
        }

        [Fact]
        public async Task S9_batch_with_two_items_keeps_the_receipt_discount_on_the_receipt()
        {
            var response = await SendBatch(1.00m, BatchItem("Milk", 1.50m, 0.25m), BatchItem("Eggs", 2.80m));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            _savedReceipts.Should().ContainSingle().Which.Discount.Should().Be(1.00m);
            _savedExpenses.Select(e => e.Discount).Should().Equal(0.25m, 0m);
        }
    }
}
