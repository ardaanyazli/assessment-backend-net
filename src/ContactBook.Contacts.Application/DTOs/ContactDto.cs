namespace ContactBook.Contacts.Application.DTOs;

public record ContactDto(Guid Id, string FullName, IList<ContactInfoDto>? ContactInfo);
