using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.ValueObjects;
using System.Globalization;
using System.Text.Json;

namespace ExpenseTracker.Infrastructure.AI
{
    /// <summary>Turns the JSON the AI model returns into a cleaned-up <see cref="ExtractedReceipt"/>.</summary>
    public static class GeminiReceiptParser
    {
        private static readonly string[] AllowedCategories = Enum.GetNames(typeof(ExpenseCategory));

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerOptions.Default)
        {
            PropertyNameCaseInsensitive = true,
        };

        public static ExtractedReceipt Parse(string? json, DateOnly today)
        {
            if (string.IsNullOrWhiteSpace(json))
                return ExtractedReceipt.Empty;

            var raw = Deserialize(json);
            if (raw?.Items is null)
                return ExtractedReceipt.Empty;

            var items = raw.Items
                .Select(item => TryBuildItem(item, today))
                .OfType<ExtractedReceiptItem>()
                .ToList();

            var totalAfterItemDiscounts = items.Sum(i => i.UnitPrice * i.Quantity - i.Discount);
            var receiptDiscount = Math.Min(ToPounds(raw.ReceiptDiscount), totalAfterItemDiscounts);

            return new ExtractedReceipt(items, receiptDiscount);
        }

        private static RawReceipt? Deserialize(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<RawReceipt>(json, JsonOptions);
            }
            catch (JsonException)
            {
                throw new DomainException("Couldn't understand the receipt data returned by the AI model. It may have been truncated by the token limit — try a simpler photo.");
            }
        }

        private static ExtractedReceiptItem? TryBuildItem(RawReceiptItem raw, DateOnly today)
        {
            if (raw.UnitPrice <= 0 || !TryNormalizeCategory(raw.Category, out var category))
                return null;

            var date = DateOnly.TryParse(raw.Date, out var parsedDate) && parsedDate <= today ? parsedDate : today;
            var quantity = raw.Quantity < 1 ? 1 : raw.Quantity;
            var merchant = string.IsNullOrWhiteSpace(raw.Merchant) ? null : ToTitleCase(raw.Merchant);
            var discount = Math.Min(ToPounds(raw.Discount), raw.UnitPrice * quantity);

            return new ExtractedReceiptItem(raw.UnitPrice, category, ToTitleCase(raw.Description), date, quantity, merchant, discount);
        }

        // Receipts print discounts as "-£0.50", so the model may return either sign.
        private static decimal ToPounds(decimal? amount)
        {
            return Math.Round(Math.Abs(amount ?? 0), 2, MidpointRounding.AwayFromZero);
        }

        private static string ToTitleCase(string? value)
        {
            var trimmed = value?.Trim();
            if (string.IsNullOrEmpty(trimmed))
                return string.Empty;

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(trimmed.ToLowerInvariant());
        }

        private static bool TryNormalizeCategory(string? raw, out ExpenseCategory category)
        {
            var name = AllowedCategories.FirstOrDefault(n => string.Equals(n, raw, StringComparison.OrdinalIgnoreCase));
            category = name is null ? default : Enum.Parse<ExpenseCategory>(name);
            return name is not null;
        }

        private class RawReceipt
        {
            public List<RawReceiptItem>? Items { get; set; }
            public decimal? ReceiptDiscount { get; set; }
        }

        private class RawReceiptItem
        {
            public decimal UnitPrice { get; set; }
            public string? Category { get; set; }
            public string? Description { get; set; }
            public string? Date { get; set; }
            public int Quantity { get; set; }
            public string? Merchant { get; set; }
            public decimal? Discount { get; set; }
        }
    }
}
