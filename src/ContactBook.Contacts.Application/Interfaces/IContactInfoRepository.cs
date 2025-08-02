using ContactBook.Contacts.Domain.Entities;

namespace  ContactBook.Contacts.Application.Interfaces;
public interface IContactInfoRepository
{
    Task<IList<ContactInfo>> GetContactInfosAsync(CancellationToken cancellationToken = default);
    Task<ContactInfo> GetContactInfoAsync(Guid id,CancellationToken cancellationToken = default);
    Task<IList<ContactInfo>> GetContactInfoByContactIdAsync(Guid contactId,CancellationToken cancellationToken = default);
    Task AddContactInfoAsync(ContactInfo contactInfo,CancellationToken cancellationToken = default);
    void UpdateContactInfo(ContactInfo contactInfo);
    Task DeleteContactInfoAsync(Guid id,CancellationToken cancellationToken = default);
}