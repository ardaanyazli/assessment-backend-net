namespace ContactBook.Contacts.Domain.Entities;

public class ContactInfo
{
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public ContactInfoType InfoType { get; set; }
    public bool isDefault { get; set; }
}
