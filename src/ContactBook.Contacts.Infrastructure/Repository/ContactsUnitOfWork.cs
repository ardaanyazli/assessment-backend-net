using System;
using ContactBook.Contacts.Application.Interfaces;
using ContactBook.Contacts.Infrastructure.Persistence;

namespace ContactBook.Contacts.Infrastructure.Repository;

public class ContactsUnitOfWork : IContactsUnitOfWork
{

    private readonly ContactsDbContext _context;

    public ContactsUnitOfWork(ContactsDbContext context)
    {
        _context = context;
        ContactRepository = new ContactRepository(_context);
        ContactInfoRepository = new ContactInfoRepository(_context);
    }
    
    public IContactRepository ContactRepository { get; }

    public IContactInfoRepository ContactInfoRepository { get; }

    public Task<int> SaveChangesAsync()
    {
        _context.ChangeTracker.DetectChanges();
        return _context.SaveChangesAsync();
    }
}
