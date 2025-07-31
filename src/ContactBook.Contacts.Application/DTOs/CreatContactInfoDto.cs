using ContactBook.Contacts.Domain.Entities;

namespace ContactBook.Contacts.Application.DTOs;

public record CreateContactInfoDto
{
    ContactInfoType contactInfoType;
    string Value;
}
