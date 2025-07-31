using ContactBook.Contacts.Application.Interfaces;
using ContactBook.Contacts.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using ContactBook.Contects.Application.DTOs;

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
            var contactsResult = contactList.Select(c => new ContactListDto()
            {
                Id = c.Id,
                FullName = $"{c.FirstName} {c.LastName}"
            });

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
            return Results.Ok(new ContactDto
            {
                Id = contact.Id,
                FullName = $"{contact.FirstName} {contact.LastName}",
                ContactInfo = contact.ContactInfos.Select(ci => new ContactInfoDto
                ()
                {
                    Type = ci.InfoType.ToString(),
                    Value = ci.Value,
                    IsDefault = ci.IsDefault
                }).ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Results.InternalServerError("An error occurred while retrieving the contact.");
        }
    }

    [HttpPost]
    public async Task<IResult> CreateContact( CreateContactDto createContactDto, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.CreateContactAsync(createContactDto);

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

            var updatedContact = await _contactRepository.UpdateContactAsync(id, updateContactDto);

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
    public async Task<IResult> AddContactInfo(Guid id, [FromBody] CreateContactInfoDto createContactInfoDto, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetContactByIdAsync(id);

            if (contact == null)
            {
                return Results.NotFound($"Contact with ID {id} not found.");
            }

            var updatedContact = await _contactInfoRepository.AddContactInfoAsync(id, createContactInfoDto);

            return Results.NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);

            return Results.InternalServerError("An error occurred while adding contact information.");
        }
    }

    [HttpPut("{id}/info/{infoId}")]
    public async Task<IResult> UpdateContactInfo(Guid id, Guid infoId, ContactInfoDto updateContactInfoDto, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetContactByIdAsync(id);
            if (contact == null)
            {
                return Results.NotFound($"Contact not found.");
            }

            if (!contact.ContactInfos.Any(ci => ci.Id == infoId))
            {
                return Results.NotFound($"Contact information not found.");
            }

            var updatedContact = await _contactInfoRepository.UpdateContactInfoAsync(id, infoId, updateContactInfoDto);
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

            await _contactInfoRepository.DeleteContactInfoAsync(id, infoId);
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Results.InternalServerError("An error occurred while deleting contact information.");
        }
    }
}

