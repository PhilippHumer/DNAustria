using DNAustria.Domain;

namespace DNAustria.Logic;

public interface IContactsLogic
{
    Task<IEnumerable<Contact>> GetAllAsync();
    Task<Contact> GetByIdAsync(Guid id);
    Task<Contact> AddAsync(string name, string? email, string? phoneNumber, Guid organisationId);
    Task<Contact> UpdateAsync(Guid id, string name, string? email, string? phoneNumber, Guid organisationId);
    Task DeleteAsync(Guid id);
}