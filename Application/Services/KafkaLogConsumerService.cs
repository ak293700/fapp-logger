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
using Confluent.Kafka.Admin;

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

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        KeyValuePair<string, string>[] config =
        {
            new("bootstrap.servers", _config.BootstrapServers)
        };

        // Create the topic if it does not exist
        using IAdminClient adminClient = new AdminClientBuilder(config).Build();

        try
        {
            await adminClient.CreateTopicsAsync(new List<TopicSpecification>
            {
                new()
                {
                    Name = _config.Topic,
                    ReplicationFactor = 1,
                    NumPartitions = 1
                }
            }, new CreateTopicsOptions { RequestTimeout = TimeSpan.FromSeconds(10) });
        }
        catch (CreateTopicsException e)
        {
            if (e.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
            {
                throw;
            }
        }

        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // stoppingToken is used from within the Perform method
#pragma warning disable CA2016
        // ReSharper disable once MethodSupportsCancellation
        return Task.Run(() => Init(stoppingToken));
#pragma warning restore CA2016
    }

    private async Task Init(CancellationToken stoppingToken)
    {
        IServiceScope scope = _scopeFactory.CreateScope();
        using LogService context = scope.ServiceProvider.GetRequiredService<LogService>();

        JsonDeserializer<KafkaLogMessage> deserializer = new JsonDeserializer<KafkaLogMessage>();
        using IConsumer<Ignore, KafkaLogMessage>? consumer =
            new ConsumerBuilder<Ignore, KafkaLogMessage>(_config.ConsumerConfig)
                .SetValueDeserializer(deserializer)
                .Build();
        consumer.Subscribe(_config.Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConsumeOnce(consumer, context, stoppingToken);
            }
            catch (Exception)
            {
                // ignore
            }
        }
    }

    private async Task ConsumeOnce(IConsumer<Ignore, KafkaLogMessage> consumer, LogService logService,
        CancellationToken stoppingToken)
    {
        ConsumeResult<Ignore, KafkaLogMessage>? consumeResult = consumer.Consume(stoppingToken);
        if (consumeResult is null)
            return;

        await logService.Create(consumeResult.Message.Value, stoppingToken);
    }
}

file class JsonDeserializer<T> : IDeserializer<T>
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