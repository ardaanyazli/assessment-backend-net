namespace ContactBook.Contacts.Application.DTOs;

public record CreateContactDto(
    string FirstName,
    string LastName,
    IList<CreateContactInfoDto>? ContactInfo
);