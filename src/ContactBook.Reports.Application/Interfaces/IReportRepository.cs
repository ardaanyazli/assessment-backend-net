using ContactBook.Reports.Domain.Entities;

namespace ContactBook.Reports.Application.Interfaces;

public interface IReportRepository
{
    Task<IList<Report>> GetReportsAsync(CancellationToken cancellationToken = default);
    Task<Report> GetReportAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Report> UpdateReportAsync(Report report,CancellationToken cancellationToken=default); //this will be used from consumer to update report status and data
    Task DeleteReportAsync(Guid reportId, CancellationToken cancellationToken = default);
    Task<Report> CreateReportAsync(Report report, CancellationToken cancellationToken = default);
}
