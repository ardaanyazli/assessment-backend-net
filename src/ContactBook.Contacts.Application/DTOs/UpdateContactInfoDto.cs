using ContactBook.Contacts.Domain.Entities;

public record UpdateContactInfoDto(
    ContactInfoType ContactInfoType,
    string Value,
    bool IsDefault
);