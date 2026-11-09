using ExpenseTracker.Domain.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ExpenseTracker.Infrastructure.Auth
{
    /// <summary>
    /// Deletes Auth0 users through the Management API, using a client-credentials token.
    /// </summary>
    public class Auth0IdentityProviderAdmin : IIdentityProviderAdmin
    {
        private readonly HttpClient _httpClient;
        private readonly Auth0ManagementOptions _options;

        public Auth0IdentityProviderAdmin(HttpClient httpClient, IOptions<Auth0ManagementOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task DeleteUserAsync(string subject, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.Domain) || string.IsNullOrWhiteSpace(_options.ClientId)
                || string.IsNullOrWhiteSpace(_options.ClientSecret))
                throw new InvalidOperationException("The Auth0 Management API is not configured.");

            var token = await GetTokenAsync(cancellationToken);

            using var request = new HttpRequestMessage(HttpMethod.Delete,
                $"https://{_options.Domain}/api/v2/users/{Uri.EscapeDataString(subject)}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _httpClient.SendAsync(request, cancellationToken);

            // Already gone, e.g. a retry after a partial failure.
            if (response.StatusCode == HttpStatusCode.NotFound)
                return;

            response.EnsureSuccessStatusCode();
        }

        private async Task<string> GetTokenAsync(CancellationToken cancellationToken)
        {
            using var response = await _httpClient.PostAsJsonAsync($"https://{_options.Domain}/oauth/token", new
            {
                grant_type = "client_credentials",
                client_id = _options.ClientId,
                client_secret = _options.ClientSecret,
                audience = $"https://{_options.Domain}/api/v2/"
            }, cancellationToken);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken);
            if (string.IsNullOrWhiteSpace(body?.AccessToken))
                throw new InvalidOperationException("Auth0 returned no Management API token.");

            return body.AccessToken;
        }

        private sealed class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string? AccessToken { get; set; }
        }
    }
}
