using ContactBook.Contacts.Application.DTOs;
using ContactBook.Contacts.Domain.Entities;
using ContactBook.Contacts.Infrastructure.Persistence;
using ContactBook.Contects.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;

namespace ContactBook.Integration.Tests;

public class ContactsApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ContactsApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ContactsDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<ContactsDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetContacts_ReturnsEmptyList_WhenNoContactsExist()
    {
        // Act
        var response = await _client.GetAsync("/Contacts");

        // Assert
         response.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var contacts = JsonSerializer.Deserialize<List<ContactListDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        contacts.Should().BeEmpty();
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

        // Act
        var response = await _client.PostAsJsonAsync("/Contacts", createContactDto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetContacts_ReturnsContacts_AfterCreation()
    {
        // Arrange - Create a contact first
        var createContactDto = new CreateContactDto("Jane", "Smith", null);
        await _client.PostAsJsonAsync("/Contacts", createContactDto);

        // Act
        var response = await _client.GetAsync("/Contacts");

        // Assert
         response.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var contacts = JsonSerializer.Deserialize<List<ContactListDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        contacts.Should().HaveCount(1);
        contacts![0].FullName.Should().Be("Jane Smith");
    }

    [Fact]
    public async Task GetContactById_ReturnsContact_WhenContactExists()
    {
        // Arrange - Create a contact and get its ID
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();
        
        var contact = new Contact 
        { 
            Id = Guid.NewGuid(), 
            FirstName = "Test", 
            LastName = "User" 
        };
        
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/Contacts/{contact.Id}");

        // Assert
         response.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var contactDto = JsonSerializer.Deserialize<ContactDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        contactDto.Should().NotBeNull();
        contactDto!.FullName.Should().Be("Test User");
    }

    [Fact]
    public async Task GetContactById_ReturnsNotFound_WhenContactDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/Contacts/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateContact_ReturnsOk_WhenContactExists()
    {
        // Arrange - Create a contact first
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();
        
        var contact = new Contact 
        { 
            Id = Guid.NewGuid(), 
            FirstName = "Original", 
            LastName = "Name" 
        };
        
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        var updateDto = new UpdateContactDto("Updated", "Name");

        // Act
        var response = await _client.PutAsJsonAsync($"/Contacts/{contact.Id}", updateDto);

        // Assert
        response.Should().Be(System.Net.HttpStatusCode.OK);
        
        // Verify the update
        var updatedContact = await context.Contacts.FindAsync(contact.Id);
        updatedContact!.FirstName.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteContact_ReturnsNoContent_WhenContactExists()
    {
        // Arrange - Create a contact first
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();
        
        var contact = new Contact 
        { 
            Id = Guid.NewGuid(), 
            FirstName = "To", 
            LastName = "Delete" 
        };
        
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/Contacts/{contact.Id}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        
        // Verify deletion
        var deletedContact = await context.Contacts.FindAsync(contact.Id);
        deletedContact.Should().BeNull();
    }

    [Fact]
    public async Task AddContactInfo_ReturnsNoContent_WhenContactExists()
    {
        // Arrange - Create a contact first
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();
        
        var contact = new Contact 
        { 
            Id = Guid.NewGuid(), 
            FirstName = "Test", 
            LastName = "User" 
        };
        
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        var createContactInfoDto = new CreateContactInfoDto(ContactInfoType.Phone, "123-456-7890", false);

        // Act
        var response = await _client.PostAsJsonAsync($"/Contacts/{contact.Id}/info", createContactInfoDto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        
        // Verify the contact info was added
        var contactInfos = await context.ContactInfos.Where(ci => ci.ContactId == contact.Id).ToListAsync();
        contactInfos.Should().HaveCount(1);
        contactInfos[0].Value.Should().Be("123-456-7890");
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }
}