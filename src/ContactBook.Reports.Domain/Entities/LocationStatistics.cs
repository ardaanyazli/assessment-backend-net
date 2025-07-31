namespace ContactBook.Reports.Domain.Entities;

public record LocationStatistics
{
    string? Location;
    int ContactCount = 0;
    int PhoneCount = 0;
};
