using ContactBook.Contacts.Application.DTOs;
using ContactBook.Contacts.Domain.Entities;

namespace  ContactBook.Contacts.Application.Interfaces;
public interface IContactInfoRepository
{
    Task<IEnumerable<ContactInfo>> GetContactInfosAsync(Guid contactId);
    Task<ContactInfo> GetContactInfoByTypeAsync(Guid contactId, Guid infoId);
    Task<ContactInfo> AddContactInfoAsync(Guid contactId, CreateContactInfoDto contactInfoDto);
    Task<ContactInfo> UpdateContactInfoAsync(Guid contactId, Guid infoId, ContactInfoDto contactInfoDto);
    Task DeleteContactInfoAsync(Guid contactId, Guid infoId);
}