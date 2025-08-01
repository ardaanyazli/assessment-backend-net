using System.Data.Common;

namespace ContactBook.Reports.Application.DTOs;

public record ReportDto(Guid Id,IList<LocationStatisticsDto> Data);
