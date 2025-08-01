using ContactBook.Contacts.Domain.Entities;

namespace ContactBook.Contacts.Application.Interfaces;

public interface IContactRepository
{
    Task<IList<Contact>> GetContactsAsync(CancellationToken cancellationToken = default);
    Task<Contact> GetContactByIdAsync(Guid id,CancellationToken cancellationToken = default);
    Task CreateContactAsync(Contact contact,CancellationToken cancellationToken = default);
    void UpdateContact(Contact contact);
    Task DeleteContactAsync(Guid id,CancellationToken cancellationToken = default);
}
