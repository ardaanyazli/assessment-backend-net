namespace ContactBook.Reports.Domain.Entities;

public enum ReportStatus
{
    Requested, //Initial state of the report
    InProgress, //Consumer taken the report request and working on generation
    Completed //report generated and ready to serve
}
