namespace ExpenseTracker.Infrastructure.Messaging
{
    public class KafkaOptions
    {
        public string BootstrapServers { get; set; } = "localhost:9092";

        public string ExtractionRequestTopic { get; set; } = "receipt.extraction.requested";

        public string ExtractionCompletedTopic { get; set; } = "receipt.extraction.completed";

        /// <summary>
        /// Prefix for the completion consumer's group id. A unique suffix is appended per process so
        /// every API instance receives every completion event — only the instance holding a given
        /// client's socket can push to it, so this must broadcast rather than load-balance.
        /// </summary>
        public string CompletedConsumerGroupPrefix { get; set; } = "expensetracker-api";
    }
}
