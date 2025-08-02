namespace ContactBook.Contacts.Domain.Entities;

public class Contact
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public IList<ContactInfo>? ContactInfos { get; set; }
}
