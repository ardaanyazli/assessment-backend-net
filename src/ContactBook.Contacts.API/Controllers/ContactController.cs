using ContactBook.Contacts.Application.Interfaces;
using ContactBook.Contacts.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using ContactBook.Contects.Application.DTOs;
using ContactBook.Contacts.Domain.Entities;

namespace ContactBook.Contacts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactRepository _contactRepository;
    private readonly IContactInfoRepository _contactInfoRepository;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(IContactRepository contactRepository,
    IContactInfoRepository contactInfoRepository,
     ILogger<ContactsController> logger)
    {
        _contactRepository = contactRepository;
        _contactInfoRepository = contactInfoRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IResult> GetContacts(CancellationToken cancellationToken)
    {
        try
        {
            var contactList = await _contactRepository.GetContactsAsync();
            var contactsResult = contactList.Select(c => new ContactListDto(
                c.Id,
                $"{c.FirstName} {c.LastName}"
            ));

            return Results.Ok(contactsResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Results.InternalServerError("An error occurred while retrieving contacts.");
        }

    }

    [HttpGet("{id}")]
    public async Task<IResult> GetContactById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetContactByIdAsync(id);
            if (contact == null)
            {
                return Results.NotFound($"Contact with ID {id} not found.");
            }
            var contactInfoList = await _contactInfoRepository.GetContactInfoByContactIdAsync(id);
            var contactDto = new ContactDto(contact.Id,
                $"{contact.FirstName} {contact.LastName}",
                contactInfoList.Select(ci => new ContactInfoDto(Enum.GetName(ci.InfoType)!, ci.Value, ci.IsDefault)).ToList()
            );

            return Results.Ok(contactDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Results.InternalServerError("An error occurred while retrieving the contact.");
        }
    }

    [HttpPost]
    public async Task<IResult> CreateContact(CreateContactDto createContactDto, CancellationToken cancellationToken)
    {
        try
        {

            var contact = new Contact()
            {
                Id = Guid.NewGuid(),
                FirstName = createContactDto.FirstName,
                LastName = createContactDto.LastName
            };

            await _contactRepository.CreateContactAsync(contact);

            return Results.Created();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Results.InternalServerError("An error occurred while creating the contact.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IResult> UpdateContact(Guid id, UpdateContactDto updateContactDto, CancellationToken cancellationToken)
    {
        try
        {
            var existingContact = await _contactRepository.GetContactByIdAsync(id);

            if (existingContact == null)
            {
                return Results.NotFound($"Contact with ID {id} not found.");
            }

            existingContact.FirstName = updateContactDto.FirstName;
            existingContact.LastName = updateContactDto.LastName;

            var updatedContact = await _contactRepository.UpdateContactAsync(existingContact);

            return Results.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Results.InternalServerError("An error occurred while updating the contact.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IResult> DeleteContact(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetContactByIdAsync(id);
            if (contact == null)
            {
                return Results.NotFound($"Contact with ID {id} not found.");
            }

            await _contactRepository.DeleteContactAsync(id);
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Results.InternalServerError("An error occurred while deleting the contact.");
        }
    }

    [HttpPost("{id}/info")]
    public async Task<IResult> AddContactInfo(Guid id, CreateContactInfoDto createContactInfoDto, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetContactByIdAsync(id);

            if (contact == null)
            {
                return Results.NotFound($"Contact with ID {id} not found.");
            }

            var updatedContact = await _contactInfoRepository.AddContactInfoAsync(new ContactInfo
            {
                Id = Guid.NewGuid(),
                InfoType = createContactInfoDto.ContactInfoType,
                Value = createContactInfoDto.Value,
                IsDefault = createContactInfoDto.IsDefault,
                ContactId = id
            });

            return Results.NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);

            return Results.InternalServerError("An error occurred while adding contact information.");
        }
    }

    [HttpPut("{id}/info/{infoId}")]
    public async Task<IResult> UpdateContactInfo(Guid id, ContactInfoDto updateContactInfoDto, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetContactByIdAsync(id);
            if (contact == null)
            {
                return Results.NotFound($"Contact not found.");
            }
            //TODO: Fix update implementation
            // if (!_contactInfoRepository.GetContactInfoAsync(ci => ci.Id == infoId))
            // {
            //     return Results.NotFound($"Contact information not found.");
            // }

            // var updatedContact = await _contactInfoRepository.UpdateContactInfoAsync(id,);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Results.InternalServerError("An error occurred while updating contact information.");
        }
    }

    [HttpDelete("{id}/info/{infoId}")]
    public async Task<IResult> DeleteContactInfo(Guid id, Guid infoId, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetContactByIdAsync(id);
            if (contact == null)
            {
                return Results.NotFound($"Contact with ID {id} not found.");
            }

            await _contactInfoRepository.DeleteContactInfoAsync(infoId);
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Results.InternalServerError("An error occurred while deleting contact information.");
        }
    }
}

