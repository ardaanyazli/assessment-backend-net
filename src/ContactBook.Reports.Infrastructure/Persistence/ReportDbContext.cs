namespace ContactBook.Reports.Infrastructure.Persistence;

public class ReportDbContext : DbContext
{
    public DbSet<Report> Reports { get; set; }
}
