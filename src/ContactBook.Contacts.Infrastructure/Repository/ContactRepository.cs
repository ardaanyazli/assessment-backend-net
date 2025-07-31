using ContactBook.Contacts.Application.DTOs;
using ContactBook.Contacts.Application.Interfaces;
using ContactBook.Contacts.Domain.Entities;

namespace ContactBook.Contacts.Infrastructure.Repository;

public class ContactRepository : IContactRepository
{
    public Task<Contact> CreateContactAsync(Contact contact)
    {
        throw new NotImplementedException();
    }

    public Task DeleteContactAsync(Guid id)
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

    public Task<Contact> UpdateContactAsync(Contact contact)
    {
        throw new NotImplementedException();
    }
}
