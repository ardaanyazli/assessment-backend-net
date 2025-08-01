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
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = "report-worker",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("report-requests");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var reportMessage = JsonSerializer.Deserialize<Guid>(result.Message.Value);

                await HandleReportGeneration(reportMessage, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming Kafka message.");
            }
        }
    }

    private async Task HandleReportGeneration(Guid reportId, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var contactDb = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();
        var reportDb = scope.ServiceProvider.GetRequiredService<ReportDbContext>();

        try
        {
            var locations = await contactDb.ContactInfos
                .Where(ci => ci.InfoType == ContactInfoType.Location)
                .ToListAsync(cancellationToken);

            var reportData = locations
                .GroupBy(ci => ci.Value)
                .Select(g =>
                {
                    var location = g.Key;
                    var contactIds = g.Select(ci => ci.ContactId).Distinct().ToList();

                    var phoneCount = contactDb.ContactInfos
                        .Count(ci => ci.InfoType == ContactInfoType.Phone && contactIds.Contains(ci.ContactId));

                    return new LocationStatistics(location, contactIds.Count, phoneCount);
                }).ToList();

            var report = await reportDb.Reports.FindAsync([reportId], cancellationToken);

            if (report != null)
            {
                report.Status = ReportStatus.Completed;
                report.Data = reportData;
                await reportDb.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Report {ReportId} generated.", report.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate report.");
        }
    }
}
