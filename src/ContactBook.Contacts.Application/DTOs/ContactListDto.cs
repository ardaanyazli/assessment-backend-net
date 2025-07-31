namespace ContactBook.Contects.Application.DTOs;

public record ContactListDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
}
