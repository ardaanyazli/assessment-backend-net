using ContactBook.Contacts.Application.DTOs;
using ContactBook.Contacts.Domain.Entities;

namespace  ContactBook.Contacts.Application.Interfaces;
public interface IContactInfoRepository
{
    Task<IList<ContactInfo>> GetContactInfosAsync();
    Task<ContactInfo> GetContactInfoAsync(Guid id);
    Task<IList<ContactInfo>> GetContactInfoByContactIdAsync(Guid contactId);
    Task AddContactInfoAsync(ContactInfo contactInfo);
    void UpdateContactInfo(ContactInfo contactInfo);
    Task DeleteContactInfoAsync(Guid id);
}