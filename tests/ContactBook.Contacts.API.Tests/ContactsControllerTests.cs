using ContactBook.Contacts.API.Controllers;
using ContactBook.Contacts.Application.DTOs;
using ContactBook.Contacts.Application.Interfaces;
using ContactBook.Contacts.Domain.Entities;
using ContactBook.Contects.Application.DTOs;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ContactBook.Contacts.API.Tests.Controllers;

public class ContactsControllerTests
{
    private readonly Mock<IContactsUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<ContactsController>> _mockLogger;
    private readonly Mock<IContactRepository> _mockContactRepository;
    private readonly Mock<IContactInfoRepository> _mockContactInfoRepository;
    private readonly ContactsController _controller;

    public ContactsControllerTests()
    {
        _mockUnitOfWork = new Mock<IContactsUnitOfWork>();
        _mockLogger = new Mock<ILogger<ContactsController>>();
        _mockContactRepository = new Mock<IContactRepository>();
        _mockContactInfoRepository = new Mock<IContactInfoRepository>();
        
        _mockUnitOfWork.Setup(x => x.ContactRepository).Returns(_mockContactRepository.Object);
        _mockUnitOfWork.Setup(x => x.ContactInfoRepository).Returns(_mockContactInfoRepository.Object);
        
        _controller = new ContactsController(_mockUnitOfWork.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetContacts_ReturnsOkResult_WhenContactsExist()
    {
        // Arrange
        var contacts = new List<Contact>
        {
            new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
            new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
        };
        
        _mockContactRepository.Setup(x => x.GetContactsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(contacts);

        // Act
        var result = await _controller.GetContacts(CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<IEnumerable<ContactListDto>>>();
    }

    [Fact]
    public async Task GetContacts_ReturnsStatusCode499_WhenOperationCancelled()
    {
        // Arrange
        _mockContactRepository.Setup(x => x.GetContactsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = await _controller.GetContacts(CancellationToken.None);

        // Assert
        var statusCodeResult = result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.StatusCodeHttpResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(499);
    }

    [Fact]
    public async Task GetContacts_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockContactRepository.Setup(x => x.GetContactsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetContacts(CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.InternalServerError<string>>();
    }

    [Fact]
    public async Task GetContactById_ReturnsOkResult_WhenContactExists()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contact = new Contact { Id = contactId, FirstName = "John", LastName = "Doe" };
        var contactInfos = new List<ContactInfo>
        {
            new() { Id = Guid.NewGuid(), InfoType = ContactInfoType.Email, Value = "john@example.com", IsDefault = true }
        };

        _mockContactRepository.Setup(x => x.GetContactByIdAsync(contactId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);
        _mockContactInfoRepository.Setup(x => x.GetContactInfoByContactIdAsync(contactId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contactInfos);

        // Act
        var result = await _controller.GetContactById(contactId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<ContactDto>>();
    }

    [Fact]
    public async Task GetContactById_ReturnsNotFound_WhenContactDoesNotExist()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        _mockContactRepository.Setup(x => x.GetContactByIdAsync(contactId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contact?)null);

        // Act
        var result = await _controller.GetContactById(contactId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>();
    }

    [Fact]
    public async Task CreateContact_ReturnsCreated_WhenValidContactProvided()
    {
        // Arrange
        var createContactDto = new CreateContactDto(
            "John",
            "Doe",
            new List<CreateContactInfoDto>
            {
                new(ContactInfoType.Email, "john@example.com", true)
            });

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _controller.CreateContact(createContactDto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Created>();
        _mockContactRepository.Verify(x => x.CreateContactAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockContactInfoRepository.Verify(x => x.AddContactInfoAsync(It.IsAny<ContactInfo>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateContact_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var createContactDto = new CreateContactDto("John", "Doe", null);
        _mockContactRepository.Setup(x => x.CreateContactAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateContact(createContactDto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.InternalServerError<string>>();
    }

    [Fact]
    public async Task UpdateContact_ReturnsOk_WhenContactExists()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contact = new Contact { Id = contactId, FirstName = "John", LastName = "Doe" };
        var updateContactDto = new UpdateContactDto("Jane", "Smith");

        _mockContactRepository.Setup(x => x.GetContactByIdAsync(contactId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _controller.UpdateContact(contactId, updateContactDto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok>();
        contact.FirstName.Should().Be("Jane");
        contact.LastName.Should().Be("Smith");
        _mockContactRepository.Verify(x => x.UpdateContact(contact), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateContact_ReturnsNotFound_WhenContactDoesNotExist()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var updateContactDto = new UpdateContactDto("Jane", "Smith");

        _mockContactRepository.Setup(x => x.GetContactByIdAsync(contactId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contact?)null);

        // Act
        var result = await _controller.UpdateContact(contactId, updateContactDto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>();
    }

    [Fact]
    public async Task DeleteContact_ReturnsNoContent_WhenContactExists()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contact = new Contact { Id = contactId, FirstName = "John", LastName = "Doe" };

        _mockContactRepository.Setup(x => x.GetContactByIdAsync(contactId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _controller.DeleteContact(contactId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NoContent>();
        _mockContactRepository.Verify(x => x.DeleteContactAsync(contactId,CancellationToken.None), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteContact_ReturnsNotFound_WhenContactDoesNotExist()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        _mockContactRepository.Setup(x => x.GetContactByIdAsync(contactId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contact?)null);

        // Act
        var result = await _controller.DeleteContact(contactId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>();
    }

    [Fact]
    public async Task AddContactInfo_ReturnsNoContent_WhenContactExists()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contact = new Contact { Id = contactId, FirstName = "John", LastName = "Doe" };
        var createContactInfoDto = new CreateContactInfoDto(ContactInfoType.Phone, "123-456-7890", false);

        _mockContactRepository.Setup(x => x.GetContactByIdAsync(contactId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _controller.AddContactInfo(contactId, createContactInfoDto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NoContent>();
        _mockContactInfoRepository.Verify(x => x.AddContactInfoAsync(It.IsAny<ContactInfo>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateContactInfo_ReturnsNoContent_WhenContactInfoExists()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contactInfoId = Guid.NewGuid();
        var contact = new Contact { Id = contactId, FirstName = "John", LastName = "Doe" };
        var contactInfo = new ContactInfo 
        { 
            Id = contactInfoId, 
            ContactId = contactId, 
            InfoType = ContactInfoType.Email, 
            Value = "old@example.com", 
            IsDefault = false 
        };
        var updateContactInfoDto = new ContactInfoDto(contactInfoId, "Email", "new@example.com", true);

        _mockContactRepository.Setup(x => x.GetContactByIdAsync(contactId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);
        _mockContactInfoRepository.Setup(x => x.GetContactInfoAsync(contactInfoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contactInfo);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _controller.UpdateContactInfo(contactId, updateContactInfoDto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NoContent>();
        contactInfo.Value.Should().Be("new@example.com");
        contactInfo.IsDefault.Should().BeTrue();
        _mockContactInfoRepository.Verify(x => x.UpdateContactInfo(contactInfo), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteContactInfo_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var infoId = Guid.NewGuid();
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _controller.DeleteContactInfo(infoId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NoContent>();
        _mockContactInfoRepository.Verify(x => x.DeleteContactInfoAsync(infoId, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}