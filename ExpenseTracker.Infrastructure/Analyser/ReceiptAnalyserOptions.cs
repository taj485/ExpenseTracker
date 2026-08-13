namespace ExpenseTracker.Infrastructure.Analyser
{
    public class ReceiptAnalyserOptions
    {
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>Auth0 client-credentials endpoint, e.g. https://{domain}/oauth/token.</summary>
        public string TokenEndpoint { get; set; } = string.Empty;

        public string ClientId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;
    }
}
