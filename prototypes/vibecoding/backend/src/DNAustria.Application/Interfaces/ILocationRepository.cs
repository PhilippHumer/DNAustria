using DNAustria.Domain.Entities;

namespace DNAustria.Application.Interfaces;

public interface ILocationRepository
{
    Task<List<Location>> GetAllAsync(CancellationToken ct = default);
    Task<Location?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Location location, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
