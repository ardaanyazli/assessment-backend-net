namespace ContactBook.Contacts.Infrastructure.Persistence;
public class ContactDbContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<ContactInfo> ContactInfos { get; set; }

}
