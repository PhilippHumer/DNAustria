using DNAustria.Application.Interfaces;
using DNAustria.Domain.Entities;
using DNAustria.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _context;

    public ContactRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Contact>> GetAllAsync(string? nameFilter, CancellationToken ct = default)
    {
        var query = _context.Contacts.Where(c => !c.IsDeleted);
        if (!string.IsNullOrEmpty(nameFilter))
            query = query.Where(c => c.Name.ToLower().Contains(nameFilter.ToLower()));
        return query.ToListAsync(ct);
    }

    public Task<Contact?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _context.Contacts.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, ct);

    public Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken ct = default)
    {
        var query = _context.Contacts
            .Where(c => !c.IsDeleted && c.Name.ToLower() == name.ToLower());
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
        return query.AnyAsync(ct);
    }

    public async Task AddAsync(Contact contact, CancellationToken ct = default)
        => await _context.Contacts.AddAsync(contact, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
