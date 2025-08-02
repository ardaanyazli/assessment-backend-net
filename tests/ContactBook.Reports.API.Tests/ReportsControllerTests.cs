using ContactBook.Reports.API.Controllers;
using ContactBook.Reports.Application.DTOs;
using ContactBook.Reports.Application.Interfaces;
using ContactBook.Reports.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ContactBook.Reports.API.Tests.Controllers;

public class ReportsControllerTests
{
    private readonly Mock<IReportRepository> _mockReportRepository;
    private readonly Mock<IKafkaProducer> _mockKafkaProducer;
    private readonly ReportsController _controller;

    public ReportsControllerTests()
    {
        _mockReportRepository = new Mock<IReportRepository>();
        _mockKafkaProducer = new Mock<IKafkaProducer>();
        _controller = new ReportsController(_mockReportRepository.Object, _mockKafkaProducer.Object);
    }

    [Fact]
    public async Task GetReports_ReturnsOkResult_WhenReportsExist()
    {
        // Arrange
        var reports = new List<Report>
        {
            new() { Id = Guid.NewGuid(), RequestedAt = DateTime.UtcNow, Status = ReportStatus.Completed },
            new() { Id = Guid.NewGuid(), RequestedAt = DateTime.UtcNow, Status = ReportStatus.Requested }
        };

        _mockReportRepository.Setup(x => x.GetReportsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(reports);

        // Act
        var result = await _controller.GetReports(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var reportDtos = okResult.Value.Should().BeAssignableTo<IEnumerable<ReportListDto>>().Subject;
        reportDtos.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetReports_ReturnsEmptyList_WhenNoReportsExist()
    {
        // Arrange
        _mockReportRepository.Setup(x => x.GetReportsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Report>());

        // Act
        var result = await _controller.GetReports(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var reportDtos = okResult.Value.Should().BeAssignableTo<IEnumerable<ReportListDto>>().Subject;
        reportDtos.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReport_ReturnsOkResult_WhenReportExistsAndCompleted()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var report = new Report
        {
            Id = reportId,
            RequestedAt = DateTime.UtcNow,
            Status = ReportStatus.Completed,
            Data = new List<LocationStatistics>
            {
                new("Istanbul", 5, 3),
                new("Ankara", 2, 1)
            }
        };

        _mockReportRepository.Setup(x => x.GetReportAsync(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);

        // Act
        var result = await _controller.GetReport(reportId, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var reportDto = okResult.Value.Should().BeOfType<ReportDto>().Subject;
        reportDto.Id.Should().Be(reportId);
        reportDto.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetReport_ReturnsNotFound_WhenReportDoesNotExist()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        _mockReportRepository.Setup(x => x.GetReportAsync(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report?)null);

        // Act
        var result = await _controller.GetReport(reportId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetReport_ReturnsBadRequest_WhenReportNotCompleted()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var report = new Report
        {
            Id = reportId,
            RequestedAt = DateTime.UtcNow,
            Status = ReportStatus.Requested
        };

        _mockReportRepository.Setup(x => x.GetReportAsync(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);

        // Act
        var result = await _controller.GetReport(reportId, CancellationToken.None);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Report is not ready yet");
    }

    [Fact]
    public async Task RequestReport_ReturnsAccepted_WhenReportCreatedSuccessfully()
    {
        // Arrange
        var report = new Report();
        _mockReportRepository.Setup(x => x.CreateReportAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);

        // Act
        var result = await _controller.RequestReport(CancellationToken.None);

        // Assert
        var acceptedResult = result.Should().BeOfType<AcceptedResult>().Subject;
        acceptedResult.Value.Should().Be("Report reequested sucessfully");
        
        _mockReportRepository.Verify(x => x.CreateReportAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockKafkaProducer.Verify(x => x.PublishAsync("report-requests", It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestReport_CreatesReportWithCorrectProperties()
    {
        // Arrange
        Report? capturedReport = null;
        _mockReportRepository.Setup(x => x.CreateReportAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()))
            .Callback<Report, CancellationToken>((report, _) => capturedReport = report)
            .ReturnsAsync(new Report());

        // Act
        await _controller.RequestReport(CancellationToken.None);

        // Assert
        capturedReport.Should().NotBeNull();
        capturedReport!.Id.Should().NotBe(Guid.Empty);
        capturedReport.RequestedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        capturedReport.Status.Should().Be(ReportStatus.Requested);
    }

    [Theory]
    [InlineData(ReportStatus.Requested)]
    [InlineData(ReportStatus.InProgress)]
    public async Task GetReport_ReturnsBadRequest_ForNonCompletedStatuses(ReportStatus status)
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var report = new Report
        {
            Id = reportId,
            RequestedAt = DateTime.UtcNow,
            Status = status
        };

        _mockReportRepository.Setup(x => x.GetReportAsync(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);

        // Act
        var result = await _controller.GetReport(reportId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}