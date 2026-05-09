using DNAustria.Domain.Entities;

namespace DNAustria.Application.Interfaces;

public interface IEventRepository
{
    Task<List<Event>> GetAllAsync(string? titleFilter, CancellationToken ct = default);
    Task<List<Event>> GetPublicAsync(CancellationToken ct = default);
    Task<Event?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Event ev, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
