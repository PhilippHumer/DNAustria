using DNAustria.Backend.Models;

namespace DNAustria.Backend.Services;

public interface IContactService
{
    Task<IEnumerable<Contact>> GetAllAsync();
    Task<Contact?> GetAsync(Guid id);
    Task<Contact> CreateAsync(Contact c);
    Task<Contact?> UpdateAsync(Guid id, Contact c);
    Task<bool> DeleteAsync(Guid id);
}