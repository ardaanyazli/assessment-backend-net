namespace ContactBook.Contacts.Application.Interfaces;

public interface IContactsUnitOfWork
{
    IContactRepository ContactRepository { get; }
    IContactInfoRepository ContactInfoRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}
