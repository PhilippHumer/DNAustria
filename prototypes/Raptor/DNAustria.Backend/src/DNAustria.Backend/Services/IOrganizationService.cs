using DNAustria.Backend.Models;

namespace DNAustria.Backend.Services;

public interface IOrganizationService
{
    Task<IEnumerable<Organization>> GetAllAsync();
    Task<Organization?> GetAsync(Guid id);
    Task<Organization> CreateAsync(Organization o);
    Task<Organization?> UpdateAsync(Guid id, Organization o);
    Task<bool> DeleteAsync(Guid id);
}