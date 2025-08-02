using ContactBook.Contacts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Contacts.Infrastructure.Persistence;

public class ContactsDbContext : DbContext
{
    public ContactsDbContext(DbContextOptions<ContactsDbContext> options)
        : base(options)
    {

    }
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<ContactInfo> ContactInfos => Set<ContactInfo>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(c =>
        {
            c.HasKey(c => c.Id);
            c.HasMany(c => c.ContactInfos)
            .WithOne(ci => ci.Contact)
            .HasForeignKey(ci => ci.ContactId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ContactInfo>(ci =>
        {
            ci.HasKey(ci => ci.Id);
            ci.Property(ci => ci.InfoType)
            .HasConversion<int>();
            ci.HasIndex(ci =>ci.ContactId)
            .HasDatabaseName("IX_ContactInfo_ContactId");
        });


        base.OnModelCreating(modelBuilder);
    }
}
