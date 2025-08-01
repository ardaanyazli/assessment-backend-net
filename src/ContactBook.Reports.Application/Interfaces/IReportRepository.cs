using ContactBook.Reports.Domain.Entities;

namespace ContactBook.Reports.Application.Interfaces;

public interface IReportRepository
{
    Task<IList<Report>> GetReportsAsync();
    Task<Report> GetReportAsync(Guid id);
    Task<Report> UpdateReportAsync(Report report);
    Task DeleteReportAsync(Guid reportId);
    Task<Report> CreateReportAsync(Report report);
}
