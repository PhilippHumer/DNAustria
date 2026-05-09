using DNAustria.Domain.Entities;

namespace DNAustria.Application.Interfaces;

public interface IContactRepository
{
    Task<List<Contact>> GetAllAsync(string? nameFilter, CancellationToken ct = default);
    Task<Contact?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken ct = default);
    Task AddAsync(Contact contact, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
