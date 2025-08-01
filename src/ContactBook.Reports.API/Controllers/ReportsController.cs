using ContactBook.Reports.Application.DTOs;
using ContactBook.Reports.Application.Interfaces;
using ContactBook.Reports.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ContactBook.Reports.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportRepository _reportRepository;
    private readonly IKafkaProducer _kafkaProducer;
    public ReportsController(IReportRepository reportRepository, IKafkaProducer kafkaProducer)
    {
        _reportRepository = reportRepository;
        _kafkaProducer = kafkaProducer;
    }

    [HttpGet]
    public async Task<IActionResult> GetReports(CancellationToken cancellationToken)
    {
        var reports = await _reportRepository.GetReportsAsync(cancellationToken);
        var result = reports.Select(r => new ReportListDto
        (
            r.Id,
            r.RequestedAt,
            r.Status.ToString()
        ));

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReport(Guid id, CancellationToken cancellationToken)
    {
        var report = await _reportRepository.GetReportAsync(id, cancellationToken);
        if (report == null)
        {
            return NotFound();
        }
        if (report.Status != ReportStatus.Completed)
        {
            return BadRequest("Report is not ready yet");
        }

        var result = new ReportDto
        (
            report.Id,
            report.Data.Select(rd => new LocationStatisticsDto
            (
                rd.Location,
                rd.ContactCount,
                rd.PhoneCount
            )).ToList()
        );

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> RequestReport(CancellationToken cancellationToken)
    {
        var report = new Report
        {
            Id = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow,
            Status = ReportStatus.Requested,
        };

        await _reportRepository.CreateReportAsync(report, cancellationToken);
        await _kafkaProducer.PublishAsync("report-requests", report.Id, cancellationToken);

        return Accepted("Report reequested sucessfully");
    }
}
