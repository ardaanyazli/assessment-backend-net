using ContactBook.Reports.Domain.Entities;
using ContactBook.Reports.Infrastructure.Persistence;
using ContactBook.Reports.Infrastructure.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Infrastructure.Tests.Repository;

public class ReportRepositoryTests : IDisposable
{
    private readonly ReportDbContext _context;
    private readonly ReportRepository _repository;

    public ReportRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ReportDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ReportDbContext(options);
        _repository = new ReportRepository(_context);
    }

    [Fact]
    public async Task GetReportsAsync_ReturnsAllReports()
    {
        // Arrange
        var reports = new List<Report>
        {
            new() { Id = Guid.NewGuid(), RequestedAt = DateTime.UtcNow, Status = ReportStatus.Completed },
            new() { Id = Guid.NewGuid(), RequestedAt = DateTime.UtcNow, Status = ReportStatus.Requested }
        };

        await _context.Reports.AddRangeAsync(reports);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetReportsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.Status == ReportStatus.Completed);
        result.Should().Contain(r => r.Status == ReportStatus.Requested);
    }

    [Fact]
    public async Task GetReportAsync_ReturnsReport_WhenExists()
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

        await _context.Reports.AddAsync(report);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetReportAsync(reportId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(reportId);
        result.Status.Should().Be(ReportStatus.Completed);
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetReportAsync_ThrowsException_WhenNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _repository.GetReportAsync(nonExistentId));
    }

    [Fact]
    public async Task CreateReportAsync_AddsReportToDatabase()
    {
        // Arrange
        var report = new Report 
        { 
            Id = Guid.NewGuid(), 
            RequestedAt = DateTime.UtcNow, 
            Status = ReportStatus.Requested 
        };

        // Act
        var result = await _repository.CreateReportAsync(report);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(report.Id);
        
        var savedReport = await _context.Reports.FindAsync(report.Id);
        savedReport.Should().NotBeNull();
        savedReport!.Status.Should().Be(ReportStatus.Requested);
    }

    [Fact]
    public async Task UpdateReportAsync_ModifiesExistingReport()
    {
        // Arrange
        var report = new Report 
        { 
            Id = Guid.NewGuid(), 
            RequestedAt = DateTime.UtcNow, 
            Status = ReportStatus.Requested 
        };
        
        await _context.Reports.AddAsync(report);
        await _context.SaveChangesAsync();

        // Modify the report
        report.Status = ReportStatus.Completed;
        report.Data = new List<LocationStatistics>
        {
            new("Test Location", 1, 1)
        };

        // Act
        var result = await _repository.UpdateReportAsync(report);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ReportStatus.Completed);
        result.Data.Should().HaveCount(1);

        var updatedReport = await _context.Reports.FindAsync(report.Id);
        updatedReport.Should().NotBeNull();
        updatedReport!.Status.Should().Be(ReportStatus.Completed);
    }

    [Fact]
    public async Task DeleteReportAsync_RemovesReportFromDatabase()
    {
        // Arrange
        var report = new Report 
        { 
            Id = Guid.NewGuid(), 
            RequestedAt = DateTime.UtcNow, 
            Status = ReportStatus.Completed 
        };
        
        await _context.Reports.AddAsync(report);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteReportAsync(report.Id);

        // Assert
        var deletedReport = await _context.Reports.FindAsync(report.Id);
        deletedReport.Should().BeNull();
    }

    [Fact]
    public async Task DeleteReportAsync_ThrowsException_WhenReportDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _repository.DeleteReportAsync(nonExistentId));
    }

    [Fact]
    public async Task CreateReportAsync_WithComplexData_SavesCorrectly()
    {
        // Arrange
        var report = new Report 
        { 
            Id = Guid.NewGuid(), 
            RequestedAt = DateTime.UtcNow, 
            Status = ReportStatus.Completed,
            Data = new List<LocationStatistics>
            {
                new("Istanbul", 10, 8),
                new("Ankara", 5, 3),
                new("Izmir", 7, 5)
            }
        };

        // Act
        var result = await _repository.CreateReportAsync(report);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Data.Should().Contain(d => d.Location == "Istanbul" && d.ContactCount == 10 && d.PhoneCount == 8);
        
        var savedReport = await _context.Reports.FindAsync(report.Id);
        savedReport.Should().NotBeNull();
        savedReport!.Data.Should().HaveCount(3);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}