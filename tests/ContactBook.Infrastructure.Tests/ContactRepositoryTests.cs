using ContactBook.Contacts.Domain.Entities;
using ContactBook.Contacts.Infrastructure.Persistence;
using ContactBook.Contacts.Infrastructure.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Infrastructure.Tests.Repository;

public class ContactRepositoryTests : IDisposable
{
    private readonly ContactsDbContext _context;
    private readonly ContactRepository _repository;

    public ContactRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ContactsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ContactsDbContext(options);
        _repository = new ContactRepository(_context);
    }

    [Fact]
    public async Task GetContactsAsync_ReturnsAllContacts()
    {
        // Arrange
        var contacts = new List<Contact>
        {
            new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
            new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
        };

        await _context.Contacts.AddRangeAsync(contacts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetContactsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.FirstName == "John" && c.LastName == "Doe");
        result.Should().Contain(c => c.FirstName == "Jane" && c.LastName == "Smith");
    }

    [Fact]
    public async Task GetContactByIdAsync_ReturnsContact_WhenContactExists()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contact = new Contact { Id = contactId, FirstName = "John", LastName = "Doe" };

        await _context.Contacts.AddAsync(contact);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetContactByIdAsync(contactId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(contactId);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task GetContactByIdAsync_ThrowsException_WhenContactDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _repository.GetContactByIdAsync(nonExistentId));
    }

    [Fact]
    public async Task CreateContactAsync_AddsContactToDatabase()
    {
        // Arrange
        var contact = new Contact { Id = Guid.NewGuid(), FirstName = "New", LastName = "Contact" };

        // Act
        await _repository.CreateContactAsync(contact);
        await _context.SaveChangesAsync();

        // Assert
        var savedContact = await _context.Contacts.FindAsync(contact.Id);
        savedContact.Should().NotBeNull();
        savedContact!.FirstName.Should().Be("New");
        savedContact.LastName.Should().Be("Contact");
    }

    [Fact]
    public async Task UpdateContact_ModifiesExistingContact()
    {
        // Arrange
        var contact = new Contact { Id = Guid.NewGuid(), FirstName = "Original", LastName = "Name" };
        await _context.Contacts.AddAsync(contact);
        await _context.SaveChangesAsync();

        // Modify the contact
        contact.FirstName = "Updated";
        contact.LastName = "Name";

        // Act
        _repository.UpdateContact(contact);
        await _context.SaveChangesAsync();

        // Assert
        var updatedContact = await _context.Contacts.FindAsync(contact.Id);
        updatedContact.Should().NotBeNull();
        updatedContact!.FirstName.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteContactAsync_RemovesContactFromDatabase()
    {
        // Arrange
        var contact = new Contact { Id = Guid.NewGuid(), FirstName = "To", LastName = "Delete" };
        await _context.Contacts.AddAsync(contact);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteContactAsync(contact.Id);
        await _context.SaveChangesAsync();

        // Assert
        var deletedContact = await _context.Contacts.FindAsync(contact.Id);
        deletedContact.Should().BeNull();
    }

    [Fact]
    public async Task DeleteContactAsync_ThrowsException_WhenContactDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _repository.DeleteContactAsync(nonExistentId));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}