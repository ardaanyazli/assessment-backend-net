using ContactBook.Contacts.Application.DTOs;
using ContactBook.Contacts.Domain.Entities;

namespace  ContactBook.Contacts.Application.Interfaces;
public interface IContactInfoRepository
{
    Task<IList<ContactInfo>> GetContactInfosAsync();
    Task<ContactInfo> GetContactInfoAsync(Guid id);
    Task<IList<ContactInfo>> GetContactInfoByContactIdAsync(Guid contactId);
    Task<ContactInfo> AddContactInfoAsync(ContactInfo contactInfo);
    Task<ContactInfo> UpdateContactInfoAsync(ContactInfo contactInfo);
    Task DeleteContactInfoAsync(Guid id);
}