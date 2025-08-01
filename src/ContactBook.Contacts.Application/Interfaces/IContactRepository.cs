using ContactBook.Contacts.Domain.Entities;

namespace ContactBook.Contacts.Application.Interfaces;

public interface IContactRepository
{
    Task<IList<Contact>> GetContactsAsync();
    Task<Contact> GetContactByIdAsync(Guid id);
    Task CreateContactAsync(Contact contact);
    void UpdateContact(Contact contact);
    Task DeleteContactAsync(Guid id);
}
