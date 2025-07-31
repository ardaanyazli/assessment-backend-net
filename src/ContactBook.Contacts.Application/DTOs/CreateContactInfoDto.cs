using ContactBook.Contacts.Domain.Entities;

namespace ContactBook.Contacts.Application.DTOs;

public record CreateContactInfoDto(
    Guid ContactId,
    ContactInfoType ContactInfoType,
    string Value,
    bool IsDefault
);
