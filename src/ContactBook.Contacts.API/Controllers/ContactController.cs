using ContactBook.Contacts.Application.Interfaces;
using ContactBook.Contacts.Application.DTOs;

namespace ContactBook.Contacts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactRepository _contactRepository;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(IContactRepository contactRepository, ILogger<ContactsController> logger)
    {
        _contactRepository = contactRepository;
        _logger = logger;
    }

    [HttpGet]
    public Task<IEnumerable<ContactListDto>> GetContacts(CancellationToken cancellationToken)
    {
        try
        {
            var contactList = await _contactRepository.GetContactsAsync();
            var contactsResult = contactList.Select(c => new ContactListDto());

            return Results.Ok(contactsResult);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return Results.ServerError("Error happened");
        }

    }

}


