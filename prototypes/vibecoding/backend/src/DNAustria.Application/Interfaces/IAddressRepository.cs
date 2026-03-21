using DNAustria.Domain.Entities;

namespace DNAustria.Application.Interfaces;

public interface IAddressRepository
{
    Task<List<Address>> GetAllAsync(CancellationToken ct = default);
    Task<Address?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Address?> FindActiveDuplicateAsync(string zip, decimal latitude, decimal longitude, CancellationToken ct = default);
    Task<bool> ExistsAnotherWithDedupKeyAsync(string zip, decimal latitude, decimal longitude, Guid excludeId, CancellationToken ct = default);
    Task<bool> HasActiveLocationsAsync(Guid addressId, CancellationToken ct = default);
    Task AddAsync(Address address, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
