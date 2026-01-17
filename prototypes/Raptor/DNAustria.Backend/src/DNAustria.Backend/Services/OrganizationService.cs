using DNAustria.Backend.Data;
using DNAustria.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Backend.Services;

public class OrganizationService : IOrganizationService
{
    private readonly AppDbContext _db;
    public OrganizationService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Organization>> GetAllAsync() => await _db.Organizations.ToListAsync();

    public async Task<Organization?> GetAsync(Guid id) => await _db.Organizations.FindAsync(id);

    public async Task<Organization> CreateAsync(Organization o)
    {
        o.Id = Guid.NewGuid();
        _db.Organizations.Add(o);
        await _db.SaveChangesAsync();
        return o;
    }

    public async Task<Organization?> UpdateAsync(Guid id, Organization o)
    {
        var ex = await _db.Organizations.FindAsync(id);
        if (ex is null) return null;
        ex.Name = o.Name;
        ex.Address = o.Address;
        ex.City = o.City;
        ex.Zip = o.Zip;
        ex.State = o.State;
        ex.Street = o.Street;
        ex.Latitude = o.Latitude;
        ex.Longitude = o.Longitude;
        await _db.SaveChangesAsync();
        return ex;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var ex = await _db.Organizations.FindAsync(id);
        if (ex is null) return false;
        _db.Organizations.Remove(ex);
        await _db.SaveChangesAsync();
        return true;
    }
}