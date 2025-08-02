using ContactBook.Contacts.Application.Interfaces;
using ContactBook.Contacts.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using ContactBook.Contects.Application.DTOs;
using ContactBook.Contacts.Domain.Entities;

namespace ContactBook.Contacts.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactsUnitOfWork _unitOfWork;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(IContactsUnitOfWork unitOfWork,
     ILogger<ContactsController> logger)
    {

        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IResult> GetContacts(CancellationToken cancellationToken)
    {
        try
        {
            var contactList = await _unitOfWork.ContactRepository.GetContactsAsync(cancellationToken);
            var contactsResult = contactList.Select(c => new ContactListDto(
                c.Id,
                $"{c.FirstName} {c.LastName}"
            ));

            return Results.Ok(contactsResult);
        }
        catch (OperationCanceledException cancelledEx)
        {
            _logger.LogWarning("Operation was cancelled: {Message}", cancelledEx.Message);
            return Results.StatusCode(499);
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
            var contact = await _unitOfWork.ContactRepository.GetContactByIdAsync(id);
            if (contact == null)
            {
                return Results.NotFound($"Contact with ID {id} not found.");
            }
            var contactInfoList = await _unitOfWork.ContactInfoRepository.GetContactInfoByContactIdAsync(id, cancellationToken);
            var contactDto = new ContactDto(contact.Id,
                $"{contact.FirstName} {contact.LastName}",
                contactInfoList.Select(ci => new ContactInfoDto(ci.Id, Enum.GetName(ci.InfoType)!, ci.Value, ci.IsDefault)).ToList()
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

            await _unitOfWork.ContactRepository.CreateContactAsync(contact, cancellationToken);

            if (createContactDto.ContactInfo != null && createContactDto.ContactInfo.Any())
            {
                foreach (var contactInfo in createContactDto.ContactInfo)
                {
                    await _unitOfWork.ContactInfoRepository.AddContactInfoAsync(new ContactInfo
                    {
                        Id = Guid.NewGuid(),
                        InfoType = contactInfo.ContactInfoType,
                        Value = contactInfo.Value,
                        IsDefault = contactInfo.IsDefault,
                        ContactId = contact.Id
                    }, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
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

        var existingContact = await _unitOfWork.ContactRepository.GetContactByIdAsync(id, cancellationToken);

        if (existingContact == null)
        {
            return Results.NotFound($"Contact with ID {id} not found.");
        }
        cancellationToken.ThrowIfCancellationRequested();
        existingContact.FirstName = updateContactDto.FirstName;
        existingContact.LastName = updateContactDto.LastName;

        _unitOfWork.ContactRepository.UpdateContact(existingContact);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Results.Ok();

    }

    [HttpDelete("{id}")]
    public async Task<IResult> DeleteContact(Guid id, CancellationToken cancellationToken)
    {

        var contact = await _unitOfWork.ContactRepository.GetContactByIdAsync(id);
        if (contact == null)
        {
            return Results.NotFound($"Contact with ID {id} not found.");
        }

        await _unitOfWork.ContactRepository.DeleteContactAsync(id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    [HttpGet("{id}/info")]
    public async Task<IResult> GetContactInfo(Guid id, CancellationToken cancellationToken)
    {
        var contact = await _unitOfWork.ContactRepository.GetContactByIdAsync(id, cancellationToken);

        if (contact == null)
        {
            return Results.NotFound($"Contact with ID {id} not found.");
        }

        var contactInfoList = await _unitOfWork.ContactInfoRepository.GetContactInfoByContactIdAsync(id, cancellationToken);

        var contactInfoDtoList = contactInfoList.Select(ci => new ContactInfoDto(
            ci.Id,
            Enum.GetName(ci.InfoType)!,
            ci.Value,
            ci.IsDefault
        )).ToList();

        return Results.Ok(contactInfoDtoList);
    }

    [HttpPost("{id}/info")]
    public async Task<IResult> AddContactInfo(Guid id, CreateContactInfoDto createContactInfoDto, CancellationToken cancellationToken)
    {

        var contact = await _unitOfWork.ContactRepository.GetContactByIdAsync(id, cancellationToken);

        if (contact == null)
        {
            return Results.NotFound($"Contact with ID {id} not found.");
        }

        await _unitOfWork.ContactInfoRepository.AddContactInfoAsync(new ContactInfo
        {
            Id = Guid.NewGuid(),
            InfoType = createContactInfoDto.ContactInfoType,
            Value = createContactInfoDto.Value,
            IsDefault = createContactInfoDto.IsDefault,
            ContactId = id
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();

    }

    [HttpPut("{id}/info")]
    public async Task<IResult> UpdateContactInfo(Guid id, ContactInfoDto updateContactInfoDto, CancellationToken cancellationToken)
    {

        var contact = await _unitOfWork.ContactRepository.GetContactByIdAsync(id, cancellationToken);

        if (contact == null)
        {
            return Results.NotFound($"Contact not found.");
        }

        var contactInfo = await _unitOfWork.ContactInfoRepository.GetContactInfoAsync(updateContactInfoDto.Id, cancellationToken);

        if (contactInfo == null)
        {
            return Results.NotFound($"Contact information not found.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var updatedContactInfo = new ContactInfo
        {
            Id = updateContactInfoDto.Id,
            InfoType = Enum.Parse<ContactInfoType>(updateContactInfoDto.Type),
            Value = updateContactInfoDto.Value,
            IsDefault = updateContactInfoDto.IsDefault,
            ContactId = id
        };

        _unitOfWork.ContactInfoRepository.UpdateContactInfo(updatedContactInfo);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok();

    }

    [HttpDelete("contactInfo/{infoId}")]
    public async Task<IResult> DeleteContactInfo(Guid infoId, CancellationToken cancellationToken)
    {
        await _unitOfWork.ContactInfoRepository.DeleteContactInfoAsync(infoId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}

