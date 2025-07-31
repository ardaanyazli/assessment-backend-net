namespace ContactBook.Reports.Domain.Entities;

public class Report
{
    public Guid Id { get; set; }
    public DateTime RequestedAt { get; set; }
    public ReportStatus Status { get; set; }
    public IList<LocationStatistics> Data { get; set; } = default!;
}
