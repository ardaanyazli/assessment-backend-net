using ContactBook.Contacts.Domain.Entities;
using ContactBook.Contacts.Infrastructure.Persistence;
using ContactBook.Contacts.Infrastructure.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Infrastructure.Tests.Repository;

public class ContactInfoRepositoryTests : IDisposable
{
    private readonly ContactsDbContext _context;
    private readonly ContactInfoRepository _repository;

    public ContactInfoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ContactsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ContactsDbContext(options);
        _repository = new ContactInfoRepository(_context);
    }

    [Fact]
    public async Task GetContactInfosAsync_ReturnsAllContactInfos()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contactInfos = new List<ContactInfo>
        {
            new() { Id = Guid.NewGuid(), ContactId = contactId, InfoType = ContactInfoType.Email, Value = "test1@example.com", IsDefault = true },
            new() { Id = Guid.NewGuid(), ContactId = contactId, InfoType = ContactInfoType.Phone, Value = "123-456-7890", IsDefault = false }
        };

        await _context.ContactInfos.AddRangeAsync(contactInfos);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetContactInfosAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(ci => ci.InfoType == ContactInfoType.Email);
        result.Should().Contain(ci => ci.InfoType == ContactInfoType.Phone);
    }

    [Fact]
    public async Task GetContactInfoAsync_ReturnsContactInfo_WhenExists()
    {
        // Arrange
        var contactInfoId = Guid.NewGuid();
        var contactInfo = new ContactInfo 
        { 
            Id = contactInfoId, 
            ContactId = Guid.NewGuid(), 
            InfoType = ContactInfoType.Email, 
            Value = "test@example.com", 
            IsDefault = true 
        };

        await _context.ContactInfos.AddAsync(contactInfo);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetContactInfoAsync(contactInfoId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(contactInfoId);
        result.Value.Should().Be("test@example.com");
        result.InfoType.Should().Be(ContactInfoType.Email);
    }

    [Fact]
    public async Task GetContactInfoAsync_ThrowsException_WhenNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.GetContactInfoAsync(nonExistentId));
    }

    [Fact]
    public async Task GetContactInfoByContactIdAsync_ReturnsContactInfosForContact()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var otherContactId = Guid.NewGuid();
        
        var contactInfos = new List<ContactInfo>
        {
            new() { Id = Guid.NewGuid(), ContactId = contactId, InfoType = ContactInfoType.Email, Value = "contact1@example.com", IsDefault = true },
            new() { Id = Guid.NewGuid(), ContactId = contactId, InfoType = ContactInfoType.Phone, Value = "123-456-7890", IsDefault = false },
            new() { Id = Guid.NewGuid(), ContactId = otherContactId, InfoType = ContactInfoType.Email, Value = "other@example.com", IsDefault = true }
        };

        await _context.ContactInfos.AddRangeAsync(contactInfos);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetContactInfoByContactIdAsync(contactId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(ci => ci.ContactId == contactId);
    }

    [Fact]
    public async Task AddContactInfoAsync_AddsContactInfoToDatabase()
    {
        // Arrange
        var contactInfo = new ContactInfo 
        { 
            Id = Guid.NewGuid(), 
            ContactId = Guid.NewGuid(), 
            InfoType = ContactInfoType.Location, 
            Value = "Istanbul, Turkey", 
            IsDefault = true 
        };

        // Act
        await _repository.AddContactInfoAsync(contactInfo);
        await _context.SaveChangesAsync();

        // Assert
        var savedContactInfo = await _context.ContactInfos.FindAsync(contactInfo.Id);
        savedContactInfo.Should().NotBeNull();
        savedContactInfo!.Value.Should().Be("Istanbul, Turkey");
        savedContactInfo.InfoType.Should().Be(ContactInfoType.Location);
    }

    [Fact]
    public async Task UpdateContactInfo_ModifiesExistingContactInfo()
    {
        // Arrange
        var contactInfo = new ContactInfo 
        { 
            Id = Guid.NewGuid(), 
            ContactId = Guid.NewGuid(), 
            InfoType = ContactInfoType.Email, 
            Value = "old@example.com", 
            IsDefault = false 
        };
        
        await _context.ContactInfos.AddAsync(contactInfo);
        await _context.SaveChangesAsync();

        // Modify the contact info
        contactInfo.Value = "new@example.com";
        contactInfo.IsDefault = true;

        // Act
        _repository.UpdateContactInfo(contactInfo);
        await _context.SaveChangesAsync();

        // Assert
        var updatedContactInfo = await _context.ContactInfos.FindAsync(contactInfo.Id);
        updatedContactInfo.Should().NotBeNull();
        updatedContactInfo!.Value.Should().Be("new@example.com");
        updatedContactInfo.IsDefault.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteContactInfoAsync_RemovesContactInfoFromDatabase()
    {
        // Arrange
        var contactInfo = new ContactInfo 
        { 
            Id = Guid.NewGuid(), 
            ContactId = Guid.NewGuid(), 
            InfoType = ContactInfoType.Phone, 
            Value = "555-1234", 
            IsDefault = false 
        };
        
        await _context.ContactInfos.AddAsync(contactInfo);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteContactInfoAsync(contactInfo.Id);
        await _context.SaveChangesAsync();

        // Assert
        var deletedContactInfo = await _context.ContactInfos.FindAsync(contactInfo.Id);
        deletedContactInfo.Should().BeNull();
    }

    [Fact]
    public async Task DeleteContactInfoAsync_ThrowsException_WhenContactInfoDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.DeleteContactInfoAsync(nonExistentId));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}