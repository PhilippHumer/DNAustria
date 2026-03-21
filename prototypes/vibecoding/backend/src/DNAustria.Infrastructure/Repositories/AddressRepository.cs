using DNAustria.Application.Interfaces;
using DNAustria.Domain.Entities;
using DNAustria.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Infrastructure.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly AppDbContext _context;

    public AddressRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Address>> GetAllAsync(CancellationToken ct = default)
        => _context.Addresses.Where(a => !a.IsDeleted).ToListAsync(ct);

    public Task<Address?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, ct);

    public Task<Address?> FindActiveDuplicateAsync(string zip, decimal latitude, decimal longitude, CancellationToken ct = default)
        => _context.Addresses.FirstOrDefaultAsync(
            a => !a.IsDeleted && a.Zip == zip && a.Latitude == latitude && a.Longitude == longitude, ct);

    public Task<bool> ExistsAnotherWithDedupKeyAsync(string zip, decimal latitude, decimal longitude, Guid excludeId, CancellationToken ct = default)
        => _context.Addresses.AnyAsync(
            a => !a.IsDeleted && a.Id != excludeId && a.Zip == zip && a.Latitude == latitude && a.Longitude == longitude, ct);

    public Task<bool> HasActiveLocationsAsync(Guid addressId, CancellationToken ct = default)
        => _context.Locations.AnyAsync(l => !l.IsDeleted && l.AddressId == addressId, ct);

    public async Task AddAsync(Address address, CancellationToken ct = default)
        => await _context.Addresses.AddAsync(address, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
