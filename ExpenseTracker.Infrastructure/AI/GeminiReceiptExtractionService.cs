using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.ValueObjects;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Options;
using GenAIType = Google.GenAI.Types.Type;

namespace ExpenseTracker.Infrastructure.AI
{
    public class GeminiReceiptExtractionService : IReceiptExtractionService
    {
        private static readonly string[] AllowedCategories = Enum.GetNames(typeof(ExpenseCategory));

        private const string PromptText =
            "You are analyzing a photo of a purchase receipt. Extract every purchased line item. " +
            "For each item, report its unit price (not the line total for that item), its category, " +
            "a short description, the purchase date if visible on the receipt, and the quantity purchased. " +
            "Also identify the shop or merchant name printed on the receipt (e.g. the store name at the top), " +
            "and repeat that exact same merchant name on every single extracted item — all items come from the same receipt. " +
            "Discounts: never report a discount line (e.g. \"Clubcard Price\", \"Price cut\", \"Coupon\", \"Meal Deal saving\", \"-£0.50\") as an item. " +
            "If a discount line clearly belongs to one single item line (printed directly under or beside it), put its amount as a positive number in that item's discount; " +
            "if one item has several such lines, add them up. The item discount applies to the whole line (quantity x unit price). " +
            "Any discount that covers more than one item (multibuy, \"3 for 2\", meal deal, \"any 2 for £5\", bundle savings), any whole-receipt discount (coupon, staff discount, a savings line not tied to one item), " +
            "and any discount where it is unclear whether it covers one item or several goes into receiptDiscount instead, never onto an item; add several such discounts together. " +
            "Do not count a \"Total savings\" or \"You saved\" summary line that only adds up discounts already reported. " +
            "If a discount is printed only as a percentage, convert it to the pound amount it takes off, rounded to the penny. Use 0 when there is no discount.";

        private readonly GeminiOptions _options;

        public GeminiReceiptExtractionService(IOptions<GeminiOptions> options)
        {
            _options = options.Value;
        }

        public async Task<ExtractedReceipt> ExtractAsync(byte[] imageBytes, string contentType, CancellationToken cancellationToken)
        {
            EnsureApiKeyConfigured(_options.ApiKey);

            var client = new Client(apiKey: _options.ApiKey);

            var contents = BuildContents(imageBytes, contentType);

            var tokenCount = await client.Models.CountTokensAsync(model: _options.Model, contents: contents);
            EnsureWithinInputLimit(tokenCount.TotalTokens, _options.MaxInputTokens);

            var config = new GenerateContentConfig
            {
                ResponseMimeType = "application/json",
                ResponseSchema = BuildResponseSchema(),
                MaxOutputTokens = _options.MaxOutputTokens,
            };

            var response = await client.Models.GenerateContentAsync(model: _options.Model, contents: contents, config: config);

            var text = ReadResponseText(response);

            return GeminiReceiptParser.Parse(text, DateOnly.FromDateTime(DateTime.UtcNow));
        }

        private static void EnsureApiKeyConfigured(string? apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Gemini API key is not configured.");
        }

        public static void EnsureWithinInputLimit(int? totalTokens, int maxInputTokens)
        {
            if ((totalTokens ?? 0) > maxInputTokens)
                throw new DomainException($"This receipt is too large or complex to process (exceeds the {maxInputTokens}-token input limit). Try a clearer or simpler photo.");
        }

        private static List<Content> BuildContents(byte[] imageBytes, string contentType)
        {
            return new List<Content>
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
        }

        private string? ReadResponseText(GenerateContentResponse response)
        {
            var candidate = response.Candidates?.FirstOrDefault();
            if (candidate?.FinishReason == FinishReason.MaxTokens)
                throw new DomainException($"The extracted receipt data exceeded the {_options.MaxOutputTokens}-token output limit. Try a receipt with fewer line items.");

            return candidate?.Content?.Parts?.FirstOrDefault()?.Text;
        }

        private static Schema BuildResponseSchema()
        {
            var itemSchema = new Schema
                {
                    Type = GenAIType.Object,
                    Properties = new Dictionary<string, Schema>
                    {
                        ["unitPrice"] = new Schema { Type = GenAIType.Number, Description = "Unit price of the item, not the line total." },
                        ["category"] = new Schema { Type = GenAIType.String, Enum = AllowedCategories.ToList() },
                        ["description"] = new Schema { Type = GenAIType.String },
                        ["date"] = new Schema { Type = GenAIType.String, Format = "date", Description = "Purchase date in YYYY-MM-DD format, if visible on the receipt." },
                        ["quantity"] = new Schema { Type = GenAIType.Integer },
                        ["merchant"] = new Schema { Type = GenAIType.String, Description = "The shop or merchant name printed on the receipt. Use the exact same value for every item on this receipt." },
                        ["discount"] = new Schema { Type = GenAIType.Number, Description = "Total of the discount lines that clearly belong to this one item only, as a positive pound amount. 0 if none." },
                    },
                    Required = new List<string> { "unitPrice", "category", "description", "date", "quantity", "merchant", "discount" },
                };

            return new Schema
            {
                Type = GenAIType.Object,
                Properties = new Dictionary<string, Schema>
                {
                    ["items"] = new Schema { Type = GenAIType.Array, Items = itemSchema },
                    ["receiptDiscount"] = new Schema { Type = GenAIType.Number, Description = "Sum of whole-receipt and multi-item discounts (coupons, multibuys, meal deals), as a positive pound amount. Excludes item discounts and savings summaries. 0 if none." },
                },
                Required = new List<string> { "items", "receiptDiscount" },
            };
        }
    }
}
