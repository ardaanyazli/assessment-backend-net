using ContactBook.Reports.Application.Interfaces;
using ContactBook.Reports.Domain.Entities;
using ContactBook.Reports.Infrastructure.Persistence;

namespace ContactBook.Reports.Infrastructure.Repository;

public class ReportRepository : IReportRepository
{
    private readonly ReportDbContext _context;

    public ReportRepository(ReportDbContext context)
    {
        _context = context;
    }

    public async Task DeleteReportAsync(Guid reportId)
    {
        var report = await _context.Reports.FindAsync(reportId) ?? throw new NullReferenceException($"Report with {reportId} not found.");

        _context.Reports.Remove(report);

        await _context.SaveChangesAsync();
    }

    public async Task<Report> GetReportAsync(Guid id)
    {
        var report = await _context.Reports.FindAsync(id) ?? throw new NullReferenceException($"Report with {id} not found.");

        return report;
    }

    public async Task<IList<Report>> GetReportsAsync()
    {
        return await Task.FromResult(_context.Reports.ToList());
    }

    public async Task<Report> UpdateReportAsync(Report report)
    {
        _context.Reports.Update(report);
        await _context.SaveChangesAsync();

        return report;
    }

    public async Task<Report> CreateReportAsync(Report report)
    {
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        return report;
    }

}
