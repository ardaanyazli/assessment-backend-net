using ContactBook.Reports.Application.Interfaces;
using ContactBook.Reports.Domain.Entities;
using ContactBook.Reports.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Reports.Infrastructure.Repository;

public class ReportRepository : IReportRepository
{
    private readonly ReportDbContext _context;

    public ReportRepository(ReportDbContext context)
    {
        _context = context;
    }

    public async Task DeleteReportAsync(Guid reportId,CancellationToken cancellationToken = default)
    {
        var report = await _context.Reports.FindAsync(reportId,cancellationToken) ?? throw new NullReferenceException($"Report with {reportId} not found.");

        _context.Reports.Remove(report);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Report> GetReportAsync(Guid id,CancellationToken cancellationToken = default)
    {
        var report = await _context.Reports.FindAsync(id,cancellationToken) ?? throw new NullReferenceException($"Report with {id} not found.");

        return report;
    }

    public async Task<IList<Report>> GetReportsAsync(CancellationToken cancellationToken = default)
    {

        return await _context.Reports.ToListAsync(cancellationToken);
    }

    public async Task<Report> UpdateReportAsync(Report report,CancellationToken cancellationToken = default)
    {
        _context.Reports.Update(report);

        await _context.SaveChangesAsync(cancellationToken);

        return report;
    }

    public async Task<Report> CreateReportAsync(Report report, CancellationToken cancellationToken = default)
    {
        _context.Reports.Add(report);

        await _context.SaveChangesAsync(cancellationToken);

        return report;
    }

}
