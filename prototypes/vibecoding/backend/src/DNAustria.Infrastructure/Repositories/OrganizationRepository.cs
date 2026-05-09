using DNAustria.Application.Interfaces;
using DNAustria.Domain.Entities;
using DNAustria.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Infrastructure.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly AppDbContext _context;

    public OrganizationRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Organization>> GetAllAsync(string? nameFilter, CancellationToken ct = default)
    {
        var query = _context.Organizations.Where(o => !o.IsDeleted);
        if (!string.IsNullOrEmpty(nameFilter))
            query = query.Where(o => o.Name.ToLower().Contains(nameFilter.ToLower()));
        return query.ToListAsync(ct);
    }

    public Task<Organization?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _context.Organizations.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted, ct);

    public Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken ct = default)
    {
        var query = _context.Organizations
            .Where(o => !o.IsDeleted && o.Name.ToLower() == name.ToLower());
        if (excludeId.HasValue)
            query = query.Where(o => o.Id != excludeId.Value);
        return query.AnyAsync(ct);
    }

    public async Task AddAsync(Organization organization, CancellationToken ct = default)
        => await _context.Organizations.AddAsync(organization, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
