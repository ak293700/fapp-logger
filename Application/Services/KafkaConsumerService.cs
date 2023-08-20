using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private const string BootstrapServers = "localhost:9092"; // Update with your Kafka broker address
    private const string GroupId = "my-consumer-group"; // Update with your consumer group ID
    private const string Topic = "my-topic"; // Update with the topic you want to consume from

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger)
    {
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // stoppingToken is used from within the Perform method
#pragma warning disable CA2016
        // ReSharper disable once MethodSupportsCancellation
        return Task.Run(() => Perform(stoppingToken));
#pragma warning restore CA2016
    }

    private void Perform(CancellationToken stoppingToken)
    {
        ConsumerConfig config = new ConsumerConfig
        {
            BootstrapServers = BootstrapServers,
            GroupId = GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using IConsumer<Ignore, string>? consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(Topic);

        _logger.LogInformation("Consumer subscribed to topic: {Topic}", Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                ConsumeResult<Ignore, string>? consumeResult = consumer.Consume(stoppingToken);
                _logger.LogInformation("Consumed message: {MessageValue}", consumeResult.Message.Value);
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming message");
            }
        }

        Console.WriteLine("Consumer disconnected");
    }
}