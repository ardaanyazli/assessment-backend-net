using ContactBook.Contacts.Infrastructure.Persistence;
using ContactBook.Contacts.Infrastructure.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Infrastructure.Tests.Repository;

public class ContactsUnitOfWorkTests : IDisposable
{
    private readonly ContactsDbContext _context;
    private readonly ContactsUnitOfWork _unitOfWork;

    public ContactsUnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<ContactsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ContactsDbContext(options);
        _unitOfWork = new ContactsUnitOfWork(_context);
    }

    [Fact]
    public void Constructor_InitializesRepositories()
    {
        // Assert
        _unitOfWork.ContactRepository.Should().NotBeNull();
        _unitOfWork.ContactInfoRepository.Should().NotBeNull();
        _unitOfWork.ContactRepository.Should().BeOfType<ContactRepository>();
        _unitOfWork.ContactInfoRepository.Should().BeOfType<ContactInfoRepository>();
    }

    [Fact]
    public async Task SaveChangesAsync_ReturnsNumberOfChanges()
    {
        // Arrange
        var contact = new ContactBook.Contacts.Domain.Entities.Contact 
        { 
            Id = Guid.NewGuid(), 
            FirstName = "Test", 
            LastName = "User" 
        };
        
        await _unitOfWork.ContactRepository.CreateContactAsync(contact);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task SaveChangesAsync_WithCancellationToken_WorksCorrectly()
    {
        // Arrange
        var contact = new ContactBook.Contacts.Domain.Entities.Contact 
        { 
            Id = Guid.NewGuid(), 
            FirstName = "Test", 
            LastName = "User" 
        };
        
        await _unitOfWork.ContactRepository.CreateContactAsync(contact);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task SaveChangesAsync_DetectsChangesBeforeSaving()
    {
        // Arrange
        var contact = new ContactBook.Contacts.Domain.Entities.Contact 
        { 
            Id = Guid.NewGuid(), 
            FirstName = "Original", 
            LastName = "Name" 
        };
        
        await _unitOfWork.ContactRepository.CreateContactAsync(contact);
        await _unitOfWork.SaveChangesAsync();

        // Modify the contact
        contact.FirstName = "Modified";
        _unitOfWork.ContactRepository.UpdateContact(contact);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(1);
        
        // Verify the change was saved
        var savedContact = await _context.Contacts.FindAsync(contact.Id);
        savedContact!.FirstName.Should().Be("Modified");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}