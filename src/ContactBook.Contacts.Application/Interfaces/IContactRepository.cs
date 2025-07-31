using ContactBook.Contacts.Domain.Entities;
using ContactBook.Contacts.Application.DTOs;

namespace ContactBook.Contacts.Application.Interfaces;

public interface IContactRepository
{
    Task<IEnumerable<Contact>> GetContactsAsync();
    Task<Contact> GetContactByIdAsync(Guid id);
    Task<Contact> CreateContactAsync(Contact contact);
    Task<Contact> UpdateContactAsync(Contact contact);
    Task DeleteContactAsync(Guid id);
}
