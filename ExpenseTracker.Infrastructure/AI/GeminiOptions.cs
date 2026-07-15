namespace ExpenseTracker.Infrastructure.AI
{
    public class GeminiOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-2.5-flash";
        public int MaxInputTokens { get; set; } = 100_000;
        public int MaxOutputTokens { get; set; } = 100_000;
    }
}
