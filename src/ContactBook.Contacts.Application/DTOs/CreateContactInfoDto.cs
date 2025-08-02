using ContactBook.Contacts.Domain.Entities;

namespace ContactBook.Contacts.Application.DTOs;

public record CreateContactInfoDto(
    ContactInfoType ContactInfoType,
    string Value,
    bool IsDefault
);
