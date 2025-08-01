using ContactBook.Reports.Application.DTOs;
using ContactBook.Reports.Application.Interfaces;
using ContactBook.Reports.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContactBook.Reports.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;

        public ReportsController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReports()
        {
            var reports = await _reportRepository.GetReportsAsync();
            var result = reports.Select(r => new ReportListDto
            (
                r.Id,
                r.RequestedAt,
                r.Status.ToString()
            ));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport(Guid id)
        {
            var report = await _reportRepository.GetReportAsync(id);
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
                report.Data.Select(rd=>new LocationStatisticsDto
                (
                    rd.Location,
                    rd.ContactCount,
                    rd.PhoneCount
                )).ToList()
            );

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> RequestReport()
        {
            var report = new Report
            {
                Id = Guid.NewGuid(),
                RequestedAt = DateTime.UtcNow,
                Status = ReportStatus.Requested,
            };

            await _reportRepository.CreateReportAsync(report);

            return Accepted("Report reequested sucessfully");

            //TODO:Add Kafka event publish for report gneration
        }

        


    }
}
