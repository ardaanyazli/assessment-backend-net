namespace ContactBook.Contacts.Application.DTOs;

public record ContactInfoDto(Guid Id,string Type, string Value, bool IsDefault);
