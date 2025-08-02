namespace ContactBook.Contacts.Domain.Entities;

public class ContactInfo
{
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public ContactInfoType InfoType { get; set; }
    public string Value { get; set; } = default!;
    public bool IsDefault { get; set; } = default!;
    public Contact Contact { get; set; } = default!;
}
