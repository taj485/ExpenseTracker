using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExpenseTracker.Infrastructure.Messaging
{
    /// <summary>Published by the API, consumed by the Java analyser.</summary>
    public record ReceiptExtractionRequestedMessage(
        Guid JobId,
        string ImageUrl,
        string ContentType,
        DateTime RequestedAtUtc);

    /// <summary>
    /// Published by the Java analyser when a job reaches a terminal state. Carries no items on
    /// purpose — the API fetches those over HTTP so push and poll share one materialisation path.
    /// </summary>
    public record ReceiptExtractionCompletedMessage(
        Guid JobId,
        string Status,
        DateTime CompletedAtUtc);

    public static class ReceiptExtractionMessageSerializer
    {
        /// <summary>camelCase on the wire, since the other end of these topics is Jackson.</summary>
        public static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }
}
