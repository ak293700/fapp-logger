using Confluent.Kafka;
using FappCommon.Kafka.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class KafkaLogConsumerService : BackgroundService
{
    private readonly ILogger<KafkaLogConsumerService> _logger;
    private readonly KafkaConsumerConfig _config;

    public KafkaLogConsumerService(KafkaConsumerConfig config, ILogger<KafkaLogConsumerService> logger)
    {
        _config = config;
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
        using IConsumer<Ignore, string>? consumer = new ConsumerBuilder<Ignore, string>(_config.ConsumerConfig).Build();
        consumer.Subscribe(_config.Topic);

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
    }
}