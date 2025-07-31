namespace ContactBook.Contacts.Application.DTOs;

public record ContactDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public IList<ContactInfoDto> ContactInfo { get; set; }
}
