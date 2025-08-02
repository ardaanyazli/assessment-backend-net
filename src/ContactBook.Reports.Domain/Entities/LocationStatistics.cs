namespace ContactBook.Reports.Domain.Entities;

public record LocationStatistics
(
    string Location,
    int ContactCount,
    int PhoneCount
);