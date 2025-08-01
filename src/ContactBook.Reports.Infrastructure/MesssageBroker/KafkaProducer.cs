using System.Text.Json;
using Confluent.Kafka;
using ContactBook.Reports.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace  ContactBook.Reports.Infrastructure.MesssageBroker;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;

    public KafkaProducer(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };
        
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json }, cancellationToken);
    }
}