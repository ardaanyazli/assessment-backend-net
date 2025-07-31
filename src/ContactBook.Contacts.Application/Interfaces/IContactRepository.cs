using ContactBook.Contacts.Domain.Entities;

namespace ContactBook.Contacts.Application.Interfaces;

public interface IContactRepository
{
    Task<IEnumerable<Contact>> GetContactsAsync();
    Task<Contact> GetContactByIdAsync(Guid id);
    Task CreateContact(Contact contact);
    Task UpdateContact(Contact contact);
    Task DeleteContact(Guid contactId);
}
