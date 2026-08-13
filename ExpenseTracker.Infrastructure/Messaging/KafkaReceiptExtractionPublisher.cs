using Confluent.Kafka;
using ExpenseTracker.Domain.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ExpenseTracker.Infrastructure.Messaging
{
    /// <summary>
    /// Registered as a singleton: Confluent's producer is thread-safe and expensive to construct, and
    /// it batches in the background, so one per process is both correct and faster.
    /// </summary>
    public sealed class KafkaReceiptExtractionPublisher : IReceiptExtractionRequestPublisher, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly KafkaOptions _options;

        public KafkaReceiptExtractionPublisher(IOptions<KafkaOptions> options)
        {
            _options = options.Value;

            _producer = new ProducerBuilder<string, string>(new ProducerConfig
            {
                BootstrapServers = _options.BootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true,
                // Default is 300s. An unreachable broker would otherwise hold the upload request open
                // for five minutes; the user should get a fast, honest failure instead.
                MessageTimeoutMs = 10_000,
            }).Build();
        }

        public async Task PublishAsync(Guid jobId, Uri imageUrl, string contentType, CancellationToken cancellationToken = default)
        {
            var payload = new ReceiptExtractionRequestedMessage(
                jobId,
                imageUrl.ToString(),
                contentType,
                DateTime.UtcNow);

            var message = new Message<string, string>
            {
                Key = jobId.ToString(),
                Value = JsonSerializer.Serialize(payload, ReceiptExtractionMessageSerializer.Options),
            };

            await _producer.ProduceAsync(_options.ExtractionRequestTopic, message, cancellationToken);
        }

        public void Dispose()
        {
            // Give queued messages a chance to reach the broker before the process exits.
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }
    }
}
