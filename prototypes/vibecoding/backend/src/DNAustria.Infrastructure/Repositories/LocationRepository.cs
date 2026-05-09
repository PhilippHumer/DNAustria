using DNAustria.Application.Interfaces;
using DNAustria.Domain.Entities;
using DNAustria.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly AppDbContext _context;

    public LocationRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Location>> GetAllAsync(CancellationToken ct = default)
        => _context.Locations
            .Where(l => !l.IsDeleted)
            .Include(l => l.Address)
            .ToListAsync(ct);

    public Task<Location?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _context.Locations
            .Include(l => l.Address)
            .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, ct);

    public async Task AddAsync(Location location, CancellationToken ct = default)
        => await _context.Locations.AddAsync(location, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
