using DNAustria.Domain.Entities;

namespace DNAustria.Application.Interfaces;

public interface IOrganizationRepository
{
    Task<List<Organization>> GetAllAsync(string? nameFilter, CancellationToken ct = default);
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken ct = default);
    Task AddAsync(Organization organization, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
