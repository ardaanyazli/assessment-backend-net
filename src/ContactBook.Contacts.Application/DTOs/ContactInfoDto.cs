namespace ContactBook.Contacts.Application.DTOs;
public record ContactInfoDto
{
    public string Type { get; set; } = default!;
    public string Value { get; set; } = default!;
    public bool IsDefault { get; set; } = default!;
}
