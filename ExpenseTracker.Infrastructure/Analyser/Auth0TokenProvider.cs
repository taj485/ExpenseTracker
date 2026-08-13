using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ExpenseTracker.Infrastructure.Analyser
{
    /// <summary>
    /// Fetches and caches a machine-to-machine access token for calling the receipt analyser.
    /// Singleton so the token is shared process-wide rather than re-minted per request.
    /// </summary>
    public sealed class Auth0TokenProvider : IDisposable
    {
        private readonly HttpClient _http = new();
        private readonly SemaphoreSlim _lock = new(1, 1);
        private readonly ReceiptAnalyserOptions _options;

        private string? _token;
        private DateTimeOffset _expiresAt;

        public Auth0TokenProvider(IOptions<ReceiptAnalyserOptions> options)
        {
            _options = options.Value;
        }

        public async Task<string> GetAsync(CancellationToken cancellationToken = default)
        {
            if (_token is not null && DateTimeOffset.UtcNow < _expiresAt)
                return _token;

            await _lock.WaitAsync(cancellationToken);
            try
            {
                // Another caller may have refreshed while we waited.
                if (_token is not null && DateTimeOffset.UtcNow < _expiresAt)
                    return _token;

                var response = await _http.PostAsJsonAsync(_options.TokenEndpoint, new
                {
                    client_id = _options.ClientId,
                    client_secret = _options.ClientSecret,
                    audience = _options.Audience,
                    grant_type = "client_credentials",
                }, cancellationToken);

                response.EnsureSuccessStatusCode();

                var payload = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken)
                    ?? throw new InvalidOperationException("Auth0 returned an empty token response.");

                _token = payload.AccessToken;
                // Retire a minute early so a token never expires mid-flight.
                _expiresAt = DateTimeOffset.UtcNow.AddSeconds(Math.Max(payload.ExpiresIn - 60, 30));

                return _token;
            }
            finally
            {
                _lock.Release();
            }
        }

        public void Dispose()
        {
            _http.Dispose();
            _lock.Dispose();
        }

        private sealed class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = string.Empty;

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
        }
    }
}
