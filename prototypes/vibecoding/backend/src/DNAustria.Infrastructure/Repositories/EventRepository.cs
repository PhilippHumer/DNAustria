using DNAustria.Application.Interfaces;
using DNAustria.Domain.Entities;
using DNAustria.Domain.Enums;
using DNAustria.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly AppDbContext _context;

    public EventRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Event>> GetAllAsync(string? titleFilter, CancellationToken ct = default)
    {
        var query = _context.Events.Where(e => !e.IsDeleted);
        if (!string.IsNullOrEmpty(titleFilter))
            query = query.Where(e => e.Title.ToLower().Contains(titleFilter.ToLower()));
        return query.ToListAsync(ct);
    }

    public Task<List<Event>> GetPublicAsync(CancellationToken ct = default)
        => _context.Events
            .Where(e => !e.IsDeleted && e.Status == EventStatus.Approved)
            .ToListAsync(ct);

    public Task<Event?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _context.Events.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);

    public async Task AddAsync(Event ev, CancellationToken ct = default)
        => await _context.Events.AddAsync(ev, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
