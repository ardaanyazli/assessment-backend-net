using System.Text.Json;
using Confluent.Kafka;
using ContactBook.Contacts.Domain.Entities;
using ContactBook.Contacts.Infrastructure.Persistence;
using ContactBook.Reports.Domain.Entities;
using ContactBook.Reports.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Reports.Consumer;

public class ReportWorker : BackgroundService
{
    private readonly ILogger<ReportWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public ReportWorker(ILogger<ReportWorker> logger, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var bootstrapServers = _configuration["Kafka:BootstrapServers"];
        var topic = _configuration["Kafka:ReportRequestTopic"] ?? "report-requests";

        if (string.IsNullOrWhiteSpace(bootstrapServers))
        {
            _logger.LogCritical("Kafka bootstrap server is not configured.");
            return;
        }

        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = _configuration["Kafka:ConsumerGroupId"] ?? "contactbook-report-worker",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false, // Manual commit after processing,
            // Debug = "all",

        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(topic);

        _logger.LogInformation("Kafka consumer subscribed to topic: {Topic}", topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(TimeSpan.FromSeconds(5));
                    if (result == null)
                    {
                        _logger.LogInformation("No message received, waiting for next iteration.");
                        continue; // No message received, continue to next iteration
                    }

                    _logger.LogInformation("Received message at offset {Offset}: {Message}", result.Offset, result.Message.Value);

                    if (!TryDeserialize(result.Message.Value, out Guid reportId))
                    {
                        _logger.LogWarning("Skipping invalid message: {RawMessage}", result.Message.Value);
                        continue;
                    }

                    await HandleReportGeneration(reportId, stoppingToken);

                    consumer.Commit(result); // ✅ Manual commit after successful handling
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while processing Kafka message.");
                }
            }
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("Kafka consumer closed.");
        }
    }

    private bool TryDeserialize(string message, out Guid reportId)
    {
        try
        {
            reportId = JsonSerializer.Deserialize<Guid>(message);
            _logger.LogInformation("Deserialized report ID: {ReportId}", reportId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize message: {Message}", message);
            reportId = Guid.Empty;
            return false;
        }
    }

    private async Task HandleReportGeneration(Guid reportId, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var contactDb = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();
        var reportDb = scope.ServiceProvider.GetRequiredService<ReportDbContext>();

        try
        {
            var locationInfos = await contactDb.ContactInfos
                .Where(ci => ci.InfoType == ContactInfoType.Location)
                .ToListAsync(cancellationToken);

            var phoneInfos = await contactDb.ContactInfos
                .Where(ci => ci.InfoType == ContactInfoType.Phone)
                .ToListAsync(cancellationToken);

            var reportData = locationInfos
                .GroupBy(ci => ci.Value)
                .Select(g =>
                {
                    var location = g.Key;
                    var contactIds = g.Select(ci => ci.ContactId).Distinct().ToList();
                    var phoneCount = phoneInfos.Count(ci => contactIds.Contains(ci.ContactId));

                    return new LocationStatistics(location, contactIds.Count, phoneCount);
                })
                .ToList();

            var report = await reportDb.Reports.FindAsync([reportId], cancellationToken);

            if (report == null)
            {
                _logger.LogWarning("Report with ID {ReportId} not found.", reportId);
                return;
            }

            report.Status = ReportStatus.Completed;
            report.Data = reportData;

            await reportDb.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Report {ReportId} generated and saved successfully.", report.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report for ID {ReportId}", reportId);
        }
    }
}