using ContactBook.Contacts.Application.Interfaces;
using ContactBook.Contacts.Domain.Entities;
using ContactBook.Contacts.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Contacts.Infrastructure.Repository;

public class ContactInfoRepository : IContactInfoRepository
{
    private readonly ContactsDbContext _context;

    public ContactInfoRepository(ContactsDbContext context)
    {
        _context = context;
    }

    public async Task AddContactInfoAsync(ContactInfo contactInfo)
    {
        await _context.ContactInfos.AddAsync(contactInfo);
    }

    public async Task DeleteContactInfoAsync(Guid id)
    {
        var contactInfo = await _context.ContactInfos.FindAsync(id) ?? throw new KeyNotFoundException($"ContactInfo with ID {id} not found.");

        _context.ContactInfos.Remove(contactInfo);
    }

    public async Task<ContactInfo> GetContactInfoAsync(Guid id)
    {
        return await _context.ContactInfos
            .FindAsync(id)
            ?? throw new KeyNotFoundException($"ContactInfo with ID {id} not found.");
    }

    public async Task<IList<ContactInfo>> GetContactInfoByContactIdAsync(Guid contactId)
    {
        return await _context.ContactInfos
            .Where(ci => ci.ContactId == contactId)
            .ToListAsync() ?? throw new KeyNotFoundException($"No ContactInfo found for Contact ID {contactId}.");
    }

    public async Task<IList<ContactInfo>> GetContactInfosAsync()
    {
        return await _context.ContactInfos
             .ToListAsync();
    }

    public void UpdateContactInfo(ContactInfo contactInfo)
    {
        _context.ContactInfos.Update(contactInfo);
    }
}