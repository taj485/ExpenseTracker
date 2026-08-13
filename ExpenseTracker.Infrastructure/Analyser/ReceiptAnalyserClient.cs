using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.ValueObjects;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExpenseTracker.Infrastructure.Analyser
{
    public class ReceiptAnalyserClient : IReceiptExtractionResultReader
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        private readonly HttpClient _http;
        private readonly Auth0TokenProvider _tokenProvider;

        public ReceiptAnalyserClient(HttpClient http, Auth0TokenProvider tokenProvider)
        {
            _http = http;
            _tokenProvider = tokenProvider;
        }

        public async Task<ReceiptExtractionResult?> GetAsync(Guid jobId, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/extractions/{jobId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenProvider.GetAsync(cancellationToken));

            using var response = await _http.SendAsync(request, cancellationToken);

            // The analyser has not seen this job yet — it is still in flight on the topic.
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<ExtractionJobResponse>(JsonOptions, cancellationToken);

            if (payload is null)
                return null;

            var status = Enum.TryParse<ReceiptExtractionStatus>(payload.Status, ignoreCase: true, out var parsed)
                ? parsed
                : ReceiptExtractionStatus.Pending;

            var items = (payload.Items ?? new List<ExtractedItemResponse>())
                .Where(i => Enum.TryParse<ExpenseCategory>(i.Category, ignoreCase: true, out _))
                .Select(i => new ExtractedReceiptItem(
                    i.Amount,
                    Enum.Parse<ExpenseCategory>(i.Category!, ignoreCase: true),
                    i.Description ?? string.Empty,
                    i.Date,
                    i.Quantity,
                    i.Merchant))
                .ToList();

            return new ReceiptExtractionResult(payload.JobId, status, items, payload.Error);
        }

        private sealed class ExtractionJobResponse
        {
            public Guid JobId { get; set; }
            public string? Status { get; set; }
            public List<ExtractedItemResponse>? Items { get; set; }
            public string? Error { get; set; }
        }

        private sealed class ExtractedItemResponse
        {
            public decimal Amount { get; set; }
            public string? Category { get; set; }
            public string? Description { get; set; }

            [JsonConverter(typeof(DateOnlyJsonConverter))]
            public DateOnly Date { get; set; }

            public int Quantity { get; set; }
            public string? Merchant { get; set; }
        }

        /// <summary>Jackson serialises LocalDate as "yyyy-MM-dd"; System.Text.Json needs telling.</summary>
        private sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
        {
            public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                => DateOnly.TryParse(reader.GetString(), out var value) ? value : DateOnly.FromDateTime(DateTime.UtcNow);

            public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
                => writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
        }
    }
}
