using Confluent.Kafka;
using ExpenseTracker.Application.Queries.GetReceiptExtractionStatus;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Infrastructure.Messaging;
using ExpenseTrackerAPI.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ExpenseTrackerAPI.Messaging
{
    /// <summary>
    /// Consumes analyser completion events and pushes the finished extraction to whoever is waiting
    /// on it over SignalR.
    /// </summary>
    /// <remarks>
    /// The consumer group id is unique per process on purpose. Only the instance holding a given
    /// client's socket can push to that client, so every instance has to see every event — a shared
    /// group id would load-balance events and silently drop pushes for clients connected elsewhere.
    /// That is also why this needs no SignalR backplane at current scale.
    /// </remarks>
    public class ReceiptExtractionCompletedConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<ReceiptExtractionHub> _hub;
        private readonly ILogger<ReceiptExtractionCompletedConsumer> _logger;
        private readonly KafkaOptions _options;

        public ReceiptExtractionCompletedConsumer(
            IServiceScopeFactory scopeFactory,
            IHubContext<ReceiptExtractionHub> hub,
            ILogger<ReceiptExtractionCompletedConsumer> logger,
            IOptions<KafkaOptions> options)
        {
            _scopeFactory = scopeFactory;
            _hub = hub;
            _logger = logger;
            _options = options.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Consume() blocks, so keep it off the thread pool.
            return Task.Factory.StartNew(
                () => ConsumeLoop(stoppingToken),
                stoppingToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default).Unwrap();
        }

        private async Task ConsumeLoop(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _options.BootstrapServers,
                GroupId = $"{_options.CompletedConsumerGroupPrefix}-{Guid.NewGuid()}",
                // Only pushes for clients connected right now matter; replaying old jobs would be noise.
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnableAutoCommit = true,
                AllowAutoCreateTopics = true,
            };

            using var consumer = new ConsumerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => _logger.LogWarning("Kafka consumer error: {Reason}", e.Reason))
                .Build();

            try
            {
                consumer.Subscribe(_options.ExtractionCompletedTopic);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(stoppingToken);

                        if (result?.Message?.Value is null)
                            continue;

                        await HandleAsync(result.Message.Value, stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        // A push that cannot be delivered must never take the host down — the client's
                        // fallback poll still resolves the job.
                        _logger.LogError(ex, "Failed to handle a receipt extraction completion event.");
                    }
                }
            }
            finally
            {
                consumer.Close();
            }
        }

        private async Task HandleAsync(string payload, CancellationToken cancellationToken)
        {
            var message = JsonSerializer.Deserialize<ReceiptExtractionCompletedMessage>(
                payload, ReceiptExtractionMessageSerializer.Options);

            if (message is null || message.JobId == Guid.Empty)
            {
                _logger.LogWarning("Discarding unreadable completion event: {Payload}", payload);
                return;
            }

            using var scope = _scopeFactory.CreateScope();

            var jobReader = scope.ServiceProvider.GetRequiredService<IReceiptExtractionJobReader>();
            var job = await jobReader.GetByIdAsync(message.JobId, cancellationToken);

            // Not our job — another API instance or environment published it against a shared broker.
            if (job is null)
                return;

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            // Same query the fallback poll uses, so push and poll can never disagree. UserId is passed
            // explicitly because there is no HTTP principal on a background thread.
            var status = await mediator.Send(new GetReceiptExtractionStatusQuery(message.JobId, job.UserId), cancellationToken);

            await _hub.Clients
                .Group(ReceiptExtractionHub.GroupFor(message.JobId))
                .SendAsync(ReceiptExtractionHub.CompletedMethod, status, cancellationToken);

            _logger.LogInformation("Pushed extraction {JobId} ({Status}) to subscribers.", message.JobId, status.Status);
        }
    }
}
