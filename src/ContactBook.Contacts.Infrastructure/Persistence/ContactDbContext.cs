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
        modelBuilder.Entity<Contact>()
                .HasMany(c => c.ContactInfos)
                .WithOne(ci => ci.Contact)
                .HasForeignKey(ci => ci.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ContactInfo>()
            .Property(ci => ci.InfoType)
            .HasConversion<int>();

        base.OnModelCreating(modelBuilder);
    }
}
