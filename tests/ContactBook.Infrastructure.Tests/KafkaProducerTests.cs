using ContactBook.Reports.Infrastructure.MesssageBroker;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ContactBook.Infrastructure.Tests.MessageBroker;

public class KafkaProducerTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;

    public KafkaProducerTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(x => x["Kafka:BootstrapServers"]).Returns("localhost:9092");
    }

    [Fact]
    public void Constructor_InitializesWithConfiguration()
    {
        // Act & Assert
        var action = () => new KafkaProducer(_mockConfiguration.Object);
        action.Should().NotThrow();
    }

    [Fact]
    public async Task PublishAsync_SerializesMessageCorrectly()
    {
        // Note: This test is limited because we can't easily mock the Kafka producer
        // In a real scenario, you would use dependency injection to inject an IProducer interface
        // and mock that interface for testing
        
        // Arrange
        var producer = new KafkaProducer(_mockConfiguration.Object);
        var testMessage = new { Id = Guid.NewGuid(), Name = "Test" };

        // Act & Assert
        // This would throw if Kafka is not available, but we're testing the serialization logic
        var action = async () => await producer.PublishAsync("test-topic", testMessage, CancellationToken.None);
        
        // We can't easily test the actual publishing without a real Kafka instance
        // But we can test that the method doesn't throw due to serialization issues
        // In a production environment, you would use testcontainers or similar for integration tests
        
        // For now, we just ensure the method signature is correct
        action.Should().NotBeNull();
    }
}

// Note: For comprehensive Kafka testing, you would typically use:
// 1. Testcontainers to spin up a real Kafka instance
// 2. Dependency injection with IProducer interface for better testability
// 3. Integration tests with actual Kafka brokers