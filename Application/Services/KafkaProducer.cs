using Confluent.Kafka;

namespace Application.Services;

public class KafkaProducer
{
    private const string BootstrapServers = "localhost:9092"; // Update with your Kafka broker address
    private const string Topic = "my-topic"; // Update with the topic you want to produce to

    public void ProduceMessage(string message)
    {
        ProducerConfig config = new ProducerConfig { BootstrapServers = BootstrapServers };

        using IProducer<Null, string>? producer = new ProducerBuilder<Null, string>(config).Build();
        Task<DeliveryResult<Null, string>>? deliveryReport =
            producer.ProduceAsync(Topic, new Message<Null, string> { Value = message });

        // Wait for the message to be delivered
        deliveryReport.ContinueWith(task =>
        {
            Console.WriteLine(
                $"Message delivered to partition {task.Result.Partition} with offset {task.Result.Offset}");
        });
    }
}