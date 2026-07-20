using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.ValueObjects;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;
using GenAIType = Google.GenAI.Types.Type;

namespace ExpenseTracker.Infrastructure.AI
{
    public class GeminiReceiptExtractionService : IReceiptExtractionService
    {
        private static readonly string[] AllowedCategories = Enum.GetNames(typeof(ExpenseCategory));

        private static readonly JsonSerializerOptions RawItemJsonOptions = new(JsonSerializerOptions.Default)
        {
            PropertyNameCaseInsensitive = true,
        };

        private const string PromptText =
            "You are analyzing a photo of a purchase receipt. Extract every purchased line item. " +
            "For each item, report its unit price (not the line total for that item), its category, " +
            "a short description, the purchase date if visible on the receipt, and the quantity purchased. " +
            "Also identify the shop or merchant name printed on the receipt (e.g. the store name at the top), " +
            "and repeat that exact same merchant name on every single extracted item — all items come from the same receipt.";

        private readonly GeminiOptions _options;

        public GeminiReceiptExtractionService(IOptions<GeminiOptions> options)
        {
            _options = options.Value;
        }

        public async Task<IReadOnlyList<ExtractedReceiptItem>> ExtractAsync(byte[] imageBytes, string contentType, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
                throw new InvalidOperationException("Gemini API key is not configured.");

            var client = new Client(apiKey: _options.ApiKey);

            var contents = new List<Content>
            {
                new Content
                {
                    Role = "user",
                    Parts = new List<Part>
                    {
                        new Part { Text = PromptText },
                        new Part { InlineData = new Blob { Data = imageBytes, MimeType = contentType } },
                    },
                },
            };

            var tokenCount = await client.Models.CountTokensAsync(model: _options.Model, contents: contents);
            if ((tokenCount.TotalTokens ?? 0) > _options.MaxInputTokens)
                throw new DomainException($"This receipt is too large or complex to process (exceeds the {_options.MaxInputTokens}-token input limit). Try a clearer or simpler photo.");

            var config = new GenerateContentConfig
            {
                ResponseMimeType = "application/json",
                ResponseSchema = BuildResponseSchema(),
                MaxOutputTokens = _options.MaxOutputTokens,
            };

            var response = await client.Models.GenerateContentAsync(model: _options.Model, contents: contents, config: config);

            var candidate = response.Candidates?.FirstOrDefault();
            if (candidate?.FinishReason == FinishReason.MaxTokens)
                throw new DomainException($"The extracted receipt data exceeded the {_options.MaxOutputTokens}-token output limit. Try a receipt with fewer line items.");

            var text = candidate?.Content?.Parts?.FirstOrDefault()?.Text;
            if (string.IsNullOrWhiteSpace(text))
                return Array.Empty<ExtractedReceiptItem>();

            List<RawReceiptItem>? rawItems;
            try
            {
                rawItems = JsonSerializer.Deserialize<List<RawReceiptItem>>(text, RawItemJsonOptions);
            }
            catch (JsonException)
            {
                throw new DomainException("Couldn't understand the receipt data returned by the AI model. It may have been truncated by the token limit — try a simpler photo.");
            }

            if (rawItems is null)
                return Array.Empty<ExtractedReceiptItem>();

            var results = new List<ExtractedReceiptItem>();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            foreach (var raw in rawItems)
            {
                if (raw.Amount <= 0)
                    continue;

                if (!TryNormalizeCategory(raw.Category, out var category))
                    continue;

                var date = DateOnly.TryParse(raw.Date, out var parsedDate) && parsedDate <= today
                    ? parsedDate
                    : today;

                var quantity = raw.Quantity < 1 ? 1 : raw.Quantity;
                var merchant = string.IsNullOrWhiteSpace(raw.Merchant) ? null : ToTitleCase(raw.Merchant);

                results.Add(new ExtractedReceiptItem(raw.Amount, category, ToTitleCase(raw.Description), date, quantity, merchant));
            }

            return results;
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
            foreach (var name in AllowedCategories)
            {
                if (string.Equals(name, raw, StringComparison.OrdinalIgnoreCase))
                {
                    category = Enum.Parse<ExpenseCategory>(name);
                    return true;
                }
            }

            category = default;
            return false;
        }

        private static Schema BuildResponseSchema()
        {
            return new Schema
            {
                Type = GenAIType.Array,
                Items = new Schema
                {
                    Type = GenAIType.Object,
                    Properties = new Dictionary<string, Schema>
                    {
                        ["amount"] = new Schema { Type = GenAIType.Number, Description = "Unit price of the item, not the line total." },
                        ["category"] = new Schema { Type = GenAIType.String, Enum = AllowedCategories.ToList() },
                        ["description"] = new Schema { Type = GenAIType.String },
                        ["date"] = new Schema { Type = GenAIType.String, Format = "date", Description = "Purchase date in YYYY-MM-DD format, if visible on the receipt." },
                        ["quantity"] = new Schema { Type = GenAIType.Integer },
                        ["merchant"] = new Schema { Type = GenAIType.String, Description = "The shop or merchant name printed on the receipt. Use the exact same value for every item on this receipt." },
                    },
                    Required = new List<string> { "amount", "category", "description", "date", "quantity", "merchant" },
                },
            };
        }

        private class RawReceiptItem
        {
            public decimal Amount { get; set; }
            public string? Category { get; set; }
            public string? Description { get; set; }
            public string? Date { get; set; }
            public int Quantity { get; set; }
            public string? Merchant { get; set; }
        }
    }
}
