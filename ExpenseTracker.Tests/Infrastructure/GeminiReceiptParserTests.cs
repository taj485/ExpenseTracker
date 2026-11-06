using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Infrastructure.AI;

namespace ExpenseTracker.Tests.Infrastructure
{
    public class GeminiReceiptParserTests
    {
        private static readonly DateOnly Today = new(2026, 11, 6);

        [Fact]
        public void Parse_ReadsItemsAndDiscounts()
        {
            var json = """
                {"items":[{"unitPrice":2.00,"category":"Food","description":"MILK","date":"2026-11-01","quantity":2,"merchant":"TESCO","discount":-0.5}],
                 "receiptDiscount":1.005}
                """;

            var result = GeminiReceiptParser.Parse(json, Today);

            var item = Assert.Single(result.Items);
            Assert.Equal("Milk", item.Description);
            Assert.Equal("Tesco", item.Merchant);
            Assert.Equal(new DateOnly(2026, 11, 1), item.Date);
            Assert.Equal(0.5m, item.Discount);
            Assert.Equal(1.01m, result.ReceiptDiscount);
        }

        [Fact]
        public void Parse_CapsDiscountsAtTheirBounds()
        {
            var json = """
                {"items":[{"unitPrice":2.00,"category":"Food","description":"a","date":"","quantity":1,"merchant":"","discount":9}],
                 "receiptDiscount":9}
                """;

            var result = GeminiReceiptParser.Parse(json, Today);

            Assert.Equal(2m, result.Items[0].Discount);
            Assert.Equal(0m, result.ReceiptDiscount);
        }

        [Fact]
        public void Parse_SkipsBadItemsAndDefaultsDateQuantityMerchant()
        {
            var json = """
                {"items":[
                  {"unitPrice":0,"category":"Food","description":"free","quantity":1},
                  {"unitPrice":1,"category":"Nonsense","description":"x","quantity":1},
                  {"unitPrice":1,"category":"food","description":"ok","date":"2999-01-01","quantity":0}]}
                """;

            var result = GeminiReceiptParser.Parse(json, Today);

            var item = Assert.Single(result.Items);
            Assert.Equal(ExpenseCategory.Food, item.Category);
            Assert.Equal(Today, item.Date);
            Assert.Equal(1, item.Quantity);
            Assert.Null(item.Merchant);
            Assert.Equal(0m, item.Discount);
            Assert.Equal(0m, result.ReceiptDiscount);
        }

        [Fact]
        public void Parse_ReturnsEmpty_WhenThereAreNoItems()
        {
            Assert.Empty(GeminiReceiptParser.Parse("{}", Today).Items);
            Assert.Empty(GeminiReceiptParser.Parse("null", Today).Items);
            Assert.Empty(GeminiReceiptParser.Parse(" ", Today).Items);
            Assert.Empty(GeminiReceiptParser.Parse(null, Today).Items);
        }

        [Fact]
        public void EnsureWithinInputLimit_ThrowsOnlyAboveTheLimit()
        {
            GeminiReceiptExtractionService.EnsureWithinInputLimit(null, 10);
            GeminiReceiptExtractionService.EnsureWithinInputLimit(10, 10);
            Assert.Throws<DomainException>(() => GeminiReceiptExtractionService.EnsureWithinInputLimit(11, 10));
        }

        [Fact]
        public void Parse_Throws_WhenJsonIsBroken()
        {
            Assert.Throws<DomainException>(() => GeminiReceiptParser.Parse("{\"items\":[", Today));
        }
    }
}
