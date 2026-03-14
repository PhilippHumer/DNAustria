using DNAustria.Domain.Entities;

namespace DNAustria.Application.Interfaces;

public interface IOrganizationRepository
{
    Task<IEnumerable<Organization>> GetAllAsync(string? nameFilter = null);
    Task<Organization?> GetByIdAsync(Guid id);
    Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null);
    Task<Organization> CreateAsync(Organization organization);
    Task<Organization> UpdateAsync(Organization organization);
    Task DeleteAsync(Organization organization);
}
