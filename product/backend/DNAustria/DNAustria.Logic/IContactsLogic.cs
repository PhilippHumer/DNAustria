using DNAustria.Domain;

namespace DNAustria.Logic;

public interface IContactsLogic
{
    Task<IEnumerable<Contact>> GetAllAsync();
    Task<Contact?> GetByIdAsync(int id);
    Task<Contact> CreateAsync(Contact contact);
    Task<Contact> UpdateAsync(Contact contact);
    Task DeleteAsync(int id);
}