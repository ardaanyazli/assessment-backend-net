using ContactBook.Contacts.Application.Interfaces;
using ContactBook.Contacts.Domain.Entities;
using ContactBook.Contacts.Infrastructure.Persistence;

namespace ContactBook.Contacts.Infrastructure.Repository;

public class ContactRepository : IContactRepository
{
    private readonly ContactsDbContext _context;

    public ContactRepository(ContactsDbContext context)
    {
        _context = context;
    }


    public async Task<Contact> CreateContactAsync(Contact contact)
    {
        await _context.Contacts.AddAsync(contact);

        await _context.SaveChangesAsync();

        return contact;
    }

    public async Task DeleteContactAsync(Guid id)
    {
        var contact = _context.Contacts.Find(id) ?? throw new NullReferenceException($"Contact with {id} not found.");

        _context.Contacts.Remove(contact);

        await _context.SaveChangesAsync();
    }

    public async Task<Contact> GetContactByIdAsync(Guid id)
    {
        var contact = _context.Contacts.Find(id) ?? throw new NullReferenceException($"Contact with {id} not found.");

        return contact;
    }

    public async Task<IList<Contact>> GetContactsAsync()
    {
        return _context.Contacts.ToList();
    }

    public async Task<Contact> UpdateContactAsync(Contact contact)
    {
        _context.Contacts.Update(contact);

        await _context.SaveChangesAsync();
        return contact;
    }
}
