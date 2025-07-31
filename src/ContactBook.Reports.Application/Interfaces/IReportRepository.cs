using ContactBook.Reports.Domain.Entities;

namespace ContactBook.Reports.Application.Interfaces;

public interface IReportRepository
{
    Task<IEnumerable<Report>> GetReportsAsync();
    Task<Report> GetReportAsync(Guid id);
    Task<Report> UpdateReport(Report report);
    Task DeleteReport(Guid reportId);
}
