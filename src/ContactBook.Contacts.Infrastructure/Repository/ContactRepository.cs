using ContactBook.Contacts.Application.Interfaces;
using ContactBook.Contacts.Domain.Entities;

namespace ContactBook.Contacts.Infrastructure.Repository;

public class ContactRepository : IContactRepository
{

    public Task CreateContact(Contact contact)
    {
        throw new NotImplementedException();
    }

    public Task DeleteContact(Guid contactId)
    {
        throw new NotImplementedException();
    }

    public Task<Contact> GetContactByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Contact>> GetContactsAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateContact(Contact contact)
    {
        throw new NotImplementedException();
    }
}
