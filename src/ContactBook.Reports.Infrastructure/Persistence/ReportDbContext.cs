using ContactBook.Reports.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Reports.Infrastructure.Persistence;

public class ReportDbContext : DbContext
{
    public ReportDbContext(DbContextOptions<ReportDbContext> options)
        : base(options)
    {
        
    }
    public DbSet<Report> Reports => Set<Report>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Report>()
            .Property(r => r.Data)
            .HasColumnType("jsonb");

        base.OnModelCreating(modelBuilder);
    }
}
