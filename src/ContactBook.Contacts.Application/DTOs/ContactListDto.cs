namespace ContactBook.Contects.Application.DTOs;

public record ContactListDto(
    Guid Id,
    string Typea,
    string Value,
    bool IsDefault
);
