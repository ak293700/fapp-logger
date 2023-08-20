using System.Text;
using Application.Common.Interfaces;
using Confluent.Kafka;
using Domain.Entities.LogEntities;
using FappCommon.Kafka.Config;
using FappCommon.Kafka.Log;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using System.Text.Json;

namespace Application.Services;

public class KafkaLogConsumerService : BackgroundService
{
    private readonly KafkaConsumerConfig _config;
    private readonly IServiceScopeFactory _scopeFactory;

    public KafkaLogConsumerService(IServiceScopeFactory scopeFactory, KafkaConsumerConfig config)
    {
        _scopeFactory = scopeFactory;
        _config = config;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // stoppingToken is used from within the Perform method
#pragma warning disable CA2016
        // ReSharper disable once MethodSupportsCancellation
        return Task.Run(() => Perform(stoppingToken));
#pragma warning restore CA2016
    }

    private async Task Perform(CancellationToken stoppingToken)
    {
        IServiceScope scope = _scopeFactory.CreateScope();
        using IApplicationDbContext context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        
        JsonDeserializer<KafkaLogMessage> deserializer = new JsonDeserializer<KafkaLogMessage>();
        using IConsumer<Ignore, KafkaLogMessage>? consumer = new ConsumerBuilder<Ignore, KafkaLogMessage>(_config.ConsumerConfig)
            .SetValueDeserializer(deserializer)
            .Build();
        consumer.Subscribe(_config.Topic);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                ConsumeResult<Ignore, KafkaLogMessage>? consumeResult = consumer.Consume(stoppingToken);
                if (consumeResult is null)
                    continue;
                
                KafkaLogMessage message = consumeResult.Message.Value;

                Log log = new Log
                {
                    Template = message.Template,
                    Level = message.LogLevel,
                    Timestamp = message.Timespan,
                    Data = BsonDocument.Parse(message.Data)
                };
                
                await context.Logs.InsertOneAsync(log, cancellationToken: stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown
            }
            catch (Exception)
            {
                // ignore
            }
        }
    }
}

public class JsonDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
        {
            return default!;
        }

        string json = Encoding.UTF8.GetString(data.ToArray());
        return JsonSerializer.Deserialize<T>(json)!;
    }
}