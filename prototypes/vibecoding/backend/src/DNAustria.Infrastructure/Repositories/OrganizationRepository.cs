using DNAustria.Application.Interfaces;
using DNAustria.Domain.Entities;
using DNAustria.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Infrastructure.Repositories;

public class OrganizationRepository(AppDbContext context) : IOrganizationRepository
{
    public async Task<IEnumerable<Organization>> GetAllAsync(string? nameFilter = null)
    {
        var query = context.Organizations.Where(o => !o.IsDeleted);

        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            var filter = $"%{nameFilter.Trim()}%";
            query = query.Where(o => EF.Functions.ILike(o.Name, filter));
        }

        return await query.ToListAsync();
    }

    public async Task<Organization?> GetByIdAsync(Guid id)
    {
        return await context.Organizations
            .Where(o => o.Id == id && !o.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null)
    {
        var query = context.Organizations
            .Where(o => !o.IsDeleted && EF.Functions.ILike(o.Name, name));

        if (excludeId.HasValue)
            query = query.Where(o => o.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<Organization> CreateAsync(Organization organization)
    {
        context.Organizations.Add(organization);
        await context.SaveChangesAsync();
        return organization;
    }

    public async Task<Organization> UpdateAsync(Organization organization)
    {
        context.Organizations.Update(organization);
        await context.SaveChangesAsync();
        return organization;
    }

    public async Task DeleteAsync(Organization organization)
    {
        context.Organizations.Update(organization);
        await context.SaveChangesAsync();
    }
}
